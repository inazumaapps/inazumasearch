
namespace InazumaSearch.Forms
{
    partial class ExtNameAddDialog
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.TxtExtNames = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.DlgDirectorySelect = new System.Windows.Forms.FolderBrowserDialog();
            this.BtnOK = new System.Windows.Forms.Button();
            this.BtnCancel = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.TxtExtLabel = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // TxtExtNames
            // 
            this.TxtExtNames.Location = new System.Drawing.Point(15, 39);
            this.TxtExtNames.Name = "TxtExtNames";
            this.TxtExtNames.Size = new System.Drawing.Size(482, 19);
            this.TxtExtNames.TabIndex = 0;
            this.TxtExtNames.TextChanged += new System.EventHandler(this.TxtPath_TextChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(13, 14);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(353, 12);
            this.label1.TabIndex = 1;
            this.label1.Text = "追加したい拡張子を、スペース区切りで入力してください。　例：「cs vb rb」";
            // 
            // DlgDirectorySelect
            // 
            this.DlgDirectorySelect.Description = "追加する検索対象フォルダを選択してください。";
            this.DlgDirectorySelect.ShowNewFolderButton = false;
            // 
            // BtnOK
            // 
            this.BtnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.BtnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.BtnOK.Enabled = false;
            this.BtnOK.Location = new System.Drawing.Point(299, 178);
            this.BtnOK.Name = "BtnOK";
            this.BtnOK.Size = new System.Drawing.Size(96, 23);
            this.BtnOK.TabIndex = 3;
            this.BtnOK.Text = "OK";
            this.BtnOK.UseVisualStyleBackColor = true;
            this.BtnOK.Click += new System.EventHandler(this.BtnOK_Click);
            // 
            // BtnCancel
            // 
            this.BtnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.BtnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.BtnCancel.Location = new System.Drawing.Point(401, 178);
            this.BtnCancel.Name = "BtnCancel";
            this.BtnCancel.Size = new System.Drawing.Size(96, 23);
            this.BtnCancel.TabIndex = 4;
            this.BtnCancel.Text = "キャンセル";
            this.BtnCancel.UseVisualStyleBackColor = true;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(13, 84);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(269, 12);
            this.label2.TabIndex = 6;
            this.label2.Text = "また、そのファイルの種類を表すラベルも入力してください。";
            // 
            // TxtExtLabel
            // 
            this.TxtExtLabel.Location = new System.Drawing.Point(15, 125);
            this.TxtExtLabel.Name = "TxtExtLabel";
            this.TxtExtLabel.Size = new System.Drawing.Size(161, 19);
            this.TxtExtLabel.TabIndex = 5;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(13, 99);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(157, 12);
            this.label3.TabIndex = 7;
            this.label3.Text = "(検索結果画面で表示されます)";
            // 
            // ExtNameAddDialog
            // 
            this.AcceptButton = this.BtnOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.BtnCancel;
            this.ClientSize = new System.Drawing.Size(515, 213);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.TxtExtLabel);
            this.Controls.Add(this.BtnCancel);
            this.Controls.Add(this.BtnOK);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.TxtExtNames);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ExtNameAddDialog";
            this.ShowIcon = false;
            this.Text = "拡張子の追加";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox TxtExtNames;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.FolderBrowserDialog DlgDirectorySelect;
        private System.Windows.Forms.Button BtnOK;
        private System.Windows.Forms.Button BtnCancel;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox TxtExtLabel;
        private System.Windows.Forms.Label label3;
    }
}