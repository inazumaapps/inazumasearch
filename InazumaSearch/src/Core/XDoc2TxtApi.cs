using System;
using System.Diagnostics;
using System.Text;

namespace InazumaSearch.Core
{
    /// <summary>
    /// xdoc2txtを実行するAPI
    /// </summary>
    public class XDoc2TxtApi
    {
        /// <summary>
        /// xdoc2txtを使用してテキストを抽出する
        /// </summary>
        /// <param name="path">対象のファイルパス</param>
        /// <returns>抽出結果のテキスト</returns>
        /// <exception cref="TimeoutException">抽出処理がタイムアウトした（一部のPDF読み込み時に発生する場合がある）</exception>
        public static string Extract(string path)
        {
            var buf = new StringBuilder();
            var proc = new Process();
            proc.StartInfo.FileName = $"externals/{ApplicationEnvironment.GetPlatform()}/xdoc2txt/xdoc2txt.exe";
            proc.StartInfo.Arguments = $"\"{path}\"";
            proc.StartInfo.CreateNoWindow = true;
            proc.StartInfo.UseShellExecute = false;
            proc.StartInfo.RedirectStandardOutput = true;
            proc.OutputDataReceived += (sender, e) => buf.Append(e.Data);
            proc.Start();

            //非同期で出力の読み取りを開始
            proc.BeginOutputReadLine();

            // 終了を待機（最大で30秒待機する）
            if (!proc.WaitForExit(30 * 1000))
            {
                proc.Kill();
                proc.Close();
                throw new TimeoutException("xdoc2txtによるテキスト抽出が規定時間内に完了しませんでした。");
            }

            // プロセスのリソースを解放
            proc.Close();

            return buf.ToString();
        }
    }
}
