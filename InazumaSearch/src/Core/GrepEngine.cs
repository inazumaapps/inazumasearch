using System;
using System.Collections.Generic;
using System.Linq;

namespace InazumaSearch.Core
{
    /// <summary>
    /// ファイル内をgrep検索するクラス
    /// </summary>
    public class GrepEngine
    {
        /// <summary>
        /// マッチ行の前後何行まで表示対象とするか
        /// </summary>
        public const long ViewRangeBeforeAndAfterMatchLine = 5;

        /// <summary>
        /// マッチ結果（1つ以上のブロックを含む）
        /// </summary>
        public class MatchResult
        {
            public IList<MatchBlock> Blocks { get; protected set; } = new List<MatchBlock>();

            public string GetPrismViewRange()
            {
                return string.Join(",", Blocks.Select(b => b.GetPrismViewRange()));
            }
            public string GetPrismMatchLines()
            {
                return string.Join(",", Blocks.Select(b => string.Join(",", b.MatchLines)));
            }
        }

        /// <summary>
        /// マッチ結果ブロック
        /// </summary>
        public class MatchBlock
        {
            public IList<long> MatchLines { get; protected set; } = new List<long>();

            /// <summary>
            /// ソースコードの総行数
            /// </summary>
            public long LineTotal { get; protected set; }

            /// <summary>
            /// 開始行
            /// </summary>
            public long StartLine
            {
                get
                {
                    var top = MatchLines.First() - ViewRangeBeforeAndAfterMatchLine;
                    return (top < 1 ? 1 : top);
                }
            }

            /// <summary>
            /// 終了行
            /// </summary>
            public long EndLine
            {
                get
                {
                    var bottom = MatchLines.Last() + ViewRangeBeforeAndAfterMatchLine;
                    return (bottom > LineTotal ? LineTotal : bottom);
                }
            }

            /// <summary>
            /// コンストラクタ
            /// </summary>
            /// <param name="lineTotal"></param>
            public MatchBlock(long lineTotal) {
                this.LineTotal = lineTotal;
            }

            public string GetPrismViewRange()
            {
                return $"{StartLine}-{EndLine}";
            }
        }

        /// <summary>
        /// grep検索を実施し、マッチしている行をすべて抽出
        /// </summary>
        public MatchResult Grep(string body, string keyword)
        {
            var res = new MatchResult();

            var lines = body.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);
            MatchBlock currentBlock = null;

            for (long i = 0; i < lines.Length; i++)
            {
                var line = lines[i];
                var lineNumber = i + 1;

                // マッチ判定
                if (line.Contains(keyword))
                {
                    // ブロック開始
                    if (currentBlock == null)
                    {
                        currentBlock = new MatchBlock(lines.Length);
                    }

                    // マッチ行を保持
                    currentBlock.MatchLines.Add(lineNumber);
                }
                else
                {

                    // 前回マッチ行から指定行数以上離れたらブロックを切断
                    if (currentBlock != null && lineNumber > currentBlock.MatchLines.Last() + ViewRangeBeforeAndAfterMatchLine)
                    {
                        res.Blocks.Add(currentBlock);
                        currentBlock = null;
                    }
                }
            }

            // ブロックを切断
            if (currentBlock != null)
            {
                res.Blocks.Add(currentBlock);
            }

            return res;
        }
    }
}
