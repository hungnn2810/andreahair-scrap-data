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
            txtUsername = new System.Windows.Forms.TextBox();
            label3 = new System.Windows.Forms.Label();
            txtPassword = new System.Windows.Forms.TextBox();
            label4 = new System.Windows.Forms.Label();
            SuspendLayout();
            // 
            // btnCrawl
            // 
            btnCrawl.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            btnCrawl.Location = new System.Drawing.Point(402, 535);
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
            label1.Location = new System.Drawing.Point(71, 194);
            label1.Name = "label1";
            label1.Size = new System.Drawing.Size(178, 40);
            label1.TabIndex = 1;
            label1.Text = "Nhập tên IG:";
            // 
            // txtTargetUserNames
            // 
            txtTargetUserNames.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            txtTargetUserNames.Location = new System.Drawing.Point(255, 194);
            txtTargetUserNames.Multiline = true;
            txtTargetUserNames.Name = "txtTargetUserNames";
            txtTargetUserNames.Size = new System.Drawing.Size(749, 258);
            txtTargetUserNames.TabIndex = 2;
            // 
            // label2
            // 
            label2.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            label2.AutoSize = true;
            label2.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            label2.ForeColor = System.Drawing.Color.DarkRed;
            label2.Location = new System.Drawing.Point(387, 477);
            label2.Name = "label2";
            label2.Size = new System.Drawing.Size(311, 32);
            label2.TabIndex = 3;
            label2.Text = "(Mỗi tên cách nhau 1 dòng)";
            // 
            // txtUsername
            // 
            txtUsername.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            txtUsername.Font = new System.Drawing.Font("Segoe UI", 10.875F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            txtUsername.Location = new System.Drawing.Point(255, 30);
            txtUsername.Name = "txtUsername";
            txtUsername.Size = new System.Drawing.Size(749, 46);
            txtUsername.TabIndex = 5;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Font = new System.Drawing.Font("Segoe UI", 10.875F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            label3.Location = new System.Drawing.Point(106, 30);
            label3.Name = "label3";
            label3.Size = new System.Drawing.Size(143, 40);
            label3.TabIndex = 4;
            label3.Text = "Tài khoản:";
            // 
            // txtPassword
            // 
            txtPassword.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            txtPassword.Font = new System.Drawing.Font("Segoe UI", 10.875F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            txtPassword.Location = new System.Drawing.Point(255, 109);
            txtPassword.Name = "txtPassword";
            txtPassword.Size = new System.Drawing.Size(749, 46);
            txtPassword.TabIndex = 7;
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Font = new System.Drawing.Font("Segoe UI", 10.875F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            label4.Location = new System.Drawing.Point(106, 112);
            label4.Name = "label4";
            label4.Size = new System.Drawing.Size(143, 40);
            label4.TabIndex = 6;
            label4.Text = "Mật khẩu:";
            // 
            // Main
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(13F, 32F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(1100, 647);
            Controls.Add(txtPassword);
            Controls.Add(label4);
            Controls.Add(txtUsername);
            Controls.Add(label3);
            Controls.Add(label2);
            Controls.Add(txtTargetUserNames);
            Controls.Add(label1);
            Controls.Add(btnCrawl);
            Name = "Main";
            Text = "Form1";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private System.Windows.Forms.Button btnCrawl;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtTargetUserNames;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtUsername;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txtPassword;
        private System.Windows.Forms.Label label4;
    }
}
