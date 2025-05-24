using System.Windows.Forms;

namespace WinFormsApp1
{
    public partial class AddComicForm : Form
    {
        public AddComicForm()
        {
            InitializeComponent();
        }

        // 在這裡可以添加屬性或方法來獲取用戶輸入的漫畫資訊
        public string ComicTitle { get; private set; }
        public string ComicISBN { get; private set; }
        public string ComicAuthor { get; private set; }
        public string ComicPublisher { get; private set; }
        public string ComicCategory { get; private set; }

        // 按鈕事件處理器，用於獲取輸入並關閉視窗
        private void btnSave_Click(object sender, EventArgs e)
        {
            // 獲取用戶輸入
            ComicTitle = txtTitle.Text;
            ComicISBN = txtISBN.Text;
            ComicAuthor = txtAuthor.Text;
            ComicPublisher = txtPublisher.Text;
            ComicCategory = txtCategory.Text;

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