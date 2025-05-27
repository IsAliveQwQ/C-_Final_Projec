using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using MySql.Data.MySqlClient; // 需要引用 MySql.Data 命名空間
// 如果 Models 目錄下的 User 類別需要用到，也可能需要引用其命名空間
// using WinFormsApp1.Models;

namespace WinFormsApp1
{
    public partial class Form1 : Form
    {
        private int loggedInUserId = 0; // 新增欄位儲存登入的 user_id
        private string loggedInUserRole = ""; // 新增欄位儲存登入的 user_id

        // 提供公開屬性以便外部獲取 user_id
        public int LoggedInUserId
        {
            get { return loggedInUserId; }
        }

        // 提供公開屬性以便外部獲取 user_role
        public string LoggedInUserRole
        {
            get { return loggedInUserRole; }
        }

        public Form1()
        {
            InitializeComponent();
            this.Text = "漫畫租書及預約系統 - 登入";
            this.btnLogin.Click += new System.EventHandler(this.btnLogin_Click);
            this.btnRegister.Click += new System.EventHandler(this.btnRegister_Click); // 註冊按鈕事件
            this.Load += new System.EventHandler(this.Form1_Load);
        }

        private void Form1_Load(object sender, System.EventArgs e)
        {
            CenterPanel(); // 取消註釋，確保面板置中
        }

        private void CenterPanel()
        {
            if (panelLogin != null)
            {
                // 計算面板在視窗中的置中位置
                int x = (this.ClientSize.Width - panelLogin.Width) / 2;
                int y = (this.ClientSize.Height - panelLogin.Height) / 2;
                panelLogin.Location = new Point(x, y);
                
                // 確保面板可見
                panelLogin.Visible = true;
                panelLogin.BringToFront();
            }
        }

        private void btnLogin_Click(object sender, System.EventArgs e)
        {
            string username = txtUsername.Text.Trim();
            string password = txtPassword.Text.Trim(); // 注意：這裡獲取的是明文密碼，實際應用中應獲取並哈希

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                MessageBox.Show("請輸入使用者名稱和密碼！", "錯誤", MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.DialogResult = DialogResult.None; // 保持對話框開啟
                return;
            }

            try
            {
                // 查詢使用者 (從 user 表)，同時獲取 status 欄位
                // 注意：這裡直接比對明文密碼，實際應用應對 password 進行哈希後再比對 password_hash
                string sql = "SELECT user_id, role, status FROM user WHERE username = @username AND password_hash = @password";
                MySqlParameter[] parameters = { // 需要使用 MySqlParameter
                    new MySqlParameter("@username", username),
                    new MySqlParameter("@password", password) // 這裡應是哈希後的密碼
                };

                DataTable dt = DBHelper.ExecuteQuery(sql, parameters);

                if (dt.Rows.Count > 0)
                {
                    // 登入成功，獲取 user_id, role 和 status
                    loggedInUserId = Convert.ToInt32(dt.Rows[0]["user_id"]); // 獲取 user_id
                    loggedInUserRole = dt.Rows[0]["role"].ToString(); // 獲取角色
                    string userStatus = dt.Rows[0]["status"].ToString(); // 獲取狀態

                    // 檢查用戶狀態
                    if (userStatus == "frozen")
                    {
                        MessageBox.Show("您的帳號因有逾期未歸還書籍而被凍結，無法登入，請聯絡管理員解凍。", "帳號凍結", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        this.DialogResult = DialogResult.None; // 保持對話框開啟
                        // Reset logged-in user info as login failed
                        loggedInUserId = 0;
                        loggedInUserRole = "";
                        return; // Stop the login process
                    }

                    // 如果用戶狀態不是 frozen，則根據角色設定 DialogResult 正常登入
                    if (loggedInUserRole == "admin")
                    {
                        this.DialogResult = DialogResult.OK; // 管理員登入成功
                    }
                    else if (loggedInUserRole == "user")
                    {
                        this.DialogResult = DialogResult.OK; // 普通用戶登入成功
                    }
                    else
                    {
                         // 未知角色，視為登入失敗
                        MessageBox.Show("未知的使用者角色！", "錯誤", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        this.DialogResult = DialogResult.None; // 保持對話框開啟
                        // Reset logged-in user info as login failed
                        loggedInUserId = 0;
                        loggedInUserRole = "";
                    }

                    // 對話框會在 DialogResult 設定為 OK 時自動關閉

                }
                else
                {
                    // 登入失敗
                    MessageBox.Show("使用者名稱或密碼錯誤！", "錯誤", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    this.DialogResult = DialogResult.None; // 保持對話框開啟
                }
            }
            catch (Exception ex)
            {
                // 捕獲資料庫連接或查詢時的錯誤
                MessageBox.Show("登入時發生錯誤：" + ex.Message, "錯誤", MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.DialogResult = DialogResult.None; // 保持對話框開啟
            }
        }

        // 註冊按鈕點擊事件處理方法
        private void btnRegister_Click(object sender, EventArgs e)
        {
            // 這裡可以彈出註冊表單，暫時僅顯示訊息
            // MessageBox.Show("註冊功能尚未實作。", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);

            using (RegisterForm registerForm = new RegisterForm())
            {
                if (registerForm.ShowDialog() == DialogResult.OK)
                {
                    string username = registerForm.Username;
                    string password = registerForm.Password; // 注意：這裡獲取的是明文密碼，實際應用中應獲取並哈希

                    try
                    {
                        // 檢查帳號是否已存在
                        string checkSql = "SELECT COUNT(*) FROM user WHERE username = @username";
                        MySqlParameter[] checkParams = { new MySqlParameter("@username", username) };
                        long userCount = Convert.ToInt64(DBHelper.ExecuteScalar(checkSql, checkParams));

                        if (userCount > 0)
                        {
                            MessageBox.Show("此帳號已被註冊，請使用其他帳號。", "註冊失敗", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        }
                        else
                        {
                            // 將新使用者插入資料庫，預設角色為 'user'，狀態為 'active'
                            string insertSql = "INSERT INTO user (username, password_hash, role, status) VALUES (@username, @password, 'user', 'active')";
                            MySqlParameter[] insertParams = {
                                new MySqlParameter("@username", username),
                                new MySqlParameter("@password", password) // 這裡應是哈希後的密碼
                            };

                            int rowsAffected = DBHelper.ExecuteNonQuery(insertSql, insertParams);

                            if (rowsAffected > 0)
                            {
                                MessageBox.Show("註冊成功！", "成功", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            }
                            else
                            {
                                MessageBox.Show("註冊失敗，請稍後再試。", "失敗", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("註冊時發生錯誤：" + ex.Message, "錯誤", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        // 可以考慮為對話框添加一個取消按鈕
        // private void btnCancel_Click(object sender, EventArgs e)
        // {
        //     this.DialogResult = DialogResult.Cancel;
        // }
    }
}