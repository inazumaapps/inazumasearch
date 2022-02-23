using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace InazumaSearch.Core.Crawl
{
    /// <summary>
    /// クロール時に実行する1つ1つの処理の基底クラス
    /// </summary>
    public abstract class WorkBase : IWork
    {
        protected Application _app;

        /// <summary>
        /// ロガー
        /// </summary>
        public virtual NLog.Logger Logger { get { return _app.Crawler.Logger; } }

        /// <summary>
        /// 常駐クロールによる処理かどうか
        /// </summary>
        public virtual bool IsBackgroundCrawl { get; set; }

        /// <summary>
        /// 処理の実行開始日時
        /// </summary>
        protected virtual DateTime? ExecutionStart { get; set; }

        /// <summary>
        /// クールタイムの最終完了日時（常駐クロール時のみ有効）
        /// </summary>
        protected virtual DateTime? LastCoolTimeEnd { get; set; }

        public WorkBase(Application app, bool isBackgroundCrawl)
        {
            _app = app;
            IsBackgroundCrawl = isBackgroundCrawl;
        }

        /// <summary>
        /// 処理実行
        /// </summary>
        public void Execute(
            Stack<IWork> workStack,
            Result crawlResult,
            CancellationToken cToken,
            IProgress<ProgressState> progress = null
        )
        {
            ExecutionStart = DateTime.Now;
            ExecuteMain(workStack, crawlResult, cToken, progress);
            ExecutionStart = null;
        }

        /// <summary>
        /// 処理実行
        /// </summary>
        public abstract void ExecuteMain(
            Stack<IWork> workStack,
            Result crawlResult,
            CancellationToken cToken,
            IProgress<ProgressState> progress = null
        );

        /// <summary>
        /// 等価比較 (Objectメソッドのオーバーライド)
        /// </summary>
        public override bool Equals(object obj)
        {
            if (obj is IWork)
            {
                return Equals((IWork)obj);
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 等価比較
        /// </summary>
        public abstract bool Equals(IWork other);

        /// <summary>
        /// ハッシュ値算出
        /// </summary>
        /// <remarks>重複チェックのために必要です。</remarks>
        public override int GetHashCode()
        {
            // デフォルトではObject.GetHashCode() と同じ動作
            return base.GetHashCode();
        }

        /// <summary>
        /// トレースログ出力時の表記
        /// </summary>
        public virtual string TraceLogCaption { get { return GetType().Name; } }

        /// <summary>
        /// クールタイムを挟む
        /// </summary>
        protected virtual void InsertCoolTime(int coolTimeMin)
        {
            // 常駐クロールの場合はクールタイムを挟む (処理の経過時間×3, 最低でもcoolTimeMin分のクールタイムを取る)
            if (IsBackgroundCrawl)
            {
                var start = (LastCoolTimeEnd ?? ExecutionStart);
                var coolTime = (DateTime.Now - start.Value).Milliseconds * 3;
                if (coolTime < coolTimeMin) coolTime = coolTimeMin;

                //Logger.Trace("Cooltime - {0}ms", coolTime);
                Thread.Sleep(coolTime);

                LastCoolTimeEnd = DateTime.Now;
            }
        }

        /// <summary>
        /// Groonga内に登録された対象文書のキー, 更新日付, サイズを取得し、それをDictionaryに格納して返す
        /// </summary>
        /// <param name="targetDirPaths">検索対象のフォルダ一覧（省略時やnull時は全検索対象フォルダを検索）</param>
        /// <returns>対象ファイルの一覧</returns>
        protected virtual IDictionary<string, Groonga.RecordSet.Record> DBDocumentRecordListUp(
              IEnumerable<string> targetDirPaths = null
        )
        {
            string filter = null;

            // 対象フォルダが指定されている場合、下記いずれかの条件を満たす文書のみを検索対象とする
            // (1) 指定された対象フォルダのいずれかに含まれている
            // (2) 設定に登録されている検索対象フォルダのどれにも含まれていない（＝検索対象フォルダが削除され、文書が残った）
            if (targetDirPaths != null)
            {
                var targetKeyPrefixes1 = targetDirPaths.Select(dir => Util.MakeDocumentDirKeyPrefix(dir));
                var exprs1 = targetKeyPrefixes1.Select(key => $"{Column.Documents.KEY} @^ {Groonga.Util.EscapeForScript(key)}");

                var targetKeyPrefixes2 = _app.UserSettings.TargetFolders.Select(folder => Util.MakeDocumentDirKeyPrefix(folder.Path));
                var exprs2 = targetKeyPrefixes2.Select(key => $"!({Column.Documents.KEY} @^ {Groonga.Util.EscapeForScript(key)})");

                filter = $"({string.Join(" || ", exprs1)}) || ({string.Join(" && ", exprs2)})";
            }

            var alreadyRecords = _app.GM.Select(
                  Table.Documents
                , outputColumns: new[] {
                          Column.Documents.KEY
                        , Column.Documents.FILE_UPDATED_AT
                        , Column.Documents.SIZE
                        , Column.Documents.FILE_PATH
                        , Column.Documents.APPLICATION_MAJOR_VERSION_ON_UPDATED
                        , Column.Documents.APPLICATION_MINOR_VERSION_ON_UPDATED
                }
                , limit: -1 // 全件
                , filter: filter
            );
            var alreadyRecordMap = new Dictionary<string, Groonga.RecordSet.Record>();
            foreach (var rec in alreadyRecords.SearchResult.Records)
            {
                alreadyRecordMap.Add((string)rec[Column.Documents.KEY], rec);
            }

            // 常駐クロールの場合はクールタイムを挟む (処理の経過時間×3, 最低0.05秒)
            InsertCoolTime(50);

            return alreadyRecordMap;
        }


        /// <summary>
        /// 進捗状況を報告。ただし一定の頻度以上で報告しないようにする
        /// </summary>
        protected virtual void ReportProgressLimitedFrequency(IProgress<ProgressState> progress, ProgressState reportingState, Result result)
        {
            if (progress == null) return;

            // 初回報告、もしくは最終報告時間から0.03秒以上経った場合のみ処理
            if (result.LastProgressReported == null || (DateTime.Now - result.LastProgressReported.Value).Milliseconds >= 30)
            {
                progress?.Report(reportingState);
                result.LastProgressReported = DateTime.Now;
            }
        }

        /// <summary>
        /// フォルダ内直下のファイルすべてに対して処理 (ファイル一覧の取得時や、ファイルへの処理時に例外が発生した場合は無視)
        /// </summary>
        /// <exception cref="OperationCanceledException">処理中にユーザーによって操作がキャンセルされた場合に発生（この例外のみ無視せず、外部にthrowする）</exception>
        protected virtual void ApplyFiles(string dirPath, Action<string> fileAction)
        {
            Util.ApplyFiles(Logger, dirPath, fileAction);
        }

        /// <summary>
        /// 直下の子ディレクトリすべてに対して処理 (フォルダ一覧の取得時や、フォルダへの処理時に例外が発生した場合は無視)
        /// </summary>
        /// <exception cref="OperationCanceledException">処理中にユーザーによって操作がキャンセルされた場合に発生（この例外のみ無視せず、外部にthrowする）</exception>
        protected virtual void ApplySubDirectories(string dirPath, Action<string> dirAction)
        {
            Util.ApplySubDirectories(Logger, dirPath, dirAction);
        }

        /// <summary>
        /// 子ディレクトリすべてに対して処理 (サブディレクトリを再帰的に検索し、例外が発生したファイルは無視する)
        /// </summary>
        /// <exception cref="OperationCanceledException">処理中にユーザーによって操作がキャンセルされた場合に発生（この例外のみ無視せず、外部にthrowする）</exception>
        protected virtual void ApplyAllDirectories(string dirPath, Action<string> dirAction)
        {
            Util.ApplyAllDirectories(Logger, dirPath, dirAction);
        }

        /// <summary>
        /// 子ファイルすべてに対して処理 (サブディレクトリを再帰的に検索し、例外が発生したファイルは無視する)
        /// </summary>
        /// <exception cref="OperationCanceledException">処理中にユーザーによって操作がキャンセルされた場合に発生（この例外のみ無視せず、外部にthrowする）</exception>
        protected virtual void ApplyAllFiles(string dirPath, Action<string> fileAction)
        {
            Util.ApplyAllFiles(Logger, dirPath, fileAction);
        }
    }
}
