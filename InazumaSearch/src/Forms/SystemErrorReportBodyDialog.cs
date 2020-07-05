using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using InazumaSearch.Core;
using Microsoft.WindowsAPICodePack.Shell;

namespace InazumaSearch.Forms
{
    public partial class SystemErrorReportBodyDialog : Form
    {
        public string Body { get; set; }

        public SystemErrorReportBodyDialog()
        {
            InitializeComponent();
        }
        public SystemErrorReportBodyDialog(string body)
        {
            InitializeComponent();
            Body = body;
        }

        private void SystemErrorDialog_Load(object sender, EventArgs e)
        {
            TxtBody.Text = Body;
        }

        private void BtnQuit_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
