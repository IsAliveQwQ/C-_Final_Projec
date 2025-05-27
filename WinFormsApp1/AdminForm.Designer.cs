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
            this.tabAdmin = new System.Windows.Forms.TabControl();
            this.tabUser = new System.Windows.Forms.TabPage();
            this.tabComic = new System.Windows.Forms.TabPage();
            this.tabBorrow = new System.Windows.Forms.TabPage();
            this.tabReserve = new System.Windows.Forms.TabPage();
            this.tabLog = new System.Windows.Forms.TabPage();
            this.dgvUser = new System.Windows.Forms.DataGridView();
            this.dgvComic = new System.Windows.Forms.DataGridView();
            this.dgvBorrow = new System.Windows.Forms.DataGridView();
            this.dgvReserve = new System.Windows.Forms.DataGridView();
            this.dgvLog = new System.Windows.Forms.DataGridView();
            this.panelComicSearch = new System.Windows.Forms.Panel();
            this.cmbComicSearchType = new System.Windows.Forms.ComboBox();
            this.txtComicKeyword = new System.Windows.Forms.TextBox();
            this.btnComicSearch = new System.Windows.Forms.Button();
            this.btnAddComic = new System.Windows.Forms.Button();
            this.btnDeleteComic = new System.Windows.Forms.Button();
            this.btnEditComic = new System.Windows.Forms.Button();
            this.panelUserSearch = new System.Windows.Forms.Panel();
            this.txtSearchUser = new System.Windows.Forms.TextBox();
            this.cmbUserSearchType = new System.Windows.Forms.ComboBox();
            this.btnUserSearch = new System.Windows.Forms.Button();
            this.btnAddUser = new System.Windows.Forms.Button();
            this.btnDeleteUser = new System.Windows.Forms.Button();
            this.btnEditUser = new System.Windows.Forms.Button();
            this.panelBorrowSearch = new System.Windows.Forms.Panel();
            this.txtBorrowKeyword = new System.Windows.Forms.TextBox();
            this.cmbBorrowSearchType = new System.Windows.Forms.ComboBox();
            this.btnBorrowSearch = new System.Windows.Forms.Button();
            this.panelReserveSearch = new System.Windows.Forms.Panel();
            this.txtReserveKeyword = new System.Windows.Forms.TextBox();
            this.cmbReserveSearchType = new System.Windows.Forms.ComboBox();
            this.btnReserveSearch = new System.Windows.Forms.Button();
            this.btnRefreshUser = new System.Windows.Forms.Button();
            this.btnRefreshComic = new System.Windows.Forms.Button();
            this.btnRefreshBorrow = new System.Windows.Forms.Button();
            this.btnRefreshReserve = new System.Windows.Forms.Button();
            // 分頁控制元件（借閱紀錄）
            this.panelBorrowPaging = new System.Windows.Forms.Panel();
            this.btnBorrowPrev = new System.Windows.Forms.Button();
            this.btnBorrowNext = new System.Windows.Forms.Button();
            this.lblBorrowPage = new System.Windows.Forms.Label();
            this.panelBorrowPaging.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panelBorrowPaging.Height = 40;
            this.btnBorrowPrev.Text = "上一頁";
            this.btnBorrowNext.Text = "下一頁";
            this.lblBorrowPage.Text = "第 1 頁";
            this.btnBorrowPrev.Location = new System.Drawing.Point(20, 8);
            this.btnBorrowNext.Location = new System.Drawing.Point(200, 8);
            this.lblBorrowPage.Location = new System.Drawing.Point(100, 10);
            this.panelBorrowPaging.Controls.Add(this.btnBorrowPrev);
            this.panelBorrowPaging.Controls.Add(this.lblBorrowPage);
            this.panelBorrowPaging.Controls.Add(this.btnBorrowNext);
            this.tabBorrow.Controls.Add(this.panelBorrowPaging);
            this.dgvBorrow.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            // 分頁控制元件（預約紀錄）
            this.panelReservePaging = new System.Windows.Forms.Panel();
            this.btnReservePrev = new System.Windows.Forms.Button();
            this.btnReserveNext = new System.Windows.Forms.Button();
            this.lblReservePage = new System.Windows.Forms.Label();
            this.panelReservePaging.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panelReservePaging.Height = 40;
            this.btnReservePrev.Text = "上一頁";
            this.btnReserveNext.Text = "下一頁";
            this.lblReservePage.Text = "第 1 頁";
            this.btnReservePrev.Location = new System.Drawing.Point(20, 8);
            this.btnReserveNext.Location = new System.Drawing.Point(200, 8);
            this.lblReservePage.Location = new System.Drawing.Point(100, 10);
            this.panelReservePaging.Controls.Add(this.btnReservePrev);
            this.panelReservePaging.Controls.Add(this.lblReservePage);
            this.panelReservePaging.Controls.Add(this.btnReserveNext);
            this.tabReserve.Controls.Add(this.panelReservePaging);
            this.dgvReserve.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            //
            // tabAdmin
            //
            this.tabAdmin.Controls.Add(this.tabUser);
            this.tabAdmin.Controls.Add(this.tabComic);
            this.tabAdmin.Controls.Add(this.tabBorrow);
            this.tabAdmin.Controls.Add(this.tabReserve);
            this.tabAdmin.Controls.Add(this.tabLog);
            this.tabAdmin.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabAdmin.Name = "tabAdmin";
            //
            // tabUser
            //
            this.tabUser.Text = "用戶管理";
            this.tabUser.Controls.Clear();
            this.tabUser.Controls.Add(this.dgvUser);
            this.tabUser.Controls.Add(this.panelUserSearch);
            // 用戶管理分頁控制
            this.panelUserPaging = new System.Windows.Forms.Panel();
            this.btnUserPrev = new System.Windows.Forms.Button();
            this.btnUserNext = new System.Windows.Forms.Button();
            this.lblUserPage = new System.Windows.Forms.Label();
            this.panelUserPaging.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panelUserPaging.Height = 40;
            this.btnUserPrev.Text = "上一頁";
            this.btnUserNext.Text = "下一頁";
            this.lblUserPage.Text = "第 1 頁";
            this.btnUserPrev.Location = new System.Drawing.Point(20, 8);
            this.btnUserNext.Location = new System.Drawing.Point(200, 8);
            this.lblUserPage.Location = new System.Drawing.Point(100, 10);
            this.panelUserPaging.Controls.Add(this.btnUserPrev);
            this.panelUserPaging.Controls.Add(this.lblUserPage);
            this.panelUserPaging.Controls.Add(this.btnUserNext);
            this.tabUser.Controls.Add(this.panelUserPaging);
            //
            // tabComic
            //
            this.tabComic.Text = "漫畫管理";
            this.tabComic.Controls.Clear();
            this.tabComic.Controls.Add(this.dgvComic);
            this.tabComic.Controls.Add(this.panelComicSearch);
            // 漫畫管理分頁控制
            this.panelComicPaging = new System.Windows.Forms.Panel();
            this.btnComicPrev = new System.Windows.Forms.Button();
            this.btnComicNext = new System.Windows.Forms.Button();
            this.lblComicPage = new System.Windows.Forms.Label();
            this.panelComicPaging.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panelComicPaging.Height = 40;
            this.btnComicPrev.Text = "上一頁";
            this.btnComicNext.Text = "下一頁";
            this.lblComicPage.Text = "第 1 頁";
            this.btnComicPrev.Location = new System.Drawing.Point(20, 8);
            this.btnComicNext.Location = new System.Drawing.Point(200, 8);
            this.lblComicPage.Location = new System.Drawing.Point(100, 10);
            this.panelComicPaging.Controls.Add(this.btnComicPrev);
            this.panelComicPaging.Controls.Add(this.lblComicPage);
            this.panelComicPaging.Controls.Add(this.btnComicNext);
            this.tabComic.Controls.Add(this.panelComicPaging);
            this.dgvComic.AllowUserToAddRows = false;
            this.dgvComic.ReadOnly = true;
            this.dgvComic.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvComic.MultiSelect = false;
            //
            // tabBorrow
            //
            this.tabBorrow.Text = "借閱紀錄查詢";
            this.tabBorrow.Controls.Clear();
            this.tabBorrow.Controls.Add(this.dgvBorrow);
            this.tabBorrow.Controls.Add(this.panelBorrowSearch);
            //
            // tabReserve
            //
            this.tabReserve.Text = "預約紀錄查詢";
            this.tabReserve.Controls.Clear();
            this.tabReserve.Controls.Add(this.dgvReserve);
            this.tabReserve.Controls.Add(this.panelReserveSearch);
            //
            // tabLog
            //
            this.tabLog.Text = "管理日誌";
            this.tabLog.Controls.Clear();
            this.tabLog.Controls.Add(this.dgvLog);
            this.tabLog.Controls.Add(this.panelLogSearch);
            //
            // dgvUser
            //
            this.dgvUser.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvUser.Name = "dgvUser";
            //
            // dgvComic
            //
            this.dgvComic.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvComic.Name = "dgvComic";
            this.dgvComic.AllowUserToAddRows = false;
            this.dgvComic.ReadOnly = true;
            this.dgvComic.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvComic.MultiSelect = false;
            //
            // dgvBorrow
            //
            this.dgvBorrow.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvBorrow.Name = "dgvBorrow";
            this.dgvBorrow.AllowUserToAddRows = false;
            this.dgvBorrow.ReadOnly = true;
            this.dgvBorrow.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvBorrow.MultiSelect = false;
            //
            // dgvReserve
            //
            this.dgvReserve.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvReserve.Name = "dgvReserve";
            this.dgvReserve.AllowUserToAddRows = false;
            this.dgvReserve.ReadOnly = true;
            this.dgvReserve.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvReserve.MultiSelect = false;
            //
            // dgvLog
            //
            this.dgvLog.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvLog.Name = "dgvLog";
            this.dgvLog.AllowUserToAddRows = false;
            this.dgvLog.ReadOnly = true;
            this.dgvLog.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvLog.MultiSelect = false;
            //
            // panelComicSearch
            //
            this.panelComicSearch.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelComicSearch.Height = 40;
            this.panelComicSearch.Controls.Add(this.txtComicKeyword);
            this.panelComicSearch.Controls.Add(this.cmbComicSearchType);
            this.panelComicSearch.Controls.Add(this.btnComicSearch);
            this.panelComicSearch.Controls.Add(this.btnAddComic);
            this.panelComicSearch.Controls.Add(this.btnEditComic);
            this.panelComicSearch.Controls.Add(this.btnDeleteComic);
            this.panelComicSearch.Controls.Add(this.btnRefreshComic);
            //
            // txtComicKeyword
            //
            this.txtComicKeyword.Location = new System.Drawing.Point(10, 6);
            this.txtComicKeyword.Size = new System.Drawing.Size(180, 27);
            //
            // cmbComicSearchType
            //
            this.cmbComicSearchType.Location = new System.Drawing.Point(200, 6);
            this.cmbComicSearchType.Size = new System.Drawing.Size(80, 27);
            this.cmbComicSearchType.Items.AddRange(new object[] { "書號", "書名" });
            this.cmbComicSearchType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbComicSearchType.SelectedIndex = 0;
            //
            // btnComicSearch
            //
            this.btnComicSearch.Location = new System.Drawing.Point(290, 6);
            this.btnComicSearch.Size = new System.Drawing.Size(70, 27);
            this.btnComicSearch.Text = "搜尋";
            //
            // btnAddComic
            //
            this.btnAddComic.Location = new System.Drawing.Point(380, 6);
            this.btnAddComic.Size = new System.Drawing.Size(90, 27);
            this.btnAddComic.Text = "新增漫畫";
            //
            // btnEditComic
            //
            this.btnEditComic.Location = new System.Drawing.Point(480, 6);
            this.btnEditComic.Size = new System.Drawing.Size(90, 27);
            this.btnEditComic.Text = "修改漫畫";
            //
            // btnDeleteComic
            //
            this.btnDeleteComic.Location = new System.Drawing.Point(580, 6);
            this.btnDeleteComic.Size = new System.Drawing.Size(90, 27);
            this.btnDeleteComic.Text = "刪除漫畫";
            //
            // panelUserSearch
            //
            this.panelUserSearch.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelUserSearch.Height = 40;
            this.panelUserSearch.Controls.Clear();
            this.panelUserSearch.Controls.Add(this.txtSearchUser);
            this.panelUserSearch.Controls.Add(this.cmbUserSearchType);
            this.panelUserSearch.Controls.Add(this.btnUserSearch);
            this.panelUserSearch.Controls.Add(this.btnAddUser);
            this.panelUserSearch.Controls.Add(this.btnEditUser);
            this.panelUserSearch.Controls.Add(this.btnDeleteUser);
            this.panelUserSearch.Controls.Add(this.btnRefreshUser);
            //
            // txtSearchUser
            //
            this.txtSearchUser.Location = new System.Drawing.Point(10, 8);
            this.txtSearchUser.Name = "txtSearchUser";
            this.txtSearchUser.Size = new System.Drawing.Size(180, 27);
            //
            // cmbUserSearchType
            //
            this.cmbUserSearchType.Location = new System.Drawing.Point(200, 8);
            this.cmbUserSearchType.Name = "cmbUserSearchType";
            this.cmbUserSearchType.Size = new System.Drawing.Size(80, 27);
            this.cmbUserSearchType.Items.AddRange(new object[] { "用戶ID", "用戶名" });
            this.cmbUserSearchType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbUserSearchType.SelectedIndex = 1;
            //
            // btnUserSearch
            //
            this.btnUserSearch.Location = new System.Drawing.Point(290, 8);
            this.btnUserSearch.Name = "btnUserSearch";
            this.btnUserSearch.Size = new System.Drawing.Size(70, 27);
            this.btnUserSearch.Text = "搜尋";
            this.btnUserSearch.UseVisualStyleBackColor = true;
            //
            // btnAddUser
            //
            this.btnAddUser.Location = new System.Drawing.Point(370, 8);
            this.btnAddUser.Name = "btnAddUser";
            this.btnAddUser.Size = new System.Drawing.Size(90, 27);
            this.btnAddUser.Text = "新增用戶";
            this.btnAddUser.UseVisualStyleBackColor = true;
            //
            // btnEditUser
            //
            this.btnEditUser.Location = new System.Drawing.Point(470, 8);
            this.btnEditUser.Name = "btnEditUser";
            this.btnEditUser.Size = new System.Drawing.Size(90, 27);
            this.btnEditUser.Text = "編輯用戶";
            this.btnEditUser.UseVisualStyleBackColor = true;
            //
            // btnDeleteUser
            //
            this.btnDeleteUser.Location = new System.Drawing.Point(570, 8);
            this.btnDeleteUser.Name = "btnDeleteUser";
            this.btnDeleteUser.Size = new System.Drawing.Size(90, 27);
            this.btnDeleteUser.Text = "刪除用戶";
            this.btnDeleteUser.UseVisualStyleBackColor = true;
            //
            // btnRefreshUser
            //
            this.btnRefreshUser.Location = new System.Drawing.Point(670, 8);
            this.btnRefreshUser.Name = "btnRefreshUser";
            this.btnRefreshUser.Size = new System.Drawing.Size(90, 27);
            this.btnRefreshUser.Text = "刷新資料";
            this.btnRefreshUser.UseVisualStyleBackColor = true;
            //
            // panelBorrowSearch
            //
            this.panelBorrowSearch.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelBorrowSearch.Height = 40;
            this.panelBorrowSearch.Controls.Clear();
            this.panelBorrowSearch.Controls.Add(this.txtBorrowKeyword);
            this.panelBorrowSearch.Controls.Add(this.cmbBorrowSearchType);
            this.panelBorrowSearch.Controls.Add(this.btnBorrowSearch);
            this.panelBorrowSearch.Controls.Add(this.btnRefreshBorrow);
            //
            // txtBorrowKeyword
            //
            this.txtBorrowKeyword.Location = new System.Drawing.Point(10, 6);
            this.txtBorrowKeyword.Size = new System.Drawing.Size(180, 27);
            //
            // cmbBorrowSearchType
            //
            this.cmbBorrowSearchType.Location = new System.Drawing.Point(200, 6);
            this.cmbBorrowSearchType.Size = new System.Drawing.Size(80, 27);
            this.cmbBorrowSearchType.Items.AddRange(new object[] { "用戶", "書名", "ISBN" });
            this.cmbBorrowSearchType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbBorrowSearchType.SelectedIndex = 0;
            //
            // btnBorrowSearch
            //
            this.btnBorrowSearch.Location = new System.Drawing.Point(290, 6);
            this.btnBorrowSearch.Size = new System.Drawing.Size(70, 27);
            this.btnBorrowSearch.Text = "搜尋";
            //
            // panelReserveSearch
            //
            this.panelReserveSearch.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelReserveSearch.Height = 40;
            this.panelReserveSearch.Controls.Clear();
            this.panelReserveSearch.Controls.Add(this.txtReserveKeyword);
            this.panelReserveSearch.Controls.Add(this.cmbReserveSearchType);
            this.panelReserveSearch.Controls.Add(this.btnReserveSearch);
            this.panelReserveSearch.Controls.Add(this.btnRefreshReserve);
            //
            // txtReserveKeyword
            //
            this.txtReserveKeyword.Location = new System.Drawing.Point(10, 6);
            this.txtReserveKeyword.Size = new System.Drawing.Size(180, 27);
            //
            // cmbReserveSearchType
            //
            this.cmbReserveSearchType.Location = new System.Drawing.Point(200, 6);
            this.cmbReserveSearchType.Size = new System.Drawing.Size(80, 27);
            this.cmbReserveSearchType.Items.AddRange(new object[] { "用戶", "書名", "ISBN" });
            this.cmbReserveSearchType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbReserveSearchType.SelectedIndex = 0;
            //
            // btnReserveSearch
            //
            this.btnReserveSearch.Location = new System.Drawing.Point(290, 6);
            this.btnReserveSearch.Size = new System.Drawing.Size(70, 27);
            this.btnReserveSearch.Text = "搜尋";
            //
            // btnRefreshComic
            //
            this.btnRefreshComic.Location = new System.Drawing.Point(680, 6);
            this.btnRefreshComic.Size = new System.Drawing.Size(90, 27);
            this.btnRefreshComic.Text = "刷新資料";
            //
            // btnRefreshBorrow
            //
            this.btnRefreshBorrow.Location = new System.Drawing.Point(370, 6);
            this.btnRefreshBorrow.Size = new System.Drawing.Size(90, 27);
            this.btnRefreshBorrow.Text = "刷新資料";
            //
            // btnRefreshReserve
            //
            this.btnRefreshReserve.Location = new System.Drawing.Point(370, 6);
            this.btnRefreshReserve.Size = new System.Drawing.Size(90, 27);
            this.btnRefreshReserve.Text = "刷新資料";
            //
            // panelLogSearch
            //
            this.panelLogSearch = new System.Windows.Forms.Panel();
            this.txtLogKeyword = new System.Windows.Forms.TextBox();
            this.cmbLogSearchType = new System.Windows.Forms.ComboBox();
            this.btnLogSearch = new System.Windows.Forms.Button();
            this.btnRefreshLog = new System.Windows.Forms.Button();
            this.panelLogSearch.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelLogSearch.Height = 40;
            this.panelLogSearch.Controls.Clear();
            this.panelLogSearch.Controls.Add(this.txtLogKeyword);
            this.panelLogSearch.Controls.Add(this.cmbLogSearchType);
            this.panelLogSearch.Controls.Add(this.btnLogSearch);
            this.panelLogSearch.Controls.Add(this.btnRefreshLog);
            //
            // txtLogKeyword
            //
            this.txtLogKeyword.Location = new System.Drawing.Point(10, 8);
            this.txtLogKeyword.Name = "txtLogKeyword";
            this.txtLogKeyword.Size = new System.Drawing.Size(180, 27);
            //
            // cmbLogSearchType
            //
            this.cmbLogSearchType.Location = new System.Drawing.Point(200, 8);
            this.cmbLogSearchType.Name = "cmbLogSearchType";
            this.cmbLogSearchType.Size = new System.Drawing.Size(120, 27);
            this.cmbLogSearchType.Items.AddRange(new object[] { "所有操作", "新增用戶", "編輯用戶", "刪除用戶", "新增漫畫", "編輯漫畫", "刪除漫畫", "歸還漫畫", "取消預約" });
            this.cmbLogSearchType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbLogSearchType.SelectedIndex = 0;
            //
            // btnLogSearch
            //
            this.btnLogSearch.Location = new System.Drawing.Point(330, 8);
            this.btnLogSearch.Name = "btnLogSearch";
            this.btnLogSearch.Size = new System.Drawing.Size(70, 27);
            this.btnLogSearch.Text = "搜尋";
            this.btnLogSearch.UseVisualStyleBackColor = true;
            //
            // btnRefreshLog
            //
            this.btnRefreshLog.Location = new System.Drawing.Point(410, 8);
            this.btnRefreshLog.Name = "btnRefreshLog";
            this.btnRefreshLog.Size = new System.Drawing.Size(90, 27);
            this.btnRefreshLog.Text = "刷新日誌";
            this.btnRefreshLog.UseVisualStyleBackColor = true;
            //
            // AdminForm
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1040, (int)(Screen.PrimaryScreen.Bounds.Height * 0.6));
            this.Controls.Add(this.tabAdmin);
            this.Name = "AdminForm";
            this.Text = "漫畫租書系統 - 管理員介面";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
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
        private System.Windows.Forms.Panel panelLogSearch;
        private System.Windows.Forms.TextBox txtLogKeyword;
        private System.Windows.Forms.ComboBox cmbLogSearchType;
        private System.Windows.Forms.Button btnLogSearch;
        private System.Windows.Forms.Button btnRefreshLog;
    }
} 