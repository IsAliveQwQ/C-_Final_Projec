using System.Windows.Forms;

namespace WinFormsApp1
{
    public partial class AddUserForm : Form
    {
        public AddUserForm()
        {
            InitializeComponent();
        }

        public string Username { get; private set; }
        public string Password { get; private set; }

        private void btnSave_Click(object sender, System.EventArgs e)
        {
            Username = txtUsername.Text;
            Password = txtPassword.Text;
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void btnCancel_Click(object sender, System.EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
    }
} 