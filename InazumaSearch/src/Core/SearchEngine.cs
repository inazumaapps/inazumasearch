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
        /// 検索条件の解析結果
        /// </summary>
        public class QueryParseResult
        {
            /// <summary>
            /// Groongaに渡すクエリのリスト
            /// </summary>
            public List<string> GroongaQueries { get; set; } = new List<string>();

            /// <summary>
            /// Groongaに渡すフィルタのリスト
            /// </summary>
            public List<string> GroongaFilters { get; set; } = new List<string>();

            /// <summary>
            /// キーワード以外の絞り込み条件を表すメッセージ（検索結果画面で表示）
            /// </summary>
            public List<string> SubMessages { get; set; } = new List<string>();
        }

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
            /// フォルダパスのドリルダウン結果
            /// </summary>
            public IList<FolderPathDrilldownLink> folderPathDrilldownLinks { get; set; }

            /// <summary>
            /// フォルダラベルのドリルダウン結果
            /// </summary>
            public IList<FolderLabelDrilldownLink> folderLabelDrilldownLinks { get; set; }

            /// <summary>
            /// 並び順の選択肢一覧
            /// </summary>
            public IList<OrderItem> orderList { get; set; }

            /// <summary>
            /// 検索時間（単位：秒）
            /// </summary>
            public double processTime { get; set; }
        }

        public class Record
        {
            public string key { get; set; }
            public string folder_path { get; set; }
            public string file_path { get; set; }
            public string file_name { get; set; }
            public string[] file_name_snippets { get; set; } = new string[] { };
            public string title { get; set; }
            public string[] title_snippets { get; set; } = new string[] { };
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
        public class FolderPathDrilldownLink
        {
            public string folderPath { get; set; }
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
        /// <param name="queryKeyword">キーワード</param>
        /// <param name="queryFolderPath">フォルダパス条件</param>
        /// <param name="queryFileName">ファイル名条件</param>
        /// <param name="queryBody">本文条件</param>
        /// <param name="queryUpdated">更新日時条件</param>
        /// <param name="offset">オフセット（検索開始位置）</param>
        /// <param name="selectedFormat">ドリルダウンで選択したファイル形式</param>
        /// <param name="selectedFolderPath">ドリルダウンで選択したフォルダパス</param>
        /// <param name="selectedFolderLabel">ドリルダウンで選択したフォルダラベル</param>
        /// <param name="selectedOrderType">並び順タイプ</param>
        /// <param name="selectedView">表示形式</param>
        /// <returns>検索結果（レコードリスト）</returns>
        public Result Search(
              string queryKeyword
            , string queryFolderPath = null
            , string queryFileName = null
            , string queryBody = null
            , string queryUpdated = null
            , int offset = 0
            , string selectedFormat = null
            , string selectedFolderPath = null
            , string selectedFolderLabel = null
            , string selectedOrderType = null
            , string selectedView = null
        )
        {
            // 検索条件を解析
            var queryParseResult = parseQuery(queryKeyword, queryFolderPath, queryFileName, queryBody, queryUpdated, selectedFormat, selectedFolderPath, selectedFolderLabel);

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

            // 並び順の設定。並び順が指定されていれば、その並び順を優先
            var sortKeys = new List<string>();
            switch (selectedOrderType)
            {
                case OrderType.FILE_PATH:
                    sortKeys.Add(Column.Documents.FILE_PATH);
                    break;
                case OrderType.FILE_UPDATED_DESC:
                    sortKeys.Add("-" + Column.Documents.FILE_UPDATED_AT);
                    break;
            }
            // 標準では最終スコア(関連度+鮮度)が高い順に並べる
            sortKeys.Add("-final_score");


            // SELECT実行
            var joinedQuery = string.Join(" ", queryParseResult.GroongaQueries.Select(q => "(" + q + ")"));
            var joinedFilter = string.Join(" && ", queryParseResult.GroongaFilters.Select(q => "(" + q + ")"));
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
                    , drilldown: new[] { Column.Documents.EXT, Column.Documents.FOLDER_PATH, Column.Documents.FOLDER_LABELS }
                    , drilldownSortKeys: new[] { Groonga.VColumn.KEY }
                    , sortKeys: sortKeys.ToArray()
                    , matchColumns: matchColumns
                    , outputColumns: new[] {
                                Column.Documents.KEY
                            , Column.Documents.FOLDER_PATH
                            , Column.Documents.FILE_PATH
                            , Groonga.VColumn.SCORE // 鮮度補正を加味していない生のスコア
                            , Column.Documents.TITLE
                            , Column.Documents.EXT
                            , Column.Documents.FILE_NAME
                            , Column.Documents.FILE_UPDATED_AT
                            , Column.Documents.SIZE
                            , Groonga.Function.SnippetHtml(Column.Documents.FILE_NAME)
                            , Groonga.Function.SnippetHtml(Column.Documents.TITLE)
                            , Groonga.Function.SnippetHtml(Column.Documents.BODY)
                            , "elapsed_hour_from_file_updated"
                            , "freshness_score_rate"
                            , "final_score"
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
                    title = (string)selectRec[Column.Documents.TITLE]
                };
                if (!string.IsNullOrEmpty(rec.file_path))
                {
                    rec.folder_path = Path.GetDirectoryName(rec.file_path);
                }
                rec.base_score = selectRec.GetIntValue(Groonga.VColumn.SCORE).Value;
                rec.final_score = selectRec.GetIntValue("final_score").Value;

                var titleSnippets = selectRec[Groonga.Function.SnippetHtml(Column.Documents.TITLE)] as object[];
                if (titleSnippets != null)
                {
                    rec.title_snippets = titleSnippets.Cast<string>().ToArray();
                }

                var fileNameSnippets = selectRec[Groonga.Function.SnippetHtml(Column.Documents.FILE_NAME)] as object[];
                if (fileNameSnippets != null)
                {
                    rec.file_name_snippets = fileNameSnippets.Cast<string>().ToArray();
                }

                var bodySnippets = selectRec[Groonga.Function.SnippetHtml(Column.Documents.BODY)] as object[];
                if (bodySnippets != null)
                {
                    rec.body_snippets = bodySnippets.Cast<string>().ToArray();
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

            // ドリルダウン結果(フォーマットごとの件数)を元に、フォーマット絞り込み用のデータを作成
            var folderPathDrilldownLinks = new List<FolderPathDrilldownLink>();
            foreach (var rec in selectRes.DrilldownResults[1].Records)
            {
                var link = new FolderPathDrilldownLink()
                {
                    folderPath = (string)rec.Key
                    ,
                    nSubRecs = rec.NSubRecs
                };
                folderPathDrilldownLinks.Add(link);
            }

            var folderLabelDrilldownLinks = new List<FolderLabelDrilldownLink>();
            foreach (var rec in selectRes.DrilldownResults[2].Records)
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

            if (queryParseResult.SubMessages.Count >= 1)
            {
                searchResultSubMessage = string.Format("({0})", string.Join("、", queryParseResult.SubMessages));
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
                folderPathDrilldownLinks = folderPathDrilldownLinks.OrderByDescending(l => l.nSubRecs).ToList() // 件数の多い順で並べる
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
        /// 全レコードのフォルダパスと、フォルダパスごとのヒット件数を取得
        /// </summary>
        /// <returns>フォルダパス、ヒット件数を格納したDictionary</returns>
        public Dictionary<string, long> GetAllFolderPath()
        {
            return SearchAllFolderPath(null, null, null, null, null, null, null, null);
        }

        /// <summary>
        /// 検索結果に合致する全レコードのフォルダパスと、フォルダパスごとのヒット件数を取得
        /// </summary>
        /// <param name="queryKeyword">キーワード</param>
        /// <param name="queryFolderPath">フォルダパス条件</param>
        /// <param name="queryFileName">ファイル名条件</param>
        /// <param name="queryBody">本文条件</param>
        /// <param name="queryUpdated">更新日時条件</param>
        /// <param name="selectedFormat">ドリルダウンで選択したファイル形式</param>
        /// <param name="selectedFolderPath">ドリルダウンで選択したフォルダパス</param>
        /// <param name="selectedFolderLabel">ドリルダウンで選択したフォルダラベル</param>
        /// <returns>フォルダパス、ヒット件数を格納したDictionary</returns>
        public Dictionary<string, long> SearchAllFolderPath(
          string queryKeyword
        , string queryFolderPath
        , string queryFileName
        , string queryBody
        , string queryUpdated
        , string selectedFormat
        , string selectedFolderPath
        , string selectedFolderLabel
    )
        {
            // 検索条件を解析
            var queryParseResult = parseQuery(queryKeyword, queryFolderPath, queryFileName, queryBody, queryUpdated, selectedFormat, selectedFolderPath, selectedFolderLabel);

            // SELECT実行
            var joinedQuery = string.Join(" ", queryParseResult.GroongaQueries.Select(q => "(" + q + ")"));
            var joinedFilter = string.Join(" && ", queryParseResult.GroongaFilters.Select(q => "(" + q + ")"));
            var matchColumns = new[] {
                Column.Documents.FILE_NAME
                , Column.Documents.BODY
            };

            Groonga.SelectResult selectRes = null;
            try
            {
                selectRes = App.GM.Select(
                      Table.Documents
                    , query: joinedQuery
                    , filter: joinedFilter
                    , limit: 1
                    , drilldown: new[] { Column.Documents.FOLDER_PATH }
                    , drilldownLimit: -1
                    , matchColumns: matchColumns
                    , outputColumns: new string[] {
                        Groonga.VColumn.KEY
                    }
                );
            }
            catch (GroongaCommandError ex)
            {
                // エラー時はnullを返す（通常発生しないはず）
                Logger.Debug(ex.ToString());
                return null;
            }

            // 結果を作成
            var res = new Dictionary<string, long>();
            foreach (var rec in selectRes.DrilldownResults[0].Records)
            {
                res[(string)rec.Key] = rec.NSubRecs;
            }
            return res;

        }

        /// <summary>
        /// 検索条件を解析し、Groongaに渡せるクエリ・フィルタ構文に変換
        /// </summary>
        /// <param name="queryKeyword">キーワード</param>
        /// <param name="queryFolderPath">フォルダパス条件</param>
        /// <param name="queryFileName">ファイル名条件</param>
        /// <param name="queryBody">本文条件</param>
        /// <param name="queryUpdated">更新日時条件</param>
        /// <param name="selectedFormat">ドリルダウンで選択したファイル形式</param>
        /// <param name="selectedFolderPath">ドリルダウンで選択したフォルダパス</param>
        /// <param name="selectedFolderLabel">ドリルダウンで選択したフォルダラベル</param>
        /// <returns>解析結果</returns>
        protected QueryParseResult parseQuery(
              string queryKeyword
            , string queryFolderPath
            , string queryFileName
            , string queryBody
            , string queryUpdated
            , string selectedFormat
            , string selectedFolderPath
            , string selectedFolderLabel
        )
        {
            var res = new QueryParseResult();

            // キーワード、ファイル名、本文での絞り込みをエスケープして追加
            if (!string.IsNullOrWhiteSpace(queryKeyword))
            {
                // Groongaのクエリ構文は一部のみ有効
                // バックスラッシュ、シングルクォート、コロンなどの特殊記号はエスケープする
                // 有効なのは " - ( ) のみ
                res.GroongaQueries.Add(queryKeyword
                                    .Replace(@"\", @"\\")
                                    .Replace("'", @"\'")
                                    .Replace(":", @"\:")
                                    .Replace("+", @"\+"));
            }
            if (!string.IsNullOrWhiteSpace(queryFolderPath))
            {
                // 最後に\マークが付いた状態にする
                var folderPath = queryFolderPath.Trim('\\') + "\\";
                res.GroongaQueries.Add(string.Format("{0}:^{1}"
                                                , Column.Documents.FOLDER_PATH
                                                , Groonga.Util.EscapeForQuery(folderPath)));
                res.SubMessages.Add(string.Format("フォルダパス: {0}", queryFolderPath));
            }
            if (!string.IsNullOrWhiteSpace(queryFileName))
            {
                res.GroongaQueries.Add(string.Format("{0}:@{1}"
                                                , Column.Documents.FILE_NAME
                                                , Groonga.Util.EscapeForQuery(queryFileName)));
                res.SubMessages.Add(string.Format("ファイル名: 「{0}」", queryFileName));
            }
            if (!string.IsNullOrWhiteSpace(queryBody))
            {
                res.GroongaQueries.Add(string.Format("{0}:@{1}"
                                                , Column.Documents.BODY
                                                , Groonga.Util.EscapeForQuery(queryBody)));
                res.SubMessages.Add(string.Format("本文: 「{0}」", queryBody));
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
                    res.GroongaFilters.Add(string.Format("!in_values({0},{1})"
                                                    , Column.Documents.EXT
                                                    , string.Join(", ", strings))
                                        );
                    res.SubMessages.Add("ファイル形式: その他");


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
                    res.GroongaFilters.Add(string.Format("in_values({0},{1})"
                                                    , Column.Documents.EXT
                                                    , string.Join(", ", strings))
                                        );
                    res.SubMessages.Add(string.Format("ファイル形式: {0}", selectedFormat));

                }
            }

            // フォルダパスでの絞り込みを追加
            if (selectedFolderPath != null)
            {
                res.GroongaQueries.Add(string.Format("{0}:{1}"
                                                , Column.Documents.FOLDER_PATH
                                                , Groonga.Util.EscapeForQuery(selectedFolderPath)));
                res.SubMessages.Add(string.Format("フォルダパス: {0}", selectedFolderPath));
            }

            // フォルダラベルでの絞り込みを追加
            if (selectedFolderLabel != null)
            {
                res.GroongaQueries.Add(string.Format("{0}:{1}"
                                                , Column.Documents.FOLDER_LABELS
                                                , Groonga.Util.EscapeForQuery(selectedFolderLabel)));
                res.SubMessages.Add(string.Format("フォルダラベル: {0}", selectedFolderLabel));
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
                res.GroongaFilters.Add(string.Format("file_updated_at > \"{0}\"", Groonga.Util.ToExprTimeFormat(updatedLeft.Value)));
                res.SubMessages.Add(string.Format("更新日付: {0}", updatedRangeCaption));
            }

            return res;
        }

        #endregion


    }
}
