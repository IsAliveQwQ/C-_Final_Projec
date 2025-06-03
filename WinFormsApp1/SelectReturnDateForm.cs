using System;
using System.Drawing;
using System.Windows.Forms;

namespace WinFormsApp1
{
    public partial class SelectReturnDateForm : Form
    {
        public DateTime SelectedDate { get; private set; }

        private DateTimePicker dateTimePicker;
        private Button btnOK;
        private Button btnCancel;

        public SelectReturnDateForm(DateTime borrowDate)
        {
            InitializeComponent();

            this.Text = "選擇歸還日期";
            this.StartPosition = FormStartPosition.CenterParent;
            this.ClientSize = new Size(300, 150);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;

            // 初始化 DateTimePicker
            dateTimePicker = new DateTimePicker()
            {
                Name = "dateTimePicker",
                Format = DateTimePickerFormat.Short,
                Location = new Point(50, 30),
                Size = new Size(200, 25)
            };

            // 設定可選日期範圍：從借閱日期的後一天開始，最長七天
            DateTime minDate = borrowDate.AddDays(1); // 至少是借閱日期的隔天
            DateTime maxDate = borrowDate.AddDays(7); // 最多七天後

            dateTimePicker.MinDate = minDate;
            dateTimePicker.MaxDate = maxDate;
            dateTimePicker.Value = DateTime.Now > minDate ? DateTime.Now : minDate; // 預設選擇今天或最小日期

            // 初始化按鈕
            btnOK = new Button()
            {
                Text = "確定",
                DialogResult = DialogResult.OK,
                Location = new Point(50, 80),
                Size = new Size(75, 30)
            };

            btnCancel = new Button()
            {
                Text = "取消",
                DialogResult = DialogResult.Cancel,
                Location = new Point(175, 80),
                Size = new Size(75, 30)
            };

            // 添加控制項到表單
            this.Controls.Add(dateTimePicker);
            this.Controls.Add(btnOK);
            this.Controls.Add(btnCancel);

            // 綁定事件
            this.Load += SelectReturnDateForm_Load;
            btnOK.Click += BtnOK_Click;
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // SelectReturnDateForm
            // 
            this.ClientSize = new System.Drawing.Size(282, 253);
            this.Name = "SelectReturnDateForm";
            this.ResumeLayout(false);
        }

        private void SelectReturnDateForm_Load(object sender, EventArgs e)
        {
            // 確保 DateTimePicker 在表單載入時顯示正確的初始日期
            // 初始值已在建構函式中設定
        }

        private void BtnOK_Click(object sender, EventArgs e)
        {
            SelectedDate = dateTimePicker.Value;
        }
    }
} 