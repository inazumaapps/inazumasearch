using System.IO;

namespace InazumaSearch
{
    public class CommandLineOptions
    {
        [CommandLine.Option("html-path")]
        public string HtmlPath { get; set; }

        [CommandLine.Option("background")]
        public bool BackgroundMode { get; set; }

        public string HtmlFullPath
        {
            get
            {
                if (HtmlPath != null)
                {
                    return Path.GetFullPath(HtmlPath);
                }
                else
                {
                    var exeLocation = System.Reflection.Assembly.GetExecutingAssembly().Location;
                    return Path.GetFullPath(Path.Combine(Path.GetDirectoryName(exeLocation), "html"));
                }
            }
        }
    }
}
