using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
            this.Close();
        }
    }
}
