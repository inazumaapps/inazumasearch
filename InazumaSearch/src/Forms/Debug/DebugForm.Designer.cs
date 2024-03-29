﻿namespace InazumaSearch.Forms
{
    partial class DebugForm
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
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.StatusText = new System.Windows.Forms.ToolStripStatusLabel();
            this.folderBrowserDialog1 = new System.Windows.Forms.FolderBrowserDialog();
            this.button10 = new System.Windows.Forms.Button();
            this.TxtPath = new System.Windows.Forms.TextBox();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.BtnGetThumbnail = new System.Windows.Forms.Button();
            this.cmbFormat = new System.Windows.Forms.ComboBox();
            this.ChkTransparent = new System.Windows.Forms.CheckBox();
            this.BtnGetExcelText = new System.Windows.Forms.Button();
            this.LsvObjectList = new System.Windows.Forms.ListView();
            this.ObjName = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.ObjSize = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.BtnGetSmallIco = new System.Windows.Forms.Button();
            this.TxtIconPath = new System.Windows.Forms.TextBox();
            this.BtnRaise = new System.Windows.Forms.Button();
            this.BtnRestart = new System.Windows.Forms.Button();
            this.BtnGetSmallSystemIco = new System.Windows.Forms.Button();
            this.LstIcons = new System.Windows.Forms.ListView();
            this.BtnOpenExcelFile = new System.Windows.Forms.Button();
            this.BtnUpdateDocumentFolderLabels = new System.Windows.Forms.Button();
            this.BtnShowProcessStatus = new System.Windows.Forms.Button();
            this.BtnParseMBox = new System.Windows.Forms.Button();
            this.BtnDBDefrag = new System.Windows.Forms.Button();
            this.BtnKillAlwaysCrawl = new System.Windows.Forms.Button();
            this.statusStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.StatusText});
            this.statusStrip1.Location = new System.Drawing.Point(0, 541);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(1163, 22);
            this.statusStrip1.TabIndex = 9;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // StatusText
            // 
            this.StatusText.Name = "StatusText";
            this.StatusText.Size = new System.Drawing.Size(118, 17);
            this.StatusText.Text = "toolStripStatusLabel1";
            // 
            // button10
            // 
            this.button10.Location = new System.Drawing.Point(12, 12);
            this.button10.Name = "button10";
            this.button10.Size = new System.Drawing.Size(75, 23);
            this.button10.TabIndex = 14;
            this.button10.Text = "DB削除";
            this.button10.UseVisualStyleBackColor = true;
            this.button10.Click += new System.EventHandler(this.button10_Click);
            // 
            // TxtPath
            // 
            this.TxtPath.Location = new System.Drawing.Point(174, 16);
            this.TxtPath.Name = "TxtPath";
            this.TxtPath.Size = new System.Drawing.Size(375, 23);
            this.TxtPath.TabIndex = 15;
            this.TxtPath.TextChanged += new System.EventHandler(this.TxtIconFilePath_TextChanged);
            // 
            // pictureBox1
            // 
            this.pictureBox1.BackColor = System.Drawing.Color.White;
            this.pictureBox1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pictureBox1.Location = new System.Drawing.Point(174, 41);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(394, 283);
            this.pictureBox1.TabIndex = 16;
            this.pictureBox1.TabStop = false;
            // 
            // BtnGetThumbnail
            // 
            this.BtnGetThumbnail.Location = new System.Drawing.Point(555, 16);
            this.BtnGetThumbnail.Name = "BtnGetThumbnail";
            this.BtnGetThumbnail.Size = new System.Drawing.Size(189, 23);
            this.BtnGetThumbnail.TabIndex = 17;
            this.BtnGetThumbnail.Text = "アイコン/サムネイル取得";
            this.BtnGetThumbnail.UseVisualStyleBackColor = true;
            this.BtnGetThumbnail.Click += new System.EventHandler(this.TxtGetThumbnail_Click);
            // 
            // cmbFormat
            // 
            this.cmbFormat.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbFormat.FormattingEnabled = true;
            this.cmbFormat.Items.AddRange(new object[] {
            "Small",
            "Medium",
            "Large",
            "ExtraLarge"});
            this.cmbFormat.Location = new System.Drawing.Point(598, 57);
            this.cmbFormat.Name = "cmbFormat";
            this.cmbFormat.Size = new System.Drawing.Size(292, 23);
            this.cmbFormat.TabIndex = 18;
            // 
            // ChkTransparent
            // 
            this.ChkTransparent.AutoSize = true;
            this.ChkTransparent.Checked = true;
            this.ChkTransparent.CheckState = System.Windows.Forms.CheckState.Checked;
            this.ChkTransparent.Location = new System.Drawing.Point(598, 84);
            this.ChkTransparent.Name = "ChkTransparent";
            this.ChkTransparent.Size = new System.Drawing.Size(96, 19);
            this.ChkTransparent.TabIndex = 19;
            this.ChkTransparent.Text = "Transparent";
            this.ChkTransparent.UseVisualStyleBackColor = true;
            // 
            // BtnGetExcelText
            // 
            this.BtnGetExcelText.Location = new System.Drawing.Point(750, 16);
            this.BtnGetExcelText.Name = "BtnGetExcelText";
            this.BtnGetExcelText.Size = new System.Drawing.Size(189, 23);
            this.BtnGetExcelText.TabIndex = 20;
            this.BtnGetExcelText.Text = "Excelの内容を各フィルタで取得";
            this.BtnGetExcelText.UseVisualStyleBackColor = true;
            this.BtnGetExcelText.Click += new System.EventHandler(this.BtnGetExcelText_Click);
            // 
            // LsvObjectList
            // 
            this.LsvObjectList.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.ObjName,
            this.ObjSize});
            this.LsvObjectList.GridLines = true;
            this.LsvObjectList.HideSelection = false;
            this.LsvObjectList.Location = new System.Drawing.Point(750, 138);
            this.LsvObjectList.Name = "LsvObjectList";
            this.LsvObjectList.Size = new System.Drawing.Size(401, 275);
            this.LsvObjectList.Sorting = System.Windows.Forms.SortOrder.Ascending;
            this.LsvObjectList.TabIndex = 23;
            this.LsvObjectList.UseCompatibleStateImageBehavior = false;
            this.LsvObjectList.View = System.Windows.Forms.View.Details;
            // 
            // ObjName
            // 
            this.ObjName.Text = "名前";
            this.ObjName.Width = 203;
            // 
            // ObjSize
            // 
            this.ObjSize.Text = "サイズ";
            this.ObjSize.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // BtnGetSmallIco
            // 
            this.BtnGetSmallIco.Location = new System.Drawing.Point(526, 375);
            this.BtnGetSmallIco.Name = "BtnGetSmallIco";
            this.BtnGetSmallIco.Size = new System.Drawing.Size(189, 23);
            this.BtnGetSmallIco.TabIndex = 25;
            this.BtnGetSmallIco.Text = "通常のアイコン取得";
            this.BtnGetSmallIco.UseVisualStyleBackColor = true;
            this.BtnGetSmallIco.Click += new System.EventHandler(this.BtnGetSmallIco_Click);
            // 
            // TxtIconPath
            // 
            this.TxtIconPath.Location = new System.Drawing.Point(23, 377);
            this.TxtIconPath.Name = "TxtIconPath";
            this.TxtIconPath.Size = new System.Drawing.Size(497, 23);
            this.TxtIconPath.TabIndex = 24;
            // 
            // BtnRaise
            // 
            this.BtnRaise.Location = new System.Drawing.Point(12, 57);
            this.BtnRaise.Name = "BtnRaise";
            this.BtnRaise.Size = new System.Drawing.Size(120, 23);
            this.BtnRaise.TabIndex = 26;
            this.BtnRaise.Text = "例外を発生";
            this.BtnRaise.UseVisualStyleBackColor = true;
            this.BtnRaise.Click += new System.EventHandler(this.BtnRaise_Click);
            // 
            // BtnRestart
            // 
            this.BtnRestart.Location = new System.Drawing.Point(12, 326);
            this.BtnRestart.Name = "BtnRestart";
            this.BtnRestart.Size = new System.Drawing.Size(120, 23);
            this.BtnRestart.TabIndex = 28;
            this.BtnRestart.Text = "再起動";
            this.BtnRestart.UseVisualStyleBackColor = true;
            this.BtnRestart.Click += new System.EventHandler(this.BtnRestart_Click);
            // 
            // BtnGetSmallSystemIco
            // 
            this.BtnGetSmallSystemIco.Location = new System.Drawing.Point(526, 404);
            this.BtnGetSmallSystemIco.Name = "BtnGetSmallSystemIco";
            this.BtnGetSmallSystemIco.Size = new System.Drawing.Size(189, 23);
            this.BtnGetSmallSystemIco.TabIndex = 29;
            this.BtnGetSmallSystemIco.Text = "システムアイコン取得";
            this.BtnGetSmallSystemIco.UseVisualStyleBackColor = true;
            this.BtnGetSmallSystemIco.Click += new System.EventHandler(this.BtnGetSmallSystemIco_Click);
            // 
            // LstIcons
            // 
            this.LstIcons.HideSelection = false;
            this.LstIcons.Location = new System.Drawing.Point(23, 402);
            this.LstIcons.Name = "LstIcons";
            this.LstIcons.Size = new System.Drawing.Size(497, 97);
            this.LstIcons.TabIndex = 30;
            this.LstIcons.UseCompatibleStateImageBehavior = false;
            this.LstIcons.View = System.Windows.Forms.View.SmallIcon;
            // 
            // BtnOpenExcelFile
            // 
            this.BtnOpenExcelFile.Location = new System.Drawing.Point(945, 57);
            this.BtnOpenExcelFile.Name = "BtnOpenExcelFile";
            this.BtnOpenExcelFile.Size = new System.Drawing.Size(189, 23);
            this.BtnOpenExcelFile.TabIndex = 31;
            this.BtnOpenExcelFile.Text = "Excelファイルを開く";
            this.BtnOpenExcelFile.UseVisualStyleBackColor = true;
            this.BtnOpenExcelFile.Click += new System.EventHandler(this.BtnOpenExcelFile_Click);
            // 
            // BtnUpdateDocumentFolderLabels
            // 
            this.BtnUpdateDocumentFolderLabels.Location = new System.Drawing.Point(750, 476);
            this.BtnUpdateDocumentFolderLabels.Name = "BtnUpdateDocumentFolderLabels";
            this.BtnUpdateDocumentFolderLabels.Size = new System.Drawing.Size(120, 23);
            this.BtnUpdateDocumentFolderLabels.TabIndex = 32;
            this.BtnUpdateDocumentFolderLabels.Text = "フォルダラベル更新";
            this.BtnUpdateDocumentFolderLabels.UseVisualStyleBackColor = true;
            this.BtnUpdateDocumentFolderLabels.Click += new System.EventHandler(this.BtnUpdateDocumentFolderLabels_Click);
            // 
            // BtnShowProcessStatus
            // 
            this.BtnShowProcessStatus.Location = new System.Drawing.Point(12, 112);
            this.BtnShowProcessStatus.Name = "BtnShowProcessStatus";
            this.BtnShowProcessStatus.Size = new System.Drawing.Size(137, 23);
            this.BtnShowProcessStatus.TabIndex = 33;
            this.BtnShowProcessStatus.Text = "プロセス状態表示";
            this.BtnShowProcessStatus.UseVisualStyleBackColor = true;
            this.BtnShowProcessStatus.Click += new System.EventHandler(this.BtnShowProcessStatus_Click);
            // 
            // BtnParseMBox
            // 
            this.BtnParseMBox.Location = new System.Drawing.Point(945, 16);
            this.BtnParseMBox.Name = "BtnParseMBox";
            this.BtnParseMBox.Size = new System.Drawing.Size(114, 23);
            this.BtnParseMBox.TabIndex = 34;
            this.BtnParseMBox.Text = "mboxパース";
            this.BtnParseMBox.UseVisualStyleBackColor = true;
            this.BtnParseMBox.Click += new System.EventHandler(this.BtnParseMBox_Click);
            // 
            // BtnDBDefrag
            // 
            this.BtnDBDefrag.Location = new System.Drawing.Point(12, 158);
            this.BtnDBDefrag.Name = "BtnDBDefrag";
            this.BtnDBDefrag.Size = new System.Drawing.Size(120, 23);
            this.BtnDBDefrag.TabIndex = 35;
            this.BtnDBDefrag.Text = "DBデフラグ";
            this.BtnDBDefrag.UseVisualStyleBackColor = true;
            this.BtnDBDefrag.Click += new System.EventHandler(this.BtnDBDefrag_Click);
            // 
            // BtnKillAlwaysCrawl
            // 
            this.BtnKillAlwaysCrawl.Location = new System.Drawing.Point(781, 97);
            this.BtnKillAlwaysCrawl.Name = "BtnKillAlwaysCrawl";
            this.BtnKillAlwaysCrawl.Size = new System.Drawing.Size(192, 23);
            this.BtnKillAlwaysCrawl.TabIndex = 36;
            this.BtnKillAlwaysCrawl.Text = "常駐クロール処理を中断";
            this.BtnKillAlwaysCrawl.UseVisualStyleBackColor = true;
            this.BtnKillAlwaysCrawl.Click += new System.EventHandler(this.BtnKillAlwaysCrawl_Click);
            // 
            // DebugForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.ClientSize = new System.Drawing.Size(1163, 563);
            this.Controls.Add(this.BtnKillAlwaysCrawl);
            this.Controls.Add(this.BtnDBDefrag);
            this.Controls.Add(this.BtnParseMBox);
            this.Controls.Add(this.BtnShowProcessStatus);
            this.Controls.Add(this.BtnUpdateDocumentFolderLabels);
            this.Controls.Add(this.BtnOpenExcelFile);
            this.Controls.Add(this.LstIcons);
            this.Controls.Add(this.BtnGetSmallSystemIco);
            this.Controls.Add(this.BtnRestart);
            this.Controls.Add(this.BtnRaise);
            this.Controls.Add(this.BtnGetSmallIco);
            this.Controls.Add(this.TxtIconPath);
            this.Controls.Add(this.LsvObjectList);
            this.Controls.Add(this.BtnGetExcelText);
            this.Controls.Add(this.ChkTransparent);
            this.Controls.Add(this.cmbFormat);
            this.Controls.Add(this.BtnGetThumbnail);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.TxtPath);
            this.Controls.Add(this.button10);
            this.Controls.Add(this.statusStrip1);
            this.Font = new System.Drawing.Font("Meiryo UI", 9F);
            this.Name = "DebugForm";
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.DebugForm_Load);
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel StatusText;
        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog1;
        private System.Windows.Forms.Button button10;
        private System.Windows.Forms.TextBox TxtPath;
        private System.Windows.Forms.Button BtnGetThumbnail;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.ComboBox cmbFormat;
        private System.Windows.Forms.CheckBox ChkTransparent;
        private System.Windows.Forms.Button BtnGetExcelText;
        private System.Windows.Forms.ListView LsvObjectList;
        private System.Windows.Forms.ColumnHeader ObjName;
        private System.Windows.Forms.ColumnHeader ObjSize;
        private System.Windows.Forms.Button BtnGetSmallIco;
        private System.Windows.Forms.TextBox TxtIconPath;
        private System.Windows.Forms.Button BtnRaise;
        private System.Windows.Forms.Button BtnRestart;
        private System.Windows.Forms.Button BtnGetSmallSystemIco;
        private System.Windows.Forms.ListView LstIcons;
        private System.Windows.Forms.Button BtnOpenExcelFile;
        private System.Windows.Forms.Button BtnUpdateDocumentFolderLabels;
        private System.Windows.Forms.Button BtnShowProcessStatus;
        private System.Windows.Forms.Button BtnParseMBox;
        private System.Windows.Forms.Button BtnDBDefrag;
        private System.Windows.Forms.Button BtnKillAlwaysCrawl;
    }
}

