using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MessagePack;

namespace InazumaSearch.Core
{
    /// <summary>
    /// 動作ログ
    /// </summary>
    [MessagePackObject(keyAsPropertyName: true)]
    public class OperationLog
    {
        public int TypeCode { get; set; } 

        public string FilePath { get; set; }

        public string Message { get; set; }

        public DateTime Timestamp { get; set; }

        public OperationLog()
        {
            Timestamp = DateTime.Now;
        }
    }
}
