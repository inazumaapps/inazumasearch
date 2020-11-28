using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Alphaleonis.Win32.Filesystem;
using CefSharp;
using CefSharp.WinForms;
using InazumaSearch.Core;
using InazumaSearch.Groonga.Exceptions;
using Microsoft.WindowsAPICodePack.Dialogs;
using Microsoft.WindowsAPICodePack.Shell;
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

        public class DBStateApi
        {
            public long DocumentCount { get; set; }
            public long TargetFolderCount { get; set; }
            public string LastCrawlTimeCaption { get; set; }
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
                return App.PortableMode;
            }

            public bool IsUpdateCheckFailed()
            {
                return !ISAutoUpdater.UpdateCheckFinished;
            }

            public string GetUserSettings()
            {
                return JsonConvert.SerializeObject(App.UserSettings.PlainData);
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



            public void CrawlStart()
            {
                OwnerForm.InvokeOnUIThread((f) => { f.CrawlStart(); });
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

            public bool AddTargetDirectory(string path)
            {
                return App.ExecuteInExceptionCatcher(() =>
                {

                    var folders = App.UserSettings.TargetFolders;

                    // 同じフォルダパスが登録されていればエラー
                    if (folders.Any(f => f.Path == path))
                    {
                        ShowErrorMessage("すでに登録済みのフォルダです。");
                        return false;
                    }

                    //// 同じフォルダパスではないが、重複する
                    //if (folders.Any(f => f.Path.StartsWith(path)))
                    //{
                    //    ShowErrorMessage("登録済みのフォルダと重複しています。\n{0}\n{1}\n\n統合して1つのフォルダにしてよろしいですか？");
                    //    return;
                    //}

                    // 同じパス設定が存在しなければ追加
                    folders.Add(new UserSetting.TargetFolder() { Path = path, Type = UserSetting.TargetFolderType.DocumentFile });
                    App.UserSettings.SaveTargetFolders(folders);

                    return true;
                });

            }
            public void DeleteTargetDirectory(string path)
            {
                App.ExecuteInExceptionCatcher(() =>
                {

                    var folders = App.UserSettings.TargetFolders;
                    folders.Remove(folders.First(f => f.Path == path));
                    App.UserSettings.SaveTargetFolders(folders);
                });
            }


            public void ShowIgnoreEditForm(string path)
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
                    var defaultPattern = "/" + relPath.Replace('\\', '/');
                    var dialog = new IgnoreEditForm(baseDirPath, defaultPattern);
                    dialog.ShowDialog(form);
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
                    var folders = App.UserSettings.TargetFolders;
                    var folder = folders.First(f2 => f2.Path == path);
                    folder.Label = label;

                    App.UserSettings.Save();

                    var t = Task.Run(() =>
                    {
                        App.UpdateDocumentFolderLabels();
                    });

                    var pf = new ProgressForm(t, "DBのラベル情報を更新中...");
                    pf.ShowDialog(f);
                });

            }

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
            /// バージョンを取得
            /// </summary>
            public string GetVersion()
            {
                return Util.GetVersion().ToString();
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

            public string SearchTargetDirectories()
            {
                return App.ExecuteInExceptionCatcher<string>(() =>
                {
                    var targetDirectories = new List<UserSetting.TargetFolder>();
                    var fileCounts = new Dictionary<string, long>();
                    foreach (var folder in App.UserSettings.TargetFolders)
                    {
                        targetDirectories.Add(folder);

                        // 収集済みのファイル数をカウント
                        var res = App.GM.Select(
                            Table.Documents
                            , limit: 0
                            , outputColumns: new string[] { Groonga.VColumn.ID }
                            , query: string.Format("{0}:^{1}", Column.Documents.FILE_PATH, Groonga.Util.EscapeForQuery(folder.Path)));
                        fileCounts[folder.Path] = res.SearchResult.NHits;
                    }

                    return JsonConvert.SerializeObject(new { target_directories = targetDirectories, file_counts = fileCounts });
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
                    groongaFilters.Add(string.Format("{0} == {1}", Column.Documents.KEY, Groonga.Util.EscapeForQuery(key)));

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

        }

        public void InitializeChromium(string htmlDirPath)
        {
            if (!Cef.IsInitialized)
            {
                // CefSharp初期化
                CefSharpSettings.LegacyJavascriptBindingEnabled = true;
                CefSharpSettings.WcfEnabled = true; // from <https://github.com/cefsharp/CefSharp/issues/2990>

                var settings = new CefSettings
                {
                    Locale = "ja-JP"
                };

                // Initialize cef with the provided settings
                Cef.Initialize(settings);
            }

            // Create a browser component
            ChromeBrowser = new ChromiumWebBrowser(Path.Combine(htmlDirPath, "index.html"));
            ChromeBrowser.BrowserSettings.AcceptLanguageList = "ja-JP";
            ChromeBrowser.BrowserSettings.WebSecurity = CefState.Disabled;

            // Add it to the form and fill it to the form window.
            Controls.Add(ChromeBrowser);
            ChromeBrowser.Dock = DockStyle.Fill;

            Api = new CefApi();
            ChromeBrowser.JavascriptObjectRepository.Register("api", Api, isAsync: false);
            AsyncApi = new CefAsyncApi(ChromeBrowser);
            ChromeBrowser.JavascriptObjectRepository.Register("asyncApi", AsyncApi, isAsync: true);
            DBState = new DBStateApi();
            ChromeBrowser.JavascriptObjectRepository.Register("dbState", DBState, isAsync: false);

            ChromeBrowser.IsBrowserInitializedChanged += ChromeBrowser_IsBrowserInitializedChanged;
            ChromeBrowser.FrameLoadStart += ChromeBrowser_FrameLoadStart;

            ChromeBrowser.KeyDown += BrowserForm_KeyDown;

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

                if (App.UserSettings.AlwaysCrawlMode)
                {
                    DBState.LastCrawlTimeCaption = "常駐クロール実行中";

                }
                else
                {
                    var lastCrawlTime = App.UserSettings.LastCrawlTime;
                    if (lastCrawlTime != null)
                    {
                        DBState.LastCrawlTimeCaption = "最終実行: " + lastCrawlTime.Value.ToString("yyyy/MM/dd HH:mm");
                    }
                }

                // 更新の有無をチェック
                ISAutoUpdater.Check(App.PortableMode, (args) =>
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


                //To disable the menu then call clear
                // model.Clear();

                //Removing existing menu item
                //bool removed = model.Remove(CefMenuCommand.ViewSource); // Remove "View Source" option

                //Add new custom menu items

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
                    if (Application.DebugMode)
                    {
                        model.AddItem((CefMenuCommand)ShowDevTools, "開発ツール");
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

        private void Browser_FormClosing(object sender, FormClosingEventArgs e)
        {
            //Cef.Shutdown();
        }


        protected virtual void CrawlStart()
        {
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
                    var msg = string.Format("{0}ドライブの空き容量が残り少なくなっているため\nクロールによってDBのサイズが拡大し、空き容量が無くなる可能性があります。\n(現在の空き容量: {1})\n\nクロールを実行してもよろしいですか？"
                                            , driveLetter.ToUpper(), freeSpaceCaption);
                    var res = MessageBox.Show(this, msg, "警告", MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2);
                    if (res == DialogResult.No)
                    {
                        return;
                    }
                }
            }

            ChromeBrowser.EvaluateScriptAsync("$('#CRAWL-START').addClass('disabled');");

            var f = new CrawlProgressForm(App, () =>
            {
                ChromeBrowser.EvaluateScriptAsync("$('#CRAWL-START').removeClass('disabled');");
            });
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

            Process.Start(path);
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


        private void BrowserForm_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F12)
            {
                //ShowDevTool();
            }
        }

        private void BrowserForm_KeyPress(object sender, KeyPressEventArgs e)
        {
        }

        private void BrowserForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            // 常駐クロールモードでない場合は、フォームを閉じたタイミングで終了
            if (!App.UserSettings.AlwaysCrawlMode)
            {
                System.Windows.Forms.Application.Exit();
            }
        }

        private void BrowserForm_Load(object sender, EventArgs e)
        {

            if (App.DebugMode)
            {
                Text = $"Inazuma Search {Util.GetVersion().ToString()} {Util.GetPlatform()} (Debug Mode)";
            }

            Debug.Print(Thread.CurrentThread.ManagedThreadId.ToString());

        }
    }
}
