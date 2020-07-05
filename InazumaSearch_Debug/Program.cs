using System;
using System.Linq;

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


    }
}
