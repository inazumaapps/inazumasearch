using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Alphaleonis.Win32.Filesystem;
using Newtonsoft.Json;

namespace InazumaSearch.Core
{
    public partial class Crawler
    {
        public class Result
        {
            public bool Finished { get; set; }
            public long Updated { get; set; }
            public long Skipped { get; set; }
            public long Deleted { get; set; }
            public IList<Error> Errors { get; set; }

            public Result()
            {
                Errors = new List<Error>();
            }
        }

        public class Error
        {
            public string Path { get; set; }
            public string Title { get; set; }
        }

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
        /// 常駐クロール時に予約されている作業
        /// </summary>
        protected virtual IList<IWork> WorkQueue { get; set; }

        /// <summary>
        /// 常駐クロール時に予約されている作業（重複判定用）
        /// </summary>
        protected virtual ISet<IWork> WorkQueueSet { get; set; }

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
            WorkQueue = new List<IWork>();
            WorkQueueSet = new HashSet<IWork>();
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

            var progress = new Progress<CrawlState>();
            if (progressChangedCallback != null) progress.ProgressChanged += progressChangedCallback;

            Running = true;

            var ctSource = new CancellationTokenSource();
            CancellationTokenSource = ctSource;

            var res = new Result() { Finished = false };

            // 全体クロールを実行
            var work = new Crawler.Work.FullCrawl(App, targetDirPaths: targetDirPaths);
            await Task.Run(() =>
            {
                try
                {
                    work.Execute(progress, ctSource.Token, res);
                }
                catch (OperationCanceledException)
                {
                    return;
                }

                Running = false;
                res.Finished = true;

            }, ctSource.Token);

            return res;
        }

        /// <summary>
        /// 非同期に常駐クロール処理を開始する
        /// </summary>
        /// <param name="progressChangedCallback"></param>
        /// <returns></returns>
        public virtual async Task RunAlwaysModeAsync(EventHandler<CrawlState> progressChangedCallback = null)
        {
            if (Running) throw new InvalidOperationException("Crawlerはすでに起動しています。");

            var progress = new Progress<CrawlState>();
            if (progressChangedCallback != null) progress.ProgressChanged += progressChangedCallback;

            Running = true;

            var ctSource = new CancellationTokenSource();
            CancellationTokenSource = ctSource;
            var res = new Result() { Finished = false };

            // ワークキューに全体クロール処理を設定
            var fullCrawl = new Work.FullCrawl(App, alwaysCrawlMode: true);
            WorkQueue.Clear();
            WorkQueue.Add(fullCrawl);
            WorkQueueSet.Add(fullCrawl);

            // 常駐タスク実行
            await Task.Factory.StartNew(() =>
            {

                // ファイル監視オブジェクトを生成
                var watchers = new List<System.IO.FileSystemWatcher>();
                Logger.Info("常駐クロール開始");
                try
                {
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


                    while (true)
                    {
                        // ファイル変更イベントがあれば、対応するワークを生成
                        if (FileSystemEventQueue.Count >= 1)
                        {
                            lock (FileSystemEventQueue)
                            {
                                lock (WorkQueue)
                                {
                                    while (FileSystemEventQueue.Count >= 1)
                                    {
                                        // キューの先頭にあるイベントを取得
                                        var evt = FileSystemEventQueue[0];
                                        FileSystemEventQueue.RemoveAt(0);

                                        Logger.Trace("FileSystemEvent: {0} {1} {2}", evt.Type, evt.TargetType, evt.Path);

                                        // イベントの種類に応じたワークを生成
                                        IWork work = null;
                                        if (evt.Type == FileSystemEventType.UPDATE)
                                        {
                                            if (evt.TargetType == FileSystemEventTargetType.FILE)
                                            {
                                                // ファイルが更新された場合、親ディレクトリを登録対象とする
                                                work = new Work.DocumentDirectoryCrawl(App, Path.GetDirectoryName(evt.Path));
                                            }
                                            else if (evt.TargetType == FileSystemEventTargetType.FOLDER)
                                            {
                                                work = new Work.DocumentDirectoryCrawl(App, evt.Path);
                                            }
                                        }
                                        else if (evt.Type == FileSystemEventType.DELETE)
                                        {
                                            if (evt.TargetType == FileSystemEventTargetType.FILE)
                                            {
                                                work = new Work.DocumentFileDelete(App, evt.Path);

                                            }
                                            else if (evt.TargetType == FileSystemEventTargetType.FOLDER)
                                            {
                                                work = new Work.DocumentDirectoryDelete(App, evt.Path);

                                            }

                                        }

                                        // 処理対象外イベントならスキップ
                                        if (work == null) continue;

                                        // キューに登録
                                        if (WorkQueueSet.Contains(work))
                                        {
                                            // 既存ワークと重複していれば、いったん削除してから再登録し、後で実行されるようにする
                                            WorkQueue.Remove(work);
                                            WorkQueueSet.Remove(work);
                                            WorkQueue.Add(work);
                                            WorkQueueSet.Add(work);
                                            Logger.Debug("Work Queue Re-added: {0}", work.LogCaption);
                                        }
                                        else
                                        {
                                            // 既存ワークと重複していなければ、そのまま登録する
                                            WorkQueue.Add(work);
                                            WorkQueueSet.Add(work);
                                            Logger.Debug("Work Queue Added: {0}", work.LogCaption);
                                        }
                                        Logger.Debug("Work Queue ({0}): [{1}]", WorkQueue.Count, string.Join(", ", WorkQueue.Select(w => w.LogCaption)));
                                    }
                                }
                            }
                        }

                        // ワークキューからワークを1つずつ取得
                        IWork nextWork = null;
                        lock (WorkQueue)
                        {
                            if (WorkQueue.Count >= 1)
                            {
                                nextWork = WorkQueue[0];
                                WorkQueue.RemoveAt(0);
                                WorkQueueSet.Remove(nextWork); // 重複チェック用セットからも削除

                                Logger.Debug("Dequeued the next work: {0}", nextWork.LogCaption);
                                Logger.Debug("Work Queue ({0}): [{1}]", WorkQueue.Count, string.Join(", ", WorkQueue.Select(w => w.LogCaption)));
                            }
                        }

                        // ワークがあれば実行
                        if (nextWork != null)
                        {
                            Logger.Info("常駐クロール内処理を実行 - {0}", nextWork.LogCaption);
                            nextWork.Execute(progress, ctSource.Token, res);
                        }

                        // キャンセルされたら中断
                        ctSource.Token.ThrowIfCancellationRequested();

                        // 5秒待機
                        Thread.Sleep(5000);

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
