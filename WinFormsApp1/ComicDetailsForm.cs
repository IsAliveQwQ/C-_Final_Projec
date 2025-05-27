using System.Windows.Forms;
using System.Drawing;

namespace WinFormsApp1
{
    public partial class ComicDetailsForm : Form
    {
        private Label lblTitle;
        private Label lblAuthor;
        private Label lblPublisher;
        private Label lblCategory;
        private Label lblISBN;
        private Label lblBorrowStatus;
        private Label lblReserveStatus;

        public ComicDetailsForm(ComicDetails comic)
        {
            InitializeComponent();
            this.Text = "漫畫詳細資訊";
            this.StartPosition = FormStartPosition.CenterParent;

            // 初始化標籤
            lblTitle = new Label { AutoSize = true, Location = new Point(20, 20), Font = new Font("Microsoft JhengHei UI", 12F) };
            lblAuthor = new Label { AutoSize = true, Location = new Point(20, 50), Font = new Font("Microsoft JhengHei UI", 12F) };
            lblPublisher = new Label { AutoSize = true, Location = new Point(20, 80), Font = new Font("Microsoft JhengHei UI", 12F) };
            lblCategory = new Label { AutoSize = true, Location = new Point(20, 110), Font = new Font("Microsoft JhengHei UI", 12F) };
            lblISBN = new Label { AutoSize = true, Location = new Point(20, 140), Font = new Font("Microsoft JhengHei UI", 12F) };
            lblBorrowStatus = new Label { AutoSize = true, Location = new Point(20, 170), Font = new Font("Microsoft JhengHei UI", 12F) };
            lblReserveStatus = new Label { AutoSize = true, Location = new Point(20, 200), Font = new Font("Microsoft JhengHei UI", 12F) };

            // 將標籤添加到表單中
            this.Controls.Add(lblTitle);
            this.Controls.Add(lblAuthor);
            this.Controls.Add(lblPublisher);
            this.Controls.Add(lblCategory);
            this.Controls.Add(lblISBN);
            this.Controls.Add(lblBorrowStatus);
            this.Controls.Add(lblReserveStatus);

            // 顯示漫畫詳細資訊
            DisplayComicDetails(comic);
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // ComicDetailsForm
            // 
            this.ClientSize = new System.Drawing.Size(280, 260);
            this.Name = "ComicDetailsForm";
            this.Text = "漫畫詳細資訊";
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        public void DisplayComicDetails(ComicDetails comic)
        {
            if (comic != null)
            {
                lblTitle.Text = $"書名：{comic.書名}";
                lblAuthor.Text = $"作者：{comic.作者}";
                lblPublisher.Text = $"出版社：{comic.出版社}";
                lblCategory.Text = $"分類：{comic.分類}";
                lblISBN.Text = $"ISBN：{comic.ISBN}";
                lblBorrowStatus.Text = $"借閱狀態：{comic.借閱狀態}";
                lblReserveStatus.Text = $"預約狀態：{comic.預約狀態}";
            }
        }
    }
} 