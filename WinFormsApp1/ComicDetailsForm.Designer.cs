using System.Windows.Forms;

namespace WinFormsApp1
{
    public partial class ComicDetailsForm : Form
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
            this.SuspendLayout();
            // 
            // ComicDetailsForm
            // 
            this.ClientSize = new System.Drawing.Size(550, 750); // 調整表單大小以符合新布局，高度增加300像素
            this.Name = "ComicDetailsForm";
            this.Text = "漫畫詳細資訊"; // 確保這裡設定了表單標題
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        #endregion
    }
}