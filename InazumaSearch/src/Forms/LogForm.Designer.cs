namespace InazumaSearch.Forms
{
    partial class LogForm
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
            this.BtnQuit = new System.Windows.Forms.Button();
            this.LogConsole = new System.Windows.Forms.RichTextBox();
            this.SuspendLayout();
            // 
            // BtnQuit
            // 
            this.BtnQuit.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.BtnQuit.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.BtnQuit.Location = new System.Drawing.Point(138, 332);
            this.BtnQuit.Name = "BtnQuit";
            this.BtnQuit.Size = new System.Drawing.Size(114, 31);
            this.BtnQuit.TabIndex = 1;
            this.BtnQuit.Text = "閉じる";
            this.BtnQuit.UseVisualStyleBackColor = true;
            this.BtnQuit.Click += new System.EventHandler(this.BtnQuit_Click);
            // 
            // LogConsole
            // 
            this.LogConsole.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.LogConsole.Location = new System.Drawing.Point(0, 0);
            this.LogConsole.Name = "LogConsole";
            this.LogConsole.Size = new System.Drawing.Size(353, 326);
            this.LogConsole.TabIndex = 2;
            this.LogConsole.Text = "";
            // 
            // LogForm
            // 
            this.AcceptButton = this.BtnQuit;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(354, 375);
            this.Controls.Add(this.LogConsole);
            this.Controls.Add(this.BtnQuit);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "LogForm";
            this.ShowIcon = false;
            this.Text = "動作ログ";
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Button BtnQuit;
        private System.Windows.Forms.RichTextBox LogConsole;
    }
}

