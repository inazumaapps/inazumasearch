using System;
using System.Windows.Forms;

namespace InazumaSearch.Forms
{
    public partial class IgnorePatternHelpDialog : Form
    {
        public IgnorePatternHelpDialog()
        {
            InitializeComponent();
        }

        private void BtnOK_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
