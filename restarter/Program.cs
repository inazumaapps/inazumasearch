using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace Restart
{
    /// <summary>
    /// from https://dobon.net/vb/dotnet/programing/applicationrestart.html 
    /// </summary>
    internal static class Program
    {

        //エントリポイント
        public static void Main()
        {
            try
            {
                Restart();
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(
                    "再起動に失敗しました。\n\nエラー: " + ex.Message,
                    "再起動に失敗しました",
                    System.Windows.Forms.MessageBoxButtons.OK,
                    System.Windows.Forms.MessageBoxIcon.Error);
            }
        }

        //コマンドライン引数で指定された設定を使用して再起動する
        private static void Restart()
        {
            //コマンドライン引数を取得する
            var args = System.Environment.GetCommandLineArgs();
            if (args.Length < 4)
            {
                throw new ArgumentException("コマンドライン引数が足りません。");
            }

            //終了を監視するプロセスIDを取得する
            int processId;
            try
            {
                processId = int.Parse(args[1]);
            }
            catch (Exception ex)
            {
                throw new ArgumentException("プロセスIDが不正です。", ex);
            }

            //終了の最長待機時間を取得する
            int waitTime;
            try
            {
                waitTime = int.Parse(args[2]);
            }
            catch (Exception ex)
            {
                throw new ArgumentException("待機時間が不正です。", ex);
            }
            if (waitTime < 1 || 60000 < waitTime)
            {
                throw new ArgumentException("待機時間は最長1分です。");
            }

            //起動するアプリケーションの実行ファイルのパスを取得する
            var exePath = args[3];
            if (!System.IO.File.Exists(exePath))
            {
                throw new ArgumentException("実行ファイルが見つかりません。");
            }
            //起動時に指定するコマンドライン引数を作成する
            var cmd = "";
            for (var i = 4; i < args.Length; i++)
            {
                if (4 < i)
                {
                    cmd += " ";
                }
                cmd += "\"" + args[i] + "\"";
            }

            //再起動する
            Restart(processId, waitTime, exePath, cmd);
        }

        /// <summary>
        /// 指定された設定を使用して再起動する
        /// </summary>
        /// <param name="exitProcessId">終了まで待機するアプリケーション</param>
        /// <param name="exitWaitTime">待機する再長時間（ミリ秒単位）</param>
        /// <param name="exePath">再起動する実行ファイルのパス</param>
        /// <param name="commandLine">再起動する時指定するコマンドライン引数</param>
        private static void Restart(int exitProcessId, int exitWaitTime,
            string exePath, string commandLine)
        {
            //終了を監視するプロセスを探す
            System.Diagnostics.Process p;
            try
            {
                p = System.Diagnostics.Process.GetProcessById(exitProcessId);
            }
            catch (ArgumentException)
            {
                //見つからない時はアプリケーションがすでに終了していると判断する
                p = null;
            }

            //アプリケーションの終了を待機する
            if (p != null)
            {
                if (!p.WaitForExit(exitWaitTime))
                {
                    throw new Exception("アプリケーションが終了しませんでした。");
                }
                p.Close();
            }

            //アプリケーションを起動する
            System.Diagnostics.Process.Start(exePath, commandLine);
        }
    }
}
