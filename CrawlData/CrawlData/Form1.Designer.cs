namespace CrawlData
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            btnCrawl = new System.Windows.Forms.Button();
            rdoWin = new System.Windows.Forms.RadioButton();
            rdoMac = new System.Windows.Forms.RadioButton();
            SuspendLayout();
            // 
            // btnCrawl
            // 
            btnCrawl.Location = new System.Drawing.Point(380, 282);
            btnCrawl.Name = "btnCrawl";
            btnCrawl.Size = new System.Drawing.Size(296, 89);
            btnCrawl.TabIndex = 0;
            btnCrawl.Text = "Crawling";
            btnCrawl.UseVisualStyleBackColor = true;
            btnCrawl.Click += btnCrawl_Click;
            // 
            // rdoWin
            // 
            rdoWin.AutoSize = true;
            rdoWin.Checked = true;
            rdoWin.Location = new System.Drawing.Point(341, 157);
            rdoWin.Name = "rdoWin";
            rdoWin.Size = new System.Drawing.Size(87, 36);
            rdoWin.TabIndex = 1;
            rdoWin.TabStop = true;
            rdoWin.Text = "Win";
            rdoWin.UseVisualStyleBackColor = true;
            // 
            // rdoMac
            // 
            rdoMac.AutoSize = true;
            rdoMac.Location = new System.Drawing.Point(617, 157);
            rdoMac.Name = "rdoMac";
            rdoMac.Size = new System.Drawing.Size(90, 36);
            rdoMac.TabIndex = 2;
            rdoMac.Text = "Mac";
            rdoMac.UseVisualStyleBackColor = true;
            // 
            // Form1
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(13F, 32F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(1100, 647);
            Controls.Add(rdoMac);
            Controls.Add(rdoWin);
            Controls.Add(btnCrawl);
            Name = "Form1";
            Text = "Form1";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private System.Windows.Forms.Button btnCrawl;
        private System.Windows.Forms.RadioButton rdoWin;
        private System.Windows.Forms.RadioButton rdoMac;
    }
}
