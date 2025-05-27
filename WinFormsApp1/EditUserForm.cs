using System;
using System.Windows.Forms;

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
            if (string.IsNullOrWhiteSpace(txtUsername.Text))
            {
                MessageBox.Show("請輸入用戶名！", "錯誤", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            Username = txtUsername.Text;
            Password = txtPassword.Text;
            Status = cmbStatus.SelectedItem.ToString();

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