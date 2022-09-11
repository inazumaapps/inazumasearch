using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace InazumaSearch.Core.Crawl
{
    public partial class Crawler
    {
        /// <summary>
        /// クロール状態をカプセル化したクラス
        /// </summary>
        public class CrawlState : IDisposable
        {
            public virtual Task Task { get; protected set; }
            public virtual CancellationTokenSource CancellationTokenSource { get; protected set; }

            /// <summary>
            /// タスクがまだ実行中かどうか
            /// </summary>
            public virtual bool Running { get { return Task.Status == TaskStatus.Running; } }

            /// <summary>
            /// コンストラクタ
            /// </summary>
            public CrawlState(Task crawlingTask, CancellationTokenSource ctSource)
            {
                Task = crawlingTask;
                CancellationTokenSource = ctSource;
            }

            /// <summary>
            /// リソースの解放
            /// </summary>
            public void Dispose()
            {
                Task.Dispose();
                CancellationTokenSource.Dispose();
            }
        }

        /// <summary>
        /// ログ出力用オブジェクト
        /// </summary>
        public NLog.Logger Logger { get; protected set; }

        public Core.Application App { get; set; }

        /// <summary>
        /// 常駐クロール時の進捗報告用オブジェクト
        /// </summary>
        public virtual Progress<ProgressState> AlwaysCrawlProgress { get; set; } = new Progress<ProgressState>();

        /// <summary>
        /// 手動クロールが実行中かどうか
        /// </summary>
        public virtual bool ManualCrawlIsRunning { get { return ManualCrawlState != null && ManualCrawlState.Running; } }

        /// <summary>
        /// 常駐クロールが実行中かどうか
        /// </summary>
        public virtual bool AlwaysCrawlIsRunning { get { return AlwaysCrawlState != null && AlwaysCrawlState.Running; } }

        /// <summary>
        /// 手動クロールの処理状態を表すオブジェクト（一度も実行していない場合はnull）
        /// </summary>
        protected virtual CrawlState ManualCrawlState { get; set; } = null;

        /// <summary>
        /// 常駐クロールの処理状態を表すオブジェクト（一度も実行していない場合はnull）
        /// </summary>
        protected virtual CrawlState AlwaysCrawlState { get; set; } = null;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public Crawler(Application app)
        {
            Logger = NLog.LogManager.GetLogger(LoggerName.Crawler);

            App = app;
        }

        /// <summary>
        /// 非同期に全体クロール処理の実行を開始する
        /// </summary>
        public virtual async Task<Result> RunFullCrawlAsync(
            IEnumerable<string> targetDirPaths
            , EventHandler<ProgressState> progressChangedCallback = null)
        {
            if (ManualCrawlIsRunning) throw new InvalidOperationException("すでに手動クロール処理が実行中です。");
            if (AlwaysCrawlIsRunning) throw new InvalidOperationException("手動クロールと常駐クロールを同時に実行しようとしました。");

            // 進捗報告用オブジェクトの作成
            var progress = new Progress<ProgressState>();
            if (progressChangedCallback != null) progress.ProgressChanged += progressChangedCallback;

            // 処理取り消しを可能とするためにCancellationTokenSourceを生成
            var ctSource = new CancellationTokenSource();

            // クロール結果オブジェクトを生成
            var res = new Result() { Finished = false };

            // 手動クロールのメイン処理（別スレッドで実行）
            var t = Task.Run(() =>
            {
                Logger.Info("手動クロール開始");

                // 動作ログ出力
                OperationLog.Add(OperationLog.LogType.ManualCrawlStart);

                try
                {
                    // (1) 登録ファイル数の計測 (別スレッドで並列実行)
                    //     ※内部で例外が発生した場合には、ログのみ出力して処理を中断するため注意
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
                        workStack.Push(new Work.FullCrawl(App, isBackgroundCrawl: false, targetDirPaths: targetDirPaths));

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
                    // 動作ログ出力
                    OperationLog.Add(OperationLog.LogType.ManualCrawlCancel);

                    return;
                }

                // 動作ログ出力
                var totalFileCountCaption = res.TotalTargetCount.HasValue ? res.TotalTargetCount.Value.ToString() : "不明";
                OperationLog.Add(OperationLog.LogType.ManualCrawlFinish, additionalMessage: $"登録対象の総ファイル件数={totalFileCountCaption},更新件数={res.Updated},スキップ件数{res.Skipped},削除件数={res.Deleted}");

                // 手動クロール処理完了を通知
                ((IProgress<ProgressState>)progress).Report(new ProgressState() { CurrentStep = ProgressState.Step.Finish });
                res.Finished = true;

            }, ctSource.Token);

            // 実行状態を設定
            ManualCrawlState = new CrawlState(t, ctSource);

            // タスク完了／キャンセルを待機
            await t;

            // タスクが完了／キャンセル受付したら結果を戻す
            return res;
        }

        /// <summary>
        /// 常駐クロール処理を開始する（処理は別スレッドで非同期に進行）
        /// </summary>
        public virtual void StartAlwaysCrawl()
        {
            if (AlwaysCrawlIsRunning) throw new InvalidOperationException("すでに常駐クロール処理が実行中です。");
            if (ManualCrawlIsRunning) throw new InvalidOperationException("手動クロールと常駐クロールを同時に実行しようとしました。");

            // キャンセル用のオブジェクトを生成
            var ctSource = new CancellationTokenSource();
            var res = new Result() { Finished = false };

            // 常駐クロールのメイン処理 (別スレッドで実行)
            var t = Task.Factory.StartNew(() =>
            {
                Logger.Info("常駐クロール開始");

                // 開始を報告
                ((IProgress<ProgressState>)AlwaysCrawlProgress).Report(new ProgressState() { CurrentStep = ProgressState.Step.AlwaysCrawlBegin });

                // ファイル監視オブジェクトを生成
                var watchers = new List<System.IO.FileSystemWatcher>();

                // ワークスタックを初期化
                var workStack = new Stack<IWork>();
                var workStackSet = new HashSet<IWork>(); // 処理の重複を確認するためのセット

                // 最初にフルクロールを実行
                workStack.Push(new Work.FullCrawl(App, isBackgroundCrawl: true));

                // ファイル監視オブジェクト生成
                var fileWatcher = new FileWatcher(Logger, App.UserSettings.TargetFolders.Select(f => f.Path).OrderBy(p => p).ToList());

                // メイン処理
                try
                {
                    // 監視スタート
                    fileWatcher.Start();

                    // 無視設定を取得
                    var ignoreSettings = App.GetIgnoreSettings();

                    while (true)
                    {
                        // ファイル変更イベントがあれば、対応するワークを生成
                        if (fileWatcher.EventQueue.Count >= 1)
                        {
                            while (fileWatcher.EventQueue.Count >= 1)
                            {
                                // キューの先頭にあるイベントを取得
                                var evt = fileWatcher.EventQueue[0];
                                fileWatcher.EventQueue.RemoveAt(0);

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
                        if (workStack.Count >= 1)
                        {
                            var nextWork = workStack.Pop();
                            nextWork.Execute(workStack, res, ctSource.Token, AlwaysCrawlProgress);
                            ctSource.Token.ThrowIfCancellationRequested(); // キャンセル受付
                        }
                        else
                        {
                            // ワークが1つも無い状態であれば、進捗を報告した上で、5秒待つ (1秒ごとにキャンセル受け付け)
                            ((IProgress<ProgressState>)AlwaysCrawlProgress).Report(new ProgressState() { CurrentStep = ProgressState.Step.Finish });
                            for (var i = 0; i < 5; i++)
                            {
                                Thread.Sleep(1000);
                                ctSource.Token.ThrowIfCancellationRequested(); // キャンセル受付
                            }

                            // ファイル監視オブジェクトを再起動
                            fileWatcher.Restart();
                        }
                    }
                }
                catch (OperationCanceledException)
                {
                    // ファイル監視オブジェクトをすべて解放
                    fileWatcher.Stop();
                    Logger.Info("常駐クロール終了");

                    // 終了を報告
                    ((IProgress<ProgressState>)AlwaysCrawlProgress).Report(new ProgressState() { CurrentStep = ProgressState.Step.AlwaysCrawlEnd });

                    return;
                }
                catch (Exception ex)
                {
                    Logger.Error("常駐クロール処理でエラーが発生しました");
                    Logger.Error(ex.ToString());
                }
            }, ctSource.Token, TaskCreationOptions.LongRunning, TaskScheduler.Default);

            // 実行状態を設定
            AlwaysCrawlState = new CrawlState(t, ctSource);
        }

        /// <summary>
        /// 手動クロール処理が実行中であれば停止（実行中でなければ何もしない）
        /// </summary>
        public virtual async Task StopManualCrawlIfRunningAsync()
        {
            if (ManualCrawlIsRunning)
            {
                // キャンセルをリクエスト
                ManualCrawlState.CancellationTokenSource.Cancel();

                // キャンセルが完了するまで待機
                await ManualCrawlState.Task;
            }
        }


        /// <summary>
        /// 常駐クロール処理が実行中であれば停止（実行中でなければ何もしない）
        /// </summary>
        public virtual async Task StopAlwaysCrawlIfRunningAsync()
        {
            if (AlwaysCrawlIsRunning)
            {
                // キャンセルをリクエスト
                AlwaysCrawlState.CancellationTokenSource.Cancel();

                // キャンセルが完了するまで待機
                await AlwaysCrawlState.Task;
            }
        }
    }
}
