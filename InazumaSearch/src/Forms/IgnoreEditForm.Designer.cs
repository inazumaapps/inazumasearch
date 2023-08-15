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
            this.lnkPatternHelp = new System.Windows.Forms.LinkLabel();
            this.TxtSetting = new System.Windows.Forms.TextBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.LblSearching = new System.Windows.Forms.Label();
            this.LstPreview = new System.Windows.Forms.ListBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.TxtBaseDirPath = new System.Windows.Forms.TextBox();
            this.BtnSave = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.SuspendLayout();
            // 
            // BtnCancel
            // 
            this.BtnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.BtnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.BtnCancel.Location = new System.Drawing.Point(676, 560);
            this.BtnCancel.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.BtnCancel.Name = "BtnCancel";
            this.BtnCancel.Size = new System.Drawing.Size(158, 35);
            this.BtnCancel.TabIndex = 4;
            this.BtnCancel.Text = "キャンセル";
            this.BtnCancel.UseVisualStyleBackColor = true;
            this.BtnCancel.Click += new System.EventHandler(this.BtnCancel_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.lnkPatternHelp);
            this.groupBox1.Controls.Add(this.TxtSetting);
            this.groupBox1.Location = new System.Drawing.Point(18, 79);
            this.groupBox1.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Padding = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.groupBox1.Size = new System.Drawing.Size(816, 125);
            this.groupBox1.TabIndex = 5;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "無視パターン";
            // 
            // lnkPatternHelp
            // 
            this.lnkPatternHelp.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lnkPatternHelp.AutoSize = true;
            this.lnkPatternHelp.Location = new System.Drawing.Point(670, 0);
            this.lnkPatternHelp.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lnkPatternHelp.Name = "lnkPatternHelp";
            this.lnkPatternHelp.Size = new System.Drawing.Size(123, 15);
            this.lnkPatternHelp.TabIndex = 9;
            this.lnkPatternHelp.TabStop = true;
            this.lnkPatternHelp.Text = "無視パターンの書き方...";
            this.lnkPatternHelp.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.lnkPatternHelp_LinkClicked);
            // 
            // TxtSetting
            // 
            this.TxtSetting.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.TxtSetting.Location = new System.Drawing.Point(8, 24);
            this.TxtSetting.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.TxtSetting.Multiline = true;
            this.TxtSetting.Name = "TxtSetting";
            this.TxtSetting.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.TxtSetting.Size = new System.Drawing.Size(800, 93);
            this.TxtSetting.TabIndex = 0;
            this.TxtSetting.TextChanged += new System.EventHandler(this.TxtSetting_TextChanged);
            // 
            // groupBox2
            // 
            this.groupBox2.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox2.Controls.Add(this.LblSearching);
            this.groupBox2.Controls.Add(this.LstPreview);
            this.groupBox2.Location = new System.Drawing.Point(18, 211);
            this.groupBox2.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Padding = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.groupBox2.Size = new System.Drawing.Size(816, 331);
            this.groupBox2.TabIndex = 6;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "プレビュー（実際に無視対象となるファイル）";
            // 
            // LblSearching
            // 
            this.LblSearching.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.LblSearching.AutoSize = true;
            this.LblSearching.BackColor = System.Drawing.Color.White;
            this.LblSearching.Location = new System.Drawing.Point(343, 156);
            this.LblSearching.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.LblSearching.Name = "LblSearching";
            this.LblSearching.Size = new System.Drawing.Size(118, 15);
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
            this.LstPreview.ItemHeight = 15;
            this.LstPreview.Location = new System.Drawing.Point(8, 22);
            this.LstPreview.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.LstPreview.Name = "LstPreview";
            this.LstPreview.ScrollAlwaysVisible = true;
            this.LstPreview.Size = new System.Drawing.Size(800, 289);
            this.LstPreview.TabIndex = 0;
            // 
            // groupBox3
            // 
            this.groupBox3.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox3.Controls.Add(this.TxtBaseDirPath);
            this.groupBox3.Location = new System.Drawing.Point(18, 8);
            this.groupBox3.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Padding = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.groupBox3.Size = new System.Drawing.Size(816, 64);
            this.groupBox3.TabIndex = 6;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "起点フォルダ";
            // 
            // TxtBaseDirPath
            // 
            this.TxtBaseDirPath.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.TxtBaseDirPath.Enabled = false;
            this.TxtBaseDirPath.Location = new System.Drawing.Point(8, 24);
            this.TxtBaseDirPath.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.TxtBaseDirPath.Name = "TxtBaseDirPath";
            this.TxtBaseDirPath.Size = new System.Drawing.Size(800, 23);
            this.TxtBaseDirPath.TabIndex = 0;
            // 
            // BtnSave
            // 
            this.BtnSave.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.BtnSave.Location = new System.Drawing.Point(486, 560);
            this.BtnSave.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.BtnSave.Name = "BtnSave";
            this.BtnSave.Size = new System.Drawing.Size(158, 35);
            this.BtnSave.TabIndex = 8;
            this.BtnSave.Text = "確定";
            this.BtnSave.UseVisualStyleBackColor = true;
            this.BtnSave.Click += new System.EventHandler(this.BtnSave_Click);
            // 
            // IgnoreEditForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.BtnCancel;
            this.ClientSize = new System.Drawing.Size(854, 610);
            this.Controls.Add(this.BtnSave);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.BtnCancel);
            this.Font = new System.Drawing.Font("Meiryo UI", 9F);
            this.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "IgnoreEditForm";
            this.ShowIcon = false;
            this.Text = "(画面名)";
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

