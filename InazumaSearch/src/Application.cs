using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using Alphaleonis.Win32.Filesystem;
using CommandLine;
using InazumaSearch.Forms;
using InazumaSearchLib.Core;

namespace InazumaSearch
{
    /// <summary>
    /// Inazuma Searchのメイン処理を担当するクラス
    /// </summary>
    public partial class Application : ApplicationBase
    {
        #region staticプロパティ

        /// <summary>
        /// 起動中のブラウザ一覧
        /// </summary>
        public static List<BrowserForm> BootingBrowserForms { get; set; } = new List<BrowserForm>();

        /// <summary>
        /// 通知アイコン。画面の初期化完了時に設定される。システム全体で必ず1つしか存在しない (static)
        /// </summary>
        public static NotifyIcon NotifyIcon { get; set; }

        #endregion

        /// <summary>
        /// HTMLフォルダのパス
        /// </summary>
        public string HtmlDirPath { get; set; }

        /// <summary>
        /// メインのブラウザ画面
        /// </summary>
        public BrowserForm MainForm { get; protected set; }

        #region 特殊パスの取得

        /// <summary>
        /// スタートアップフォルダに配置するショートカットのパス
        /// </summary>
        public virtual string StartupShortcutPath { get { return Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Startup), @"Inazuma Search.lnk"); } }

        /// <inheritdoc/>
        public override string DataDirPath
        {
            get
            {
                if (ApplicationEnvironment.IsPortableMode())
                {
                    return Path.Combine(System.Windows.Forms.Application.StartupPath, @"..\data");
                }
                else
                {
                    return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), @"Inazuma Search\log");
                }
            }
        }

        /// <inheritdoc/>
        public override string GroongaExePath
        {
            get
            {
                return $"externals/{ApplicationEnvironment.GetPlatform()}/xdoc2txt/xdoc2txt.exe";
            }
        }

        /// <inheritdoc/>
        public override string XDoc2TxtExePath
        {
            get
            {
                return $"externals/{ApplicationEnvironment.GetPlatform()}/xdoc2txt/xdoc2txt.exe";
            }
        }

        #endregion

        #region abstractメソッドの実装

        public override bool Confirm(string message, bool defaultNo = false)
        {
            return GUIUtil.Confirm(message, defaultNo);
        }

        public override void InvokeWithProgressDisplay(Task task, string message)
        {
            // 進捗状況ダイアログを開き、メイン処理を実行
            var pf = new ProgressForm(task, message);
            pf.ShowDialog(MainForm);
        }

        public override void ShowInformationMessage(string message, string title = "情報")
        {
            GUIUtil.ShowInformationMessage(message, title);
        }

        public override void ShowErrorMessage(string message)
        {
            GUIUtil.ShowErrorMessage(message);
        }

        #endregion

        /// <summary>
        /// 常駐クロールモードを切り替える
        /// </summary>
        /// <param name="ownerForm"></param>
        /// <param name="flag"></param>
        public virtual void ChangeAlwaysCrawlMode(IWin32Window ownerForm, bool flag)
        {
            UserSettings.SaveAlwaysCrawlMode(flag);

            // 常駐クロールの自動再起動を無効化
            Crawler.DisableAlwaysCrawlAutoReboot(() =>
            {
                // 常駐クロールを起動or停止
                if (flag)
                {
                    // 常駐クロール開始
                    Crawler.StartAlwaysCrawl();

                    // 通知アイコンを表示する
                    if (Application.NotifyIcon != null)
                    {
                        Application.NotifyIcon.Visible = true;
                    }
                }
                else
                {
                    var mainTask = Task.Run(async () =>
                    {
                        // 常駐クロール停止
                        await Crawler.StopAlwaysCrawlIfRunningAsync(); // 停止完了まで待機

                        // 合わせてスタートアップ起動もオフ
                        UserSettings.SaveStartUp(false);
                        if (File.Exists(StartupShortcutPath)) File.Delete(StartupShortcutPath);
                    });

                    var f = new ProgressForm(mainTask, "常駐クロールを停止しています...");
                    f.ShowDialog(ownerForm);

                    // 通知アイコンを隠す
                    if (Application.NotifyIcon != null)
                    {
                        Application.NotifyIcon.Visible = false;
                    }

                }
            });
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
        /// 進捗フォームを表示した状態で、指定処理を実行。
        /// 現在実行中の常駐クロール処理がある場合、その常駐クロールを停止したうえで指定処理を実行し、完了後に必要に応じて常駐クロールを再開する
        /// （DB全体に影響を与える更新操作などの実行時に使用）
        /// </summary>
        public virtual void InvokeWithProgressFormWithoutAlwaysCrawl(IWin32Window ownerForm, string caption, Action mainProc)
        {
            // 常駐クロールの自動再起動を無効化
            Crawler.DisableAlwaysCrawlAutoReboot(() =>
        {
            // メイン処理
            var mainTask = Task.Run(async () =>
            {
                // 常駐クロール実行中の場合、停止
                await Crawler.StopAlwaysCrawlIfRunningAsync();

                // メイン処理を実行
                mainProc.Invoke();
            });

            // 進捗状況ダイアログを開き、メイン処理を実行
            var pf = new ProgressForm(mainTask, caption);
            pf.ShowDialog(ownerForm);

            // ユーザー設定で常駐クロールがONの場合、メイン処理完了後に常駐クロールを再開
            if (UserSettings.AlwaysCrawlMode)
            {
                Crawler.StartAlwaysCrawl();
            }
        });
        }

        /// <summary>
        /// 常駐クロール実行中であれば再起動する
        /// </summary>
        public virtual void RestartAlwaysCrawlIfRunning(Form ownerForm)
        {
            // 常駐クロール実行中の場合、一度常駐クロールを止めて、最初から常駐クロールを再開する
            if (Crawler.AlwaysCrawlIsRunning)
            {
                // UI側スレッドで処理実行
                ownerForm.Invoke((MethodInvoker)delegate
                {
                    // 常駐クロールの自動再起動を無効化
                    Crawler.DisableAlwaysCrawlAutoReboot(() =>
                    {
                        // 実行中の常駐クロール処理を中止
                        var stoppingTask = Crawler.StopAlwaysCrawlIfRunningAsync();
                        var pf = new ProgressForm(stoppingTask, "常駐クロールを再起動中...");
                        pf.ShowDialog(ownerForm);

                        // 常駐クロール再開
                        Crawler.StartAlwaysCrawl();
                    });
                });
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
                    innerErrorMessage = $"有効なhtmlフォルダが存在しません。(探索パス: {opts.HtmlFullPath})";
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
        public static void Restart(bool forceDebug = false)
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
                (forceDebug ? System.Environment.CommandLine.Replace("InazumaSearch.exe", "InazumaSearch_Debug.exe") : System.Environment.CommandLine);
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
