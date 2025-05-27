using System;
using System.Data;
using System.Windows.Forms;
using MySql.Data.MySqlClient;
using System.Threading.Tasks;
using System.Collections.Generic;

#nullable disable  // 禁用 nullable reference types 檢查

namespace WinFormsApp1
{
    public partial class UserForm : Form
    {
        private int loggedInUserId = 0; // 用於儲存登入使用者的 ID
        private string loggedInUsername = ""; // 用於儲存登入使用者的用戶名
        private string loggedInUserRole = ""; // 用於儲存登入使用者的用戶角色
        private int currentBorrowPage = 1;
        private int currentReservePage = 1;
        private const int PageSize = 10; // 每頁顯示 10 筆資料

        // 新增首頁漫畫主表 DataGridView
        private DataGridView dgvUserComics;

        // 1. 新增 TabControl 與四個分頁
        private System.Windows.Forms.TabControl tabUserMain;
        private System.Windows.Forms.TabPage tabPageHome;
        private System.Windows.Forms.TabPage tabPageBorrow;
        private System.Windows.Forms.TabPage tabPageReserve;
        private System.Windows.Forms.TabPage tabPageProfile;

        // 使用者介面表單的建構子 (接收使用者 ID 和角色)
        public UserForm(int userId, string userRole)
        {
            // 初始化元件
            InitializeComponent();
            
            // 啟用雙緩衝，減少重繪閃爍
            typeof(DataGridView).InvokeMember(
                "DoubleBuffered",
                System.Reflection.BindingFlags.SetProperty |
                System.Reflection.BindingFlags.Instance |
                System.Reflection.BindingFlags.NonPublic,
                null,
                dgvComics,
                new object[] { true });

            // 設定 DataGridView 基本樣式
            dgvComics.Font = new System.Drawing.Font("Microsoft JhengHei UI", 20F);
            dgvComics.ColumnHeadersDefaultCellStyle.Font = new System.Drawing.Font("Microsoft JhengHei UI", 20F, System.Drawing.FontStyle.Bold);
            dgvComics.RowTemplate.Height = 48;
            dgvComics.RowHeadersWidth = 60;
            dgvComics.EnableHeadersVisualStyles = false;
            dgvComics.ColumnHeadersDefaultCellStyle.BackColor = System.Drawing.Color.WhiteSmoke;
            dgvComics.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvComics.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvComics.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.None;
            dgvComics.AllowUserToResizeRows = false;
            dgvComics.AllowUserToResizeColumns = true;
            dgvComics.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvComics.MultiSelect = false;
            dgvComics.ReadOnly = true;
            dgvComics.AllowUserToAddRows = false;
            dgvComics.AllowUserToDeleteRows = false;
            dgvComics.AllowUserToOrderColumns = false;
            dgvComics.StandardTab = true;
            dgvComics.TabStop = true;
            dgvComics.TabIndex = 0;

            // 設定表單標題
            string username = GetUsernameById(userId);
            this.Text = $"漫畫租書及預約系統 - 使用者介面（ID:{userId} 用戶名:{username}）";

            this.loggedInUserId = userId;
            this.loggedInUserRole = userRole; // 保存用戶角色

            // 設定視窗大小為固定寬度 1042，高度為螢幕 60%，並置中
            var screen = Screen.PrimaryScreen.WorkingArea;
            this.Size = new Size(1042, (int)(screen.Height * 0.6));
            this.StartPosition = FormStartPosition.CenterScreen;

            // 為搜尋按鈕添加點擊事件
            this.btnSearch.Click += new System.EventHandler(this.btnSearch_Click);
            // 為 DataGridView 添加選中行改變事件
            this.dgvComics.SelectionChanged += new System.EventHandler(this.dgvComics_SelectionChanged);
            // 為登入/登出按鈕添加點擊事件
            this.btnLoginLogout.Click += new System.EventHandler(this.btnLoginLogout_Click);
            // 為管理按鈕添加點擊事件
            this.btnAdmin.Click += new System.EventHandler(this.btnAdmin_Click);

            // 在建構子中註冊大按鈕事件
            this.btnBigRent.Click += new System.EventHandler(this.btnBigRent_Click);
            this.btnBigReserve.Click += new System.EventHandler(this.btnBigReserve_Click);

            // 初始化時隱藏操作面板，直到選擇漫畫
            panelComicActions.Visible = false;

            // 視窗載入時載入漫畫資料並刷新紀錄
            this.Load += new System.EventHandler(this.UserForm_Load);

            // 初始狀態為未登入，更新 UI 顯示
            // UpdateLoginStatus(false); // 這個應該在登入成功後呼叫

            // 修改：初始化時根據角色禁用按鈕
            DisableButtonsForAdmin();

            // 綁定分頁按鈕事件
            this.btnBorrowPrev.Click += new EventHandler(btnBorrowPrev_Click);
            this.btnBorrowNext.Click += new EventHandler(btnBorrowNext_Click);
            this.btnReservePrev.Click += new EventHandler(btnReservePrev_Click);
            this.btnReserveNext.Click += new EventHandler(btnReserveNext_Click);

            // 只在建構子初始化分頁與首頁 UI
            InitializeUserTabs();
        }

        // 新增建構子以兼容舊程式碼 (雖然登入流程已修改，但保留以防萬一)
        public UserForm() : this(0, "")
        {
            // 初始化時禁用按鈕，因為沒有登入信息
             DisableButtonsForAdmin();
        }

        // 根據用戶角色禁用相關按鈕
        private void DisableButtonsForAdmin()
        {
            if (loggedInUserRole == "admin")
            {
                btnRent.Enabled = false; // 禁用小借閱按鈕
                btnReserve.Enabled = false; // 禁用小預約按鈕
                btnBigRent.Enabled = false; // 禁用大借閱/還書按鈕
                btnBigReserve.Enabled = false; // 禁用大預約/取消預約按鈕
            }
            else
            {
                // 對於普通用戶，按鈕的啟用狀態會在 SelectionChanged 事件中處理
                // 這裡確保它們在初始化時是禁用的，直到選擇漫畫
                btnRent.Enabled = false;
                btnReserve.Enabled = false;
                btnBigRent.Enabled = false;
                btnBigReserve.Enabled = false;
            }
             // 根據用戶角色控制管理按鈕的可見性
             btnAdmin.Visible = (loggedInUserRole == "admin");
             // 更新登入狀態顯示
             UpdateLoginStatus(loggedInUserId > 0, loggedInUserId, loggedInUsername, loggedInUserRole);
        }

        // UserForm Load 事件處理方法
        private async void UserForm_Load(object sender, EventArgs e)
        {
            // 1. 先初始化 UI，不等待數據
            SetupHomePageLayout();
            SetupBorrowPageLayout();
            SetupReservePageLayout();
            SetupProfilePageLayout();

            // 2. 設置基本 UI 屬性
            dgvComics.Font = new System.Drawing.Font("Microsoft JhengHei UI", 20F);
            dgvComics.ColumnHeadersDefaultCellStyle.Font = new System.Drawing.Font("Microsoft JhengHei UI", 20F, System.Drawing.FontStyle.Bold);
            dgvComics.RowTemplate.Height = 48;
            dgvComics.RowHeadersWidth = 60;
            SetComicsGridColumnSettings();

            // 3. 立即顯示表單，不等待數據
            this.Show();

            // 4. 在背景載入數據
            _ = Task.Run(async () =>
            {
                try
                {
                    // 4.1 載入首頁漫畫列表
                    await this.InvokeAsync(() => RefreshUserComicsGrid("", "全部"));

                    // 4.2 並行載入其他分頁數據
                    var borrowTask = RefreshBorrowRecordsAsync();
                    var reserveTask = RefreshReserveRecordsAsync();
                    await Task.WhenAll(borrowTask, reserveTask);
                }
                catch (Exception ex)
                {
                    await this.InvokeAsync(() => 
                        MessageBox.Show("載入數據時發生錯誤：" + ex.Message, "錯誤", MessageBoxButtons.OK, MessageBoxIcon.Error));
                }
            });
        }

        // 1. 新增 InitializeAllUserDataAsync 方法
        private async Task InitializeAllUserDataAsync()
        {
            await Task.WhenAll(
                RefreshUserComicsGrid("", "全部"),
                RefreshBorrowRecordsAsync(),
                RefreshReserveRecordsAsync()
            );
        }

        // 載入漫畫資料到 DataGridView，包含 ISBN、出版社、借閱狀態、預約狀態
        private void LoadComicsDataFromDatabase()
        {
            try
            {
                string sql = "SELECT comic_id, isbn, title, author, publisher, category FROM comic WHERE 1=1";
                DataTable dt = DBHelper.ExecuteQuery(sql);
                if (!dt.Columns.Contains("借閱狀態")) dt.Columns.Add("借閱狀態", typeof(string));
                if (!dt.Columns.Contains("預約狀態")) dt.Columns.Add("預約狀態", typeof(string));

                // 優化：一次查所有未還借閱紀錄
                var borrowDict = new Dictionary<int, int>();
                var borrowDt = DBHelper.ExecuteQuery("SELECT comic_id, user_id FROM borrow_record WHERE return_date IS NULL");
                foreach (DataRow row in borrowDt.Rows)
                    borrowDict[Convert.ToInt32(row["comic_id"])] = Convert.ToInt32(row["user_id"]);

                // 優化：一次查所有24小時內預約紀錄
                var reserveDict = new Dictionary<int, int>();
                var reserveDt = DBHelper.ExecuteQuery("SELECT comic_id, user_id FROM reservation WHERE status = 'active' AND reservation_date > DATE_SUB(NOW(), INTERVAL 24 HOUR)");
                foreach (DataRow row in reserveDt.Rows)
                    reserveDict[Convert.ToInt32(row["comic_id"])] = Convert.ToInt32(row["user_id"]);

                foreach (DataRow row in dt.Rows)
                {
                    int comicId = Convert.ToInt32(row["comic_id"]);
                    // 借閱狀態
                    bool isBorrowed = borrowDict.ContainsKey(comicId);
                    row["借閱狀態"] = isBorrowed ? "已被借" : "未被借";
                    // 預約狀態
                    string reserveStatus = isBorrowed ? "不可預約" :
                        reserveDict.ContainsKey(comicId) ? "已被預約" : "可預約";
                    row["預約狀態"] = reserveStatus;
                }
                dgvComics.DataSource = dt;
                dgvComics.ColumnHeadersVisible = true; // 強制顯示標題列
                // 設定 DataGridView 的欄位標題中文
                if (dgvComics.Columns.Contains("comic_id")) dgvComics.Columns["comic_id"].HeaderText = "書號";
                if (dgvComics.Columns.Contains("isbn")) dgvComics.Columns["isbn"].HeaderText = "ISBN";
                if (dgvComics.Columns.Contains("title")) dgvComics.Columns["title"].HeaderText = "書名";
                if (dgvComics.Columns.Contains("author")) dgvComics.Columns["author"].HeaderText = "作者";
                if (dgvComics.Columns.Contains("publisher")) dgvComics.Columns["publisher"].HeaderText = "出版社";
                if (dgvComics.Columns.Contains("category")) dgvComics.Columns["category"].HeaderText = "分類";
                if (dgvComics.Columns.Contains("借閱狀態")) dgvComics.Columns["借閱狀態"].HeaderText = "借閱狀態";
                if (dgvComics.Columns.Contains("預約狀態")) dgvComics.Columns["預約狀態"].HeaderText = "預約狀態";
                // 調整欄寬
                if (dgvComics.Columns.Contains("title")) dgvComics.Columns["title"].Width = 260;
                if (dgvComics.Columns.Contains("publisher")) dgvComics.Columns["publisher"].Width = 200;
                if (dgvComics.Columns.Contains("借閱狀態")) dgvComics.Columns["借閱狀態"].Width = 160;
                if (dgvComics.Columns.Contains("預約狀態")) dgvComics.Columns["預約狀態"].Width = 160;
            }
            catch (Exception ex)
            {
                MessageBox.Show("載入漫畫資料時發生錯誤：" + ex.Message, "錯誤", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // 搜尋按鈕點擊事件處理方法
        private void btnSearch_Click(object sender, EventArgs e)
        {
            DoSearch();
        }

        // 關鍵字與分類綜合查詢（包含 ISBN、出版社）
        private void DoSearch()
        {
            string searchTerm = txtSearch.Text.Trim();
            string searchType = cmbSearchType.SelectedItem?.ToString();
            try
            {
                string sql = "SELECT comic_id, isbn, title, author, publisher, category FROM comic WHERE 1=1";
                var paramList = new System.Collections.Generic.List<MySql.Data.MySqlClient.MySqlParameter>();
                if (!string.IsNullOrEmpty(searchTerm))
                {
                    if (searchType == "全部")
                    {
                        sql += " AND (comic_id LIKE @searchTerm OR title LIKE @searchTerm OR author LIKE @searchTerm OR category LIKE @searchTerm OR isbn LIKE @searchTerm OR publisher LIKE @searchTerm)";
                        paramList.Add(new MySql.Data.MySqlClient.MySqlParameter("@searchTerm", "%" + searchTerm + "%"));
                    }
                    else if (searchType == "書名")
                    {
                        sql += " AND title LIKE @searchTerm";
                        paramList.Add(new MySql.Data.MySqlClient.MySqlParameter("@searchTerm", "%" + searchTerm + "%"));
                    }
                    else if (searchType == "ISBN")
                    {
                        sql += " AND isbn LIKE @searchTerm";
                        paramList.Add(new MySql.Data.MySqlClient.MySqlParameter("@searchTerm", "%" + searchTerm + "%"));
                    }
                    else if (searchType == "作者")
                    {
                        sql += " AND author LIKE @searchTerm";
                        paramList.Add(new MySql.Data.MySqlClient.MySqlParameter("@searchTerm", "%" + searchTerm + "%"));
                    }
                    else if (searchType == "出版社")
                    {
                        sql += " AND publisher LIKE @searchTerm";
                        paramList.Add(new MySql.Data.MySqlClient.MySqlParameter("@searchTerm", "%" + searchTerm + "%"));
                    }
                    else if (searchType == "分類")
                    {
                        sql += " AND category LIKE @searchTerm";
                        paramList.Add(new MySql.Data.MySqlClient.MySqlParameter("@searchTerm", "%" + searchTerm + "%"));
                    }
                }
                var dt = DBHelper.ExecuteQuery(sql, paramList.ToArray());
                if (!dt.Columns.Contains("借閱狀態")) dt.Columns.Add("借閱狀態", typeof(string));
                if (!dt.Columns.Contains("預約狀態")) dt.Columns.Add("預約狀態", typeof(string));

                // 優化：一次查所有未還借閱紀錄
                var borrowDict = new Dictionary<int, int>();
                var borrowDt = DBHelper.ExecuteQuery("SELECT comic_id, user_id FROM borrow_record WHERE return_date IS NULL");
                foreach (DataRow row in borrowDt.Rows)
                    borrowDict[Convert.ToInt32(row["comic_id"])] = Convert.ToInt32(row["user_id"]);

                // 優化：一次查所有24小時內預約紀錄
                var reserveDict = new Dictionary<int, int>();
                var reserveDt = DBHelper.ExecuteQuery("SELECT comic_id, user_id FROM reservation WHERE status = 'active' AND reservation_date > DATE_SUB(NOW(), INTERVAL 24 HOUR)");
                foreach (DataRow row in reserveDt.Rows)
                    reserveDict[Convert.ToInt32(row["comic_id"])] = Convert.ToInt32(row["user_id"]);

                foreach (DataRow row in dt.Rows)
                {
                    int comicId = Convert.ToInt32(row["comic_id"]);
                    // 借閱狀態
                    bool isBorrowed = borrowDict.ContainsKey(comicId);
                    row["借閱狀態"] = isBorrowed ? "已被借" : "未被借";
                    // 預約狀態
                    string reserveStatus = isBorrowed ? "不可預約" :
                        reserveDict.ContainsKey(comicId) ? "已被預約" : "可預約";
                    row["預約狀態"] = reserveStatus;
                }
                dgvComics.DataSource = dt;
                dgvComics.ColumnHeadersVisible = true; // 強制顯示標題列
                // 設定 DataGridView 的欄位標題中文
                if (dgvComics.Columns.Contains("comic_id")) dgvComics.Columns["comic_id"].HeaderText = "書號";
                if (dgvComics.Columns.Contains("isbn")) dgvComics.Columns["isbn"].HeaderText = "ISBN";
                if (dgvComics.Columns.Contains("title")) dgvComics.Columns["title"].HeaderText = "書名";
                if (dgvComics.Columns.Contains("author")) dgvComics.Columns["author"].HeaderText = "作者";
                if (dgvComics.Columns.Contains("publisher")) dgvComics.Columns["publisher"].HeaderText = "出版社";
                if (dgvComics.Columns.Contains("category")) dgvComics.Columns["category"].HeaderText = "分類";
                if (dgvComics.Columns.Contains("借閱狀態")) dgvComics.Columns["借閱狀態"].HeaderText = "借閱狀態";
                if (dgvComics.Columns.Contains("預約狀態")) dgvComics.Columns["預約狀態"].HeaderText = "預約狀態";
                // 調整欄寬
                if (dgvComics.Columns.Contains("title")) dgvComics.Columns["title"].Width = 260;
                if (dgvComics.Columns.Contains("publisher")) dgvComics.Columns["publisher"].Width = 200;
                if (dgvComics.Columns.Contains("借閱狀態")) dgvComics.Columns["借閱狀態"].Width = 160;
                if (dgvComics.Columns.Contains("預約狀態")) dgvComics.Columns["預約狀態"].Width = 160;
                // 若有資料，自動選第一筆並顯示右側詳細資訊
                if (dt.Rows.Count > 0)
                {
                    dgvComics.ClearSelection();
                    dgvComics.Rows[0].Selected = true;
                    dgvComics_SelectionChanged(dgvComics, EventArgs.Empty);
                }
                else
                {
                    // 沒有資料時清空右側詳細資訊
                    lblDetailTitle.Text = "書名：";
                    lblDetailAuthor.Text = "作者：";
                    lblDetailCategory.Text = "分類：";
                    lblDetailStatus.Text = "借閱狀態：";
                    lblDetailReserve.Text = "預約狀態：";
                    lblDetailPublisher.Text = "出版社：";
                    btnBigRent.Enabled = false;
                    btnBigReserve.Enabled = false;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("搜尋漫畫時發生錯誤：" + ex.Message, "錯誤", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // DataGridView 選中行改變事件處理方法（顯示出版社）
        private void dgvComics_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvComics.CurrentRow != null && dgvComics.CurrentRow.Index >= 0 && dgvComics.CurrentRow.DataBoundItem != null)
            {
                DataGridViewRow selectedRow = dgvComics.CurrentRow;
                int comicId = Convert.ToInt32(selectedRow.Cells["comic_id"].Value);
                string isbn = selectedRow.Cells["isbn"].Value.ToString();
                string title = selectedRow.Cells["title"].Value.ToString();
                string author = selectedRow.Cells["author"].Value.ToString();
                string publisher = selectedRow.Cells["publisher"].Value.ToString();
                string category = selectedRow.Cells["category"].Value.ToString();

                // 更新右側詳細資訊
                lblDetailTitle.Text = "書名：" + title;
                lblDetailAuthor.Text = "作者：" + author;
                lblDetailCategory.Text = "分類：" + category;
                lblDetailPublisher.Text = "出版社：" + publisher;

                // 查詢借閱狀態
                string borrowStatus = "未被借";
                string sqlBorrow = "SELECT user_id FROM borrow_record WHERE comic_id = @cid AND return_date IS NULL";
                MySqlParameter[] p1 = { new MySql.Data.MySqlClient.MySqlParameter("@cid", comicId) };
                var dtBorrow = DBHelper.ExecuteQuery(sqlBorrow, p1);
                int borrowedUserId = -1;
                if (dtBorrow.Rows.Count > 0)
                {
                    borrowStatus = "已被借";
                    borrowedUserId = Convert.ToInt32(dtBorrow.Rows[0]["user_id"]);
                }
                lblDetailStatus.Text = "借閱狀態：" + borrowStatus;

                // 查詢預約狀態
                string reserveStatus = "可預約";
                int reservedUserId = -1;
                
                // 如果書被借出，直接設置為不可預約
                if (borrowStatus == "已被借")
                {
                    reserveStatus = "不可預約";
                }
                else
                {
                    // 只有在書未被借出時才檢查預約狀態
                    string sqlReserve = "SELECT user_id FROM reservation WHERE comic_id = @cid AND status = 'active' AND reservation_date > DATE_SUB(NOW(), INTERVAL 24 HOUR)";
                    var dtReserve = DBHelper.ExecuteQuery(sqlReserve, p1);
                    if (dtReserve.Rows.Count > 0)
                    {
                        reserveStatus = "已被預約";
                        reservedUserId = Convert.ToInt32(dtReserve.Rows[0]["user_id"]);
                    }
                }
                
                // 更新預約狀態顯示
                lblDetailReserve.Text = "預約狀態：" + reserveStatus;

                // 冷卻期判斷
                bool isCoolingBorrow = false;
                bool isCoolingReserve = false;
                DateTime? lastReturnTime = null;
                DateTime? lastReserveTime = null;

                if (loggedInUserId > 0)
                {
                    // 借書冷卻期
                    string sqlLastReturn = "SELECT return_date FROM borrow_record WHERE user_id = @uid AND comic_id = @cid AND return_date IS NOT NULL ORDER BY return_date DESC LIMIT 1";
                    var dtReturn = DBHelper.ExecuteQuery(sqlLastReturn, new MySql.Data.MySqlClient.MySqlParameter[] {
                        new MySql.Data.MySqlClient.MySqlParameter("@uid", loggedInUserId),
                        new MySql.Data.MySqlClient.MySqlParameter("@cid", comicId)
                    });
                    if (dtReturn.Rows.Count > 0)
                    {
                        lastReturnTime = Convert.ToDateTime(dtReturn.Rows[0]["return_date"]);
                        if ((DateTime.Now - lastReturnTime.Value).TotalHours < 24)
                        {
                            isCoolingBorrow = true;
                        }
                    }

                    // 預約冷卻期
                    string sqlLastReserve = "SELECT reservation_date FROM reservation WHERE user_id = @uid AND comic_id = @cid AND reservation_date > DATE_SUB(NOW(), INTERVAL 24 HOUR) ORDER BY reservation_date DESC LIMIT 1";
                    var dtLastReserve = DBHelper.ExecuteQuery(sqlLastReserve, new MySql.Data.MySqlClient.MySqlParameter[] {
                        new MySql.Data.MySqlClient.MySqlParameter("@uid", loggedInUserId),
                        new MySql.Data.MySqlClient.MySqlParameter("@cid", comicId)
                    });
                    if (dtLastReserve.Rows.Count > 0)
                    {
                        lastReserveTime = Convert.ToDateTime(dtLastReserve.Rows[0]["reservation_date"]);
                        if ((DateTime.Now - lastReserveTime.Value).TotalHours < 24)
                        {
                            isCoolingReserve = true;
                        }
                    }
                }

                // 按鈕啟用條件
                btnBigRent.Enabled = false;
                btnBigReserve.Enabled = false;

                if (loggedInUserRole != "admin" && loggedInUserId > 0)
                {
                    // 書被借出時
                    if (borrowStatus == "已被借")
                    {
                        // 只有借書者能還書
                        if (borrowedUserId == loggedInUserId)
                        {
                            btnBigRent.Text = "還書";
                            btnBigRent.Enabled = true;
                        }
                        else
                        {
                            btnBigRent.Text = "借書";
                            btnBigRent.Enabled = false;
                        }
                        btnBigReserve.Text = "不可預約";
                        btnBigReserve.Enabled = false;
                    }
                    // 書未被借出
                    else
                    {
                        // 有人預約時
                        if (reserveStatus == "已被預約")
                        {
                            // 只有預約者能借，且不在冷卻期
                            if (reservedUserId == loggedInUserId && !isCoolingBorrow)
                            {
                                btnBigRent.Text = "借書";
                                btnBigRent.Enabled = true;
                            }
                            else
                            {
                                btnBigRent.Text = "借書";
                                btnBigRent.Enabled = false;
                            }
                            btnBigReserve.Text = "不可預約";
                            btnBigReserve.Enabled = false;
                        }
                        // 無人預約時
                        else
                        {
                            // 借書按鈕狀態
                            if (!isCoolingBorrow)
                            {
                                btnBigRent.Text = "借書";
                                btnBigRent.Enabled = true;
                            }
                            else if (lastReturnTime.HasValue)
                            {
                                btnBigRent.Text = $"冷卻中({24 - (DateTime.Now - lastReturnTime.Value).TotalHours:F1}h)";
                                btnBigRent.Enabled = false;
                            }

                            // 預約按鈕狀態
                            if (!isCoolingReserve)
                            {
                                btnBigReserve.Text = "預約";
                                btnBigReserve.Enabled = true;
                            }
                            else if (lastReserveTime.HasValue)
                            {
                                btnBigReserve.Text = $"冷卻中({24 - (DateTime.Now - lastReserveTime.Value).TotalHours:F1}h)";
                                btnBigReserve.Enabled = false;
                            }
                        }
                    }
                }

                // 更新主表狀態欄位
                selectedRow.Cells["借閱狀態"].Value = borrowStatus;
                selectedRow.Cells["預約狀態"].Value = reserveStatus;
            }
            else
            {
                // 清空右側詳細資訊
                lblDetailTitle.Text = "書名：";
                lblDetailAuthor.Text = "作者：";
                lblDetailCategory.Text = "分類：";
                lblDetailPublisher.Text = "出版社：";
                lblDetailStatus.Text = "借閱狀態：";
                lblDetailReserve.Text = "預約狀態：";
                btnBigRent.Enabled = false;
                btnBigReserve.Enabled = false;
            }
        }

        // 借閱按鈕點擊事件處理方法
        private async void btnBigRent_Click(object sender, EventArgs e)
        {
            if (dgvComics.CurrentRow == null) return;
            int comicId = Convert.ToInt32(dgvComics.CurrentRow.Cells["comic_id"].Value);
            try
            {
                // 先查詢最新狀態
                string sqlBorrow = "SELECT user_id FROM borrow_record WHERE comic_id = @cid AND return_date IS NULL";
                var dtBorrow = DBHelper.ExecuteQuery(sqlBorrow, new MySql.Data.MySqlClient.MySqlParameter[] {
                    new MySql.Data.MySqlClient.MySqlParameter("@cid", comicId)
                });
                bool isBorrowed = dtBorrow.Rows.Count > 0;
                int borrowedUserId = isBorrowed ? Convert.ToInt32(dtBorrow.Rows[0]["user_id"]) : -1;
                if (isBorrowed && borrowedUserId == loggedInUserId)
                {
                    // 還書
                    string sqlReturn = "UPDATE borrow_record SET return_date = NOW() WHERE comic_id = @cid AND user_id = @uid AND return_date IS NULL";
                    int rowsAffected = DBHelper.ExecuteNonQuery(sqlReturn, new MySql.Data.MySqlClient.MySqlParameter[] {
                        new MySql.Data.MySqlClient.MySqlParameter("@cid", comicId),
                        new MySql.Data.MySqlClient.MySqlParameter("@uid", loggedInUserId)
                    });
                    if (rowsAffected > 0)
                    {
                        MessageBox.Show("還書成功！", "成功", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        await RefreshAllSectionsAsync();
                    }
                }
                else if (!isBorrowed)
                {
                    // 檢查預約
                    string sqlReserve = "SELECT user_id FROM reservation WHERE comic_id = @cid AND status = 'active' AND reservation_date > DATE_SUB(NOW(), INTERVAL 24 HOUR)";
                    var dtReserve = DBHelper.ExecuteQuery(sqlReserve, new MySql.Data.MySqlClient.MySqlParameter[] {
                        new MySql.Data.MySqlClient.MySqlParameter("@cid", comicId)
                    });
                    int reservedUserId = dtReserve.Rows.Count > 0 ? Convert.ToInt32(dtReserve.Rows[0]["user_id"]) : -1;
                    if (dtReserve.Rows.Count > 0 && reservedUserId != loggedInUserId)
                    {
                        MessageBox.Show("此書已被預約，只有預約者才能借閱。", "借閱失敗", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        await RefreshAllSectionsAsync();
                        return;
                    }
                    if (IsUserInCoolingPeriod(loggedInUserId, comicId))
                    {
                        MessageBox.Show("您仍在冷卻期內，無法借閱此書。", "借閱失敗", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        await RefreshAllSectionsAsync();
                        return;
                    }
                    // 借書
                    string sqlBorrowInsert = "INSERT INTO borrow_record (user_id, comic_id, borrow_date) VALUES (@uid, @cid, NOW())";
                    int rowsAffected = DBHelper.ExecuteNonQuery(sqlBorrowInsert, new MySql.Data.MySqlClient.MySqlParameter[] {
                        new MySql.Data.MySqlClient.MySqlParameter("@uid", loggedInUserId),
                        new MySql.Data.MySqlClient.MySqlParameter("@cid", comicId)
                    });
                    if (rowsAffected > 0)
                    {
                        // 自動取消自己預約（只取消24小時內的預約）
                        string sqlCancelReserve = "UPDATE reservation SET status = 'canceled' WHERE comic_id = @cid AND user_id = @uid AND status = 'active' AND reservation_date > DATE_SUB(NOW(), INTERVAL 24 HOUR)";
                        DBHelper.ExecuteNonQuery(sqlCancelReserve, new MySql.Data.MySqlClient.MySqlParameter[] {
                            new MySql.Data.MySqlClient.MySqlParameter("@cid", comicId),
                            new MySql.Data.MySqlClient.MySqlParameter("@uid", loggedInUserId)
                        });
                        MessageBox.Show("借書成功！", "成功", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        await RefreshAllSectionsAsync();
                    }
                }
                else
                {
                    MessageBox.Show("您無法借閱此書。", "借閱失敗", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("操作失敗：" + ex.Message, "錯誤", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void btnBigReserve_Click(object sender, EventArgs e)
        {
            if (dgvComics.CurrentRow == null) return;
            int comicId = Convert.ToInt32(dgvComics.CurrentRow.Cells["comic_id"].Value);
            try
            {
                // 首先檢查借書冷卻期，避免預約連續霸佔
                if (IsUserInCoolingPeriod(loggedInUserId, comicId))
                {
                    MessageBox.Show("您剛歸還此書，正在借閱冷卻期內，暫時無法預約。", "預約失敗", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // 查詢最新借閱狀態
                bool isBorrowed = await Task.Run(() => {
                    string sqlBorrow = "SELECT user_id FROM borrow_record WHERE comic_id = @cid AND return_date IS NULL";
                    var dtBorrow = DBHelper.ExecuteQuery(sqlBorrow, new MySql.Data.MySqlClient.MySqlParameter[] {
                        new MySql.Data.MySqlClient.MySqlParameter("@cid", comicId)
                    });
                    return dtBorrow.Rows.Count > 0;
                });

                if (isBorrowed)
                {
                    MessageBox.Show("此書已被借出，無法預約。", "預約失敗", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // 查詢預約狀態
                var activeReservation = await Task.Run(() => {
                    string sqlReserve = "SELECT user_id, reservation_id FROM reservation WHERE comic_id = @cid AND status = 'active' AND reservation_date > DATE_SUB(NOW(), INTERVAL 24 HOUR)";
                    var dtReserve = DBHelper.ExecuteQuery(sqlReserve, new MySql.Data.MySqlClient.MySqlParameter[] {
                        new MySql.Data.MySqlClient.MySqlParameter("@cid", comicId)
                    });
                    return dtReserve.Rows.Count > 0 ? new { UserId = Convert.ToInt32(dtReserve.Rows[0]["user_id"]), ReservationId = Convert.ToInt32(dtReserve.Rows[0]["reservation_id"]) } : null;
                });

                // 檢查是否是當前用戶自己的活躍預約 (表示嘗試取消預約)
                if (activeReservation != null && activeReservation.UserId == loggedInUserId)
                {
                    // 執行取消預約邏輯
                    string sqlCancel = "UPDATE reservation SET status = 'canceled' WHERE reservation_id = @rid AND user_id = @uid AND status = 'active'";
                    int rowsAffected = await Task.Run(() => DBHelper.ExecuteNonQuery(sqlCancel, new MySql.Data.MySqlClient.MySqlParameter[] {
                        new MySql.Data.MySqlClient.MySqlParameter("@rid", activeReservation.ReservationId),
                        new MySql.Data.MySqlClient.MySqlParameter("@uid", loggedInUserId)
                    }));

                    if (rowsAffected > 0)
                    {
                        MessageBox.Show("取消預約成功！", "成功", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        MessageBox.Show("取消預約失敗。", "錯誤", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    return;
                }
                // 檢查是否有其他用戶的活躍預約
                else if (activeReservation != null)
                {
                    MessageBox.Show("此書已被預約，無法再次預約。", "預約失敗", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // 檢查預約冷卻期 (只在嘗試新增預約時檢查)
                if (IsUserInReservationCoolingPeriod(loggedInUserId, comicId))
                {
                    MessageBox.Show("您仍在預約冷卻期內，無法預約此書。", "預約失敗", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    await RefreshAllSectionsAsync();
                    return;
                }

                // 新增預約 (只有在沒有被借出、沒有其他用戶活躍預約、且不在冷卻期時執行)
                string sqlInsert = "INSERT INTO reservation (user_id, comic_id, reservation_date, status) VALUES (@uid, @cid, NOW(), 'active')";
                int insertRowsAffected = await Task.Run(() => {
                    return DBHelper.ExecuteNonQuery(sqlInsert, new MySql.Data.MySqlClient.MySqlParameter[] {
                        new MySql.Data.MySqlClient.MySqlParameter("@uid", loggedInUserId),
                        new MySql.Data.MySqlClient.MySqlParameter("@cid", comicId)
                    });
                });
                if (insertRowsAffected > 0)
                {
                    MessageBox.Show("預約成功！", "成功", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("操作失敗：" + ex.Message, "錯誤", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // 登入/登出按鈕點擊事件處理方法
        private void btnLoginLogout_Click(object sender, EventArgs e)
        {
            if (loggedInUserId > 0) // 如果已登入，執行登出操作
            {
                Logout();
            }
            else // 如果未登入，彈出登入對話框
            {
                using (Form1 loginForm = new Form1())
                {
                    if (loginForm.ShowDialog() == DialogResult.OK)
                    {
                        // 登入成功
                        int userId = loginForm.LoggedInUserId; // 從登入表單獲取用戶 ID
                        string userRole = loginForm.LoggedInUserRole; // 從登入表單獲取用戶角色

                        // 使用 GetUsernameById 獲取用戶名
                        string username = GetUsernameById(userId);

                        UpdateLoginStatus(true, userId, username, userRole);
                        MessageBox.Show($"登入成功！歡迎，{username}", "成功", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        // 登入失敗或取消
                        MessageBox.Show("登入失敗或已取消", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            }
        }

        // 更新登入狀態和 UI 顯示
        private void UpdateLoginStatus(bool isLoggedIn, int userId = 0, string username = "", string userRole = "")
        {
            loggedInUserId = userId;
            loggedInUsername = username;
            loggedInUserRole = userRole; // 保存用戶角色

            if (isLoggedIn)
            {
                lblUsername.Text = $"歡迎，{loggedInUsername}";
                btnLoginLogout.Text = "登出";

                // 根據用戶角色控制管理按鈕的可見性
                btnAdmin.Visible = (loggedInUserRole == "admin");

                // 登入後，如果選中漫畫且狀態允許，則啟用借閱/預約按鈕 (已在 dgvComics_SelectionChanged 中處理)
                // 刷新 DataGridView 選擇，觸發 SelectionChanged 事件，更新按鈕狀態
                 if (dgvComics.SelectedRows.Count > 0)
                {
                    dgvComics_SelectionChanged(dgvComics, EventArgs.Empty);
                }

                // 修改：UpdateLoginStatus 也控制按鈕
                btnRent.Enabled = (loggedInUserRole != "admin");
                btnReserve.Enabled = (loggedInUserRole != "admin");
            }
            else
            {
                lblUsername.Text = ""; // 清空用戶名顯示
                btnLoginLogout.Text = "登入";

                // 未登入，隱藏管理按鈕，禁用借閱和預約按鈕
                btnAdmin.Visible = false;
                btnRent.Enabled = false;
                btnReserve.Enabled = false;
            }
        }

        // 執行登出操作
        private void Logout()
        {
            UpdateLoginStatus(false);
            MessageBox.Show("您已登出", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
             // 可選：清空或重新載入漫畫列表
            LoadComicsDataFromDatabase();
        }

        // 管理按鈕點擊事件處理方法
        private void btnAdmin_Click(object sender, EventArgs e)
        {
            // 檢查是否為管理員
            if (loggedInUserRole == "admin")
            {
                // 開啟管理員表單，並傳遞用戶 ID (可選)
                AdminForm adminForm = new AdminForm(loggedInUserId);
                // 使用 Show() 而不是 ShowDialog()，這樣 UserForm 不會被阻塞
                adminForm.Show();
            }
            else
            {
                // 理論上普通用戶看不到這個按鈕，但作為防護措施
                MessageBox.Show("您沒有權限訪問管理介面！", "警告", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        // 登入後自動刷新首頁
        public async Task OnLoginSuccess(int userId, string userRole)
        {
            this.loggedInUserId = userId;
            this.loggedInUserRole = userRole;
            btnRent.Enabled = true;
            btnReserve.Enabled = true;
            UpdateLoginStatus(true, userId, "", userRole);
            if (userRole == "admin")
            {
                AdminForm adminForm = new AdminForm(userId);
                adminForm.Show();
            }
            // 切換到首頁分頁
            tabUserMain.SelectedTab = tabPageHome;
            // 自動刷新首頁
            await RefreshAllSectionsAsync();
        }

        // 查詢並顯示用戶借閱/預約紀錄（完整歷史，不會清除）
        private async Task RefreshUserRecordsAsync()
        {
            var borrowTask = RefreshBorrowRecordsAsync();
            var reserveTask = RefreshReserveRecordsAsync();
            await Task.WhenAll(borrowTask, reserveTask);
        }

        // 分頁按鈕事件處理方法
        private async void btnBorrowPrev_Click(object sender, EventArgs e)
        {
            if (currentBorrowPage > 1)
            {
                currentBorrowPage--;
                await RefreshBorrowRecordsAsync();
            }
        }

        private async void btnBorrowNext_Click(object sender, EventArgs e)
        {
            currentBorrowPage++;
            await RefreshBorrowRecordsAsync();
        }

        private async void btnReservePrev_Click(object sender, EventArgs e)
        {
            if (currentReservePage > 1)
            {
                currentReservePage--;
                await RefreshReserveRecordsAsync();
            }
        }

        private async void btnReserveNext_Click(object sender, EventArgs e)
        {
            currentReservePage++;
            await RefreshReserveRecordsAsync();
        }

        // 修改刷新紀錄的方法，加入分頁功能
        private async Task RefreshBorrowRecordsAsync(string searchTerm = "")
        {
            try
            {
                // 在後台執行數據查詢
                var dt = await Task.Run(() => {
                    string sql = @"SELECT br.borrow_id, c.title, c.isbn, br.borrow_date, br.return_date, 
                                 CASE WHEN br.return_date IS NULL THEN '未還' ELSE '已還' END as status
                                 FROM borrow_record br
                                 JOIN comic c ON br.comic_id = c.comic_id
                                 WHERE br.user_id = @userId";
                    
                    var paramList = new List<MySqlParameter> {
                        new MySqlParameter("@userId", loggedInUserId)
                    };

                    if (!string.IsNullOrWhiteSpace(searchTerm))
                    {
                        sql += " AND (c.title LIKE @search OR c.isbn LIKE @search)";
                        paramList.Add(new MySqlParameter("@search", "%" + searchTerm + "%"));
                    }

                    sql += " ORDER BY br.borrow_date DESC LIMIT @offset, @pageSize";
                    paramList.Add(new MySqlParameter("@offset", (currentBorrowPage - 1) * PageSize));
                    paramList.Add(new MySqlParameter("@pageSize", PageSize));

                    return DBHelper.ExecuteQuery(sql, paramList.ToArray());
                });

                // 在 UI 線程上更新 DataGridView
                await this.InvokeAsync(() => {
                    // 暫停佈局以提高性能
                    dgvBorrowRecord.SuspendLayout();
                    
                    // 更新數據源
                    dgvBorrowRecord.DataSource = dt;
                    
                    // 確保在數據綁定後立即設定欄位屬性和按鈕狀態
                    SetBorrowGridSettingsAndButtonStatus(); // 新增此行，確保設置執行

                    // 更新分頁標籤和按鈕狀態
                    lblBorrowPage.Text = $"第 {currentBorrowPage} 頁";
                    btnBorrowPrev.Enabled = currentBorrowPage > 1;
                    
                    // 恢復佈局
                    dgvBorrowRecord.ResumeLayout();
                });

                // 在後台查詢總記錄數
                await Task.Run(async () => {
                    string countSql = @"SELECT COUNT(*) FROM borrow_record br
                                       JOIN comic c ON br.comic_id = c.comic_id
                                       WHERE br.user_id = @userId";
                    if (!string.IsNullOrWhiteSpace(searchTerm))
                    {
                        countSql += " AND (c.title LIKE @search OR c.isbn LIKE @search)";
                    }
                    var countParamList = new List<MySqlParameter> {
                        new MySqlParameter("@userId", loggedInUserId)
                    };
                    if (!string.IsNullOrWhiteSpace(searchTerm))
                    {
                        countParamList.Add(new MySqlParameter("@search", "%" + searchTerm + "%"));
                    }
                    long totalRecords = Convert.ToInt64(DBHelper.ExecuteScalar(countSql, countParamList.ToArray()));
                    
                    // 在 UI 線程上更新下一頁按鈕狀態
                    await this.InvokeAsync(() => {
                        btnBorrowNext.Enabled = (currentBorrowPage * PageSize) < totalRecords;
                    });
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show("載入借閱紀錄時發生錯誤：" + ex.Message, "錯誤", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // 修改 DgvBorrowRecord_DataBindingComplete 方法
        private void DgvBorrowRecord_DataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
        {
            if (dgvBorrowRecord.Columns.Count == 0) return;

            // 設定列寬和標題
            var columnSettings = new Dictionary<string, (string Header, int Width)>
            {
                { "borrow_id", ("編號", 60) },
                { "title", ("書名", 260) },
                { "isbn", ("ISBN", 120) },
                { "borrow_date", ("借閱日期", 180) },
                { "return_date", ("歸還日期", 180) },
                { "status", ("狀態", 80) }
            };

            foreach (var setting in columnSettings)
            {
                if (dgvBorrowRecord.Columns.Contains(setting.Key))
                {
                    var col = dgvBorrowRecord.Columns[setting.Key];
                    col.HeaderText = setting.Value.Header;
                    col.Width = setting.Value.Width;
                }
            }

            // 添加或獲取 "操作" 按鈕列
            var operationCol = dgvBorrowRecord.Columns.OfType<DataGridViewButtonColumn>().FirstOrDefault(c => c.Name == "操作");
            if (operationCol == null)
            {
                operationCol = new DataGridViewButtonColumn
                {
                    Name = "操作",
                    HeaderText = "操作",
                    UseColumnTextForButtonValue = false, // 設定為 false，與預約紀錄一致
                    Width = 80
                };
                dgvBorrowRecord.Columns.Add(operationCol);
            }
            else
            {
                operationCol.UseColumnTextForButtonValue = false; // 確保屬性正確設定為 false
            }

            // 更新按鈕狀態
            foreach (DataGridViewRow row in dgvBorrowRecord.Rows)
            {
                if (row.IsNewRow) continue;
                var statusCell = row.Cells["status"];
                var operationCell = row.Cells["操作"] as DataGridViewButtonCell;
                
                if (statusCell != null && operationCell != null)
                {
                    string status = statusCell.Value?.ToString() ?? "";
                    operationCell.Value = status == "未還" ? "還書" : "已還";
                    operationCell.ReadOnly = status != "未還";
                }
            }
        }

        

        // 優化預約紀錄查詢
        private async Task RefreshReserveRecordsAsync(string searchTerm = "")
        {
            try
            {
                string sql = @"SELECT r.reservation_id, c.title, c.isbn, r.reservation_date,
                             DATE_ADD(r.reservation_date, INTERVAL 24 HOUR) as expiry_date,
                             CASE
                                WHEN r.status = 'active' AND r.reservation_date > DATE_SUB(NOW(), INTERVAL 24 HOUR) THEN '預約中'
                                WHEN r.status = 'canceled' THEN '已取消'
                                WHEN r.reservation_date <= DATE_SUB(NOW(), INTERVAL 24 HOUR) THEN '已過期'
                                ELSE '未知'
                             END as status
                             FROM reservation r
                             JOIN comic c ON r.comic_id = c.comic_id
                             WHERE r.user_id = @userId";

                var paramList = new List<MySqlParameter> {
                    new MySqlParameter("@userId", loggedInUserId)
                };

                if (!string.IsNullOrWhiteSpace(searchTerm))
                {
                    sql += " AND (c.title LIKE @search OR c.isbn LIKE @search)";
                    paramList.Add(new MySqlParameter("@search", "%" + searchTerm + "%"));
                }

                sql += " ORDER BY r.reservation_date DESC LIMIT @offset, @pageSize";
                paramList.Add(new MySqlParameter("@offset", (currentReservePage - 1) * PageSize));
                paramList.Add(new MySql.Data.MySqlClient.MySqlParameter("@pageSize", PageSize));

                // 在後台執行數據查詢
                DataTable dt = await Task.Run(() => DBHelper.ExecuteQuery(sql, paramList.ToArray()));

                // 在 UI 線程上更新 DataGridView
                this.Invoke((MethodInvoker)delegate {
                    dgvReserveRecord.DataSource = dt;
                    SetReserveGridSettingsAndButtonStatus(); // 直接在此呼叫設定方法
                });

                // 更新分頁標籤和按鈕狀態
                lblReservePage.Text = $"第 {currentReservePage} 頁";
                btnReservePrev.Enabled = currentReservePage > 1;
                string countSql = @"SELECT COUNT(*) FROM reservation r
                                   JOIN comic c ON r.comic_id = c.comic_id
                                   WHERE r.user_id = @userId";
                 if (!string.IsNullOrWhiteSpace(searchTerm))
                {
                    countSql += " AND (c.title LIKE @search OR c.isbn LIKE @search)";
                }
                var countParamList = new List<MySqlParameter> {
                    new MySqlParameter("@userId", loggedInUserId)
                };
                 if (!string.IsNullOrWhiteSpace(searchTerm))
                 {
                     countParamList.Add(new MySql.Data.MySqlClient.MySqlParameter("@search", "%" + searchTerm + "%"));
                 }

                long totalRecords = await Task.Run(() => Convert.ToInt64(DBHelper.ExecuteScalar(countSql, countParamList.ToArray())));
                btnReserveNext.Enabled = (currentReservePage * PageSize) < totalRecords;

            }
            catch (Exception ex)
            {
                MessageBox.Show("載入預約紀錄時發生錯誤：" + ex.Message, "錯誤", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // 設定漫畫主表欄位標題、寬度、順序
        private void SetComicsGridColumnSettings()
        {
            dgvComics.ColumnHeadersHeight = 72;
            dgvComics.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.EnableResizing;
            if (dgvComics.Columns.Contains("comic_id")) { dgvComics.Columns["comic_id"].HeaderText = "書號"; dgvComics.Columns["comic_id"].Width = 80; }
            if (dgvComics.Columns.Contains("isbn")) { dgvComics.Columns["isbn"].HeaderText = "ISBN"; dgvComics.Columns["isbn"].Width = 120; }
            if (dgvComics.Columns.Contains("title")) { dgvComics.Columns["title"].HeaderText = "書名"; dgvComics.Columns["title"].Width = 440; }
            if (dgvComics.Columns.Contains("author")) { dgvComics.Columns["author"].HeaderText = "作者"; dgvComics.Columns["author"].Width = 180; }
            if (dgvComics.Columns.Contains("publisher")) { dgvComics.Columns["publisher"].HeaderText = "出版社"; dgvComics.Columns["publisher"].Width = 200; }
            if (dgvComics.Columns.Contains("category")) { dgvComics.Columns["category"].HeaderText = "分類"; dgvComics.Columns["category"].Width = 140; }
            if (dgvComics.Columns.Contains("借閱狀態")) { dgvComics.Columns["借閱狀態"].HeaderText = "借閱狀態"; dgvComics.Columns["借閱狀態"].Width = 100; }
            if (dgvComics.Columns.Contains("預約狀態")) { dgvComics.Columns["預約狀態"].HeaderText = "預約狀態"; dgvComics.Columns["預約狀態"].Width = 120; }
        }

        private bool borrowGridSettingsApplied = false;

        // 設定借閱紀錄欄位屬性和按鈕狀態
        private void SetBorrowGridSettingsAndButtonStatus()
        {
            if (dgvBorrowRecord == null || dgvBorrowRecord.Columns.Count == 0) return; // 檢查 DataGridView 和 Columns 是否存在

            // 設定 DataGridView 屬性 (只設定一次，而不是每次數據綁定都設定)
            dgvBorrowRecord.Font = new System.Drawing.Font("Microsoft JhengHei UI", 10F);
            dgvBorrowRecord.ColumnHeadersDefaultCellStyle.Font = new System.Drawing.Font("Microsoft JhengHei UI", 10F, System.Drawing.FontStyle.Bold);
            dgvBorrowRecord.RowTemplate.Height = 36;
            dgvBorrowRecord.RowHeadersWidth = 60;
            dgvBorrowRecord.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.EnableResizing;
            dgvBorrowRecord.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None;
            dgvBorrowRecord.EnableHeadersVisualStyles = false;
            dgvBorrowRecord.ColumnHeadersDefaultCellStyle.BackColor = System.Drawing.Color.WhiteSmoke;

            // 重新添加欄位標題和寬度設定
            var columnSettings = new Dictionary<string, (string Header, int Width)>
            {
                { "borrow_id", ("編號", 60) },
                { "title", ("書名", 260) },
                { "isbn", ("ISBN", 120) },
                { "borrow_date", ("借閱日期", 180) },
                { "return_date", ("歸還日期", 180) },
                { "status", ("狀態", 80) }
            };

            foreach (var setting in columnSettings)
            {
                if (dgvBorrowRecord.Columns.Contains(setting.Key))
                {
                    var col = dgvBorrowRecord.Columns[setting.Key];
                    col.HeaderText = setting.Value.Header;
                    col.Width = setting.Value.Width;
                }
            }

            // 添加或獲取 "操作" 按鈕列
            var operationCol = dgvBorrowRecord.Columns.OfType<DataGridViewButtonColumn>().FirstOrDefault(c => c.Name == "操作");
            if (operationCol == null)
            {
                operationCol = new DataGridViewButtonColumn
                {
                    Name = "操作",
                    HeaderText = "操作",
                    UseColumnTextForButtonValue = false, // 設定為 false，與預約紀錄一致
                    Width = 80
                };
                dgvBorrowRecord.Columns.Add(operationCol);
            }
            else
            {
                operationCol.UseColumnTextForButtonValue = false; // 確保屬性正確設定為 false
            }

            // 根據狀態設定按鈕文字和啟用狀態
            foreach (DataGridViewRow row in dgvBorrowRecord.Rows)
            {
                if (row.IsNewRow) continue;
                
                // 檢查是否存在狀態和操作欄位
                var statusCell = row.Cells["status"];
                var operationCell = row.Cells["操作"] as DataGridViewButtonCell;

                   if (statusCell != null && operationCell != null)
                   {
                       string status = statusCell.Value?.ToString() ?? "";

                       if (status == "未還")
                       {
                        operationCell.Value = "還書";
                           operationCell.ReadOnly = false;
                       }
                       else
                       {
                        operationCell.Value = "已還";
                           operationCell.ReadOnly = true;
                       }
                   }
            }
        }

        // dgvBorrowRecord 的 Paint 事件處理程式
        private void dgvBorrowRecord_Paint(object sender, PaintEventArgs e)
        {
            // 確保 DataGridView 有資料且設定尚未應用
            if (dgvBorrowRecord.Rows.Count > 0 && !borrowGridSettingsApplied)
            {
                // 設定 DataGridView 的欄位標題中文和寬度
                var borrowIdCol = dgvBorrowRecord.Columns.OfType<DataGridViewColumn>().FirstOrDefault(c => c.Name == "borrow_id");
                if (borrowIdCol != null) { borrowIdCol.HeaderText = "編號"; borrowIdCol.Width = 60; }

                var titleCol = dgvBorrowRecord.Columns.OfType<DataGridViewColumn>().FirstOrDefault(c => c.Name == "title");
                if (titleCol != null) { titleCol.HeaderText = "書名"; titleCol.Width = 260; }

                var isbnCol = dgvBorrowRecord.Columns.OfType<DataGridViewColumn>().FirstOrDefault(c => c.Name == "isbn");
                if (isbnCol != null) { isbnCol.HeaderText = "ISBN"; isbnCol.Width = 120; }

                var borrowDateCol = dgvBorrowRecord.Columns.OfType<DataGridViewColumn>().FirstOrDefault(c => c.Name == "borrow_date");
                if (borrowDateCol != null) { borrowDateCol.HeaderText = "借閱日期"; borrowDateCol.Width = 180; }

                var returnDateCol = dgvBorrowRecord.Columns.OfType<DataGridViewColumn>().FirstOrDefault(c => c.Name == "return_date");
                if (returnDateCol != null) { returnDateCol.HeaderText = "歸還日期"; returnDateCol.Width = 180; }

                var statusCol = dgvBorrowRecord.Columns.OfType<DataGridViewColumn>().FirstOrDefault(c => c.Name == "status");
                if (statusCol != null) { statusCol.HeaderText = "狀態"; statusCol.Width = 80; }

                // 設定操作按鈕列的 UseColumnTextForButtonValue
                var operationCol = dgvBorrowRecord.Columns.OfType<DataGridViewButtonColumn>().FirstOrDefault(c => c.Name == "操作");
                 if (operationCol != null)
                 {
                     operationCol.UseColumnTextForButtonValue = true; // 讓按鈕顯示 Value
                 }

                // 標記設定已應用
                borrowGridSettingsApplied = true;
            }
        }

        // dgvReserveRecord 的 DataBindingComplete 事件處理程式
        private void DgvReserveRecord_DataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
        {
            SetReserveGridSettingsAndButtonStatus(); // 在數據綁定完成後設定所有屬性和按鈕狀態
        }

        // 優化主表查詢：一次查回所有漫畫的借閱/預約狀態
        private async Task<DataTable> QueryComicsWithStatusAsync()
        {
            string sql = @"SELECT c.comic_id, c.isbn, c.title, c.author, c.publisher, c.category,
       CASE WHEN br.comic_id IS NOT NULL THEN '已被借' ELSE '未被借' END AS 借閱狀態,
       CASE
         WHEN br.comic_id IS NOT NULL THEN '不可預約'
         WHEN r.comic_id IS NOT NULL THEN '已被預約'
         ELSE '可預約'
       END AS 預約狀態
FROM comic c
LEFT JOIN borrow_record br ON c.comic_id = br.comic_id AND br.return_date IS NULL
LEFT JOIN (
    SELECT comic_id FROM reservation
    WHERE status = 'active' AND reservation_date > DATE_SUB(NOW(), INTERVAL 24 HOUR)
) r ON c.comic_id = r.comic_id";
            return await Task.Run(() => DBHelper.ExecuteQuery(sql));
        }
        // 優化借閱紀錄查詢
        private async Task<DataTable> QueryBorrowRecordsAsync()
        {
            string sql = @"SELECT br.borrow_id, c.title, c.isbn, br.borrow_date, br.return_date, 
                             CASE WHEN br.return_date IS NULL THEN '未還' ELSE '已還' END as status
                             FROM borrow_record br
                             JOIN comic c ON br.comic_id = c.comic_id
                             WHERE br.user_id = @userId
                             ORDER BY br.borrow_date DESC
                             LIMIT @offset, @pageSize";
            MySqlParameter[] parameters = {
                new MySqlParameter("@userId", loggedInUserId),
                new MySqlParameter("@offset", (currentBorrowPage - 1) * PageSize),
                new MySqlParameter("@pageSize", PageSize)
            };
            return await Task.Run(() => DBHelper.ExecuteQuery(sql, parameters));
        }
        // 優化預約紀錄查詢
        private async Task<DataTable> QueryReserveRecordsAsync()
        {
            string sql = @"SELECT r.reservation_id, c.title, c.isbn, r.reservation_date,
                             DATE_ADD(r.reservation_date, INTERVAL 24 HOUR) as expiry_date,
                             CASE
                                WHEN r.status = 'active' AND r.reservation_date > DATE_SUB(NOW(), INTERVAL 24 HOUR) THEN '預約中'
                                WHEN r.status = 'canceled' THEN '已取消'
                                WHEN r.reservation_date <= DATE_SUB(NOW(), INTERVAL 24 HOUR) THEN '已過期'
                                ELSE '未知'
                             END as status
                             FROM reservation r
                             JOIN comic c ON r.comic_id = c.comic_id
                             WHERE r.user_id = @userId
                             ORDER BY r.reservation_date DESC
                             LIMIT @offset, @pageSize";
            MySqlParameter[] parameters = {
                new MySqlParameter("@userId", loggedInUserId),
                new MySqlParameter("@offset", (currentReservePage - 1) * PageSize),
                new MySqlParameter("@pageSize", PageSize)
            };
            return await Task.Run(() => DBHelper.ExecuteQuery(sql, parameters));
        }
        // 三區查詢同步進行，全部完成後同步更新 UI
        private async Task RefreshAllSectionsAsync()
        {
            // 異步載入主頁漫畫列表
            // var comicsTask = RefreshUserComicsGrid("", "全部"); // 調用修改後的異步方法
            var borrowTask = QueryBorrowRecordsAsync();
            var reserveTask = QueryReserveRecordsAsync();
            await Task.WhenAll(borrowTask, reserveTask);
            // DataGridView 的更新已經在各自的 Refresh 方法內部處理
        }
        // 判斷借書冷卻期
        private bool IsUserInCoolingPeriod(int userId, int comicId)
        {
            string sql = "SELECT return_date FROM borrow_record WHERE user_id = @uid AND comic_id = @cid AND return_date IS NOT NULL AND return_date > DATE_SUB(NOW(), INTERVAL 24 HOUR) ORDER BY return_date DESC LIMIT 1";
            var dt = DBHelper.ExecuteQuery(sql, new MySql.Data.MySqlClient.MySqlParameter[] {
                new MySql.Data.MySqlClient.MySqlParameter("@uid", userId),
                new MySql.Data.MySqlClient.MySqlParameter("@cid", comicId)
            });
            return dt.Rows.Count > 0;
        }

        // 判斷預約冷卻期
        private bool IsUserInReservationCoolingPeriod(int userId, int comicId)
        {
            // 檢查用戶在過去 24 小時內是否有任何預約紀錄
            string sql = @"SELECT reservation_date 
                          FROM reservation 
                          WHERE user_id = @uid 
                          AND comic_id = @cid 
                           AND reservation_date > DATE_SUB(NOW(), INTERVAL 24 HOUR) 
                          ORDER BY reservation_date DESC 
                          LIMIT 1";
            var dt = DBHelper.ExecuteQuery(sql, new MySql.Data.MySqlClient.MySqlParameter[] {
                new MySql.Data.MySqlClient.MySqlParameter("@uid", userId),
                new MySql.Data.MySqlClient.MySqlParameter("@cid", comicId)
            });
            return dt.Rows.Count > 0;
        }

        // 3. 分頁切換自動刷新
        private async void tabUserMain_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (tabUserMain.SelectedTab.Text)
            {
                case "首頁":
                    // 首頁數據已在 RefreshAllSectionsAsync 中處理
                    break;
                case "借閱紀錄":
                    await RefreshBorrowRecordsAsync();
                    break;
                case "預約紀錄":
                    await RefreshReserveRecordsAsync();
                    break;
                case "會員中心":
                    break;
            }
        }

        // 新增首頁 UI 組裝方法
        private void SetupHomePageLayout()
        {
            tabPageHome.Controls.Clear();
            // 搜尋區（Dock=Top，高度40）
            var panelSearch = new Panel { Dock = DockStyle.Top, Height = 40, BackColor = System.Drawing.Color.WhiteSmoke };
            var labelSearch = new Label { Text = "搜尋：", Location = new System.Drawing.Point(10, 10), Size = new System.Drawing.Size(60, 20), Font = new System.Drawing.Font("Microsoft JhengHei UI", 10F) };
            var txtSearch = new TextBox { Name = "txtSearch", Location = new System.Drawing.Point(70, 6), Size = new System.Drawing.Size(180, 27), Font = new System.Drawing.Font("Microsoft JhengHei UI", 10F) };
            var cmbSearchType = new ComboBox { Name = "cmbSearchType", Location = new System.Drawing.Point(260, 6), Size = new System.Drawing.Size(80, 27), Font = new System.Drawing.Font("Microsoft JhengHei UI", 10F), DropDownStyle = ComboBoxStyle.DropDownList };
            cmbSearchType.Items.AddRange(new object[] { "全部", "書名", "ISBN", "作者", "出版社", "分類" });
            cmbSearchType.SelectedIndex = 0;
            var btnSearch = new Button { Name = "btnSearch", Text = "搜尋", Location = new System.Drawing.Point(350, 6), Size = new System.Drawing.Size(60, 27), Font = new System.Drawing.Font("Microsoft JhengHei UI", 10F) };
            btnSearch.Click += async (s, e) => await RefreshUserComicsGrid(txtSearch.Text, cmbSearchType.SelectedItem?.ToString());
            // 新增刷新按鈕
            var btnRefresh = new Button { Name = "btnHomeRefresh", Text = "刷新", Location = new System.Drawing.Point(420, 6), Size = new System.Drawing.Size(60, 27), Font = new System.Drawing.Font("Microsoft JhengHei UI", 10F) };
            btnRefresh.Click += async (s, e) => await RefreshUserComicsGrid(txtSearch.Text, cmbSearchType.SelectedItem?.ToString());
            panelSearch.Controls.Add(labelSearch);
            panelSearch.Controls.Add(txtSearch);
            panelSearch.Controls.Add(cmbSearchType);
            panelSearch.Controls.Add(btnSearch);
            panelSearch.Controls.Add(btnRefresh);
            // 主表 DataGridView（Dock=Fill）
            dgvUserComics = new DataGridView
            {
                Dock = DockStyle.Fill,
                Font = new System.Drawing.Font("Microsoft JhengHei UI", 10F),
                ColumnHeadersDefaultCellStyle = { Font = new System.Drawing.Font("Microsoft JhengHei UI", 10F, System.Drawing.FontStyle.Bold) },
                RowTemplate = { Height = 36 },
                AllowUserToAddRows = false,
                ReadOnly = true,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false,
                Name = "dgvUserComics",
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize,
                EnableHeadersVisualStyles = false,
                ColumnHeadersVisible = true
            };
            dgvUserComics.CellContentClick += DgvUserComics_CellContentClick;
            tabPageHome.Controls.Add(dgvUserComics);
            tabPageHome.Controls.Add(panelSearch);
        }

        private void SetupBorrowPageLayout()
        {
            tabPageBorrow.Controls.Clear();
            
            // 搜尋區
            var panelSearch = new Panel { Dock = DockStyle.Top, Height = 40, BackColor = System.Drawing.Color.WhiteSmoke };
            var labelSearch = new Label { Text = "搜尋：", Location = new System.Drawing.Point(10, 10), Size = new System.Drawing.Size(60, 20), Font = new System.Drawing.Font("Microsoft JhengHei UI", 10F) };
            var txtSearch = new TextBox { Name = "txtBorrowSearch", Location = new System.Drawing.Point(70, 6), Size = new System.Drawing.Size(180, 27), Font = new System.Drawing.Font("Microsoft JhengHei UI", 10F) };
            var btnSearch = new Button { Name = "btnBorrowSearch", Text = "搜尋", Location = new System.Drawing.Point(260, 6), Size = new System.Drawing.Size(60, 27), Font = new System.Drawing.Font("Microsoft JhengHei UI", 10F) };
            btnSearch.Click += async (s, e) => await RefreshBorrowRecordsAsync(txtSearch.Text);
            panelSearch.Controls.Add(labelSearch);
            panelSearch.Controls.Add(txtSearch);
            panelSearch.Controls.Add(btnSearch);

            // 分頁區
            var panelPaging = new Panel { Dock = DockStyle.Bottom, Height = 40, BackColor = System.Drawing.Color.WhiteSmoke };
            btnBorrowPrev = new Button { Name = "btnBorrowPrev", Text = "上一頁", Location = new System.Drawing.Point(10, 6), Size = new System.Drawing.Size(80, 27), Font = new System.Drawing.Font("Microsoft JhengHei UI", 10F) };
            btnBorrowNext = new Button { Name = "btnBorrowNext", Text = "下一頁", Location = new System.Drawing.Point(100, 6), Size = new System.Drawing.Size(80, 27), Font = new System.Drawing.Font("Microsoft JhengHei UI", 10F) };
            lblBorrowPage = new Label { Name = "lblBorrowPage", Text = "第 1 頁", Location = new System.Drawing.Point(200, 10), Size = new System.Drawing.Size(100, 20), Font = new System.Drawing.Font("Microsoft JhengHei UI", 10F) };
            btnBorrowPrev.Click += btnBorrowPrev_Click;
            btnBorrowNext.Click += btnBorrowNext_Click;
            panelPaging.Controls.Add(btnBorrowPrev);
            panelPaging.Controls.Add(btnBorrowNext);
            panelPaging.Controls.Add(lblBorrowPage);

            // DataGridView - 在初始化時設置所有樣式
            dgvBorrowRecord = new DataGridView
            {
                Name = "dgvBorrowRecord",
                Dock = DockStyle.Fill,
                AllowUserToAddRows = false,
                ReadOnly = true,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false,
                Font = new System.Drawing.Font("Microsoft JhengHei UI", 10F),
                ColumnHeadersDefaultCellStyle = new DataGridViewCellStyle 
                { 
                    Font = new System.Drawing.Font("Microsoft JhengHei UI", 10F, System.Drawing.FontStyle.Bold),
                    BackColor = System.Drawing.Color.WhiteSmoke
                },
                RowTemplate = { Height = 36 },
                RowHeadersWidth = 60,
                ColumnHeadersHeight = 24,
                ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.EnableResizing,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None,
                EnableHeadersVisualStyles = false,
                BackgroundColor = System.Drawing.Color.White
            };

            // 啟用雙緩衝
            typeof(DataGridView).InvokeMember(
                "DoubleBuffered",
                System.Reflection.BindingFlags.SetProperty |
                System.Reflection.BindingFlags.Instance |
                System.Reflection.BindingFlags.NonPublic,
                null,
                dgvBorrowRecord,
                new object[] { true });

            // 只綁定必要的事件
            dgvBorrowRecord.DataBindingComplete += DgvBorrowRecord_DataBindingComplete;
            // 綁定 CellContentClick 事件處理程式
            dgvBorrowRecord.CellContentClick += DgvBorrowRecord_CellContentClick; // 新增此行

            tabPageBorrow.Controls.Add(dgvBorrowRecord);
            tabPageBorrow.Controls.Add(panelPaging);
            tabPageBorrow.Controls.Add(panelSearch);
        }

        private void SetupReservePageLayout()
        {
            tabPageReserve.Controls.Clear();
            // 搜尋區
            var panelSearch = new Panel { Dock = DockStyle.Top, Height = 40, BackColor = System.Drawing.Color.WhiteSmoke };
            var labelSearch = new Label { Text = "搜尋：", Location = new System.Drawing.Point(10, 10), Size = new System.Drawing.Size(60, 20), Font = new System.Drawing.Font("Microsoft JhengHei UI", 10F) };
            var txtSearch = new TextBox { Name = "txtReserveSearch", Location = new System.Drawing.Point(70, 6), Size = new System.Drawing.Size(180, 27), Font = new System.Drawing.Font("Microsoft JhengHei UI", 10F) };
            var btnSearch = new Button { Name = "btnReserveSearch", Text = "搜尋", Location = new System.Drawing.Point(260, 6), Size = new System.Drawing.Size(60, 27), Font = new System.Drawing.Font("Microsoft JhengHei UI", 10F) };
            btnSearch.Click += async (s, e) => await RefreshReserveRecordsAsync(txtSearch.Text);
            panelSearch.Controls.Add(labelSearch);
            panelSearch.Controls.Add(txtSearch);
            panelSearch.Controls.Add(btnSearch);
            // 分頁區
            var panelPaging = new Panel { Dock = DockStyle.Bottom, Height = 40, BackColor = System.Drawing.Color.WhiteSmoke };
            btnReservePrev = new Button { Name = "btnReservePrev", Text = "上一頁", Location = new System.Drawing.Point(10, 6), Size = new System.Drawing.Size(80, 27), Font = new System.Drawing.Font("Microsoft JhengHei UI", 10F) };
            btnReserveNext = new Button { Name = "btnReserveNext", Text = "下一頁", Location = new System.Drawing.Point(100, 6), Size = new System.Drawing.Size(80, 27), Font = new System.Drawing.Font("Microsoft JhengHei UI", 10F) };
            lblReservePage = new Label { Name = "lblReservePage", Text = "第 1 頁", Location = new System.Drawing.Point(200, 10), Size = new System.Drawing.Size(100, 20), Font = new System.Drawing.Font("Microsoft JhengHei UI", 10F) };
            btnReservePrev.Click += btnReservePrev_Click;
            btnReserveNext.Click += btnReserveNext_Click;
            panelPaging.Controls.Add(btnReservePrev);
            panelPaging.Controls.Add(btnReserveNext);
            panelPaging.Controls.Add(lblReservePage);
            // DataGridView
            dgvReserveRecord = new DataGridView
            {
                Name = "dgvReserveRecord",
                Dock = DockStyle.Fill,
                AllowUserToAddRows = false,
                ReadOnly = true,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false,
                Font = new System.Drawing.Font("Microsoft JhengHei UI", 10F),
                ColumnHeadersDefaultCellStyle = new DataGridViewCellStyle
                {
                    Font = new System.Drawing.Font("Microsoft JhengHei UI", 10F, System.Drawing.FontStyle.Bold),
                    BackColor = System.Drawing.Color.WhiteSmoke
                },
                RowTemplate = { Height = 36 },
                RowHeadersWidth = 60,
                ColumnHeadersHeight = 24,
                ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.EnableResizing,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None,
                BackgroundColor = System.Drawing.Color.White
            };
            dgvReserveRecord.EnableHeadersVisualStyles = false;
            dgvReserveRecord.DataBindingComplete += DgvReserveRecord_DataBindingComplete;
            // 新增：綁定 CellContentClick 事件處理程式
            dgvReserveRecord.CellContentClick += DgvReserveRecord_CellContentClick;
            tabPageReserve.Controls.Add(dgvReserveRecord);
            tabPageReserve.Controls.Add(panelPaging);
            tabPageReserve.Controls.Add(panelSearch);
        }

        private void SetupProfilePageLayout()
        {
            tabPageProfile.Controls.Clear();
            // 會員中心簡單顯示用戶資訊
            var lblInfo = new Label
            {
                Name = "lblProfileInfo",
                Text = "會員中心功能開發中...",
                Dock = DockStyle.Top,
                Font = new System.Drawing.Font("Microsoft JhengHei UI", 14F),
                Height = 60,
                TextAlign = ContentAlignment.MiddleLeft
            };
            tabPageProfile.Controls.Add(lblInfo);
        }

        // 記錄上次操作的 comicId 以便刷新後自動選中
        private int lastActionComicId = 0;

        private async Task RefreshUserComicsGrid(string searchTerm, string searchType)
        {
            try
            {
                // 優化 SQL 查詢，一次性獲取所有需要的數據
                string sql = @"SELECT 
                    c.comic_id AS 書號, 
                    c.isbn AS ISBN, 
                    c.title AS 書名, 
                    c.author AS 作者, 
                    c.publisher AS 出版社, 
                    c.category AS 分類,
                    CASE WHEN br.comic_id IS NOT NULL THEN '已被借' ELSE '未被借' END AS 借閱狀態,
                    CASE
                        WHEN br.comic_id IS NOT NULL THEN '不可預約'
                        WHEN r.comic_id IS NOT NULL THEN '已被預約'
                        ELSE '可預約'
                    END AS 預約狀態,
                    br.user_id AS borrowed_by,
                    r.user_id AS reserved_by
                FROM comic c
                LEFT JOIN borrow_record br ON c.comic_id = br.comic_id AND br.return_date IS NULL
                LEFT JOIN (
                    SELECT comic_id, user_id 
                    FROM reservation 
                    WHERE status = 'active' 
                    AND reservation_date > DATE_SUB(NOW(), INTERVAL 24 HOUR)
                ) r ON c.comic_id = r.comic_id
                WHERE 1=1";

                var paramList = new List<MySqlParameter>();
                if (!string.IsNullOrWhiteSpace(searchTerm))
                {
                    if (searchType == "全部")
                    {
                        sql += " AND (c.comic_id LIKE @kw OR c.title LIKE @kw OR c.author LIKE @kw OR c.category LIKE @kw OR c.isbn LIKE @kw OR c.publisher LIKE @kw)";
                        paramList.Add(new MySqlParameter("@kw", "%" + searchTerm + "%"));
                    }
                    else if (searchType == "書名")
                    {
                        sql += " AND c.title LIKE @kw";
                        paramList.Add(new MySqlParameter("@kw", "%" + searchTerm + "%"));
                    }
                    else if (searchType == "ISBN")
                    {
                        sql += " AND c.isbn LIKE @kw";
                        paramList.Add(new MySqlParameter("@kw", "%" + searchTerm + "%"));
                    }
                    else if (searchType == "作者")
                    {
                        sql += " AND c.author LIKE @kw";
                        paramList.Add(new MySqlParameter("@kw", "%" + searchTerm + "%"));
                    }
                    else if (searchType == "出版社")
                    {
                        sql += " AND c.publisher LIKE @kw";
                        paramList.Add(new MySqlParameter("@kw", "%" + searchTerm + "%"));
                    }
                    else if (searchType == "分類")
                    {
                        sql += " AND c.category LIKE @kw";
                        paramList.Add(new MySqlParameter("@kw", "%" + searchTerm + "%"));
                    }
                }

                // 在後台執行數據查詢
                DataTable dt = await Task.Run(() => DBHelper.ExecuteQuery(sql, paramList.ToArray()));

                // 在 UI 線程上更新 DataGridView
                await this.InvokeAsync(() => {
                    dgvUserComics.DataSource = dt;
                    SetUserComicsGridColumnSettings();
                    UpdateComicsButtonColumnStates();
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show("載入漫畫資料時發生錯誤：" + ex.Message, "錯誤", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void SetUserComicsGridColumnSettings()
        {
            if (dgvUserComics == null || dgvUserComics.Columns.Count == 0) return;

            // Define desired order and width for all potential columns
            var columnSettings = new List<(string Name, string Header, int Width)>()
            {
                ("書號", "編號", 60), // 將顯示標題「書號」改為「編號」
                ("ISBN", "ISBN", 80),
                ("書名", "書名", 200),
                ("作者", "作者", 80),
                ("出版社", "出版社", 100),
                ("分類", "分類", 100),
                ("借閱狀態", "借閱狀態", 90),
                ("預約狀態", "預約狀態", 90),
                ("借書", "借書", 80),
                ("預約", "預約", 80)
            };

            // Add button columns if they don't exist yet
            if (dgvUserComics.Columns["借書"] == null)
            {
                var btnRent = new DataGridViewButtonColumn
                {
                    Name = "借書",
                    HeaderText = "借書",
                    Text = "借書", // Fallback text
                    UseColumnTextForButtonValue = false, // Use Value property
                    Width = 80     // 增加寬度從 60 到 80
                };
                dgvUserComics.Columns.Add(btnRent);
            }
            if (dgvUserComics.Columns["預約"] == null)
            {
                var btnReserve = new DataGridViewButtonColumn
                {
                    Name = "預約",
                    HeaderText = "預約",
                    Text = "預約", // Fallback text
                    UseColumnTextForButtonValue = false, // Use Value property
                    Width = 80     // 增加寬度從 60 到 80
                };
                dgvUserComics.Columns.Add(btnReserve);
            }
            // Apply settings and reorder for existing columns
            var currentColumns = dgvUserComics.Columns.OfType<DataGridViewColumn>().ToList();
            int displayIndex = 0;
            foreach (var setting in columnSettings)
            {
                var col = currentColumns.FirstOrDefault(c => c.Name == setting.Name);
                if (col != null)
                {
                    col.HeaderText = setting.Header;
                    col.Width = setting.Width;
                    try
                    {
                        // Ensure the column's DataPropertyName is set if it's a data column
                        if (!(col is DataGridViewButtonColumn))
                        {
                            col.DataPropertyName = setting.Name; // Set DataPropertyName for data columns
                        }
                        col.DisplayIndex = displayIndex++;
                    }
                    catch (ArgumentOutOfRangeException ex)
                    {
                        System.Diagnostics.Debug.WriteLine($"Error setting DisplayIndex for column {setting.Name}: {ex.Message}");
                        // Fallback: set to the end if desired index is out of range
                        col.DisplayIndex = dgvUserComics.Columns.Count > 0 ? dgvUserComics.Columns.Count - 1 : 0; // Handle empty column collection
                    }
                }
            }
            // Hide any columns that exist in the DataGridView but are not in columnSettings (unexpected columns)
            foreach (var col in currentColumns)
            {
                if (!columnSettings.Any(setting => setting.Name == col.Name))
                {
                    col.Visible = false;
                }
                else
                {
                    col.Visible = true; // Ensure expected columns are visible
                }
            }
            // Note: Button text and state setting logic remains in RefreshUserComicsGrid after data source is set.
        }

        // 動態設置主頁漫畫列表按鈕文字和狀態的方法
        private void UpdateComicsButtonColumnStates()
        {
            if (dgvUserComics == null || dgvUserComics.Rows.Count == 0) return;

            if (!dgvUserComics.Columns.Contains("借書") || !dgvUserComics.Columns.Contains("預約")) return;

            // 一次性獲取所有冷卻期和狀態數據
            var borrowCoolingSet = GetUserBorrowCoolingComicIds();
            var reserveCoolingSet = GetUserReserveCoolingComicIds();

            foreach (DataGridViewRow row in dgvUserComics.Rows)
            {
                if (row.IsNewRow) continue;
                
                int comicId = 0;
                if (dgvUserComics.Columns.Contains("書號") && row.Cells["書號"] != null && row.Cells["書號"].Value != null)
                    int.TryParse(row.Cells["書號"].Value.ToString(), out comicId);

                string borrowStatus = row.Cells["借閱狀態"]?.Value?.ToString() ?? "";
                string reserveStatus = row.Cells["預約狀態"]?.Value?.ToString() ?? "";
                int? borrowedBy = row.Cells["borrowed_by"]?.Value as int?;
                int? reservedBy = row.Cells["reserved_by"]?.Value as int?;

                // 更新借書按鈕
                var cellRent = row.Cells["借書"] as DataGridViewButtonCell;
                if (cellRent != null)
                {
                    if (borrowStatus == "已被借")
                    {
                        if (borrowedBy == loggedInUserId)
                        {
                            cellRent.Value = "還書";
                            cellRent.ReadOnly = false;
                        }
                        else
                        {
                            cellRent.Value = "借書";
                            cellRent.ReadOnly = true;
                        }
                    }
                    else
                    {
                        cellRent.Value = borrowCoolingSet.Contains(comicId) ? "冷卻中" : "借書";
                        cellRent.ReadOnly = borrowCoolingSet.Contains(comicId);
                    }
                }

                // 更新預約按鈕
                var cellReserve = row.Cells["預約"] as DataGridViewButtonCell;
                if (cellReserve != null)
                {
                    if (borrowStatus == "已被借")
                    {
                        cellReserve.Value = "不可預約";
                        cellReserve.ReadOnly = true;
                    }
                    else if (reserveStatus == "已被預約")
                    {
                        if (reservedBy == loggedInUserId)
                        {
                            cellReserve.Value = "取消預約";
                            cellReserve.ReadOnly = false;
                        }
                        else
                        {
                            cellReserve.Value = "已被預約";
                            cellReserve.ReadOnly = true;
                        }
                    }
                    else
                    {
                        cellReserve.Value = reserveCoolingSet.Contains(comicId) ? "冷卻中" : "預約";
                        cellReserve.ReadOnly = reserveCoolingSet.Contains(comicId);
                    }
                }
            }

            // 如果有上次操作的漫畫，自動選中
            if (lastActionComicId > 0)
            {
                foreach (DataGridViewRow row in dgvUserComics.Rows)
                {
                    if (row.IsNewRow) continue;
                    int cid = 0;
                    if (dgvUserComics.Columns.Contains("書號") && row.Cells["書號"] != null && row.Cells["書號"].Value != null)
                        int.TryParse(row.Cells["書號"].Value.ToString(), out cid);
                    if (cid == lastActionComicId)
                    {
                        row.Selected = true;
                        if (dgvUserComics.Columns.Contains("書名"))
                        {
                            dgvUserComics.CurrentCell = row.Cells["書名"];
                        }
                        break;
                    }
                }
            }
        }

        // DataGridView 按鈕點擊事件：根據欄位與行資料執行借書/預約邏輯
        private async void DgvUserComics_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || dgvUserComics.CurrentRow == null) return;
            var row = dgvUserComics.Rows[e.RowIndex];
            // 獲取點擊的儲存格
            var clickedCell = row.Cells[e.ColumnIndex];
            // 確保點擊的是按鈕欄位且是 DataGridViewButtonCell 類型
            if (clickedCell is DataGridViewButtonCell && clickedCell.OwningColumn.Name == "借書")
            {
                // 取得 comicId
                int comicId = 0;
                if (dgvUserComics.Columns.Contains("書號") && row.Cells["書號"] != null && row.Cells["書號"].Value != null)
                    int.TryParse(row.Cells["書號"].Value.ToString(), out comicId);
                if (comicId == 0) return;
                lastActionComicId = comicId; // 記錄本次操作
                // 記錄目前搜尋條件
                string searchTerm = "";
                string searchType = "全部";
                // 查找 TabPageHome 中的 txtSearch 和 cmbSearchType
                var homeTabPage = tabUserMain.TabPages["tabPageHome"];
                var txtSearch = homeTabPage.Controls.OfType<Panel>().FirstOrDefault()?.Controls.OfType<TextBox>().FirstOrDefault(c => c.Name == "txtSearch");
                var cmbSearchType = homeTabPage.Controls.OfType<Panel>().FirstOrDefault()?.Controls.OfType<ComboBox>().FirstOrDefault(c => c.Name == "cmbSearchType");

                if (txtSearch != null) searchTerm = txtSearch.Text;
                if (cmbSearchType != null && cmbSearchType.SelectedItem != null) searchType = cmbSearchType.SelectedItem.ToString();

                await HandleRentAction(comicId);
                // 異步刷新主頁漫畫列表
                await RefreshUserComicsGrid(searchTerm, searchType); // 使用異步刷新
                // 同步刷新借閱紀錄和預約紀錄（因為借書會影響這些列表的狀態）
                await RefreshBorrowRecordsAsync(); // Ensure this is awaited
                await RefreshReserveRecordsAsync(); // Ensure this is awaited
            }
            else if (clickedCell is DataGridViewButtonCell && clickedCell.OwningColumn.Name == "預約")
            {
                 // 取得 comicId
                int comicId = 0;
                if (dgvUserComics.Columns.Contains("書號") && row.Cells["書號"] != null && row.Cells["書號"].Value != null)
                    int.TryParse(row.Cells["書號"].Value.ToString(), out comicId);
                if (comicId == 0) return;
                 lastActionComicId = comicId; // 記錄本次操作
                // 記錄目前搜尋條件
                string searchTerm = "";
                string searchType = "全部";
                // 查找 TabPageHome 中的 txtSearch 和 cmbSearchType
                var homeTabPage = tabUserMain.TabPages["tabPageHome"];
                var txtSearch = homeTabPage.Controls.OfType<Panel>().FirstOrDefault()?.Controls.OfType<TextBox>().FirstOrDefault(c => c.Name == "txtSearch");
                var cmbSearchType = homeTabPage.Controls.OfType<Panel>().FirstOrDefault()?.Controls.OfType<ComboBox>().FirstOrDefault(c => c.Name == "cmbSearchType");

                if (txtSearch != null) searchTerm = txtSearch.Text;
                if (cmbSearchType != null && cmbSearchType.SelectedItem != null) searchType = cmbSearchType.SelectedItem.ToString();

                // 檢查預約狀態
                string reserveStatus = (dgvUserComics.Columns.Contains("預約狀態") && row.Cells["預約狀態"] != null) ? (row.Cells["預約狀態"].Value?.ToString() ?? "") : "";
                if (reserveStatus == "已被預約")
                {
                    // 查詢預約者是否為自己
                    string sql = "SELECT user_id FROM reservation WHERE comic_id = @cid AND status = 'active' AND reservation_date > DATE_SUB(NOW(), INTERVAL 24 HOUR)";
                    var dt = DBHelper.ExecuteQuery(sql, new MySql.Data.MySqlClient.MySqlParameter[] {
                        new MySql.Data.MySqlClient.MySqlParameter("@cid", comicId)
                    });
                    int reservedUserId = dt.Rows.Count > 0 ? Convert.ToInt32(dt.Rows[0]["user_id"]) : -1;

                    if (reservedUserId == loggedInUserId)
                    {
                await HandleReserveAction(comicId);
                // 異步刷新主頁漫畫列表
                        await RefreshUserComicsGrid(searchTerm, searchType);
                              // 刷新預約紀錄列表，並刷新所有 sections 以更新主頁狀態
                              await RefreshReserveRecordsAsync();
                              await RefreshAllSectionsAsync();
                         }
                         else
                         {
                        MessageBox.Show("此書已被其他用戶預約，無法預約。", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
                else if (reserveStatus == "不可預約")
                {
                    MessageBox.Show("此書已被借出，無法預約。", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else if (reserveStatus == "可預約")
                {
                    // 檢查是否在冷卻期
                    // Removed redundant cooling period check to ensure HandleReserveAction displays the detailed message.
                    await HandleReserveAction(comicId);
                    // 異步刷新主頁漫畫列表
                    await RefreshUserComicsGrid(searchTerm, searchType);
                    // 刷新預約紀錄列表，並刷新所有 sections 以更新主頁狀態
                    await RefreshReserveRecordsAsync();
                    await RefreshAllSectionsAsync();
                }
                 else
                 {
                      // 理論上不會發生此情況，但作為防護措施
                      MessageBox.Show("預約狀態異常，無法執行操作。", "錯誤", MessageBoxButtons.OK, MessageBoxIcon.Error);
                 }
            }
        }

        // Method definitions for InitializeUserTabs, SetReserveGridSettingsAndButtonStatus, HandleRentAction, HandleReserveAction and others if they exist and are missing.
        // Based on the errors, these methods seem to be missing or misplaced.
        // Assuming they are intended to be part of the UserForm class, I will add placeholder or relocate them here.

        // Example placeholder for missing methods (replace with actual implementation if available):

        // This method seems to initialize the user interface tabs. It was called in the constructor.
        private void InitializeUserTabs()
        {
            // 初始化分頁
            tabPageHome = new TabPage("首頁");
            tabPageHome.Name = "tabPageHome";
            tabPageBorrow = new TabPage("借閱紀錄");
            tabPageBorrow.Name = "tabPageBorrow";
            tabPageReserve = new TabPage("預約紀錄");
            tabPageReserve.Name = "tabPageReserve";
            tabPageProfile = new TabPage("會員中心");
            tabPageProfile.Name = "tabPageProfile";

            // 初始化 TabControl
            tabUserMain = new TabControl();
            tabUserMain.Name = "tabUserMain";
            tabUserMain.Dock = DockStyle.Fill;
            tabUserMain.TabPages.Add(tabPageHome);
            tabUserMain.TabPages.Add(tabPageBorrow);
            tabUserMain.TabPages.Add(tabPageReserve);
            tabUserMain.TabPages.Add(tabPageProfile);

            // 加入主表單控制項
            this.Controls.Add(tabUserMain);

            // 綁定分頁切換事件（如有需要）
            tabUserMain.SelectedIndexChanged += tabUserMain_SelectedIndexChanged;
            SetupHomePageLayout();
            SetupBorrowPageLayout();
            SetupReservePageLayout();
            SetupProfilePageLayout();
        }

        // This method seems to set up the settings for the reserve record DataGridView.
        // It was called in DgvReserveRecord_DataBindingComplete and RefreshReserveRecordsAsync.
        private void SetReserveGridSettingsAndButtonStatus()
        {
            if (dgvReserveRecord == null || dgvReserveRecord.Columns.Count == 0) return; // 檢查 DataGridView 和 Columns 是否存在

            // 設定 DataGridView 屬性
            dgvReserveRecord.Font = new System.Drawing.Font("Microsoft JhengHei UI", 10F);
            dgvReserveRecord.ColumnHeadersDefaultCellStyle.Font = new System.Drawing.Font("Microsoft JhengHei UI", 10F, System.Drawing.FontStyle.Bold);
            dgvReserveRecord.RowTemplate.Height = 36;
            dgvReserveRecord.RowHeadersWidth = 60;
            dgvReserveRecord.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.EnableResizing;
            dgvReserveRecord.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None;
            dgvReserveRecord.EnableHeadersVisualStyles = false;
            dgvReserveRecord.ColumnHeadersDefaultCellStyle.BackColor = System.Drawing.Color.WhiteSmoke;

            // 定義期望的欄位順序、標題和寬度
            var columnSettings = new List<(string Name, string Header, int Width)>()
            {
                ("reservation_id", "編號", 60),
                ("title", "書名", 260),
                ("isbn", "ISBN", 120),
                ("reservation_date", "預約日期", 180),
                ("expiry_date", "到期日期", 180), // 確保顯示到期日期
                ("status", "狀態", 80)
            };

             // 隱藏所有現有欄位
             foreach (DataGridViewColumn col in dgvReserveRecord.Columns) {
                 col.Visible = false;
             }

            // 按定義的順序添加或獲取欄位，並設定屬性
            int displayIndex = 0;
            foreach (var setting in columnSettings)
            {
                var col = dgvReserveRecord.Columns.OfType<DataGridViewColumn>().FirstOrDefault(c => c.Name == setting.Name);
                if (col != null)
                {
                    col.HeaderText = setting.Header;
                    col.Width = setting.Width;
                    col.DisplayIndex = displayIndex++;
                    col.Visible = true; // 確保期望的欄位是可見的
                }
            }

            // 添加或獲取 "操作" 按鈕列
            var operationCol = dgvReserveRecord.Columns.OfType<DataGridViewButtonColumn>().FirstOrDefault(c => c.Name == "操作");
            if (operationCol == null)
            {
                operationCol = new DataGridViewButtonColumn
                {
                    Name = "操作",
                    HeaderText = "操作",
                    UseColumnTextForButtonValue = false, // 使用 Value property
                    Width = 80,
                    DisplayIndex = displayIndex++
                };
                dgvReserveRecord.Columns.Add(operationCol);
            } else
            {
                 operationCol.UseColumnTextForButtonValue = false; // 確保屬性正確設定
                 operationCol.Visible = true; // 確保按鈕列可見
                 operationCol.DisplayIndex = displayIndex++;
            }

            // 根據狀態設定按鈕文字和啟用狀態
            foreach (DataGridViewRow row in dgvReserveRecord.Rows)
            {
                if (row.IsNewRow) continue;
                // 檢查是否存在狀態和操作欄位
                var statusCell = (dgvReserveRecord.Columns.Contains("status") && row.Cells["status"] != null) ? row.Cells["status"] : null;
                var operationCell = (dgvReserveRecord.Columns.Contains("操作") && row.Cells["操作"] != null) ? row.Cells["操作"] as DataGridViewButtonCell : null;

                if (statusCell != null && operationCell != null)
                {
                    string status = statusCell.Value?.ToString() ?? "";

                    // 只有狀態為 "預約中" 且未過期的紀錄才顯示 "取消預約" 按鈕
                    // 過期判斷已經在 SQL 查詢中處理 (reservation_date > DATE_SUB(NOW(), INTERVAL 24 HOUR))
                    if (status == "預約中")
                    {
                         // 檢查是否是自己的預約 (避免非預約者看到取消按鈕)
                         int reservationId = 0;
                         if (dgvReserveRecord.Columns.Contains("reservation_id") && row.Cells["reservation_id"] != null && row.Cells["reservation_id"].Value != null) {
                             int.TryParse(row.Cells["reservation_id"].Value.ToString(), out reservationId);
                         }

                         // 查詢預約的 user_id
                         string sql = "SELECT user_id FROM reservation WHERE reservation_id = @rid";
                         var dt = DBHelper.ExecuteQuery(sql, new MySql.Data.MySqlClient.MySqlParameter[] {
                             new MySql.Data.MySqlClient.MySqlParameter("@rid", reservationId)
                         });
                         int reservedUserId = dt.Rows.Count > 0 ? Convert.ToInt32(dt.Rows[0]["user_id"]) : -1;

                         if (reservedUserId == loggedInUserId)
                         {
                            operationCell.Value = "取消預約";
                            operationCell.ReadOnly = false;
                            if (operationCell.Style != null) operationCell.Style.ForeColor = System.Drawing.Color.DarkBlue;
                         } else
                         {
                              operationCell.Value = status; // 顯示狀態文字
                              operationCell.ReadOnly = true;
                              if (operationCell.Style != null) operationCell.Style.ForeColor = System.Drawing.Color.Gray;
                         }
                    }
                    else
                    {
                        operationCell.Value = status == "" ? "操作" : status; // 顯示狀態或預設文字
                        operationCell.ReadOnly = true;
                        if (operationCell.Style != null) operationCell.Style.ForeColor = System.Drawing.Color.Gray;
                    }
                }
            }
        }

        // 處理借書/還書邏輯的方法
        // 根據目前的狀態執行借書或還書操作
        private async Task HandleRentAction(int comicId)
        {
            try
            {
                // 檢查借書冷卻期並顯示詳細時間提示
                var dtReturn = await Task.Run(() => DBHelper.ExecuteQuery(
                    "SELECT return_date FROM borrow_record WHERE user_id = @uid AND comic_id = @cid AND return_date IS NOT NULL ORDER BY return_date DESC LIMIT 1",
                    new MySql.Data.MySqlClient.MySqlParameter[] {
                        new MySql.Data.MySqlClient.MySqlParameter("@uid", loggedInUserId),
                        new MySql.Data.MySqlClient.MySqlParameter("@cid", comicId)
                    }
                ));
                if (dtReturn.Rows.Count > 0)
                {
                    DateTime lastReturnTime = Convert.ToDateTime(dtReturn.Rows[0]["return_date"]);
                    TimeSpan elapsed = DateTime.Now - lastReturnTime;
                    TimeSpan coolingDuration = TimeSpan.FromHours(24);
                    TimeSpan remainingTime = coolingDuration - elapsed;
                    if (remainingTime.TotalSeconds > 0)
                    {
                        string timeMessage = $"您仍在借閱冷卻期內，還需等待 {remainingTime.Hours} 小時 {remainingTime.Minutes} 分 {remainingTime.Seconds} 秒。";
                        MessageBox.Show(timeMessage, "借閱失敗", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                }
                var dtBorrow = await Task.Run(() => DBHelper.ExecuteQuery(
                    "SELECT user_id FROM borrow_record WHERE comic_id = @cid AND return_date IS NULL",
                    new MySql.Data.MySqlClient.MySqlParameter[] {
                        new MySql.Data.MySqlClient.MySqlParameter("@cid", comicId)
                    }
                ));
                bool isBorrowed = dtBorrow.Rows.Count > 0;
                int borrowedUserId = isBorrowed ? Convert.ToInt32(dtBorrow.Rows[0]["user_id"]) : -1;
                if (isBorrowed && borrowedUserId == loggedInUserId)
                {
                    int rowsAffected = await Task.Run(() => DBHelper.ExecuteNonQuery(
                        "UPDATE borrow_record SET return_date = NOW() WHERE comic_id = @cid AND user_id = @uid AND return_date IS NULL",
                        new MySql.Data.MySqlClient.MySqlParameter[] {
                            new MySql.Data.MySqlClient.MySqlParameter("@cid", comicId),
                            new MySql.Data.MySqlClient.MySqlParameter("@uid", loggedInUserId)
                        }
                    ));
                    if (rowsAffected > 0)
                    {
                        MessageBox.Show("還書成功！", "成功", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
                else if (!isBorrowed)
                {
                    var dtReserve = await Task.Run(() => DBHelper.ExecuteQuery(
                        "SELECT user_id FROM reservation WHERE comic_id = @cid AND status = 'active' AND reservation_date > DATE_SUB(NOW(), INTERVAL 24 HOUR)",
                        new MySql.Data.MySqlClient.MySqlParameter[] {
                            new MySql.Data.MySqlClient.MySqlParameter("@cid", comicId)
                        }
                    ));
                    int reservedUserId = dtReserve.Rows.Count > 0 ? Convert.ToInt32(dtReserve.Rows[0]["user_id"]) : -1;
                    if (dtReserve.Rows.Count > 0 && reservedUserId != loggedInUserId)
                    {
                        MessageBox.Show("此書已被預約，只有預約者才能借閱。", "借閱失敗", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                    if (IsUserInCoolingPeriod(loggedInUserId, comicId))
                    {
                        MessageBox.Show("您仍在冷卻期內，無法借閱此書。", "借閱失敗", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                    int rowsAffected = await Task.Run(() => DBHelper.ExecuteNonQuery(
                        "INSERT INTO borrow_record (user_id, comic_id, borrow_date) VALUES (@uid, @cid, NOW())",
                        new MySql.Data.MySqlClient.MySqlParameter[] {
                            new MySql.Data.MySqlClient.MySqlParameter("@uid", loggedInUserId),
                            new MySql.Data.MySqlClient.MySqlParameter("@cid", comicId)
                        }
                    ));
                    if (rowsAffected > 0)
                    {
                        await Task.Run(() => DBHelper.ExecuteNonQuery(
                            "UPDATE reservation SET status = 'canceled' WHERE comic_id = @cid AND user_id = @uid AND status = 'active' AND reservation_date > DATE_SUB(NOW(), INTERVAL 24 HOUR)",
                            new MySql.Data.MySqlClient.MySqlParameter[] {
                                new MySql.Data.MySqlClient.MySqlParameter("@cid", comicId),
                                new MySql.Data.MySqlClient.MySqlParameter("@uid", loggedInUserId)
                            }
                        ));
                        MessageBox.Show("借書成功！", "成功", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
                else
                {
                    MessageBox.Show("您無法借閱此書。", "借閱失敗", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("操作失敗：" + ex.Message, "錯誤", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // 處理預約/取消預約邏輯的方法
        // 根據目前的狀態執行預約或取消預約操作
        private async Task HandleReserveAction(int comicId)
        {
            try
            {
                var activeReservation = await Task.Run(() => {
                    string sqlReserve = "SELECT user_id, reservation_id FROM reservation WHERE comic_id = @cid AND status = 'active' AND reservation_date > DATE_SUB(NOW(), INTERVAL 24 HOUR)";
                    var dtReserve = DBHelper.ExecuteQuery(sqlReserve, new MySql.Data.MySqlClient.MySqlParameter[] {
                        new MySql.Data.MySqlClient.MySqlParameter("@cid", comicId)
                    });
                    return dtReserve.Rows.Count > 0 ? new { UserId = Convert.ToInt32(dtReserve.Rows[0]["user_id"]), ReservationId = Convert.ToInt32(dtReserve.Rows[0]["reservation_id"]) } : null;
                });
                if (activeReservation != null && activeReservation.UserId == loggedInUserId)
                {
                    int rowsAffected = await Task.Run(() => DBHelper.ExecuteNonQuery(
                        "UPDATE reservation SET status = 'canceled' WHERE reservation_id = @rid AND user_id = @uid AND status = 'active'",
                        new MySql.Data.MySqlClient.MySqlParameter[] {
                            new MySql.Data.MySqlClient.MySqlParameter("@rid", activeReservation.ReservationId),
                            new MySql.Data.MySqlClient.MySqlParameter("@uid", loggedInUserId)
                        }
                    ));
                    if (rowsAffected > 0)
                    {
                        MessageBox.Show("取消預約成功！", "成功", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        MessageBox.Show("取消預約失敗。", "錯誤", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    return;
                }
                else if (activeReservation != null)
                {
                    MessageBox.Show("此書已被預約，無法再次預約。", "預約失敗", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                if (IsUserInCoolingPeriod(loggedInUserId, comicId))
                {
                    MessageBox.Show("您剛歸還此書，正在借閱冷卻期內，暫時無法預約。", "預約失敗", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                var dtReturn = await Task.Run(() => DBHelper.ExecuteQuery(
                    "SELECT return_date FROM borrow_record WHERE user_id = @uid AND comic_id = @cid AND return_date IS NOT NULL ORDER BY return_date DESC LIMIT 1",
                    new MySql.Data.MySqlClient.MySqlParameter[] {
                        new MySql.Data.MySqlClient.MySqlParameter("@uid", loggedInUserId),
                        new MySql.Data.MySqlClient.MySqlParameter("@cid", comicId)
                    }
                ));
                if (dtReturn.Rows.Count > 0)
                {
                    DateTime lastReturnTime = Convert.ToDateTime(dtReturn.Rows[0]["return_date"]);
                    TimeSpan elapsed = DateTime.Now - lastReturnTime;
                    TimeSpan coolingDuration = TimeSpan.FromHours(24);
                    TimeSpan remainingTime = coolingDuration - elapsed;
                    if (remainingTime.TotalSeconds > 0)
                    {
                        string timeMessage = $"您剛歸還此書，正在借閱冷卻期內，還需等待 {remainingTime.Hours} 小時 {remainingTime.Minutes} 分 {remainingTime.Seconds} 秒。";
                        MessageBox.Show(timeMessage, "預約失敗", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                }
                var dtBorrow = await Task.Run(() => DBHelper.ExecuteQuery(
                    "SELECT user_id FROM borrow_record WHERE comic_id = @cid AND return_date IS NULL",
                    new MySql.Data.MySqlClient.MySqlParameter[] {
                        new MySql.Data.MySqlClient.MySqlParameter("@cid", comicId)
                    }
                ));
                bool isBorrowed = dtBorrow.Rows.Count > 0;
                if (isBorrowed)
                {
                    MessageBox.Show("此書已被借出，無法預約。", "預約失敗", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                string sqlCooling = @"SELECT reservation_date 
                            FROM reservation 
                            WHERE user_id = @uid 
                            AND comic_id = @cid 
                            AND reservation_date > DATE_SUB(NOW(), INTERVAL 24 HOUR) 
                            ORDER BY reservation_date DESC 
                            LIMIT 1";
                var dtCooling = await Task.Run(() => DBHelper.ExecuteQuery(sqlCooling, new MySql.Data.MySqlClient.MySqlParameter[] {
                    new MySql.Data.MySqlClient.MySqlParameter("@uid", loggedInUserId),
                    new MySql.Data.MySqlClient.MySqlParameter("@cid", comicId)
                }));
                if (dtCooling.Rows.Count > 0)
                {
                    DateTime lastReserveTime = Convert.ToDateTime(dtCooling.Rows[0]["reservation_date"]);
                    TimeSpan remainingTime = TimeSpan.FromHours(24) - (DateTime.Now - lastReserveTime);
                    // 只有剩餘時間大於0才顯示詳細提示
                    if (remainingTime.TotalSeconds > 0)
                    {
                        string timeMessage = $"您仍在預約冷卻期內，還需等待 {remainingTime.Hours} 小時 {remainingTime.Minutes} 分 {remainingTime.Seconds} 秒。";
                        MessageBox.Show(timeMessage, "預約失敗", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                }
                int insertRowsAffected = await Task.Run(() => DBHelper.ExecuteNonQuery(
                    "INSERT INTO reservation (user_id, comic_id, reservation_date, status) VALUES (@uid, @cid, NOW(), 'active')",
                    new MySql.Data.MySqlClient.MySqlParameter[] {
                        new MySql.Data.MySqlClient.MySqlParameter("@uid", loggedInUserId),
                        new MySql.Data.MySqlClient.MySqlParameter("@cid", comicId)
                    }
                ));
                if (insertRowsAffected > 0)
                {
                    MessageBox.Show("預約成功！", "成功", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("操作失敗：" + ex.Message, "錯誤", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // 新增借閱紀錄 DataGridView 的 CellContentClick 事件處理程式
        private async void DgvBorrowRecord_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            // 確保點擊的是「操作」欄位且不是新行
            if (e.RowIndex < 0 || dgvBorrowRecord.Columns[e.ColumnIndex].Name != "操作") return;

            var row = dgvBorrowRecord.Rows[e.RowIndex];
            var statusCell = row.Cells["status"];
            var operationCell = row.Cells["操作"] as DataGridViewButtonCell;

            // 確保儲存格存在且狀態為「未還」
            if (statusCell != null && operationCell != null)
            {
                string status = statusCell.Value?.ToString() ?? "";

                if (status == "未還")
                {
                    // 獲取借閱 ID
                    int borrowId = 0;
                    if (dgvBorrowRecord.Columns.Contains("borrow_id") && row.Cells["borrow_id"] != null && row.Cells["borrow_id"].Value != null)
                    {
                         int.TryParse(row.Cells["borrow_id"].Value.ToString(), out borrowId);
                    }
                    
                    if (borrowId == 0) return; // 無效的借閱 ID

                    try
                    {
                        // 執行還書操作：更新 borrow_record 表的 return_date
                        string sql = "UPDATE borrow_record SET return_date = NOW() WHERE borrow_id = @bid";
                        int rowsAffected = await Task.Run(() => DBHelper.ExecuteNonQuery(sql, new MySql.Data.MySqlClient.MySqlParameter[] {
                            new MySql.Data.MySqlClient.MySqlParameter("@bid", borrowId)
                        }));

                        if (rowsAffected > 0)
                        {
                            MessageBox.Show("還書成功！", "成功", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            // 刷新借閱紀錄列表
                            await RefreshBorrowRecordsAsync();
                            // 同步刷新首頁漫畫列表，因為還書會影響其狀態
                            await RefreshUserComicsGrid("", "全部"); // 使用預設搜尋條件刷新
                        }
                        else
                        {
                            MessageBox.Show("還書失敗或紀錄已處理！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("還書操作發生錯誤：" + ex.Message, "錯誤", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                // 如果狀態不是「未還」，則按鈕是只讀的，不執行任何操作
            }
        }

        // 新增預約紀錄 DataGridView 的 CellContentClick 事件處理程式
        private async void DgvReserveRecord_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            // 確保點擊的是「操作」欄位且不是新行
            if (e.RowIndex < 0 || dgvReserveRecord.Columns.Count <= e.ColumnIndex || dgvReserveRecord.Columns[e.ColumnIndex].Name != "操作") return;

            var row = dgvReserveRecord.Rows[e.RowIndex];
            var clickedCell = row.Cells[e.ColumnIndex] as DataGridViewButtonCell;

            // 確保是按鈕儲存格且按鈕文字是「取消預約」
            if (clickedCell != null && clickedCell.Value?.ToString() == "取消預約")
            {
                // 獲取預約 ID
                int reservationId = 0;
                if (dgvReserveRecord.Columns.Contains("reservation_id") && row.Cells["reservation_id"] != null && row.Cells["reservation_id"].Value != null)
                {
                     int.TryParse(row.Cells["reservation_id"].Value.ToString(), out reservationId);
                }

                if (reservationId == 0) return; // 無效的預約 ID

                try
                {
                    // 透過 reservation_id 查詢 comic_id
                    string sqlComicId = "SELECT comic_id FROM reservation WHERE reservation_id = @rid";
                    var dtComic = DBHelper.ExecuteQuery(sqlComicId, new MySql.Data.MySqlClient.MySqlParameter[] {
                        new MySql.Data.MySqlClient.MySqlParameter("@rid", reservationId)
                    });

                    if (dtComic.Rows.Count > 0)
                    {
                        int comicId = Convert.ToInt32(dtComic.Rows[0]["comic_id"]);

                        // 呼叫 HandleReserveAction 來執行取消預約邏輯
                        await HandleReserveAction(comicId);

                        // 刷新預約紀錄列表和首頁漫畫列表
                        await RefreshReserveRecordsAsync();
                        await RefreshUserComicsGrid("", "全部"); // 使用預設搜尋條件刷新首頁
                    }
                    else
                    {
                        MessageBox.Show("找不到對應的漫畫資料。", "錯誤", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("取消預約操作發生錯誤：" + ex.Message, "錯誤", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        // 一次查出目前用戶所有借閱冷卻期漫畫ID
        private HashSet<int> GetUserBorrowCoolingComicIds()
        {
            var result = new HashSet<int>();
            string sql = @"SELECT comic_id FROM borrow_record 
                           WHERE user_id = @uid 
                           AND return_date IS NOT NULL 
                           AND return_date > DATE_SUB(NOW(), INTERVAL 24 HOUR)";
            var dt = DBHelper.ExecuteQuery(sql, new MySql.Data.MySqlClient.MySqlParameter[] {
                new MySql.Data.MySqlClient.MySqlParameter("@uid", loggedInUserId)
            });
            foreach (DataRow row in dt.Rows)
            {
                result.Add(Convert.ToInt32(row["comic_id"]));
            }
            return result;
        }

        // 一次查出目前用戶所有預約冷卻期漫畫ID
        private HashSet<int> GetUserReserveCoolingComicIds()
        {
            var result = new HashSet<int>();
            string sql = @"SELECT comic_id FROM reservation 
                           WHERE user_id = @uid 
                           AND reservation_date > DATE_SUB(NOW(), INTERVAL 24 HOUR)";
            var dt = DBHelper.ExecuteQuery(sql, new MySql.Data.MySqlClient.MySqlParameter[] {
                new MySql.Data.MySqlClient.MySqlParameter("@uid", loggedInUserId)
            });
            foreach (DataRow row in dt.Rows)
            {
                result.Add(Convert.ToInt32(row["comic_id"]));
            }
            return result;
        }

        // 一次查出所有目前被誰借走的漫畫ID與userId
        private Dictionary<int, int> GetCurrentBorrowedComicUserDict()
        {
            var dict = new Dictionary<int, int>();
            string sql = "SELECT comic_id, user_id FROM borrow_record WHERE return_date IS NULL";
            var dt = DBHelper.ExecuteQuery(sql);
            foreach (DataRow row in dt.Rows)
            {
                dict[Convert.ToInt32(row["comic_id"])] = Convert.ToInt32(row["user_id"]);
            }
            return dict;
        }

        // 一次查出所有目前被誰預約的漫畫ID與userId
        private Dictionary<int, int> GetCurrentReservedComicUserDict()
        {
            var dict = new Dictionary<int, int>();
            string sql = "SELECT comic_id, user_id FROM reservation WHERE status = 'active' AND reservation_date > DATE_SUB(NOW(), INTERVAL 24 HOUR)";
            var dt = DBHelper.ExecuteQuery(sql);
            foreach (DataRow row in dt.Rows)
            {
                dict[Convert.ToInt32(row["comic_id"])] = Convert.ToInt32(row["user_id"]);
            }
            return dict;
        }

        // 新增方法：根據用戶ID獲取用戶名
        private string GetUsernameById(int userId)
        {
            try
            {
                string sql = "SELECT username FROM user WHERE user_id = @userId";
                var param = new MySqlParameter("@userId", userId);
                object result = DBHelper.ExecuteScalar(sql, new[] { param });
                return result?.ToString() ?? "未知用戶";
            }
            catch
            {
                return "未知用戶";
            }
        }
    }
} // UserForm 結尾與 namespace 結尾

// 添加擴展方法以支持異步 UI 更新
namespace WinFormsApp1
{
    public static class ControlExtensions
    {
        public static async Task InvokeAsync(this Control control, Action action)
        {
            if (control.InvokeRequired)
            {
                await Task.Run(() => control.Invoke(action));
            }
            else
            {
                action();
            }
        }
    }
}

