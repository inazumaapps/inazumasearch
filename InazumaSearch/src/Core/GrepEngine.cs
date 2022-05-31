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

            public string BodyWithoutTag { get; set; }

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
            public IList<Tuple<int, int>> MatchRanges { get; protected set; } = new List<Tuple<int, int>>();

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
        public MatchResult Grep(string body)
        {
            var res = new MatchResult();

            var lines = body.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);
            MatchBlock currentBlock = null;

            var openTag = "<match>";
            var openTagLength = openTag.Length;
            var closeTag = "</match>";
            var closeTagLength = closeTag.Length;


            for (long i = 0; i < lines.Length; i++)
            {
                var line = lines[i];
                var lineNumber = i + 1;

                // まずは行中に1つでもマッチ箇所があるかどうかを判定
                if (line.Contains(openTag))
                {
                    // ブロック開始
                    if (currentBlock == null)
                    {
                        currentBlock = new MatchBlock(lines.Length);
                    }

                    // マッチ行を保持
                    var matchLine = new MatchLine(lineNumber);
                    currentBlock.MatchLines.Add(matchLine);

                    // <match></match>タグの位置を元にマッチした位置をすべて取得し、タグを削除する
                    {
                        var newLine = "";
                        var restLine = line;
                        var curPosWithoutTag = 0; // <match></match>タグを無視した場合のカレントポジション（桁位置）
                        var matchRanges = new List<Tuple<int, int>>();

                        while (true)
                        {
                            // まずはマッチ開始位置を判定
                            var matchedStartPosInRestLine = restLine.IndexOf(openTag);

                            // マッチ開始位置が見つからなければ脱出
                            if (matchedStartPosInRestLine < 0) break;

                            // マッチ開始位置が見つかった場合、マッチ開始位置を記録して、マッチ開始位置までの文字列を切り出し、カレントポジションを進める
                            var matchStartPos = curPosWithoutTag + matchedStartPosInRestLine;
                            var beforeMatched = restLine.Substring(0, matchedStartPosInRestLine);
                            curPosWithoutTag += beforeMatched.Length;
                            newLine += beforeMatched;
                            restLine = restLine.Substring(beforeMatched.Length + openTagLength);

                            // マッチ終了位置を判定
                            var matchedEndPosInRestLine = restLine.IndexOf(closeTag);

                            // 不具合が発生してマッチ終了位置が見つからなければ脱出
                            if (matchedEndPosInRestLine < 0) break;

                            // マッチ開始位置が見つかった場合、マッチ終了位置を記録して、マッチ終了位置までの文字列を切り出し、カレントポジションを進める
                            var matchEndPos = curPosWithoutTag + matchedEndPosInRestLine;
                            var matched = restLine.Substring(0, matchedEndPosInRestLine);
                            curPosWithoutTag += matched.Length;
                            newLine += matched;
                            restLine = restLine.Substring(matched.Length + closeTagLength);

                            // マッチ範囲を記録
                            matchLine.MatchRanges.Add(Tuple.Create(matchStartPos, matchEndPos));
                        }

                        // 最後にタグを削除した後の行を上書き
                        lines[i] = newLine;
                    }
                    //matchLine.MatchRanges.Add(Tuple.Create((long)pos, (long)pos + keyword.Length));
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

            // タグ削除後のボディをセット
            res.BodyWithoutTag = string.Join("\n", lines);

            return res;
        }
    }
}
