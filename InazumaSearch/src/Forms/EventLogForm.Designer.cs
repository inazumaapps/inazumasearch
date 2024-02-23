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
            System.Windows.Forms.ListViewItem listViewItem1 = new System.Windows.Forms.ListViewItem("9999/99/99 99:99");
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.TxtMessage = new System.Windows.Forms.TextBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.lnkOpenExplorer = new System.Windows.Forms.LinkLabel();
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
            this.BtnUpdate = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.TxtMessage);
            this.groupBox1.Location = new System.Drawing.Point(12, 313);
            this.groupBox1.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Padding = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.groupBox1.Size = new System.Drawing.Size(983, 53);
            this.groupBox1.TabIndex = 1;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "メッセージ";
            // 
            // TxtMessage
            // 
            this.TxtMessage.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.TxtMessage.Location = new System.Drawing.Point(9, 21);
            this.TxtMessage.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.TxtMessage.Name = "TxtMessage";
            this.TxtMessage.ReadOnly = true;
            this.TxtMessage.Size = new System.Drawing.Size(971, 23);
            this.TxtMessage.TabIndex = 2;
            this.TxtMessage.TabStop = false;
            // 
            // groupBox2
            // 
            this.groupBox2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox2.Controls.Add(this.lnkOpenExplorer);
            this.groupBox2.Controls.Add(this.TxtTargetPath);
            this.groupBox2.Location = new System.Drawing.Point(12, 371);
            this.groupBox2.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Padding = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.groupBox2.Size = new System.Drawing.Size(983, 53);
            this.groupBox2.TabIndex = 3;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "対象ファイル";
            // 
            // lnkOpenExplorer
            // 
            this.lnkOpenExplorer.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.lnkOpenExplorer.AutoSize = true;
            this.lnkOpenExplorer.Location = new System.Drawing.Point(875, 24);
            this.lnkOpenExplorer.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lnkOpenExplorer.Name = "lnkOpenExplorer";
            this.lnkOpenExplorer.Size = new System.Drawing.Size(97, 15);
            this.lnkOpenExplorer.TabIndex = 3;
            this.lnkOpenExplorer.TabStop = true;
            this.lnkOpenExplorer.Text = "エクスプローラで開く";
            this.lnkOpenExplorer.Visible = false;
            this.lnkOpenExplorer.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.lnkOpenExplorer_LinkClicked);
            // 
            // TxtTargetPath
            // 
            this.TxtTargetPath.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.TxtTargetPath.Location = new System.Drawing.Point(9, 21);
            this.TxtTargetPath.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.TxtTargetPath.Name = "TxtTargetPath";
            this.TxtTargetPath.ReadOnly = true;
            this.TxtTargetPath.Size = new System.Drawing.Size(859, 23);
            this.TxtTargetPath.TabIndex = 2;
            this.TxtTargetPath.TabStop = false;
            // 
            // BtnClose
            // 
            this.BtnClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.BtnClose.Location = new System.Drawing.Point(873, 454);
            this.BtnClose.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.BtnClose.Name = "BtnClose";
            this.BtnClose.Size = new System.Drawing.Size(123, 35);
            this.BtnClose.TabIndex = 5;
            this.BtnClose.Text = "閉じる";
            this.BtnClose.UseVisualStyleBackColor = true;
            this.BtnClose.Click += new System.EventHandler(this.BtnClose_Click);
            // 
            // BtnExport
            // 
            this.BtnExport.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.BtnExport.Location = new System.Drawing.Point(569, 454);
            this.BtnExport.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.BtnExport.Name = "BtnExport";
            this.BtnExport.Size = new System.Drawing.Size(284, 35);
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
            this.lsvLogList.Items.AddRange(new System.Windows.Forms.ListViewItem[] {
            listViewItem1});
            this.lsvLogList.Location = new System.Drawing.Point(9, 10);
            this.lsvLogList.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.lsvLogList.Name = "lsvLogList";
            this.lsvLogList.Size = new System.Drawing.Size(987, 267);
            this.lsvLogList.TabIndex = 7;
            this.lsvLogList.UseCompatibleStateImageBehavior = false;
            this.lsvLogList.View = System.Windows.Forms.View.Details;
            this.lsvLogList.SelectedIndexChanged += new System.EventHandler(this.lsvLogList_SelectedIndexChanged);
            // 
            // Timestamp
            // 
            this.Timestamp.Text = "日時";
            this.Timestamp.Width = 150;
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
            this.label1.Location = new System.Drawing.Point(6, 474);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(200, 15);
            this.label1.TabIndex = 18;
            this.label1.Text = "※1,000件を超えたログは削除されます。";
            // 
            // dlgExport
            // 
            this.dlgExport.FileName = "InazumaSearch_EventLog.log";
            this.dlgExport.Filter = "すべてのファイル|*.*";
            this.dlgExport.Title = "出力先の選択";
            // 
            // BtnUpdate
            // 
            this.BtnUpdate.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.BtnUpdate.Location = new System.Drawing.Point(929, 281);
            this.BtnUpdate.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.BtnUpdate.Name = "BtnUpdate";
            this.BtnUpdate.Size = new System.Drawing.Size(67, 26);
            this.BtnUpdate.TabIndex = 19;
            this.BtnUpdate.Text = "更新";
            this.BtnUpdate.UseVisualStyleBackColor = true;
            this.BtnUpdate.Click += new System.EventHandler(this.BtnUpdate_Click);
            // 
            // EventLogForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.ClientSize = new System.Drawing.Size(1008, 507);
            this.Controls.Add(this.BtnUpdate);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.lsvLogList);
            this.Controls.Add(this.BtnExport);
            this.Controls.Add(this.BtnClose);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Font = new System.Drawing.Font("Meiryo UI", 9F);
            this.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.MinimizeBox = false;
            this.Name = "EventLogForm";
            this.ShowIcon = false;
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
        private System.Windows.Forms.Button BtnUpdate;
        private System.Windows.Forms.LinkLabel lnkOpenExplorer;
    }
}