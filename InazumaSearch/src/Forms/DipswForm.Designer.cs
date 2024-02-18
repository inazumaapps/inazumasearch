﻿namespace InazumaSearch.Forms
{
    partial class DipswForm
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
            this.BtnClearDocumentDB = new System.Windows.Forms.Button();
            this.BtnClose = new System.Windows.Forms.Button();
            this.BtnClearAllData = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.tblDebug = new System.Windows.Forms.TableLayoutPanel();
            this.lblDocumentDBSize = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.LblDBDocumentDirUnchangable = new System.Windows.Forms.Label();
            this.BtnResetDocumentDBDirPath = new System.Windows.Forms.Button();
            this.BtnChangeDocumentDBDirPath = new System.Windows.Forms.Button();
            this.TxtDocumentDBDirPath = new System.Windows.Forms.TextBox();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.lblVersion = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.lsvTextExtensions = new System.Windows.Forms.ListView();
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.label9 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.BtnDeleteTextExt = new System.Windows.Forms.Button();
            this.BtnAddTextExt = new System.Windows.Forms.Button();
            this.lnkOpenDataFolder = new System.Windows.Forms.LinkLabel();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.NumDisplayPageSizeForListView = new System.Windows.Forms.NumericUpDown();
            this.NumDisplayPageSizeForNormalView = new System.Windows.Forms.NumericUpDown();
            this.BtnRebootDebugMode = new System.Windows.Forms.Button();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.label10 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.NumDocumentExtractTimeout = new System.Windows.Forms.NumericUpDown();
            this.lnlEventLogForm = new System.Windows.Forms.LinkLabel();
            this.groupBox5 = new System.Windows.Forms.GroupBox();
            this.label12 = new System.Windows.Forms.Label();
            this.label13 = new System.Windows.Forms.Label();
            this.NumTextFileMaxSizeByMB = new System.Windows.Forms.NumericUpDown();
            this.tblDebug.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.tableLayoutPanel2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.NumDisplayPageSizeForListView)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.NumDisplayPageSizeForNormalView)).BeginInit();
            this.groupBox4.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.NumDocumentExtractTimeout)).BeginInit();
            this.groupBox5.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.NumTextFileMaxSizeByMB)).BeginInit();
            this.SuspendLayout();
            // 
            // BtnClearDocumentDB
            // 
            this.BtnClearDocumentDB.Location = new System.Drawing.Point(18, 18);
            this.BtnClearDocumentDB.Margin = new System.Windows.Forms.Padding(4);
            this.BtnClearDocumentDB.Name = "BtnClearDocumentDB";
            this.BtnClearDocumentDB.Size = new System.Drawing.Size(402, 34);
            this.BtnClearDocumentDB.TabIndex = 0;
            this.BtnClearDocumentDB.Text = "クロールした文書データをクリア";
            this.BtnClearDocumentDB.UseVisualStyleBackColor = true;
            this.BtnClearDocumentDB.Click += new System.EventHandler(this.BtnClearCrawledData_Click);
            // 
            // BtnClose
            // 
            this.BtnClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.BtnClose.Location = new System.Drawing.Point(1070, 766);
            this.BtnClose.Margin = new System.Windows.Forms.Padding(4);
            this.BtnClose.Name = "BtnClose";
            this.BtnClose.Size = new System.Drawing.Size(165, 42);
            this.BtnClose.TabIndex = 4;
            this.BtnClose.Text = "閉じる";
            this.BtnClose.UseVisualStyleBackColor = true;
            this.BtnClose.Click += new System.EventHandler(this.BtnClose_Click);
            // 
            // BtnClearAllData
            // 
            this.BtnClearAllData.Location = new System.Drawing.Point(18, 80);
            this.BtnClearAllData.Margin = new System.Windows.Forms.Padding(4);
            this.BtnClearAllData.Name = "BtnClearAllData";
            this.BtnClearAllData.Size = new System.Drawing.Size(402, 34);
            this.BtnClearAllData.TabIndex = 1;
            this.BtnClearAllData.Text = "全てのデータを初期化";
            this.BtnClearAllData.UseVisualStyleBackColor = true;
            this.BtnClearAllData.Click += new System.EventHandler(this.BtnClearAllData_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(18, 118);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(524, 23);
            this.label1.TabIndex = 17;
            this.label1.Text = "※クロールした文書データと、ユーザーの設定情報をすべて初期化します。";
            // 
            // tblDebug
            // 
            this.tblDebug.ColumnCount = 2;
            this.tblDebug.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 61.46341F));
            this.tblDebug.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 38.53659F));
            this.tblDebug.Controls.Add(this.lblDocumentDBSize, 1, 0);
            this.tblDebug.Controls.Add(this.label6, 0, 0);
            this.tblDebug.Location = new System.Drawing.Point(22, 328);
            this.tblDebug.Margin = new System.Windows.Forms.Padding(4);
            this.tblDebug.Name = "tblDebug";
            this.tblDebug.RowCount = 1;
            this.tblDebug.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 47.76119F));
            this.tblDebug.Size = new System.Drawing.Size(352, 46);
            this.tblDebug.TabIndex = 22;
            this.tblDebug.Visible = false;
            // 
            // lblDocumentDBSize
            // 
            this.lblDocumentDBSize.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.lblDocumentDBSize.AutoSize = true;
            this.lblDocumentDBSize.Location = new System.Drawing.Point(255, 11);
            this.lblDocumentDBSize.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblDocumentDBSize.Name = "lblDocumentDBSize";
            this.lblDocumentDBSize.Size = new System.Drawing.Size(93, 23);
            this.lblDocumentDBSize.TabIndex = 1;
            this.lblDocumentDBSize.Text = "9,999 MB";
            // 
            // label6
            // 
            this.label6.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(4, 11);
            this.label6.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(175, 23);
            this.label6.TabIndex = 0;
            this.label6.Text = "文書データベースサイズ";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.LblDBDocumentDirUnchangable);
            this.groupBox2.Controls.Add(this.BtnResetDocumentDBDirPath);
            this.groupBox2.Controls.Add(this.BtnChangeDocumentDBDirPath);
            this.groupBox2.Controls.Add(this.TxtDocumentDBDirPath);
            this.groupBox2.Location = new System.Drawing.Point(20, 195);
            this.groupBox2.Margin = new System.Windows.Forms.Padding(4);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Padding = new System.Windows.Forms.Padding(4);
            this.groupBox2.Size = new System.Drawing.Size(598, 120);
            this.groupBox2.TabIndex = 2;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "文書データベースの保存先";
            // 
            // LblDBDocumentDirUnchangable
            // 
            this.LblDBDocumentDirUnchangable.BackColor = System.Drawing.SystemColors.Control;
            this.LblDBDocumentDirUnchangable.ForeColor = System.Drawing.Color.Gray;
            this.LblDBDocumentDirUnchangable.Location = new System.Drawing.Point(230, 66);
            this.LblDBDocumentDirUnchangable.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.LblDBDocumentDirUnchangable.Name = "LblDBDocumentDirUnchangable";
            this.LblDBDocumentDirUnchangable.Size = new System.Drawing.Size(360, 34);
            this.LblDBDocumentDirUnchangable.TabIndex = 25;
            this.LblDBDocumentDirUnchangable.Text = "（ポータブル版では変更不可）";
            this.LblDBDocumentDirUnchangable.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.LblDBDocumentDirUnchangable.Visible = false;
            // 
            // BtnResetDocumentDBDirPath
            // 
            this.BtnResetDocumentDBDirPath.Enabled = false;
            this.BtnResetDocumentDBDirPath.Location = new System.Drawing.Point(261, 66);
            this.BtnResetDocumentDBDirPath.Margin = new System.Windows.Forms.Padding(4);
            this.BtnResetDocumentDBDirPath.Name = "BtnResetDocumentDBDirPath";
            this.BtnResetDocumentDBDirPath.Size = new System.Drawing.Size(228, 34);
            this.BtnResetDocumentDBDirPath.TabIndex = 30;
            this.BtnResetDocumentDBDirPath.Text = "初期設定フォルダに戻す";
            this.BtnResetDocumentDBDirPath.UseVisualStyleBackColor = true;
            this.BtnResetDocumentDBDirPath.Click += new System.EventHandler(this.BtnResetDocumentDBDirPath_Click);
            // 
            // BtnChangeDocumentDBDirPath
            // 
            this.BtnChangeDocumentDBDirPath.Location = new System.Drawing.Point(498, 66);
            this.BtnChangeDocumentDBDirPath.Margin = new System.Windows.Forms.Padding(4);
            this.BtnChangeDocumentDBDirPath.Name = "BtnChangeDocumentDBDirPath";
            this.BtnChangeDocumentDBDirPath.Size = new System.Drawing.Size(92, 34);
            this.BtnChangeDocumentDBDirPath.TabIndex = 29;
            this.BtnChangeDocumentDBDirPath.Text = "変更";
            this.BtnChangeDocumentDBDirPath.UseVisualStyleBackColor = true;
            this.BtnChangeDocumentDBDirPath.Click += new System.EventHandler(this.BtnChangeDocumentDBDirPath_Click);
            // 
            // TxtDocumentDBDirPath
            // 
            this.TxtDocumentDBDirPath.Location = new System.Drawing.Point(10, 28);
            this.TxtDocumentDBDirPath.Margin = new System.Windows.Forms.Padding(4);
            this.TxtDocumentDBDirPath.Name = "TxtDocumentDBDirPath";
            this.TxtDocumentDBDirPath.ReadOnly = true;
            this.TxtDocumentDBDirPath.Size = new System.Drawing.Size(577, 30);
            this.TxtDocumentDBDirPath.TabIndex = 0;
            // 
            // tableLayoutPanel2
            // 
            this.tableLayoutPanel2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.tableLayoutPanel2.ColumnCount = 2;
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 32.79766F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 67.20234F));
            this.tableLayoutPanel2.Controls.Add(this.lblVersion, 1, 0);
            this.tableLayoutPanel2.Controls.Add(this.label8, 0, 0);
            this.tableLayoutPanel2.Location = new System.Drawing.Point(20, 766);
            this.tableLayoutPanel2.Margin = new System.Windows.Forms.Padding(4);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            this.tableLayoutPanel2.RowCount = 1;
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel2.Size = new System.Drawing.Size(423, 38);
            this.tableLayoutPanel2.TabIndex = 23;
            // 
            // lblVersion
            // 
            this.lblVersion.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.lblVersion.AutoSize = true;
            this.lblVersion.Location = new System.Drawing.Point(142, 7);
            this.lblVersion.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblVersion.Name = "lblVersion";
            this.lblVersion.Size = new System.Drawing.Size(52, 23);
            this.lblVersion.TabIndex = 1;
            this.lblVersion.Text = "x.x.x";
            // 
            // label8
            // 
            this.label8.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(4, 7);
            this.label8.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(77, 23);
            this.label8.TabIndex = 0;
            this.label8.Text = "バージョン";
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.lsvTextExtensions);
            this.groupBox3.Controls.Add(this.label9);
            this.groupBox3.Controls.Add(this.label7);
            this.groupBox3.Controls.Add(this.BtnDeleteTextExt);
            this.groupBox3.Controls.Add(this.BtnAddTextExt);
            this.groupBox3.Location = new System.Drawing.Point(662, 160);
            this.groupBox3.Margin = new System.Windows.Forms.Padding(4);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Padding = new System.Windows.Forms.Padding(4);
            this.groupBox3.Size = new System.Drawing.Size(412, 338);
            this.groupBox3.TabIndex = 3;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "テキストファイルとして登録する拡張子";
            // 
            // lsvTextExtensions
            // 
            this.lsvTextExtensions.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader2});
            this.lsvTextExtensions.FullRowSelect = true;
            this.lsvTextExtensions.HideSelection = false;
            this.lsvTextExtensions.Location = new System.Drawing.Point(28, 27);
            this.lsvTextExtensions.Margin = new System.Windows.Forms.Padding(4);
            this.lsvTextExtensions.Name = "lsvTextExtensions";
            this.lsvTextExtensions.Size = new System.Drawing.Size(344, 187);
            this.lsvTextExtensions.TabIndex = 0;
            this.lsvTextExtensions.UseCompatibleStateImageBehavior = false;
            this.lsvTextExtensions.View = System.Windows.Forms.View.Details;
            this.lsvTextExtensions.SelectedIndexChanged += new System.EventHandler(this.lsvTextExtensions_SelectedIndexChanged);
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "拡張子";
            this.columnHeader1.Width = 80;
            // 
            // columnHeader2
            // 
            this.columnHeader2.Text = "ラベル";
            this.columnHeader2.Width = 150;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(26, 304);
            this.label9.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(323, 23);
            this.label9.TabIndex = 28;
            this.label9.Text = "そのファイルも検索対象とすることができます。";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(26, 279);
            this.label7.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(316, 23);
            this.label7.TabIndex = 25;
            this.label7.Text = "ソースファイルなどの拡張子を登録することで";
            // 
            // BtnDeleteTextExt
            // 
            this.BtnDeleteTextExt.Enabled = false;
            this.BtnDeleteTextExt.Location = new System.Drawing.Point(129, 225);
            this.BtnDeleteTextExt.Margin = new System.Windows.Forms.Padding(4);
            this.BtnDeleteTextExt.Name = "BtnDeleteTextExt";
            this.BtnDeleteTextExt.Size = new System.Drawing.Size(92, 34);
            this.BtnDeleteTextExt.TabIndex = 2;
            this.BtnDeleteTextExt.Text = "削除";
            this.BtnDeleteTextExt.UseVisualStyleBackColor = true;
            this.BtnDeleteTextExt.Click += new System.EventHandler(this.BtnDeleteTextExt_Click);
            // 
            // BtnAddTextExt
            // 
            this.BtnAddTextExt.Location = new System.Drawing.Point(28, 225);
            this.BtnAddTextExt.Margin = new System.Windows.Forms.Padding(4);
            this.BtnAddTextExt.Name = "BtnAddTextExt";
            this.BtnAddTextExt.Size = new System.Drawing.Size(92, 34);
            this.BtnAddTextExt.TabIndex = 1;
            this.BtnAddTextExt.Text = "追加";
            this.BtnAddTextExt.UseVisualStyleBackColor = true;
            this.BtnAddTextExt.Click += new System.EventHandler(this.BtnAddTextExt_Click);
            // 
            // lnkOpenDataFolder
            // 
            this.lnkOpenDataFolder.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lnkOpenDataFolder.AutoSize = true;
            this.lnkOpenDataFolder.Location = new System.Drawing.Point(18, 676);
            this.lnkOpenDataFolder.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lnkOpenDataFolder.Name = "lnkOpenDataFolder";
            this.lnkOpenDataFolder.Size = new System.Drawing.Size(145, 23);
            this.lnkOpenDataFolder.TabIndex = 24;
            this.lnkOpenDataFolder.TabStop = true;
            this.lnkOpenDataFolder.Text = "データフォルダを開く";
            this.lnkOpenDataFolder.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.lnkOpenDataFolder_LinkClicked);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.NumDisplayPageSizeForListView);
            this.groupBox1.Controls.Add(this.NumDisplayPageSizeForNormalView);
            this.groupBox1.Location = new System.Drawing.Point(662, 18);
            this.groupBox1.Margin = new System.Windows.Forms.Padding(4);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Padding = new System.Windows.Forms.Padding(4);
            this.groupBox1.Size = new System.Drawing.Size(278, 118);
            this.groupBox1.TabIndex = 31;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "一度に表示する検索結果件数";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(24, 70);
            this.label3.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(100, 23);
            this.label3.TabIndex = 30;
            this.label3.Text = "一覧表示：";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(24, 30);
            this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(100, 23);
            this.label2.TabIndex = 29;
            this.label2.Text = "通常表示：";
            // 
            // NumDisplayPageSizeForListView
            // 
            this.NumDisplayPageSizeForListView.Increment = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.NumDisplayPageSizeForListView.Location = new System.Drawing.Point(129, 68);
            this.NumDisplayPageSizeForListView.Margin = new System.Windows.Forms.Padding(4);
            this.NumDisplayPageSizeForListView.Maximum = new decimal(new int[] {
            99999,
            0,
            0,
            0});
            this.NumDisplayPageSizeForListView.Minimum = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.NumDisplayPageSizeForListView.Name = "NumDisplayPageSizeForListView";
            this.NumDisplayPageSizeForListView.Size = new System.Drawing.Size(110, 30);
            this.NumDisplayPageSizeForListView.TabIndex = 2;
            this.NumDisplayPageSizeForListView.Value = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.NumDisplayPageSizeForListView.ValueChanged += new System.EventHandler(this.NumDisplayPageSizeForListView_ValueChanged);
            // 
            // NumDisplayPageSizeForNormalView
            // 
            this.NumDisplayPageSizeForNormalView.Increment = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.NumDisplayPageSizeForNormalView.Location = new System.Drawing.Point(129, 27);
            this.NumDisplayPageSizeForNormalView.Margin = new System.Windows.Forms.Padding(4);
            this.NumDisplayPageSizeForNormalView.Maximum = new decimal(new int[] {
            99999,
            0,
            0,
            0});
            this.NumDisplayPageSizeForNormalView.Minimum = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.NumDisplayPageSizeForNormalView.Name = "NumDisplayPageSizeForNormalView";
            this.NumDisplayPageSizeForNormalView.Size = new System.Drawing.Size(110, 30);
            this.NumDisplayPageSizeForNormalView.TabIndex = 1;
            this.NumDisplayPageSizeForNormalView.Value = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.NumDisplayPageSizeForNormalView.ValueChanged += new System.EventHandler(this.NumDisplayPageSizeForNormalView_ValueChanged);
            // 
            // BtnRebootDebugMode
            // 
            this.BtnRebootDebugMode.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.BtnRebootDebugMode.Location = new System.Drawing.Point(577, 766);
            this.BtnRebootDebugMode.Margin = new System.Windows.Forms.Padding(4);
            this.BtnRebootDebugMode.Name = "BtnRebootDebugMode";
            this.BtnRebootDebugMode.Size = new System.Drawing.Size(402, 42);
            this.BtnRebootDebugMode.TabIndex = 32;
            this.BtnRebootDebugMode.Text = "デバッグモードで再起動 (不具合調査用)";
            this.BtnRebootDebugMode.UseVisualStyleBackColor = true;
            this.BtnRebootDebugMode.Click += new System.EventHandler(this.BtnRebootDebugMode_Click);
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.label10);
            this.groupBox4.Controls.Add(this.label5);
            this.groupBox4.Controls.Add(this.label4);
            this.groupBox4.Controls.Add(this.NumDocumentExtractTimeout);
            this.groupBox4.Location = new System.Drawing.Point(662, 517);
            this.groupBox4.Margin = new System.Windows.Forms.Padding(4);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Padding = new System.Windows.Forms.Padding(4);
            this.groupBox4.Size = new System.Drawing.Size(573, 88);
            this.groupBox4.TabIndex = 32;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "文書ファイル（Excel, Word, PDFなど）の登録処理";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(14, 40);
            this.label10.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(112, 23);
            this.label10.TabIndex = 32;
            this.label10.Text = "1ファイルあたり";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(21, 78);
            this.label5.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(0, 23);
            this.label5.TabIndex = 29;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(248, 40);
            this.label4.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(307, 23);
            this.label4.TabIndex = 31;
            this.label4.Text = "秒経っても完了しなければ登録失敗とする";
            // 
            // NumDocumentExtractTimeout
            // 
            this.NumDocumentExtractTimeout.Increment = new decimal(new int[] {
            5,
            0,
            0,
            0});
            this.NumDocumentExtractTimeout.Location = new System.Drawing.Point(130, 38);
            this.NumDocumentExtractTimeout.Margin = new System.Windows.Forms.Padding(4);
            this.NumDocumentExtractTimeout.Maximum = new decimal(new int[] {
            99999,
            0,
            0,
            0});
            this.NumDocumentExtractTimeout.Minimum = new decimal(new int[] {
            5,
            0,
            0,
            0});
            this.NumDocumentExtractTimeout.Name = "NumDocumentExtractTimeout";
            this.NumDocumentExtractTimeout.Size = new System.Drawing.Size(110, 30);
            this.NumDocumentExtractTimeout.TabIndex = 1;
            this.NumDocumentExtractTimeout.Value = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.NumDocumentExtractTimeout.ValueChanged += new System.EventHandler(this.NumDocumentExtractTimeout_ValueChanged);
            // 
            // lnlEventLogForm
            // 
            this.lnlEventLogForm.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lnlEventLogForm.AutoSize = true;
            this.lnlEventLogForm.Location = new System.Drawing.Point(18, 713);
            this.lnlEventLogForm.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lnlEventLogForm.Name = "lnlEventLogForm";
            this.lnlEventLogForm.Size = new System.Drawing.Size(317, 23);
            this.lnlEventLogForm.TabIndex = 33;
            this.lnlEventLogForm.TabStop = true;
            this.lnlEventLogForm.Text = "イベントログ（文書登録エラーなどの確認）";
            this.lnlEventLogForm.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.lnlEventLogForm_LinkClicked);
            // 
            // groupBox5
            // 
            this.groupBox5.Controls.Add(this.label12);
            this.groupBox5.Controls.Add(this.label13);
            this.groupBox5.Controls.Add(this.NumTextFileMaxSizeByMB);
            this.groupBox5.Location = new System.Drawing.Point(662, 622);
            this.groupBox5.Margin = new System.Windows.Forms.Padding(4);
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.Padding = new System.Windows.Forms.Padding(4);
            this.groupBox5.Size = new System.Drawing.Size(573, 88);
            this.groupBox5.TabIndex = 33;
            this.groupBox5.TabStop = false;
            this.groupBox5.Text = "テキストファイルの登録処理";
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(21, 78);
            this.label12.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(0, 23);
            this.label12.TabIndex = 29;
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(142, 36);
            this.label13.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(322, 23);
            this.label13.TabIndex = 31;
            this.label13.Text = "MB以上のテキストファイルは登録失敗とする";
            // 
            // NumTextFileMaxSizeByMB
            // 
            this.NumTextFileMaxSizeByMB.Increment = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.NumTextFileMaxSizeByMB.Location = new System.Drawing.Point(24, 34);
            this.NumTextFileMaxSizeByMB.Margin = new System.Windows.Forms.Padding(4);
            this.NumTextFileMaxSizeByMB.Maximum = new decimal(new int[] {
            1999,
            0,
            0,
            0});
            this.NumTextFileMaxSizeByMB.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.NumTextFileMaxSizeByMB.Name = "NumTextFileMaxSizeByMB";
            this.NumTextFileMaxSizeByMB.Size = new System.Drawing.Size(110, 30);
            this.NumTextFileMaxSizeByMB.TabIndex = 1;
            this.NumTextFileMaxSizeByMB.Value = new decimal(new int[] {
            200,
            0,
            0,
            0});
            this.NumTextFileMaxSizeByMB.ValueChanged += new System.EventHandler(this.NumTextFileMaxSizeByMB_ValueChanged);
            // 
            // DipswForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(144F, 144F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.ClientSize = new System.Drawing.Size(1262, 834);
            this.Controls.Add(this.groupBox5);
            this.Controls.Add(this.lnlEventLogForm);
            this.Controls.Add(this.groupBox4);
            this.Controls.Add(this.BtnRebootDebugMode);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.lnkOpenDataFolder);
            this.Controls.Add(this.BtnClose);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.tableLayoutPanel2);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.tblDebug);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.BtnClearAllData);
            this.Controls.Add(this.BtnClearDocumentDB);
            this.Font = new System.Drawing.Font("Meiryo UI", 9F);
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "DipswForm";
            this.ShowIcon = false;
            this.Text = "DIPSW";
            this.Load += new System.EventHandler(this.DipswForm_Load);
            this.tblDebug.ResumeLayout(false);
            this.tblDebug.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.tableLayoutPanel2.ResumeLayout(false);
            this.tableLayoutPanel2.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.NumDisplayPageSizeForListView)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.NumDisplayPageSizeForNormalView)).EndInit();
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.NumDocumentExtractTimeout)).EndInit();
            this.groupBox5.ResumeLayout(false);
            this.groupBox5.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.NumTextFileMaxSizeByMB)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Button BtnClearDocumentDB;
        private System.Windows.Forms.Button BtnClose;
        private System.Windows.Forms.Button BtnClearAllData;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TableLayoutPanel tblDebug;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label lblDocumentDBSize;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.TextBox TxtDocumentDBDirPath;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label lblVersion;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.Button BtnDeleteTextExt;
        private System.Windows.Forms.Button BtnAddTextExt;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.ListView lsvTextExtensions;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private System.Windows.Forms.LinkLabel lnkOpenDataFolder;
        private System.Windows.Forms.Button BtnResetDocumentDBDirPath;
        private System.Windows.Forms.Button BtnChangeDocumentDBDirPath;
        private System.Windows.Forms.Label LblDBDocumentDirUnchangable;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.NumericUpDown NumDisplayPageSizeForListView;
        private System.Windows.Forms.NumericUpDown NumDisplayPageSizeForNormalView;
        private System.Windows.Forms.Button BtnRebootDebugMode;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.NumericUpDown NumDocumentExtractTimeout;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.LinkLabel lnlEventLogForm;
        private System.Windows.Forms.GroupBox groupBox5;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.NumericUpDown NumTextFileMaxSizeByMB;
    }
}

