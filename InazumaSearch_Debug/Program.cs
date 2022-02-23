using System;
using InazumaSearch.Forms;

namespace InazumaSearch_Debug
{
    internal static class Program
    {
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

            System.Windows.Forms.Application.EnableVisualStyles();
            System.Windows.Forms.Application.SetCompatibleTextRenderingDefault(false);
            //Application.Run(new MainForm());


            // コマンドライン引数の解析
            string errorMessage;
            var opts = InazumaSearch.Core.Application.ParseCommandLineOptions(args, out errorMessage);
            if (opts == null)
            {
                InazumaSearch.Core.Util.ShowErrorMessage(errorMessage);
                return;
            }

            // 起動
            System.Windows.Forms.Application.Run(new InazumaSearch.ApplicationContext(
                  showBrowser: !opts.BackgroundMode
                , appDebugMode: true
                , htmlDirPath: opts.HtmlFullPath
            ));
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
