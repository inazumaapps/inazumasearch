using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InazumaSearch.Core
{
    /// <summary>
    /// ファイル内をgrep検索するクラス
    /// </summary>
    public class GrepEngine
    {
        public class MatchResult
        {
            public IList<MatchBlock> Blocks { get; protected set; } = new List<MatchBlock>();

            public string GetPrismLineRange()
            {
                return string.Join(",", Blocks.Select(b => b.GetPrismLineRange()));
            }
        }

        public class MatchBlock {
            public IList<MatchRange> Ranges { get; protected set; } = new List<MatchRange>();

            public string GetPrismLineRange()
            {
                return string.Join(",", Ranges.Select(r => r.GetPrismLineRange()));
            }
        }

        public class MatchRange {
            public long StartLine;
            public long EndLine;

            public string GetPrismLineRange()
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
            long? lastMatchLineNumber = null;
            MatchBlock currentBlock = null;
            MatchRange currentRange = null;

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
                        currentBlock = new MatchBlock();
                    }

                    // Range開始
                    if (currentRange == null)
                    {
                        currentRange = new MatchRange() { StartLine = lineNumber };
                    }

                    // 最終マッチ行を保持
                    lastMatchLineNumber = lineNumber;
                }
                else
                {
                    // Range確定
                    if (currentRange != null)
                    {
                        currentRange.EndLine = lineNumber - 1;
                        currentBlock.Ranges.Add(currentRange);
                        currentRange = null;
                    }

                    // 前回マッチ行から4行以上離れたらブロックを切断
                    if (currentBlock != null && lineNumber >= lastMatchLineNumber + 4)
                    {
                        res.Blocks.Add(currentBlock);
                        currentBlock = null;
                    }
                }
            }

            // Range内であれば確定
            if (currentRange != null)
            {
                currentRange.EndLine = lines.Length;
                currentBlock.Ranges.Add(currentRange);
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
