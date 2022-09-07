using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MessagePack;

namespace InazumaSearch.Core
{
    /// <summary>
    /// 動作ログ記録・出力クラス
    /// </summary>
    public class OperationLogger
    {
        public Queue<OperationLog> Buffer = new Queue<OperationLog>();

    }
}
