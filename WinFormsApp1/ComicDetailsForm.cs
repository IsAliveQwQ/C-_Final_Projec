using System.Windows.Forms;
using System.Drawing;
using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

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
        private PictureBox pbComicImage;

        private string imageUrl;

        public ComicDetailsForm(ComicDetails comic)
        {
            InitializeComponent();
            this.Text = "漫畫詳細資訊";
            this.StartPosition = FormStartPosition.CenterParent;

            this.imageUrl = comic.ImageUrl;

            pbComicImage = new PictureBox
            {
                Location = new Point(20, 20),
                Size = new Size(250, 369),
                BorderStyle = BorderStyle.FixedSingle,
                SizeMode = PictureBoxSizeMode.Zoom
            };
            this.Controls.Add(pbComicImage);

            int textLeft = pbComicImage.Right + 20;
            int initialTop = 20;
            int lineHeight = 57;

            lblTitle = new Label { AutoSize = true, Location = new Point(textLeft, initialTop), Font = new Font("Microsoft JhengHei UI", 16F) };
            lblAuthor = new Label { AutoSize = true, Location = new Point(textLeft, initialTop + lineHeight), Font = new Font("Microsoft JhengHei UI", 16F) };
            lblPublisher = new Label { AutoSize = true, Location = new Point(textLeft, initialTop + 2 * lineHeight), Font = new Font("Microsoft JhengHei UI", 16F) };
            lblCategory = new Label { AutoSize = true, Location = new Point(textLeft, initialTop + 3 * lineHeight), Font = new Font("Microsoft JhengHei UI", 16F) };
            lblISBN = new Label { AutoSize = true, Location = new Point(textLeft, initialTop + 4 * lineHeight), Font = new Font("Microsoft JhengHei UI", 16F) };
            lblBorrowStatus = new Label { AutoSize = true, Location = new Point(textLeft, initialTop + 5 * lineHeight), Font = new Font("Microsoft JhengHei UI", 16F) };
            lblReserveStatus = new Label { AutoSize = true, Location = new Point(textLeft, initialTop + 6 * lineHeight), Font = new Font("Microsoft JhengHei UI", 16F) };

            this.Controls.Add(lblTitle);
            this.Controls.Add(lblAuthor);
            this.Controls.Add(lblPublisher);
            this.Controls.Add(lblCategory);
            this.Controls.Add(lblISBN);
            this.Controls.Add(lblBorrowStatus);
            this.Controls.Add(lblReserveStatus);

            DisplayComicDetails(comic);

            this.Load += ComicDetailsForm_Load;
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

        private async void ComicDetailsForm_Load(object sender, EventArgs e)
        {
            await LoadComicImageAsync(this.imageUrl);
        }

        private async Task LoadComicImageAsync(string imageUrl)
        {
            if (!string.IsNullOrEmpty(imageUrl))
            {
                string imageUrlToLoad = imageUrl;
                if (!imageUrlToLoad.StartsWith("http://") && !imageUrlToLoad.StartsWith("https://"))
                {
                    imageUrlToLoad = "https://" + imageUrlToLoad;
                }

                try
                {
                    using (var httpClient = new System.Net.Http.HttpClient())
                    {
                        using (var stream = await httpClient.GetStreamAsync(imageUrlToLoad))
                        {
                            using (var memoryStream = new MemoryStream())
                            {
                                await stream.CopyToAsync(memoryStream);
                                memoryStream.Position = 0;

                                this.Invoke((MethodInvoker)delegate
                                {
                                    try
                                    {
                                        pbComicImage.Image = System.Drawing.Image.FromStream(memoryStream);
                                    }
                                    catch (Exception uiEx)
                                    {
                                        System.Diagnostics.Debug.WriteLine($"在 UI 線程上從 MemoryStream 建立 Image 物件時發生錯誤: {uiEx.Message}");
                                        pbComicImage.Image = null;
                                    }
                                });
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"下載或處理圖片時發生錯誤: {ex.Message}\n{ex.StackTrace}");
                    this.Invoke((MethodInvoker)delegate
                    {
                        pbComicImage.Image = null;
                    });
                }
            }
        }
    }
} 