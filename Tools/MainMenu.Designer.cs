namespace Tools
{
    partial class MainMenu
    {
        /// <summary>
        /// 必要なデザイナー変数です。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 使用中のリソースをすべてクリーンアップします。
        /// </summary>
        /// <param name="disposing">マネージド リソースを破棄する場合は true を指定し、その他の場合は false を指定します。</param>
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
            this.BtnDummyDocumentGenerator = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // BtnDummyDocumentGenerator
            // 
            this.BtnDummyDocumentGenerator.Location = new System.Drawing.Point(242, 87);
            this.BtnDummyDocumentGenerator.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.BtnDummyDocumentGenerator.Name = "BtnDummyDocumentGenerator";
            this.BtnDummyDocumentGenerator.Size = new System.Drawing.Size(388, 47);
            this.BtnDummyDocumentGenerator.TabIndex = 0;
            this.BtnDummyDocumentGenerator.Text = "ダミー文書生成";
            this.BtnDummyDocumentGenerator.UseVisualStyleBackColor = true;
            this.BtnDummyDocumentGenerator.Click += new System.EventHandler(this.BtnDummyDocumentGenerator_Click);
            // 
            // MainMenu
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(11F, 23F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(880, 287);
            this.Controls.Add(this.BtnDummyDocumentGenerator);
            this.Font = new System.Drawing.Font("Meiryo UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.Name = "MainMenu";
            this.Text = "Form1";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button BtnDummyDocumentGenerator;
    }
}

