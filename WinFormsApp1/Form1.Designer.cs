namespace WinFormsApp1
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        ///  必要的設計器變數。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        ///  清理所有正在使用的資源。
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
        // Windows Form 設計器生成的程式碼

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        ///  設計器支援所需的方法 - 請勿使用程式碼編輯器修改
        /// </summary>
        private void InitializeComponent()
        {
            this.panelLogin = new System.Windows.Forms.Panel();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.txtUsername = new System.Windows.Forms.TextBox();
            this.txtPassword = new System.Windows.Forms.TextBox();
            this.btnLogin = new System.Windows.Forms.Button();
            this.btnRegister = new System.Windows.Forms.Button();
            this.panelLogin.SuspendLayout();
            this.SuspendLayout();
            // 
            // panelLogin
            // 
            // 登入面板，用於置中元件
            this.panelLogin.Controls.Add(this.label1);
            this.panelLogin.Controls.Add(this.label2);
            this.panelLogin.Controls.Add(this.txtUsername);
            this.panelLogin.Controls.Add(this.txtPassword);
            this.panelLogin.Controls.Add(this.btnLogin);
            this.panelLogin.Controls.Add(this.btnRegister);
            this.panelLogin.Location = new System.Drawing.Point(0, 0); // 初始位置不重要，會在程式碼中計算
            this.panelLogin.Name = "panelLogin";
            this.panelLogin.Size = new System.Drawing.Size(500, 250); // 調整面板大小
            this.panelLogin.TabIndex = 0;
            // 
            // label1
            // 
            // 使用者名稱標籤
            this.label1.AutoSize = true; // 自動調整大小
            this.label1.Font = new System.Drawing.Font("Microsoft JhengHei UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.label1.Location = new System.Drawing.Point(30, 50); // 調整位置
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(73, 20);
            this.label1.TabIndex = 3;
            this.label1.Text = "用戶名：";
            // 
            // label2
            // 
            // 密碼標籤
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft JhengHei UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.label2.Location = new System.Drawing.Point(30, 100); // 調整位置
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(57, 20);
            this.label2.TabIndex = 4;
            this.label2.Text = "密碼：";
            // 
            // txtUsername
            // 
            // 使用者名稱文字框
            this.txtUsername.Font = new System.Drawing.Font("Microsoft JhengHei UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.txtUsername.Location = new System.Drawing.Point(110, 47); // 調整位置
            this.txtUsername.Name = "txtUsername";
            this.txtUsername.Size = new System.Drawing.Size(350, 28); // 調整大小
            this.txtUsername.TabIndex = 0;
            // 
            // txtPassword
            // 
            // 密碼文字框
            this.txtPassword.Font = new System.Drawing.Font("Microsoft JhengHei UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.txtPassword.Location = new System.Drawing.Point(110, 97); // 調整位置
            this.txtPassword.Name = "txtPassword";
            this.txtPassword.PasswordChar = '*'; // 設定密碼字元
            this.txtPassword.Size = new System.Drawing.Size(350, 28); // 調整大小
            this.txtPassword.TabIndex = 1;
            // 
            // btnLogin
            // 
            // 登入按鈕
            this.btnLogin.Font = new System.Drawing.Font("Microsoft JhengHei UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.btnLogin.Location = new System.Drawing.Point(200, 150); // 調整位置
            this.btnLogin.Name = "btnLogin";
            this.btnLogin.Size = new System.Drawing.Size(100, 35); // 調整大小
            this.btnLogin.TabIndex = 2;
            this.btnLogin.Text = "登入"; // 設定按鈕文字
            // 
            // btnRegister
            // 
            // 註冊按鈕
            this.btnRegister.Font = new System.Drawing.Font("Microsoft JhengHei UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.btnRegister.Location = new System.Drawing.Point(320, 150); // 放在登入按鈕右側
            this.btnRegister.Name = "btnRegister";
            this.btnRegister.Size = new System.Drawing.Size(100, 35);
            this.btnRegister.TabIndex = 5;
            this.btnRegister.Text = "註冊";
            this.btnRegister.UseVisualStyleBackColor = true;
            // 
            // Form1
            // 
            // Form1 視窗本身
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font; // 自動縮放模式
            this.ClientSize = new System.Drawing.Size(480, 320); // 設定視窗大小
            this.Controls.Add(this.panelLogin); // 將登入面板添加到視窗
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle; // 設定邊框樣式 (如果需要置中，通常不設 FixedSingle，但先保留)
            this.MaximizeBox = false; // 禁止最大化 (如果要做全螢幕置中，通常要允許最大化)
            this.Name = "Form1";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen; // 視窗啟動位置
            this.Text = "漫畫租書系統 - 登入"; // 設定視窗標題
            this.WindowState = System.Windows.Forms.FormWindowState.Normal; // 設定視窗狀態為一般
            this.panelLogin.ResumeLayout(false);
            this.panelLogin.PerformLayout();
            this.ResumeLayout(false);
        }

        #endregion

        // UI 控制項變數宣告
        private System.Windows.Forms.TextBox txtUsername;
        private System.Windows.Forms.TextBox txtPassword;
        private System.Windows.Forms.Button btnLogin;
        private System.Windows.Forms.Button btnRegister;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Panel panelLogin; // 新增的登入面板變數
    }
}