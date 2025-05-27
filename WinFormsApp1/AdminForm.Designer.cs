namespace WinFormsApp1
{
    partial class AdminForm
    {
        /// <summary>
        ///  Required designer variable.
        ///  必要的設計器變數。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        ///  清理所有正在使用的資源。
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code
        // Windows Form 設計器生成的程式碼

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        ///  設計器支援所需的方法 - 請勿使用程式碼編輯器修改
        /// </summary>
        private void InitializeComponent()
        {
            tabAdmin = new TabControl();
            tabUser = new TabPage();
            dgvUser = new DataGridView();
            panelUserSearch = new Panel();
            txtSearchUser = new TextBox();
            cmbUserSearchType = new ComboBox();
            btnUserSearch = new Button();
            btnAddUser = new Button();
            btnEditUser = new Button();
            btnDeleteUser = new Button();
            btnRefreshUser = new Button();
            panelUserPaging = new Panel();
            btnUserPrev = new Button();
            lblUserPage = new Label();
            btnUserNext = new Button();
            tabComic = new TabPage();
            dgvComic = new DataGridView();
            panelComicSearch = new Panel();
            txtComicKeyword = new TextBox();
            cmbComicSearchType = new ComboBox();
            btnComicSearch = new Button();
            btnAddComic = new Button();
            btnEditComic = new Button();
            btnDeleteComic = new Button();
            btnRefreshComic = new Button();
            panelComicPaging = new Panel();
            btnComicPrev = new Button();
            lblComicPage = new Label();
            btnComicNext = new Button();
            tabBorrow = new TabPage();
            panelBorrowPaging = new Panel();
            btnBorrowPrev = new Button();
            lblBorrowPage = new Label();
            btnBorrowNext = new Button();
            dgvBorrow = new DataGridView();
            panelBorrowSearch = new Panel();
            txtBorrowKeyword = new TextBox();
            cmbBorrowSearchType = new ComboBox();
            btnBorrowSearch = new Button();
            btnRefreshBorrow = new Button();
            tabReserve = new TabPage();
            panelReservePaging = new Panel();
            btnReservePrev = new Button();
            lblReservePage = new Label();
            btnReserveNext = new Button();
            dgvReserve = new DataGridView();
            panelReserveSearch = new Panel();
            txtReserveKeyword = new TextBox();
            cmbReserveSearchType = new ComboBox();
            btnReserveSearch = new Button();
            btnRefreshReserve = new Button();
            tabLog = new TabPage();
            dgvLog = new DataGridView();
            panelLogSearch = new Panel();
            txtLogKeyword = new TextBox();
            btnLogSearch = new Button();
            btnRefreshLog = new Button();
            panelLogPaging = new Panel();
            btnLogPrev = new Button();
            lblLogPage = new Label();
            btnLogNext = new Button();
            tabAdmin.SuspendLayout();
            tabUser.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dgvUser).BeginInit();
            panelUserSearch.SuspendLayout();
            panelUserPaging.SuspendLayout();
            tabComic.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dgvComic).BeginInit();
            panelComicSearch.SuspendLayout();
            panelComicPaging.SuspendLayout();
            tabBorrow.SuspendLayout();
            panelBorrowPaging.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dgvBorrow).BeginInit();
            panelBorrowSearch.SuspendLayout();
            tabReserve.SuspendLayout();
            panelReservePaging.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dgvReserve).BeginInit();
            panelReserveSearch.SuspendLayout();
            tabLog.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dgvLog).BeginInit();
            panelLogSearch.SuspendLayout();
            panelLogPaging.SuspendLayout();
            SuspendLayout();
            // 
            // tabAdmin
            // 
            tabAdmin.Controls.Add(tabUser);
            tabAdmin.Controls.Add(tabComic);
            tabAdmin.Controls.Add(tabBorrow);
            tabAdmin.Controls.Add(tabReserve);
            tabAdmin.Controls.Add(tabLog);
            tabAdmin.Dock = DockStyle.Fill;
            tabAdmin.Location = new Point(0, 0);
            tabAdmin.Margin = new Padding(3, 2, 3, 2);
            tabAdmin.Name = "tabAdmin";
            tabAdmin.SelectedIndex = 0;
            tabAdmin.Size = new Size(910, 796);
            tabAdmin.TabIndex = 0;
            // 
            // tabUser
            // 
            tabUser.Controls.Add(dgvUser);
            tabUser.Controls.Add(panelUserSearch);
            tabUser.Controls.Add(panelUserPaging);
            tabUser.Location = new Point(4, 24);
            tabUser.Margin = new Padding(3, 2, 3, 2);
            tabUser.Name = "tabUser";
            tabUser.Size = new Size(902, 768);
            tabUser.TabIndex = 0;
            tabUser.Text = "用戶管理";
            // 
            // dgvUser
            // 
            dgvUser.Dock = DockStyle.Fill;
            dgvUser.Location = new Point(0, 30);
            dgvUser.Margin = new Padding(3, 2, 3, 2);
            dgvUser.Name = "dgvUser";
            dgvUser.Size = new Size(902, 708);
            dgvUser.TabIndex = 0;
            // 
            // panelUserSearch
            // 
            panelUserSearch.Controls.Add(txtSearchUser);
            panelUserSearch.Controls.Add(cmbUserSearchType);
            panelUserSearch.Controls.Add(btnUserSearch);
            panelUserSearch.Controls.Add(btnAddUser);
            panelUserSearch.Controls.Add(btnEditUser);
            panelUserSearch.Controls.Add(btnDeleteUser);
            panelUserSearch.Controls.Add(btnRefreshUser);
            panelUserSearch.Dock = DockStyle.Top;
            panelUserSearch.Location = new Point(0, 0);
            panelUserSearch.Margin = new Padding(3, 2, 3, 2);
            panelUserSearch.Name = "panelUserSearch";
            panelUserSearch.Size = new Size(902, 35);
            panelUserSearch.TabIndex = 1;
            // 
            // txtSearchUser
            // 
            txtSearchUser.Location = new Point(9, 6);
            txtSearchUser.Margin = new Padding(3, 2, 3, 2);
            txtSearchUser.Name = "txtSearchUser";
            txtSearchUser.Size = new Size(158, 23);
            txtSearchUser.TabIndex = 0;
            // 
            // cmbUserSearchType
            // 
            cmbUserSearchType.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbUserSearchType.Items.AddRange(new object[] { "用戶ID", "用戶名" });
            cmbUserSearchType.Location = new Point(175, 6);
            cmbUserSearchType.Margin = new Padding(3, 2, 3, 2);
            cmbUserSearchType.Name = "cmbUserSearchType";
            cmbUserSearchType.Size = new Size(70, 23);
            cmbUserSearchType.TabIndex = 1;
            // 
            // btnUserSearch
            // 
            btnUserSearch.Location = new Point(254, 8);
            btnUserSearch.Margin = new Padding(3, 2, 3, 2);
            btnUserSearch.Name = "btnUserSearch";
            btnUserSearch.Size = new Size(61, 25);
            btnUserSearch.TabIndex = 2;
            btnUserSearch.Text = "搜尋";
            btnUserSearch.UseVisualStyleBackColor = true;
            // 
            // btnAddUser
            // 
            btnAddUser.Location = new Point(324, 8);
            btnAddUser.Margin = new Padding(3, 2, 3, 2);
            btnAddUser.Name = "btnAddUser";
            btnAddUser.Size = new Size(79, 25);
            btnAddUser.TabIndex = 3;
            btnAddUser.Text = "新增用戶";
            btnAddUser.UseVisualStyleBackColor = true;
            // 
            // btnEditUser
            // 
            btnEditUser.Location = new Point(411, 8);
            btnEditUser.Margin = new Padding(3, 2, 3, 2);
            btnEditUser.Name = "btnEditUser";
            btnEditUser.Size = new Size(79, 25);
            btnEditUser.TabIndex = 4;
            btnEditUser.Text = "編輯用戶";
            btnEditUser.UseVisualStyleBackColor = true;
            // 
            // btnDeleteUser
            // 
            btnDeleteUser.Location = new Point(499, 8);
            btnDeleteUser.Margin = new Padding(3, 2, 3, 2);
            btnDeleteUser.Name = "btnDeleteUser";
            btnDeleteUser.Size = new Size(79, 25);
            btnDeleteUser.TabIndex = 5;
            btnDeleteUser.Text = "刪除用戶";
            btnDeleteUser.UseVisualStyleBackColor = true;
            // 
            // btnRefreshUser
            // 
            btnRefreshUser.Location = new Point(586, 8);
            btnRefreshUser.Margin = new Padding(3, 2, 3, 2);
            btnRefreshUser.Name = "btnRefreshUser";
            btnRefreshUser.Size = new Size(79, 25);
            btnRefreshUser.TabIndex = 6;
            btnRefreshUser.Text = "刷新資料";
            btnRefreshUser.UseVisualStyleBackColor = true;
            // 
            // panelUserPaging
            // 
            panelUserPaging.Controls.Add(btnUserPrev);
            panelUserPaging.Controls.Add(lblUserPage);
            panelUserPaging.Controls.Add(btnUserNext);
            panelUserPaging.Dock = DockStyle.Bottom;
            panelUserPaging.Location = new Point(0, 738);
            panelUserPaging.Margin = new Padding(3, 2, 3, 2);
            panelUserPaging.Name = "panelUserPaging";
            panelUserPaging.Size = new Size(902, 30);
            panelUserPaging.TabIndex = 2;
            // 
            // btnUserPrev
            // 
            btnUserPrev.Location = new Point(9, 4);
            btnUserPrev.Margin = new Padding(3, 2, 3, 2);
            btnUserPrev.Name = "btnUserPrev";
            btnUserPrev.Size = new Size(70, 20);
            btnUserPrev.TabIndex = 0;
            btnUserPrev.Text = "上一頁";
            // 
            // lblUserPage
            // 
            lblUserPage.Location = new Point(88, 8);
            lblUserPage.Name = "lblUserPage";
            lblUserPage.Size = new Size(88, 15);
            lblUserPage.TabIndex = 1;
            lblUserPage.Text = "第 1 頁";
            lblUserPage.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // btnUserNext
            // 
            btnUserNext.Location = new Point(184, 4);
            btnUserNext.Margin = new Padding(3, 2, 3, 2);
            btnUserNext.Name = "btnUserNext";
            btnUserNext.Size = new Size(70, 20);
            btnUserNext.TabIndex = 2;
            btnUserNext.Text = "下一頁";
            // 
            // tabComic
            // 
            tabComic.Controls.Add(dgvComic);
            tabComic.Controls.Add(panelComicSearch);
            tabComic.Controls.Add(panelComicPaging);
            tabComic.Location = new Point(4, 24);
            tabComic.Margin = new Padding(3, 2, 3, 2);
            tabComic.Name = "tabComic";
            tabComic.Size = new Size(902, 768);
            tabComic.TabIndex = 1;
            tabComic.Text = "漫畫管理";
            // 
            // dgvComic
            // 
            dgvComic.AllowUserToAddRows = false;
            dgvComic.Dock = DockStyle.Fill;
            dgvComic.Location = new Point(0, 30);
            dgvComic.Margin = new Padding(3, 2, 3, 2);
            dgvComic.MultiSelect = false;
            dgvComic.Name = "dgvComic";
            dgvComic.ReadOnly = true;
            dgvComic.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvComic.Size = new Size(902, 708);
            dgvComic.TabIndex = 0;
            // 
            // panelComicSearch
            // 
            panelComicSearch.Controls.Add(txtComicKeyword);
            panelComicSearch.Controls.Add(cmbComicSearchType);
            panelComicSearch.Controls.Add(btnComicSearch);
            panelComicSearch.Controls.Add(btnAddComic);
            panelComicSearch.Controls.Add(btnEditComic);
            panelComicSearch.Controls.Add(btnDeleteComic);
            panelComicSearch.Controls.Add(btnRefreshComic);
            panelComicSearch.Dock = DockStyle.Top;
            panelComicSearch.Location = new Point(0, 0);
            panelComicSearch.Margin = new Padding(3, 2, 3, 2);
            panelComicSearch.Name = "panelComicSearch";
            panelComicSearch.Size = new Size(902, 35);
            panelComicSearch.TabIndex = 1;
            // 
            // txtComicKeyword
            // 
            txtComicKeyword.Location = new Point(9, 8);
            txtComicKeyword.Margin = new Padding(3, 2, 3, 2);
            txtComicKeyword.Name = "txtComicKeyword";
            txtComicKeyword.Size = new Size(158, 23);
            txtComicKeyword.TabIndex = 0;
            // 
            // cmbComicSearchType
            // 
            cmbComicSearchType.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbComicSearchType.Items.AddRange(new object[] { "書號", "書名" });
            cmbComicSearchType.Location = new Point(175, 8);
            cmbComicSearchType.Margin = new Padding(3, 2, 3, 2);
            cmbComicSearchType.Name = "cmbComicSearchType";
            cmbComicSearchType.Size = new Size(70, 23);
            cmbComicSearchType.TabIndex = 1;
            // 
            // btnComicSearch
            // 
            btnComicSearch.Location = new Point(254, 8);
            btnComicSearch.Margin = new Padding(3, 2, 3, 2);
            btnComicSearch.Name = "btnComicSearch";
            btnComicSearch.Size = new Size(61, 25);
            btnComicSearch.TabIndex = 2;
            btnComicSearch.Text = "搜尋";
            // 
            // btnAddComic
            // 
            btnAddComic.Location = new Point(321, 8);
            btnAddComic.Margin = new Padding(3, 2, 3, 2);
            btnAddComic.Name = "btnAddComic";
            btnAddComic.Size = new Size(79, 25);
            btnAddComic.TabIndex = 3;
            btnAddComic.Text = "新增漫畫";
            // 
            // btnEditComic
            // 
            btnEditComic.Location = new Point(406, 8);
            btnEditComic.Margin = new Padding(3, 2, 3, 2);
            btnEditComic.Name = "btnEditComic";
            btnEditComic.Size = new Size(79, 25);
            btnEditComic.TabIndex = 4;
            btnEditComic.Text = "修改漫畫";
            // 
            // btnDeleteComic
            // 
            btnDeleteComic.Location = new Point(491, 8);
            btnDeleteComic.Margin = new Padding(3, 2, 3, 2);
            btnDeleteComic.Name = "btnDeleteComic";
            btnDeleteComic.Size = new Size(79, 25);
            btnDeleteComic.TabIndex = 5;
            btnDeleteComic.Text = "刪除漫畫";
            // 
            // btnRefreshComic
            // 
            btnRefreshComic.Location = new Point(576, 8);
            btnRefreshComic.Margin = new Padding(3, 2, 3, 2);
            btnRefreshComic.Name = "btnRefreshComic";
            btnRefreshComic.Size = new Size(79, 25);
            btnRefreshComic.TabIndex = 6;
            btnRefreshComic.Text = "刷新資料";
            // 
            // panelComicPaging
            // 
            panelComicPaging.Controls.Add(btnComicPrev);
            panelComicPaging.Controls.Add(lblComicPage);
            panelComicPaging.Controls.Add(btnComicNext);
            panelComicPaging.Dock = DockStyle.Bottom;
            panelComicPaging.Location = new Point(0, 738);
            panelComicPaging.Margin = new Padding(3, 2, 3, 2);
            panelComicPaging.Name = "panelComicPaging";
            panelComicPaging.Size = new Size(902, 30);
            panelComicPaging.TabIndex = 2;
            // 
            // btnComicPrev
            // 
            btnComicPrev.Location = new Point(9, 4);
            btnComicPrev.Margin = new Padding(3, 2, 3, 2);
            btnComicPrev.Name = "btnComicPrev";
            btnComicPrev.Size = new Size(70, 20);
            btnComicPrev.TabIndex = 0;
            btnComicPrev.Text = "上一頁";
            // 
            // lblComicPage
            // 
            lblComicPage.Location = new Point(88, 8);
            lblComicPage.Name = "lblComicPage";
            lblComicPage.Size = new Size(88, 15);
            lblComicPage.TabIndex = 1;
            lblComicPage.Text = "第 1 頁";
            lblComicPage.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // btnComicNext
            // 
            btnComicNext.Location = new Point(184, 4);
            btnComicNext.Margin = new Padding(3, 2, 3, 2);
            btnComicNext.Name = "btnComicNext";
            btnComicNext.Size = new Size(70, 20);
            btnComicNext.TabIndex = 2;
            btnComicNext.Text = "下一頁";
            // 
            // tabBorrow
            // 
            tabBorrow.Controls.Add(dgvBorrow);
            tabBorrow.Controls.Add(panelBorrowSearch);
            tabBorrow.Controls.Add(panelBorrowPaging);
            tabBorrow.Location = new Point(4, 24);
            tabBorrow.Margin = new Padding(3, 2, 3, 2);
            tabBorrow.Name = "tabBorrow";
            tabBorrow.Size = new Size(902, 768);
            tabBorrow.TabIndex = 2;
            tabBorrow.Text = "借閱紀錄查詢";
            // 
            // panelBorrowPaging
            // 
            panelBorrowPaging.Controls.Add(btnBorrowPrev);
            panelBorrowPaging.Controls.Add(lblBorrowPage);
            panelBorrowPaging.Controls.Add(btnBorrowNext);
            panelBorrowPaging.Dock = DockStyle.Bottom;
            panelBorrowPaging.Location = new Point(0, 738);
            panelBorrowPaging.Margin = new Padding(3, 2, 3, 2);
            panelBorrowPaging.Name = "panelBorrowPaging";
            panelBorrowPaging.Size = new Size(902, 30);
            panelBorrowPaging.TabIndex = 0;
            // 
            // btnBorrowPrev
            // 
            btnBorrowPrev.Location = new Point(9, 4);
            btnBorrowPrev.Margin = new Padding(3, 2, 3, 2);
            btnBorrowPrev.Name = "btnBorrowPrev";
            btnBorrowPrev.Size = new Size(70, 20);
            btnBorrowPrev.TabIndex = 0;
            btnBorrowPrev.Text = "上一頁";
            // 
            // lblBorrowPage
            // 
            lblBorrowPage.Location = new Point(88, 8);
            lblBorrowPage.Name = "lblBorrowPage";
            lblBorrowPage.Size = new Size(88, 15);
            lblBorrowPage.TabIndex = 1;
            lblBorrowPage.Text = "第 1 頁";
            lblBorrowPage.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // btnBorrowNext
            // 
            btnBorrowNext.Location = new Point(184, 4);
            btnBorrowNext.Margin = new Padding(3, 2, 3, 2);
            btnBorrowNext.Name = "btnBorrowNext";
            btnBorrowNext.Size = new Size(70, 20);
            btnBorrowNext.TabIndex = 2;
            btnBorrowNext.Text = "下一頁";
            // 
            // dgvBorrow
            // 
            dgvBorrow.AllowUserToAddRows = false;
            dgvBorrow.Dock = DockStyle.Fill;
            dgvBorrow.Location = new Point(0, 30);
            dgvBorrow.Margin = new Padding(3, 2, 3, 2);
            dgvBorrow.MultiSelect = false;
            dgvBorrow.Name = "dgvBorrow";
            dgvBorrow.ReadOnly = true;
            dgvBorrow.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvBorrow.Size = new Size(902, 708);
            dgvBorrow.TabIndex = 1;
            // 
            // panelBorrowSearch
            // 
            panelBorrowSearch.Controls.Add(txtBorrowKeyword);
            panelBorrowSearch.Controls.Add(cmbBorrowSearchType);
            panelBorrowSearch.Controls.Add(btnBorrowSearch);
            panelBorrowSearch.Controls.Add(btnRefreshBorrow);
            panelBorrowSearch.Dock = DockStyle.Top;
            panelBorrowSearch.Location = new Point(0, 0);
            panelBorrowSearch.Margin = new Padding(3, 2, 3, 2);
            panelBorrowSearch.Name = "panelBorrowSearch";
            panelBorrowSearch.Size = new Size(902, 35);
            panelBorrowSearch.TabIndex = 2;
            // 
            // txtBorrowKeyword
            // 
            txtBorrowKeyword.Location = new Point(9, 8);
            txtBorrowKeyword.Margin = new Padding(3, 2, 3, 2);
            txtBorrowKeyword.Name = "txtBorrowKeyword";
            txtBorrowKeyword.Size = new Size(158, 23);
            txtBorrowKeyword.TabIndex = 0;
            // 
            // cmbBorrowSearchType
            // 
            cmbBorrowSearchType.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbBorrowSearchType.Items.AddRange(new object[] { "用戶", "書名", "ISBN" });
            cmbBorrowSearchType.Location = new Point(175, 8);
            cmbBorrowSearchType.Margin = new Padding(3, 2, 3, 2);
            cmbBorrowSearchType.Name = "cmbBorrowSearchType";
            cmbBorrowSearchType.Size = new Size(70, 23);
            cmbBorrowSearchType.TabIndex = 1;
            // 
            // btnBorrowSearch
            // 
            btnBorrowSearch.Location = new Point(254, 8);
            btnBorrowSearch.Margin = new Padding(3, 2, 3, 2);
            btnBorrowSearch.Name = "btnBorrowSearch";
            btnBorrowSearch.Size = new Size(61, 25);
            btnBorrowSearch.TabIndex = 2;
            btnBorrowSearch.Text = "搜尋";
            // 
            // btnRefreshBorrow
            // 
            btnRefreshBorrow.Location = new Point(321, 8);
            btnRefreshBorrow.Margin = new Padding(3, 2, 3, 2);
            btnRefreshBorrow.Name = "btnRefreshBorrow";
            btnRefreshBorrow.Size = new Size(79, 25);
            btnRefreshBorrow.TabIndex = 3;
            btnRefreshBorrow.Text = "刷新資料";
            // 
            // tabReserve
            // 
            tabReserve.Controls.Add(dgvReserve);
            tabReserve.Controls.Add(panelReserveSearch);
            tabReserve.Controls.Add(panelReservePaging);
            tabReserve.Location = new Point(4, 24);
            tabReserve.Margin = new Padding(3, 2, 3, 2);
            tabReserve.Name = "tabReserve";
            tabReserve.Size = new Size(902, 768);
            tabReserve.TabIndex = 3;
            tabReserve.Text = "預約紀錄查詢";
            // 
            // panelReservePaging
            // 
            panelReservePaging.Controls.Add(btnReservePrev);
            panelReservePaging.Controls.Add(lblReservePage);
            panelReservePaging.Controls.Add(btnReserveNext);
            panelReservePaging.Dock = DockStyle.Bottom;
            panelReservePaging.Location = new Point(0, 738);
            panelReservePaging.Margin = new Padding(3, 2, 3, 2);
            panelReservePaging.Name = "panelReservePaging";
            panelReservePaging.Size = new Size(902, 30);
            panelReservePaging.TabIndex = 0;
            // 
            // btnReservePrev
            // 
            btnReservePrev.Location = new Point(9, 4);
            btnReservePrev.Margin = new Padding(3, 2, 3, 2);
            btnReservePrev.Name = "btnReservePrev";
            btnReservePrev.Size = new Size(70, 20);
            btnReservePrev.TabIndex = 0;
            btnReservePrev.Text = "上一頁";
            // 
            // lblReservePage
            // 
            lblReservePage.Location = new Point(88, 8);
            lblReservePage.Name = "lblReservePage";
            lblReservePage.Size = new Size(88, 15);
            lblReservePage.TabIndex = 1;
            lblReservePage.Text = "第 1 頁";
            lblReservePage.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // btnReserveNext
            // 
            btnReserveNext.Location = new Point(184, 4);
            btnReserveNext.Margin = new Padding(3, 2, 3, 2);
            btnReserveNext.Name = "btnReserveNext";
            btnReserveNext.Size = new Size(70, 20);
            btnReserveNext.TabIndex = 2;
            btnReserveNext.Text = "下一頁";
            // 
            // dgvReserve
            // 
            dgvReserve.AllowUserToAddRows = false;
            dgvReserve.Dock = DockStyle.Fill;
            dgvReserve.Location = new Point(0, 30);
            dgvReserve.Margin = new Padding(3, 2, 3, 2);
            dgvReserve.MultiSelect = false;
            dgvReserve.Name = "dgvReserve";
            dgvReserve.ReadOnly = true;
            dgvReserve.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvReserve.Size = new Size(902, 708);
            dgvReserve.TabIndex = 1;
            // 
            // panelReserveSearch
            // 
            panelReserveSearch.Controls.Add(txtReserveKeyword);
            panelReserveSearch.Controls.Add(cmbReserveSearchType);
            panelReserveSearch.Controls.Add(btnReserveSearch);
            panelReserveSearch.Controls.Add(btnRefreshReserve);
            panelReserveSearch.Dock = DockStyle.Top;
            panelReserveSearch.Location = new Point(0, 0);
            panelReserveSearch.Margin = new Padding(3, 2, 3, 2);
            panelReserveSearch.Name = "panelReserveSearch";
            panelReserveSearch.Size = new Size(902, 35);
            panelReserveSearch.TabIndex = 2;
            // 
            // txtReserveKeyword
            // 
            txtReserveKeyword.Location = new Point(9, 6);
            txtReserveKeyword.Margin = new Padding(3, 2, 3, 2);
            txtReserveKeyword.Name = "txtReserveKeyword";
            txtReserveKeyword.Size = new Size(158, 23);
            txtReserveKeyword.TabIndex = 0;
            // 
            // cmbReserveSearchType
            // 
            cmbReserveSearchType.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbReserveSearchType.Items.AddRange(new object[] { "用戶", "書名", "ISBN" });
            cmbReserveSearchType.Location = new Point(175, 6);
            cmbReserveSearchType.Margin = new Padding(3, 2, 3, 2);
            cmbReserveSearchType.Name = "cmbReserveSearchType";
            cmbReserveSearchType.Size = new Size(70, 23);
            cmbReserveSearchType.TabIndex = 1;
            // 
            // btnReserveSearch
            // 
            btnReserveSearch.Location = new Point(254, 8);
            btnReserveSearch.Margin = new Padding(3, 2, 3, 2);
            btnReserveSearch.Name = "btnReserveSearch";
            btnReserveSearch.Size = new Size(61, 25);
            btnReserveSearch.TabIndex = 2;
            btnReserveSearch.Text = "搜尋";
            // 
            // btnRefreshReserve
            // 
            btnRefreshReserve.Location = new Point(324, 8);
            btnRefreshReserve.Margin = new Padding(3, 2, 3, 2);
            btnRefreshReserve.Name = "btnRefreshReserve";
            btnRefreshReserve.Size = new Size(79, 25);
            btnRefreshReserve.TabIndex = 3;
            btnRefreshReserve.Text = "刷新資料";
            // 
            // tabLog
            // 
            tabLog.Controls.Add(dgvLog);
            tabLog.Controls.Add(panelLogPaging);
            tabLog.Controls.Add(panelLogSearch);
            tabLog.Location = new Point(4, 24);
            tabLog.Margin = new Padding(3, 2, 3, 2);
            tabLog.Name = "tabLog";
            tabLog.Size = new Size(902, 768);
            tabLog.TabIndex = 4;
            tabLog.Text = "管理日誌";
            // 
            // dgvLog
            // 
            dgvLog.AllowUserToAddRows = false;
            dgvLog.Dock = DockStyle.Fill;
            dgvLog.Location = new Point(0, 30);
            dgvLog.Margin = new Padding(3, 2, 3, 2);
            dgvLog.MultiSelect = false;
            dgvLog.Name = "dgvLog";
            dgvLog.ReadOnly = true;
            dgvLog.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvLog.Size = new Size(902, 708);
            dgvLog.TabIndex = 0;
            // 
            // panelLogSearch
            // 
            panelLogSearch.Controls.Add(txtLogKeyword);
            panelLogSearch.Controls.Add(btnLogSearch);
            panelLogSearch.Controls.Add(btnRefreshLog);
            panelLogSearch.Dock = DockStyle.Top;
            panelLogSearch.Location = new Point(0, 0);
            panelLogSearch.Margin = new Padding(3, 2, 3, 2);
            panelLogSearch.Name = "panelLogSearch";
            panelLogSearch.Size = new Size(902, 35);
            panelLogSearch.TabIndex = 1;
            // 
            // txtLogKeyword
            // 
            txtLogKeyword.Location = new Point(9, 8);
            txtLogKeyword.Margin = new Padding(3, 2, 3, 2);
            txtLogKeyword.Name = "txtLogKeyword";
            txtLogKeyword.Size = new Size(158, 23);
            txtLogKeyword.TabIndex = 0;
            // 
            // btnLogSearch
            // 
            btnLogSearch.Location = new Point(175, 8);
            btnLogSearch.Margin = new Padding(3, 2, 3, 2);
            btnLogSearch.Name = "btnLogSearch";
            btnLogSearch.Size = new Size(61, 25);
            btnLogSearch.TabIndex = 1;
            btnLogSearch.Text = "搜尋";
            // 
            // btnRefreshLog
            // 
            btnRefreshLog.Location = new Point(245, 8);
            btnRefreshLog.Margin = new Padding(3, 2, 3, 2);
            btnRefreshLog.Name = "btnRefreshLog";
            btnRefreshLog.Size = new Size(79, 25);
            btnRefreshLog.TabIndex = 2;
            btnRefreshLog.Text = "刷新日誌";
            // 
            // panelLogPaging
            // 
            panelLogPaging.Controls.Add(btnLogPrev);
            panelLogPaging.Controls.Add(lblLogPage);
            panelLogPaging.Controls.Add(btnLogNext);
            panelLogPaging.Dock = DockStyle.Bottom;
            panelLogPaging.Location = new Point(0, 738);
            panelLogPaging.Margin = new Padding(3, 2, 3, 2);
            panelLogPaging.Name = "panelLogPaging";
            panelLogPaging.Size = new Size(902, 30);
            panelLogPaging.TabIndex = 2;
            // 
            // btnLogPrev
            // 
            btnLogPrev.Location = new Point(9, 4);
            btnLogPrev.Margin = new Padding(3, 2, 3, 2);
            btnLogPrev.Name = "btnLogPrev";
            btnLogPrev.Size = new Size(70, 20);
            btnLogPrev.TabIndex = 0;
            btnLogPrev.Text = "上一頁";
            // 
            // lblLogPage
            // 
            lblLogPage.Location = new Point(88, 8);
            lblLogPage.Name = "lblLogPage";
            lblLogPage.Size = new Size(88, 15);
            lblLogPage.TabIndex = 1;
            lblLogPage.Text = "第 1 頁";
            lblLogPage.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // btnLogNext
            // 
            btnLogNext.Location = new Point(184, 4);
            btnLogNext.Margin = new Padding(3, 2, 3, 2);
            btnLogNext.Name = "btnLogNext";
            btnLogNext.Size = new Size(70, 20);
            btnLogNext.TabIndex = 2;
            btnLogNext.Text = "下一頁";
            // 
            // AdminForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(910, 796);
            Controls.Add(tabAdmin);
            Margin = new Padding(3, 2, 3, 2);
            Name = "AdminForm";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "漫畫租書系統 - 管理員介面";
            tabAdmin.ResumeLayout(false);
            tabUser.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)dgvUser).EndInit();
            panelUserSearch.ResumeLayout(false);
            panelUserSearch.PerformLayout();
            panelUserPaging.ResumeLayout(false);
            tabComic.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)dgvComic).EndInit();
            panelComicSearch.ResumeLayout(false);
            panelComicSearch.PerformLayout();
            panelComicPaging.ResumeLayout(false);
            tabBorrow.ResumeLayout(false);
            panelBorrowPaging.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)dgvBorrow).EndInit();
            panelBorrowSearch.ResumeLayout(false);
            panelBorrowSearch.PerformLayout();
            tabReserve.ResumeLayout(false);
            panelReservePaging.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)dgvReserve).EndInit();
            panelReserveSearch.ResumeLayout(false);
            panelReserveSearch.PerformLayout();
            tabLog.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)dgvLog).EndInit();
            panelLogSearch.ResumeLayout(false);
            panelLogSearch.PerformLayout();
            panelLogPaging.ResumeLayout(false);
            ResumeLayout(false);
        }
        #endregion
        private System.Windows.Forms.TabControl tabAdmin;
        private System.Windows.Forms.TabPage tabUser;
        private System.Windows.Forms.TabPage tabComic;
        private System.Windows.Forms.TabPage tabBorrow;
        private System.Windows.Forms.TabPage tabReserve;
        private System.Windows.Forms.TabPage tabLog;
        private System.Windows.Forms.DataGridView dgvUser;
        private System.Windows.Forms.DataGridView dgvComic;
        private System.Windows.Forms.DataGridView dgvBorrow;
        private System.Windows.Forms.DataGridView dgvReserve;
        private System.Windows.Forms.DataGridView dgvLog;
        private System.Windows.Forms.Panel panelComicSearch;
        private System.Windows.Forms.ComboBox cmbComicSearchType;
        private System.Windows.Forms.TextBox txtComicKeyword;
        private System.Windows.Forms.Button btnComicSearch;
        private System.Windows.Forms.Button btnAddComic;
        private System.Windows.Forms.Button btnDeleteComic;
        private System.Windows.Forms.Button btnEditComic;
        private System.Windows.Forms.Panel panelUserSearch;
        private System.Windows.Forms.TextBox txtSearchUser;
        private System.Windows.Forms.ComboBox cmbUserSearchType;
        private System.Windows.Forms.Button btnUserSearch;
        private System.Windows.Forms.Button btnAddUser;
        private System.Windows.Forms.Button btnDeleteUser;
        private System.Windows.Forms.Button btnEditUser;
        private System.Windows.Forms.Panel panelBorrowSearch;
        private System.Windows.Forms.TextBox txtBorrowKeyword;
        private System.Windows.Forms.ComboBox cmbBorrowSearchType;
        private System.Windows.Forms.Button btnBorrowSearch;
        private System.Windows.Forms.Panel panelReserveSearch;
        private System.Windows.Forms.TextBox txtReserveKeyword;
        private System.Windows.Forms.ComboBox cmbReserveSearchType;
        private System.Windows.Forms.Button btnReserveSearch;
        private System.Windows.Forms.Button btnRefreshUser;
        private System.Windows.Forms.Button btnRefreshComic;
        private System.Windows.Forms.Button btnRefreshBorrow;
        private System.Windows.Forms.Button btnRefreshReserve;
        // 分頁控制元件（借閱紀錄）
        private System.Windows.Forms.Panel panelBorrowPaging;
        private System.Windows.Forms.Button btnBorrowPrev;
        private System.Windows.Forms.Button btnBorrowNext;
        private System.Windows.Forms.Label lblBorrowPage;
        // 分頁控制元件（預約紀錄）
        private System.Windows.Forms.Panel panelReservePaging;
        private System.Windows.Forms.Button btnReservePrev;
        private System.Windows.Forms.Button btnReserveNext;
        private System.Windows.Forms.Label lblReservePage;
        // 用戶管理分頁控制
        private System.Windows.Forms.Panel panelUserPaging;
        private System.Windows.Forms.Button btnUserPrev;
        private System.Windows.Forms.Button btnUserNext;
        private System.Windows.Forms.Label lblUserPage;
        // 漫畫管理分頁控制
        private System.Windows.Forms.Panel panelComicPaging;
        private System.Windows.Forms.Button btnComicPrev;
        private System.Windows.Forms.Button btnComicNext;
        private System.Windows.Forms.Label lblComicPage;
        // 管理日誌分頁控制
        private System.Windows.Forms.Panel panelLogPaging;
        private System.Windows.Forms.Button btnLogPrev;
        private System.Windows.Forms.Button btnLogNext;
        private System.Windows.Forms.Label lblLogPage;
        private System.Windows.Forms.Panel panelLogSearch;
        private System.Windows.Forms.TextBox txtLogKeyword;
        private System.Windows.Forms.Button btnLogSearch;
        private System.Windows.Forms.Button btnRefreshLog;
    }
}