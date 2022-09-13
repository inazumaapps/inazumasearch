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
        /// xdoc2txtを使って抽出する
        /// </summary>
        /// <param name="path">抽出対象のファイル</param>
        /// <returns>抽出結果の文字列</returns>
        /// <exception cref="TimeoutException">xdoc2txtの処理がタイムアウトした</exception>
        public static string Extract(string path)
        {
            var buf = new StringBuilder();

            using (var p = new Process())
            {
                p.StartInfo.FileName = $"externals/{ApplicationEnvironment.GetPlatform()}/xdoc2txt/xdoc2txt.exe";
                p.StartInfo.Arguments = $"\"{path}\"";
                p.StartInfo.CreateNoWindow = true;
                p.StartInfo.UseShellExecute = false;
                p.StartInfo.RedirectStandardOutput = true;
                p.OutputDataReceived += (object o, DataReceivedEventArgs args) =>
                {
                    buf.Append(args.Data);
                };
                p.Start();
                p.BeginOutputReadLine(); // 子プロセスの出力読み込み開始

                // 10秒間待つ
                if (p.WaitForExit(10000))
                {
                    var output = buf.ToString();
                    return (output ?? "");
                }
                else
                {
                    // 規定時間内に終了しなければタイムアウトとして、プロセスを強制終了
                    p.Kill();
                    throw new TimeoutException();
                }
            }
        }
    }
}
