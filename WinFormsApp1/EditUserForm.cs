using System;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace WinFormsApp1
{
    public partial class EditUserForm : Form
    {
        public string Username { get; private set; }
        public string Password { get; private set; }
        public string Status { get; private set; }

        private int userId;

        public EditUserForm(int userId, string username, string status)
        {
            InitializeComponent();
            this.userId = userId;
            this.Username = username;
            this.Status = status;

            // 設置表單標題
            this.Text = "編輯用戶";

            // 初始化控件
            txtUsername.Text = username;
            cmbStatus.Items.AddRange(new string[] { "正常", "凍結" });
            cmbStatus.SelectedItem = status == "正常" ? "正常" : "凍結";

            // 設置字體
            this.Font = new System.Drawing.Font("Microsoft JhengHei UI", 10.5F);
            lblUsername.Font = new System.Drawing.Font("Microsoft JhengHei UI", 10.5F);
            lblPassword.Font = new System.Drawing.Font("Microsoft JhengHei UI", 10.5F);
            lblStatus.Font = new System.Drawing.Font("Microsoft JhengHei UI", 10.5F);
            txtUsername.Font = new System.Drawing.Font("Microsoft JhengHei UI", 10.5F);
            txtPassword.Font = new System.Drawing.Font("Microsoft JhengHei UI", 10.5F);
            cmbStatus.Font = new System.Drawing.Font("Microsoft JhengHei UI", 10.5F);
            btnSave.Font = new System.Drawing.Font("Microsoft JhengHei UI", 10.5F);
            btnCancel.Font = new System.Drawing.Font("Microsoft JhengHei UI", 10.5F);
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            string newUsername = txtUsername.Text.Trim();
            string newPassword = txtPassword.Text.Trim();
            string status = cmbStatus.SelectedItem.ToString();
            if (string.IsNullOrWhiteSpace(newUsername))
            {
                MessageBox.Show("請輸入用戶名！", "錯誤", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            // 用戶名長度與格式
            if (newUsername.Length < 4 || !System.Text.RegularExpressions.Regex.IsMatch(newUsername, "^[A-Za-z0-9]+$"))
            {
                MessageBox.Show("用戶名必須為4位以上英數字！", "錯誤", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            // 檢查用戶名唯一（排除自己）
            string checkSql = "SELECT COUNT(*) FROM user WHERE username = @username AND user_id <> @uid";
            var checkParams = new[] {
                new MySqlParameter("@username", newUsername),
                new MySqlParameter("@uid", userId)
            };
            long userCount = System.Convert.ToInt64(DBHelper.ExecuteScalar(checkSql, checkParams));
            if (userCount > 0)
            {
                MessageBox.Show("用戶名已存在，請更換！", "錯誤", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            // 密碼可空，但若有填寫需檢查格式
            if (!string.IsNullOrEmpty(newPassword))
            {
                if (newPassword.Length < 4 || !System.Text.RegularExpressions.Regex.IsMatch(newPassword, "^[A-Za-z0-9]+$"))
                {
                    MessageBox.Show("密碼必須為4位以上英數字！", "錯誤", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }
            Username = newUsername;
            Password = newPassword;
            Status = status;
            DialogResult = DialogResult.OK;
            Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }
    }
} 