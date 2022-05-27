using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
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
            /// 検索時に使用したページサイズ（1ページ当たりの結果表示数）
            /// </summary>
            public int pageSize { get; set; }

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
            /// 並び順の選択肢一覧
            /// </summary>
            public IList<OrderItem> orderList { get; set; }
        }
        public class Record
        {
            public string key { get; set; }
            public string folder_path { get; set; }
            public string file_path { get; set; }
            public string file_name { get; set; }
            public string title { get; set; }
            public string body { get; set; }
            public string ext { get; set; }
            public string timestamp_updated_caption { get; set; }
            public string timestamp_updated_caption_for_list_view { get; set; }
            public string size_caption { get; set; }

            public string icon_data_url { get; set; }
            public string thumbnail_path { get; set; }

            public string prism_view_range { get; set; }
            public string prism_match_lines { get; set; }
        }

        public class FormatDrilldownLink
        {
            public string name { get; set; }
            public string caption { get; set; }
            public long nSubRecs { get; set; }
        }
        public class FolderLabelDrilldownLink
        {
            public string folderLabel { get; set; }
            public long nSubRecs { get; set; }
        }

        /// <summary>
        /// 並び順項目
        /// </summary>
        public class OrderItem
        {
            public string Caption { get; set; }
            public string Type { get; set; }
            public string GroongaExpr { get; set; }
        }

        /// <summary>
        /// 並び順タイプ
        /// </summary>
        public class OrderType
        {
            /// <summary>
            /// 関連度順
            /// </summary>
            public const string SCORE = "score";

            /// <summary>
            /// ファイルの更新日が新しい順
            /// </summary>
            public const string FILE_UPDATED_DESC = "file_updated_desc";

            /// <summary>
            /// ファイルパス順
            /// </summary>
            public const string FILE_PATH = "file_path";
        }

        /// <summary>
        /// 表示形式
        /// </summary>
        public class ViewType
        {
            /// <summary>
            /// 通常
            /// </summary>
            public const string NORMAL = "normal";

            /// <summary>
            /// ファイルの更新日が新しい順
            /// </summary>
            public const string LIST = "list";
        }


        #endregion

        #region プロパティ

        public NLog.Logger Logger { get; protected set; }
        public Core.Application App { get; set; }

        public IList<OrderItem> OrderList { get; } = new List<OrderItem>();

        #endregion

        #region コンストラクタ

        public SearchEngine(Application app)
        {
            Logger = NLog.LogManager.GetLogger(LoggerName.Crawler);

            App = app;

            OrderList.Add(new OrderItem { Type = OrderType.SCORE, Caption = "関連度順" });
            OrderList.Add(new OrderItem { Type = OrderType.FILE_UPDATED_DESC, Caption = "更新日が新しい順" });
            OrderList.Add(new OrderItem { Type = OrderType.FILE_PATH, Caption = "ファイルパス順" });
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
            , int offset = 0
            , string selectedFormat = null
            , string selectedFolderLabel = null
            , string selectedOrderType = null
            , string selectedView = null
        )
        {
            var groongaQueries = new List<string>();
            var groongaFilters = new List<string>();
            var querySubMessages = new List<string>();

            // キーワード、ファイル名、本文での絞り込みをエスケープして追加
            if (!string.IsNullOrWhiteSpace(queryKeyword))
            {
                groongaQueries.Add(Groonga.Util.EscapeForQuery(queryKeyword));
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

            // フォルダラベルでの絞り込みを追加
            if (selectedFolderLabel != null)
            {
                groongaQueries.Add(string.Format("{0}:{1}"
                                                , Column.Documents.FOLDER_LABELS
                                                , Groonga.Util.EscapeForQuery(selectedFolderLabel)));
                querySubMessages.Add(string.Format("フォルダラベル: {0}", selectedFolderLabel));
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
                Column.Documents.FILE_NAME + "," + Column.Documents.BODY
            };

            var columns = new List<Groonga.DynamicColumn>();

            // 並び順の設定。並び順が指定されていれば、その並び順を優先
            var sortKeys = new List<string>();
            sortKeys.Add(Column.Documents.FILE_PATH);


            // SELECT実行
            var joinedQuery = string.Join(" ", groongaQueries.Select(q => "(" + q + ")"));
            var joinedFilter = string.Join(" && ", groongaFilters.Select(q => "(" + q + ")"));
            var pageSize = (selectedView == ViewType.LIST ? App.UserSettings.DisplayPageSizeForListView : App.UserSettings.DisplayPageSizeForNormalView);

            Groonga.SelectResult selectRes = null;
            try
            {
                selectRes = App.GM.Select(
                      Table.Documents
                    , query: joinedQuery
                    , filter: joinedFilter
                    , offset: offset
                    , limit: pageSize
                    //, drilldown: new[] { Column.Documents.EXT, Column.Documents.FILE_UPDATED_YEAR }
                    , drilldown: new[] { Column.Documents.EXT, Column.Documents.FOLDER_LABELS }
                    , drilldownSortKeys: new[] { Column.Documents.KEY }
                    , sortKeys: sortKeys.ToArray()
                    , matchColumns: matchColumns
                    , outputColumns: new[] {
                                Column.Documents.KEY
                            , Column.Documents.FILE_PATH
                            , Column.Documents.TITLE
                            , Column.Documents.EXT
                            , Column.Documents.BODY
                            , Column.Documents.FILE_NAME
                            , Column.Documents.FILE_UPDATED_AT
                            , Column.Documents.SIZE
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
                    file_path = (string)selectRec[Column.Documents.FILE_PATH],
                    title = (string)selectRec[Column.Documents.TITLE],
                    body = (string)selectRec[Column.Documents.BODY]
                };
                if (!string.IsNullOrEmpty(rec.file_path))
                {
                    rec.folder_path = Path.GetDirectoryName(rec.file_path);
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

                // grep結果を設定
                var engine = new GrepEngine();
                var grepResult = engine.Grep(rec.body, queryKeyword);
                rec.prism_view_range = grepResult.GetPrismViewRange();
                rec.prism_match_lines = grepResult.GetPrismMatchLines();

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
                orderList = OrderList
                ,
                pageSize = pageSize
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
