using System.Diagnostics;

namespace portableLaunch
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            //アプリケーションを起動する
            var proc = new Process();
            proc.StartInfo.FileName = @"InazumaSearch.exe";
            proc.StartInfo.WorkingDirectory = @".\program";
            proc.Start();
        }
    }
}
