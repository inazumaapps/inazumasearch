using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InazumaSearch.Core.Crawl
{
    public class ProgressState
    {
        public enum Step
        {
            /// <summary>DBの文書データ取得開始</summary>
            DBRecordListUpBegin
            ,
            /// <summary>文書データ登録開始（クロール開始時に一度のみ実行）</summary>
            RecordUpdateProcessBegin
            ,
            /// <summary>文書データを登録するかどうかの判定開始（ファイルごとに実行）</summary>
            RecordUpdateCheckBegin
            ,
            /// <summary>文書データの実登録開始（新しいファイル／変更されたファイル1件ごとに実行）</summary>
            RecordUpdateBegin
            ,
            /// <summary>文書データの実登録完了（新しいファイル／変更されたファイル1件ごとに実行）</summary>
            RecordUpdateEnd
            ,
            /// <summary>DB内の不要データ削除処理開始（削除処理全体で一度のみ実行）</summary>
            PurgeProcessBegin
            ,
            /// <summary>DB内の不要データ実削除開始（削除対象のデータごとに実行）</summary>
            PurgeBegin
            ,
            /// <summary>DB内の不要データ削除処理完了（削除処理全体で一度のみ実行）</summary>
            PurgeProcessEnd
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
