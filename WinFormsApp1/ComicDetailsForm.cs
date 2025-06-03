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
        private Label lblOfferDate;
        private Label lblPages;
        private Label lblSummaryTitle;
        private TextBox txtSummaryContent;
        private PictureBox pbComicImage;

        private string imageUrl;

        public ComicDetailsForm(ComicDetails comic)
        {
            InitializeComponent();
            this.Text = "漫畫詳細資訊";
            this.StartPosition = FormStartPosition.CenterParent;
            this.ClientSize = new Size(700, 725);
            this.MinimumSize = new Size(700, 725);
            this.MaximumSize = new Size(700, 725);

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
            int infoTop = pbComicImage.Top;
            int infoHeight = pbComicImage.Height;
            int infoLines = 7;
            Label[] infoLabels = new Label[infoLines];
            for (int i = 0; i < infoLines; i++)
            {
                infoLabels[i] = new Label { AutoSize = true, Font = new Font("Microsoft JhengHei UI", 16F) };
                this.Controls.Add(infoLabels[i]);
            }
            // 先填內容
            infoLabels[0].Text = $"書名：{comic.書名}";
            infoLabels[1].Text = $"作者：{comic.作者}";
            infoLabels[2].Text = $"出版社：{comic.出版社}";
            infoLabels[3].Text = $"分類：{comic.分類}";
            infoLabels[4].Text = $"ISBN：{comic.ISBN}";
            infoLabels[5].Text = $"出版日：{comic.OfferDate}";
            infoLabels[6].Text = $"頁數：{comic.Pages}";
            // 強制計算Label高度
            this.PerformLayout();
            // 計算所有Label的總高度
            int totalLabelsHeight = 0;
            for (int i = 0; i < infoLines; i++)
                totalLabelsHeight += infoLabels[i].Height;
            // 計算需要平均分配的間隔空間
            int totalGapSpace = infoHeight - totalLabelsHeight;
            int numberOfGaps = infoLines > 1 ? infoLines - 1 : 0;
            int adjustedGapHeight = numberOfGaps > 0 ? totalGapSpace / numberOfGaps : 0; // 剛好的行距，不再減去像素
            // 根據計算出的間隔重新設定每一行的Y座標
            int currentY = infoTop;
            for (int i = 0; i < infoLines; i++)
            {
                infoLabels[i].Location = new Point(textLeft, currentY);
                currentY += infoLabels[i].Height + adjustedGapHeight;
            }
            int summaryTop = pbComicImage.Bottom + 30;
            lblSummaryTitle = new Label
            {
                AutoSize = true,
                Location = new Point(20, summaryTop),
                Font = new Font("Microsoft JhengHei UI", 14F, FontStyle.Bold),
                Text = "內容簡介"
            };
            txtSummaryContent = new TextBox
            {
                Location = new Point(20, summaryTop + 36),
                Size = new Size(this.ClientSize.Width - 40, this.ClientSize.Height - (summaryTop + 36) - 20),
                Multiline = true,
                ReadOnly = true,
                ScrollBars = ScrollBars.Vertical,
                Font = new Font("Microsoft JhengHei UI", 11F),
                WordWrap = true,
                BorderStyle = BorderStyle.FixedSingle
            };
            this.Controls.Add(lblSummaryTitle);
            this.Controls.Add(txtSummaryContent);
            txtSummaryContent.Text = string.IsNullOrWhiteSpace(comic.BookSummary) ? "(無摘要)" : comic.BookSummary;
            txtSummaryContent.SelectionStart = 0;
            txtSummaryContent.SelectionLength = 0;

            this.Load += ComicDetailsForm_Load;
            this.Shown += (s, e) =>
            {
                this.Height = 725;
                this.MinimumSize = new Size(700, 725);
                this.MaximumSize = new Size(700, 725);
            };
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