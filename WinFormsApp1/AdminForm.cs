using System.Windows.Forms;
using MySql.Data.MySqlClient;
using System.Collections.Generic;
using System; // Ensure System is included for EventArgs
using System.Drawing;
using System.Threading.Tasks;

#nullable disable  // 禁用 nullable reference types 檢查

namespace WinFormsApp1
{
    public partial class AdminForm : Form
    {
        // 分頁相關常數
        private const int PageSize = 15;
        private const string DefaultSearchType = "用戶";

        // 當前分頁索引
        private int currentUserPage = 1;
        private int currentComicPage = 1;
        private int currentBorrowPage = 1;
        private int currentReservePage = 1;
        private int currentLogPage = 1; // 新增：管理日誌分頁

        // 搜尋參數
        private string currentComicSearchKeyword = "";
        private string currentComicSearchType = "書號";
        private string currentBorrowSearchKeyword = "";
        private string currentBorrowSearchType = DefaultSearchType;
        private string currentReserveSearchKeyword = "";
        private string currentReserveSearchType = DefaultSearchType;

        // 當前用戶ID
        private int currentUserId;

        public AdminForm(int userId)
        {
            InitializeComponent();
            this.currentUserId = userId;

            // 獲取當前登入用戶的用戶名
            string username = GetUsernameById(userId);
            this.Text = $"漫畫租書及預約系統 - 管理員介面（ID:{userId} 用戶名:{username}）";

            // 設定字體大小
            this.Font = new System.Drawing.Font("Microsoft JhengHei UI", 10.5F);

            // 設定視窗大小為固定寬度 910，高度為螢幕 60%，並置中
            var screen = Screen.PrimaryScreen.WorkingArea;
            this.Size = new Size(910, (int)(screen.Height * 0.6));
            this.StartPosition = FormStartPosition.CenterScreen;

            // 設定所有 DataGridView 的行高和標題列樣式
            dgvUser.RowTemplate.Height = 30;
            dgvUser.EnableHeadersVisualStyles = false;
            dgvUser.ColumnHeadersDefaultCellStyle.BackColor = System.Drawing.Color.WhiteSmoke;
            dgvUser.Font = new System.Drawing.Font("Microsoft JhengHei UI", 10.5F);
            dgvUser.ColumnHeadersDefaultCellStyle.Font = new System.Drawing.Font("Microsoft JhengHei UI", 10.5F, System.Drawing.FontStyle.Bold);

            dgvComic.RowTemplate.Height = 30;
            dgvComic.EnableHeadersVisualStyles = false;
            dgvComic.ColumnHeadersDefaultCellStyle.BackColor = System.Drawing.Color.WhiteSmoke;
            dgvComic.Font = new System.Drawing.Font("Microsoft JhengHei UI", 10.5F);
            dgvComic.ColumnHeadersDefaultCellStyle.Font = new System.Drawing.Font("Microsoft JhengHei UI", 10.5F, System.Drawing.FontStyle.Bold);

            dgvBorrow.RowTemplate.Height = 30;
            dgvBorrow.EnableHeadersVisualStyles = false;
            dgvBorrow.ColumnHeadersDefaultCellStyle.BackColor = System.Drawing.Color.WhiteSmoke;
            dgvBorrow.Font = new System.Drawing.Font("Microsoft JhengHei UI", 10.5F);
            dgvBorrow.ColumnHeadersDefaultCellStyle.Font = new System.Drawing.Font("Microsoft JhengHei UI", 10.5F, System.Drawing.FontStyle.Bold);

            dgvReserve.RowTemplate.Height = 30;
            dgvReserve.EnableHeadersVisualStyles = false;
            dgvReserve.ColumnHeadersDefaultCellStyle.BackColor = System.Drawing.Color.WhiteSmoke;
            dgvReserve.Font = new System.Drawing.Font("Microsoft JhengHei UI", 10.5F);
            dgvReserve.ColumnHeadersDefaultCellStyle.Font = new System.Drawing.Font("Microsoft JhengHei UI", 10.5F, System.Drawing.FontStyle.Bold);

            dgvLog.RowTemplate.Height = 30;
            dgvLog.EnableHeadersVisualStyles = false;
            dgvLog.ColumnHeadersDefaultCellStyle.BackColor = System.Drawing.Color.WhiteSmoke;
            dgvLog.Font = new System.Drawing.Font("Microsoft JhengHei UI", 10.5F);
            dgvLog.ColumnHeadersDefaultCellStyle.Font = new System.Drawing.Font("Microsoft JhengHei UI", 10.5F, System.Drawing.FontStyle.Bold);

            EnsureUserActionButtons();
            InitializeEventHandlers();
        }

        private void InitializeEventHandlers()
        {
            // 漫畫管理事件
            this.btnAddComic.Click += new EventHandler(this.btnAddComic_Click);
            this.btnEditComic.Click += new EventHandler(this.btnEditComic_Click);
            this.btnDeleteComic.Click += new EventHandler(this.btnDeleteComic_Click);
            this.btnComicSearch.Click += new EventHandler(this.btnComicSearch_Click);
            this.btnRefreshComic.Click += async (s, e) => await RefreshComicRecordsAsync();
            this.btnComicPrev.Click += new EventHandler(this.btnComicPrev_Click);
            this.btnComicNext.Click += new EventHandler(this.btnComicNext_Click);

            // 用戶管理事件
            this.btnAddUser.Click += new EventHandler(this.btnAddUser_Click);
            this.btnEditUser.Click += new EventHandler(this.btnEditUser_Click);
            this.btnDeleteUser.Click += new EventHandler(this.btnDeleteUser_Click);
            this.btnUserSearch.Click += new EventHandler(this.btnUserSearch_Click);
            this.btnRefreshUser.Click += async (s, e) => await RefreshUserRecordsAsync();

            // 借閱紀錄事件
            this.btnBorrowSearch.Click += new EventHandler(this.btnBorrowSearch_Click);
            this.btnRefreshBorrow.Click += async (s, e) => await RefreshBorrowRecordsAsync();
            this.btnBorrowPrev.Click += async (s, e) => { currentBorrowPage -= 1; await RefreshBorrowRecordsAsync(); };
            this.btnBorrowNext.Click += async (s, e) => { currentBorrowPage += 1; await RefreshBorrowRecordsAsync(); };

            // 預約紀錄事件
            this.btnReserveSearch.Click += new EventHandler(this.btnReserveSearch_Click);
            this.btnRefreshReserve.Click += async (s, e) => await RefreshReserveRecordsAsync();
            this.btnReservePrev.Click += async (s, e) => { currentReservePage -= 1; await RefreshReserveRecordsAsync(); };
            this.btnReserveNext.Click += async (s, e) => { currentReservePage += 1; await RefreshReserveRecordsAsync(); };

            // 管理日誌事件
            this.btnLogSearch.Click += new EventHandler(this.btnLogSearch_Click);
            this.btnRefreshLog.Click += async (s, e) => await RefreshLogRecordsAsync();

            // DataGridView 事件
            this.dgvComic.CellContentClick += new DataGridViewCellEventHandler(this.dgvComic_CellContentClick);
            this.dgvUser.CellContentClick += new DataGridViewCellEventHandler(this.dgvUser_CellContentClick);
            this.dgvBorrow.CellContentClick += new DataGridViewCellEventHandler(this.dgvBorrow_CellContentClick);
            this.dgvReserve.CellContentClick += new DataGridViewCellEventHandler(this.dgvReserve_CellContentClick);

            // 表單載入事件
            this.Load += async (s, e) =>
            {
                await RefreshUserRecordsAsync();
                await RefreshComicRecordsAsync();
                currentBorrowPage = 1;
                currentReservePage = 1;
                await RefreshBorrowRecordsAsync();
                await RefreshReserveRecordsAsync();

                // 載入管理日誌
                await RefreshLogRecordsAsync();
            };

            // 註冊管理日誌分頁按鈕事件
            this.btnLogPrev.Click += btnLogPrev_Click;
            this.btnLogNext.Click += btnLogNext_Click;
        }

        // 新增漫畫按鈕點擊事件處理器
        private async void btnAddComic_Click(object sender, EventArgs e)
        {
            using (AddComicForm addComicForm = new AddComicForm())
            {
                if (addComicForm.ShowDialog() == DialogResult.OK)
                {
                    string title = addComicForm.ComicTitle;
                    string isbn = addComicForm.ComicISBN;
                    string author = addComicForm.ComicAuthor;
                    string publisher = addComicForm.ComicPublisher;
                    string category = addComicForm.ComicCategory;
                    string imageUrl = addComicForm.ImageUrl;
                    string offerDate = addComicForm.OfferDate; // 獲取發售日
                    string pages = addComicForm.Pages; // 獲取頁數
                    string bookSummary = addComicForm.BookSummary; // 獲取摘要

                    try
                    {
                        string sql = "INSERT INTO comic (title, isbn, author, publisher, category, image_path, offer_date, pages, book_summary) " + // 添加新欄位
                                     "VALUES (@title, @isbn, @author, @publisher, @category, @image_path, @offer_date, @pages, @book_summary)"; // 添加新參數
                        MySqlParameter[] parameters = {
                            new MySqlParameter("@title", title),
                            new MySqlParameter("@isbn", isbn),
                            new MySqlParameter("@author", author),
                            new MySqlParameter("@publisher", publisher),
                            new MySqlParameter("@category", category),
                            new MySqlParameter("@image_path", imageUrl ?? (object)DBNull.Value),
                            new MySqlParameter("@offer_date", offerDate ?? (object)DBNull.Value), // 添加發售日參數
                            new MySqlParameter("@pages", pages ?? (object)DBNull.Value), // 添加頁數參數
                            new MySqlParameter("@book_summary", bookSummary ?? (object)DBNull.Value) // 添加摘要參數
                        };
                        int rowsAffected = DBHelper.ExecuteNonQuery(sql, parameters);
                        if (rowsAffected > 0)
                        {
                            MessageBox.Show("漫畫新增成功！", "成功", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            await RefreshComicRecordsAsync();
                            int newComicId = 0;
                            using (var conn = new MySql.Data.MySqlClient.MySqlConnection(DBHelper.GetConnectionString()))
                            {
                                conn.Open();
                                using (var cmd = new MySql.Data.MySqlClient.MySqlCommand("SELECT LAST_INSERT_ID()", conn))
                                {
                                    newComicId = Convert.ToInt32(cmd.ExecuteScalar());
                                }
                            }
                            WriteLogEntry("新增漫畫", $"新增了漫畫id:{newComicId}的漫畫");
                            await RefreshLogRecordsAsync();
                        }
                        else
                        {
                            MessageBox.Show("漫畫新增失敗，請稍後再試。", "失敗", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("新增漫畫時發生錯誤：" + ex.Message, "錯誤", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private async void AdminForm_Load(object sender, System.EventArgs e)
        {
            try
            {
                // 初始化載入時，不使用分頁
                await RefreshUserRecordsAsync(applyPagination: false);
                await RefreshComicRecordsAsync(applyPagination: false);

                currentBorrowPage = 1;
                currentReservePage = 1;
                await RefreshBorrowRecordsAsync();
                await RefreshReserveRecordsAsync();

                // 載入管理日誌
                await RefreshLogRecordsAsync();
            }
            catch (Exception ex)
            {
                MessageBox.Show("載入資料時發生錯誤：" + ex.Message, "錯誤", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // 新增用戶
        private async void btnAddUser_Click(object sender, System.EventArgs e)
        {
            using (AddUserForm addUserForm = new AddUserForm())
            {
                if (addUserForm.ShowDialog() == DialogResult.OK)
                {
                    string username = addUserForm.Username;
                    string password = addUserForm.Password;
                    if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
                    {
                        MessageBox.Show("請輸入帳號與密碼！", "錯誤", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                    try
                    {
                        string sql = "INSERT INTO user (username, password_hash, role) VALUES (@username, @password, 'user')";
                        MySqlParameter[] p = {
                            new MySqlParameter("@username", username),
                            new MySqlParameter("@password", password)
                        };
                        int rows = DBHelper.ExecuteNonQuery(sql, p);
                        if (rows > 0)
                        {
                            MessageBox.Show($"新增用戶成功！帳號：{username} (資料庫：{GetCurrentDatabaseName()})", "成功", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            await RefreshUserRecordsAsync();
                            int newUserId = GetInsertedUserId(username);
                            WriteLogEntry("新增用戶", $"新增了uid:{newUserId}的用戶");
                            await RefreshLogRecordsAsync();
                        }
                        else
                        {
                            MessageBox.Show($"新增用戶失敗，請檢查帳號是否重複或資料庫限制。\nSQL: {sql}\n帳號: {username}", "錯誤", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                    catch (System.Exception ex)
                    {
                        MessageBox.Show($"新增用戶時發生錯誤：{ex.Message}\nSQL: INSERT INTO user (username, password_hash, role) VALUES (@username, @password, 'user')\n帳號: {username}", "錯誤", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        // 取得目前資料庫名稱（for debug）
        private string GetCurrentDatabaseName()
        {
            try
            {
                using (var conn = new MySql.Data.MySqlClient.MySqlConnection(DBHelper.GetConnectionString()))
                {
                    conn.Open();
                    return conn.Database;
                }
            }
            catch { return "未知"; }
        }

        // 刪除用戶
        private async void btnDeleteUser_Click(object sender, System.EventArgs e)
        {
            if (dgvUser.CurrentRow == null)
            {
                MessageBox.Show("請先選擇要刪除的用戶！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            int userId = int.Parse(dgvUser.CurrentRow.Cells["用戶ID"].Value.ToString());
            string role = dgvUser.CurrentRow.Cells["角色"].Value.ToString();
            if (userId == currentUserId)
            {
                MessageBox.Show("不能刪除自己！", "錯誤", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (role == "admin")
            {
                MessageBox.Show("不能刪除管理員帳號！", "錯誤", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (MessageBox.Show("確定要刪除此用戶？", "確認", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                try
                {
                    // 查詢原始資料
                    string sqlQuery = "SELECT user_id, username FROM user WHERE user_id = @uid";
                    MySqlParameter[] pQuery = { new MySqlParameter("@uid", userId) };
                    var dt = DBHelper.ExecuteQuery(sqlQuery, pQuery);
                    string deletedUserName = "";
                    if (dt.Rows.Count > 0)
                    {
                        var row = dt.Rows[0];
                        deletedUserName = row["username"].ToString();
                    }
                    // 先刪除該用戶在 borrow_record 表中的相關記錄
                    string sqlDeleteBorrow = "DELETE FROM borrow_record WHERE user_id = @uid";
                    MySqlParameter[] pBorrow = { new MySqlParameter("@uid", userId) };
                    DBHelper.ExecuteNonQuery(sqlDeleteBorrow, pBorrow);
                    // 再刪除該用戶在 reservation 表中的相關記錄 (如果存在)
                    string sqlDeleteReserve = "DELETE FROM reservation WHERE user_id = @uid";
                    MySqlParameter[] pReserve = { new MySqlParameter("@uid", userId) };
                    DBHelper.ExecuteNonQuery(sqlDeleteReserve, pReserve);
                    // 最後刪除用戶記錄
                    string sql = "DELETE FROM user WHERE user_id = @uid";
                    MySqlParameter[] p = { new MySqlParameter("@uid", userId) };
                    int rows = DBHelper.ExecuteNonQuery(sql, p);
                    if (rows > 0)
                    {
                        MessageBox.Show("刪除成功！", "成功", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        WriteLogEntry("刪除用戶", $"刪除了uid:{userId}的用戶");
                        await RefreshLogRecordsAsync();
                        AdminForm_Load(null, null);
                    }
                    else
                    {
                        MessageBox.Show("刪除失敗。", "錯誤", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                catch (System.Exception ex)
                {
                    MessageBox.Show("刪除用戶時發生錯誤：" + ex.Message, "錯誤", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        // 用戶管理分頁
        private async Task RefreshUserRecordsAsync(
            string keyword = null,
            string searchType = "用戶名", // 新增 searchType 參數，預設用戶名
            bool applyPagination = true,
            int page = 1)
        {
            try
            {
                string sql = "SELECT user_id AS 用戶ID, username AS 用戶名, role AS 角色, " +
                           "CASE WHEN status = 'active' THEN '正常' " +
                           "WHEN status = 'frozen' THEN '凍結' " +
                           "ELSE status END AS 狀態 " +
                           "FROM user WHERE 1=1";
                var paramList = new List<MySqlParameter>();
                if (!string.IsNullOrWhiteSpace(keyword))
                {
                    if (searchType == "用戶ID")
                    {
                        if (int.TryParse(keyword, out int userId))
                        {
                            sql += " AND user_id = @userId";
                            paramList.Add(new MySqlParameter("@userId", userId));
                        }
                        else
                        {
                            sql += " AND 1 = 0";
                        }
                    }
                    else
                    {
                        sql += " AND username LIKE @keyword";
                        paramList.Add(new MySqlParameter("@keyword", "%" + keyword + "%"));
                    }
                }
                sql += " ORDER BY user_id";
                // 分頁
                sql += " LIMIT @offset, @pageSize";
                paramList.Add(new MySqlParameter("@offset", (currentUserPage - 1) * PageSize));
                paramList.Add(new MySqlParameter("@pageSize", PageSize));
                var dt = await Task.Run(() => DBHelper.ExecuteQuery(sql, paramList.ToArray()));
                this.Invoke((MethodInvoker)delegate
                {
                    dgvUser.DataSource = null;
                    dgvUser.DataSource = dt;
                    SetUserGridColumnWidths();
                    dgvUser.Refresh();
                    // 更新分頁標籤
                    lblUserPage.Text = $"第 {currentUserPage} 頁";
                    // 查詢總數以決定按鈕狀態
                    string countSql = "SELECT COUNT(*) FROM user WHERE 1=1";
                    if (!string.IsNullOrWhiteSpace(keyword))
                    {
                        if (searchType == "用戶ID")
                        {
                            if (int.TryParse(keyword, out int userId))
                            {
                                countSql += " AND user_id = @userId";
                            }
                        }
                        else
                        {
                            countSql += " AND username LIKE @keyword";
                        }
                    }
                    long totalRecords = Convert.ToInt64(DBHelper.ExecuteScalar(countSql, paramList.ToArray()));
                    btnUserPrev.Enabled = currentUserPage > 1;
                    btnUserNext.Enabled = (currentUserPage * PageSize) < totalRecords;
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show($"刷新用戶資料時發生錯誤：{ex.Message}", "錯誤", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // 用戶搜尋按鈕點擊事件
        private async void btnUserSearch_Click(object sender, EventArgs e)
        {
            // currentUserSearchKeyword = txtSearchUser.Text.Trim(); // 更新當前搜尋關鍵字 - Removed
            string searchTerm = txtSearchUser.Text.Trim();
            string searchType = cmbUserSearchType.SelectedItem?.ToString() ?? "用戶名"; // 預設用戶名搜尋
            // currentUserSearchType = searchType; // 更新當前搜尋類型 (如果需要儲存以便刷新) - Removed
            // currentUserPage = 1; // 重置到第一頁 - Removed

            // 呼叫修改後的 RefreshUserRecordsAsync 方法
            await RefreshUserRecordsAsync(keyword: searchTerm, searchType: searchType, applyPagination: false);
        }

        private async void btnRefreshUser_Click(object sender, EventArgs e)
        {
            // 直接使用當前的搜尋條件和頁碼刷新 - Reverted to original behavior
            // This now just refreshes the list, effectively clearing any search filter
            await RefreshUserRecordsAsync(applyPagination: false);
        }

        private async void btnComicSearch_Click(object sender, EventArgs e)
        {
            currentComicSearchKeyword = txtComicKeyword.Text.Trim();
            currentComicSearchType = cmbComicSearchType.SelectedItem?.ToString() ?? "書號";
            currentComicPage = 1;
            await RefreshComicRecordsAsync(keyword: currentComicSearchKeyword, searchType: currentComicSearchType);
        }

        private async void btnBorrowSearch_Click(object sender, EventArgs e)
        {
            currentBorrowSearchKeyword = txtBorrowKeyword.Text.Trim();
            currentBorrowSearchType = cmbBorrowSearchType.SelectedItem?.ToString() ?? "用戶";
            currentBorrowPage = 1; // 重置到第一頁
            await RefreshBorrowRecordsAsync();
        }

        private async void btnReserveSearch_Click(object sender, EventArgs e)
        {
            currentReserveSearchKeyword = txtReserveKeyword.Text.Trim();
            currentReserveSearchType = cmbReserveSearchType.SelectedItem?.ToString() ?? "用戶";
            currentReservePage = 1;
            await RefreshReserveRecordsAsync();
        }

        private async void dgvComic_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && dgvComic.Columns[e.ColumnIndex].Name == "Delete")
            {
                var comicId = dgvComic.Rows[e.RowIndex].Cells["書號"].Value.ToString();
                var title = dgvComic.Rows[e.RowIndex].Cells["書名"].Value.ToString();
                if (MessageBox.Show($"確定要刪除漫畫「{title}」嗎？", "確認刪除", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    try
                    {
                        string sql = "DELETE FROM comic WHERE comic_id = @id";
                        var param = new MySql.Data.MySqlClient.MySqlParameter("@id", comicId);
                        int rows = DBHelper.ExecuteNonQuery(sql, new[] { param });
                        if (rows > 0)
                        {
                            MessageBox.Show("刪除成功！", "成功", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            btnComicSearch_Click(null, null); // 重新查詢刷新
                            WriteLogEntry("刪除漫畫", $"{title}漫畫被刪除");
                            await RefreshLogRecordsAsync(); // 刷新日誌列表
                        }
                        else
                        {
                            MessageBox.Show("刪除失敗。", "錯誤", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("刪除漫畫時發生錯誤：" + ex.Message, "錯誤", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private async void btnDeleteComic_Click(object sender, System.EventArgs e)
        {
            if (dgvComic.CurrentRow == null)
            {
                MessageBox.Show("請先選擇要刪除的漫畫！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            int comicId = int.Parse(dgvComic.CurrentRow.Cells["書號"].Value.ToString());
            if (MessageBox.Show($"確定要刪除書號 {comicId} 的漫畫？", "確認", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                try
                {
                    // 查詢原始資料
                    string sqlQuery = "SELECT isbn, title, author, publisher, category, image_path, offer_date, pages, book_summary FROM comic WHERE comic_id = @cid";
                    MySqlParameter[] pQuery = { new MySqlParameter("@cid", comicId) };
                    var dt = DBHelper.ExecuteQuery(sqlQuery, pQuery);
                    string oldIsbn = "", oldTitle = "", oldAuthor = "", oldPublisher = "", oldCategory = "", oldImagePath = "", oldOfferDate = null, oldPages = null, oldBookSummary = null;
                    if (dt.Rows.Count > 0)
                    {
                        var row = dt.Rows[0];
                        oldIsbn = row["isbn"].ToString();
                        oldTitle = row["title"].ToString();
                        oldAuthor = row["author"].ToString();
                        oldPublisher = row["publisher"].ToString();
                        oldCategory = row["category"].ToString();
                        oldImagePath = row["image_path"]?.ToString();
                        oldOfferDate = row["offer_date"]?.ToString(); // 獲取舊的發售日
                        oldPages = row["pages"]?.ToString(); // 獲取舊的頁數
                        oldBookSummary = row["book_summary"]?.ToString(); // 獲取舊的摘要
                    }
                    string sql = "DELETE FROM comic WHERE comic_id = @cid";
                    MySqlParameter[] p = { new MySqlParameter("@cid", comicId) };
                    int rows = DBHelper.ExecuteNonQuery(sql, p);
                    if (rows > 0)
                    {
                        MessageBox.Show("刪除成功！", "成功", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        WriteLogEntry("刪除漫畫", $"刪除了漫畫id:{comicId}的漫畫");
                        await RefreshLogRecordsAsync();
                        RefreshComicRecordsAsync();
                    }
                    else
                    {
                        MessageBox.Show("刪除失敗。", "錯誤", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                catch (System.Exception ex)
                {
                    MessageBox.Show("刪除漫畫時發生錯誤：" + ex.Message, "錯誤", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        // 用戶管理分頁操作按鈕事件
        private void dgvUser_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;
            var colName = dgvUser.Columns[e.ColumnIndex].Name;
            var userId = dgvUser.Rows[e.RowIndex].Cells["用戶ID"].Value.ToString();
            var username = dgvUser.Rows[e.RowIndex].Cells["用戶名"].Value.ToString();
            if (colName == "查看借閱紀錄")
            {
                dgvBorrow.DataSource = null; // 先清空，避免閃現舊資料
                tabAdmin.SelectedTab = tabBorrow;
                txtBorrowKeyword.Text = username;
                cmbBorrowSearchType.SelectedItem = "用戶";
                btnBorrowSearch.PerformClick();
            }
            else if (colName == "查看預約紀錄")
            {
                dgvReserve.DataSource = null; // 先清空，避免閃現舊資料
                tabAdmin.SelectedTab = tabReserve;
                txtReserveKeyword.Text = username;
                cmbReserveSearchType.SelectedItem = "用戶";
                btnReserveSearch.PerformClick();
            }
        }

        // 借閱紀錄分頁操作按鈕事件
        private async void dgvBorrow_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;
            var colName = dgvBorrow.Columns[e.ColumnIndex].Name;
            if (colName == "還書")
            {
                var borrowId = dgvBorrow.Rows[e.RowIndex].Cells["編號"].Value.ToString();
                var returnDateCell = dgvBorrow.Rows[e.RowIndex].Cells["歸還日期"];
                if (returnDateCell.Value != null && !string.IsNullOrWhiteSpace(returnDateCell.Value.ToString()))
                {
                    MessageBox.Show("此紀錄已還書。", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                if (MessageBox.Show("確定要標記為已還書嗎？", "確認", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    string sql = "UPDATE borrow_record SET return_date = @now WHERE borrow_id = @id";
                    var p = new MySqlParameter[] {
                        new MySqlParameter("@now", DateTime.Now),
                        new MySqlParameter("@id", borrowId)
                    };
                    DBHelper.ExecuteNonQuery(sql, p);
                    MessageBox.Show("已標記為已還書！", "成功", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    var borrowRow = dgvBorrow.Rows[e.RowIndex];
                    string userName = borrowRow.Cells["用戶"].Value.ToString();
                    string comicTitle = borrowRow.Cells["書名"].Value.ToString();
                    WriteLogEntry("歸還漫畫", $"用戶={userName}, 書名={comicTitle}");
                    AdminForm_Load(null, null);
                    await RefreshLogRecordsAsync(); // 刷新日誌列表
                }
            }
        }

        // 預約紀錄分頁操作按鈕事件
        private async void dgvReserve_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;
            var colName = dgvReserve.Columns[e.ColumnIndex].Name;
            if (colName == "取消預約")
            {
                try
                {
                    var reserveIdObj = dgvReserve.Rows[e.RowIndex].Cells["編號"].Value;
                    if (reserveIdObj == null || !int.TryParse(reserveIdObj.ToString(), out int reserveId))
                    {
                        MessageBox.Show("預約編號無效，無法取消。", "錯誤", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                    if (MessageBox.Show("確定要取消此預約嗎？", "確認", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    {
                        string sql = "DELETE FROM reservation WHERE reservation_id = @id";
                        var p = new MySqlParameter[] {
                            new MySqlParameter("@id", reserveId)
                        };
                        DBHelper.ExecuteNonQuery(sql, p);
                        MessageBox.Show("已取消預約！", "成功", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        var reserveRow = dgvReserve.Rows[e.RowIndex];
                        string userName = reserveRow.Cells["用戶"].Value.ToString();
                        string comicTitle = reserveRow.Cells["書名"].Value.ToString();
                        WriteLogEntry("取消預約", $"用戶={userName}, 書名={comicTitle}");
                        AdminForm_Load(null, null);
                        await RefreshLogRecordsAsync(); // 刷新日誌列表
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("取消預約時發生錯誤：" + ex.Message, "錯誤", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        // 新增借書冷卻期檢查方法
        private bool IsUserInCoolingPeriod(int userId, int comicId)
        {
            string sql = @"
                SELECT COUNT(*) 
                FROM borrow_record 
                WHERE user_id = @userId 
                AND comic_id = @comicId 
                AND return_date IS NOT NULL 
                AND return_date > DATE_SUB(NOW(), INTERVAL 24 HOUR)";

            MySqlParameter[] parameters = {
                new MySqlParameter("@userId", userId),
                new MySqlParameter("@comicId", comicId)
            };

            int count = Convert.ToInt32(DBHelper.ExecuteScalar(sql, parameters));
            return count > 0;
        }

        // 新增預約冷卻期檢查方法
        private bool IsUserInReservationCoolingPeriod(int userId, int comicId)
        {
            string sql = @"
                SELECT COUNT(*) 
                FROM reservation 
                WHERE user_id = @userId 
                AND comic_id = @comicId 
                AND reservation_date > DATE_SUB(NOW(), INTERVAL 24 HOUR)";

            MySqlParameter[] parameters = {
                new MySqlParameter("@userId", userId),
                new MySqlParameter("@comicId", comicId)
            };

            int count = Convert.ToInt32(DBHelper.ExecuteScalar(sql, parameters));
            return count > 0;
        }

        // 修改預約按鈕點擊事件，加入借書冷卻期與預約冷卻期檢查
        private void btnReserve_Click(object sender, EventArgs e)
        {
            // 用戶自己預約時的邏輯
            int userId = this.currentUserId;
            if (dgvComic.CurrentRow == null)
            {
                MessageBox.Show("請先選擇要預約的漫畫！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            int comicId = Convert.ToInt32(dgvComic.CurrentRow.Cells["書號"].Value);

            if (IsUserInCoolingPeriod(userId, comicId))
            {
                MessageBox.Show("您處於借書冷卻期，無法預約此漫畫。", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (IsUserInReservationCoolingPeriod(userId, comicId))
            {
                MessageBox.Show("您處於預約冷卻期，無法預約此漫畫。", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // 繼續預約邏輯...
        }

        private async Task RefreshBorrowRecordsAsync()
        {
            try
            {
                string sql = @"SELECT b.borrow_id AS 編號, u.username AS 用戶, 
                             c.title AS 書名, c.isbn AS ISBN, 
                             b.borrow_date AS 借閱日期, b.return_date AS 歸還日期,
                             CASE WHEN b.return_date IS NOT NULL THEN '已歸還' ELSE '未歸還' END AS 狀態
                             FROM borrow_record b
                             JOIN user u ON b.user_id = u.user_id
                             JOIN comic c ON b.comic_id = c.comic_id
                             WHERE 1=1";
                var paramList = new List<MySqlParameter>();

                if (!string.IsNullOrWhiteSpace(currentBorrowSearchKeyword))
                {
                    string field = currentBorrowSearchType switch
                    {
                        "用戶" => "u.username",
                        "書名" => "c.title",
                        "ISBN" => "c.isbn",
                        _ => "u.username"
                    };
                    if (currentBorrowSearchType == "用戶")
                    {
                        sql += $" AND {field} = @keyword";
                        paramList.Add(new MySqlParameter("@keyword", currentBorrowSearchKeyword));
                    }
                    else
                    {
                        sql += $" AND {field} LIKE @keyword";
                        paramList.Add(new MySqlParameter("@keyword", "%" + currentBorrowSearchKeyword + "%"));
                    }
                }

                // 重新構造用於計數的 SQL，只包含 FROM, JOIN, WHERE
                string countSql = @"SELECT COUNT(*)
                                    FROM borrow_record b
                                    JOIN user u ON b.user_id = u.user_id
                                    JOIN comic c ON b.comic_id = c.comic_id
                                    WHERE 1=1";
                // 將搜尋條件也應用到計數 SQL
                if (!string.IsNullOrWhiteSpace(currentBorrowSearchKeyword))
                {
                    string field = currentBorrowSearchType switch
                    {
                        "用戶" => "u.username",
                        "書名" => "c.title",
                        "ISBN" => "c.isbn",
                        _ => "u.username"
                    };
                    countSql += $" AND {field} LIKE @keyword";
                    // 參數列表已經包含 @keyword，無需重複添加
                }

                long totalRecords = Convert.ToInt64(DBHelper.ExecuteScalar(countSql, paramList.ToArray()));

                // 添加分頁參數到主查詢
                sql += " ORDER BY b.borrow_date DESC LIMIT @offset, @pageSize";
                paramList.Add(new MySqlParameter("@offset", (currentBorrowPage - 1) * PageSize));
                paramList.Add(new MySqlParameter("@pageSize", PageSize));

                var dt = await Task.Run(() => DBHelper.ExecuteQuery(sql, paramList.ToArray()));

                this.Invoke((MethodInvoker)delegate
                {
                    dgvBorrow.DataSource = null;
                    dgvBorrow.DataSource = dt;
                    SetBorrowGridColumnWidths();
                    dgvBorrow.Refresh();
                    // 更新分頁控制項
                    lblBorrowPage.Text = $"第 {currentBorrowPage} 頁";
                    btnBorrowPrev.Enabled = currentBorrowPage > 1;
                    btnBorrowNext.Enabled = (currentBorrowPage * PageSize) < totalRecords;
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show($"刷新借閱紀錄時發生錯誤：{ex.Message}", "錯誤", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private async Task RefreshReserveRecordsAsync()
        {
            try
            {
                string sql = @"SELECT r.reservation_id AS 編號, u.username AS 用戶, 
                             c.title AS 書名, c.isbn AS ISBN, 
                             r.reservation_date AS 預約日期, 
                             DATE_ADD(r.reservation_date, INTERVAL 1 DAY) AS 預約到期時間, 
                             CASE 
                                 -- 檢查是否有對應的借閱記錄（表示已完成預約）
                                 WHEN br.borrow_id IS NOT NULL AND br.borrow_date >= r.reservation_date THEN '已完成'
                                 WHEN r.status = 'active' AND r.reservation_date > DATE_SUB(NOW(), INTERVAL 24 HOUR) THEN '預約中'
                                 WHEN r.status = 'canceled' THEN '已取消'
                                 WHEN r.reservation_date <= DATE_SUB(NOW(), INTERVAL 24 HOUR) THEN '已過期'
                                 ELSE r.status -- 保留其他未知狀態
                             END AS 狀態
                             FROM reservation r
                             JOIN user u ON r.user_id = u.user_id
                             JOIN comic c ON r.comic_id = c.comic_id
                             LEFT JOIN borrow_record br ON r.user_id = br.user_id AND r.comic_id = br.comic_id AND br.borrow_date >= r.reservation_date
                             WHERE 1=1";
                var paramList = new List<MySqlParameter>();

                if (!string.IsNullOrWhiteSpace(currentReserveSearchKeyword))
                {
                    string field = currentReserveSearchType switch
                    {
                        "用戶" => "u.username",
                        "書名" => "c.title",
                        "ISBN" => "c.isbn",
                        _ => "u.username"
                    };
                    if (currentReserveSearchType == "用戶")
                    {
                        sql += $" AND {field} = @keyword";
                        paramList.Add(new MySqlParameter("@keyword", currentReserveSearchKeyword));
                    }
                    else
                    {
                        sql += $" AND {field} LIKE @keyword";
                        paramList.Add(new MySqlParameter("@keyword", "%" + currentReserveSearchKeyword + "%"));
                    }
                }

                // 重新構造用於計數的 SQL，只包含 FROM, JOIN, WHERE
                string countSql = @"SELECT COUNT(*)
                                    FROM reservation r
                                    JOIN user u ON r.user_id = u.user_id
                                    JOIN comic c ON r.comic_id = c.comic_id
                                    WHERE 1=1";
                // 將搜尋條件也應用到計數 SQL
                if (!string.IsNullOrWhiteSpace(currentReserveSearchKeyword))
                {
                    string field = currentReserveSearchType switch
                    {
                        "用戶" => "u.username",
                        "書名" => "c.title",
                        "ISBN" => "c.isbn",
                        _ => "u.username"
                    };
                    countSql += $" AND {field} LIKE @keyword";
                    // 參數列表已經包含 @keyword，無需重複添加
                }

                long totalRecords = Convert.ToInt64(DBHelper.ExecuteScalar(countSql, paramList.ToArray()));

                // 添加分頁參數到主查詢
                sql += " ORDER BY r.reservation_date DESC LIMIT @offset, @pageSize";
                paramList.Add(new MySqlParameter("@offset", (currentReservePage - 1) * PageSize));
                paramList.Add(new MySqlParameter("@pageSize", PageSize));

                var dt = await Task.Run(() => DBHelper.ExecuteQuery(sql, paramList.ToArray()));

                this.Invoke((MethodInvoker)delegate
                {
                    dgvReserve.DataSource = null;
                    dgvReserve.DataSource = dt;
                    SetReserveGridColumnWidths();
                    dgvReserve.Refresh();
                    // 更新分頁控制項
                    lblReservePage.Text = $"第 {currentReservePage} 頁";
                    btnReservePrev.Enabled = currentReservePage > 1;
                    btnReserveNext.Enabled = (currentReservePage * PageSize) < totalRecords;
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show($"刷新預約紀錄時發生錯誤：{ex.Message}", "錯誤", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async Task InitializeAllDataAsync()
        {
            // 此方法不再用於 AdminForm_Load，但可能在其他地方被呼叫，暫時保留，但其邏輯已轉移
            try
            {
                await Task.WhenAll(
                    RefreshUserRecordsAsync(),
                    RefreshComicRecordsAsync(),
                    RefreshBorrowRecordsAsync(),
                    RefreshReserveRecordsAsync()
                );
            }
            catch (Exception ex)
            {
                MessageBox.Show($"初始化資料時發生錯誤：{ex.Message}", "錯誤", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        #region 通用方法
        private void UpdatePagingControls(DataGridView dgv, int currentPage, Button btnPrev, Button btnNext, Label lblPage)
        {
            lblPage.Text = $"第 {currentPage} 頁";
            btnPrev.Enabled = currentPage > 1;
            btnNext.Enabled = dgv.Rows.Count == PageSize; // 根據返回的記錄數判斷是否有下一頁
        }

        private void ResetSearchParameters()
        {
            // currentUserSearchKeyword = ""; // Removed as it's no longer used to store state
            currentComicSearchKeyword = "";
            currentBorrowSearchKeyword = "";
            currentReserveSearchKeyword = "";
            currentUserPage = 1;
            currentComicPage = 1;
            currentBorrowPage = 1;
            currentReservePage = 1;
        }

        private void EnsureUserActionButtons()
        {
            // 避免重複加入
            if (dgvUser.Columns["查看借閱紀錄"] == null)
            {
                var btnBorrow = new DataGridViewButtonColumn();
                btnBorrow.Name = "查看借閱紀錄";
                btnBorrow.HeaderText = "操作";
                btnBorrow.Text = "查看借閱紀錄";
                btnBorrow.UseColumnTextForButtonValue = true;
                dgvUser.Columns.Add(btnBorrow);
            }
            if (dgvUser.Columns["查看預約紀錄"] == null)
            {
                var btnReserve = new DataGridViewButtonColumn();
                btnReserve.Name = "查看預約紀錄";
                btnReserve.HeaderText = "操作";
                btnReserve.Text = "查看預約紀錄";
                btnReserve.UseColumnTextForButtonValue = true;
                dgvUser.Columns.Add(btnReserve);
            }
        }

        private void SetUserGridColumnWidths()
        {
            if (dgvUser.Columns.Contains("用戶ID"))
            {
                dgvUser.Columns["用戶ID"].Width = 60;
                dgvUser.Columns["用戶ID"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
                dgvUser.Columns["用戶ID"].DisplayIndex = 0;
            }
            if (dgvUser.Columns.Contains("用戶名"))
            {
                dgvUser.Columns["用戶名"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                dgvUser.Columns["用戶名"].DisplayIndex = 1;
            }
            if (dgvUser.Columns.Contains("角色"))
            {
                dgvUser.Columns["角色"].Width = 80;
                dgvUser.Columns["角色"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
                dgvUser.Columns["角色"].DisplayIndex = 2;
            }
            if (dgvUser.Columns.Contains("狀態"))
            {
                dgvUser.Columns["狀態"].Width = 80;
                dgvUser.Columns["狀態"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
                dgvUser.Columns["狀態"].DisplayIndex = 3;
            }
            if (dgvUser.Columns.Contains("查看借閱紀錄"))
            {
                dgvUser.Columns["查看借閱紀錄"].Width = 110;
                dgvUser.Columns["查看借閱紀錄"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
                dgvUser.Columns["查看借閱紀錄"].DisplayIndex = 4;
            }
            if (dgvUser.Columns.Contains("查看預約紀錄"))
            {
                dgvUser.Columns["查看預約紀錄"].Width = 110;
                dgvUser.Columns["查看預約紀錄"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
                dgvUser.Columns["查看預約紀錄"].DisplayIndex = 5;
            }
        }
        private void SetBorrowGridColumnWidths()
        {
            // 明確設定各欄位寬度，只讓書名自動填滿
            if (dgvBorrow.Columns.Contains("編號")) dgvBorrow.Columns["編號"].Width = 60;
            if (dgvBorrow.Columns.Contains("用戶")) dgvBorrow.Columns["用戶"].Width = 100;
            if (dgvBorrow.Columns.Contains("ISBN")) dgvBorrow.Columns["ISBN"].Width = 120; // 參考 UserForm 設定
            if (dgvBorrow.Columns.Contains("借閱日期")) dgvBorrow.Columns["借閱日期"].Width = 160;
            if (dgvBorrow.Columns.Contains("歸還日期")) dgvBorrow.Columns["歸還日期"].Width = 160;
            if (dgvBorrow.Columns.Contains("狀態")) dgvBorrow.Columns["狀態"].Width = 80;

            // 讓書名自動填滿剩餘空間
            if (dgvBorrow.Columns.Contains("書名"))
            {
                dgvBorrow.Columns["書名"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            }

            // 確保狀態欄位在最後
            if (dgvBorrow.Columns.Contains("狀態"))
            {
                dgvBorrow.Columns["狀態"].DisplayIndex = dgvBorrow.Columns.Count - 1;
            }

            // 將其他未明確設定寬度的欄位 AutoSizeMode 設為 None，防止擠壓
            foreach (DataGridViewColumn col in dgvBorrow.Columns)
            {
                if (!new[] { "編號", "用戶", "書名", "ISBN", "借閱日期", "歸還日期", "狀態" }.Contains(col.Name))
                {
                    col.AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
                }
            }
        }
        private void SetReserveGridColumnWidths()
        {
            // 明確設定各欄位寬度
            if (dgvReserve.Columns.Contains("編號")) dgvReserve.Columns["編號"].Width = 60;
            if (dgvReserve.Columns.Contains("用戶")) dgvReserve.Columns["用戶"].Width = 100;
            if (dgvReserve.Columns.Contains("ISBN")) dgvReserve.Columns["ISBN"].Width = 120;
            if (dgvReserve.Columns.Contains("預約日期")) dgvReserve.Columns["預約日期"].Width = 160;
            if (dgvReserve.Columns.Contains("預約到期時間")) dgvReserve.Columns["預約到期時間"].Width = 160;
            if (dgvReserve.Columns.Contains("狀態")) dgvReserve.Columns["狀態"].Width = 80;

            // 讓書名自動填滿剩餘空間
            if (dgvReserve.Columns.Contains("書名"))
            {
                dgvReserve.Columns["書名"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            }

            // 設定顯示順序
            if (dgvReserve.Columns.Contains("編號")) dgvReserve.Columns["編號"].DisplayIndex = 0;
            if (dgvReserve.Columns.Contains("用戶")) dgvReserve.Columns["用戶"].DisplayIndex = 1;
            if (dgvReserve.Columns.Contains("書名")) dgvReserve.Columns["書名"].DisplayIndex = 2;
            if (dgvReserve.Columns.Contains("ISBN")) dgvReserve.Columns["ISBN"].DisplayIndex = 3;
            if (dgvReserve.Columns.Contains("預約日期")) dgvReserve.Columns["預約日期"].DisplayIndex = 4;
            if (dgvReserve.Columns.Contains("預約到期時間")) dgvReserve.Columns["預約到期時間"].DisplayIndex = 5;
            if (dgvReserve.Columns.Contains("狀態")) dgvReserve.Columns["狀態"].DisplayIndex = 6;

            // 確保其他欄位沒有 AutoSizeMode.Fill
            foreach (DataGridViewColumn col in dgvReserve.Columns)
            {
                if (col.Name != "書名")
                {
                    col.AutoSizeMode = DataGridViewAutoSizeColumnMode.None; // 確保其他欄位不自動填充
                }
            }
        }
        private void SetComicGridColumnWidths()
        {
            // 設置固定寬度欄位
            if (dgvComic.Columns.Contains("書號"))
            {
                dgvComic.Columns["書號"].Width = 80;
                dgvComic.Columns["書號"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
                dgvComic.Columns["書號"].DisplayIndex = 0;
            }
            if (dgvComic.Columns.Contains("ISBN"))
            {
                dgvComic.Columns["ISBN"].Width = 120;
                dgvComic.Columns["ISBN"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
                dgvComic.Columns["ISBN"].DisplayIndex = 2;
            }

            // 設置自動填滿欄位
            if (dgvComic.Columns.Contains("書名"))
            {
                dgvComic.Columns["書名"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                dgvComic.Columns["書名"].DisplayIndex = 1;
            }
            if (dgvComic.Columns.Contains("作者"))
            {
                dgvComic.Columns["作者"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                dgvComic.Columns["作者"].DisplayIndex = 3;
            }
            if (dgvComic.Columns.Contains("出版社"))
            {
                dgvComic.Columns["出版社"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                dgvComic.Columns["出版社"].DisplayIndex = 4;
            }
            if (dgvComic.Columns.Contains("分類"))
            {
                dgvComic.Columns["分類"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                dgvComic.Columns["分類"].DisplayIndex = 5;
            }
        }
        #endregion

        // 修改漫畫按鈕點擊事件處理器
        private async void btnEditComic_Click(object sender, EventArgs e)
        {
            if (dgvComic.CurrentRow == null)
            {
                MessageBox.Show("請先選擇要修改的漫畫！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            int comicId = int.Parse(dgvComic.CurrentRow.Cells["書號"].Value.ToString());
            string isbn = dgvComic.CurrentRow.Cells["ISBN"].Value.ToString();
            string title = dgvComic.CurrentRow.Cells["書名"].Value.ToString();
            string author = dgvComic.CurrentRow.Cells["作者"].Value.ToString();
            string publisher = dgvComic.CurrentRow.Cells["出版社"].Value.ToString();
            string category = dgvComic.CurrentRow.Cells["分類"].Value.ToString();

            // 查詢原始資料
            string sqlQuery = "SELECT isbn, title, author, publisher, category, image_path, offer_date, pages, book_summary FROM comic WHERE comic_id = @cid";
            MySqlParameter[] pQuery = { new MySqlParameter("@cid", comicId) };
            var dt = DBHelper.ExecuteQuery(sqlQuery, pQuery);
            string oldIsbn = isbn, oldTitle = title, oldAuthor = author, oldPublisher = publisher, oldCategory = category, oldImagePath = null, oldOfferDate = null, oldPages = null, oldBookSummary = null;
            if (dt.Rows.Count > 0)
            {
                var row = dt.Rows[0];
                oldIsbn = row["isbn"].ToString();
                oldTitle = row["title"].ToString();
                oldAuthor = row["author"].ToString();
                oldPublisher = row["publisher"].ToString();
                oldCategory = row["category"].ToString();
                oldImagePath = row["image_path"]?.ToString();
                oldOfferDate = row["offer_date"]?.ToString(); // 獲取舊的發售日
                oldPages = row["pages"]?.ToString(); // 獲取舊的頁數
                oldBookSummary = row["book_summary"]?.ToString(); // 獲取舊的摘要
            }

            using (EditComicForm editComicForm = new EditComicForm(comicId, isbn, title, author, publisher, category, oldImagePath, oldOfferDate, oldPages, oldBookSummary))
            {
                if (editComicForm.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        string sql = @"UPDATE comic 
                                     SET isbn = @isbn, 
                                         title = @title, 
                                         author = @author, 
                                         publisher = @publisher, 
                                         category = @category,
                                         image_path = @image_path,
                                         offer_date = @offer_date,
                                         pages = @pages,
                                         book_summary = @book_summary
                                     WHERE comic_id = @comic_id";
                        MySqlParameter[] parameters = {
                            new MySqlParameter("@comic_id", comicId),
                            new MySqlParameter("@isbn", editComicForm.ComicISBN),
                            new MySqlParameter("@title", editComicForm.ComicTitle),
                            new MySqlParameter("@author", editComicForm.ComicAuthor),
                            new MySqlParameter("@publisher", editComicForm.ComicPublisher),
                            new MySqlParameter("@category", editComicForm.ComicCategory),
                            new MySqlParameter("@image_path", editComicForm.ImageUrl ?? (object)DBNull.Value),
                            new MySqlParameter("@offer_date", editComicForm.OfferDate ?? (object)DBNull.Value), // 添加發售日參數
                            new MySqlParameter("@pages", editComicForm.Pages ?? (object)DBNull.Value), // 添加頁數參數
                            new MySqlParameter("@book_summary", editComicForm.BookSummary ?? (object)DBNull.Value) // 添加摘要參數
                        };
                        int rowsAffected = DBHelper.ExecuteNonQuery(sql, parameters);
                        if (rowsAffected > 0)
                        {
                            MessageBox.Show("漫畫修改成功！", "成功", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            RefreshComicRecordsAsync();
                            List<string> changes = new List<string>();
                            if (oldTitle != editComicForm.ComicTitle)
                                changes.Add($"書名由{oldTitle}改為{editComicForm.ComicTitle}");
                            if (oldIsbn != editComicForm.ComicISBN)
                                changes.Add($"ISBN由{oldIsbn}改為{editComicForm.ComicISBN}");
                            if (oldAuthor != editComicForm.ComicAuthor)
                                changes.Add($"作者由{oldAuthor}改為{editComicForm.ComicAuthor}");
                            if (oldPublisher != editComicForm.ComicPublisher)
                                changes.Add($"出版社由{oldPublisher}改為{editComicForm.ComicPublisher}");
                            if (oldCategory != editComicForm.ComicCategory)
                                changes.Add($"分類由{oldCategory}改為{editComicForm.ComicCategory}");
                            if (oldImagePath != editComicForm.ImageUrl)
                                changes.Add($"更新了漫畫id:{comicId}的圖片");
                            if (oldOfferDate != editComicForm.OfferDate)
                                changes.Add($"發售日由{oldOfferDate}改為{editComicForm.OfferDate}");
                            if (oldPages != editComicForm.Pages)
                                changes.Add($"頁數由{oldPages}改為{editComicForm.Pages}");
                            if (oldBookSummary != editComicForm.BookSummary)
                                changes.Add($"摘要由{oldBookSummary}改為{editComicForm.BookSummary}");
                            if (changes.Count > 0)
                                WriteLogEntry("編輯漫畫", $"修改了漫畫id:{comicId}的漫畫，" + string.Join("，", changes));
                            await RefreshLogRecordsAsync();
                        }
                        else
                        {
                            MessageBox.Show("漫畫修改失敗，請稍後再試。", "失敗", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("修改漫畫時發生錯誤：" + ex.Message, "錯誤", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
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

        // 漫畫管理分頁
        private async Task RefreshComicRecordsAsync(
            string keyword = null,
            string searchType = "書號",
            bool applyPagination = true)
        {
            try
            {
                string sql = @"SELECT comic_id AS 書號, title AS 書名, isbn AS ISBN,
                             author AS 作者, publisher AS 出版社, category AS 分類
                             FROM comic WHERE 1=1";
                var paramList = new List<MySqlParameter>();
                if (!string.IsNullOrWhiteSpace(keyword))
                {
                    if (searchType == "書號" && int.TryParse(keyword, out int comicId))
                    {
                        sql += " AND comic_id = @comic_id";
                        paramList.Add(new MySqlParameter("@comic_id", comicId));
                    }
                    else
                    {
                        string field = searchType switch
                        {
                            "書名" => "title",
                            "ISBN" => "isbn",
                            "作者" => "author",
                            "出版社" => "publisher",
                            "分類" => "category",
                            _ => "comic_id"
                        };
                        sql += $" AND {field} LIKE @keyword";
                        paramList.Add(new MySqlParameter("@keyword", "%" + keyword + "%"));
                    }
                }
                sql += " ORDER BY comic_id";
                if (applyPagination)
                {
                    sql += " LIMIT @offset, @pageSize";
                    paramList.Add(new MySqlParameter("@offset", (currentComicPage - 1) * PageSize));
                    paramList.Add(new MySqlParameter("@pageSize", PageSize));
                }
                var dt = await Task.Run(() => DBHelper.ExecuteQuery(sql, paramList.ToArray()));

                this.Invoke((MethodInvoker)delegate
                {
                    dgvComic.DataSource = null;
                    dgvComic.DataSource = dt;
                    SetComicGridColumnWidths();
                    dgvComic.Refresh();
                    // 分頁控制
                    if (lblComicPage != null && btnComicPrev != null && btnComicNext != null)
                    {
                        string countSql = "SELECT COUNT(*) FROM comic WHERE 1=1";
                        if (!string.IsNullOrWhiteSpace(keyword))
                        {
                            if (searchType == "書號" && int.TryParse(keyword, out int comicId))
                            {
                                countSql += " AND comic_id = @comic_id";
                            }
                            else
                            {
                                string field = searchType switch
                                {
                                    "書名" => "title",
                                    "ISBN" => "isbn",
                                    "作者" => "author",
                                    "出版社" => "publisher",
                                    "分類" => "category",
                                    _ => "comic_id"
                                };
                                countSql += $" AND {field} LIKE @keyword";
                            }
                        }
                        long totalRecords = Convert.ToInt64(DBHelper.ExecuteScalar(countSql, paramList.ToArray()));
                        lblComicPage.Text = $"第 {currentComicPage} 頁";
                        btnComicPrev.Enabled = currentComicPage > 1;
                        btnComicNext.Enabled = (currentComicPage * PageSize) < totalRecords;
                    }
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show($"刷新漫畫資料時發生錯誤：{ex.Message}", "錯誤", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // 編輯用戶按鈕點擊事件
        private async void btnEditUser_Click(object sender, EventArgs e)
        {
            if (dgvUser.CurrentRow == null)
            {
                MessageBox.Show("請先選擇要編輯的用戶！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            int userId = int.Parse(dgvUser.CurrentRow.Cells["用戶ID"].Value.ToString());
            string username = dgvUser.CurrentRow.Cells["用戶名"].Value.ToString();
            string status = dgvUser.CurrentRow.Cells["狀態"].Value.ToString();
            string role = dgvUser.CurrentRow.Cells["角色"].Value.ToString();

            // 檢查是否為自己
            if (userId == currentUserId)
            {
                MessageBox.Show("不能修改自己的帳號！", "錯誤", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            // 檢查是否為管理員
            if (role == "admin")
            {
                MessageBox.Show("不能修改管理員帳號！", "錯誤", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // 查詢原始資料
            string sqlQuery = "SELECT username, status, password_hash FROM user WHERE user_id = @uid";
            MySqlParameter[] pQuery = { new MySqlParameter("@uid", userId) };
            var dt = DBHelper.ExecuteQuery(sqlQuery, pQuery);
            string oldUsername = username, oldStatus = status, oldPassword = "";
            if (dt.Rows.Count > 0)
            {
                oldUsername = dt.Rows[0]["username"].ToString();
                oldStatus = dt.Rows[0]["status"].ToString() == "active" ? "正常" : "凍結";
                oldPassword = dt.Rows[0]["password_hash"].ToString();
            }
            using (EditUserForm editUserForm = new EditUserForm(userId, username, status, oldPassword))
            {
                if (editUserForm.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        string sql = "UPDATE user SET username = @username, status = @status";
                        var paramList = new List<MySqlParameter>
                        {
                            new MySqlParameter("@username", editUserForm.Username),
                            new MySqlParameter("@status", editUserForm.Status == "正常" ? "active" : "frozen")
                        };
                        if (!string.IsNullOrWhiteSpace(editUserForm.Password))
                        {
                            sql += ", password_hash = @password";
                            paramList.Add(new MySqlParameter("@password", editUserForm.Password));
                        }
                        sql += " WHERE user_id = @userId";
                        paramList.Add(new MySqlParameter("@userId", userId));
                        int rowsAffected = DBHelper.ExecuteNonQuery(sql, paramList.ToArray());
                        if (rowsAffected > 0)
                        {
                            MessageBox.Show("用戶修改成功！", "成功", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            await RefreshUserRecordsAsync();
                            List<string> changes = new List<string>();
                            if (oldUsername != editUserForm.Username)
                                changes.Add($"用戶名由{oldUsername}改為{editUserForm.Username}");
                            if (oldStatus != editUserForm.Status)
                                changes.Add($"狀態由{oldStatus}改為{editUserForm.Status}");
                            if (!string.IsNullOrWhiteSpace(editUserForm.Password))
                                changes.Add($"密碼被修改");
                            if (changes.Count > 0)
                                WriteLogEntry("編輯用戶", $"修改了uid:{userId}的用戶，" + string.Join("，", changes));
                            await RefreshLogRecordsAsync();
                        }
                        else
                        {
                            MessageBox.Show("用戶修改失敗，請稍後再試。", "失敗", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("修改用戶時發生錯誤：" + ex.Message, "錯誤", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        // 管理日誌搜尋按鈕點擊事件
        private async void btnLogSearch_Click(object sender, EventArgs e)
        {
            string searchTerm = txtLogKeyword.Text.Trim();
            await RefreshLogRecordsAsync(keyword: searchTerm);
        }

        // 刷新管理日誌
        private async Task RefreshLogRecordsAsync(string keyword = null)
        {
            try
            {
                string sql = "SELECT action_timestamp AS 時間, u.username AS 管理員名稱, action_type AS 操作類型, action_details AS 操作詳情 FROM admin_log al JOIN user u ON al.admin_user_id = u.user_id WHERE 1=1";
                var paramList = new List<MySqlParameter>();

                if (!string.IsNullOrWhiteSpace(keyword))
                {
                    sql += " AND (u.username LIKE @keyword OR action_type LIKE @keyword OR action_details LIKE @keyword)";
                    paramList.Add(new MySqlParameter("@keyword", "%" + keyword + "%"));
                }

                // 分頁查詢
                sql += " ORDER BY action_timestamp DESC LIMIT @offset, @pageSize";
                paramList.Add(new MySqlParameter("@offset", (currentLogPage - 1) * PageSize));
                paramList.Add(new MySqlParameter("@pageSize", PageSize));

                var dt = await Task.Run(() => DBHelper.ExecuteQuery(sql, paramList.ToArray()));

                // 查詢總數
                string countSql = "SELECT COUNT(*) FROM admin_log al JOIN user u ON al.admin_user_id = u.user_id WHERE 1=1";
                var countParamList = new List<MySqlParameter>();
                if (!string.IsNullOrWhiteSpace(keyword))
                {
                    countSql += " AND (u.username LIKE @keyword OR action_type LIKE @keyword OR action_details LIKE @keyword)";
                    countParamList.Add(new MySqlParameter("@keyword", "%" + keyword + "%"));
                }
                long totalRecords = Convert.ToInt64(DBHelper.ExecuteScalar(countSql, countParamList.ToArray()));

                this.Invoke((MethodInvoker)delegate
                {
                    dgvLog.DataSource = null;
                    dgvLog.DataSource = dt;
                    SetLogGridColumnWidths();
                    dgvLog.Refresh();
                    // 分頁控制
                    lblLogPage.Text = $"第 {currentLogPage} 頁";
                    btnLogPrev.Enabled = currentLogPage > 1;
                    btnLogNext.Enabled = (currentLogPage * PageSize) < totalRecords;
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show($"刷新管理日誌時發生錯誤：{ex.Message}", "錯誤", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // 寫入管理日誌
        private void WriteLogEntry(string actionType, string actionDetails)
        {
            try
            {
                string sql = "INSERT INTO admin_log (admin_user_id, action_type, action_details) VALUES (@adminUserId, @actionType, @actionDetails)";
                MySqlParameter[] parameters = {
                    new MySqlParameter("@adminUserId", this.currentUserId),
                    new MySqlParameter("@actionType", actionType),
                    new MySqlParameter("@actionDetails", actionDetails)
                };
                DBHelper.ExecuteNonQuery(sql, parameters);
            }
            catch (Exception ex)
            {
                // 在這裡可以選擇記錄錯誤到檔案或控制台，避免因為日誌寫入失敗而影響主要操作
                System.Diagnostics.Debug.WriteLine($"寫入日誌時發生錯誤: {ex.Message}");
            }
        }

        // 設定日誌 DataGridView 列寬 (稍後在 AdminForm.Designer.cs 中實現)
        private void SetLogGridColumnWidths()
        {
            if (dgvLog.Columns.Contains("時間")) { dgvLog.Columns["時間"].Width = 150; }
            if (dgvLog.Columns.Contains("管理員名稱")) { dgvLog.Columns["管理員名稱"].Width = 100; }
            if (dgvLog.Columns.Contains("操作類型")) { dgvLog.Columns["操作類型"].Width = 120; }
            if (dgvLog.Columns.Contains("操作詳情")) { dgvLog.Columns["操作詳情"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill; }
        }

        // 分頁按鈕事件
        private async void btnUserPrev_Click(object sender, EventArgs e)
        {
            if (currentUserPage > 1)
            {
                currentUserPage--;
                await RefreshUserRecordsAsync();
            }
        }
        private async void btnUserNext_Click(object sender, EventArgs e)
        {
            currentUserPage++;
            await RefreshUserRecordsAsync();
        }

        private async void btnComicPrev_Click(object sender, EventArgs e)
        {
            if (currentComicPage > 1)
            {
                currentComicPage--;
                await RefreshComicRecordsAsync();
            }
        }
        private async void btnComicNext_Click(object sender, EventArgs e)
        {
            currentComicPage++;
            await RefreshComicRecordsAsync();
        }

        // 管理日誌分頁按鈕事件
        private async void btnLogPrev_Click(object sender, EventArgs e)
        {
            if (currentLogPage > 1)
            {
                currentLogPage--;
                await RefreshLogRecordsAsync();
            }
        }
        private async void btnLogNext_Click(object sender, EventArgs e)
        {
            currentLogPage++;
            await RefreshLogRecordsAsync();
        }

        // 新增方法：根據用戶名查詢新插入的用戶ID
        private int GetInsertedUserId(string username)
        {
            string sql = "SELECT user_id FROM user WHERE username = @username ORDER BY user_id DESC LIMIT 1";
            var param = new MySqlParameter("@username", username);
            object result = DBHelper.ExecuteScalar(sql, new[] { param });
            return result != null ? Convert.ToInt32(result) : 0;
        }
    }
}