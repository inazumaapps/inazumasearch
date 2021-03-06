﻿using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace InazumaSearch.Forms
{
    public partial class ExtNameAddDialog : Form
    {
        public IList<string> ExtNames { get; set; }
        public string ExtLabel { get; set; }

        public ExtNameAddDialog()
        {
            InitializeComponent();
        }

        private void BtnRef_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(TxtExtNames.Text))
            {
                DlgDirectorySelect.SelectedPath = TxtExtNames.Text;
            }
            if (DlgDirectorySelect.ShowDialog(this) == DialogResult.OK)
            {
                TxtExtNames.Text = DlgDirectorySelect.SelectedPath;
            }
        }

        private void BtnOK_Click(object sender, EventArgs e)
        {
            ExtNames = TxtExtNames.Text.Split(new[] { ' ', '　' });
            ExtLabel = TxtExtLabel.Text;
        }

        private void TxtPath_TextChanged(object sender, EventArgs e)
        {
            BtnOK.Enabled = !string.IsNullOrWhiteSpace(TxtExtNames.Text);
        }
    }
}
