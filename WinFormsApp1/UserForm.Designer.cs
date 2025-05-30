namespace WinFormsApp1
{
    partial class UserForm
    {
        /// <summary>
        ///  Required designer variable.
        ///  必要的設計器變數。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        // 收藏紀錄分頁相關欄位
        private System.Windows.Forms.TabPage tabPageFavorite;
        private System.Windows.Forms.DataGridView dgvFavoriteRecord;
        private System.Windows.Forms.Button btnFavoritePrev, btnFavoriteNext;
        private System.Windows.Forms.Label lblFavoritePage;
        private int currentFavoritePage = 1;
        private const int FavoritePageSize = 10;

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
            this.panelUserContent = new System.Windows.Forms.Panel();
            this.panelComicActions = new System.Windows.Forms.Panel();
            this.btnReserve = new System.Windows.Forms.Button();
            this.btnRent = new System.Windows.Forms.Button();
            this.lblStatus = new System.Windows.Forms.Label();
            this.lblAuthor = new System.Windows.Forms.Label();
            this.lblTitle = new System.Windows.Forms.Label();
            this.dgvComics = new System.Windows.Forms.DataGridView();
            this.btnSearch = new System.Windows.Forms.Button();
            this.txtSearch = new System.Windows.Forms.TextBox();
            this.labelSearch = new System.Windows.Forms.Label();
            this.btnLoginLogout = new System.Windows.Forms.Button();
            this.lblUsername = new System.Windows.Forms.Label();
            this.btnAdmin = new System.Windows.Forms.Button();
            this.panelHeader = new System.Windows.Forms.Panel();
            this.panelDetail = new System.Windows.Forms.Panel();
            this.lblDetailTitle = new System.Windows.Forms.Label();
            this.lblDetailAuthor = new System.Windows.Forms.Label();
            this.lblDetailCategory = new System.Windows.Forms.Label();
            this.lblDetailStatus = new System.Windows.Forms.Label();
            this.lblDetailReserve = new System.Windows.Forms.Label();
            this.btnBigRent = new System.Windows.Forms.Button();
            this.btnBigReserve = new System.Windows.Forms.Button();
            this.cmbSearchType = new System.Windows.Forms.ComboBox();
            this.dgvBorrowRecord = new System.Windows.Forms.DataGridView();
            this.btnBorrowPrev = new System.Windows.Forms.Button();
            this.btnBorrowNext = new System.Windows.Forms.Button();
            this.lblBorrowPage = new System.Windows.Forms.Label();
            this.dgvReserveRecord = new System.Windows.Forms.DataGridView();
            this.btnReservePrev = new System.Windows.Forms.Button();
            this.btnReserveNext = new System.Windows.Forms.Button();
            this.lblReservePage = new System.Windows.Forms.Label();
            this.lblDetailISBN = new System.Windows.Forms.Label();
            this.lblDetailPublisher = new System.Windows.Forms.Label();
            this.panelUserContent.SuspendLayout();
            this.panelComicActions.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvComics)).BeginInit();
            this.panelHeader.SuspendLayout();
            this.panelDetail.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvBorrowRecord)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvReserveRecord)).BeginInit();
            this.SuspendLayout();
            // 
            // panelUserContent
            // 
            // 使用者內容面板，包含搜尋、列表和操作區域
            this.panelUserContent.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelUserContent.Padding = new System.Windows.Forms.Padding(0);
            this.panelUserContent.Margin = new System.Windows.Forms.Padding(0);
            this.panelUserContent.Controls.Clear();
            this.dgvComics.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right))); // 停靠設定，使其隨視窗大小變化
            this.dgvComics.AllowUserToAddRows = false;
            this.dgvComics.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvComics.Location = new System.Drawing.Point(0, 0); // 相對於 panelUserContent 的位置
            this.dgvComics.Name = "dgvComics";
            this.dgvComics.RowTemplate.Height = 48;
            this.dgvComics.RowHeadersWidth = 60;
            this.dgvComics.Size = new System.Drawing.Size(800, 300); // 調整大小，為下方操作面板留空間
            this.dgvComics.TabIndex = 0;
            this.dgvComics.Font = new System.Drawing.Font("Microsoft JhengHei UI", 20F);
            this.dgvComics.ColumnHeadersDefaultCellStyle.Font = new System.Drawing.Font("Microsoft JhengHei UI", 20F, System.Drawing.FontStyle.Bold);
            this.dgvComics.Dock = System.Windows.Forms.DockStyle.Fill;

            // 設定 DataGridView 基本樣式
            this.dgvComics.Font = new System.Drawing.Font("Microsoft JhengHei UI", 10.5F); // 設定字體
            this.dgvComics.ColumnHeadersDefaultCellStyle.Font = new System.Drawing.Font("Microsoft JhengHei UI", 10.5F, System.Drawing.FontStyle.Bold); // 設定標題字體
            this.dgvComics.EnableHeadersVisualStyles = false; // 禁用視覺樣式以設定背景色
            this.dgvComics.ColumnHeadersDefaultCellStyle.BackColor = System.Drawing.Color.WhiteSmoke; // 設定標題背景色為灰色
            
            // 禁用不必要的用戶交互
            this.dgvComics.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill; // 讓欄位自動填滿
            this.dgvComics.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.None; // 禁用行高自動調整
            this.dgvComics.AllowUserToResizeRows = false; // 禁用用戶調整行高
            this.dgvComics.AllowUserToResizeColumns = true; // 允許用戶調整列寬
            this.dgvComics.SelectionMode = DataGridViewSelectionMode.FullRowSelect; // 整行選取
            this.dgvComics.MultiSelect = false; // 禁用多重選取
            this.dgvComics.ReadOnly = true; // 設定為只讀
            this.dgvComics.AllowUserToDeleteRows = false; // 禁用刪除行
            this.dgvComics.AllowUserToOrderColumns = false; // 禁用重新排序欄位

            // 其他屬性
            this.dgvComics.StandardTab = true;
            this.dgvComics.TabStop = true;
            this.dgvComics.BackgroundColor = System.Drawing.Color.White; // 設定背景色

            this.panelUserContent.Controls.Add(this.dgvComics);
            this.panelUserContent.Location = new System.Drawing.Point(0, 50); // 調整位置，在 headerPanel 下方
            this.panelUserContent.Name = "panelUserContent";
            this.panelUserContent.Size = new System.Drawing.Size(800, 400); // 調整大小
            this.panelUserContent.TabIndex = 0;
            // 
            // panelComicActions
            // 
            // 漫畫操作面板，顯示詳細資訊和按鈕
            this.panelComicActions.Controls.Add(this.btnReserve);
            this.panelComicActions.Controls.Add(this.btnRent);
            this.panelComicActions.Controls.Add(this.lblStatus);
            this.panelComicActions.Controls.Add(this.lblAuthor);
            this.panelComicActions.Controls.Add(this.lblTitle);
            this.panelComicActions.Dock = System.Windows.Forms.DockStyle.Bottom; // 停靠在底部
            this.panelComicActions.Location = new System.Drawing.Point(0, 350); // 初始位置，Dock 設置後會自動調整
            this.panelComicActions.Name = "panelComicActions";
            this.panelComicActions.Size = new System.Drawing.Size(800, 100); // 設定面板高度
            this.panelComicActions.TabIndex = 1;
            // 
            // btnReserve
            // 
            // 預約按鈕
            this.btnReserve.Location = new System.Drawing.Point(105, 94);
            this.btnReserve.Name = "btnReserve";
            this.btnReserve.Size = new System.Drawing.Size(100, 40);
            this.btnReserve.TabIndex = 4;
            this.btnReserve.Text = "預約";
            this.btnReserve.UseVisualStyleBackColor = true;
            // 
            // btnRent
            // 
            // 借閱按鈕
            this.btnRent.Location = new System.Drawing.Point(15, 94);
            this.btnRent.Name = "btnRent";
            this.btnRent.Size = new System.Drawing.Size(100, 40);
            this.btnRent.TabIndex = 3;
            this.btnRent.Text = "借閱";
            this.btnRent.UseVisualStyleBackColor = true;
            // 
            // lblStatus
            // 
            // 狀態標籤
            this.lblStatus.AutoSize = true;
            this.lblStatus.Location = new System.Drawing.Point(20, 70); // 調整位置
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(43, 15);
            this.lblStatus.TabIndex = 2;
            this.lblStatus.Text = "狀態：";
            // 
            // lblAuthor
            // 
            // 作者標籤
            this.lblAuthor.AutoSize = true;
            this.lblAuthor.Location = new System.Drawing.Point(20, 45); // 調整位置
            this.lblAuthor.Name = "lblAuthor";
            this.lblAuthor.Size = new System.Drawing.Size(43, 15);
            this.lblAuthor.TabIndex = 1;
            this.lblAuthor.Text = "作者：";
            // 
            // lblTitle
            // 
            // 標題標籤
            this.lblTitle.AutoSize = true;
            this.lblTitle.Location = new System.Drawing.Point(20, 20); // 調整位置
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(43, 15);
            this.lblTitle.TabIndex = 0;
            this.lblTitle.Text = "標題：";
            // 
            // btnSearch
            // 
            // 搜尋按鈕
            this.btnSearch.Location = new System.Drawing.Point(590, 32);
            this.btnSearch.Name = "btnSearch";
            this.btnSearch.Size = new System.Drawing.Size(120, 48);
            this.btnSearch.TabIndex = 1;
            this.btnSearch.Text = "搜尋";
            this.btnSearch.UseVisualStyleBackColor = true;
            this.btnSearch.Font = new System.Drawing.Font("Microsoft JhengHei UI", 20F);
            // 
            // txtSearch
            // 
            // 搜尋文字框
            this.txtSearch.Location = new System.Drawing.Point(120, 32);
            this.txtSearch.Name = "txtSearch";
            this.txtSearch.Size = new System.Drawing.Size(300, 48);
            this.txtSearch.TabIndex = 2;
            this.txtSearch.Font = new System.Drawing.Font("Microsoft JhengHei UI", 20F);
            // 
            // labelSearch
            // 
            // 搜尋標籤
            this.labelSearch.AutoSize = true;
            this.labelSearch.Location = new System.Drawing.Point(20, 35);
            this.labelSearch.Text = "關鍵字：";
            this.labelSearch.Font = new System.Drawing.Font("Microsoft JhengHei UI", 20F);
            // cmbSearchType
            this.cmbSearchType.Location = new System.Drawing.Point(430, 32);
            this.cmbSearchType.Size = new System.Drawing.Size(150, 48);
            this.cmbSearchType.Font = new System.Drawing.Font("Microsoft JhengHei UI", 20F);
            this.cmbSearchType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbSearchType.Items.Clear();
            this.cmbSearchType.Items.AddRange(new object[] { "全部", "書名", "ISBN", "作者", "出版社", "分類" });
            this.cmbSearchType.SelectedIndex = 0;
            // btnLoginLogout
            this.btnLoginLogout.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnLoginLogout.Location = new System.Drawing.Point(690, 10); // Adjusted position
            this.btnLoginLogout.Name = "btnLoginLogout";
            this.btnLoginLogout.Size = new System.Drawing.Size(100, 30);
            this.btnLoginLogout.TabIndex = 4;
            this.btnLoginLogout.Text = "登入";
            this.btnLoginLogout.UseVisualStyleBackColor = true;
            // 
            // lblUsername
            // 
            this.lblUsername.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblUsername.AutoSize = true;
            this.lblUsername.Location = new System.Drawing.Point(570, 15); // Adjusted position
            this.lblUsername.Name = "lblUsername";
            this.lblUsername.Size = new System.Drawing.Size(0, 20);
            this.lblUsername.TabIndex = 5;
            this.lblUsername.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // btnAdmin
            // 
            this.btnAdmin.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnAdmin.Location = new System.Drawing.Point(480, 10); // Adjusted position
            this.btnAdmin.Name = "btnAdmin";
            this.btnAdmin.Size = new System.Drawing.Size(80, 30);
            this.btnAdmin.TabIndex = 6;
            this.btnAdmin.Text = "管理";
            this.btnAdmin.UseVisualStyleBackColor = true;
            this.btnAdmin.Visible = false; // Initially hide the button
            // 
            // panelHeader
            // 
            this.panelHeader = new System.Windows.Forms.Panel();
            this.panelHeader.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelHeader.Height = 40;
            this.panelHeader.Padding = new System.Windows.Forms.Padding(0);
            this.panelHeader.Margin = new System.Windows.Forms.Padding(0);
            this.panelHeader.Controls.Clear();
            this.labelSearch.Location = new System.Drawing.Point(10, 10);
            this.labelSearch.Size = new System.Drawing.Size(60, 16);
            this.labelSearch.Font = new System.Drawing.Font("Microsoft JhengHei UI", 10F);
            this.txtSearch.Location = new System.Drawing.Point(70, 6);
            this.txtSearch.Size = new System.Drawing.Size(180, 27);
            this.txtSearch.Font = new System.Drawing.Font("Microsoft JhengHei UI", 10F);
            this.cmbSearchType.Location = new System.Drawing.Point(260, 6);
            this.cmbSearchType.Size = new System.Drawing.Size(80, 27);
            this.cmbSearchType.Font = new System.Drawing.Font("Microsoft JhengHei UI", 10F);
            this.btnSearch.Location = new System.Drawing.Point(350, 6);
            this.btnSearch.Size = new System.Drawing.Size(60, 27);
            this.btnSearch.Font = new System.Drawing.Font("Microsoft JhengHei UI", 10F);
            this.btnAdmin.Location = new System.Drawing.Point(420, 6);
            this.btnAdmin.Size = new System.Drawing.Size(60, 27);
            this.btnAdmin.Font = new System.Drawing.Font("Microsoft JhengHei UI", 10F);
            this.lblUsername.Location = new System.Drawing.Point(490, 10);
            this.lblUsername.Size = new System.Drawing.Size(100, 16);
            this.lblUsername.Font = new System.Drawing.Font("Microsoft JhengHei UI", 10F);
            this.btnLoginLogout.Location = new System.Drawing.Point(600, 6);
            this.btnLoginLogout.Size = new System.Drawing.Size(80, 27);
            this.btnLoginLogout.Font = new System.Drawing.Font("Microsoft JhengHei UI", 10F);
            this.panelHeader.Controls.Add(this.labelSearch);
            this.panelHeader.Controls.Add(this.txtSearch);
            this.panelHeader.Controls.Add(this.cmbSearchType);
            this.panelHeader.Controls.Add(this.btnSearch);
            this.panelHeader.Controls.Add(this.btnAdmin);
            this.panelHeader.Controls.Add(this.lblUsername);
            this.panelHeader.Controls.Add(this.btnLoginLogout);
            // 
            // panelDetail
            // 
            this.panelDetail = new System.Windows.Forms.Panel();
            this.panelDetail.Dock = System.Windows.Forms.DockStyle.Right;
            this.panelDetail.Width = 420;
            this.panelDetail.BackColor = System.Drawing.Color.White;
            this.lblDetailTitle = new System.Windows.Forms.Label();
            this.lblDetailAuthor = new System.Windows.Forms.Label();
            this.lblDetailCategory = new System.Windows.Forms.Label();
            this.lblDetailStatus = new System.Windows.Forms.Label();
            this.lblDetailReserve = new System.Windows.Forms.Label();
            this.btnBigRent = new System.Windows.Forms.Button();
            this.btnBigReserve = new System.Windows.Forms.Button();
            this.lblDetailISBN = new System.Windows.Forms.Label();
            this.lblDetailPublisher = new System.Windows.Forms.Label();
            this.panelDetail.Controls.Add(this.lblDetailTitle);
            this.panelDetail.Controls.Add(this.lblDetailAuthor);
            this.panelDetail.Controls.Add(this.lblDetailCategory);
            this.panelDetail.Controls.Add(this.lblDetailStatus);
            this.panelDetail.Controls.Add(this.lblDetailReserve);
            this.panelDetail.Controls.Add(this.btnBigRent);
            this.panelDetail.Controls.Add(this.btnBigReserve);
            this.panelDetail.Controls.Add(this.lblDetailISBN);
            this.panelDetail.Controls.Add(this.lblDetailPublisher);
            // 
            // lblDetailTitle
            // 
            this.lblDetailTitle.Font = new System.Drawing.Font("Microsoft JhengHei UI", 20F, System.Drawing.FontStyle.Bold);
            this.lblDetailTitle.Location = new System.Drawing.Point(20, 20);
            this.lblDetailTitle.Size = new System.Drawing.Size(380, 48);
            // 
            // lblDetailAuthor
            // 
            this.lblDetailAuthor.Font = new System.Drawing.Font("Microsoft JhengHei UI", 20F);
            this.lblDetailAuthor.Location = new System.Drawing.Point(20, 70);
            this.lblDetailAuthor.Size = new System.Drawing.Size(380, 40);
            // 
            // lblDetailCategory
            // 
            this.lblDetailCategory.Font = new System.Drawing.Font("Microsoft JhengHei UI", 20F);
            this.lblDetailCategory.Location = new System.Drawing.Point(20, 110);
            this.lblDetailCategory.Size = new System.Drawing.Size(380, 40);
            // 
            // lblDetailStatus
            // 
            this.lblDetailStatus.Font = new System.Drawing.Font("Microsoft JhengHei UI", 20F);
            this.lblDetailStatus.Location = new System.Drawing.Point(20, 230);
            this.lblDetailStatus.Size = new System.Drawing.Size(380, 40);
            // 
            // lblDetailReserve
            // 
            this.lblDetailReserve.Font = new System.Drawing.Font("Microsoft JhengHei UI", 20F);
            this.lblDetailReserve.Location = new System.Drawing.Point(20, 270);
            this.lblDetailReserve.Size = new System.Drawing.Size(380, 40);
            // 
            // btnBigRent
            // 
            this.btnBigRent.Font = new System.Drawing.Font("Microsoft JhengHei UI", 20F, System.Drawing.FontStyle.Bold);
            this.btnBigRent.Location = new System.Drawing.Point(40, 370);
            this.btnBigRent.Size = new System.Drawing.Size(160, 60);
            this.btnBigRent.Text = "借書";
            // 
            // btnBigReserve
            // 
            this.btnBigReserve.Font = new System.Drawing.Font("Microsoft JhengHei UI", 20F, System.Drawing.FontStyle.Bold);
            this.btnBigReserve.Location = new System.Drawing.Point(220, 370);
            this.btnBigReserve.Size = new System.Drawing.Size(160, 60);
            this.btnBigReserve.Text = "預約";
            // 
            // lblDetailISBN
            // 
            this.lblDetailISBN.Font = new System.Drawing.Font("Microsoft JhengHei UI", 20F);
            this.lblDetailISBN.Location = new System.Drawing.Point(20, 150);
            this.lblDetailISBN.Size = new System.Drawing.Size(380, 40);
            // 
            // lblDetailPublisher
            // 
            this.lblDetailPublisher = new System.Windows.Forms.Label();
            this.lblDetailPublisher.Font = new System.Drawing.Font("Microsoft JhengHei UI", 20F);
            this.lblDetailPublisher.Location = new System.Drawing.Point(20, 190);
            this.lblDetailPublisher.Size = new System.Drawing.Size(380, 40);
            // 
            // dgvBorrowRecord
            // 
            this.dgvBorrowRecord.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right))); // 停靠設定，使其隨視窗大小變化
            this.dgvBorrowRecord.AllowUserToAddRows = false;
            this.dgvBorrowRecord.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvBorrowRecord.Location = new System.Drawing.Point(0, 0); // 相對於 panelUserContent 的位置
            this.dgvBorrowRecord.Name = "dgvBorrowRecord";
            this.dgvBorrowRecord.RowTemplate.Height = 48;
            this.dgvBorrowRecord.RowHeadersWidth = 60;
            this.dgvBorrowRecord.Size = new System.Drawing.Size(800, 300); // 調整大小，為下方操作面板留空間
            this.dgvBorrowRecord.TabIndex = 0;
            this.dgvBorrowRecord.Font = new System.Drawing.Font("Microsoft JhengHei UI", 20F);
            this.dgvBorrowRecord.ColumnHeadersDefaultCellStyle.Font = new System.Drawing.Font("Microsoft JhengHei UI", 20F, System.Drawing.FontStyle.Bold);
            this.dgvBorrowRecord.Dock = System.Windows.Forms.DockStyle.Fill;
            // 
            // btnBorrowPrev
            // 
            this.btnBorrowPrev.Location = new System.Drawing.Point(590, 32);
            this.btnBorrowPrev.Name = "btnBorrowPrev";
            this.btnBorrowPrev.Size = new System.Drawing.Size(120, 48);
            this.btnBorrowPrev.TabIndex = 1;
            this.btnBorrowPrev.Text = "上一頁";
            this.btnBorrowPrev.UseVisualStyleBackColor = true;
            // 
            // btnBorrowNext
            // 
            this.btnBorrowNext.Location = new System.Drawing.Point(710, 32);
            this.btnBorrowNext.Name = "btnBorrowNext";
            this.btnBorrowNext.Size = new System.Drawing.Size(120, 48);
            this.btnBorrowNext.TabIndex = 2;
            this.btnBorrowNext.Text = "下一頁";
            this.btnBorrowNext.UseVisualStyleBackColor = true;
            // 
            // lblBorrowPage
            // 
            this.lblBorrowPage.AutoSize = true;
            this.lblBorrowPage.Location = new System.Drawing.Point(600, 35);
            this.lblBorrowPage.Name = "lblBorrowPage";
            this.lblBorrowPage.Size = new System.Drawing.Size(0, 20);
            this.lblBorrowPage.TabIndex = 3;
            // 
            // dgvReserveRecord
            // 
            this.dgvReserveRecord.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right))); // 停靠設定，使其隨視窗大小變化
            this.dgvReserveRecord.AllowUserToAddRows = false;
            this.dgvReserveRecord.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvReserveRecord.Location = new System.Drawing.Point(0, 0); // 相對於 panelUserContent 的位置
            this.dgvReserveRecord.Name = "dgvReserveRecord";
            this.dgvReserveRecord.RowTemplate.Height = 48;
            this.dgvReserveRecord.RowHeadersWidth = 60;
            this.dgvReserveRecord.Size = new System.Drawing.Size(800, 300); // 調整大小，為下方操作面板留空間
            this.dgvReserveRecord.TabIndex = 0;
            this.dgvReserveRecord.Font = new System.Drawing.Font("Microsoft JhengHei UI", 20F);
            this.dgvReserveRecord.ColumnHeadersDefaultCellStyle.Font = new System.Drawing.Font("Microsoft JhengHei UI", 20F, System.Drawing.FontStyle.Bold);
            this.dgvReserveRecord.Dock = System.Windows.Forms.DockStyle.Fill;
            // 
            // btnReservePrev
            // 
            this.btnReservePrev.Location = new System.Drawing.Point(590, 32);
            this.btnReservePrev.Name = "btnReservePrev";
            this.btnReservePrev.Size = new System.Drawing.Size(120, 48);
            this.btnReservePrev.TabIndex = 1;
            this.btnReservePrev.Text = "上一頁";
            this.btnReservePrev.UseVisualStyleBackColor = true;
            // 
            // btnReserveNext
            // 
            this.btnReserveNext.Location = new System.Drawing.Point(710, 32);
            this.btnReserveNext.Name = "btnReserveNext";
            this.btnReserveNext.Size = new System.Drawing.Size(120, 48);
            this.btnReserveNext.TabIndex = 2;
            this.btnReserveNext.Text = "下一頁";
            this.btnReserveNext.UseVisualStyleBackColor = true;
            // 
            // lblReservePage
            // 
            this.lblReservePage.AutoSize = true;
            this.lblReservePage.Location = new System.Drawing.Point(600, 35);
            this.lblReservePage.Name = "lblReservePage";
            this.lblReservePage.Size = new System.Drawing.Size(0, 20);
            this.lblReservePage.TabIndex = 3;
            // 
            // UserForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1152, 648); // 60% of 1920x1080
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Name = "UserForm";
            this.Text = "漫畫租書系統 - 使用者介面";
            this.WindowState = System.Windows.Forms.FormWindowState.Normal; // 不要最大化
            this.panelUserContent.ResumeLayout(false);
            this.panelUserContent.PerformLayout();
            this.panelComicActions.ResumeLayout(false);
            this.panelComicActions.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvComics)).EndInit();
            this.panelHeader.ResumeLayout(false); // Add ResumeLayout for panelHeader
            this.panelHeader.PerformLayout(); // Add PerformLayout for panelHeader
            this.panelDetail.ResumeLayout(false);
            this.panelDetail.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvBorrowRecord)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvReserveRecord)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        #endregion

        // UI 控制項變數宣告
        private System.Windows.Forms.Panel panelUserContent;
        private System.Windows.Forms.DataGridView dgvComics;
        private System.Windows.Forms.Button btnSearch;
        private System.Windows.Forms.TextBox txtSearch;
        private System.Windows.Forms.Label labelSearch;
        private System.Windows.Forms.Panel panelComicActions; // 漫畫操作面板變數
        private System.Windows.Forms.Button btnReserve; // 預約按鈕變數
        private System.Windows.Forms.Button btnRent; // 借閱按鈕變數
        private System.Windows.Forms.Label lblStatus; // 狀態標籤變數
        private System.Windows.Forms.Label lblAuthor; // 作者標籤變數
        private System.Windows.Forms.Label lblTitle; // 標題標籤變數

        // Add declarations for the new controls
        private System.Windows.Forms.Button btnLoginLogout;
        private System.Windows.Forms.Label lblUsername;
        private System.Windows.Forms.Button btnAdmin; // Add declaration for Admin button
        private System.Windows.Forms.Panel panelHeader; // Declare header panel
        private System.Windows.Forms.Panel panelDetail;
        private System.Windows.Forms.Label lblDetailTitle;
        private System.Windows.Forms.Label lblDetailAuthor;
        private System.Windows.Forms.Label lblDetailCategory;
        private System.Windows.Forms.Label lblDetailStatus;
        private System.Windows.Forms.Label lblDetailReserve;
        private System.Windows.Forms.Button btnBigRent;
        private System.Windows.Forms.Button btnBigReserve;
        private System.Windows.Forms.ComboBox cmbSearchType;
        private System.Windows.Forms.DataGridView dgvBorrowRecord;
        private System.Windows.Forms.Button btnBorrowPrev;
        private System.Windows.Forms.Button btnBorrowNext;
        private System.Windows.Forms.Label lblBorrowPage;
        private System.Windows.Forms.DataGridView dgvReserveRecord;
        private System.Windows.Forms.Button btnReservePrev;
        private System.Windows.Forms.Button btnReserveNext;
        private System.Windows.Forms.Label lblReservePage;
        private System.Windows.Forms.Label lblDetailISBN;
        private System.Windows.Forms.Label lblDetailPublisher;
    }
} 