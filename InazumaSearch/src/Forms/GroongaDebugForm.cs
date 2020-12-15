using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Alphaleonis.Win32.Filesystem;
using InazumaSearch.Core;
using Microsoft.WindowsAPICodePack.Shell;
using Newtonsoft.Json;

namespace InazumaSearch.Forms
{
    public partial class GroongaDebugForm : Form
    {
        public string Body { get; set; }
        public Groonga.Manager GM { get; set; }

        public GroongaDebugForm()
        {
            InitializeComponent();
        }

        private void BtnQuit_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void BtnRun_Click(object sender, EventArgs e)
        {
            var res = GM.ExecuteCommandLineAsString(TxtCommand.Text);
            dynamic parsedJson = JsonConvert.DeserializeObject(res);
            TxtResult.Text = JsonConvert.SerializeObject(parsedJson, Formatting.Indented);
        }
    }
}
