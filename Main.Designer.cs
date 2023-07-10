namespace CrawlData
{
    partial class Main
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
            label1 = new System.Windows.Forms.Label();
            txtTargetUserNames = new System.Windows.Forms.TextBox();
            label2 = new System.Windows.Forms.Label();
            SuspendLayout();
            // 
            // btnCrawl
            // 
            btnCrawl.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            btnCrawl.Location = new System.Drawing.Point(409, 454);
            btnCrawl.Name = "btnCrawl";
            btnCrawl.Size = new System.Drawing.Size(296, 89);
            btnCrawl.TabIndex = 0;
            btnCrawl.Text = "Crawling";
            btnCrawl.UseVisualStyleBackColor = true;
            btnCrawl.Click += btnCrawl_Click;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new System.Drawing.Font("Segoe UI", 10.875F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            label1.Location = new System.Drawing.Point(37, 78);
            label1.Name = "label1";
            label1.Size = new System.Drawing.Size(178, 40);
            label1.TabIndex = 1;
            label1.Text = "Nhập tên IG:";
            // 
            // txtTargetUserNames
            // 
            txtTargetUserNames.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            txtTargetUserNames.Location = new System.Drawing.Point(245, 74);
            txtTargetUserNames.Multiline = true;
            txtTargetUserNames.Name = "txtTargetUserNames";
            txtTargetUserNames.Size = new System.Drawing.Size(749, 240);
            txtTargetUserNames.TabIndex = 2;
            // 
            // label2
            // 
            label2.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            label2.AutoSize = true;
            label2.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            label2.ForeColor = System.Drawing.Color.DarkRed;
            label2.Location = new System.Drawing.Point(395, 339);
            label2.Name = "label2";
            label2.Size = new System.Drawing.Size(311, 32);
            label2.TabIndex = 3;
            label2.Text = "(Mỗi tên cách nhau 1 dòng)";
            // 
            // Form1
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(13F, 32F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(1100, 647);
            Controls.Add(label2);
            Controls.Add(txtTargetUserNames);
            Controls.Add(label1);
            Controls.Add(btnCrawl);
            Name = "Form1";
            Text = "Form1";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private System.Windows.Forms.Button btnCrawl;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtTargetUserNames;
        private System.Windows.Forms.Label label2;
    }
}
