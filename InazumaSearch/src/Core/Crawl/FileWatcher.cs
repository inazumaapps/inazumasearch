using System.Collections.Generic;
using System.Linq;
using Alphaleonis.Win32.Filesystem;

namespace InazumaSearch.Core.Crawl
{
    /// <summary>
    /// ファイル監視を行うオブジェクト（常駐クロール時に使用）
    /// </summary>
    public class FileWatcher
    {
        /// <summary>
        /// 検索対象ディレクトリごとのファイル監視オブジェクトを格納したDictionary
        /// </summary>
        protected IDictionary<string, System.IO.FileSystemWatcher> watchers = new Dictionary<string, System.IO.FileSystemWatcher>();

        /// <summary>
        /// ファイルシステムの変更イベントキュー
        /// </summary>
        public virtual IList<FileSystemEvent> EventQueue { get; set; } = new List<FileSystemEvent>();

        protected NLog.Logger logger;
        protected IList<string> targetDirPaths;

        public FileWatcher(NLog.Logger logger, IList<string> targetDirPaths)
        {
            this.logger = logger;
            this.targetDirPaths = targetDirPaths;
        }

        /// <summary>
        /// ファイル監視を開始
        /// </summary>
        public virtual void Start()
        {
            foreach (var targetDirPath in targetDirPaths)
            {
                var watcher = new System.IO.FileSystemWatcher(targetDirPath)
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
                watcher.Error += Watcher_Error;

                watcher.EnableRaisingEvents = true;
                watcher.IncludeSubdirectories = true;

                watchers[targetDirPath] = watcher;
            }
        }

        /// <summary>
        /// ファイル監視を開始しているかどうか
        /// </summary>
        public virtual bool IsStarted()
        {
            return watchers.Any();
        }

        /// <summary>
        /// ファイル監視を再開始（すでにファイル監視を行っていれば停止してから開始する）
        /// </summary>
        public virtual void Restart()
        {
            if (IsStarted()) Stop();
            Start();
        }

        /// <summary>
        /// ファイル監視を停止
        /// </summary>
        public virtual void Stop()
        {
            // FileSystemWatcherによる監視を終了する
            foreach (var watcher in watchers.Values)
            {
                watcher.EnableRaisingEvents = false;
                watcher.Dispose();
            }

            // 監視オブジェクトをクリア
            watchers.Clear();
        }

        private void Watcher_Deleted(object sender, System.IO.FileSystemEventArgs e)
        {
            logger.Trace("[FileWatcher] Deleted - {0}", e.FullPath);
            var targetType = (Directory.Exists(e.FullPath) ? FileSystemEventTargetType.FOLDER : FileSystemEventTargetType.FILE);
            EventQueue.Add(new FileSystemEvent() { Type = FileSystemEventType.DELETE, Path = e.FullPath, TargetType = targetType });
        }

        private void Watcher_Renamed(object sender, System.IO.RenamedEventArgs e)
        {
            logger.Trace("[FileWatcher] Renamed - {0} -> {1}", e.OldFullPath, e.FullPath);
            var targetType = (Directory.Exists(e.FullPath) ? FileSystemEventTargetType.FOLDER : FileSystemEventTargetType.FILE);
            EventQueue.Add(new FileSystemEvent() { Type = FileSystemEventType.DELETE, Path = e.OldFullPath, TargetType = targetType });
            EventQueue.Add(new FileSystemEvent() { Type = FileSystemEventType.UPDATE, Path = e.FullPath, TargetType = targetType });
        }

        private void Watcher_Changed(object sender, System.IO.FileSystemEventArgs e)
        {
            logger.Trace("[FileWatcher] Changed - {0}", e.FullPath);
            var targetType = (Directory.Exists(e.FullPath) ? FileSystemEventTargetType.FOLDER : FileSystemEventTargetType.FILE);
            EventQueue.Add(new FileSystemEvent() { Type = FileSystemEventType.UPDATE, Path = e.FullPath, TargetType = targetType });
        }

        private void Watcher_Created(object sender, System.IO.FileSystemEventArgs e)
        {
            logger.Trace("[FileWatcher] Created - {0}", e.FullPath);
            var targetType = (Directory.Exists(e.FullPath) ? FileSystemEventTargetType.FOLDER : FileSystemEventTargetType.FILE);
            EventQueue.Add(new FileSystemEvent() { Type = FileSystemEventType.UPDATE, Path = e.FullPath, TargetType = targetType });
        }

        private void Watcher_Error(object sender, System.IO.ErrorEventArgs e)
        {
            logger.Warn("[FileWatcher] Error!");
            logger.Warn(e.GetException());

            // エラー発生時はFileSystemWatcherを終了（次の自動再起動を待つ）
            this.Stop();
        }
    }
}
