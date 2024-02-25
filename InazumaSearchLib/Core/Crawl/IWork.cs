using System;
using System.Collections.Generic;
using System.Threading;

namespace InazumaSearchLib.Core.Crawl
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
            IProgress<ProgressState> progress = null
        );

        /// <summary>
        /// トレースログ出力時の表記
        /// </summary>
        string TraceLogCaption { get; }
    }
}
