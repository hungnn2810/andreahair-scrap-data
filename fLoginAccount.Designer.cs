namespace ScrappingData
{
    partial class fLoginAccount
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
            btnCancel = new System.Windows.Forms.Button();
            btnSave = new System.Windows.Forms.Button();
            label8 = new System.Windows.Forms.Label();
            label5 = new System.Windows.Forms.Label();
            label4 = new System.Windows.Forms.Label();
            txtAccount = new System.Windows.Forms.TextBox();
            label3 = new System.Windows.Forms.Label();
            SuspendLayout();
            // 
            // btnCancel
            // 
            btnCancel.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            btnCancel.Location = new System.Drawing.Point(653, 642);
            btnCancel.Name = "btnCancel";
            btnCancel.Size = new System.Drawing.Size(296, 89);
            btnCancel.TabIndex = 43;
            btnCancel.Text = "Hủy";
            btnCancel.UseVisualStyleBackColor = true;
            btnCancel.Click += btnCancel_Click;
            // 
            // btnSave
            // 
            btnSave.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            btnSave.Location = new System.Drawing.Point(235, 642);
            btnSave.Name = "btnSave";
            btnSave.Size = new System.Drawing.Size(296, 89);
            btnSave.TabIndex = 42;
            btnSave.Text = "Lưu";
            btnSave.UseVisualStyleBackColor = true;
            btnSave.Click += btnSave_Click;
            // 
            // label8
            // 
            label8.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            label8.AutoSize = true;
            label8.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            label8.ForeColor = System.Drawing.Color.DarkRed;
            label8.Location = new System.Drawing.Point(396, 519);
            label8.Name = "label8";
            label8.Size = new System.Drawing.Size(387, 32);
            label8.TabIndex = 41;
            label8.Text = "Ví dụ: \"tên đăng nhập = mật khẩu\"";
            // 
            // label5
            // 
            label5.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            label5.AutoSize = true;
            label5.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            label5.ForeColor = System.Drawing.Color.DarkRed;
            label5.Location = new System.Drawing.Point(330, 464);
            label5.Name = "label5";
            label5.Size = new System.Drawing.Size(536, 32);
            label5.TabIndex = 40;
            label5.Text = "Tên đăng nhập và mật khẩu cách nhau 1 dấu \"=\"";
            // 
            // label4
            // 
            label4.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            label4.AutoSize = true;
            label4.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            label4.ForeColor = System.Drawing.Color.DarkRed;
            label4.Location = new System.Drawing.Point(396, 571);
            label4.Name = "label4";
            label4.Size = new System.Drawing.Size(375, 32);
            label4.TabIndex = 39;
            label4.Text = "(Mỗi tài khoản cách nhau 1 dòng)";
            // 
            // txtAccount
            // 
            txtAccount.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            txtAccount.Location = new System.Drawing.Point(235, 32);
            txtAccount.Multiline = true;
            txtAccount.Name = "txtAccount";
            txtAccount.Size = new System.Drawing.Size(829, 400);
            txtAccount.TabIndex = 38;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Font = new System.Drawing.Font("Segoe UI", 10.875F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            label3.Location = new System.Drawing.Point(78, 26);
            label3.Name = "label3";
            label3.Size = new System.Drawing.Size(151, 40);
            label3.TabIndex = 37;
            label3.Text = "Tài khoản :";
            // 
            // fLoginAccount
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(13F, 32F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(1143, 757);
            Controls.Add(btnCancel);
            Controls.Add(btnSave);
            Controls.Add(label8);
            Controls.Add(label5);
            Controls.Add(label4);
            Controls.Add(txtAccount);
            Controls.Add(label3);
            Name = "fLoginAccount";
            StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            Text = "fLoginAccount";
            Activated += fLoginAccount_Activated;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox txtAccount;
        private System.Windows.Forms.Label label3;
    }
}