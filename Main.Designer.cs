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
            label3 = new System.Windows.Forms.Label();
            btnExport = new System.Windows.Forms.Button();
            label7 = new System.Windows.Forms.Label();
            lblTotalScan = new System.Windows.Forms.Label();
            timer1 = new System.Windows.Forms.Timer(components);
            label6 = new System.Windows.Forms.Label();
            lblTimer = new System.Windows.Forms.Label();
            backgroundWorker1 = new System.ComponentModel.BackgroundWorker();
            lblSDT = new System.Windows.Forms.Label();
            label13 = new System.Windows.Forms.Label();
            lblHasScan = new System.Windows.Forms.Label();
            label10 = new System.Windows.Forms.Label();
            btnAccount = new System.Windows.Forms.Button();
            dtgvAccount = new System.Windows.Forms.DataGridView();
            cUserName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            cPassword = new System.Windows.Forms.DataGridViewTextBoxColumn();
            ((System.ComponentModel.ISupportInitialize)dtgvAccount).BeginInit();
            SuspendLayout();
            // 
            // btnStart
            // 
            btnStart.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            btnStart.BackColor = System.Drawing.Color.MediumSeaGreen;
            btnStart.Location = new System.Drawing.Point(1184, 505);
            btnStart.Name = "btnStart";
            btnStart.Size = new System.Drawing.Size(296, 89);
            btnStart.TabIndex = 0;
            btnStart.Text = "Bắt đầu";
            btnStart.UseVisualStyleBackColor = false;
            btnStart.Click += btnCrawl_Click;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new System.Drawing.Font("Segoe UI", 10.875F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            label1.Location = new System.Drawing.Point(35, 544);
            label1.Name = "label1";
            label1.Size = new System.Drawing.Size(214, 40);
            label1.TabIndex = 1;
            label1.Text = "Nhập link Insta:";
            // 
            // txtTargetLinks
            // 
            txtTargetLinks.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            txtTargetLinks.Location = new System.Drawing.Point(255, 547);
            txtTargetLinks.Multiline = true;
            txtTargetLinks.Name = "txtTargetLinks";
            txtTargetLinks.Size = new System.Drawing.Size(824, 363);
            txtTargetLinks.TabIndex = 2;
            txtTargetLinks.Text = "https://www.instagram.com/jk.hair_uka/\r\nhttps://www.instagram.com/humanhairbeauty_ind/\r\nhttps://www.instagram.com/thanhanhair_russia/";
            // 
            // label2
            // 
            label2.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            label2.AutoSize = true;
            label2.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            label2.ForeColor = System.Drawing.Color.DarkRed;
            label2.Location = new System.Drawing.Point(480, 913);
            label2.Name = "label2";
            label2.Size = new System.Drawing.Size(314, 32);
            label2.TabIndex = 3;
            label2.Text = "(Mỗi link cách nhau 1 dòng)";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Font = new System.Drawing.Font("Segoe UI", 10.875F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            label3.Location = new System.Drawing.Point(98, 33);
            label3.Name = "label3";
            label3.Size = new System.Drawing.Size(151, 40);
            label3.TabIndex = 4;
            label3.Text = "Tài khoản :";
            // 
            // btnExport
            // 
            btnExport.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            btnExport.BackColor = System.Drawing.Color.DodgerBlue;
            btnExport.Location = new System.Drawing.Point(1184, 631);
            btnExport.Name = "btnExport";
            btnExport.Size = new System.Drawing.Size(296, 89);
            btnExport.TabIndex = 9;
            btnExport.Text = "Xuất file";
            btnExport.UseVisualStyleBackColor = false;
            btnExport.Click += btnExport_Click;
            // 
            // label7
            // 
            label7.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            label7.AutoSize = true;
            label7.Font = new System.Drawing.Font("Segoe UI", 10.875F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            label7.Location = new System.Drawing.Point(1159, 36);
            label7.Name = "label7";
            label7.Size = new System.Drawing.Size(187, 40);
            label7.TabIndex = 12;
            label7.Text = "Link tìm thấy:";
            // 
            // lblTotalScan
            // 
            lblTotalScan.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            lblTotalScan.AutoSize = true;
            lblTotalScan.Font = new System.Drawing.Font("Segoe UI", 10.875F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            lblTotalScan.ForeColor = System.Drawing.SystemColors.MenuHighlight;
            lblTotalScan.Location = new System.Drawing.Point(1360, 36);
            lblTotalScan.Name = "lblTotalScan";
            lblTotalScan.Size = new System.Drawing.Size(139, 40);
            lblTotalScan.TabIndex = 13;
            lblTotalScan.Text = "Đang chờ";
            // 
            // timer1
            // 
            timer1.Tick += timer1_Tick;
            // 
            // label6
            // 
            label6.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            label6.AutoSize = true;
            label6.Font = new System.Drawing.Font("Segoe UI", 10.875F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            label6.Location = new System.Drawing.Point(1139, 219);
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
            lblTimer.Location = new System.Drawing.Point(1360, 219);
            lblTimer.Name = "lblTimer";
            lblTimer.Size = new System.Drawing.Size(139, 40);
            lblTimer.TabIndex = 15;
            lblTimer.Text = "Đang chờ";
            // 
            // lblSDT
            // 
            lblSDT.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            lblSDT.AutoSize = true;
            lblSDT.Font = new System.Drawing.Font("Segoe UI", 10.875F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            lblSDT.ForeColor = System.Drawing.Color.Tomato;
            lblSDT.Location = new System.Drawing.Point(1360, 155);
            lblSDT.Name = "lblSDT";
            lblSDT.Size = new System.Drawing.Size(139, 40);
            lblSDT.TabIndex = 25;
            lblSDT.Text = "Đang chờ";
            // 
            // label13
            // 
            label13.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            label13.AutoSize = true;
            label13.Font = new System.Drawing.Font("Segoe UI", 10.875F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            label13.Location = new System.Drawing.Point(1159, 155);
            label13.Name = "label13";
            label13.Size = new System.Drawing.Size(186, 40);
            label13.TabIndex = 24;
            label13.Text = "SĐT tìm thấy:";
            // 
            // lblHasScan
            // 
            lblHasScan.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            lblHasScan.AutoSize = true;
            lblHasScan.Font = new System.Drawing.Font("Segoe UI", 10.875F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            lblHasScan.ForeColor = System.Drawing.Color.Olive;
            lblHasScan.Location = new System.Drawing.Point(1360, 97);
            lblHasScan.Name = "lblHasScan";
            lblHasScan.Size = new System.Drawing.Size(139, 40);
            lblHasScan.TabIndex = 31;
            lblHasScan.Text = "Đang chờ";
            // 
            // label10
            // 
            label10.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            label10.AutoSize = true;
            label10.Font = new System.Drawing.Font("Segoe UI", 10.875F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            label10.Location = new System.Drawing.Point(1165, 97);
            label10.Name = "label10";
            label10.Size = new System.Drawing.Size(180, 40);
            label10.TabIndex = 30;
            label10.Text = "Link đã quét:";
            // 
            // btnAccount
            // 
            btnAccount.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            btnAccount.BackColor = System.Drawing.Color.BlanchedAlmond;
            btnAccount.Location = new System.Drawing.Point(1184, 367);
            btnAccount.Name = "btnAccount";
            btnAccount.Size = new System.Drawing.Size(296, 89);
            btnAccount.TabIndex = 32;
            btnAccount.Text = "Thêm tài khoản";
            btnAccount.UseVisualStyleBackColor = false;
            btnAccount.Click += btnAccount_Click;
            // 
            // dtgvAccount
            // 
            dtgvAccount.AllowUserToAddRows = false;
            dtgvAccount.AllowUserToDeleteRows = false;
            dtgvAccount.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            dtgvAccount.BackgroundColor = System.Drawing.Color.White;
            dtgvAccount.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dtgvAccount.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] { cUserName, cPassword });
            dtgvAccount.Location = new System.Drawing.Point(255, 33);
            dtgvAccount.Name = "dtgvAccount";
            dtgvAccount.ReadOnly = true;
            dtgvAccount.RowHeadersVisible = false;
            dtgvAccount.RowHeadersWidth = 82;
            dtgvAccount.RowTemplate.Height = 41;
            dtgvAccount.Size = new System.Drawing.Size(824, 484);
            dtgvAccount.TabIndex = 33;
            // 
            // cUserName
            // 
            cUserName.HeaderText = "Tài khoản";
            cUserName.MinimumWidth = 10;
            cUserName.Name = "cUserName";
            cUserName.ReadOnly = true;
            cUserName.Width = 400;
            // 
            // cPassword
            // 
            cPassword.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            cPassword.HeaderText = "Mật khẩu";
            cPassword.MinimumWidth = 10;
            cPassword.Name = "cPassword";
            cPassword.ReadOnly = true;
            // 
            // Main
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(13F, 32F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(1555, 968);
            Controls.Add(dtgvAccount);
            Controls.Add(btnAccount);
            Controls.Add(lblHasScan);
            Controls.Add(label10);
            Controls.Add(lblSDT);
            Controls.Add(label13);
            Controls.Add(lblTimer);
            Controls.Add(label6);
            Controls.Add(lblTotalScan);
            Controls.Add(label7);
            Controls.Add(btnExport);
            Controls.Add(label3);
            Controls.Add(label2);
            Controls.Add(txtTargetLinks);
            Controls.Add(label1);
            Controls.Add(btnStart);
            Name = "Main";
            StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            Text = "Scrapping Data";
            Activated += Main_Activated;
            ((System.ComponentModel.ISupportInitialize)dtgvAccount).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private System.Windows.Forms.Button btnStart;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtTargetLinks;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button btnExport;
        private System.Windows.Forms.Label lblTotalFollower;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label lblTotalScan;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label lblTimer;
        private System.ComponentModel.BackgroundWorker backgroundWorker1;
        private System.Windows.Forms.Label lblSDT;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.Label lblHasScan;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Button btnAccount;
        private System.Windows.Forms.DataGridView dtgvAccount;
        private System.Windows.Forms.DataGridViewTextBoxColumn cUserName;
        private System.Windows.Forms.DataGridViewTextBoxColumn cPassword;
    }
}
