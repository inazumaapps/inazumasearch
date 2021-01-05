using System;
using System.Linq;
using System.Threading;
using System.Windows;
using Alphaleonis.Win32.Filesystem;
using InazumaSearch.Core;

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
            // Mutex名
            const string mutexName = "inazumaapps.info/InazumaSearch";

            // Mutexオブジェクトを作成する
            bool createdNew;
            var mutex = new Mutex(true, mutexName, out createdNew);

            //ミューテックスの初期所有権が付与されたか調べる
            if (!createdNew)
            {
                //されなかった場合は、すでに起動していると判断して終了
                Util.ShowErrorMessage("二重起動は行えません。");
                mutex.Close();
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
    }

}
