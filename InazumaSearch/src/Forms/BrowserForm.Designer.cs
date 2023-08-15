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
            this.BrowserPanel = new System.Windows.Forms.Panel();
            this.StatusBar.SuspendLayout();
            this.SuspendLayout();
            // 
            // StatusBar
            // 
            this.StatusBar.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.StlBackgroundCrawl});
            this.StatusBar.Location = new System.Drawing.Point(0, 780);
            this.StatusBar.Name = "StatusBar";
            this.StatusBar.Padding = new System.Windows.Forms.Padding(1, 0, 16, 0);
            this.StatusBar.Size = new System.Drawing.Size(1379, 22);
            this.StatusBar.TabIndex = 0;
            // 
            // StlBackgroundCrawl
            // 
            this.StlBackgroundCrawl.Name = "StlBackgroundCrawl";
            this.StlBackgroundCrawl.Size = new System.Drawing.Size(1362, 17);
            this.StlBackgroundCrawl.Spring = true;
            this.StlBackgroundCrawl.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // BrowserPanel
            // 
            this.BrowserPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.BrowserPanel.Location = new System.Drawing.Point(0, 0);
            this.BrowserPanel.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.BrowserPanel.Name = "BrowserPanel";
            this.BrowserPanel.Size = new System.Drawing.Size(1379, 780);
            this.BrowserPanel.TabIndex = 1;
            // 
            // BrowserForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1379, 802);
            this.Controls.Add(this.BrowserPanel);
            this.Controls.Add(this.StatusBar);
            this.Font = new System.Drawing.Font("Meiryo UI", 9F);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.Name = "BrowserForm";
            this.Text = "Inazuma Search";
            this.TopMost = true;
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.BrowserForm_FormClosed);
            this.Load += new System.EventHandler(this.BrowserForm_Load);
            this.Shown += new System.EventHandler(this.BrowserForm_Shown);
            this.StatusBar.ResumeLayout(false);
            this.StatusBar.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.StatusStrip StatusBar;
        private System.Windows.Forms.ToolStripStatusLabel StlBackgroundCrawl;
        private System.Windows.Forms.Panel BrowserPanel;
    }
}