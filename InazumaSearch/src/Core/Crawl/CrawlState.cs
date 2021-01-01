using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InazumaSearch.Core.Crawl
{
    public class CrawlState
    {
        public enum Step
        {
            /// <summary>DBの文書データ取得開始</summary>
            DBRecordListUpBegin
            ,
            /// <summary>文書データ登録開始（クロール開始時に一度のみ実行）</summary>
            RecordUpdateBegin
            ,
            /// <summary>文書データ登録中（クロール中定期的に実行）</summary>
            RecordUpdating
            ,
            /// <summary>DB内の不要データ削除開始</summary>
            PurgeBegin
            ,
            /// <summary>常駐クロールでの文書ファイル削除開始</summary>
            AlwaysCrawlDBDocumentDeleteBegin
            ,
            /// <summary>常駐クロールでのフォルダ内文書ファイル削除開始</summary>
            AlwaysCrawlDBDirectoryDeleteBegin
            ,
            /// <summary>完了（常駐クロールの場合は、待機状態に戻った）</summary>
            Finish
            ,
            /// <summary>常駐クロールを開始</summary>
            AlwaysCrawlBegin
            ,
            /// <summary>常駐クロールを終了</summary>
            AlwaysCrawlEnd
        }

        public Step CurrentStep { get; set; } = Step.DBRecordListUpBegin;
        public string Path { get; set; }
        public int CurrentValue { get; set; } = 0;
        public int? TotalValue { get; set; } = null;

        /// <summary>
        /// クロール時に見つかった、対象のファイルパスセット（スキップされたファイルも含む）
        /// </summary>
        public ISet<string> FoundTargetFilePaths { get; protected set; } = new HashSet<string>();
    }
}
