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
            this.txtOfferDate.Enter += TxtOfferDate_Enter;
            this.txtOfferDate.MouseClick += TxtOfferDate_MouseClick;

            txtISBN.Text = isbn;
            txtTitle.Text = title;
            txtAuthor.Text = author;
            txtPublisher.Text = publisher;
            txtCategory.Text = category;
            txtOfferDate.Text = offerDate; 
            txtPages.Text = pages; 
            txtBookSummary.Text = bookSummary; 

          
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
                MessageBox.Show($"初始化 MinIO 時發生錯誤: {ex.Message}", "MinIO 錯誤", MessageBoxButtons.OK, MessageBoxIcon.Error);
                minioClient = null;
            }
        }

        
        private async Task LoadComicImageAsync()
        {
            if (!string.IsNullOrEmpty(currentImageUrl))
            {
                
                string imageUrlToLoad = currentImageUrl;
                if (!imageUrlToLoad.StartsWith("http://") && !imageUrlToLoad.StartsWith("https://"))
                {
                    imageUrlToLoad = "https://" + imageUrlToLoad;
                    System.Diagnostics.Debug.WriteLine($": {imageUrlToLoad}");
                }

                System.Diagnostics.Debug.WriteLine($": {imageUrlToLoad}");
                try
                {
                    using (var httpClient = new System.Net.Http.HttpClient())
                    {
                        System.Diagnostics.Debug.WriteLine("");
                        using (var stream = await httpClient.GetStreamAsync(imageUrlToLoad))
                        {
                            System.Diagnostics.Debug.WriteLine("");
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
                                        pbComicImage.Image = null;
                                    }
                                });
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                     this.Invoke((MethodInvoker)delegate
                    {
                         pbComicImage.Image = null;
                    });
                }
            }
           
        }
        public string ComicTitle { get; private set; }
        public string ComicISBN { get; private set; }
        public string ComicAuthor { get; private set; }
        public string ComicPublisher { get; private set; }
        public string ComicCategory { get; private set; }
        public string ImagePath { get; private set; }
        public string ImageUrl { get; private set; }
        public int ComicId => comicId;
        public string OfferDate { get; private set; } 
        public string Pages { get; private set; } 
        public string BookSummary { get; private set; } 

        private async void btnSave_Click(object sender, EventArgs e)
        {
            ComicTitle = txtTitle.Text.Trim();
            ComicISBN = txtISBN.Text.Trim();
            ComicAuthor = txtAuthor.Text.Trim();
            ComicPublisher = txtPublisher.Text.Trim();
            ComicCategory = txtCategory.Text.Trim();
            OfferDate = txtOfferDate.Text.Trim();
            Pages = txtPages.Text.Trim();
            BookSummary = txtBookSummary.Text.Trim();

            if (string.IsNullOrWhiteSpace(ComicTitle) || string.IsNullOrWhiteSpace(ComicISBN) ||
                string.IsNullOrWhiteSpace(ComicAuthor) || string.IsNullOrWhiteSpace(ComicPublisher) ||
                string.IsNullOrWhiteSpace(ComicCategory) || string.IsNullOrWhiteSpace(OfferDate) ||
                string.IsNullOrWhiteSpace(Pages) || string.IsNullOrWhiteSpace(BookSummary))
            {
                MessageBox.Show("所有欄位皆為必填，請完整填寫！", "輸入錯誤", MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.DialogResult = DialogResult.None;
                return;
            }

            if (!long.TryParse(ComicISBN, out _) || ComicISBN.Length < 6)
            {
                MessageBox.Show("ISBN 必須為至少6位數字！", "輸入錯誤", MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.DialogResult = DialogResult.None;
                return;
            }

            if (!System.Text.RegularExpressions.Regex.IsMatch(OfferDate, @"^\d{4}/\d{2}/\d{2}$"))
            {
                MessageBox.Show("發售日格式必須為 yyyy/mm/dd！", "輸入錯誤", MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.DialogResult = DialogResult.None;
                return;
            }
            else
            {
                
                if (!DateTime.TryParseExact(OfferDate, "yyyy/MM/dd", null, System.Globalization.DateTimeStyles.None, out _))
                {
                    MessageBox.Show("發售日不是有效日期！", "輸入錯誤", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    this.DialogResult = DialogResult.None;
                    return;
                }
            }

            
            if (!int.TryParse(Pages, out int pagesInt) || pagesInt <= 0)
            {
                MessageBox.Show("頁數必須為正整數！", "輸入錯誤", MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.DialogResult = DialogResult.None;
                return;
            }

            if (string.IsNullOrEmpty(ImagePath) && string.IsNullOrEmpty(currentImageUrl))
            {
                MessageBox.Show("請選擇漫畫圖片！", "輸入錯誤", MessageBoxButtons.OK, MessageBoxIcon.Error);
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

        
        private async void EditComicForm_Load(object sender, EventArgs e)
        {
            await LoadComicImageAsync();
        }

        private void TxtOfferDate_Enter(object sender, EventArgs e)
        {
            ((MaskedTextBox)sender).SelectionStart = 0;
        }

        private void TxtOfferDate_MouseClick(object sender, MouseEventArgs e)
        {
            ((MaskedTextBox)sender).SelectionStart = 0;
        }
    }
} 