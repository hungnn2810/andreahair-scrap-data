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
            components = new System.ComponentModel.Container();
            btnStart = new System.Windows.Forms.Button();
            label1 = new System.Windows.Forms.Label();
            txtTargetLinks = new System.Windows.Forms.TextBox();
            label2 = new System.Windows.Forms.Label();
            txtUsername = new System.Windows.Forms.TextBox();
            label3 = new System.Windows.Forms.Label();
            txtPassword = new System.Windows.Forms.TextBox();
            label4 = new System.Windows.Forms.Label();
            btnStop = new System.Windows.Forms.Button();
            btnExport = new System.Windows.Forms.Button();
            label5 = new System.Windows.Forms.Label();
            lblTotalFollower = new System.Windows.Forms.Label();
            label7 = new System.Windows.Forms.Label();
            lblTotalScan = new System.Windows.Forms.Label();
            timer1 = new System.Windows.Forms.Timer(components);
            label6 = new System.Windows.Forms.Label();
            lblTimer = new System.Windows.Forms.Label();
            SuspendLayout();
            // 
            // btnStart
            // 
            btnStart.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            btnStart.Location = new System.Drawing.Point(1219, 281);
            btnStart.Name = "btnStart";
            btnStart.Size = new System.Drawing.Size(296, 89);
            btnStart.TabIndex = 0;
            btnStart.Text = "Bắt đầu";
            btnStart.UseVisualStyleBackColor = true;
            btnStart.Click += btnCrawl_Click;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new System.Drawing.Font("Segoe UI", 10.875F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            label1.Location = new System.Drawing.Point(35, 191);
            label1.Name = "label1";
            label1.Size = new System.Drawing.Size(214, 40);
            label1.TabIndex = 1;
            label1.Text = "Nhập link Insta:";
            // 
            // txtTargetLinks
            // 
            txtTargetLinks.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            txtTargetLinks.Location = new System.Drawing.Point(255, 194);
            txtTargetLinks.Multiline = true;
            txtTargetLinks.Name = "txtTargetLinks";
            txtTargetLinks.Size = new System.Drawing.Size(824, 431);
            txtTargetLinks.TabIndex = 2;
            txtTargetLinks.Text = "https://www.instagram.com/jk.hair_uka/";
            // 
            // label2
            // 
            label2.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            label2.AutoSize = true;
            label2.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            label2.ForeColor = System.Drawing.Color.DarkRed;
            label2.Location = new System.Drawing.Point(484, 656);
            label2.Name = "label2";
            label2.Size = new System.Drawing.Size(314, 32);
            label2.TabIndex = 3;
            label2.Text = "(Mỗi link cách nhau 1 dòng)";
            // 
            // txtUsername
            // 
            txtUsername.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            txtUsername.Font = new System.Drawing.Font("Segoe UI", 10.875F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            txtUsername.Location = new System.Drawing.Point(255, 30);
            txtUsername.Name = "txtUsername";
            txtUsername.Size = new System.Drawing.Size(824, 46);
            txtUsername.TabIndex = 5;
            txtUsername.Text = "duong.insta";
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
            txtPassword.Size = new System.Drawing.Size(824, 46);
            txtPassword.TabIndex = 7;
            txtPassword.Text = "@Hello123";
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
            // btnStop
            // 
            btnStop.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            btnStop.Location = new System.Drawing.Point(1219, 412);
            btnStop.Name = "btnStop";
            btnStop.Size = new System.Drawing.Size(296, 89);
            btnStop.TabIndex = 8;
            btnStop.Text = "Dừng";
            btnStop.UseVisualStyleBackColor = true;
            btnStop.Click += btnStop_Click;
            // 
            // btnExport
            // 
            btnExport.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            btnExport.Location = new System.Drawing.Point(1219, 536);
            btnExport.Name = "btnExport";
            btnExport.Size = new System.Drawing.Size(296, 89);
            btnExport.TabIndex = 9;
            btnExport.Text = "Xuất file";
            btnExport.UseVisualStyleBackColor = true;
            // 
            // label5
            // 
            label5.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            label5.AutoSize = true;
            label5.Font = new System.Drawing.Font("Segoe UI", 10.875F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            label5.Location = new System.Drawing.Point(1116, 27);
            label5.Name = "label5";
            label5.Size = new System.Drawing.Size(274, 40);
            label5.TabIndex = 10;
            label5.Text = "Số lượng có thể tìm:";
            // 
            // lblTotalFollower
            // 
            lblTotalFollower.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            lblTotalFollower.AutoSize = true;
            lblTotalFollower.Font = new System.Drawing.Font("Segoe UI", 10.875F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            lblTotalFollower.ForeColor = System.Drawing.Color.OliveDrab;
            lblTotalFollower.Location = new System.Drawing.Point(1404, 30);
            lblTotalFollower.Name = "lblTotalFollower";
            lblTotalFollower.Size = new System.Drawing.Size(139, 40);
            lblTotalFollower.TabIndex = 11;
            lblTotalFollower.Text = "Đang chờ";
            // 
            // label7
            // 
            label7.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            label7.AutoSize = true;
            label7.Font = new System.Drawing.Font("Segoe UI", 10.875F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            label7.Location = new System.Drawing.Point(1219, 100);
            label7.Name = "label7";
            label7.Size = new System.Drawing.Size(171, 40);
            label7.TabIndex = 12;
            label7.Text = "Đã tìm thấy:";
            // 
            // lblTotalScan
            // 
            lblTotalScan.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            lblTotalScan.AutoSize = true;
            lblTotalScan.Font = new System.Drawing.Font("Segoe UI", 10.875F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            lblTotalScan.ForeColor = System.Drawing.SystemColors.MenuHighlight;
            lblTotalScan.Location = new System.Drawing.Point(1404, 97);
            lblTotalScan.Name = "lblTotalScan";
            lblTotalScan.Size = new System.Drawing.Size(139, 40);
            lblTotalScan.TabIndex = 13;
            lblTotalScan.Text = "Đang chờ";
            // 
            // timer1
            // 
            timer1.Tick += btnCrawl_Click;
            // 
            // label6
            // 
            label6.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            label6.AutoSize = true;
            label6.Font = new System.Drawing.Font("Segoe UI", 10.875F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            label6.Location = new System.Drawing.Point(1183, 172);
            label6.Name = "label6";
            label6.Size = new System.Drawing.Size(207, 40);
            label6.TabIndex = 14;
            label6.Text = "Thời gian chạy:";
            // 
            // lblTimer
            // 
            lblTimer.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            lblTimer.AutoSize = true;
            lblTimer.Font = new System.Drawing.Font("Segoe UI", 10.875F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            lblTimer.ForeColor = System.Drawing.Color.Blue;
            lblTimer.Location = new System.Drawing.Point(1404, 172);
            lblTimer.Name = "lblTimer";
            lblTimer.Size = new System.Drawing.Size(139, 40);
            lblTimer.TabIndex = 15;
            lblTimer.Text = "Đang chờ";
            // 
            // Main
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(13F, 32F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(1555, 748);
            Controls.Add(lblTimer);
            Controls.Add(label6);
            Controls.Add(lblTotalScan);
            Controls.Add(label7);
            Controls.Add(lblTotalFollower);
            Controls.Add(label5);
            Controls.Add(btnExport);
            Controls.Add(btnStop);
            Controls.Add(txtPassword);
            Controls.Add(label4);
            Controls.Add(txtUsername);
            Controls.Add(label3);
            Controls.Add(label2);
            Controls.Add(txtTargetLinks);
            Controls.Add(label1);
            Controls.Add(btnStart);
            Name = "Main";
            StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            Text = "Scrapping Data";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private System.Windows.Forms.Button btnStart;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtTargetLinks;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtUsername;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txtPassword;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button btnStop;
        private System.Windows.Forms.Button btnExport;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label lblTotalFollower;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label lblTotalScan;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label lblTimer;
    }
}
