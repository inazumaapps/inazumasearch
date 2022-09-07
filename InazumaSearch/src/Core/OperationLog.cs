using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MessagePack;

namespace InazumaSearch.Core
{
    [MessagePackObject(keyAsPropertyName: true)]
    public class OperationLog
    {
        public int TypeCode { get; set; } 

        public string FilePath { get; set; }

        public string Message { get; set; }

        public double Timestamp { get; set; }

        public OperationLog()
        {
            Timestamp = Groonga.Util.ToUnixTime(DateTime.Now);
        }
    }
}
