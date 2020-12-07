using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Alphaleonis.Win32.Filesystem;
using CommandLine;
using CommandLine.Text;
using Hnx8.ReadJEnc;
using InazumaSearch.Forms;

namespace InazumaSearch.Core
{
    /// <summary>
    /// Inazuma Searchのメイン処理を担当するクラス
    /// </summary>
    public partial class Application
    {
        /// <summary>
        /// デバッグモード。
        /// </summary>
        public bool DebugMode { get; set; }

        /// <summary>
        /// Portableモード
        /// </summary>
        public bool PortableMode { get; set; }

        /// <summary>
        /// HTMLフォルダのパス
        /// </summary>
        public string HtmlDirPath { get; set; }

        /// <summary>
        /// ログ出力用オブジェクト
        /// </summary>
        public NLog.Logger Logger { get; protected set; }

        /// <summary>
        /// ユーザー設定
        /// </summary>
        public UserSetting.Store UserSettings { get; protected set; }

        /// <summary>
        /// Groonga制御用オブジェクト
        /// </summary>
        public Groonga.Manager GM { get; protected set; }

        /// <summary>
        /// プラグイン管理用オブジェクト
        /// </summary>
        public Plugin.Manager PluginManager { get; protected set; }

        /// <summary>
        /// 対応しているすべてのフォーマットリスト
        /// </summary>
        public IList<Format> Formats { get; protected set; }

        /// <summary>
        /// クローラ
        /// </summary>
        public Crawler Crawler { get; protected set; }

        /// <summary>
        /// 通知アイコン。画面の初期化完了時に設定される。システム全体で必ず1つしか存在しない (static)
        /// </summary>
        public static NotifyIcon NotifyIcon { get; set; }

        public Application()
        {
            DebugMode = false;
            PortableMode = false;
            Formats = new List<Format>();
        }

        #region 特殊パスの取得

        /// <summary>
        /// DBディレクトリのパス
        /// </summary>
        public virtual string DBDirPath
        {
            get
            {
                if (PortableMode)
                {
                    return Path.Combine(System.Windows.Forms.Application.StartupPath, @"..\data\db");
                }
                else
                {
                    return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), @"Inazuma Search\db");
                }
            }
        }
        /// <summary>
        /// サムネイルディレクトリのパス
        /// </summary>
        public virtual string ThumbnailDirPath
        {
            get
            {
                if (PortableMode)
                {
                    return Path.Combine(System.Windows.Forms.Application.StartupPath, @"..\data\thumbnail");
                }
                else
                {
                    return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), @"Inazuma Search\thumbnail");
                }
            }
        }
        /// <summary>
        /// ログディレクトリのパス
        /// </summary>
        public virtual string LogDirPath
        {
            get
            {
                if (PortableMode)
                {
                    return Path.Combine(System.Windows.Forms.Application.StartupPath, @"..\data\log");
                }
                else
                {
                    return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), @"Inazuma Search\log");
                }
            }
        }

        /// <summary>
        /// ユーザー設定ファイルディレクトリのパス
        /// </summary>
        public virtual string UserSettingDirPath
        {
            get
            {
                if (PortableMode)
                {
                    return Path.Combine(System.Windows.Forms.Application.StartupPath, @"..\data");
                }
                else
                {
                    return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), @"Inazuma Search");
                }
            }
        }

        /// <summary>
        /// ユーザー設定ファイルのパス
        /// </summary>
        public virtual string UserSettingPath { get { return Path.Combine(UserSettingDirPath, @"UserSettings.json"); } }

        /// <summary>
        /// スタートアップフォルダに配置するショートカットのパス
        /// </summary>
        public virtual string StartupShortcutPath { get { return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Startup), @"Inazuma Search.lnk"); } }

        #endregion

        /// <summary>
        /// 対応フォーマット一覧を更新
        /// </summary>
        public void RefreshFormats()
        {
            // フォーマット関連の変数を初期化
            // (1)プラグインが対応している形式
            // (2)テキストファイルとして設定されている形式
            // (3)デフォルト形式
            // の順で全フォーマットを登録
            var formats = new List<Format>();
            foreach (var pair in PluginManager.GetTextExtNameToLabelMap())
            {
                var extName = pair.Key;
                var label = pair.Value;
                formats.Add(new Format("plugindoc", label, new[] { extName }));
            }
            formats.Add(new Format("text", "テキストファイル", new[] { "txt" }));
            foreach (var ext in UserSettings.TextExtensions)
            {
                formats.Add(new Format("text", ext.Label, new[] { ext.ExtName }));
            }
            formats.AddRange(Format.ALL_DEFAULT_FORMATS);
            Formats = formats;
        }

        /// <summary>
        /// アプリケーションの起動 (進捗ダイアログを表示する場合があるため、UIスレッドで立ち上げる必要がある)
        /// </summary>
        /// <returns>正常に起動したかどうか。falseを返した場合はアプリケーションを終了</returns>
        public virtual bool Boot()
        {
            // ロガー初期化
            Logger = NLog.LogManager.GetLogger("Application");

            // ログ出力
            Logger.Info("アプリケーション起動中...");

            // ユーザー設定の読み込み
            UserSettings = UserSetting.Store.Setup(UserSettingPath);

            // Groongaの起動 (Groongaが存在しなければ終了)
            GM = new Groonga.Manager(DBDirPath, LogDirPath);
            if (!File.Exists(GM.GroongaExePath))
            {
                Util.ShowErrorMessage("groonga.exeが見つかりませんでした。");
                Quit();
                return false;
            }
            GM.Boot();

            // プラグインの読み込み
            PluginManager = new Plugin.Manager();
            var loadedPluginVersionNumbers = PluginManager.LoadPlugins();

            // 対応フォーマット一覧の更新
            RefreshFormats();

            // クローラの初期化
            Crawler = new Core.Crawler(this);

            // 現在DBのスキーマバージョンを取得 (DBが存在しない場合は0)
            var schemaVer = GM.GetSchemaVersion();

            // 現在DBのスキーマバージョンが、アプリケーションが要求するスキーマバージョンより高い場合はエラーとして終了
            if (schemaVer > Groonga.Manager.AppSchemaVersion)
            {
                Util.ShowErrorMessage(string.Format("このバージョンのInazuma Searchが最新バージョンではないため、データベースを読み込むことができません。\n最新バージョンのInazuma Searchを使用してください。"));
                Quit();
                return false;
            }

            // 現在DBのスキーマバージョンが、アプリケーションが要求するスキーマバージョンより低い場合は
            // DBの新規作成orアップグレード処理を実行
            if (schemaVer < Groonga.Manager.AppSchemaVersion)
            {
                var reCrawlRequired = false;
                var t = Task.Run(() =>
                {
                    GM.SetupSchema(schemaVer, out reCrawlRequired);
                });
                var msg = (schemaVer == 0 ? "データベースを作成しています。しばらくお待ちください..." : "データベースを更新しています。しばらくお待ちください...");
                var f = new ProgressForm(t, msg);
                f.ShowDialog();

                // 再度のクロールが必要ならメッセージを表示
                if (reCrawlRequired)
                {
                    MessageBox.Show(string.Format("Inazuma Searchのアップグレードにより、再度のクロール実行が必要となります。\nお手数をおかけしますが、起動後にクロールを実行してください。"), "確認");
                }

                // ユーザー設定を再読み込み (スキーマ更新で影響が及ぶ場合があるため)
                UserSettings.Load();
            }

            // プラグイン構成に変更があれば、確認ダイアログを削除してクロール結果を削除
            foreach (var pair in loadedPluginVersionNumbers)
            {
                var pluginFullName = pair.Key;
                var versionNumber = pair.Value;

                if (!UserSettings.LastLoadedPluginVersionNumbers.ContainsKey(pluginFullName)
                    || versionNumber != UserSettings.LastLoadedPluginVersionNumbers[pluginFullName])
                {
                    if (Util.Confirm("プラグインの構成が変更されています。\nこの変更により、クロール結果に差異が出る可能性があります。\n\nInazuma Searchが保持している、クロール済みの文書情報を削除してもよろしいですか？\n（文書ファイルは削除されません）"))
                    {
                        var t = Task.Run(() =>
                        {
                            GM.Truncate(Table.Documents);
                            GM.Truncate(Table.DocumentsIndex);
                            DeleteAllThumbnailFiles();
                        });
                        var f = new ProgressForm(t, "文書データをクリアしています...");
                        f.ShowDialog();
                    };
                    break;
                }
            }

            // プラグイン構成を保存
            UserSettings.SaveLastLoadedPluginVersionNumbers(new Dictionary<string, int>(loadedPluginVersionNumbers));

            // Groongaの必要プラグインを登録
            GM.PluginRegister("functions/time");

            // ログ出力
            Logger.Info("アプリケーションを起動しました");

            // 常駐クロールモードがONであれば、クロールを開始する
            if (UserSettings.AlwaysCrawlMode)
            {
                var t = Crawler.RunAlwaysModeAsync();
            }

            // 正常起動
            return true;
        }

        /// <summary>
        /// テキストファイルとして扱うファイル拡張子の一覧を取得。結果にドット記号は含まない (例: "txt")
        /// </summary>
        public virtual List<string> GetTextExtNames()
        {
            var textExtNames = UserSettings.TextExtensions.Select(x => x.ExtName).ToList();
            textExtNames.Add("txt");
            return textExtNames;
        }

        /// <summary>
        /// プラグインで扱うファイル拡張子の一覧を取得。結果にドット記号は含まない (例: "xdw")
        /// </summary>
        public virtual List<string> GetPluginExtNames()
        {
            return PluginManager.GetTextExtNameToLabelMap().Keys.ToList();
        }

        /// <summary>
        /// 拡張子に応じて、指定した文書ファイルの本文テキストを抽出
        /// </summary>
        /// <param name="path">文書ファイルパス</param>
        /// <returns>ファイル本文。何らかのエラー</returns>
        public virtual string ExtractFileText(
            string path
            , IEnumerable<string> textExtNames
            , IEnumerable<string> pluginExtNames
        )
        {
            var ext = Path.GetExtension(path).TrimStart('.').ToLower();

            if (pluginExtNames.Contains(ext))
            {
                // プラグインが対応している場合は、プラグインを使用してテキスト抽出
                Logger.Trace($"Extract by plugin - {path}");
                return PluginManager.ExtractText(path);
            }
            else if (textExtNames.Contains(ext))
            {
                // テキストの拡張子として登録されている場合は、テキストファイルとして読み込み
                Logger.Trace($"Extract as text file - {path}");
                var body = "";
                var bytes = File.ReadAllBytes(path);
                var charCode = ReadJEnc.JP.GetEncoding(bytes, bytes.Length, out body);
                return body ?? "";
            }
            else
            {
                // 上記以外の場合はXDoc2Txtを使用
                Logger.Trace($"Extract by xdoc2txt - {path}");
                return XDoc2TxtApi.Extract(path);
            }
        }

        /// <summary>
        /// 常駐クロールモードを切り替える
        /// </summary>
        /// <param name="ownerForm"></param>
        /// <param name="flag"></param>
        public virtual void ChangeAlwaysCrawlMode(IWin32Window ownerForm, bool flag)
        {
            UserSettings.SaveAlwaysCrawlMode(flag);

            // 常駐クロールを起動or停止
            if (flag)
            {
                var mainTask = Task.Run(() =>
                {
                    var t = Crawler.RunAlwaysModeAsync();
                });

                var f = new ProgressForm(mainTask, "常駐クロールを開始しています...");
                f.ShowDialog(ownerForm);

                // 通知アイコンを表示する
                if (Core.Application.NotifyIcon != null)
                {
                    Core.Application.NotifyIcon.Visible = true;
                }
            }
            else
            {
                var mainTask = Task.Run(() =>
                {
                    Crawler.StopIfRunning();

                    // 合わせてスタートアップ起動もオフ
                    UserSettings.SaveStartUp(false);
                    if (File.Exists(StartupShortcutPath)) File.Delete(StartupShortcutPath);
                });

                var f = new ProgressForm(mainTask, "常駐クロールを停止しています...");
                f.ShowDialog(ownerForm);

                // 通知アイコンを隠す
                if (Core.Application.NotifyIcon != null)
                {
                    Core.Application.NotifyIcon.Visible = false;
                }

            }
        }

        /// <summary>
        /// スタートアップ起動ON/OFFを切り替える
        /// </summary>
        /// <param name="ownerForm"></param>
        /// <param name="flag"></param>
        public virtual void ChangeStartUp(IWin32Window ownerForm, bool flag)
        {
            UserSettings.SaveStartUp(flag);

            // スタートアップフォルダに作成するショートカットのパス
            if (flag)
            {
                var mainTask = Task.Run(() =>
                {
                    // InazumaSearchへのショートカットを作成
                    // from https://dobon.net/vb/dotnet/file/createshortcut.html

                    // ショートカットのリンク先
                    var targetPath = Path.Combine(System.Windows.Forms.Application.StartupPath, "InazumaSearch.exe");

                    // WshShellを作成
                    var t = Type.GetTypeFromCLSID(new Guid("72C24DD5-D70A-438B-8A42-98424B88AFB8"));
                    dynamic shell = Activator.CreateInstance(t);

                    // WshShortcutを作成
                    var shortcut = shell.CreateShortcut(StartupShortcutPath);

                    // リンク先
                    shortcut.TargetPath = targetPath;
                    shortcut.Arguments = " --background";
                    // アイコンのパス
                    shortcut.IconLocation = targetPath + ",0";

                    // ショートカットを作成
                    shortcut.Save();

                    // 後始末
                    System.Runtime.InteropServices.Marshal.FinalReleaseComObject(shortcut);
                    System.Runtime.InteropServices.Marshal.FinalReleaseComObject(shell);
                });

                var f = new ProgressForm(mainTask, "自動起動の設定を行っています...");
                f.ShowDialog(ownerForm);

            }
            else
            {
                var mainTask = Task.Run(() =>
                {
                    // ショートカットの削除
                    if (File.Exists(StartupShortcutPath)) File.Delete(StartupShortcutPath);
                });

                var f = new ProgressForm(mainTask, "自動起動の設定を解除しています...");
                f.ShowDialog(ownerForm);

            }
        }


        /// <summary>
        /// 文書データのラベル情報を一括更新
        /// </summary>
        /// <param name="documentKeys">更新対象のキーのコレクション。省略した場合はすべてのデータを更新</param>
        public virtual void UpdateDocumentFolderLabels(IEnumerable<string> documentKeys = null)
        {
            if (documentKeys == null)
            {
                var selectRes = GM.Select(
                      Table.Documents
                    , outputColumns: new[] { Column.Documents.KEY, Column.Documents.FILE_PATH }
                    , limit: -1
                    , sortKeys: new[] { Column.Documents.KEY }
                );

                documentKeys = selectRes.SearchResult.Records.Select((r) => (string)r.Key);
            }
            var values = new List<IDictionary<string, object>>();
            foreach (var key in documentKeys)
            {
                var valueDict = new Dictionary<string, object>
                {
                    [Column.Documents.KEY] = key
                };

                var labels = UserSettings.FindTargetFoldersFromDocumentKey(key)
                                .Where((f2) => !string.IsNullOrEmpty(f2.Label))
                                .Select((f2) => f2.Label)
                                .ToList();

                valueDict[Column.Documents.FOLDER_LABELS] = labels;

                values.Add(valueDict);
            }

            GM.Load(table: Table.Documents, values: values);
        }

        /// <summary>
        /// サムネイル画像をすべて削除
        /// </summary>
        public virtual void DeleteAllThumbnailFiles()
        {
            if (!Directory.Exists(ThumbnailDirPath)) return;
            foreach (var path in Directory.GetFiles(ThumbnailDirPath, "*.png"))
            {
                File.Delete(path);
            }
        }

        /// <summary>
        /// 指定された処理を実行。例外が発生した場合は例外ダイアログを表示し、処理を終了する
        /// </summary>
        public virtual void ExecuteInExceptionCatcher(Action act)
        {
            // デバッガがアタッチされていれば、例外ダイアログは表示しない
            if (Debugger.IsAttached)
            {
                act.Invoke();
                return;
            }

            try
            {
                act.Invoke();
            }
            catch (Exception ex)
            {
                var f2 = new SystemErrorDialog(ex);
                f2.ShowDialog();
                Quit();
            }
        }

        /// <summary>
        /// 指定された処理を実行。例外が発生した場合は例外ダイアログを表示し、処理を終了する
        /// </summary>
        public virtual T ExecuteInExceptionCatcher<T>(Func<T> act)
        {
            // デバッガがアタッチされていれば、例外ダイアログは表示しない
            if (Debugger.IsAttached)
            {
                return act.Invoke();
            }

            try
            {
                return act.Invoke();
            }
            catch (Exception ex)
            {
                var f2 = new SystemErrorDialog(ex);
                f2.ShowDialog();
                Quit();
                return default(T);
            }
        }

        /// <summary>
        /// コマンドライン引数を解析する
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public static CommandLineOptions ParseCommandLineOptions(string[] args, out string errorMessage)
        {
            // コマンドライン引数の解析
            CommandLineOptions res = null;
            string innerErrorMessage = null;

            CommandLine.Parser.Default.ParseArguments<CommandLineOptions>(args).WithParsed((opts) =>
            {
                res = opts;

                if (!Directory.Exists(opts.HtmlFullPath) || !File.Exists(Path.Combine(opts.HtmlFullPath, "index.html")))
                {
                    innerErrorMessage = string.Format("有効なhtmlフォルダが存在しません。(探索パス: {0})", opts.HtmlFullPath);
                }

            }).WithNotParsed((errors) =>
            {
                innerErrorMessage = errors.First().ToString();
            });

            // 結果の返却
            errorMessage = innerErrorMessage;
            return res;
        }

        /// <summary>
        /// アプリケーションを終了
        /// </summary>
        public static void Quit()
        {
            // 通知アイコンが存在していれば解放
            if (NotifyIcon != null)
            {
                NotifyIcon.Dispose();
            }

            System.Windows.Forms.Application.Exit();

        }

        /// <summary>
        /// アプリケーション全体を再起動
        /// </summary>
        /// <remarks>from https://dobon.net/vb/dotnet/programing/applicationrestart.html</remarks>
        public static void Restart()
        {
            //拡張:デバッガがアタッチされている場合は再起動不可 (メインアプリを終了した時点でデバッグも終了するため)
            if (Debugger.IsAttached)
            {
                return;
            }

            //プロセスのIDを取得する
            var processId = System.Diagnostics.Process.GetCurrentProcess().Id;
            //アプリケーションが終了するまで待機する時間
            var waitTime = 30000;
            //コマンドライン引数を作成する
            var cmd = "\"" + processId.ToString() + "\" " +
                "\"" + waitTime.ToString() + "\" " +
                Environment.CommandLine;
            //再起動用アプリケーションのパスを取得する
            var restartPath = Path.Combine(
                System.Windows.Forms.Application.StartupPath, "restart.exe");
            //再起動用アプリケーションを起動する
            System.Diagnostics.Process.Start(restartPath, cmd);
            //アプリケーションを終了する
            Quit();
        }
    }
}
