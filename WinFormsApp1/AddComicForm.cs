using System.Windows.Forms;
using System.Drawing;
using System;
using System.IO;

namespace WinFormsApp1
{
    public partial class AddComicForm : Form
    {
        private PictureBox pbComicImage;
        private Button btnSelectImage;
        private OpenFileDialog openFileDialog;

        public AddComicForm()
        {
            InitializeComponent();
            this.Text = "新增/編輯漫畫";

            pbComicImage = new PictureBox
            {
                BorderStyle = BorderStyle.FixedSingle,
                SizeMode = PictureBoxSizeMode.Zoom,
                Size = new Size(150, 200),
                Location = new Point(250, 20)
            };

            btnSelectImage = new Button
            {
                Text = "選擇圖片",
                Size = new Size(100, 30),
                Location = new Point(275, 230)
            };
            btnSelectImage.Click += BtnSelectImage_Click;

            openFileDialog = new OpenFileDialog
            {
                Filter = "圖片檔案|*.jpg;*.jpeg;*.png;*.gif;*.bmp",
                Title = "選擇漫畫圖片"
            };

            this.Controls.Add(pbComicImage);
            this.Controls.Add(btnSelectImage);
        }

        public string ComicTitle { get; private set; }
        public string ComicISBN { get; private set; }
        public string ComicAuthor { get; private set; }
        public string ComicPublisher { get; private set; }
        public string ComicCategory { get; private set; }
        public string ImagePath { get; private set; }

        private void btnSave_Click(object sender, EventArgs e)
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