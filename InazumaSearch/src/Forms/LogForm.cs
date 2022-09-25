using System;
using System.Windows.Forms;
using NLog.Windows.Forms;

namespace InazumaSearch.Forms
{
    public partial class LogForm : Form
    {
        public LogForm()
        {
            InitializeComponent();

            RichTextBoxTarget.ReInitializeAllTextboxes(this);
        }

        private void BtnQuit_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
