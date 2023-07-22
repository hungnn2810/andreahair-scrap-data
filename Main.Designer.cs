namespace Instagram
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Main));
            btnStart = new System.Windows.Forms.Button();
            label1 = new System.Windows.Forms.Label();
            txtTargetLinks = new System.Windows.Forms.TextBox();
            label2 = new System.Windows.Forms.Label();
            label3 = new System.Windows.Forms.Label();
            btnExport = new System.Windows.Forms.Button();
            timer1 = new System.Windows.Forms.Timer(components);
            label6 = new System.Windows.Forms.Label();
            lblTimer = new System.Windows.Forms.Label();
            backgroundWorker1 = new System.ComponentModel.BackgroundWorker();
            lblHasScan = new System.Windows.Forms.Label();
            label10 = new System.Windows.Forms.Label();
            btnAccount = new System.Windows.Forms.Button();
            dtgvAccount = new System.Windows.Forms.DataGridView();
            cUserName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            cPassword = new System.Windows.Forms.DataGridViewTextBoxColumn();
            lblThread = new System.Windows.Forms.Label();
            label5 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)dtgvAccount).BeginInit();
            SuspendLayout();
            // 
            // btnStart
            // 
            btnStart.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            btnStart.BackColor = System.Drawing.Color.White;
            btnStart.Font = new System.Drawing.Font("Times New Roman", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            btnStart.Location = new System.Drawing.Point(294, 309);
            btnStart.Margin = new System.Windows.Forms.Padding(2, 1, 2, 1);
            btnStart.Name = "btnStart";
            btnStart.Size = new System.Drawing.Size(182, 48);
            btnStart.TabIndex = 0;
            btnStart.Text = "Bắt đầu";
            btnStart.UseVisualStyleBackColor = false;
            btnStart.Click += btnCrawl_Click;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new System.Drawing.Font("Times New Roman", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            label1.Location = new System.Drawing.Point(499, 25);
            label1.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            label1.Name = "label1";
            label1.Size = new System.Drawing.Size(140, 22);
            label1.TabIndex = 1;
            label1.Text = "Link instagram:";
            // 
            // txtTargetLinks
            // 
            txtTargetLinks.Anchor = System.Windows.Forms.AnchorStyles.None;
            txtTargetLinks.Location = new System.Drawing.Point(499, 60);
            txtTargetLinks.Margin = new System.Windows.Forms.Padding(2, 1, 2, 1);
            txtTargetLinks.Multiline = true;
            txtTargetLinks.Name = "txtTargetLinks";
            txtTargetLinks.Size = new System.Drawing.Size(451, 219);
            txtTargetLinks.TabIndex = 2;
            // 
            // label2
            // 
            label2.Anchor = System.Windows.Forms.AnchorStyles.None;
            label2.AutoSize = true;
            label2.Font = new System.Drawing.Font("Times New Roman", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            label2.ForeColor = System.Drawing.Color.DarkRed;
            label2.Location = new System.Drawing.Point(730, 25);
            label2.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            label2.Name = "label2";
            label2.Size = new System.Drawing.Size(220, 21);
            label2.TabIndex = 3;
            label2.Text = "(Mỗi link cách nhau 1 dòng)";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Font = new System.Drawing.Font("Times New Roman", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            label3.Location = new System.Drawing.Point(34, 25);
            label3.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            label3.Name = "label3";
            label3.Size = new System.Drawing.Size(104, 22);
            label3.TabIndex = 4;
            label3.Text = "Tài khoản :";
            // 
            // btnExport
            // 
            btnExport.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            btnExport.BackColor = System.Drawing.Color.White;
            btnExport.Font = new System.Drawing.Font("Times New Roman", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            btnExport.Location = new System.Drawing.Point(34, 309);
            btnExport.Margin = new System.Windows.Forms.Padding(2, 1, 2, 1);
            btnExport.Name = "btnExport";
            btnExport.Size = new System.Drawing.Size(182, 48);
            btnExport.TabIndex = 9;
            btnExport.Text = "Xuất file excel";
            btnExport.UseVisualStyleBackColor = false;
            btnExport.Click += btnExport_Click;
            // 
            // timer1
            // 
            timer1.Tick += timer1_Tick;
            // 
            // label6
            // 
            label6.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            label6.AutoSize = true;
            label6.Font = new System.Drawing.Font("Times New Roman", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            label6.Location = new System.Drawing.Point(565, 339);
            label6.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            label6.Name = "label6";
            label6.Size = new System.Drawing.Size(137, 22);
            label6.TabIndex = 14;
            label6.Text = "Thời gian chạy:";
            // 
            // lblTimer
            // 
            lblTimer.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            lblTimer.AutoSize = true;
            lblTimer.Font = new System.Drawing.Font("Times New Roman", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            lblTimer.ForeColor = System.Drawing.Color.Blue;
            lblTimer.Location = new System.Drawing.Point(730, 338);
            lblTimer.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            lblTimer.Name = "lblTimer";
            lblTimer.Size = new System.Drawing.Size(83, 21);
            lblTimer.TabIndex = 15;
            lblTimer.Text = "Đang chờ";
            // 
            // lblHasScan
            // 
            lblHasScan.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            lblHasScan.AutoSize = true;
            lblHasScan.Font = new System.Drawing.Font("Times New Roman", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            lblHasScan.ForeColor = System.Drawing.Color.Olive;
            lblHasScan.Location = new System.Drawing.Point(730, 309);
            lblHasScan.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            lblHasScan.Name = "lblHasScan";
            lblHasScan.Size = new System.Drawing.Size(33, 21);
            lblHasScan.TabIndex = 31;
            lblHasScan.Text = "0/0";
            // 
            // label10
            // 
            label10.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            label10.AutoSize = true;
            label10.Font = new System.Drawing.Font("Times New Roman", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            label10.Location = new System.Drawing.Point(565, 309);
            label10.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            label10.Name = "label10";
            label10.Size = new System.Drawing.Size(79, 22);
            label10.TabIndex = 30;
            label10.Text = "Đã quét:";
            // 
            // btnAccount
            // 
            btnAccount.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            btnAccount.BackColor = System.Drawing.Color.White;
            btnAccount.Font = new System.Drawing.Font("Times New Roman", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            btnAccount.Location = new System.Drawing.Point(356, 18);
            btnAccount.Margin = new System.Windows.Forms.Padding(2, 1, 2, 1);
            btnAccount.Name = "btnAccount";
            btnAccount.Size = new System.Drawing.Size(120, 37);
            btnAccount.TabIndex = 32;
            btnAccount.Text = "Thêm";
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
            dtgvAccount.Location = new System.Drawing.Point(34, 60);
            dtgvAccount.Margin = new System.Windows.Forms.Padding(2, 1, 2, 1);
            dtgvAccount.Name = "dtgvAccount";
            dtgvAccount.ReadOnly = true;
            dtgvAccount.RowHeadersVisible = false;
            dtgvAccount.RowHeadersWidth = 82;
            dtgvAccount.RowTemplate.Height = 41;
            dtgvAccount.Size = new System.Drawing.Size(442, 219);
            dtgvAccount.TabIndex = 33;
            // 
            // cUserName
            // 
            cUserName.HeaderText = "Tài khoản";
            cUserName.MinimumWidth = 10;
            cUserName.Name = "cUserName";
            cUserName.ReadOnly = true;
            cUserName.Width = 200;
            // 
            // cPassword
            // 
            cPassword.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            cPassword.HeaderText = "Mật khẩu";
            cPassword.MinimumWidth = 10;
            cPassword.Name = "cPassword";
            cPassword.ReadOnly = true;
            // 
            // lblThread
            // 
            lblThread.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            lblThread.AutoSize = true;
            lblThread.Font = new System.Drawing.Font("Times New Roman", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            lblThread.ForeColor = System.Drawing.Color.DarkViolet;
            lblThread.Location = new System.Drawing.Point(730, 370);
            lblThread.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            lblThread.Name = "lblThread";
            lblThread.Size = new System.Drawing.Size(83, 21);
            lblThread.TabIndex = 35;
            lblThread.Text = "Đang chờ";
            // 
            // label5
            // 
            label5.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            label5.AutoSize = true;
            label5.Font = new System.Drawing.Font("Times New Roman", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            label5.Location = new System.Drawing.Point(565, 369);
            label5.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            label5.Name = "label5";
            label5.Size = new System.Drawing.Size(155, 22);
            label5.TabIndex = 34;
            label5.Text = "Luồng đang chạy:";
            // 
            // Main
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(8F, 17F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(987, 430);
            Controls.Add(lblThread);
            Controls.Add(label5);
            Controls.Add(dtgvAccount);
            Controls.Add(btnAccount);
            Controls.Add(lblHasScan);
            Controls.Add(label10);
            Controls.Add(lblTimer);
            Controls.Add(label6);
            Controls.Add(btnExport);
            Controls.Add(label3);
            Controls.Add(label2);
            Controls.Add(txtTargetLinks);
            Controls.Add(label1);
            Controls.Add(btnStart);
            Font = new System.Drawing.Font("Times New Roman", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            Icon = (System.Drawing.Icon)resources.GetObject("$this.Icon");
            Margin = new System.Windows.Forms.Padding(2, 1, 2, 1);
            MaximizeBox = false;
            Name = "Main";
            StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            Text = "Instagram";
            Activated += Main_Activated;
            FormClosed += Main_FormClosed;
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
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label lblTimer;
        private System.ComponentModel.BackgroundWorker backgroundWorker1;
        private System.Windows.Forms.Label lblHasScan;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Button btnAccount;
        private System.Windows.Forms.DataGridView dtgvAccount;
        private System.Windows.Forms.Label lblThread;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.DataGridViewTextBoxColumn cUserName;
        private System.Windows.Forms.DataGridViewTextBoxColumn cPassword;
    }
}
