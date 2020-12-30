using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Alphaleonis.Win32.Filesystem;
using Newtonsoft.Json;

namespace InazumaSearch.Core.Crawl
{
    public partial class Crawler
    {
        public class FileSystemEvent
        {
            public FileSystemEventType Type { get; set; }
            public FileSystemEventTargetType? TargetType { get; set; }
            public string Path { get; set; }
        }

        public enum FileSystemEventType
        {
            UPDATE
            , DELETE
        }

        public enum FileSystemEventTargetType
        {
            FILE
            , FOLDER
        }


        /// <summary>
        /// ログ出力用オブジェクト
        /// </summary>
        public NLog.Logger Logger { get; protected set; }

        public Core.Application App { get; set; }
        protected IProgress<CrawlState> Progress;
        protected CancellationTokenSource CancellationTokenSource;
        public bool Running { get; set; }

        /// <summary>
        /// ファイルシステムの変更イベントキュー
        /// </summary>
        protected virtual IList<FileSystemEvent> FileSystemEventQueue { get; set; }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public Crawler(Application app)
        {
            Logger = NLog.LogManager.GetLogger(LoggerName.Crawler);

            App = app;
            Running = false;
            FileSystemEventQueue = new List<FileSystemEvent>();
        }

        /// <summary>
        /// 非同期に全体クロール処理の実行を開始する
        /// </summary>
        /// <returns></returns>
        public virtual async Task<Result> RunFullCrawlAsync(
            IEnumerable<string> targetDirPaths
            , EventHandler<CrawlState> progressChangedCallback = null)
        {
            if (Running) throw new InvalidOperationException("Crawlerはすでに起動しています。");

            // 進捗報告用オブジェクトの作成
            var progress = new Progress<CrawlState>();
            if (progressChangedCallback != null) progress.ProgressChanged += progressChangedCallback;

            // 実行中フラグオン
            Running = true;

            // 処理取り消しを可能とするためにCancellationTokenSourceを生成
            var ctSource = new CancellationTokenSource();
            CancellationTokenSource = ctSource;

            // クロール結果オブジェクトを生成
            var res = new Result() { Finished = false };

            // 手動クロールを実行
            await Task.Run(() =>
            {
                Logger.Info("クロール開始");

                try
                {
                    // (1) 登録ファイル数の計測 (別スレッドで並列実行)
                    Task.Run(() =>
                    {
                        var workStack = new Stack<IWork>();
                        workStack.Push(new Work.TargetFileCount(App, isBackgroundCrawl: true, targetDirPaths: targetDirPaths));

                        // ワークスタックが空になるまで順次実行
                        while (workStack.Count >= 1)
                        {
                            var work = workStack.Pop();
                            work.Execute(workStack, res, ctSource.Token, progress);

                            Thread.Sleep(0); // 他のスレッドに処理を渡す
                            ctSource.Token.ThrowIfCancellationRequested(); // 取り消し判定
                        }

                    });

                    // (2) メインクロール処理
                    {
                        var workStack = new Stack<IWork>();
                        workStack.Push(new Work.ManualCrawl(App, isBackgroundCrawl: false, targetDirPaths: targetDirPaths));

                        // ワークスタックが空になるまで順次実行
                        while (workStack.Count >= 1)
                        {
                            var work = workStack.Pop();
                            work.Execute(workStack, res, ctSource.Token, progress);

                            Thread.Sleep(0); // 他のスレッドに処理を渡す
                            ctSource.Token.ThrowIfCancellationRequested(); // 取り消し判定
                        }
                    }
                }
                catch (OperationCanceledException)
                {
                    return;
                }

                Logger.Info("クロール完了");

                // 手動クロール処理完了を通知
                ((IProgress<CrawlState>)progress).Report(new CrawlState() { CurrentStep = CrawlState.Step.Finish });
                res.Finished = true;

                // 起動フラグオフ
                Running = false;

            }, ctSource.Token);

            return res;
        }

        /// <summary>
        /// 非同期に常駐クロール処理を開始する
        /// </summary>
        /// <param name="progressChangedCallback"></param>
        /// <returns></returns>
        public virtual async Task RunAlwaysModeAsync()
        {
            if (Running) throw new InvalidOperationException("Crawlerはすでに起動しています。");

            Running = true;

            var ctSource = new CancellationTokenSource();
            CancellationTokenSource = ctSource;
            var res = new Result() { Finished = false };

            try
            {
                // 常駐クロール処理 (別スレッドで実行)
                await Task.Factory.StartNew(() =>
                {
                    Logger.Info("常駐クロール開始");

                    // ファイル監視オブジェクトを生成
                    var watchers = new List<System.IO.FileSystemWatcher>();

                    // ワークスタックを初期化
                    var workStack = new Stack<IWork>();
                    var workStackSet = new HashSet<IWork>(); // 処理の重複を確認するためのセット

                    // 最初にフルクロールを実行
                    workStack.Push(new Work.ManualCrawl(App, isBackgroundCrawl: true));

                    // メイン処理
                    try
                    {
                        // 対象フォルダ全件について変更を監視
                        foreach (var targetFolder in App.UserSettings.TargetFolders)
                        {
                            var watcher = new System.IO.FileSystemWatcher(targetFolder.Path)
                            {
                                NotifyFilter =
                                System.IO.NotifyFilters.LastWrite |
                                System.IO.NotifyFilters.DirectoryName |
                                System.IO.NotifyFilters.FileName |
                                System.IO.NotifyFilters.Security |
                                System.IO.NotifyFilters.Size |
                                System.IO.NotifyFilters.Attributes,
                                Filter = "" // すべてのファイルが対象
                            };

                            watcher.Created += Watcher_Created;
                            watcher.Changed += Watcher_Changed;
                            watcher.Renamed += Watcher_Renamed;
                            watcher.Deleted += Watcher_Deleted;
                            watcher.EnableRaisingEvents = true;
                            watcher.IncludeSubdirectories = true;
                        }

                        // 無視設定を取得
                        var ignoreSettings = App.GetIgnoreSettings();

                        while (true)
                        {
                            // ファイル変更イベントがあれば、対応するワークを生成
                            if (FileSystemEventQueue.Count >= 1)
                            {
                                while (FileSystemEventQueue.Count >= 1)
                                {
                                    // キューの先頭にあるイベントを取得
                                    var evt = FileSystemEventQueue[0];
                                    FileSystemEventQueue.RemoveAt(0);

                                    if (Logger.IsTraceEnabled)
                                    {
                                        Logger.Trace("FileSystemEvent: {0} {1} {2}", evt.Type, evt.TargetType, evt.Path);
                                    }

                                    // イベントの種類に応じたワークを生成
                                    IWork work = null;
                                    if (evt.Type == FileSystemEventType.UPDATE)
                                    {
                                        if (evt.TargetType == FileSystemEventTargetType.FILE)
                                        {
                                            work = new Work.BackgroundFileCrawl(App, evt.Path, ignoreSettings);
                                        }
                                        else if (evt.TargetType == FileSystemEventTargetType.FOLDER)
                                        {
                                            work = new Work.BackgroundDirectoryCrawl(App, evt.Path, ignoreSettings);
                                        }
                                    }
                                    else if (evt.Type == FileSystemEventType.DELETE)
                                    {
                                        if (evt.TargetType == FileSystemEventTargetType.FILE)
                                        {
                                            work = new Work.DBDocumentDelete(App, true, evt.Path);

                                        }
                                        else if (evt.TargetType == FileSystemEventTargetType.FOLDER)
                                        {
                                            work = new Work.DBDirectoryDelete(App, true, evt.Path);
                                        }
                                    }

                                    // 処理対象外イベントならスキップ
                                    if (work == null) continue;

                                    // スタックに登録
                                    workStack.Push(work);
                                    workStackSet.Add(work);
                                    if (Logger.IsTraceEnabled)
                                    {
                                        Logger.Trace("Work Push: {0}", work.TraceLogCaption);
                                        Logger.Trace("Work Stack ({0}): [{1}]", workStack.Count, string.Join(", ", workStack.Select(w => w.TraceLogCaption)));
                                    }
                                }
                            }

                            // ワークキューにワークがあれば、1つ取得して実行
                            IWork nextWork = null;
                            if (workStack.Count >= 1)
                            {
                                nextWork = workStack.Pop();

                                //if (Logger.IsTraceEnabled)
                                //{
                                //    Logger.Trace("Poped the next work: {0}", nextWork.TraceLogCaption);
                                //    Logger.Trace("Work Stack ({0}): [{1}]", workStack.Count, string.Join(", ", workStack.Select(w => w.TraceLogCaption)));
                                //}
                                nextWork.Execute(workStack, res, ctSource.Token);
                            }
                            else
                            {
                                // ワークが1つも無い状態であれば、10秒待つ
                                Thread.Sleep(10000);
                            }

                            // キャンセルされたら中断
                            ctSource.Token.ThrowIfCancellationRequested();
                        }
                    }
                    catch (OperationCanceledException)
                    {
                        // ファイル監視オブジェクトをすべて解放
                        foreach (var watcher in watchers)
                        {
                            watcher.Dispose();
                        }
                        Logger.Info("常駐クロール終了");
                        return;
                    }
                    catch (Exception ex)
                    {
                        Logger.Error("常駐クロール処理でエラーが発生しました");
                        Logger.Error(ex.ToString());
                    }
                }, CancellationTokenSource.Token, TaskCreationOptions.LongRunning, TaskScheduler.Default);
            }
            catch (OperationCanceledException)
            {
                return;
            }
        }

        private void Watcher_Deleted(object sender, System.IO.FileSystemEventArgs e)
        {
            Logger.Trace("[FileSystemWatcher] Deleted - {0}", e.FullPath);
            var targetType = (Directory.Exists(e.FullPath) ? FileSystemEventTargetType.FOLDER : FileSystemEventTargetType.FILE);
            FileSystemEventQueue.Add(new FileSystemEvent() { Type = FileSystemEventType.DELETE, Path = e.FullPath, TargetType = targetType });
        }

        private void Watcher_Renamed(object sender, System.IO.RenamedEventArgs e)
        {
            Logger.Trace("[FileSystemWatcher] Renamed - {0} -> {1}", e.OldFullPath, e.FullPath);
            var targetType = (Directory.Exists(e.FullPath) ? FileSystemEventTargetType.FOLDER : FileSystemEventTargetType.FILE);
            FileSystemEventQueue.Add(new FileSystemEvent() { Type = FileSystemEventType.DELETE, Path = e.OldFullPath, TargetType = targetType });
            FileSystemEventQueue.Add(new FileSystemEvent() { Type = FileSystemEventType.UPDATE, Path = e.FullPath, TargetType = targetType });
        }

        private void Watcher_Changed(object sender, System.IO.FileSystemEventArgs e)
        {
            Logger.Trace("[FileSystemWatcher] Changed - {0}", e.FullPath);
            var targetType = (Directory.Exists(e.FullPath) ? FileSystemEventTargetType.FOLDER : FileSystemEventTargetType.FILE);
            FileSystemEventQueue.Add(new FileSystemEvent() { Type = FileSystemEventType.UPDATE, Path = e.FullPath, TargetType = targetType });
        }

        private void Watcher_Created(object sender, System.IO.FileSystemEventArgs e)
        {
            Logger.Trace("[FileSystemWatcher] Created - {0}", e.FullPath);
            var targetType = (Directory.Exists(e.FullPath) ? FileSystemEventTargetType.FOLDER : FileSystemEventTargetType.FILE);
            FileSystemEventQueue.Add(new FileSystemEvent() { Type = FileSystemEventType.UPDATE, Path = e.FullPath, TargetType = targetType });
        }

        public void StopIfRunning()
        {
            //if (!Running) throw new InvalidOperationException("Crawlerが起動していない状態で停止しようとしました。");

            if (Running)
            {
                CancellationTokenSource.Cancel();
                Debug.WriteLine("Canceled.");
                //Worker.CancelAsync();
                Running = false;
            }
        }


    }
}
