namespace InazumaSearch.Forms
{
    partial class SearchFolderSelectDialog
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
            this.BtnCancel = new System.Windows.Forms.Button();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.TreeFolder = new System.Windows.Forms.TreeView();
            this.delayTimer = new System.Windows.Forms.Timer(this.components);
            this.BtnDecide = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // BtnCancel
            // 
            this.BtnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.BtnCancel.Location = new System.Drawing.Point(337, 385);
            this.BtnCancel.Name = "BtnCancel";
            this.BtnCancel.Size = new System.Drawing.Size(89, 33);
            this.BtnCancel.TabIndex = 15;
            this.BtnCancel.Text = "キャンセル";
            this.BtnCancel.UseVisualStyleBackColor = true;
            this.BtnCancel.Click += new System.EventHandler(this.BtnClose_Click);
            // 
            // TreeFolder
            // 
            this.TreeFolder.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.TreeFolder.Location = new System.Drawing.Point(25, 60);
            this.TreeFolder.Name = "TreeFolder";
            this.TreeFolder.Size = new System.Drawing.Size(387, 277);
            this.TreeFolder.TabIndex = 16;
            this.TreeFolder.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.TreeFolder_AfterSelect);
            // 
            // delayTimer
            // 
            this.delayTimer.Enabled = true;
            this.delayTimer.Tick += new System.EventHandler(this.delayTimer_Tick);
            // 
            // BtnDecide
            // 
            this.BtnDecide.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.BtnDecide.Location = new System.Drawing.Point(206, 385);
            this.BtnDecide.Name = "BtnDecide";
            this.BtnDecide.Size = new System.Drawing.Size(105, 33);
            this.BtnDecide.TabIndex = 18;
            this.BtnDecide.Text = "確定";
            this.BtnDecide.UseVisualStyleBackColor = true;
            this.BtnDecide.Click += new System.EventHandler(this.BtnDecide_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(22, 15);
            this.label1.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(209, 15);
            this.label1.TabIndex = 19;
            this.label1.Text = "検索対象とするフォルダを選択してください。";
            // 
            // label2
            // 
            this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label2.AutoSize = true;
            this.label2.ForeColor = System.Drawing.Color.Gray;
            this.label2.Location = new System.Drawing.Point(22, 344);
            this.label2.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(231, 15);
            this.label2.TabIndex = 20;
            this.label2.Text = "※文書が存在しないフォルダは表示されません。";
            // 
            // SearchFolderSelectDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.ClientSize = new System.Drawing.Size(438, 431);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.BtnDecide);
            this.Controls.Add(this.TreeFolder);
            this.Controls.Add(this.BtnCancel);
            this.Font = new System.Drawing.Font("Meiryo UI", 9F);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "SearchFolderSelectDialog";
            this.ShowIcon = false;
            this.Text = "フォルダ選択";
            this.Load += new System.EventHandler(this.SearchFolderSelectDialog_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Button BtnCancel;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.TreeView TreeFolder;
        private System.Windows.Forms.Timer delayTimer;
        private System.Windows.Forms.Button BtnDecide;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
    }
}

