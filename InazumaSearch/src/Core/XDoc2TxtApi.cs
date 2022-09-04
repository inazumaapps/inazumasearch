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
        /// シェルに出力するエンコーディング
        /// </summary>
        public static Encoding SHELL_OUTPUT_ENCODING = null;

        /// <summary>
        /// xdoc2txtを実行し、テキストを抽出する
        /// </summary>
        public static string Extract(string path)
        {
            var p = CreateXDoc2TxtProcess(path);
            p.StartInfo.StandardOutputEncoding = SHELL_OUTPUT_ENCODING;
            p.Start();

            return (p.StandardOutput.ReadToEnd() ?? "");
        }

        /// <summary>
        /// テスト用のファイルに対してxdoc2txtを実行し、最適なエンコーディングを判別する（判別結果はstaticメンバに保存）
        /// </summary>
        public static void IdentifyOutputEncoding()
        {
            const string EXAMPLE_FILE_PATH = @"res\encoding-identifier.docx";
            const string EXAMPLE_TEXT = "エンコーディング判別用のテキスト";

            {
                var p = CreateXDoc2TxtProcess(EXAMPLE_FILE_PATH);
                p.Start();
                if (p.StandardOutput.ReadToEnd().Trim() == EXAMPLE_TEXT)
                {
                    Debug.WriteLine("シェル出力に最適なエンコーディング: null");
                    SHELL_OUTPUT_ENCODING = null;
                    return;
                }
            }

            {
                var p = CreateXDoc2TxtProcess(EXAMPLE_FILE_PATH);
                var sjis = Encoding.GetEncoding("Shift-JIS");
                p.StartInfo.StandardOutputEncoding = sjis;
                p.Start();
                if (p.StandardOutput.ReadToEnd().Trim() == EXAMPLE_TEXT)
                {
                    Debug.WriteLine("シェル出力に最適なエンコーディング: Shift-JIS");
                    SHELL_OUTPUT_ENCODING = sjis;
                    return;
                }
            }

            {
                var p = CreateXDoc2TxtProcess(EXAMPLE_FILE_PATH);
                p.StartInfo.StandardOutputEncoding = Encoding.UTF8;
                p.Start();
                if (p.StandardOutput.ReadToEnd().Trim() == EXAMPLE_TEXT)
                {
                    Debug.WriteLine("シェル出力に最適なエンコーディング: UTF-8");
                    SHELL_OUTPUT_ENCODING = Encoding.UTF8;
                    return;
                }
            }

            throw new NotSupportedException("シェル出力用のエンコーディングを判定できませんでした。");
        }

        protected static Process CreateXDoc2TxtProcess(string path)
        {
            var p = new Process();
            p.StartInfo.FileName = $"externals/{ApplicationEnvironment.GetPlatform()}/xdoc2txt/xdoc2txt.exe";
            p.StartInfo.Arguments = $"\"{path}\"";
            p.StartInfo.CreateNoWindow = true;
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.RedirectStandardOutput = true;
            return p;
        }
    }
}
