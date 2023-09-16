namespace Tools
{
    partial class DummyDocumentGenerator
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
            this.BtnExecute = new System.Windows.Forms.Button();
            this.NumSeed = new System.Windows.Forms.NumericUpDown();
            this.label1 = new System.Windows.Forms.Label();
            this.NumDocCount = new System.Windows.Forms.NumericUpDown();
            this.label2 = new System.Windows.Forms.Label();
            this.TxtOutputFolderPath = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.ChkMakeTimestampSubFolder = new System.Windows.Forms.CheckBox();
            ((System.ComponentModel.ISupportInitialize)(this.NumSeed)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.NumDocCount)).BeginInit();
            this.SuspendLayout();
            // 
            // BtnExecute
            // 
            this.BtnExecute.Location = new System.Drawing.Point(325, 266);
            this.BtnExecute.Name = "BtnExecute";
            this.BtnExecute.Size = new System.Drawing.Size(227, 53);
            this.BtnExecute.TabIndex = 0;
            this.BtnExecute.Text = "実行";
            this.BtnExecute.UseVisualStyleBackColor = true;
            this.BtnExecute.Click += new System.EventHandler(this.BtnExecute_Click);
            // 
            // NumSeed
            // 
            this.NumSeed.Location = new System.Drawing.Point(145, 32);
            this.NumSeed.Name = "NumSeed";
            this.NumSeed.Size = new System.Drawing.Size(120, 30);
            this.NumSeed.TabIndex = 2;
            this.NumSeed.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(29, 32);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(89, 23);
            this.label1.TabIndex = 3;
            this.label1.Text = "乱数シード";
            // 
            // NumDocCount
            // 
            this.NumDocCount.Location = new System.Drawing.Point(560, 32);
            this.NumDocCount.Maximum = new decimal(new int[] {
            999999,
            0,
            0,
            0});
            this.NumDocCount.Name = "NumDocCount";
            this.NumDocCount.Size = new System.Drawing.Size(147, 30);
            this.NumDocCount.TabIndex = 4;
            this.NumDocCount.Value = new decimal(new int[] {
            5000,
            0,
            0,
            0});
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(424, 34);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(100, 23);
            this.label2.TabIndex = 5;
            this.label2.Text = "生成文書数";
            // 
            // TxtOutputFolderPath
            // 
            this.TxtOutputFolderPath.Location = new System.Drawing.Point(165, 99);
            this.TxtOutputFolderPath.Name = "TxtOutputFolderPath";
            this.TxtOutputFolderPath.Size = new System.Drawing.Size(632, 30);
            this.TxtOutputFolderPath.TabIndex = 6;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(29, 99);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(116, 23);
            this.label3.TabIndex = 7;
            this.label3.Text = "出力先フォルダ";
            // 
            // ChkMakeTimestampSubFolder
            // 
            this.ChkMakeTimestampSubFolder.AutoSize = true;
            this.ChkMakeTimestampSubFolder.Location = new System.Drawing.Point(179, 136);
            this.ChkMakeTimestampSubFolder.Name = "ChkMakeTimestampSubFolder";
            this.ChkMakeTimestampSubFolder.Size = new System.Drawing.Size(627, 27);
            this.ChkMakeTimestampSubFolder.TabIndex = 8;
            this.ChkMakeTimestampSubFolder.Text = "実行日時でサブフォルダを作る（例：C:\\Documents\\20230910_102533）";
            this.ChkMakeTimestampSubFolder.UseVisualStyleBackColor = true;
            // 
            // DummyDocumentGenerator
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(144F, 144F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.ClientSize = new System.Drawing.Size(895, 350);
            this.Controls.Add(this.ChkMakeTimestampSubFolder);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.TxtOutputFolderPath);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.NumDocCount);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.NumSeed);
            this.Controls.Add(this.BtnExecute);
            this.Font = new System.Drawing.Font("Meiryo UI", 9F);
            this.Name = "DummyDocumentGenerator";
            this.Text = "DummyDocumentGenerator";
            this.Load += new System.EventHandler(this.DummyDocumentGenerator_Load);
            ((System.ComponentModel.ISupportInitialize)(this.NumSeed)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.NumDocCount)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button BtnExecute;
        private System.Windows.Forms.NumericUpDown NumSeed;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.NumericUpDown NumDocCount;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox TxtOutputFolderPath;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.CheckBox ChkMakeTimestampSubFolder;
    }
}