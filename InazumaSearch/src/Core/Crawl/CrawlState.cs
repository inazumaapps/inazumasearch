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
            DBRecordListUpBegin
            , RecordUpdateBegin
            , RecordUpdating
            , PurgeBegin
            , Finish
        }

        public Step CurrentStep { get; set; } = Step.DBRecordListUpBegin;
        public int CurrentValue { get; set; } = 0;
        public int? TotalValue { get; set; } = null;

        /// <summary>
        /// クロール時に見つかった、対象のファイルパスセット（スキップされたファイルも含む）
        /// </summary>
        public ISet<string> FoundTargetFilePaths { get; protected set; } = new HashSet<string>();
    }
}
