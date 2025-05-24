using System;
using System.Windows.Forms;

namespace WinFormsApp1
{
    internal static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            // To customize application configuration such as set high DPI settings or default font,
            // see https://aka.ms/applicationconfiguration.
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            // 彈出登入表單
            Form1 loginForm = new Form1();
            if (loginForm.ShowDialog() == DialogResult.OK)
            {
                // 登入成功，根據角色開啟不同介面
                if (loginForm.LoggedInUserRole == "admin")
                {
                    // 管理員登入，開啟管理員介面
                    Application.Run(new AdminForm(loginForm.LoggedInUserId));
                }
                else
                {
                    // 普通用戶登入，開啟使用者介面
                    UserForm userForm = new UserForm(loginForm.LoggedInUserId, loginForm.LoggedInUserRole);
                    Application.Run(userForm);
                }
            }
            else
            {
                // 登入失敗或取消，退出應用程式
                Application.Exit();
            }
        }
    }
}