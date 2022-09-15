namespace InazumaSearch.Forms
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
            this.NumDocumentExtractTimeout = new System.Windows.Forms.NumericUpDown();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.tblDebug.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.tableLayoutPanel2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.NumDisplayPageSizeForListView)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.NumDisplayPageSizeForNormalView)).BeginInit();
            this.groupBox4.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.NumDocumentExtractTimeout)).BeginInit();
            this.SuspendLayout();
            // 
            // BtnClearDocumentDB
            // 
            this.BtnClearDocumentDB.Location = new System.Drawing.Point(12, 12);
            this.BtnClearDocumentDB.Name = "BtnClearDocumentDB";
            this.BtnClearDocumentDB.Size = new System.Drawing.Size(268, 23);
            this.BtnClearDocumentDB.TabIndex = 0;
            this.BtnClearDocumentDB.Text = "クロールした文書データをクリア";
            this.BtnClearDocumentDB.UseVisualStyleBackColor = true;
            this.BtnClearDocumentDB.Click += new System.EventHandler(this.BtnClearCrawledData_Click);
            // 
            // BtnClose
            // 
            this.BtnClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.BtnClose.Location = new System.Drawing.Point(608, 356);
            this.BtnClose.Name = "BtnClose";
            this.BtnClose.Size = new System.Drawing.Size(110, 28);
            this.BtnClose.TabIndex = 4;
            this.BtnClose.Text = "閉じる";
            this.BtnClose.UseVisualStyleBackColor = true;
            this.BtnClose.Click += new System.EventHandler(this.BtnClose_Click);
            // 
            // BtnClearAllData
            // 
            this.BtnClearAllData.Location = new System.Drawing.Point(12, 53);
            this.BtnClearAllData.Name = "BtnClearAllData";
            this.BtnClearAllData.Size = new System.Drawing.Size(268, 23);
            this.BtnClearAllData.TabIndex = 1;
            this.BtnClearAllData.Text = "全てのデータを初期化";
            this.BtnClearAllData.UseVisualStyleBackColor = true;
            this.BtnClearAllData.Click += new System.EventHandler(this.BtnClearAllData_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 79);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(348, 12);
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
            this.tblDebug.Location = new System.Drawing.Point(177, 324);
            this.tblDebug.Name = "tblDebug";
            this.tblDebug.RowCount = 1;
            this.tblDebug.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 47.76119F));
            this.tblDebug.Size = new System.Drawing.Size(235, 31);
            this.tblDebug.TabIndex = 22;
            this.tblDebug.Visible = false;
            // 
            // lblDocumentDBSize
            // 
            this.lblDocumentDBSize.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.lblDocumentDBSize.AutoSize = true;
            this.lblDocumentDBSize.Location = new System.Drawing.Point(180, 9);
            this.lblDocumentDBSize.Name = "lblDocumentDBSize";
            this.lblDocumentDBSize.Size = new System.Drawing.Size(52, 12);
            this.lblDocumentDBSize.TabIndex = 1;
            this.lblDocumentDBSize.Text = "9,999 MB";
            // 
            // label6
            // 
            this.label6.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(3, 9);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(115, 12);
            this.label6.TabIndex = 0;
            this.label6.Text = "文書データベースサイズ";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.LblDBDocumentDirUnchangable);
            this.groupBox2.Controls.Add(this.BtnResetDocumentDBDirPath);
            this.groupBox2.Controls.Add(this.BtnChangeDocumentDBDirPath);
            this.groupBox2.Controls.Add(this.TxtDocumentDBDirPath);
            this.groupBox2.Location = new System.Drawing.Point(13, 130);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(399, 80);
            this.groupBox2.TabIndex = 2;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "文書データベースの保存先";
            // 
            // LblDBDocumentDirUnchangable
            // 
            this.LblDBDocumentDirUnchangable.BackColor = System.Drawing.SystemColors.Control;
            this.LblDBDocumentDirUnchangable.ForeColor = System.Drawing.Color.Gray;
            this.LblDBDocumentDirUnchangable.Location = new System.Drawing.Point(153, 44);
            this.LblDBDocumentDirUnchangable.Name = "LblDBDocumentDirUnchangable";
            this.LblDBDocumentDirUnchangable.Size = new System.Drawing.Size(240, 23);
            this.LblDBDocumentDirUnchangable.TabIndex = 25;
            this.LblDBDocumentDirUnchangable.Text = "（ポータブル版では変更不可）";
            this.LblDBDocumentDirUnchangable.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.LblDBDocumentDirUnchangable.Visible = false;
            // 
            // BtnResetDocumentDBDirPath
            // 
            this.BtnResetDocumentDBDirPath.Enabled = false;
            this.BtnResetDocumentDBDirPath.Location = new System.Drawing.Point(174, 44);
            this.BtnResetDocumentDBDirPath.Name = "BtnResetDocumentDBDirPath";
            this.BtnResetDocumentDBDirPath.Size = new System.Drawing.Size(152, 23);
            this.BtnResetDocumentDBDirPath.TabIndex = 30;
            this.BtnResetDocumentDBDirPath.Text = "初期設定フォルダに戻す";
            this.BtnResetDocumentDBDirPath.UseVisualStyleBackColor = true;
            this.BtnResetDocumentDBDirPath.Click += new System.EventHandler(this.BtnResetDocumentDBDirPath_Click);
            // 
            // BtnChangeDocumentDBDirPath
            // 
            this.BtnChangeDocumentDBDirPath.Location = new System.Drawing.Point(332, 44);
            this.BtnChangeDocumentDBDirPath.Name = "BtnChangeDocumentDBDirPath";
            this.BtnChangeDocumentDBDirPath.Size = new System.Drawing.Size(61, 23);
            this.BtnChangeDocumentDBDirPath.TabIndex = 29;
            this.BtnChangeDocumentDBDirPath.Text = "変更";
            this.BtnChangeDocumentDBDirPath.UseVisualStyleBackColor = true;
            this.BtnChangeDocumentDBDirPath.Click += new System.EventHandler(this.BtnChangeDocumentDBDirPath_Click);
            // 
            // TxtDocumentDBDirPath
            // 
            this.TxtDocumentDBDirPath.Location = new System.Drawing.Point(7, 19);
            this.TxtDocumentDBDirPath.Name = "TxtDocumentDBDirPath";
            this.TxtDocumentDBDirPath.ReadOnly = true;
            this.TxtDocumentDBDirPath.Size = new System.Drawing.Size(386, 19);
            this.TxtDocumentDBDirPath.TabIndex = 0;
            // 
            // tableLayoutPanel2
            // 
            this.tableLayoutPanel2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.tableLayoutPanel2.ColumnCount = 2;
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 20.23121F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 79.76878F));
            this.tableLayoutPanel2.Controls.Add(this.lblVersion, 1, 0);
            this.tableLayoutPanel2.Controls.Add(this.label8, 0, 0);
            this.tableLayoutPanel2.Location = new System.Drawing.Point(13, 359);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            this.tableLayoutPanel2.RowCount = 1;
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 45.28302F));
            this.tableLayoutPanel2.Size = new System.Drawing.Size(282, 25);
            this.tableLayoutPanel2.TabIndex = 23;
            // 
            // lblVersion
            // 
            this.lblVersion.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.lblVersion.AutoSize = true;
            this.lblVersion.Location = new System.Drawing.Point(60, 6);
            this.lblVersion.Name = "lblVersion";
            this.lblVersion.Size = new System.Drawing.Size(27, 12);
            this.lblVersion.TabIndex = 1;
            this.lblVersion.Text = "x.x.x";
            // 
            // label8
            // 
            this.label8.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(3, 6);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(50, 12);
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
            this.groupBox3.Location = new System.Drawing.Point(441, 107);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(275, 225);
            this.groupBox3.TabIndex = 3;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "テキストファイルとして登録する拡張子";
            // 
            // lsvTextExtensions
            // 
            this.lsvTextExtensions.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader2});
            this.lsvTextExtensions.Font = new System.Drawing.Font("ＭＳ ゴシック", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.lsvTextExtensions.FullRowSelect = true;
            this.lsvTextExtensions.HideSelection = false;
            this.lsvTextExtensions.Location = new System.Drawing.Point(19, 18);
            this.lsvTextExtensions.Name = "lsvTextExtensions";
            this.lsvTextExtensions.Size = new System.Drawing.Size(231, 126);
            this.lsvTextExtensions.TabIndex = 0;
            this.lsvTextExtensions.UseCompatibleStateImageBehavior = false;
            this.lsvTextExtensions.View = System.Windows.Forms.View.Details;
            this.lsvTextExtensions.SelectedIndexChanged += new System.EventHandler(this.lsvTextExtensions_SelectedIndexChanged);
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "拡張子";
            // 
            // columnHeader2
            // 
            this.columnHeader2.Text = "ラベル";
            this.columnHeader2.Width = 121;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(17, 203);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(214, 12);
            this.label9.TabIndex = 28;
            this.label9.Text = "そのファイルも検索対象とすることができます。";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(17, 186);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(210, 12);
            this.label7.TabIndex = 25;
            this.label7.Text = "ソースファイルなどの拡張子を登録することで";
            // 
            // BtnDeleteTextExt
            // 
            this.BtnDeleteTextExt.Enabled = false;
            this.BtnDeleteTextExt.Location = new System.Drawing.Point(86, 150);
            this.BtnDeleteTextExt.Name = "BtnDeleteTextExt";
            this.BtnDeleteTextExt.Size = new System.Drawing.Size(61, 23);
            this.BtnDeleteTextExt.TabIndex = 2;
            this.BtnDeleteTextExt.Text = "削除";
            this.BtnDeleteTextExt.UseVisualStyleBackColor = true;
            this.BtnDeleteTextExt.Click += new System.EventHandler(this.BtnDeleteTextExt_Click);
            // 
            // BtnAddTextExt
            // 
            this.BtnAddTextExt.Location = new System.Drawing.Point(19, 150);
            this.BtnAddTextExt.Name = "BtnAddTextExt";
            this.BtnAddTextExt.Size = new System.Drawing.Size(61, 23);
            this.BtnAddTextExt.TabIndex = 1;
            this.BtnAddTextExt.Text = "追加";
            this.BtnAddTextExt.UseVisualStyleBackColor = true;
            this.BtnAddTextExt.Click += new System.EventHandler(this.BtnAddTextExt_Click);
            // 
            // lnkOpenDataFolder
            // 
            this.lnkOpenDataFolder.AutoSize = true;
            this.lnkOpenDataFolder.Location = new System.Drawing.Point(317, 363);
            this.lnkOpenDataFolder.Name = "lnkOpenDataFolder";
            this.lnkOpenDataFolder.Size = new System.Drawing.Size(95, 12);
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
            this.groupBox1.Location = new System.Drawing.Point(441, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(185, 79);
            this.groupBox1.TabIndex = 31;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "一度に表示する結果件数";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(16, 47);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(59, 12);
            this.label3.TabIndex = 30;
            this.label3.Text = "一覧表示：";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(16, 20);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(59, 12);
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
            this.NumDisplayPageSizeForListView.Location = new System.Drawing.Point(86, 45);
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
            this.NumDisplayPageSizeForListView.Size = new System.Drawing.Size(73, 19);
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
            this.NumDisplayPageSizeForNormalView.Location = new System.Drawing.Point(86, 18);
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
            this.NumDisplayPageSizeForNormalView.Size = new System.Drawing.Size(73, 19);
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
            this.BtnRebootDebugMode.Location = new System.Drawing.Point(144, 361);
            this.BtnRebootDebugMode.Name = "BtnRebootDebugMode";
            this.BtnRebootDebugMode.Size = new System.Drawing.Size(268, 23);
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
            this.groupBox4.Location = new System.Drawing.Point(14, 221);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(398, 59);
            this.groupBox4.TabIndex = 32;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "文書ファイル（Excel, Word, PDFなど）の内容抽出処理";
            // 
            // NumDocumentExtractTimeout
            // 
            this.NumDocumentExtractTimeout.Increment = new decimal(new int[] {
            5,
            0,
            0,
            0});
            this.NumDocumentExtractTimeout.Location = new System.Drawing.Point(87, 25);
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
            this.NumDocumentExtractTimeout.Size = new System.Drawing.Size(73, 19);
            this.NumDocumentExtractTimeout.TabIndex = 1;
            this.NumDocumentExtractTimeout.Value = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.NumDocumentExtractTimeout.ValueChanged += new System.EventHandler(this.NumDocumentExtractTimeout_ValueChanged);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(165, 27);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(164, 12);
            this.label4.TabIndex = 31;
            this.label4.Text = "秒経っても完了しなければスキップ";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(14, 52);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(0, 12);
            this.label5.TabIndex = 29;
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(9, 27);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(72, 12);
            this.label10.TabIndex = 32;
            this.label10.Text = "1ファイルあたり";
            // 
            // DipswForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(748, 404);
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
    }
}

