using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Alphaleonis.Win32.Filesystem;
using InazumaSearch.Core.Crawl;

namespace InazumaSearch.Core.Crawl.Work
{
    /// <summary>
    /// DB内に存在する、指定フォルダ以下の全文書データを削除
    /// </summary>
    public class DBDirectoryDelete : WorkBase
    {
        /// <summary>
        /// 削除対象のフォルダパス
        /// </summary>
        public virtual string DirPath { get; protected set; }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public DBDirectoryDelete(
            Application app,
            bool isBackgroundCrawl,
            string dirPath
        ) : base(app, isBackgroundCrawl)
        {
            DirPath = dirPath;
        }

        /// <summary>
        /// 等価比較
        /// </summary>
        /// <remarks>重複チェックのために必要です。</remarks>
        public override bool Equals(IWork other)
        {
            var work = other as DBDirectoryDelete;
            return (work != null && work.DirPath == DirPath);
        }

        /// <summary>
        /// ハッシュ値算出
        /// </summary>
        /// <remarks>重複チェックのために必要です。</remarks>
        public override int GetHashCode()
        {
            return GetType().GetHashCode() ^ DirPath.GetHashCode();
        }

        /// <summary>
        /// トレースログ出力時の表記
        /// </summary>
        public override string TraceLogCaption { get { return $"{GetType().Name}({DirPath})"; } }

        /// <summary>
        /// 処理実行
        /// </summary>
        public override void ExecuteMain(
            Stack<IWork> workStack,
            Result crawlResult,
            CancellationToken cToken,
            IProgress<CrawlState> progress = null
        )
        {
            // 削除直前に、フォルダが存在しないことを再確認 (対象パスの特定から削除の実行までに時間差があるため)
            if (!Directory.Exists(DirPath))
            {
                Logger.Info("フォルダ内の文書ファイルデータ全削除 - {0}", DirPath);

                // 進捗を報告
                ReportProgressLimitedFrequency(
                    progress,
                    new CrawlState() { CurrentStep = CrawlState.Step.AlwaysCrawlDBDirectoryDeleteBegin, Path = DirPath },
                    crawlResult
                );

                // 文書データを全削除
                var filter = $"{Column.Documents.KEY} @^ {Groonga.Util.EscapeForScript(Util.MakeDocumentDirKeyPrefix(DirPath))}";
                _app.GM.Delete(Table.Documents, filter: filter);

                // 常駐クロールの場合はクールタイムを挟む (処理の経過時間×3, 最低0.5秒)
                InsertCoolTime(500);
            }
        }
    }
}
