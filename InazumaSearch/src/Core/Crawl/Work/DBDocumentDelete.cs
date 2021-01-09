using System;
using System.Collections.Generic;
using System.Threading;
using Alphaleonis.Win32.Filesystem;

namespace InazumaSearch.Core.Crawl.Work
{
    /// <summary>
    /// DB内に存在する文書データ1件を削除
    /// </summary>
    public class DBDocumentDelete : WorkBase
    {
        /// <summary>
        /// 削除対象のファイルパス
        /// </summary>
        public virtual string FilePath { get; protected set; }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public DBDocumentDelete(
            Application app,
            bool isBackgroundCrawl,
            string filePath
        ) : base(app, isBackgroundCrawl)
        {
            FilePath = filePath;
        }

        /// <summary>
        /// 等価比較
        /// </summary>
        /// <remarks>重複チェックのために必要です。</remarks>
        public override bool Equals(IWork other)
        {
            var work = other as DBDocumentDelete;
            return (work != null && work.FilePath == FilePath);
        }

        /// <summary>
        /// ハッシュ値算出
        /// </summary>
        /// <remarks>重複チェックのために必要です。</remarks>
        public override int GetHashCode()
        {
            return GetType().GetHashCode() ^ FilePath.GetHashCode();
        }

        /// <summary>
        /// トレースログ出力時の表記
        /// </summary>
        public override string TraceLogCaption { get { return $"{GetType().Name}({FilePath})"; } }

        /// <summary>
        /// 処理実行
        /// </summary>
        public override void ExecuteMain(
            Stack<IWork> workStack,
            Result crawlResult,
            CancellationToken cToken,
            IProgress<ProgressState> progress = null
        )
        {
            // 削除直前に、ファイルが存在しないことを再確認 (対象ファイルパスの特定から削除の実行までに時間差があるため)
            if (!File.Exists(FilePath))
            {
                Logger.Info("文書ファイルデータ削除 - {0}", FilePath);

                // 進捗を報告
                ReportProgressLimitedFrequency(
                    progress,
                    new ProgressState() { CurrentStep = ProgressState.Step.AlwaysCrawlDBDocumentDeleteBegin, Path = FilePath },
                    crawlResult
                );

                // DBから対象の文書データを削除
                _app.GM.Delete(Table.Documents, key: Util.MakeDocumentFileKey(FilePath));

                // 常駐クロールの場合はクールタイムを挟む (処理の経過時間×3, 最低0.5秒)
                InsertCoolTime(500);
            }
        }
    }
}
