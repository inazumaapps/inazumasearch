namespace InazumaSearch.Forms
{
    partial class IgnoreEditForm
    {
        /// <summary>
        /// 必要なデザイナー変数です。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 使用中のリソースをすべてクリーンアップします。
        /// </summary>
        /// <param name="disposing">マネージ リソースを破棄する場合は true を指定し、その他の場合は false を指定します。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows フォーム デザイナーで生成されたコード

        /// <summary>
        /// デザイナー サポートに必要なメソッドです。このメソッドの内容を
        /// コード エディターで変更しないでください。
        /// </summary>
        private void InitializeComponent()
        {
            this.BtnCancel = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.TxtSetting = new System.Windows.Forms.TextBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.LblSearching = new System.Windows.Forms.Label();
            this.LstPreview = new System.Windows.Forms.ListBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.TxtBaseDirPath = new System.Windows.Forms.TextBox();
            this.BtnSave = new System.Windows.Forms.Button();
            this.lnkPatternHelp = new System.Windows.Forms.LinkLabel();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.SuspendLayout();
            // 
            // BtnCancel
            // 
            this.BtnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.BtnCancel.Location = new System.Drawing.Point(577, 396);
            this.BtnCancel.Name = "BtnCancel";
            this.BtnCancel.Size = new System.Drawing.Size(135, 28);
            this.BtnCancel.TabIndex = 4;
            this.BtnCancel.Text = "キャンセル";
            this.BtnCancel.UseVisualStyleBackColor = true;
            this.BtnCancel.Click += new System.EventHandler(this.BtnCancel_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.lnkPatternHelp);
            this.groupBox1.Controls.Add(this.TxtSetting);
            this.groupBox1.Location = new System.Drawing.Point(15, 63);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(699, 100);
            this.groupBox1.TabIndex = 5;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "無視パターン";
            // 
            // TxtSetting
            // 
            this.TxtSetting.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.TxtSetting.Location = new System.Drawing.Point(7, 19);
            this.TxtSetting.Multiline = true;
            this.TxtSetting.Name = "TxtSetting";
            this.TxtSetting.Size = new System.Drawing.Size(686, 75);
            this.TxtSetting.TabIndex = 0;
            this.TxtSetting.TextChanged += new System.EventHandler(this.TxtSetting_TextChanged);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.LblSearching);
            this.groupBox2.Controls.Add(this.LstPreview);
            this.groupBox2.Location = new System.Drawing.Point(15, 169);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(699, 211);
            this.groupBox2.TabIndex = 6;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "プレビュー（実際に無視対象となるファイル）";
            // 
            // LblSearching
            // 
            this.LblSearching.AutoSize = true;
            this.LblSearching.BackColor = System.Drawing.Color.White;
            this.LblSearching.Location = new System.Drawing.Point(287, 97);
            this.LblSearching.Name = "LblSearching";
            this.LblSearching.Size = new System.Drawing.Size(110, 12);
            this.LblSearching.TabIndex = 2;
            this.LblSearching.Text = "ファイルを検索中です...";
            this.LblSearching.Visible = false;
            // 
            // LstPreview
            // 
            this.LstPreview.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.LstPreview.FormattingEnabled = true;
            this.LstPreview.ItemHeight = 12;
            this.LstPreview.Location = new System.Drawing.Point(7, 18);
            this.LstPreview.Name = "LstPreview";
            this.LstPreview.ScrollAlwaysVisible = true;
            this.LstPreview.Size = new System.Drawing.Size(686, 184);
            this.LstPreview.TabIndex = 0;
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.TxtBaseDirPath);
            this.groupBox3.Location = new System.Drawing.Point(15, 6);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(699, 51);
            this.groupBox3.TabIndex = 6;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "無視設定の起点フォルダ";
            // 
            // TxtBaseDirPath
            // 
            this.TxtBaseDirPath.Enabled = false;
            this.TxtBaseDirPath.Location = new System.Drawing.Point(7, 19);
            this.TxtBaseDirPath.Name = "TxtBaseDirPath";
            this.TxtBaseDirPath.Size = new System.Drawing.Size(686, 19);
            this.TxtBaseDirPath.TabIndex = 0;
            // 
            // BtnSave
            // 
            this.BtnSave.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.BtnSave.Location = new System.Drawing.Point(415, 396);
            this.BtnSave.Name = "BtnSave";
            this.BtnSave.Size = new System.Drawing.Size(135, 28);
            this.BtnSave.TabIndex = 8;
            this.BtnSave.Text = "確定";
            this.BtnSave.UseVisualStyleBackColor = true;
            this.BtnSave.Click += new System.EventHandler(this.BtnSave_Click);
            // 
            // lnkPatternHelp
            // 
            this.lnkPatternHelp.AutoSize = true;
            this.lnkPatternHelp.Location = new System.Drawing.Point(574, 0);
            this.lnkPatternHelp.Name = "lnkPatternHelp";
            this.lnkPatternHelp.Size = new System.Drawing.Size(115, 12);
            this.lnkPatternHelp.TabIndex = 9;
            this.lnkPatternHelp.TabStop = true;
            this.lnkPatternHelp.Text = "無視パターンの書き方...";
            this.lnkPatternHelp.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.lnkPatternHelp_LinkClicked);
            // 
            // IgnoreEditForm
            // 
            this.AcceptButton = this.BtnSave;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.BtnCancel;
            this.ClientSize = new System.Drawing.Size(742, 444);
            this.Controls.Add(this.BtnSave);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.BtnCancel);
            this.Name = "IgnoreEditForm";
            this.Text = "ファイル無視設定";
            this.Load += new System.EventHandler(this.IgnoreEditForm_Load);
            this.Shown += new System.EventHandler(this.IgnoreEditForm_Shown);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Button BtnCancel;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.TextBox TxtSetting;
        private System.Windows.Forms.ListBox LstPreview;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.TextBox TxtBaseDirPath;
        private System.Windows.Forms.Button BtnSave;
        private System.Windows.Forms.Label LblSearching;
        private System.Windows.Forms.LinkLabel lnkPatternHelp;
    }
}

