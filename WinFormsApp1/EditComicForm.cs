using System.Windows.Forms;

namespace WinFormsApp1
{
    public partial class EditComicForm : Form
    {
        private int comicId;
        private string originalISBN;

        public EditComicForm(int comicId, string isbn, string title, string author, string publisher, string category)
        {
            InitializeComponent();
            this.comicId = comicId;
            this.originalISBN = isbn;

            // 載入現有資料
            txtISBN.Text = isbn;
            txtTitle.Text = title;
            txtAuthor.Text = author;
            txtPublisher.Text = publisher;
            txtCategory.Text = category;
        }

        // 屬性用於獲取修改後的漫畫資訊
        public string ComicTitle { get; private set; }
        public string ComicISBN { get; private set; }
        public string ComicAuthor { get; private set; }
        public string ComicPublisher { get; private set; }
        public string ComicCategory { get; private set; }
        public int ComicId => comicId;

        private void btnSave_Click(object sender, EventArgs e)
        {
            // 獲取用戶輸入
            ComicTitle = txtTitle.Text;
            ComicISBN = txtISBN.Text;
            ComicAuthor = txtAuthor.Text;
            ComicPublisher = txtPublisher.Text;
            ComicCategory = txtCategory.Text;

            // Validate ISBN is numeric
            if (!long.TryParse(ComicISBN, out _))
            {
                MessageBox.Show("ISBN 必須是數字！", "輸入錯誤", MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.DialogResult = DialogResult.None; // Stay on the form
                return;
            }

            // 設定 DialogResult 為 OK，表示用戶點擊了儲存
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            // 設定 DialogResult 為 Cancel，表示用戶取消
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
    }
} 