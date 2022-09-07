using System;
using System.Windows.Forms;

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
