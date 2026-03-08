using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;
using Alphaleonis.Win32.Filesystem;
using InazumaSearch.Core;
using InazumaSearch.Forms;

namespace InazumaSearch
{
    internal static class Program
    {
        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        private static extern int RegisterWindowMessage(string lpString);

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        private static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool PostMessage(IntPtr hWnd, int Msg, IntPtr wParam, IntPtr lParam);

        private static readonly int WM_SHOW_BROWSER = RegisterWindowMessage("InazumaSearch_ShowBrowser");
        /// <summary>
        /// アプリケーションのメイン エントリ ポイントです。
        /// </summary>
        [STAThread]
        private static void Main(string[] args)
        {
            // 例外ハンドラ登録
            System.Windows.Forms.Application.ThreadException += Application_ThreadException;
            System.Windows.Forms.Application.SetUnhandledExceptionMode(System.Windows.Forms.UnhandledExceptionMode.CatchException);
            System.AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;

            // Mutex名
            const string mutexName = "inazumaapps.info/InazumaSearch";

            // Mutexオブジェクトを作成する
            bool createdNew;
            var mutex = new Mutex(true, mutexName, out createdNew);

            //ミューテックスの初期所有権が付与されたか調べる
            if (!createdNew)
            {
                // 初期所有権が付与されなかった場合は二重起動とみなし、
                // 既存のプロセスのBackgroundMainFormを探してメッセージを送信
                var hWnd = FindWindow(null, InazumaSearch.src.Forms.BackgroundMainForm.WindowTitle);
                if (hWnd != IntPtr.Zero)
                {
                    PostMessage(hWnd, WM_SHOW_BROWSER, IntPtr.Zero, IntPtr.Zero);
                }
                return;
            }

            try
            {
                System.Windows.Forms.Application.EnableVisualStyles();
                System.Windows.Forms.Application.SetCompatibleTextRenderingDefault(false);
                //Application.Run(new MainForm());

                // コマンドライン引数の解析
                string errorMessage;
                var opts = Core.Application.ParseCommandLineOptions(args, out errorMessage);
                if (opts == null)
                {
                    Core.Util.ShowErrorMessage(errorMessage);
                    return;
                }

                // HTMLフォルダが存在しなければエラー
                if (!Directory.Exists(opts.HtmlFullPath))
                {
                    Core.Util.ShowErrorMessage("htmlフォルダが見つかりませんでした。\nデバッグ起動の場合は、コマンドライン引数の --html-path でhtmlフォルダのパスを指定してください。");
                    return;
                }

                var appDebugMode = false;
#if DEBUG
                appDebugMode = true;
#endif

                // 起動
                System.Windows.Forms.Application.Run(new InazumaSearch.ApplicationContext(
                      htmlDirPath: opts.HtmlFullPath
                    , showBrowser: !opts.BackgroundMode
                    , appDebugMode: appDebugMode
                ));
            }
            finally
            {
                //ミューテックスを解放する
                mutex.ReleaseMutex();
                mutex.Close();
            }

        }

        private static void Application_ThreadException(object sender, System.Threading.ThreadExceptionEventArgs e)
        {
            // システムエラーダイアログ表示
            var f = new SystemErrorDialog(e.Exception);
            f.ShowDialog();
            System.Windows.Forms.Application.Exit();
        }

        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            var ex = (Exception)e.ExceptionObject;

            // システムエラーダイアログ表示
            var f = new SystemErrorDialog(ex);
            f.ShowDialog();
            System.Windows.Forms.Application.Exit();
        }
    }
}
