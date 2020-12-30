using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace InazumaSearch.Core.Crawl
{
    /// <summary>
    /// クロール時に実行する1つ1つの処理を表すインターフェース
    /// </summary>
    public interface IWork : IEquatable<IWork>
    {
        /// <summary>
        /// 処理実行
        /// </summary>
        void Execute(
            Stack<IWork> workStack,
            Result crawlResult,
            CancellationToken cToken,
            IProgress<CrawlState> progress = null
        );

        /// <summary>
        /// トレースログ出力時の表記
        /// </summary>
        string TraceLogCaption { get; }
    }
}
