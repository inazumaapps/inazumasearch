using System.Diagnostics;

namespace InazumaSearch.Core
{
    /// <summary>
    /// xdoc2txtを実行するAPI
    /// </summary>
    public class XDoc2TxtApi
    {
        public static string Extract(string path)
        {
            var p = new Process();
            p.StartInfo.FileName = $"externals/{ApplicationEnvironment.GetPlatform()}/xdoc2txt/xdoc2txt.exe";
            p.StartInfo.Arguments = $"\"{path}\"";
            p.StartInfo.CreateNoWindow = true;
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.RedirectStandardOutput = true;
            p.Start();

            return (p.StandardOutput.ReadToEnd() ?? "");
        }
    }


}
