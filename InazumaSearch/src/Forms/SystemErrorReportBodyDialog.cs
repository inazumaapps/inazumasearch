using System;
using System.Windows.Forms;

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
