using System.Windows.Forms;
using System.Drawing;
using System;
using System.IO;
using Minio;
using Minio.Exceptions;
using System.Threading.Tasks;
using Minio.DataModel.Args;
using System.Net.Http;

namespace WinFormsApp1
{
    public partial class EditComicForm : Form
    {
        private int comicId;
        private string originalISBN;
        private IMinioClient minioClient;
        private const string MinioEndpoint = "bucket-production-63a9.up.railway.app";
        private const string MinioAccessKey = "dkF1y6M79nH7i8BTBfXuOmf6x6bNJ1rW";
        private const string MinioSecretKey = "Lq4ijpczUkbRznusbOAm0hOWiXLRBdQDb16fJQgbcPH3Q0Xn";
        private const string BucketName = "comicimage";
        private string currentImageUrl;
        private OpenFileDialog openFileDialog;

        public EditComicForm(int comicId, string isbn, string title, string author, string publisher, string category, string imageUrl = null, string offerDate = null, string pages = null, string bookSummary = null)
        {
            InitializeComponent();
            this.comicId = comicId;
            this.originalISBN = isbn;
            this.currentImageUrl = imageUrl;

            InitializeMinioClient();

            openFileDialog = new OpenFileDialog
            {
                Filter = "圖片檔案|*.jpg;*.jpeg;*.png;*.gif;*.bmp",
                Title = "選擇漫畫圖片"
            };

            this.btnSelectImage.Click += BtnSelectImage_Click;
            // 為 txtOfferDate 添加 Enter 和 MouseClick 事件處理程式
            this.txtOfferDate.Enter += TxtOfferDate_Enter;
            this.txtOfferDate.MouseClick += TxtOfferDate_MouseClick;

            // 載入現有資料到控制項
            txtISBN.Text = isbn;
            txtTitle.Text = title;
            txtAuthor.Text = author;
            txtPublisher.Text = publisher;
            txtCategory.Text = category;
            txtOfferDate.Text = offerDate; // 載入發售日
            txtPages.Text = pages; // 載入頁數
            txtBookSummary.Text = bookSummary; // 載入摘要

            // 如果有現有圖片，顯示它
            // 圖片載入邏輯將移至 LoadComicImageAsync 方法
        }

        private void InitializeMinioClient()
        {
            try
            {
                minioClient = new MinioClient()
                    .WithEndpoint(MinioEndpoint)
                    .WithCredentials(MinioAccessKey, MinioSecretKey)
                    .Build();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"初始化 MinIO 客戶端時發生錯誤: {ex.Message}", "MinIO 錯誤", MessageBoxButtons.OK, MessageBoxIcon.Error);
                minioClient = null;
            }
        }

        // 處理圖片載入的非同步方法
        private async Task LoadComicImageAsync()
        {
            if (!string.IsNullOrEmpty(currentImageUrl))
            {
                // 檢查並修正 URL，確保有協定
                string imageUrlToLoad = currentImageUrl;
                if (!imageUrlToLoad.StartsWith("http://") && !imageUrlToLoad.StartsWith("https://"))
                {
                    imageUrlToLoad = "https://" + imageUrlToLoad;
                    System.Diagnostics.Debug.WriteLine($"LoadComicImageAsync: 修正後的圖片 URL: {imageUrlToLoad}");
                }

                System.Diagnostics.Debug.WriteLine($"LoadComicImageAsync: 嘗試從 URL 載入圖片: {imageUrlToLoad}");
                try
                {
                    using (var httpClient = new System.Net.Http.HttpClient())
                    {
                        System.Diagnostics.Debug.WriteLine("LoadComicImageAsync: 開始下載圖片流...");
                        // 確保使用修正後的 imageUrlToLoad 進行下載
                        using (var stream = await httpClient.GetStreamAsync(imageUrlToLoad))
                        {
                            System.Diagnostics.Debug.WriteLine("LoadComicImageAsync: 圖片流下載完成，複製到 MemoryStream...");
                            using (var memoryStream = new MemoryStream())
                            {
                                await stream.CopyToAsync(memoryStream);
                                memoryStream.Position = 0; // 重設流的位置到開頭
                                System.Diagnostics.Debug.WriteLine($"LoadComicImageAsync: 圖片流複製完成，MemoryStream 大小: {memoryStream.Length}");

                                // 將圖片載入到 PictureBox (需要在 UI 線程上更新)
                                this.Invoke((MethodInvoker)delegate
                                {
                                    try
                                    {
                                        System.Diagnostics.Debug.WriteLine("LoadComicImageAsync: 在 UI 線程上嘗試從 MemoryStream 建立 Image 物件...");
                                        pbComicImage.Image = System.Drawing.Image.FromStream(memoryStream);
                                        System.Diagnostics.Debug.WriteLine("LoadComicImageAsync: Image 物件建立成功，更新 PictureBox。");
                                    }
                                    catch (Exception uiEx)
                                    {
                                        System.Diagnostics.Debug.WriteLine($"LoadComicImageAsync: 在 UI 線程上從 MemoryStream 建立 Image 物件時發生錯誤: {uiEx.Message}");
                                        // 可以在 UI 線程上清空 PictureBox 或顯示預設圖片
                                        pbComicImage.Image = null;
                                    }
                                });
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    // 如果無法載入圖片，顯示錯誤或忽略
                    System.Diagnostics.Debug.WriteLine($"LoadComicImageAsync: 下載或處理圖片時發生錯誤: {ex.Message}\n{ex.StackTrace}");
                    // 可以在 UI 線程上清空 PictureBox 或顯示預設圖片
                     this.Invoke((MethodInvoker)delegate
                    {
                         pbComicImage.Image = null;
                    });
                }
            }
            else
            {
                 System.Diagnostics.Debug.WriteLine("LoadComicImageAsync: currentImageUrl 為空，跳過圖片載入。");
            }
        }

        // 屬性用於獲取修改後的漫畫資訊
        public string ComicTitle { get; private set; }
        public string ComicISBN { get; private set; }
        public string ComicAuthor { get; private set; }
        public string ComicPublisher { get; private set; }
        public string ComicCategory { get; private set; }
        public string ImagePath { get; private set; }
        public string ImageUrl { get; private set; }
        public int ComicId => comicId;
        public string OfferDate { get; private set; } // 新增 OfferDate 屬性
        public string Pages { get; private set; } // 新增 Pages 屬性
        public string BookSummary { get; private set; } // 新增 BookSummary 屬性

        private async void btnSave_Click(object sender, EventArgs e)
        {
            // 獲取用戶輸入
            ComicTitle = txtTitle.Text;
            ComicISBN = txtISBN.Text;
            ComicAuthor = txtAuthor.Text;
            ComicPublisher = txtPublisher.Text;
            ComicCategory = txtCategory.Text;
            OfferDate = txtOfferDate.Text; // 獲取發售日
            Pages = txtPages.Text; // 獲取頁數
            BookSummary = txtBookSummary.Text; // 獲取摘要

            // Validate ISBN is numeric
            if (!long.TryParse(ComicISBN, out _))
            {
                MessageBox.Show("ISBN 必須是數字！", "輸入錯誤", MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.DialogResult = DialogResult.None;
                return;
            }

            if (!string.IsNullOrEmpty(ImagePath))
            {
                System.Diagnostics.Debug.WriteLine($"嘗試上傳圖片: {ImagePath}");
                try
                {
                    string objectName = $"{ComicISBN}_{DateTime.Now.Ticks}{Path.GetExtension(ImagePath)}";
                    System.Diagnostics.Debug.WriteLine($"MinIO 物件名稱: {objectName}");
                    using (var fileStream = File.OpenRead(ImagePath))
                    {
                        var putObjectArgs = new PutObjectArgs()
                            .WithBucket(BucketName)
                            .WithObject(objectName)
                            .WithStreamData(fileStream)
                            .WithObjectSize(fileStream.Length)
                            .WithContentType("image/" + Path.GetExtension(ImagePath).TrimStart('.'));

                        await minioClient.PutObjectAsync(putObjectArgs);
                        ImageUrl = $"https://{MinioEndpoint}/{BucketName}/{objectName}";
                        System.Diagnostics.Debug.WriteLine($"圖片上傳成功，URL: {ImageUrl}");
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"上傳圖片時發生錯誤: {ex.Message}");
                    MessageBox.Show($"上傳圖片時發生錯誤: {ex.Message}", "上傳錯誤", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    this.DialogResult = DialogResult.None;
                    return;
                }
            }
            else
            {
                ImageUrl = currentImageUrl;
                System.Diagnostics.Debug.WriteLine("未選擇新圖片，使用現有圖片URL。");
            }

            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void BtnSelectImage_Click(object sender, EventArgs e)
        {
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                ImagePath = openFileDialog.FileName;
                pbComicImage.Image = Image.FromFile(ImagePath);
            }
        }

        // 新增表單載入事件處理程式
        private async void EditComicForm_Load(object sender, EventArgs e)
        {
            await LoadComicImageAsync();
        }

        // txtOfferDate 的 Enter 事件處理程式
        private void TxtOfferDate_Enter(object sender, EventArgs e)
        {
            ((MaskedTextBox)sender).SelectionStart = 0;
        }

        // txtOfferDate 的 MouseClick 事件處理程式
        private void TxtOfferDate_MouseClick(object sender, MouseEventArgs e)
        {
            ((MaskedTextBox)sender).SelectionStart = 0;
        }
    }
} 