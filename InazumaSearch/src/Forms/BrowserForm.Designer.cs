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
            this.SuspendLayout();
            // 
            // BrowserForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1182, 642);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "BrowserForm";
            this.Text = "Inazuma Search x.x.x";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Browser_FormClosing);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.BrowserForm_FormClosed);
            this.Load += new System.EventHandler(this.BrowserForm_Load);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.BrowserForm_KeyDown);
            this.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.BrowserForm_KeyPress);
            this.ResumeLayout(false);

        }

        #endregion
    }
}