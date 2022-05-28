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
        public const long ViewRangeBeforeAndAfterMatchLine = 3;

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
                return string.Join(",", Blocks.Select(b => b.GetPrismMatchLines()));
            }
            public string GetPrismMatchRanges()
            {
                return string.Join(",", Blocks.Select(b => b.GetPrismMatchRanges()));
            }
        }

        /// <summary>
        /// マッチ結果ブロック
        /// </summary>
        public class MatchBlock
        {
            public IList<MatchLine> MatchLines { get; protected set; } = new List<MatchLine>();

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
                    var top = MatchLines.First().LineNumber - ViewRangeBeforeAndAfterMatchLine;
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
                    var bottom = MatchLines.Last().LineNumber + ViewRangeBeforeAndAfterMatchLine;
                    return (bottom > LineTotal ? LineTotal : bottom);
                }
            }

            /// <summary>
            /// コンストラクタ
            /// </summary>
            /// <param name="lineTotal"></param>
            public MatchBlock(long lineTotal)
            {
                this.LineTotal = lineTotal;
            }

            public string GetPrismViewRange()
            {
                return $"{StartLine}-{EndLine}";
            }
            public string GetPrismMatchLines()
            {
                return string.Join(",", this.MatchLines.Select(l => l.LineNumber));
            }
            public string GetPrismMatchRanges()
            {
                return string.Join(",", this.MatchLines.Select(l => l.GetPrismMatchRanges()));
            }
        }



        /// <summary>
        /// マッチ結果行
        /// </summary>
        public class MatchLine
        {
            /// <summary>
            /// 行番号
            /// </summary>
            public long LineNumber { get; protected set; }

            /// <summary>
            /// 行内でのマッチ範囲のリスト
            /// </summary>
            public IList<Tuple<long, long>> MatchRanges { get; protected set; } = new List<Tuple<long, long>>();

            /// <summary>
            /// コンストラクタ
            /// </summary>
            /// <param name="lineTotal"></param>
            public MatchLine(long lineNumber)
            {
                this.LineNumber = lineNumber;
            }

            public string GetPrismMatchRanges()
            {
                return String.Join(",", MatchRanges.Select(r => $"{LineNumber}:{r.Item1}-{r.Item2}"));
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
                var pos = line.IndexOf(keyword);
                if (pos >= 0)
                {
                    // ブロック開始
                    if (currentBlock == null)
                    {
                        currentBlock = new MatchBlock(lines.Length);
                    }

                    // マッチ行を保持
                    var matchLine = new MatchLine(lineNumber);
                    matchLine.MatchRanges.Add(Tuple.Create((long)pos, (long)pos + keyword.Length));
                    currentBlock.MatchLines.Add(matchLine);
                }
                else
                {

                    // 前回マッチ行から指定行数以上離れたらブロックを切断
                    if (currentBlock != null && lineNumber > currentBlock.MatchLines.Last().LineNumber + ViewRangeBeforeAndAfterMatchLine)
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
