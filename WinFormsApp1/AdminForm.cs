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
        private const int PageSize = 10;
        private const string DefaultSearchType = "用戶";

        // 當前分頁索引
        private int currentUserPage = 1;
        private int currentComicPage = 1;
        private int currentBorrowPage = 1;
        private int currentReservePage = 1;

        // 搜尋參數
        private string currentUserSearchKeyword = "";
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

            // 用戶管理事件
            this.btnAddUser.Click += new EventHandler(this.btnAddUser_Click);
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
            };
        }

        // 新增漫畫按鈕點擊事件處理器
        private void btnAddComic_Click(object sender, EventArgs e)
        {
            // 創建 AddComicForm 的實例
            using (AddComicForm addComicForm = new AddComicForm())
            {
                // 顯示 AddComicForm 作為對話框
                if (addComicForm.ShowDialog() == DialogResult.OK)
                {
                    // 如果用戶點擊了儲存按鈕，獲取輸入的漫畫資訊
                    string title = addComicForm.ComicTitle;
                    string isbn = addComicForm.ComicISBN;
                    string author = addComicForm.ComicAuthor;
                    string publisher = addComicForm.ComicPublisher;
                    string category = addComicForm.ComicCategory;

                    // 在這裡添加將漫畫資訊存入資料庫的邏輯
                    try
                    {
                        string sql = "INSERT INTO comic (title, isbn, author, publisher, category) " +
                                     "VALUES (@title, @isbn, @author, @publisher, @category)";

                        MySqlParameter[] parameters = {
                            new MySqlParameter("@title", title),
                            new MySqlParameter("@isbn", isbn),
                            new MySqlParameter("@author", author),
                            new MySqlParameter("@publisher", publisher),
                            new MySqlParameter("@category", category)
                        };

                        int rowsAffected = DBHelper.ExecuteNonQuery(sql, parameters);

                        if (rowsAffected > 0)
                        {
                            MessageBox.Show("漫畫新增成功！", "成功", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            // TODO: 刷新漫畫列表 DataGridView
                            AdminForm_Load(null, null); // 刷新 DataGridView
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

                    // TODO: 刷新漫畫列表 DataGridView
                }
            }
        }

        private async void AdminForm_Load(object sender, System.EventArgs e)
        {
            try
              {
                // 初始化載入時，呼叫 RefreshUserRecordsAsync 獲取所有用戶
                await RefreshUserRecordsAsync();

                // 查詢所有漫畫
                string sqlComic = "SELECT comic_id AS 書號, isbn AS ISBN, title AS 書名, author AS 作者, publisher AS 出版社, category AS 分類 FROM comic ORDER BY comic_id";
                var dtComic = await Task.Run(() => DBHelper.ExecuteQuery(sqlComic));
                this.Invoke((MethodInvoker)delegate {
                    dgvComic.DataSource = dtComic;
                    SetComicGridColumnWidths();
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show("載入資料時發生錯誤：" + ex.Message, "錯誤", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            currentBorrowPage = 1;
            currentReservePage = 1;
            await RefreshBorrowRecordsAsync();
            await RefreshReserveRecordsAsync();
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
                            // 新增用戶成功後，呼叫 RefreshUserRecordsAsync 刷新用戶列表 (獲取所有用戶)
                            await RefreshUserRecordsAsync();
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
        private void btnDeleteUser_Click(object sender, System.EventArgs e)
        {
            if (dgvUser.CurrentRow == null)
            {
                MessageBox.Show("請先選擇要刪除的用戶！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            int userId = int.Parse(dgvUser.CurrentRow.Cells["用戶ID"].Value.ToString());
            if (userId == currentUserId)
            {
                MessageBox.Show("不能刪除自己！", "錯誤", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (MessageBox.Show("確定要刪除此用戶？", "確認", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                try
                {
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
            bool applyPagination = false,
            int page = 1)
        {
            try
            {
                string sql = @"SELECT user_id AS 用戶ID, username AS 用戶名, role AS 角色 
                             FROM user 
                             WHERE 1=1";
                var paramList = new List<MySqlParameter>();

                // 根據搜尋類型構建 WHERE 子句
                if (!string.IsNullOrWhiteSpace(keyword))
                {
                    if (searchType == "用戶ID")
                    {
                        // 如果選擇用戶ID搜尋，嚴格檢查是否為數字
                        if (int.TryParse(keyword, out int userId))
                        {
                            sql += " AND user_id = @userId";
                            paramList.Add(new MySqlParameter("@userId", userId));
                        }
                        else
                        {
                            // 如果不是有效的數字，不進行搜尋，可以考慮返回空結果或提示
                            // 這裡選擇一個永遠不為真的條件，以返回空集
                            sql += " AND 1 = 0"; 
                            // 或者您希望提示用戶？可以在這裡加 MessageBox，但 await 會影響 UI 線程
                            // MessageBox.Show("用戶ID必須是數字", "錯誤", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            // return; // 如果要提示並停止，需要重新考慮 async Task 的處理
                        }
                    }
                    else // 用戶名搜尋 (模糊匹配)
                    {
                         sql += " AND username LIKE @keyword";
                         paramList.Add(new MySqlParameter("@keyword", "%" + keyword + "%"));
                    }
                }

                sql += " ORDER BY user_id";

                // 如果應用分頁，則加入 LIMIT 條件
                if (applyPagination)
                {
                    sql += " LIMIT @offset, @pageSize";
                    paramList.Add(new MySqlParameter("@offset", (page - 1) * PageSize));
                    paramList.Add(new MySqlParameter("@pageSize", PageSize));
                }

                var dt = await Task.Run(() => DBHelper.ExecuteQuery(sql, paramList.ToArray()));
                
                // 在 UI 線程上更新 DataGridView
                this.Invoke((MethodInvoker)delegate {
                     dgvUser.DataSource = null; // 保留此行
                     dgvUser.DataSource = dt;   // 保留此行
                     // 將 SetUserGridColumnWidths 的呼叫移到這裡，確保在所有列存在後再設定 DisplayIndex
                     SetUserGridColumnWidths(); // 將呼叫移到 Refresh() 之前
                     dgvUser.Refresh();         // 將呼叫移到 SetUserGridColumnWidths() 之後
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
            string searchTerm = txtSearchUser.Text.Trim();
            string searchType = cmbUserSearchType.SelectedItem?.ToString() ?? "用戶名"; // 預設用戶名搜尋

            // 呼叫修改後的 RefreshUserRecordsAsync 方法
            await RefreshUserRecordsAsync(keyword: searchTerm, searchType: searchType, applyPagination: false);
        }

        private async void btnComicSearch_Click(object sender, EventArgs e)
        {
            currentComicSearchKeyword = txtComicKeyword.Text.Trim();
            currentComicSearchType = cmbComicSearchType.SelectedItem?.ToString() ?? "書號";
            currentComicPage = 1;
            await RefreshComicRecordsAsync();
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

        private void dgvComic_CellContentClick(object sender, DataGridViewCellEventArgs e)
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

        private void btnDeleteComic_Click(object sender, System.EventArgs e)
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
                    string sql = "DELETE FROM comic WHERE comic_id = @cid";
                    MySqlParameter[] p = { new MySqlParameter("@cid", comicId) };
                    int rows = DBHelper.ExecuteNonQuery(sql, p);
                    if (rows > 0)
                    {
                        MessageBox.Show("刪除成功！", "成功", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        AdminForm_Load(null, null);
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
            var username = dgvUser.Rows[e.RowIndex].Cells["用戶名"].Value.ToString();
            if (colName == "查看借閱紀錄")
            {
                dgvBorrow.DataSource = null; // 先清空，避免閃現舊資料
                tabAdmin.SelectedTab = tabBorrow;
                txtBorrowKeyword.Text = username;
                btnBorrowSearch.PerformClick();
            }
            else if (colName == "查看預約紀錄")
            {
                dgvReserve.DataSource = null; // 先清空，避免閃現舊資料
                tabAdmin.SelectedTab = tabReserve;
                txtReserveKeyword.Text = username;
                btnReserveSearch.PerformClick();
            }
        }

        // 借閱紀錄分頁操作按鈕事件
        private void dgvBorrow_CellContentClick(object sender, DataGridViewCellEventArgs e)
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
                    AdminForm_Load(null, null);
                }
            }
        }

        // 預約紀錄分頁操作按鈕事件
        private void dgvReserve_CellContentClick(object sender, DataGridViewCellEventArgs e)
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
                        AdminForm_Load(null, null);
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
                    sql += $" AND {field} LIKE @keyword";
                    paramList.Add(new MySqlParameter("@keyword", "%" + currentBorrowSearchKeyword + "%"));
                }

                sql += " ORDER BY b.borrow_date DESC LIMIT @offset, @pageSize";
                paramList.Add(new MySqlParameter("@offset", (currentBorrowPage - 1) * PageSize));
                paramList.Add(new MySqlParameter("@pageSize", PageSize));

                var dt = await Task.Run(() => DBHelper.ExecuteQuery(sql, paramList.ToArray()));
                dgvBorrow.DataSource = dt;
                SetBorrowGridColumnWidths();

                // 更新分頁控制項
                UpdatePagingControls(dgvBorrow, currentBorrowPage, btnBorrowPrev, btnBorrowNext, lblBorrowPage);
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
                                 WHEN r.status = 'active' THEN '預約中'
                                 WHEN r.status = 'canceled' THEN '已取消'
                                 ELSE r.status -- 保留其他未知狀態
                             END AS 狀態
                             FROM reservation r
                             JOIN user u ON r.user_id = u.user_id
                             JOIN comic c ON r.comic_id = c.comic_id
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
                    sql += $" AND {field} LIKE @keyword";
                    paramList.Add(new MySqlParameter("@keyword", "%" + currentReserveSearchKeyword + "%"));
                }

                sql += " ORDER BY r.reservation_date DESC LIMIT @offset, @pageSize";
                paramList.Add(new MySqlParameter("@offset", (currentReservePage - 1) * PageSize));
                paramList.Add(new MySqlParameter("@pageSize", PageSize));

                var dt = await Task.Run(() => DBHelper.ExecuteQuery(sql, paramList.ToArray()));
                dgvReserve.DataSource = dt;
                SetReserveGridColumnWidths();

                // 更新分頁控制項
                UpdatePagingControls(dgvReserve, currentReservePage, btnReservePrev, btnReserveNext, lblReservePage);
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
            btnNext.Enabled = dgv.Rows.Count == PageSize;
        }

        private void ResetSearchParameters()
        {
            currentUserSearchKeyword = "";
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
            if (dgvUser.Columns.Contains("用戶ID")) {
                dgvUser.Columns["用戶ID"].Width = 60;
                dgvUser.Columns["用戶ID"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
                dgvUser.Columns["用戶ID"].DisplayIndex = 0;
            }
            if (dgvUser.Columns.Contains("用戶名")) {
                dgvUser.Columns["用戶名"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                dgvUser.Columns["用戶名"].DisplayIndex = 1;
            }
            if (dgvUser.Columns.Contains("角色")) {
                dgvUser.Columns["角色"].Width = 80;
                dgvUser.Columns["角色"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
                dgvUser.Columns["角色"].DisplayIndex = 2;
            }
            if (dgvUser.Columns.Contains("查看借閱紀錄")) {
                dgvUser.Columns["查看借閱紀錄"].Width = 110;
                dgvUser.Columns["查看借閱紀錄"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
                dgvUser.Columns["查看借閱紀錄"].DisplayIndex = 3;
            }
            if (dgvUser.Columns.Contains("查看預約紀錄")) {
                dgvUser.Columns["查看預約紀錄"].Width = 110;
                dgvUser.Columns["查看預約紀錄"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
                dgvUser.Columns["查看預約紀錄"].DisplayIndex = 4;
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
                if (!new[] {"編號", "用戶", "書名", "ISBN", "借閱日期", "歸還日期", "狀態"}.Contains(col.Name))
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
            // 設置前三欄的順序和寬度
            if (dgvComic.Columns.Contains("書號")) {
                dgvComic.Columns["書號"].Width = 80;
                dgvComic.Columns["書號"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
                dgvComic.Columns["書號"].DisplayIndex = 0;
            }
            if (dgvComic.Columns.Contains("書名")) {
                dgvComic.Columns["書名"].Width = 250;
                dgvComic.Columns["書名"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
                dgvComic.Columns["書名"].DisplayIndex = 1;
            }
            if (dgvComic.Columns.Contains("ISBN")) {
                dgvComic.Columns["ISBN"].Width = 120;
                dgvComic.Columns["ISBN"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
                dgvComic.Columns["ISBN"].DisplayIndex = 2;
            }
            
            // 設置其他欄位的順序和寬度
            if (dgvComic.Columns.Contains("作者")) {
                dgvComic.Columns["作者"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                dgvComic.Columns["作者"].DisplayIndex = 3;
            }
            if (dgvComic.Columns.Contains("出版社")) {
                dgvComic.Columns["出版社"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                dgvComic.Columns["出版社"].DisplayIndex = 4;
            }
            if (dgvComic.Columns.Contains("分類")) {
                dgvComic.Columns["分類"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                dgvComic.Columns["分類"].DisplayIndex = 5;
            }
        }
        #endregion

        // 修改漫畫按鈕點擊事件處理器
        private void btnEditComic_Click(object sender, EventArgs e)
        {
            if (dgvComic.CurrentRow == null)
            {
                MessageBox.Show("請先選擇要修改的漫畫！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // 獲取當前選中的漫畫資訊
            int comicId = int.Parse(dgvComic.CurrentRow.Cells["書號"].Value.ToString());
            string isbn = dgvComic.CurrentRow.Cells["ISBN"].Value.ToString();
            string title = dgvComic.CurrentRow.Cells["書名"].Value.ToString();
            string author = dgvComic.CurrentRow.Cells["作者"].Value.ToString();
            string publisher = dgvComic.CurrentRow.Cells["出版社"].Value.ToString();
            string category = dgvComic.CurrentRow.Cells["分類"].Value.ToString();

            // 創建並顯示修改漫畫表單
            using (EditComicForm editComicForm = new EditComicForm(comicId, isbn, title, author, publisher, category))
            {
                if (editComicForm.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        // 更新資料庫中的漫畫資訊
                        string sql = @"UPDATE comic 
                                     SET isbn = @isbn, 
                                         title = @title, 
                                         author = @author, 
                                         publisher = @publisher, 
                                         category = @category 
                                     WHERE comic_id = @comic_id";

                        MySqlParameter[] parameters = {
                            new MySqlParameter("@comic_id", comicId),
                            new MySqlParameter("@isbn", editComicForm.ComicISBN),
                            new MySqlParameter("@title", editComicForm.ComicTitle),
                            new MySqlParameter("@author", editComicForm.ComicAuthor),
                            new MySqlParameter("@publisher", editComicForm.ComicPublisher),
                            new MySqlParameter("@category", editComicForm.ComicCategory)
                        };

                        int rowsAffected = DBHelper.ExecuteNonQuery(sql, parameters);

                        if (rowsAffected > 0)
                        {
                            MessageBox.Show("漫畫修改成功！", "成功", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            RefreshComicRecordsAsync(); // 刷新漫畫列表
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
        private async Task RefreshComicRecordsAsync()
        {
            try
            {
                string sql = @"SELECT comic_id AS 書號, title AS 書名, isbn AS ISBN,
                             author AS 作者, publisher AS 出版社, category AS 分類 
                             FROM comic WHERE 1=1";
                var paramList = new List<MySqlParameter>();

                if (!string.IsNullOrWhiteSpace(currentComicSearchKeyword))
                {
                    if (currentComicSearchType == "書號" && int.TryParse(currentComicSearchKeyword, out int comicId))
                    {
                        sql += " AND comic_id = @comic_id";
                        paramList.Add(new MySqlParameter("@comic_id", comicId));
                    }
                    else
                    {
                        string field = currentComicSearchType switch
                        {
                            "書名" => "title",
                            "ISBN" => "isbn",
                            "作者" => "author",
                            "出版社" => "publisher",
                            "分類" => "category",
                            _ => "comic_id"
                        };
                        sql += $" AND {field} LIKE @keyword";
                        paramList.Add(new MySqlParameter("@keyword", "%" + currentComicSearchKeyword + "%"));
                    }
                }

                sql += " ORDER BY comic_id LIMIT @offset, @pageSize";
                paramList.Add(new MySqlParameter("@offset", (currentComicPage - 1) * PageSize));
                paramList.Add(new MySqlParameter("@pageSize", PageSize));

                var dt = await Task.Run(() => DBHelper.ExecuteQuery(sql, paramList.ToArray()));
                dgvComic.DataSource = dt;
                SetComicGridColumnWidths();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"刷新漫畫資料時發生錯誤：{ex.Message}", "錯誤", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
} 