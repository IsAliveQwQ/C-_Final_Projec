using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace WinFormsApp1
{
    public partial class AddUserForm : Form
    {
        public AddUserForm()
        {
            InitializeComponent();
        }

        public string Username { get; private set; }
        public string Password { get; private set; }

        private void btnSave_Click(object sender, System.EventArgs e)
        {
            Username = txtUsername.Text.Trim();
            Password = txtPassword.Text.Trim();
            if (string.IsNullOrEmpty(Username) || string.IsNullOrEmpty(Password))
            {
                MessageBox.Show("請輸入帳號與密碼！", "錯誤", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // 用戶名長度與格式
            if (Username.Length < 4 || !System.Text.RegularExpressions.Regex.IsMatch(Username, "^[A-Za-z0-9]+$"))
            {
                MessageBox.Show("用戶名必須為4位以上英數字！", "錯誤", MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.DialogResult = DialogResult.None;
                return;
            }
            // 密碼長度與格式
            if (Password.Length < 4 || !System.Text.RegularExpressions.Regex.IsMatch(Password, "^[A-Za-z0-9]+$"))
            {
                MessageBox.Show("密碼必須為4位以上英數字！", "錯誤", MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.DialogResult = DialogResult.None;
                return;
            }
            // 檢查用戶名唯一
            string checkSql = "SELECT COUNT(*) FROM user WHERE username = @username";
            var checkParam = new MySqlParameter("@username", Username);
            long userCount = System.Convert.ToInt64(DBHelper.ExecuteScalar(checkSql, new[] { checkParam }));
            if (userCount > 0)
            {
                MessageBox.Show("用戶名已存在，請更換！", "錯誤", MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.DialogResult = DialogResult.None;
                return;
            }
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void btnCancel_Click(object sender, System.EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
    }
} 