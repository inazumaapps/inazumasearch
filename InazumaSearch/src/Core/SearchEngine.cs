using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Alphaleonis.Win32.Filesystem;
using InazumaSearch.Groonga.Exceptions;

namespace InazumaSearch.Core
{
    public class SearchEngine
    {

        #region 内部クラス

        /// <summary>
        /// 検索の実行結果
        /// </summary>
        public class Result
        {
            /// <summary>
            /// 検索が成功したかどうか
            /// </summary>
            public bool success { get; set; }

            /// <summary>
            /// 検索実行時にエラーが発生した場合、そのエラーメッセージ
            /// </summary>
            public string errorMessage { get; set; }

            /// <summary>
            /// 検索結果の総件数
            /// </summary>
            public long nHits { get; set; }

            /// <summary>
            /// 取得した検索結果 (offsetから10件分)
            /// </summary>
            public IList<Record> records { get; set; }

            /// <summary>
            /// 検索結果を表すメッセージ
            /// </summary>
            public string searchResultMessage { get; set; }

            /// <summary>
            /// 検索結果を表す補助メッセージ (存在しない場合はnull)
            /// </summary>
            public string searchResultSubMessage { get; set; }

            /// <summary>
            /// ファイル形式のドリルダウン結果
            /// </summary>
            public IList<FormatDrilldownLink> formatDrilldownLinks { get; set; }

            /// <summary>
            /// フォルダラベルのドリルダウン結果
            /// </summary>
            public IList<FolderLabelDrilldownLink> folderLabelDrilldownLinks { get; set; }

            /// <summary>
            /// 更新時期のドリルダウン結果
            /// </summary>
            public IList<LastUpdatedDrilldownLink> lastUpdatedDrilldownLinks { get; set; }

            //public IList<YearDrilldownLink> yearDrilldownLinks { get; set; }
        }
        public class Record
        {
            public string key { get; set; }
            public string file_path { get; set; }
            public string file_name { get; set; }
            public string folder_path { get; set; }
            public string[] file_name_snippets { get; set; } = new string[] { };
            public string[] body_snippets { get; set; } = new string[] { };
            public string ext { get; set; }
            public string timestamp_updated_caption { get; set; }
            public string timestamp_updated_caption_for_list_view { get; set; }
            public string size_caption { get; set; }
            public long base_score { get; set; }
            public long final_score { get; set; }

            public string icon_data_url { get; set; }
            public string thumbnail_path { get; set; }
        }

        public class FormatDrilldownLink
        {
            public string name { get; set; }
            public string caption { get; set; }
            public long nSubRecs { get; set; }
        }
        public class LastUpdatedDrilldownLink
        {
            public long group { get; set; }
            public string caption { get; set; }
            public long nSubRecs { get; set; }
        }
        public class FolderLabelDrilldownLink
        {
            public string folderLabel { get; set; }
            public long nSubRecs { get; set; }
        }

        /// <summary>
        /// 経過日数グループ
        /// </summary>
        public class ElapsedGroup
        {
            /// <summary>
            /// インデックス (小さいほど近い)
            /// </summary>
            public int Index { get; set; }

            /// <summary>
            /// 経過日数が何日以内か
            /// </summary>
            public long DayRange { get; set; }

            /// <summary>
            /// 表示名
            /// </summary>
            public string Caption { get; set; }
        }

        #endregion

        #region プロパティ

        public NLog.Logger Logger { get; protected set; }
        public Core.Application App { get; set; }

        /// <summary>
        /// 経過日数グループデータ
        /// </summary>
        protected List<ElapsedGroup> ElapsedGroups { get; set; } = new List<ElapsedGroup>();

        /// <summary>
        /// 経過日数グループのマップ
        /// </summary>
        protected Dictionary<int, ElapsedGroup> ElapsedGroupMap { get; set; } = new Dictionary<int, ElapsedGroup>();

        #endregion

        #region コンストラクタ

        public SearchEngine(Application app)
        {
            Logger = NLog.LogManager.GetLogger(LoggerName.Crawler);

            App = app;

            // 経過日数グループ情報の初期化
            var elapsedGroupBaseData = new[]{
                  Tuple.Create(0, "今日")
                , Tuple.Create(1, "昨日")
                , Tuple.Create(2, "2日前")
                , Tuple.Create(3, "3日前")
                , Tuple.Create(7, "約1週間以内")
                , Tuple.Create(7*2, "約2週間以内")
                , Tuple.Create(30, "約1ヶ月以内")
                , Tuple.Create(30*2, "約2ヶ月以内")
                , Tuple.Create(30*6, "約半年以内")
                , Tuple.Create(365, "約1年以内")
                , Tuple.Create(365*3, "約3年以内")
                , Tuple.Create(-1, "もっと前") // 上記全ての条件に当てはまらないときに使われる特殊グループ
            };
            ElapsedGroups = elapsedGroupBaseData.Select((e, i) => new ElapsedGroup { Index = i, DayRange = e.Item1, Caption = e.Item2 })
                                                .ToList();
            ElapsedGroupMap = ElapsedGroups.ToDictionary(g => g.Index);
        }

        #endregion

        #region メソッド

        /// <summary>
        /// 全文検索の実行
        /// </summary>
        /// <param name="query"></param>
        /// <param name="offset"></param>
        /// <returns></returns>
        public Result Search(
              string queryKeyword
            , string queryFileName = null
            , string queryBody = null
            , string queryUpdated = null
            , string querySortBy = null
            , int offset = 0
            , string selectedFormat = null
            , string selectedFolderLabel = null
            , int? selectedLastUpdatedGroup = null
        )
        {
            var groongaQueries = new List<string>();
            var groongaFilters = new List<string>();
            var querySubMessages = new List<string>();

            // キーワード、ファイル名、本文での絞り込みをエスケープして追加
            if (!string.IsNullOrWhiteSpace(queryKeyword))
            {
                // Groongaのクエリ構文は一部のみ有効
                // バックスラッシュ、シングルクォート、コロンなどの特殊記号はエスケープする
                // 有効なのは " - ( ) のみ
                groongaQueries.Add(queryKeyword
                                    .Replace(@"\", @"\\")
                                    .Replace("'", @"\'")
                                    .Replace(":", @"\:")
                                    .Replace("+", @"\+"));

                //// 特殊記号をエスケープ
                //// バックスラッシュ(\)、シングルクォート(')、一部パターンを除くコロン(:)、プラス(+)をエスケープする
                //var escapeRegex = new Regex(@"\\|\'|(?<!fname)\:|\+", RegexOptions.IgnoreCase);
                //var formattedQuery = escapeRegex.Replace(queryKeyword, (m) => { return @"\" + m.Value; });

                //// "fname:" "filename:" 形式であれば、file_nameに置換
                //var aliasRegex = new Regex(@"fname\:|filename\:", RegexOptions.IgnoreCase);
                //formattedQuery = aliasRegex.Replace(formattedQuery, (m) => { return "file_name:@"; });

                //groongaQueries.Add(formattedQuery);
            }
            if (!string.IsNullOrWhiteSpace(queryFileName))
            {
                groongaQueries.Add(string.Format("{0}:@{1}"
                                                , Column.Documents.FILE_NAME
                                                , Groonga.Util.EscapeForQuery(queryFileName)));
                querySubMessages.Add(string.Format("ファイル名: 「{0}」", queryFileName));
            }
            if (!string.IsNullOrWhiteSpace(queryBody))
            {
                groongaQueries.Add(string.Format("{0}:@{1}"
                                                , Column.Documents.BODY
                                                , Groonga.Util.EscapeForQuery(queryBody)));
                querySubMessages.Add(string.Format("本文: 「{0}」", queryBody));
            }
            // 拡張子での絞り込みを追加 (フォーマットから生成する)
            if (selectedFormat != null)
            {
                if (selectedFormat == "その他")
                {
                    // 「その他」が指定された場合は、登録されている拡張子をすべて除外
                    var excludeExtNames = new List<string>();
                    foreach (var format in App.Formats)
                    {
                        excludeExtNames.AddRange(format.Extensions);
                    }

                    var strings = excludeExtNames.Select(e => string.Format("\"{0}\"", e));
                    if (!strings.Any()) strings = new[] { "\"\"" };
                    groongaFilters.Add(string.Format("!in_values({0},{1})"
                                                    , Column.Documents.EXT
                                                    , string.Join(", ", strings))
                                        );
                    querySubMessages.Add("ファイル形式: その他");


                }
                else
                {
                    var targetExtNames = new List<string>();
                    foreach (var format in App.Formats.Where(f => f.Label == selectedFormat))
                    {
                        targetExtNames.AddRange(format.Extensions);
                    };
                    var strings = targetExtNames.Select(e => string.Format("\"{0}\"", e));
                    if (!strings.Any()) strings = new[] { "\"\"" };
                    groongaFilters.Add(string.Format("in_values({0},{1})"
                                                    , Column.Documents.EXT
                                                    , string.Join(", ", strings))
                                        );
                    querySubMessages.Add(string.Format("ファイル形式: {0}", selectedFormat));

                }
            }

            //// 年での絞り込みを追加
            //if (selectedYear != null)
            //{
            //    groongaQueries.Add(string.Format("{0}:{1}"
            //                                   , Column.Documents.FILE_UPDATED_YEAR
            //                                   , selectedYear)
            //                       );
            //}
            // フォルダラベルでの絞り込みを追加
            if (selectedFolderLabel != null)
            {
                groongaQueries.Add(string.Format("{0}:{1}"
                                                , Column.Documents.FOLDER_LABELS
                                                , Groonga.Util.EscapeForQuery(selectedFolderLabel)));
                querySubMessages.Add(string.Format("フォルダラベル: {0}", selectedFolderLabel));
            }

            // 最終更新日での絞り込みを追加
            if (selectedLastUpdatedGroup != null)
            {
                var grp = ElapsedGroupMap[selectedLastUpdatedGroup.Value];
                groongaFilters.Add($"last_updated_group == {grp.Index}");
                querySubMessages.Add(string.Format("最終更新日: {0}", grp.Caption));
            }

            // 日付範囲の指定があれば、その範囲を追加する
            DateTime? updatedLeft = null;
            string updatedRangeCaption = null;
            switch (queryUpdated)
            {
                case "day":
                    updatedLeft = DateTime.Now.AddDays(-1);
                    updatedRangeCaption = "1日以内";
                    break;
                case "week":
                    updatedLeft = DateTime.Now.AddDays(-7);
                    updatedRangeCaption = "1週間以内";
                    break;
                case "month":
                    updatedLeft = DateTime.Now.AddMonths(-1);
                    updatedRangeCaption = "1ヶ月以内";
                    break;
                case "half_year":
                    updatedLeft = DateTime.Now.AddMonths(-6);
                    updatedRangeCaption = "半年以内";
                    break;
                case "year":
                    updatedLeft = DateTime.Now.AddYears(-1);
                    updatedRangeCaption = "1年以内";
                    break;
                case "3years":
                    updatedLeft = DateTime.Now.AddYears(-3);
                    updatedRangeCaption = "3年以内";
                    break;
            }
            if (updatedLeft != null)
            {
                groongaFilters.Add(string.Format("file_updated_at > \"{0}\"", Groonga.Util.ToExprTimeFormat(updatedLeft.Value)));
                querySubMessages.Add(string.Format("更新日付: {0}", updatedRangeCaption));
            }

            var matchColumns = new[] {
                Column.Documents.FILE_NAME + " * 1000"
                , string.Format("scorer_tf_idf({0})", Column.Documents.BODY)
            };

            var columns = new List<Groonga.DynamicColumn>();
            // 最終更新からの経過時間
            var elapsedExpr = string.Format("((now() - {0}) / (60 * 60))", Column.Documents.FILE_UPDATED_AT);
            columns.Add(new Groonga.DynamicColumn(
                    "elapsed_hour_from_file_updated"
                , Groonga.Stage.FILTERED
                , Groonga.DataType.Int64
                , elapsedExpr
            ));
            // 鮮度補正 (最終更新からの経過時間によるスコア補正倍率)
            var rateData = new[] {
                        Tuple.Create(12, 30.0) // 半日以内なら×30
                    , Tuple.Create(24, 20.0) // 1日以内なら×20
                    , Tuple.Create(24 * 3, 10.0) // 3日以内なら×10
                    , Tuple.Create(24 * 7, 7.0) // 1週間以内なら×7
                    , Tuple.Create(24 * 15, 5.0) // 15日以内なら×5
                    , Tuple.Create(24 * 30, 3.0) // 30日(約1ヶ月)以内なら×3
                    , Tuple.Create(24 * 60, 2.0) // 60日(約2ヶ月)以内なら×2
                    , Tuple.Create(24 * 90, 1.5) // 90日(約3ヶ月)以内なら×1.5
                };
            var rateExpr = "1.0";
            foreach (var t in rateData.Reverse())
            {
                var border = t.Item1;
                var rate = t.Item2;

                rateExpr = string.Format("({0} <= {1} ? {2} : {3})", elapsedExpr, border, rate, rateExpr);
            }
            columns.Add(new Groonga.DynamicColumn(
                    "freshness_score_rate"
                , Groonga.Stage.FILTERED
                , Groonga.DataType.Float
                , rateExpr
            ));

            // 最終スコア
            columns.Add(new Groonga.DynamicColumn(
                    "final_score"
                , Groonga.Stage.FILTERED
                , Groonga.DataType.Int64
                , string.Format("{0} * {1}", Groonga.VColumn.SCORE, rateExpr)
            ));

            // 最終更新日が何日前か
            columns.Add(new Groonga.DynamicColumn(
                  "day_before_file_updated"
                , Groonga.Stage.INITIAL
                , Groonga.DataType.Time
                , $"({Column.Documents.FILE_UPDATED_AT} >= 1 ? time_classify_day(now()) - time_classify_day({Column.Documents.FILE_UPDATED_AT}) : 10000) / (60 * 60 * 24)"
            //, $"(time_classify_day(now()) - time_classify_day({Column.Documents.FILE_UPDATED_AT})) / (60 * 60 * 24)"
            ));

            // 最終更新日グループを判定するための条件式を構築
            var elapsedGroupExpr = $"{ElapsedGroups.Last().Index}";
            foreach (var item in ElapsedGroups.Where(g => g.DayRange >= 0)
                                              .OrderByDescending(g => g.Index))
            {
                elapsedGroupExpr = $"(day_before_file_updated <= {item.DayRange} ? {item.Index} : {elapsedGroupExpr})";
            }
            columns.Add(new Groonga.DynamicColumn(
                  "last_updated_group"
                , Groonga.Stage.INITIAL
                , Groonga.DataType.UInt8
                , elapsedGroupExpr
            ));

            // 並び順の設定。詳細検索で並び順が指定されていれば、その並び順を優先
            var sortKeys = new List<string>();
            switch (querySortBy)
            {
                case "path":
                    sortKeys.Add(Column.Documents.FILE_PATH);
                    break;
                case "updated":
                    sortKeys.Add(Column.Documents.FILE_UPDATED_AT);
                    break;
                case "updated_desc":
                    sortKeys.Add("-" + Column.Documents.FILE_UPDATED_AT);
                    break;
                case "size":
                    sortKeys.Add(Column.Documents.SIZE);
                    break;
                case "size_desc":
                    sortKeys.Add("-" + Column.Documents.SIZE);
                    break;
            }
            // 標準では最終スコア(関連度+鮮度)が高い順に並べる
            sortKeys.Add("-final_score");


            // SELECT実行
            var joinedQuery = string.Join(" ", groongaQueries.Select(q => "(" + q + ")"));
            var joinedFilter = string.Join(" && ", groongaFilters.Select(q => "(" + q + ")"));

            Groonga.SelectResult selectRes = null;
            try
            {
                selectRes = App.GM.Select(
                      Table.Documents
                    , query: joinedQuery
                    , filter: joinedFilter
                    , offset: offset
                    //, drilldown: new[] { Column.Documents.EXT, Column.Documents.FILE_UPDATED_YEAR }
                    , drilldown: new[] { Column.Documents.EXT, Column.Documents.FOLDER_LABELS, "last_updated_group" }
                    , drilldownSortKeys: new[] { Column.Documents.KEY }
                    , sortKeys: sortKeys.ToArray()
                    , matchColumns: matchColumns
                    , outputColumns: new[] {
                                Column.Documents.KEY
                            , Column.Documents.FILE_PATH
                            , Groonga.VColumn.SCORE // 鮮度補正を加味していない生のスコア
                            , Column.Documents.TITLE
                            , Column.Documents.EXT
                            , Column.Documents.FILE_NAME
                            , Column.Documents.FILE_UPDATED_AT
                            , Column.Documents.SIZE
                            , Groonga.Function.SnippetHtml(Column.Documents.FILE_NAME)
                            , Groonga.Function.SnippetHtml(Column.Documents.BODY)
                            , "elapsed_hour_from_file_updated"
                            , "freshness_score_rate"
                            , "final_score"
                            , "last_updated_group"
                    }
                    , columns: columns
                );
            }
            catch (GroongaCommandError ex)
            {
                if (ex.ReturnCode == Groonga.CommandReturnCode.GRN_SYNTAX_ERROR
                    || ex.ReturnCode == Groonga.CommandReturnCode.GRN_INVALID_ARGUMENT)
                {
                    // シンタックスエラーの場合、クエリ構文にエラーがあるかどうかをチェック
                    // 引数を単純化した上でもう一度Selectを実行し、同じエラーが出るならクエリ構文が不正とみなす
                    Logger.Debug(ex.ToString());
                    try
                    {
                        App.GM.Select(Table.Documents, limit: 0, outputColumns: new[] { Groonga.VColumn.ID }, query: joinedQuery);
                    }
                    catch (GroongaCommandError)
                    {
                        if (ex.ReturnCode == Groonga.CommandReturnCode.GRN_SYNTAX_ERROR
                            || ex.ReturnCode == Groonga.CommandReturnCode.GRN_INVALID_ARGUMENT)
                        {
                            var errorRet = new Result
                            {
                                success = false,
                                errorMessage = "検索語の解析時にエラーが発生しました。\n単語をダブルクォート (\") で囲んで試してみてください。"
                            };
                            return errorRet;
                        }

                        throw;
                    }

                }

                throw;
            }
            var searchResult = selectRes.SearchResult;
            var records = new List<Record>();
            //var imgConv = new ImageConverter();

            var cryptProvider = new SHA1CryptoServiceProvider();
            var thumbnailDirPath = App.ThumbnailDirPath;

            foreach (var selectRec in searchResult.Records)
            {
                var rec = new Record
                {
                    key = (string)selectRec.Key,
                    file_name = (string)selectRec[Column.Documents.FILE_NAME],
                    file_path = (string)selectRec[Column.Documents.FILE_PATH]
                };
                if (!string.IsNullOrEmpty(rec.file_path))
                {
                    rec.folder_path = Path.GetDirectoryName(rec.file_path);
                }
                rec.base_score = selectRec.GetIntValue(Groonga.VColumn.SCORE).Value;
                rec.final_score = selectRec.GetIntValue("final_score").Value;

                //// OSからアイコン画像を取得
                //var icon = WindowsIconGetter.GetFileIcon(path);
                //byte[] bytes;
                //using (var ms = new MemoryStream())
                //{
                //    icon.Save(ms);
                //    bytes = ms.ToArray();
                //}

                //var b64String = Convert.ToBase64String(bytes);
                //rec.icon_data_url = "data:image/png;base64," + b64String;

                //// OSからサムネイル画像を取得
                //var sh = ShellObject.FromParsingName(path);
                //sh.Thumbnail.FormatOption = ShellThumbnailFormatOption.IconOnly;
                //var bmp = sh.Thumbnail.LargeBitmap;
                //bmp.MakeTransparent(Color.Black);

                //byte[] bytes;
                //using (var ms = new MemoryStream())
                //{
                //    bmp.Save(ms, ImageFormat.Png);
                //    bytes = ms.GetBuffer();
                //}

                //var b64String = Convert.ToBase64String(bytes);
                //rec.icon_data_url = "data:image/png;base64," + b64String;


                var fileNameSnippets = selectRec[Groonga.Function.SnippetHtml(Column.Documents.FILE_NAME)] as object[];
                if (fileNameSnippets != null)
                {
                    rec.file_name_snippets = fileNameSnippets.Cast<string>().ToArray();
                }

                var bodySnippets = selectRec[Groonga.Function.SnippetHtml(Column.Documents.BODY)] as object[];
                if (bodySnippets != null)
                {
                    rec.body_snippets = bodySnippets.Cast<string>().Select(s => s.Replace("\n", "<br>")).ToArray();
                }

                rec.ext = (string)selectRec[Column.Documents.EXT];
                var updated = Groonga.Util.FromUnixTime((double)selectRec[Column.Documents.FILE_UPDATED_AT]);
                rec.timestamp_updated_caption = updated.ToString("yyyy/MM/dd(ddd) HH:mm");
                rec.timestamp_updated_caption_for_list_view = updated.ToString("yyyy/MM/dd HH:mm");
                rec.size_caption = Util.FormatFileSizeByKB((long)selectRec[Column.Documents.SIZE]);

                // サムネイル画像があれば、そのパスも設定
                var thumbnailPath = Path.Combine(thumbnailDirPath, Util.HexDigest(cryptProvider, rec.key) + ".png");
                if (File.Exists(thumbnailPath))
                {
                    rec.thumbnail_path = thumbnailPath;
                }
                else
                {
                    rec.thumbnail_path = null;
                }

                records.Add(rec);
            }


            // ドリルダウン結果(拡張子ごとの件数)を元に、フォーマット絞り込み用のデータを作成
            var formatDrilldownLinks = new List<FormatDrilldownLink>();
            var formatCounts = new Dictionary<string, long>();
            var formatToLabelMap = new Dictionary<string, string>();
            foreach (var format in App.Formats)
            {
                foreach (var ext in format.Extensions)
                {
                    formatToLabelMap[ext] = format.Label;
                }
            }

            foreach (var extRec in selectRes.DrilldownResults[0].Records)
            {
                var ext = (string)extRec.Key;
                var label = (formatToLabelMap.ContainsKey(ext) ? formatToLabelMap[ext] : "その他");

                if (!formatCounts.ContainsKey(label)) formatCounts[label] = 0;
                formatCounts[label] += extRec.GetIntValue(Groonga.VColumn.NSUBRECS).Value;
            }


            foreach (var formatLabel in formatCounts.Keys)
            {
                var link = new FormatDrilldownLink()
                {
                    name = formatLabel
                    ,
                    caption = formatLabel
                    ,
                    nSubRecs = formatCounts[formatLabel]
                };
                formatDrilldownLinks.Add(link);
            }

            //// ドリルダウン結果(年ごとの件数)を元に、フォーマット絞り込み用のデータを作成
            //var yearDrilldownLinks = new List<YearDrilldownLink>();
            //foreach (var yearRec in selectRes.DrilldownResults[1].Records)
            //{
            //    var link = new YearDrilldownLink()
            //    {
            //        year = (int)(long)yearRec.Key
            //        ,
            //        nSubRecs = yearRec.NSubRecs
            //    };
            //    yearDrilldownLinks.Add(link);
            //}

            // ドリルダウン結果(フォーマットごとの件数)を元に、フォーマット絞り込み用のデータを作成
            var folderLabelDrilldownLinks = new List<FolderLabelDrilldownLink>();
            foreach (var rec in selectRes.DrilldownResults[1].Records)
            {
                var link = new FolderLabelDrilldownLink()
                {
                    folderLabel = (string)rec.Key
                    ,
                    nSubRecs = rec.NSubRecs
                };
                folderLabelDrilldownLinks.Add(link);
            }

            // ドリルダウン結果(更新時期ごとの件数)を元に、更新時期絞り込み用のデータを作成
            var lastUpdatedDrilldownLinks = new List<LastUpdatedDrilldownLink>();
            foreach (var rec in selectRes.DrilldownResults[2].Records)
            {
                var index = (long)rec.Key;
                var link = new LastUpdatedDrilldownLink()
                {
                    group = index
                    ,
                    caption = ElapsedGroupMap[(int)index].Caption
                    ,
                    nSubRecs = rec.NSubRecs
                };
                lastUpdatedDrilldownLinks.Add(link);
            }


            string searchResultMessage;
            var searchResultSubMessage = "";

            if (searchResult.NHits == 0)
            {
                searchResultMessage = "文書が見つかりませんでした。";
            }
            else
            {
                if (string.IsNullOrWhiteSpace(queryKeyword))
                {
                    searchResultMessage = string.Format("計{0}件見つかりました。", searchResult.NHits);
                }
                else
                {
                    searchResultMessage = string.Format("「{0}」で検索して、計{1}件見つかりました。", queryKeyword, searchResult.NHits);
                }
            }

            if (querySubMessages.Count >= 1)
            {
                searchResultSubMessage = string.Format("({0})", string.Join("、", querySubMessages));
            }

            var ret = new Result()
            {
                success = true
                ,
                nHits = searchResult.NHits
                ,
                records = records.ToArray()
                ,
                searchResultMessage = searchResultMessage
                ,
                searchResultSubMessage = searchResultSubMessage
                ,
                formatDrilldownLinks = formatDrilldownLinks.OrderByDescending(l => l.nSubRecs).ToList() // 件数の多い順で並べる
                ,
                folderLabelDrilldownLinks = folderLabelDrilldownLinks.OrderByDescending(l => l.nSubRecs).ToList() // 件数の多い順で並べる
                ,
                lastUpdatedDrilldownLinks = lastUpdatedDrilldownLinks.OrderBy(l => l.group).ToList() // 更新日が近い順で並べる
            };

            return ret;

        }

        /// <summary>
        /// 類似文書検索の実行
        /// </summary>
        /// <param name="query"></param>
        /// <param name="offset"></param>
        /// <returns></returns>
        public Groonga.SelectResult SearchSimilarDocuments(
              string key
        )
        {
            // 対象文書の本文を取得
            var targetRes = App.GM.Select(
                  Table.Documents
                , query: string.Format("{0}:{1}", Column.Documents.KEY, Groonga.Util.EscapeForQuery(key))
            );
            var body = targetRes.SearchResult.Records[0].GetTextValue(Column.Documents.BODY);

            var selectRes = App.GM.Select(
                  Table.Documents
                , filter: string.Format("{0} *S {1}", Column.Documents.BODY, Groonga.Util.EscapeForScript(body))
            );

            return selectRes;

        }


        #endregion


    }
}
