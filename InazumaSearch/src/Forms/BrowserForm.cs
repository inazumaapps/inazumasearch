using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using Alphaleonis.Win32.Filesystem;
using CefSharp;
using CefSharp.WinForms;
using InazumaSearch.Core;
using InazumaSearch.Core.Crawl;
using Microsoft.WindowsAPICodePack.Dialogs;
using Newtonsoft.Json;

namespace InazumaSearch.Forms
{
    public partial class BrowserForm : Form
    {
        public ChromiumWebBrowser ChromeBrowser { get; set; }
        public Core.Application App { get; set; }
        public DBStateApi DBState { get; set; }
        public CefApi Api { get; set; }
        public CefAsyncApi AsyncApi { get; set; }

        protected int AlwaysCrawlProgressTick { get; set; } = 0;
        protected const int AlwaysCrawlProgressTickEnd = 30;
        protected DateTime? LastUpdatedCountDataInSettingPage { get; set; } = null;

        public class DBStateApi
        {
            public long DocumentCount { get; set; }
            public long TargetFolderCount { get; set; }
            public bool AlwaysCrawlMode { get; set; }
        }

        /// <summary>
        /// JavaScript内からアクセス可能なAPIオブジェクト
        /// </summary>
        public class CefApi
        {
            public BrowserForm OwnerForm { get; set; }
            public Core.Application App { get; set; }

            public CefApi()
            {
            }

            public bool IsDebugMode()
            {
                return App.DebugMode;
            }

            public bool IsPortableMode()
            {
                return ApplicationEnvironment.IsPortableMode();
            }

            public bool IsUpdateCheckFailed()
            {
                return !ISAutoUpdater.UpdateCheckFinished;
            }

            public string GetUserSettings()
            {
                return JsonConvert.SerializeObject(App.UserSettings.PlainData);
            }

            public string GetVersionCaption()
            {
                return ApplicationEnvironment.GetVersionCaption();
            }

            /// <summary>
            /// 最終クロール日時を表す文字列を取得
            /// </summary>
            /// <returns></returns>
            public string GetLastCrawlTimeCaption()
            {
                if (App.UserSettings.AlwaysCrawlMode)
                {
                    return "常駐クロール実行中";

                }
                else
                {
                    var lastCrawlTime = App.UserSettings.LastCrawlTime;
                    if (lastCrawlTime != null)
                    {
                        return "最終実行: " + lastCrawlTime.Value.ToString("yyyy/MM/dd HH:mm");
                    }
                }

                return "";
            }

            public void ShowErrorMessage(string message)
            {
                OwnerForm.InvokeOnUIThread((f) => Util.ShowErrorMessage(f,
                    message
                ));
            }


            public void OpenFile(string path)
            {
                OwnerForm.InvokeOnUIThread((f) => f.OpenFileByResultSelect(path));
            }


            public void OpenFolder(string path)
            {
                OwnerForm.InvokeOnUIThread((f) => f.OpenFolder(path));
            }

            /// <summary>
            /// 外部リンクを標準ブラウザで開く
            /// </summary>
            /// <param name="url"></param>
            public void OpenExternalLink(string url)
            {
                Process.Start(url);
            }

            public void OpenDipswForm()
            {
                OwnerForm.InvokeOnUIThread((owner) =>
                {
                    var f = new DipswForm(App);
                    f.ShowDialog(owner);
                });

            }



            public void CrawlStart(string targetFoldersJSON = null)
            {
                string[] targetFolders = null;
                if (targetFoldersJSON != null)
                {
                    targetFolders = JsonConvert.DeserializeObject<string[]>(targetFoldersJSON);
                }
                OwnerForm.InvokeOnUIThread((f) => { f.CrawlStart(targetFolders); });
            }

            public string SelectDirectory()
            {
                string path = null;
                OwnerForm.InvokeOnUIThread((f) =>
                {
                    path = f.SelectDirectory();
                });
                return path;
            }

            /// <summary>
            /// 検索対象フォルダを追加する
            /// </summary>
            /// <returns>現在の文書数</returns>
            public string AddTargetDirectory(string path)
            {
                return App.ExecuteInExceptionCatcher(() =>
                {
                    var folders = App.UserSettings.TargetFolders;

                    // 同じフォルダパスが登録されていればエラー
                    if (folders.Any(f => f.Path == path))
                    {
                        ShowErrorMessage("すでに登録済みのフォルダです。");
                        return null;
                    }

                    // 同じフォルダパスが存在しなければ追加
                    folders.Add(new UserSetting.TargetFolder() { Path = path, Type = UserSetting.TargetFolderType.DocumentFile });
                    App.UserSettings.SaveTargetFolders(folders);

                    // 常駐クロール実行中であれば再起動
                    App.RestartAlwaysCrawlIfRunning(OwnerForm);

                    // すでに登録されているファイル数をカウント
                    var res = App.GM.Select(
                        Table.Documents
                        , limit: 0
                        , outputColumns: new string[] { Groonga.VColumn.ID }
                        , filter: $"{Column.Documents.KEY} @^ {Groonga.Util.EscapeForScript(Util.MakeDocumentDirKeyPrefix(path))}"
                    );
                    return JsonConvert.SerializeObject(new { fileCount = res.SearchResult.NHits, pathHash = Util.HexDigest(App.HashProvider, path) });
                });

            }

            /// <summary>
            /// 検索対象フォルダを削除する
            /// </summary>
            public void DeleteTargetDirectory(string path)
            {
                App.ExecuteInExceptionCatcher(() =>
                {

                    var folders = App.UserSettings.TargetFolders;
                    folders.Remove(folders.First(f => f.Path == path));
                    App.UserSettings.SaveTargetFolders(folders);

                    // 常駐クロール実行中であれば再起動
                    App.RestartAlwaysCrawlIfRunning(OwnerForm);
                });
            }

            public void ShowIgnoreEditFormFromSearchResult(string path)
            {
                // 無視設定フォームを開く
                OwnerForm.InvokeOnUIThread((form) =>
                {
                    // 登録された対象フォルダのリストを取得し、パスが長い順に並べておく
                    var folders = App.UserSettings.TargetFolders.OrderByDescending(folder => folder.Path.Length);

                    // クリックしたファイルについて、どの対象フォルダに存在するかを特定（最もパスが長い＝階層が深いものを優先）
                    var dirPath = Path.GetDirectoryName(path);
                    var baseDirPath = folders.First(folder => dirPath.ToLower().StartsWith(folder.Path.ToLower())).Path;

                    // 無視設定ダイアログを開く
                    var relPath = path.Substring(baseDirPath.Length + 1);
                    var defaultPattern = (relPath.Contains(@"\") ? relPath : $@"\{relPath}");
                    var dialog = new IgnoreEditForm(IgnoreEditForm.EditMode.APPEND, baseDirPath, defaultPattern, App);
                    var res = dialog.ShowDialog(form);
                    if (res == DialogResult.OK)
                    {
                        // 無視設定を更新した場合、常駐クロール実行中であれば再起動
                        App.RestartAlwaysCrawlIfRunning(OwnerForm);
                    }
                });

            }

            public void ShowIgnoreEditFormFromSetting(string dirPath)
            {
                // 無視設定フォームを開く
                OwnerForm.InvokeOnUIThread((form) =>
                {
                    // 無視設定ダイアログを開く
                    var dialog = new IgnoreEditForm(IgnoreEditForm.EditMode.UPDATE, dirPath, "", App);
                    var res = dialog.ShowDialog(form);
                    if (res == DialogResult.OK)
                    {
                        // 無視設定を更新した場合、常駐クロール実行中であれば再起動
                        App.RestartAlwaysCrawlIfRunning(OwnerForm);
                    }
                });
            }

            public void ShowUpdateForm()
            {
                // 無視設定フォームを開く
                OwnerForm.InvokeOnUIThread((form) =>
                {
                    ISAutoUpdater.ShowUpdateForm();
                });

            }

            public void UpdateFolderLabel(string path, string label)
            {
                // 既存データのラベル値を更新
                OwnerForm.InvokeOnUIThread((f) =>
                {
                    // クロール中の場合は停止してから処理
                    App.InvokeWithProgressFormWithoutAlwaysCrawl(f, "DBのラベル情報を更新中...", () =>
                    {
                        var folders = App.UserSettings.TargetFolders;
                        var folder = folders.First(f2 => f2.Path == path);
                        folder.Label = label;

                        App.UserSettings.Save();
                        App.UpdateDocumentFolderLabels();
                    });
                });
            }
        }

        /// <summary>
        /// JavaScript内からアクセス可能な非同期処理用APIオブジェクト
        /// </summary>
        public class CefAsyncApi
        {

            public ChromiumWebBrowser Browser { get; set; }
            public BrowserForm OwnerForm { get; set; }
            public Core.Application App { get; set; }

            public IList<Tuple<DateTime, string>> UserInputLogs { get; protected set; }

            public CefAsyncApi(ChromiumWebBrowser browser)
            {
                Browser = browser;
                UserInputLogs = new List<Tuple<DateTime, string>>();
            }

            public string SearchTargetDirectories(string order = null)
            {
                return App.ExecuteInExceptionCatcher<string>(() =>
                {
                    var targetDirectories = new List<UserSetting.TargetFolder>();
                    var pathHashes = new Dictionary<string, string>();
                    var fileCounts = new Dictionary<string, long>();
                    var ignoreSettingCounts = new Dictionary<string, long>();
                    var excludingFlags = new Dictionary<string, bool>();

                    // フォルダ設定ごとにループ
                    // 処理順は引数により変える
                    IEnumerable<UserSetting.TargetFolder> folders;
                    if (order == "crawlFolderSelect" && App.UserSettings.LastExcludingDirPaths != null)
                    {
                        // クロール時のフォルダ選択で、前回クロール時のフォルダ情報が存在する場合は、
                        // 前回クロール時に除外しているフォルダを後ろに回す
                        folders = App.UserSettings.TargetFolders.OrderBy(f => Tuple.Create((App.UserSettings.LastExcludingDirPaths.Contains(f.Path) ? 1 : 0), f.Path));
                    }
                    else
                    {
                        folders = App.UserSettings.TargetFolders.OrderBy(f => f.Path);
                    }

                    foreach (var folder in folders)
                    {
                        targetDirectories.Add(folder);

                        // 収集済みのファイル数をカウント
                        var res = App.GM.Select(
                            Table.Documents
                            , limit: 0
                            , outputColumns: new string[] { Groonga.VColumn.ID }
                            , filter: $"{Column.Documents.KEY} @^ {Groonga.Util.EscapeForScript(Util.MakeDocumentDirKeyPrefix(folder.Path))}"
                        );
                        pathHashes[folder.Path] = Util.HexDigest(App.HashProvider, folder.Path);
                        fileCounts[folder.Path] = res.SearchResult.NHits;
                        ignoreSettingCounts[folder.Path] = folder.IgnoreSettingLines.Count;
                        excludingFlags[folder.Path] = (App.UserSettings.LastExcludingDirPaths != null && App.UserSettings.LastExcludingDirPaths.Contains(folder.Path));
                    }

                    return JsonConvert.SerializeObject(new { targetDirectories, pathHashes, fileCounts, ignoreSettingCounts, excludingFlags });
                });
            }

            public string GetSimilarDocuments(string key)
            {
                return App.ExecuteInExceptionCatcher<string>(() =>
                {
                    var searchEngine = new SearchEngine(App);
                    return JsonConvert.SerializeObject(searchEngine.SearchSimilarDocuments(key));
                });
            }

            public string GetHighlightedBody(string key, string queryKeyword, string queryBody)
            {
                return App.ExecuteInExceptionCatcher<string>(() =>
                {
                    var groongaQueries = new List<string>();
                    var groongaFilters = new List<string>();

                    if (!string.IsNullOrWhiteSpace(queryKeyword))
                    {
                        // Groongaのクエリ構文有効。ただしバックスラッシュとシングルクォートはエスケープする
                        groongaQueries.Add(queryKeyword.Replace(@"\", @"\\").Replace("'", @"\'"));
                    }
                    if (!string.IsNullOrWhiteSpace(queryBody))
                    {
                        groongaQueries.Add(string.Format("{0}:@{1}"
                                                       , Column.Documents.BODY
                                                       , Groonga.Util.EscapeForQuery(queryBody)));
                    }
                    groongaFilters.Add(string.Format("{0} == {1}", Column.Documents.KEY, Groonga.Util.EscapeForScript(key)));

                    // SELECT実行
                    var joinedQuery = string.Join(" ", groongaQueries.Select(q => "(" + q + ")"));
                    var joinedFilter = string.Join(" && ", groongaFilters.Select(q => "(" + q + ")"));

                    Groonga.SelectResult selectRes = null;
                    var expr = "highlight_html(body)";
                    selectRes = App.GM.Select(
                          Table.Documents
                        , query: joinedQuery
                        , filter: joinedFilter
                        , matchColumns: new[] { "body" }
                        , outputColumns: new[] { expr, Groonga.VColumn.SCORE }
                        , limit: 1
                    );
                    var searchResult = selectRes.SearchResult;
                    if (searchResult.NHits >= 1)
                    {
                        // ハイライトHTMLを取得
                        var html = searchResult.Records[0].GetTextValue(expr);

                        // 行単位で分割し、「ハイライトを含む行数」の前後N行を取得
                        var lines = html.Split(new[] { "\r\n" }, StringSplitOptions.None).ToList();
                        var segments = new List<Tuple<int, int>>();
                        int? firstMatchInSegment = null;
                        int? lastMatchInSegment = null;
                        var segmentRange = 3;

                        for (var i = 0; i < lines.Count; i++)
                        {
                            var line = lines[i];
                            if (line.Contains("<span class=\"keyword\">"))
                            {
                                firstMatchInSegment = firstMatchInSegment ?? i; // セグメントがまだ開始していなければ開始
                                lastMatchInSegment = i;
                            }

                            // 最後にハイライトが含まれていた行から、規定行数以上離れていれば、セグメント確定
                            if (lastMatchInSegment != null && i > lastMatchInSegment + segmentRange)
                            {
                                var start = firstMatchInSegment.Value - segmentRange;
                                if (start < 0) start = 0;
                                var end = lastMatchInSegment.Value + segmentRange;
                                segments.Add(Tuple.Create(start, end));

                                // マッチ情報初期化
                                lastMatchInSegment = null;
                                firstMatchInSegment = null;
                            }
                        }

                        // ループ終了時に終了していない情報があれば、同様にセグメント確定
                        if (lastMatchInSegment != null)
                        {
                            var start = firstMatchInSegment.Value - segmentRange;
                            if (start < 0) start = 0;
                            var end = lines.Count - 1;
                            segments.Add(Tuple.Create(start, end));
                        }

                        // ハイライトHTMLの抜粋を生成
                        var outLines = new List<string>();
                        foreach (var segment in segments)
                        {
                            outLines.Add("<div style=\"border: 1px solid silver; margin: 0; padding: 1em; font-size: small; overflow-y: auto;\">");
                            outLines.AddRange(lines.GetRange(segment.Item1, segment.Item2 - segment.Item1 + 1));
                            outLines.Add("</div>");
                        }

                        // ハイライトされたHTML
                        var res = new { body = string.Join("\r\n", outLines), hitCount = searchResult.Records[0].GetIntValue(Groonga.VColumn.SCORE) };
                        return JsonConvert.SerializeObject(res);
                    }
                    else
                    {
                        return JsonConvert.SerializeObject(null);
                    }
                });
            }

            public void GetFileBody(string filePath)
            {
                OwnerForm.InvokeOnUIThread((f) =>
                {
                    App.ExecuteInExceptionCatcher(() =>
                    {
                        // ファイル本文の抽出
                        var textExtNames = App.GetTextExtNames();
                        var pluginExtNames = App.GetPluginExtNames();
                        var extRes = App.ExtractFile(filePath, textExtNames, pluginExtNames);

                        var dialog = new FileBodyViewDialog
                        {
                            Body = extRes.Body
                        };
                        dialog.ShowDialog(f);
                    });
                });

            }

            /// <summary>
            /// 学習のために入力ログを保持
            /// </summary>
            public void AddUserInputLog(DateTime timestamp, string text)
            {
                UserInputLogs.Add(Tuple.Create(timestamp, text));
            }

            /// <summary>
            /// 入力ログをクリア (ページ切り替え時などに使用)
            /// </summary>
            public void ClearUserInputLog()
            {
                UserInputLogs.Clear();
            }

            /// <summary>
            /// 全文検索の実行
            /// </summary>
            /// <param name="query"></param>
            /// <param name="offset"></param>
            /// <returns></returns>
            public string Search(
                IDictionary<string, object> queryObject
                , bool learning = false
                , int offset = 0
                , string selectedFormat = null
                , string selectedFolderLabel = null
                , string selectedOrder = null
                , string selectedView = null
                , bool ignoreError = false
            )
            {
                return App.ExecuteInExceptionCatcher<string>(() =>
                {

                    // 学習
                    if (learning)
                    {
                        var learnData = new List<Dictionary<string, object>>();
                        foreach (var log in UserInputLogs)
                        {
                            var data = new Dictionary<string, object>
                            {
                                ["sequence"] = "1",
                                ["time"] = Groonga.Util.ToUnixTime(log.Item1),
                                ["item"] = log.Item2
                            };
                            learnData.Add(data);
                        }
                        var submitData = new Dictionary<string, object>
                        {
                            ["sequence"] = "1",
                            ["time"] = Groonga.Util.ToUnixTime(DateTime.Now),
                            ["item"] = (string)queryObject["keyword"],
                            ["type"] = "submit"
                        };
                        learnData.Add(submitData);
                        App.GM.Load(table: "event_query", each: "suggest_preparer(_id, type, item, sequence, time, pair_query)", values: learnData);
                        UserInputLogs.Clear();
                    }

                    // 処理時間の計測を開始
                    var sw = Stopwatch.StartNew();

                    // 全文検索の実行
                    var queryKeyword = (string)queryObject["keyword"];
                    var queryFileName = (string)queryObject["fileName"];
                    var queryBody = (string)queryObject["body"];
                    var queryUpdated = (string)queryObject["updated"];

                    var searchEngine = new SearchEngine(App);
                    var ret = searchEngine.Search(
                        queryKeyword
                        , queryFileName
                        , queryBody
                        , queryUpdated
                        , offset
                        , selectedFormat
                        , selectedFolderLabel
                        , selectedOrder
                        , selectedView
                    );

                    // 失敗した場合はエラーダイアログを表示して終了
                    if (!ret.success)
                    {
                        if (!ignoreError)
                        {
                            OwnerForm.InvokeOnUIThread((f) => Util.ShowErrorMessage(f,
                                "検索語の解析時にエラーが発生しました。\n単語をダブルクォート (\") で囲んで試してみてください。"
                            ));
                        }

                        return null;
                    }

                    // 処理時間を結果オブジェクトにセット
                    ret.processTime = sw.Elapsed.TotalSeconds;

                    // 結果の返却
                    return JsonConvert.SerializeObject(ret);
                });

            }

            public string GetAutoCompleteData(string val)
            {
                return App.ExecuteInExceptionCatcher<string>(() =>
                {

                    var res = new Dictionary<string, object>();
                    if (string.IsNullOrWhiteSpace(val))
                    {
                        return JsonConvert.SerializeObject(res);
                    }

                    var recordSet = App.GM.Suggest(types: new[] { "complete", "correct", "suggest" }, table: "item_query", column: "kana", query: val, frequencyThreshold: 1);
                    foreach (var rec in recordSet.Records)
                    {
                        res[(string)rec.Key] = null;
                    }

                    return JsonConvert.SerializeObject(res);

                });
            }

            /// <summary>
            /// 常駐クロール設定の変更
            /// </summary>
            /// <param name="checked"></param>
            public void ChangeAlwaysCrawlMode(bool @checked)
            {
                OwnerForm.InvokeOnUIThread((f) =>
                {
                    App.ChangeAlwaysCrawlMode(f, @checked);
                    if (@checked)
                    {
                        Util.ShowInformationMessage(f, "常駐クロールを開始しました。\nウインドウを閉じた後も、クロール処理を継続します。\n\nInazuma Searchを完全に終了したい場合は\nタスクバー内通知エリアのアイコンを右クリックして\n「終了」を選択してください。");
                    }
                });

            }

            /// <summary>
            /// 「Windowsログイン時に自動で起動」設定の変更
            /// </summary>
            /// <param name="checked"></param>
            public void ChangeStartUp(bool @checked)
            {
                OwnerForm.InvokeOnUIThread((f) =>
                {
                    App.ChangeStartUp(f, @checked);
                });
            }

            /// <summary>
            /// 検索処理時間表示ON/OFFの変更
            /// </summary>
            /// <param name="checked">変更後の値</param>
            public void ChangeDisplaySearchProcessTime(bool @checked)
            {
                App.UserSettings.SaveDisplaySearchProcessTime(@checked);
            }


        }

        public void InitializeChromium(string htmlDirPath)
        {
            if (!Cef.IsInitialized)
            {
                var settings = new CefSettings
                {
                    Locale = "ja-JP"
                };

                // レンダリングを最適化
                // from <https://teratail.com/questions/136022>
                settings.SetOffScreenRenderingBestPerformanceArgs();

                // Initialize cef with the provided settings
                Cef.Initialize(settings);
            }

            // Create a browser component
            ChromeBrowser = new ChromiumWebBrowser(Path.Combine(htmlDirPath, "index.html"));
            ChromeBrowser.BrowserSettings.AcceptLanguageList = "ja-JP";
            BrowserPanel.Controls.Add(ChromeBrowser);

            Api = new CefApi();
            ChromeBrowser.JavascriptObjectRepository.Register("api", Api, isAsync: false);
            AsyncApi = new CefAsyncApi(ChromeBrowser);
            ChromeBrowser.JavascriptObjectRepository.Register("asyncApi", AsyncApi, isAsync: true);
            DBState = new DBStateApi();
            ChromeBrowser.JavascriptObjectRepository.Register("dbState", DBState, isAsync: false);

            ChromeBrowser.IsBrowserInitializedChanged += ChromeBrowser_IsBrowserInitializedChanged;
            ChromeBrowser.FrameLoadStart += ChromeBrowser_FrameLoadStart;
        }

        private void ChromeBrowser_FrameLoadStart(object sender, FrameLoadStartEventArgs e)
        {
            if (e.Url.EndsWith("index.html"))
            {
                var selectRes1 = App.GM.Select(Table.Documents
                    , limit: 0
                    , outputColumns: new[] { Groonga.VColumn.ID }
                );

                DBState.DocumentCount = selectRes1.SearchResult.NHits;
                DBState.TargetFolderCount = App.UserSettings.TargetFolders.Count;
                DBState.AlwaysCrawlMode = App.UserSettings.AlwaysCrawlMode;

                // 更新の有無をチェック
                ISAutoUpdater.Check(ApplicationEnvironment.IsPortableMode(), (args) =>
                {
                    var msg = $"新しいバージョン ({args.CurrentVersion.TrimEnd('0').TrimEnd('.')}) に更新可能です";
                    ChromeBrowser.EvaluateScriptAsync($"$('#UPDATE-LINK .message').text('{msg}'); $('#UPDATE-LINK').show();");
                });
            }
        }

        private void ChromeBrowser_IsBrowserInitializedChanged(object sender, EventArgs e)
        {
            //chromeBrowser.ShowDevTools();
            Api.App = App;
            Api.OwnerForm = this;
            AsyncApi.App = App;
            AsyncApi.OwnerForm = this;
            ChromeBrowser.MenuHandler = new MenuHandler(this, App);

            // フォルダ選択ダイアログの初期パスを設定
            //DlgDirectorySelect.SelectedPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        }

        internal class MenuHandler : IContextMenuHandler
        {
            public BrowserForm OwnerForm { get; set; }
            public Core.Application Application { get; set; }
            private const int ShowDevTools = 26501;
            private const int ShowDBBrowser = 26505;
            private const int ShowFolderSelect = 26509;
            private const int GroongaDebug = 26506;
            private const int ShowDebugForm = 26502;
            private const int OpenFile = 26503;
            private const int OpenFolder = 26504;

            public MenuHandler(BrowserForm ownerForm, Core.Application app)
            {
                OwnerForm = ownerForm;
                Application = app;
            }

            void IContextMenuHandler.OnBeforeContextMenu(IWebBrowser browserControl, IBrowser browser, IFrame frame, IContextMenuParams parameters, IMenuModel model)
            {
                model.Clear();
                if (parameters.TypeFlags.HasFlag(ContextMenuType.Link)
                    && new Uri(parameters.LinkUrl).Fragment.StartsWith("#FILE:"))
                {
                    model.AddItem((CefMenuCommand)OpenFile, "開く");
                    model.AddSeparator();
                    model.AddItem((CefMenuCommand)OpenFolder, "フォルダを開く");

                }
                else
                {
                    // テキストボックス選択時のメニュー
                    if (parameters.TypeFlags.HasFlag(ContextMenuType.Editable))
                    {
                        model.AddItem(CefMenuCommand.Copy, "コピー(&C)");
                        model.AddItem(CefMenuCommand.Cut, "切り取り(&T)");
                        model.AddItem(CefMenuCommand.Paste, "貼り付け(&P)");
                        model.AddSeparator();
                        model.AddItem(CefMenuCommand.SelectAll, "すべて選択(&A)");
                    }
                    else if (parameters.TypeFlags.HasFlag(ContextMenuType.Selection))
                    {
                        // テキスト選択時のメニュー
                        model.AddItem(CefMenuCommand.Copy, "コピー(&C)");
                    }
                    else if (Application.DebugMode)
                    {
                        model.AddItem((CefMenuCommand)ShowDevTools, "開発ツール");
                        model.AddItem((CefMenuCommand)ShowDBBrowser, "DBブラウザー(β版)");
                        model.AddItem((CefMenuCommand)ShowFolderSelect, "フォルダ選択");
                        model.AddItem((CefMenuCommand)GroongaDebug, "Groongaコマンド実行");
                        model.AddItem((CefMenuCommand)ShowDebugForm, "デバッグウインドウを開く");
                        model.AddSeparator();
                        model.AddItem(CefMenuCommand.ReloadNoCache, "更新");
                    }
                }
            }

            bool IContextMenuHandler.OnContextMenuCommand(IWebBrowser browserControl, IBrowser browser, IFrame frame, IContextMenuParams parameters, CefMenuCommand commandId, CefEventFlags eventFlags)
            {
                return Application.ExecuteInExceptionCatcher<bool>(() =>
                {
                    string filePath = null;
                    if (parameters.TypeFlags.HasFlag(ContextMenuType.Link)
                        && new Uri(parameters.LinkUrl).Fragment.StartsWith("#FILE:"))
                    {
                        filePath = Uri.UnescapeDataString(new Uri(parameters.LinkUrl).Fragment.Replace("#FILE:", ""));
                    }

                    switch ((int)commandId)
                    {
                        case ShowDevTools:
                            break;

                        case ShowDebugForm:
                            break;

                        case OpenFile:
                            OwnerForm.OpenFileByResultSelect(filePath);
                            break;

                        case OpenFolder:
                            Process.Start(Path.GetDirectoryName(filePath));
                            break;
                    }

                    if ((int)commandId == ShowDevTools)
                    {
                        browser.ShowDevTools();
                    }

                    if ((int)commandId == ShowDBBrowser)
                    {
                        OwnerForm.InvokeOnUIThread((f) =>
                        {
                            var f2 = new DBBrowserForm(Application);
                            f2.Show(f);
                        });
                    }

                    if ((int)commandId == ShowFolderSelect)
                    {
                        OwnerForm.InvokeOnUIThread((f) =>
                        {
                            var f2 = new SearchFolderSelectForm(Application);
                            f2.Show(f);
                        });
                    }

                    if ((int)commandId == GroongaDebug)
                    {
                        OwnerForm.InvokeOnUIThread((f) =>
                        {
                            var f2 = new GroongaDebugForm() { GM = Application.GM };
                            f2.Show(f);
                        });
                    }

                    if ((int)commandId == ShowDebugForm)
                    {
                        OwnerForm.InvokeOnUIThread((f) =>
                        {
                            var f2 = new DebugForm(Application);
                            f2.Show(f);
                        });


                    }

                    if ((int)commandId == OpenFile)
                    {
                    }
                    if ((int)commandId == OpenFolder)
                    {

                    }

                    return false;
                });
            }

            void IContextMenuHandler.OnContextMenuDismissed(IWebBrowser browserControl, IBrowser browser, IFrame frame)
            {

            }

            bool IContextMenuHandler.RunContextMenu(IWebBrowser browserControl, IBrowser browser, IFrame frame, IContextMenuParams parameters, IMenuModel model, IRunContextMenuCallback callback)
            {
                return false;
            }
        }

        public BrowserForm(string htmlDirPath)
        {
            InitializeComponent();

            // Start the browser after initialize global component
            InitializeChromium(htmlDirPath);
        }


        protected virtual void CrawlStart(IEnumerable<string> targetDirPaths = null)
        {
            // 検索対象フォルダが存在するかどうかをチェック
            foreach (var dirPath in App.GetCrawlTargetDirPaths(targetDirPaths))
            {
                if (!Directory.Exists(dirPath))
                {
                    Util.ShowErrorMessage(this, $"下記の検索対象フォルダが見つかりませんでした。\n{dirPath}\n\n検索対象フォルダの設定を変更してから、再度クロールを行ってください。");
                    return;
                }
            }

            // 実行前にDBのサイズを取得
            var dbFileSize = App.GM.GetDBFileSizeTotal();

            // ルートパスを取得 
            var rootPath = Path.GetPathRoot(Path.GetFullPath(App.GM.DBDirPath));

            // ルートパスがアルファベットから始まっていれば、ローカルにDBがあるとみなし、空き容量チェック
            var driveLetter = rootPath.Substring(0, 1);
            if (Regex.IsMatch(driveLetter, "[a-zA-Z]"))
            {
                // ドライブの空き容量を取得 
                var driveFreeSpaceSize = new DriveInfo(driveLetter).AvailableFreeSpace;

                // ドライブの空き容量(DBサイズ分を除く)が2GB未満の場合は警告
                if ((driveFreeSpaceSize - dbFileSize) < 1024L * 1024L * 1024L * 2L)
                {
                    var freeSpaceByGB = Math.Round(driveFreeSpaceSize / (decimal)(1024L * 1024L * 1024L), 2);
                    var freeSpaceCaption = freeSpaceByGB.ToString() + "GB";
                    var msg = $"{driveLetter.ToUpper()}ドライブの空き容量が残り少なくなっているため\nクロールによってDBのサイズが拡大し、空き容量が無くなる可能性があります。\n(現在の空き容量: {freeSpaceCaption})\n\nクロールを実行してもよろしいですか？";
                    var res = MessageBox.Show(this, msg, "警告", MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2);
                    if (res == DialogResult.No)
                    {
                        return;
                    }
                }
            }

            ChromeBrowser.EvaluateScriptAsync("$('#CRAWL-START').addClass('disabled'); $('#SETTING-LINK').addClass('disabled'); refreshLastCrawlTimeCaption();");

            var f = new CrawlProgressForm(App, cancelRequestingCallback: () =>
            {
                ChromeBrowser.EvaluateScriptAsync("displayCrawlInterruptingMessageIfTakeLongTime();");
            }, stoppedCallback: () =>
            {
                ChromeBrowser.EvaluateScriptAsync("$('#CRAWL-START').removeClass('disabled'); $('#SETTING-LINK').removeClass('disabled'); refreshLastCrawlTimeCaption();");
            })
            {
                TargetDirPaths = targetDirPaths
            };
            f.Show(this);
        }


        public virtual string SelectDirectory()
        {
            using (var dialog = new CommonOpenFileDialog())
            {
                dialog.IsFolderPicker = true;
                var ret = dialog.ShowDialog();

                if (ret == CommonFileDialogResult.Ok)
                {
                    return dialog.FileName;
                }
            }
            return null;
            //using (var dialog = new DirectoryAddDialog())
            //{
            //    if (dialog.ShowDialog(this) == DialogResult.OK)
            //    {
            //        return dialog.InputDirectoryPath;
            //    }
            //}

            //if (DlgDirectorySelect.ShowDialog(this) == DialogResult.OK)
            //{
            //    ret = DlgDirectorySelect.SelectedPath;
            //}

            //return null;
        }

        /// <summary>
        /// 指定したファイルを開き、同時に対応するレコードの最終選択日付を更新
        /// </summary>
        public virtual void OpenFileByResultSelect(string path)
        {
            if (!File.Exists(path))
            {
                Util.ShowErrorMessage(this,
                    "ファイルが存在しません。\n前回のクロール後に、移動または削除された可能性があるため、再度クロールを実行してください。"
                );
                return;
            }

            // ファイルパスがWindowsの標準最大長を超えている場合は確認
            if (path.Length > SystemConst.WindowsMaxPath)
            {
                if (!Util.Confirm($"ファイルパスが{SystemConst.WindowsMaxPath}文字を越えているため、このファイルを正しく開けない可能性があります。\n（エクスプローラを含む多くのアプリケーションでは、{SystemConst.WindowsMaxPath}文字を超えるパスのファイルを正しく扱えません）\n\n開いてもよろしいですか？"))
                {
                    return;
                }
            }

            // ファイルを開く
            // 例外発生時はエラーダイアログ表示
            try
            {
                Process.Start(path);
            }
            catch (Exception ex)
            {
                App.Logger.Error(ex);
                Util.ShowErrorMessage(this, $"下記のエラーにより、ファイルを開くことができませんでした。\n\n{ex.Message}");
                return;
            }

            var valueDict = new Dictionary<string, object>
            {
                [Column.Documents.KEY] = Core.Util.MakeDocumentFileKey(path),
                [Column.Documents.RESULT_SELECTED_AT] = Groonga.Util.ToUnixTime(DateTime.Now)
            };
            App.GM.Load(table: Table.Documents, values: new[] { valueDict });
        }

        public void OpenFolder(string path)
        {
            if (!File.Exists(path))
            {
                Util.ShowErrorMessage(this,
                    "ファイルが存在しません。\n前回のクロール後に、移動または削除された可能性があるため、再度クロールを実行してください。"
                );
                return;
            }
            Process.Start("explorer.exe", $"/select,\"{path}\"");
        }

        public virtual void InvokeOnUIThread(Action<BrowserForm> act)
        {
            // UI側スレッドで処理実行

            Invoke((MethodInvoker)delegate
            {
                App.ExecuteInExceptionCatcher(() =>
                {
                    act.Invoke(this);
                });
            });
        }

        #region フォームイベント

        private void BrowserForm_Load(object sender, EventArgs e)
        {
            if (App.DebugMode)
            {
                Text = $"Inazuma Search {ApplicationEnvironment.GetVersionCaption()} [Debug Mode]";
            }
            App.Crawler.AlwaysCrawlProgress.ProgressChanged += AlwaysCrawlProgress_ProgressChanged;
        }

        private void BrowserForm_Shown(object sender, EventArgs e)
        {
            // 表示時にアクティブにする
            Activate();

            // 最前面への表示固定をオフ
            // ※初期状態では最前面固定としておくことで、必ずフォアグラウンドに表示されるようにする
            TopMost = false;
        }

        private void BrowserForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            // 起動中のフォーム一覧から削除
            Core.Application.BootingBrowserForms.Remove(this);

            // 常駐クロールモードでない場合は、全フォームを閉じたタイミングで終了
            if (!App.UserSettings.AlwaysCrawlMode && Core.Application.BootingBrowserForms.Count == 0)
            {
                System.Windows.Forms.Application.Exit();
            }
        }

        #endregion

        private void AlwaysCrawlProgress_ProgressChanged(object sender, ProgressState state)
        {
            AlwaysCrawlProgressTick++;
            if (AlwaysCrawlProgressTick >= AlwaysCrawlProgressTickEnd) AlwaysCrawlProgressTick = 0;

            // 設定画面を表示中の場合、一部ステップ時にカウント表示を更新
            if (ChromeBrowser.Address.EndsWith("setting.html"))
            {
                switch (state.CurrentStep)
                {
                    case ProgressState.Step.RecordUpdateEnd:
                    case ProgressState.Step.PurgeProcessEnd:
                    case ProgressState.Step.AlwaysCrawlDBDocumentDeleteBegin:
                        // 前回から1秒以上経っている場合のみ更新
                        if (LastUpdatedCountDataInSettingPage == null || (DateTime.Now - LastUpdatedCountDataInSettingPage.Value).TotalMilliseconds >= 1000)
                        {
                            if (ChromeBrowser.CanExecuteJavascriptInMainFrame)
                            {
                                ChromeBrowser.EvaluateScriptAsync("updateCountsAsync();");
                            }
                            LastUpdatedCountDataInSettingPage = DateTime.Now;
                        }
                        break;
                }
            }

            string suffix;
            if (AlwaysCrawlProgressTick < AlwaysCrawlProgressTickEnd / 3)
            {
                suffix = ".";
            }
            else if (AlwaysCrawlProgressTick < AlwaysCrawlProgressTickEnd / 3 * 2)
            {
                suffix = "..";
            }
            else
            {
                suffix = "...";
            }

            bool onProgress;
            var newCaption = GetBackgroundCrawlCaption(state.CurrentStep, state.Path, out onProgress);
            if (newCaption != null)
            {
                if (!string.IsNullOrEmpty(newCaption) && onProgress) newCaption += suffix;
                StlBackgroundCrawl.Text = newCaption;
            }
        }

        protected virtual string GetBackgroundCrawlCaption(ProgressState.Step currentStep, string path, out bool onProgress)
        {
            onProgress = true;

            // デバッグモードかどうかにかかわらず共通の表示
            switch (currentStep)
            {
                case ProgressState.Step.AlwaysCrawlBegin:
                    onProgress = false;
                    return "";

                case ProgressState.Step.Finish:
                    onProgress = false;
                    return $"常駐クロール: 更新済";

                case ProgressState.Step.AlwaysCrawlEnd:
                    onProgress = false;
                    return "";

                default:
                    // 上記以外はスキップ
                    break;
            }


            if (App.DebugMode)
            {
                // デバッグモード時のみの表示
                switch (currentStep)
                {
                    case ProgressState.Step.RecordUpdateCheckBegin:
                        return $"常駐クロール: 文書ファイルを検索中 ({Path.GetDirectoryName(path)})";

                    case ProgressState.Step.RecordUpdateBegin:
                        return $"常駐クロール: 文書データ登録中 ({path})";

                    case ProgressState.Step.PurgeBegin:
                        return $"常駐クロール: 存在しない文書データを削除中";

                    case ProgressState.Step.AlwaysCrawlDBDocumentDeleteBegin:
                    case ProgressState.Step.AlwaysCrawlDBDirectoryDeleteBegin:
                        return $"常駐クロール: 文書データを削除中 ({path})";
                    default:
                        // 上記以外はスキップ
                        return null;
                }
            }
            else
            {
                // 通常モード時のみの表示
                switch (currentStep)
                {
                    case ProgressState.Step.RecordUpdateCheckBegin:
                    case ProgressState.Step.RecordUpdateBegin:
                    case ProgressState.Step.PurgeBegin:
                    case ProgressState.Step.AlwaysCrawlDBDocumentDeleteBegin:
                    case ProgressState.Step.AlwaysCrawlDBDirectoryDeleteBegin:
                        return $"常駐クロール: 文書ファイルの情報を更新中";
                    default:
                        // 上記以外はスキップ
                        return null;
                }
            }
        }
    }


}
