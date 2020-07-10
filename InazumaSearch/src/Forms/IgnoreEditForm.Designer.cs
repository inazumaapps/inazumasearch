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
            this.BtnClose = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.TxtSetting = new System.Windows.Forms.TextBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.ProgPreviewing = new System.Windows.Forms.ProgressBar();
            this.LstPreview = new System.Windows.Forms.ListBox();
            this.BtnRefreshPreview = new System.Windows.Forms.Button();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.TxtBaseDirPath = new System.Windows.Forms.TextBox();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.SuspendLayout();
            // 
            // BtnClose
            // 
            this.BtnClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.BtnClose.Location = new System.Drawing.Point(602, 396);
            this.BtnClose.Name = "BtnClose";
            this.BtnClose.Size = new System.Drawing.Size(110, 28);
            this.BtnClose.TabIndex = 4;
            this.BtnClose.Text = "閉じる";
            this.BtnClose.UseVisualStyleBackColor = true;
            this.BtnClose.Click += new System.EventHandler(this.BtnClose_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.TxtSetting);
            this.groupBox1.Location = new System.Drawing.Point(15, 63);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(699, 100);
            this.groupBox1.TabIndex = 5;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "追記する無視設定の内容";
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
            this.groupBox2.Controls.Add(this.ProgPreviewing);
            this.groupBox2.Controls.Add(this.LstPreview);
            this.groupBox2.Location = new System.Drawing.Point(15, 169);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(699, 211);
            this.groupBox2.TabIndex = 6;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "プレビュー（実際に無視対象となるファイル）";
            // 
            // ProgPreviewing
            // 
            this.ProgPreviewing.Location = new System.Drawing.Point(265, 96);
            this.ProgPreviewing.MarqueeAnimationSpeed = 10;
            this.ProgPreviewing.Name = "ProgPreviewing";
            this.ProgPreviewing.Size = new System.Drawing.Size(170, 23);
            this.ProgPreviewing.Style = System.Windows.Forms.ProgressBarStyle.Marquee;
            this.ProgPreviewing.TabIndex = 1;
            this.ProgPreviewing.Visible = false;
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
            // BtnRefreshPreview
            // 
            this.BtnRefreshPreview.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.BtnRefreshPreview.Location = new System.Drawing.Point(20, 396);
            this.BtnRefreshPreview.Name = "BtnRefreshPreview";
            this.BtnRefreshPreview.Size = new System.Drawing.Size(208, 28);
            this.BtnRefreshPreview.TabIndex = 7;
            this.BtnRefreshPreview.Text = "プレビュー表示の強制更新";
            this.BtnRefreshPreview.UseVisualStyleBackColor = true;
            this.BtnRefreshPreview.Click += new System.EventHandler(this.BtnRefreshPreview_Click);
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.TxtBaseDirPath);
            this.groupBox3.Location = new System.Drawing.Point(15, 6);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(699, 51);
            this.groupBox3.TabIndex = 6;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "無視設定の起点となるフォルダ";
            // 
            // TxtBaseDirPath
            // 
            this.TxtBaseDirPath.Enabled = false;
            this.TxtBaseDirPath.Location = new System.Drawing.Point(7, 19);
            this.TxtBaseDirPath.Name = "TxtBaseDirPath";
            this.TxtBaseDirPath.Size = new System.Drawing.Size(686, 19);
            this.TxtBaseDirPath.TabIndex = 0;
            // 
            // IgnoreEditForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(742, 444);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.BtnRefreshPreview);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.BtnClose);
            this.Name = "IgnoreEditForm";
            this.Text = "ファイル無視設定";
            this.Load += new System.EventHandler(this.IgnoreEditForm_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Button BtnClose;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.TextBox TxtSetting;
        private System.Windows.Forms.ListBox LstPreview;
        private System.Windows.Forms.Button BtnRefreshPreview;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.TextBox TxtBaseDirPath;
        private System.Windows.Forms.ProgressBar ProgPreviewing;
    }
}

