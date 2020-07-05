namespace InazumaSearch.Forms
{
    partial class SystemErrorDialog
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
            this.label1 = new System.Windows.Forms.Label();
            this.BtnQuit = new System.Windows.Forms.Button();
            this.ChkErrorReportSend = new System.Windows.Forms.CheckBox();
            this.BtnShowReportBody = new System.Windows.Forms.Button();
            this.TxtErrorComment = new System.Windows.Forms.TextBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.PicErrorIcon = new System.Windows.Forms.PictureBox();
            this.label2 = new System.Windows.Forms.Label();
            this.BtnQuitAndRestart = new System.Windows.Forms.Button();
            this.LblWebsiteUrl = new System.Windows.Forms.LinkLabel();
            this.label3 = new System.Windows.Forms.Label();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.PicErrorIcon)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(71, 23);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(298, 72);
            this.label1.TabIndex = 0;
            this.label1.Text = "システムエラーが発生したため、Inazuma Searchを終了します。\r\nご迷惑をおかけして申し訳ありません。\r\n\r\nよろしければ、エラーの情報を送信していただ" +
    "き\r\n不具合内容の改善にご協力ください。\r\n（文書ファイルの内容は送信されません）";
            // 
            // BtnQuit
            // 
            this.BtnQuit.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.BtnQuit.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.BtnQuit.Location = new System.Drawing.Point(276, 424);
            this.BtnQuit.Name = "BtnQuit";
            this.BtnQuit.Size = new System.Drawing.Size(71, 31);
            this.BtnQuit.TabIndex = 1;
            this.BtnQuit.Text = "終了";
            this.BtnQuit.UseVisualStyleBackColor = true;
            this.BtnQuit.Click += new System.EventHandler(this.BtnQuit_Click);
            // 
            // ChkErrorReportSend
            // 
            this.ChkErrorReportSend.AutoSize = true;
            this.ChkErrorReportSend.Checked = true;
            this.ChkErrorReportSend.CheckState = System.Windows.Forms.CheckState.Checked;
            this.ChkErrorReportSend.Location = new System.Drawing.Point(28, 136);
            this.ChkErrorReportSend.Name = "ChkErrorReportSend";
            this.ChkErrorReportSend.Size = new System.Drawing.Size(173, 16);
            this.ChkErrorReportSend.TabIndex = 2;
            this.ChkErrorReportSend.Text = "エラー情報を開発者へ送信する";
            this.ChkErrorReportSend.UseVisualStyleBackColor = true;
            this.ChkErrorReportSend.CheckedChanged += new System.EventHandler(this.ChkErrorReportSend_CheckedChanged);
            // 
            // BtnShowReportBody
            // 
            this.BtnShowReportBody.Location = new System.Drawing.Point(322, 132);
            this.BtnShowReportBody.Name = "BtnShowReportBody";
            this.BtnShowReportBody.Size = new System.Drawing.Size(121, 23);
            this.BtnShowReportBody.TabIndex = 3;
            this.BtnShowReportBody.Text = "送信内容の表示";
            this.BtnShowReportBody.UseVisualStyleBackColor = true;
            this.BtnShowReportBody.Click += new System.EventHandler(this.BtnShowReportBody_Click);
            // 
            // TxtErrorComment
            // 
            this.TxtErrorComment.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.TxtErrorComment.Location = new System.Drawing.Point(11, 23);
            this.TxtErrorComment.Multiline = true;
            this.TxtErrorComment.Name = "TxtErrorComment";
            this.TxtErrorComment.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.TxtErrorComment.Size = new System.Drawing.Size(393, 126);
            this.TxtErrorComment.TabIndex = 4;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.TxtErrorComment);
            this.groupBox1.Location = new System.Drawing.Point(28, 170);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Padding = new System.Windows.Forms.Padding(8);
            this.groupBox1.Size = new System.Drawing.Size(415, 164);
            this.groupBox1.TabIndex = 5;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "エラーの再現手順、条件などがあれば補足をお願いいたします (空欄可)";
            // 
            // PicErrorIcon
            // 
            this.PicErrorIcon.Location = new System.Drawing.Point(28, 23);
            this.PicErrorIcon.Name = "PicErrorIcon";
            this.PicErrorIcon.Size = new System.Drawing.Size(32, 32);
            this.PicErrorIcon.TabIndex = 6;
            this.PicErrorIcon.TabStop = false;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(61, 353);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(382, 12);
            this.label2.TabIndex = 7;
            this.label2.Text = "※返信が必要な場合は、下記公式サイトよりメールやGitter等でご連絡ください。";
            // 
            // BtnQuitAndRestart
            // 
            this.BtnQuitAndRestart.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.BtnQuitAndRestart.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.BtnQuitAndRestart.Location = new System.Drawing.Point(101, 424);
            this.BtnQuitAndRestart.Name = "BtnQuitAndRestart";
            this.BtnQuitAndRestart.Size = new System.Drawing.Size(134, 31);
            this.BtnQuitAndRestart.TabIndex = 8;
            this.BtnQuitAndRestart.Text = "終了して再起動";
            this.BtnQuitAndRestart.UseVisualStyleBackColor = true;
            this.BtnQuitAndRestart.Click += new System.EventHandler(this.BtnQuitAndRestart_Click);
            // 
            // LblWebsiteUrl
            // 
            this.LblWebsiteUrl.AutoSize = true;
            this.LblWebsiteUrl.LinkVisited = true;
            this.LblWebsiteUrl.Location = new System.Drawing.Point(237, 374);
            this.LblWebsiteUrl.Name = "LblWebsiteUrl";
            this.LblWebsiteUrl.Size = new System.Drawing.Size(206, 12);
            this.LblWebsiteUrl.TabIndex = 9;
            this.LblWebsiteUrl.TabStop = true;
            this.LblWebsiteUrl.Text = "http://inazumaapp.info/inazumasearch/";
            this.LblWebsiteUrl.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.LblWebsiteUrl_LinkClicked);
            // 
            // label3
            // 
            this.label3.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label3.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.label3.Location = new System.Drawing.Point(10, 410);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(448, 2);
            this.label3.TabIndex = 10;
            // 
            // SystemErrorDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(470, 467);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.LblWebsiteUrl);
            this.Controls.Add(this.BtnQuitAndRestart);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.PicErrorIcon);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.BtnShowReportBody);
            this.Controls.Add(this.ChkErrorReportSend);
            this.Controls.Add(this.BtnQuit);
            this.Controls.Add(this.label1);
            this.Name = "SystemErrorDialog";
            this.ShowIcon = false;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.Text = "エラー";
            this.TopMost = true;
            this.Load += new System.EventHandler(this.SystemErrorDialog_Load);
            this.Shown += new System.EventHandler(this.SystemErrorDialog_Shown);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.PicErrorIcon)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button BtnQuit;
        private System.Windows.Forms.CheckBox ChkErrorReportSend;
        private System.Windows.Forms.Button BtnShowReportBody;
        private System.Windows.Forms.TextBox TxtErrorComment;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.PictureBox PicErrorIcon;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button BtnQuitAndRestart;
        private System.Windows.Forms.LinkLabel LblWebsiteUrl;
        private System.Windows.Forms.Label label3;
    }
}

