using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Instagram
{
    public partial class fLoginAccount : Form
    {
        public fLoginAccount()
        {
            InitializeComponent();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            var folderPath = @"C:\Andreahair";
            var isLoginAccountExisted = File.Exists($@"{folderPath}\loginAccount.txt");
            // Check if the folder already exists
            if (!Directory.Exists(folderPath))
            { // Create the folder
                Directory.CreateDirectory(folderPath);
            }
            if (!isLoginAccountExisted)
            {
                File.Create($@"{folderPath}\loginAccount.txt").Close();
            }

            var acc = txtAccount.Text;
            File.WriteAllText($@"{folderPath}\loginAccount.txt", acc);
            MessageBox.Show("Lưu thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);

            this.Close();
        }

        private void fLoginAccount_Activated(object sender, EventArgs e)
        {
            var folderPath = @"C:\Andreahair";
            var isLoginAccountExisted = File.Exists($@"{folderPath}\loginAccount.txt");
            if (isLoginAccountExisted)
            {
                txtAccount.Text = File.ReadAllText($@"{folderPath}\loginAccount.txt");
            }
        }
    }
}
