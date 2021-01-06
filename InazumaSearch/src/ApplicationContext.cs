using System;
using System.Diagnostics;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Ipc;
using Alphaleonis.Win32.Filesystem;
using InazumaSearch.Core;
using InazumaSearch.Forms;

namespace InazumaSearch
{
    public class ApplicationContext : System.Windows.Forms.ApplicationContext
    {
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public ApplicationContext(
              string htmlDirPath
            , bool showBrowser = true
            , bool appDebugMode = false
        ) : base()
        {
            // 例外ハンドラ登録
            System.Windows.Forms.Application.ThreadException += Application_ThreadException;
            System.Windows.Forms.Application.SetUnhandledExceptionMode(System.Windows.Forms.UnhandledExceptionMode.CatchException);
            System.AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;

            // アプリケーション生成
            var app = new Core.Application
            {
                DebugMode = appDebugMode,
                HtmlDirPath = htmlDirPath
            };

            // ロガー用にログディレクトリパスを設定
            NLog.GlobalDiagnosticsContext.Set("LogDirPath", app.LogDirPath);
            // ログパスを設定
            var nlogConfPath =
                Path.Combine(System.Windows.Forms.Application.StartupPath, "nlogconf", (app.DebugMode ? @"Debug.config" : @"Release.config"));
            NLog.LogManager.Configuration = new NLog.Config.XmlLoggingConfiguration(nlogConfPath);

            // Inazuma Searchアプリケーションクラスの起動
            var bootSuccess = app.Boot();
            if (!bootSuccess)
            {
                Environment.Exit(0);
            }

            // メインコンポーネントの生成
            var comp = new MainComponent(app);

            // 通知アイコンをアプリケーションのstaticプロパティに設定
            Core.Application.NotifyIcon = comp.NotifyIcon;

            // 常駐クロールモードであれば、通知アイコンを表示する
            if (app.UserSettings.AlwaysCrawlMode)
            {
                Core.Application.NotifyIcon.Visible = true;
            }

            // IPCサーバーを起動（二重起動時に他プロセスからの操作を受けるために使用）
            var ipcChannel = new IpcServerChannel(IPCReceiver.GetIPCPortName());
            ChannelServices.RegisterChannel(ipcChannel, true);
            var ipcReceiver = new IPCReceiver(comp);
            RemotingServices.Marshal(ipcReceiver, IPCReceiver.UriName, typeof(IPCReceiver));

            // ブラウザの立ち上げ
            if (showBrowser)
            {
                comp.StartBrowser();
            }
        }

        private static void Application_ThreadException(object sender, System.Threading.ThreadExceptionEventArgs e)
        {
            // システムエラーダイアログ表示
            Debug.Print("<Application_ThreadException>");
            Debug.Print(e.Exception.ToString());
            var f = new SystemErrorDialog(e.Exception);
            f.ShowDialog();
            System.Windows.Forms.Application.Exit();
        }

        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            var ex = (Exception)e.ExceptionObject;

            // システムエラーダイアログ表示
            Debug.Print("<CurrentDomain_UnhandledException>");
            Debug.Print(ex.ToString());
            var f = new SystemErrorDialog(ex);
            f.ShowDialog();
            System.Windows.Forms.Application.Exit();
        }

    }

}
