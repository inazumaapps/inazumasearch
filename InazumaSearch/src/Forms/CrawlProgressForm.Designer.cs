namespace InazumaSearch.Forms
{
    partial class CrawlProgressForm
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
            this.components = new System.ComponentModel.Container();
            this.ProgressBar = new System.Windows.Forms.ProgressBar();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.statusText = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
            this.statusTimeCount = new System.Windows.Forms.ToolStripStatusLabel();
            this.folderBrowserDialog1 = new System.Windows.Forms.FolderBrowserDialog();
            this.BtnCancel = new System.Windows.Forms.Button();
            this.timeCounter = new System.Windows.Forms.Timer(this.components);
            this.lblInfo = new System.Windows.Forms.Label();
            this.lnkShowEventLog = new System.Windows.Forms.LinkLabel();
            this.statusStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // ProgressBar
            // 
            this.ProgressBar.Enabled = false;
            this.ProgressBar.Location = new System.Drawing.Point(30, 22);
            this.ProgressBar.MarqueeAnimationSpeed = 50;
            this.ProgressBar.Name = "ProgressBar";
            this.ProgressBar.Size = new System.Drawing.Size(435, 23);
            this.ProgressBar.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
            this.ProgressBar.TabIndex = 8;
            // 
            // statusStrip1
            // 
            this.statusStrip1.ImageScalingSize = new System.Drawing.Size(24, 24);
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.statusText,
            this.toolStripStatusLabel1,
            this.statusTimeCount});
            this.statusStrip1.Location = new System.Drawing.Point(0, 113);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(500, 22);
            this.statusStrip1.TabIndex = 9;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // statusText
            // 
            this.statusText.Name = "statusText";
            this.statusText.Size = new System.Drawing.Size(0, 17);
            // 
            // toolStripStatusLabel1
            // 
            this.toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            this.toolStripStatusLabel1.Size = new System.Drawing.Size(397, 17);
            this.toolStripStatusLabel1.Spring = true;
            // 
            // statusTimeCount
            // 
            this.statusTimeCount.ForeColor = System.Drawing.Color.Blue;
            this.statusTimeCount.Name = "statusTimeCount";
            this.statusTimeCount.Size = new System.Drawing.Size(88, 17);
            this.statusTimeCount.Text = "処理時間: 00:00";
            // 
            // BtnCancel
            // 
            this.BtnCancel.Location = new System.Drawing.Point(377, 70);
            this.BtnCancel.Name = "BtnCancel";
            this.BtnCancel.Size = new System.Drawing.Size(88, 23);
            this.BtnCancel.TabIndex = 10;
            this.BtnCancel.Text = "中断";
            this.BtnCancel.UseVisualStyleBackColor = true;
            this.BtnCancel.Click += new System.EventHandler(this.BtnCancel_Click);
            // 
            // timeCounter
            // 
            this.timeCounter.Interval = 1000;
            this.timeCounter.Tick += new System.EventHandler(this.timeCounter_Tick);
            // 
            // lblInfo
            // 
            this.lblInfo.AutoSize = true;
            this.lblInfo.ForeColor = System.Drawing.SystemColors.GrayText;
            this.lblInfo.Location = new System.Drawing.Point(12, 81);
            this.lblInfo.Name = "lblInfo";
            this.lblInfo.Size = new System.Drawing.Size(181, 15);
            this.lblInfo.TabIndex = 11;
            this.lblInfo.Text = "※ クロール実行中も検索可能です。";
            // 
            // lnkShowEventLog
            // 
            this.lnkShowEventLog.AutoSize = true;
            this.lnkShowEventLog.Location = new System.Drawing.Point(12, 81);
            this.lnkShowEventLog.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lnkShowEventLog.Name = "lnkShowEventLog";
            this.lnkShowEventLog.Size = new System.Drawing.Size(219, 15);
            this.lnkShowEventLog.TabIndex = 12;
            this.lnkShowEventLog.TabStop = true;
            this.lnkShowEventLog.Text = "登録失敗の詳細を確認する（イベントログ）";
            this.lnkShowEventLog.Visible = false;
            this.lnkShowEventLog.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.lnkShowEventLog_LinkClicked);
            // 
            // CrawlProgressForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.ClientSize = new System.Drawing.Size(500, 135);
            this.Controls.Add(this.lnkShowEventLog);
            this.Controls.Add(this.lblInfo);
            this.Controls.Add(this.BtnCancel);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.ProgressBar);
            this.Font = new System.Drawing.Font("Meiryo UI", 9F);
            this.MaximizeBox = false;
            this.Name = "CrawlProgressForm";
            this.ShowIcon = false;
            this.Text = "クロール実行中...";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.Form_FormClosed);
            this.Load += new System.EventHandler(this.Form_Load);
            this.Shown += new System.EventHandler(this.Form_Shown);
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.ProgressBar ProgressBar;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel statusText;
        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog1;
        private System.Windows.Forms.Button BtnCancel;
        private System.Windows.Forms.ToolStripStatusLabel statusTimeCount;
        private System.Windows.Forms.Timer timeCounter;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel1;
        private System.Windows.Forms.Label lblInfo;
        private System.Windows.Forms.LinkLabel lnkShowEventLog;
    }
}

