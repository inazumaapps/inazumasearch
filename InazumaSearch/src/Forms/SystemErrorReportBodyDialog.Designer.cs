namespace InazumaSearch.Forms
{
    partial class SystemErrorReportBodyDialog
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
            this.TxtBody = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // BtnQuit
            // 
            this.BtnQuit.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.BtnQuit.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.BtnQuit.Location = new System.Drawing.Point(516, 293);
            this.BtnQuit.Name = "BtnQuit";
            this.BtnQuit.Size = new System.Drawing.Size(82, 31);
            this.BtnQuit.TabIndex = 1;
            this.BtnQuit.Text = "閉じる";
            this.BtnQuit.UseVisualStyleBackColor = true;
            this.BtnQuit.Click += new System.EventHandler(this.BtnQuit_Click);
            // 
            // TxtBody
            // 
            this.TxtBody.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.TxtBody.Font = new System.Drawing.Font("ＭＳ ゴシック", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.TxtBody.Location = new System.Drawing.Point(13, 13);
            this.TxtBody.Multiline = true;
            this.TxtBody.Name = "TxtBody";
            this.TxtBody.ReadOnly = true;
            this.TxtBody.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.TxtBody.Size = new System.Drawing.Size(585, 274);
            this.TxtBody.TabIndex = 2;
            // 
            // SystemErrorReportBodyDialog
            // 
            this.AcceptButton = this.BtnQuit;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(610, 336);
            this.Controls.Add(this.TxtBody);
            this.Controls.Add(this.BtnQuit);
            this.Name = "SystemErrorReportBodyDialog";
            this.ShowIcon = false;
            this.Text = "レポートの内容";
            this.TopMost = true;
            this.Load += new System.EventHandler(this.SystemErrorDialog_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Button BtnQuit;
        private System.Windows.Forms.TextBox TxtBody;
    }
}

