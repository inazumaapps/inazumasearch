using InazumaSearch.PluginSDK.V1;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace InazumaSearch.Plugins.CSSourceExtractor
{
    public class Plugin : IPlugin
    {
        public IPluginAPI Api { get; set; }

        public int VersionNumber { get { return 1; } }

        public async void Load()
        {
            Api.RegisterTextExtractor("C#", new[] { "cs" }, TextExtract);
        }

        public string TextExtract(string path)
        {
            return File.ReadAllText(path);
        }
    }
}
