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

namespace InazumaSearch.Forms
{
    public partial class FileBodyViewDialog : Form
    {
        public string Body { get; set; }

        public FileBodyViewDialog()
        {
            InitializeComponent();
        }
        public FileBodyViewDialog(string body)
        {
            InitializeComponent();
            Body = body;
        }

        private void DBBrowserBodyViewDialog_Load(object sender, EventArgs e)
        {
            TxtBody.Text = Body;
        }

        private void BtnQuit_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
