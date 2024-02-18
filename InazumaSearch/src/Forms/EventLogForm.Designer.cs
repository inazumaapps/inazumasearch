namespace InazumaSearch.src.Forms
{
    partial class EventLogForm
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
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.TxtMessage = new System.Windows.Forms.TextBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.TxtTargetPath = new System.Windows.Forms.TextBox();
            this.BtnClose = new System.Windows.Forms.Button();
            this.BtnExport = new System.Windows.Forms.Button();
            this.lsvLogList = new System.Windows.Forms.ListView();
            this.Timestamp = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.Summary = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.TargetPath = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.FileSize = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.label1 = new System.Windows.Forms.Label();
            this.dlgExport = new System.Windows.Forms.SaveFileDialog();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.TxtMessage);
            this.groupBox1.Location = new System.Drawing.Point(18, 469);
            this.groupBox1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Padding = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.groupBox1.Size = new System.Drawing.Size(1208, 80);
            this.groupBox1.TabIndex = 1;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "メッセージ";
            // 
            // TxtMessage
            // 
            this.TxtMessage.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.TxtMessage.Location = new System.Drawing.Point(13, 31);
            this.TxtMessage.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.TxtMessage.Name = "TxtMessage";
            this.TxtMessage.ReadOnly = true;
            this.TxtMessage.Size = new System.Drawing.Size(1188, 30);
            this.TxtMessage.TabIndex = 2;
            this.TxtMessage.TabStop = false;
            // 
            // groupBox2
            // 
            this.groupBox2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox2.Controls.Add(this.TxtTargetPath);
            this.groupBox2.Location = new System.Drawing.Point(18, 557);
            this.groupBox2.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Padding = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.groupBox2.Size = new System.Drawing.Size(1208, 80);
            this.groupBox2.TabIndex = 3;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "対象ファイル";
            // 
            // TxtTargetPath
            // 
            this.TxtTargetPath.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.TxtTargetPath.Location = new System.Drawing.Point(13, 31);
            this.TxtTargetPath.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.TxtTargetPath.Name = "TxtTargetPath";
            this.TxtTargetPath.ReadOnly = true;
            this.TxtTargetPath.Size = new System.Drawing.Size(1188, 30);
            this.TxtTargetPath.TabIndex = 2;
            this.TxtTargetPath.TabStop = false;
            // 
            // BtnClose
            // 
            this.BtnClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.BtnClose.Location = new System.Drawing.Point(1042, 681);
            this.BtnClose.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.BtnClose.Name = "BtnClose";
            this.BtnClose.Size = new System.Drawing.Size(184, 53);
            this.BtnClose.TabIndex = 5;
            this.BtnClose.Text = "閉じる";
            this.BtnClose.UseVisualStyleBackColor = true;
            this.BtnClose.Click += new System.EventHandler(this.BtnClose_Click);
            // 
            // BtnExport
            // 
            this.BtnExport.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.BtnExport.Location = new System.Drawing.Point(587, 681);
            this.BtnExport.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.BtnExport.Name = "BtnExport";
            this.BtnExport.Size = new System.Drawing.Size(426, 53);
            this.BtnExport.TabIndex = 6;
            this.BtnExport.Text = "全ログ情報をファイルに出力（不具合報告用）";
            this.BtnExport.UseVisualStyleBackColor = true;
            this.BtnExport.Click += new System.EventHandler(this.BtnExport_Click);
            // 
            // lsvLogList
            // 
            this.lsvLogList.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lsvLogList.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.Timestamp,
            this.Summary,
            this.TargetPath,
            this.FileSize});
            this.lsvLogList.FullRowSelect = true;
            this.lsvLogList.HideSelection = false;
            this.lsvLogList.Location = new System.Drawing.Point(13, 15);
            this.lsvLogList.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.lsvLogList.Name = "lsvLogList";
            this.lsvLogList.Size = new System.Drawing.Size(1212, 401);
            this.lsvLogList.TabIndex = 7;
            this.lsvLogList.UseCompatibleStateImageBehavior = false;
            this.lsvLogList.View = System.Windows.Forms.View.Details;
            this.lsvLogList.SelectedIndexChanged += new System.EventHandler(this.lsvLogList_SelectedIndexChanged);
            // 
            // Timestamp
            // 
            this.Timestamp.Text = "日時";
            this.Timestamp.Width = 211;
            // 
            // Summary
            // 
            this.Summary.Text = "種別";
            this.Summary.Width = 140;
            // 
            // TargetPath
            // 
            this.TargetPath.Text = "対象ファイル";
            this.TargetPath.Width = 537;
            // 
            // FileSize
            // 
            this.FileSize.Text = "ファイルサイズ";
            this.FileSize.Width = 142;
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(9, 711);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(303, 23);
            this.label1.TabIndex = 18;
            this.label1.Text = "※1,000件を超えたログは削除されます。";
            // 
            // dlgExport
            // 
            this.dlgExport.FileName = "InazumaSearch_EventLog.log";
            this.dlgExport.Filter = "すべてのファイル|*.*";
            this.dlgExport.Title = "出力先の選択";
            // 
            // EventLogForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(11F, 23F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1245, 761);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.lsvLogList);
            this.Controls.Add(this.BtnExport);
            this.Controls.Add(this.BtnClose);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Font = new System.Drawing.Font("Meiryo UI", 9F);
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.Name = "EventLogForm";
            this.Text = "イベントログ";
            this.Load += new System.EventHandler(this.EventLogForm_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.TextBox TxtMessage;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.TextBox TxtTargetPath;
        private System.Windows.Forms.Button BtnClose;
        private System.Windows.Forms.Button BtnExport;
        private System.Windows.Forms.ListView lsvLogList;
        private System.Windows.Forms.ColumnHeader Timestamp;
        private System.Windows.Forms.ColumnHeader Summary;
        private System.Windows.Forms.ColumnHeader TargetPath;
        private System.Windows.Forms.ColumnHeader FileSize;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.SaveFileDialog dlgExport;
    }
}