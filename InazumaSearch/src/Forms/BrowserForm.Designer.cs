namespace InazumaSearch.Forms
{
    partial class BrowserForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(BrowserForm));
            this.StatusBar = new System.Windows.Forms.StatusStrip();
            this.StlBackgroundCrawl = new System.Windows.Forms.ToolStripStatusLabel();
            this.StatusBar.SuspendLayout();
            this.SuspendLayout();
            // 
            // StatusBar
            // 
            this.StatusBar.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.StlBackgroundCrawl});
            this.StatusBar.Location = new System.Drawing.Point(0, 620);
            this.StatusBar.Name = "StatusBar";
            this.StatusBar.Size = new System.Drawing.Size(1182, 22);
            this.StatusBar.TabIndex = 0;
            // 
            // StlBackgroundCrawl
            // 
            this.StlBackgroundCrawl.Name = "StlBackgroundCrawl";
            this.StlBackgroundCrawl.Size = new System.Drawing.Size(1136, 17);
            this.StlBackgroundCrawl.Spring = true;
            this.StlBackgroundCrawl.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // BrowserForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1182, 642);
            this.Controls.Add(this.StatusBar);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "BrowserForm";
            this.Text = "Inazuma Search";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Browser_FormClosing);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.BrowserForm_FormClosed);
            this.Load += new System.EventHandler(this.BrowserForm_Load);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.BrowserForm_KeyDown);
            this.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.BrowserForm_KeyPress);
            this.StatusBar.ResumeLayout(false);
            this.StatusBar.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.StatusStrip StatusBar;
        private System.Windows.Forms.ToolStripStatusLabel StlBackgroundCrawl;
    }
}