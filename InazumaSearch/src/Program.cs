using System;
using System.Linq;

namespace InazumaSearch
{
    internal static class Program
    {
        /// <summary>
        /// アプリケーションのメイン エントリ ポイントです。
        /// </summary>
        [STAThread]
        private static void Main(string[] args)
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
    }

}
