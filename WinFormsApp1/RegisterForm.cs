using System;
using System.Windows.Forms;

namespace WinFormsApp1
{
    public partial class RegisterForm : Form
    {
        public string Username { get; private set; }
        public string Password { get; private set; }

        public RegisterForm()
        {
            InitializeComponent();
        }

        private void btnRegister_Click(object sender, EventArgs e)
        {
            Username = txtUsername.Text;
            Password = txtPassword.Text;

            if (string.IsNullOrWhiteSpace(Username) || string.IsNullOrWhiteSpace(Password))
            {
                MessageBox.Show("請輸入帳號與密碼！", "錯誤", MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.DialogResult = DialogResult.None; // Stay on the form
                return; // Add return to stop further processing
            }

            // Add username minimum length validation
            if (Username.Length < 4)
            {
                MessageBox.Show("使用者名稱長度必須大於等於4個字元！", "錯誤", MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.DialogResult = DialogResult.None; // Stay on the form
                return; // Stop further processing
            }

            // Add password length validation
            if (Password.Length < 4 || Password.Length > 12)
            {
                MessageBox.Show("密碼長度必須在4到12個字元之間！", "錯誤", MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.DialogResult = DialogResult.None; // Stay on the form
                return; // Stop further processing
            }

            // If validation passes
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
    }
} 