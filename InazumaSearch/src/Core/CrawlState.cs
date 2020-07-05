using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InazumaSearch.Core
{
    public class CrawlState
    {
        public enum Step
        {
            TargetListUp
            , DBRecordListUpBegin
            , RecordAddBegin
            , RecordAdding
            , PurgeBegin
            , Finish
        }

        public Step CurrentStep { get; set; } = Step.TargetListUp;
        public int CurrentValue { get; set; } = 0;
        public int UpdatedDocumentCount { get; set; } = 0;
        public int TotalValue { get; set; } = 0;
    }
}
