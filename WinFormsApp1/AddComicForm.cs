using System.Windows.Forms;
using System.Drawing;
using System;
using System.IO;
using Minio;
using Minio.Exceptions;
using System.Threading.Tasks;
using Minio.DataModel.Args;

namespace WinFormsApp1
{
    public partial class AddComicForm : Form
    {
        private IMinioClient minioClient;
        private const string MinioEndpoint = "bucket-production-63a9.up.railway.app";
        private const string MinioAccessKey = "dkF1y6M79nH7i8BTBfXuOmf6x6bNJ1rW";
        private const string MinioSecretKey = "Lq4ijpczUkbRznusbOAm0hOWiXLRBdQDb16fJQgbcPH3Q0Xn";
        private const string BucketName = "comicimage";
        private OpenFileDialog openFileDialog;

        public AddComicForm()
        {
            InitializeComponent();
            this.Text = "新增/編輯漫畫";
            InitializeMinioClient();

            openFileDialog = new OpenFileDialog
            {
                Filter = "圖片檔案|*.jpg;*.jpeg;*.png;*.gif;*.bmp",
                Title = "選擇漫畫圖片"
            };

            this.btnSelectImage.Click += BtnSelectImage_Click;
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

        public string ComicTitle { get; private set; }
        public string ComicISBN { get; private set; }
        public string ComicAuthor { get; private set; }
        public string ComicPublisher { get; private set; }
        public string ComicCategory { get; private set; }
        public string ImagePath { get; private set; }
        public string ImageUrl { get; private set; }

        private async void btnSave_Click(object sender, EventArgs e)
        {
            ComicTitle = txtTitle.Text;
            ComicISBN = txtISBN.Text;
            ComicAuthor = txtAuthor.Text;
            ComicPublisher = txtPublisher.Text;
            ComicCategory = txtCategory.Text;

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
                System.Diagnostics.Debug.WriteLine("未選擇圖片，跳過上傳。");
            }

            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
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
    }
} 