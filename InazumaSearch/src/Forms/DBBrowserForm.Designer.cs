namespace InazumaSearch.Forms
{
    partial class DBBrowserForm
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
            this.BtnClose = new System.Windows.Forms.Button();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.TreeFolder = new System.Windows.Forms.TreeView();
            this.TreeIconImageList = new System.Windows.Forms.ImageList(this.components);
            this.BtnRefresh = new System.Windows.Forms.Button();
            this.delayTimer = new System.Windows.Forms.Timer(this.components);
            this.LstFile = new System.Windows.Forms.ListView();
            this.FileName = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.FileUpdated = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.FileSize = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.SuspendLayout();
            // 
            // BtnClose
            // 
            this.BtnClose.Location = new System.Drawing.Point(829, 355);
            this.BtnClose.Name = "BtnClose";
            this.BtnClose.Size = new System.Drawing.Size(75, 23);
            this.BtnClose.TabIndex = 15;
            this.BtnClose.Text = "閉じる";
            this.BtnClose.UseVisualStyleBackColor = true;
            this.BtnClose.Click += new System.EventHandler(this.BtnClose_Click);
            // 
            // TreeFolder
            // 
            this.TreeFolder.ImageIndex = 0;
            this.TreeFolder.ImageList = this.TreeIconImageList;
            this.TreeFolder.Location = new System.Drawing.Point(22, 54);
            this.TreeFolder.Name = "TreeFolder";
            this.TreeFolder.SelectedImageIndex = 0;
            this.TreeFolder.Size = new System.Drawing.Size(288, 295);
            this.TreeFolder.TabIndex = 16;
            this.TreeFolder.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.TreeFolder_AfterSelect);
            // 
            // TreeIconImageList
            // 
            this.TreeIconImageList.ColorDepth = System.Windows.Forms.ColorDepth.Depth32Bit;
            this.TreeIconImageList.ImageSize = new System.Drawing.Size(16, 16);
            this.TreeIconImageList.TransparentColor = System.Drawing.Color.Black;
            // 
            // BtnRefresh
            // 
            this.BtnRefresh.Location = new System.Drawing.Point(785, 12);
            this.BtnRefresh.Name = "BtnRefresh";
            this.BtnRefresh.Size = new System.Drawing.Size(119, 23);
            this.BtnRefresh.TabIndex = 17;
            this.BtnRefresh.Text = "最新の表示に更新";
            this.BtnRefresh.UseVisualStyleBackColor = true;
            this.BtnRefresh.Click += new System.EventHandler(this.BtnRefresh_Click);
            // 
            // delayTimer
            // 
            this.delayTimer.Enabled = true;
            this.delayTimer.Tick += new System.EventHandler(this.delayTimer_Tick);
            // 
            // LstFile
            // 
            this.LstFile.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.FileName,
            this.FileUpdated,
            this.FileSize});
            this.LstFile.Location = new System.Drawing.Point(316, 54);
            this.LstFile.Name = "LstFile";
            this.LstFile.Size = new System.Drawing.Size(588, 295);
            this.LstFile.TabIndex = 18;
            this.LstFile.UseCompatibleStateImageBehavior = false;
            this.LstFile.View = System.Windows.Forms.View.Details;
            // 
            // FileName
            // 
            this.FileName.Text = "ファイル名";
            this.FileName.Width = 312;
            // 
            // FileUpdated
            // 
            this.FileUpdated.Text = "更新日付";
            this.FileUpdated.Width = 99;
            // 
            // FileSize
            // 
            this.FileSize.Text = "サイズ";
            this.FileSize.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.FileSize.Width = 86;
            // 
            // DBBrowserForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(916, 390);
            this.Controls.Add(this.LstFile);
            this.Controls.Add(this.BtnRefresh);
            this.Controls.Add(this.TreeFolder);
            this.Controls.Add(this.BtnClose);
            this.Name = "DBBrowserForm";
            this.Text = "データベースブラウザ";
            this.Load += new System.EventHandler(this.DBBrowserForm_Load);
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Button BtnClose;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.TreeView TreeFolder;
        private System.Windows.Forms.ImageList TreeIconImageList;
        private System.Windows.Forms.Button BtnRefresh;
        private System.Windows.Forms.Timer delayTimer;
        private System.Windows.Forms.ListView LstFile;
        private System.Windows.Forms.ColumnHeader FileName;
        private System.Windows.Forms.ColumnHeader FileUpdated;
        private System.Windows.Forms.ColumnHeader FileSize;
    }
}

