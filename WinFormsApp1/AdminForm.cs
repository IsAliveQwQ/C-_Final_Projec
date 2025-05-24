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
            this.Text = $"管理員介面 (用戶 ID: {userId})";
            InitializeEventHandlers();
        }

        private void InitializeEventHandlers()
        {
            // 漫畫管理事件
            this.btnAddComic.Click += new EventHandler(this.btnAddComic_Click);
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
            this.Load += async (s, e) => await InitializeAllDataAsync();
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
                // 查詢所有用戶
                string sqlUser = "SELECT user_id AS 用戶ID, username AS 帳號, role AS 角色 FROM user ORDER BY user_id";
                var dtUser = await Task.Run(() => DBHelper.ExecuteQuery(sqlUser));
                this.Invoke((MethodInvoker)delegate {
                    dgvUser.DataSource = dtUser;
                    EnsureUserActionButtons();
                    SetUserGridColumnWidths();
                });
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
        private void btnAddUser_Click(object sender, System.EventArgs e)
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
                            AdminForm_Load(null, null); // 重新載入用戶表格
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

        private async void btnUserSearch_Click(object sender, EventArgs e)
        {
            currentUserSearchKeyword = txtSearchUser.Text.Trim();
            currentUserPage = 1;
            await RefreshUserRecordsAsync();
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
            currentBorrowSearchType = cmbBorrowSearchType.SelectedItem?.ToString() ?? DefaultSearchType;
            currentBorrowPage = 1;
            await RefreshBorrowRecordsAsync();
        }

        private async void btnReserveSearch_Click(object sender, EventArgs e)
        {
            currentReserveSearchKeyword = txtReserveKeyword.Text.Trim();
            currentReserveSearchType = cmbReserveSearchType.SelectedItem?.ToString() ?? DefaultSearchType;
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
            var username = dgvUser.Rows[e.RowIndex].Cells["帳號"].Value.ToString();
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
                             b.borrow_date AS 借閱日期, b.return_date AS 歸還日期
                             FROM borrow_record b
                             JOIN user u ON b.user_id = u.user_id
                             JOIN comic c ON b.comic_id = c.comic_id
                             WHERE 1=1";
                var paramList = new List<MySqlParameter>();

                if (!string.IsNullOrWhiteSpace(currentBorrowSearchKeyword))
                {
                    string field = currentBorrowSearchType == "用戶" ? "u.username" : "c.title";
                    sql += $" AND {field} LIKE @keyword";
                    paramList.Add(new MySqlParameter("@keyword", "%" + currentBorrowSearchKeyword + "%"));
                }

                sql += " ORDER BY b.borrow_date DESC LIMIT @offset, @pageSize";
                paramList.Add(new MySqlParameter("@offset", (currentBorrowPage - 1) * PageSize));
                paramList.Add(new MySqlParameter("@pageSize", PageSize));

                var dt = await Task.Run(() => DBHelper.ExecuteQuery(sql, paramList.ToArray()));
                dgvBorrow.DataSource = dt;
                SetBorrowGridColumnWidths();
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
                string sql = @"SELECT r.reservation_id AS 預約編號, u.username AS 預約者, 
                             c.title AS 書名, c.isbn AS ISBN, 
                             r.reservation_date AS 預約日期, r.status AS 狀態
                             FROM reservation r
                             JOIN user u ON r.user_id = u.user_id
                             JOIN comic c ON r.comic_id = c.comic_id
                             WHERE 1=1";
                var paramList = new List<MySqlParameter>();

                if (!string.IsNullOrWhiteSpace(currentReserveSearchKeyword))
                {
                    string field = currentReserveSearchType == "用戶" ? "u.username" : "c.title";
                    sql += $" AND {field} LIKE @keyword";
                    paramList.Add(new MySqlParameter("@keyword", "%" + currentReserveSearchKeyword + "%"));
                }

                sql += " ORDER BY r.reservation_date DESC LIMIT @offset, @pageSize";
                paramList.Add(new MySqlParameter("@offset", (currentReservePage - 1) * PageSize));
                paramList.Add(new MySqlParameter("@pageSize", PageSize));

                var dt = await Task.Run(() => DBHelper.ExecuteQuery(sql, paramList.ToArray()));
                dgvReserve.DataSource = dt;
                SetReserveGridColumnWidths();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"刷新預約紀錄時發生錯誤：{ex.Message}", "錯誤", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async Task InitializeAllDataAsync()
        {
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

        #region 用戶管理分頁
        private async Task RefreshUserRecordsAsync()
        {
            try
            {
                string sql = @"SELECT user_id AS 用戶ID, username AS 帳號, role AS 角色 
                             FROM user 
                             WHERE 1=1";
                var paramList = new List<MySqlParameter>();

                if (!string.IsNullOrWhiteSpace(currentUserSearchKeyword))
                {
                    sql += " AND username LIKE @keyword";
                    paramList.Add(new MySqlParameter("@keyword", "%" + currentUserSearchKeyword + "%"));
                }

                sql += " ORDER BY user_id LIMIT @offset, @pageSize";
                paramList.Add(new MySqlParameter("@offset", (currentUserPage - 1) * PageSize));
                paramList.Add(new MySqlParameter("@pageSize", PageSize));

                var dt = await Task.Run(() => DBHelper.ExecuteQuery(sql, paramList.ToArray()));
                dgvUser.DataSource = dt;
                EnsureUserActionButtons();
                SetUserGridColumnWidths();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"刷新用戶資料時發生錯誤：{ex.Message}", "錯誤", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion

        #region 漫畫管理分頁
        private async Task RefreshComicRecordsAsync()
        {
            try
            {
                string sql = @"SELECT comic_id AS 書號, isbn AS ISBN, title AS 書名, 
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
        #endregion

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
            }
            if (dgvUser.Columns.Contains("帳號")) {
                dgvUser.Columns["帳號"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            }
            if (dgvUser.Columns.Contains("角色")) {
                dgvUser.Columns["角色"].Width = 80;
                dgvUser.Columns["角色"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            }
            if (dgvUser.Columns.Contains("查看借閱紀錄")) {
                dgvUser.Columns["查看借閱紀錄"].Width = 110;
                dgvUser.Columns["查看借閱紀錄"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            }
            if (dgvUser.Columns.Contains("查看預約紀錄")) {
                dgvUser.Columns["查看預約紀錄"].Width = 110;
                dgvUser.Columns["查看預約紀錄"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            }
        }
        private void SetBorrowGridColumnWidths()
        {
            if (dgvBorrow.Columns.Contains("書名")) dgvBorrow.Columns["書名"].Width = 250;
            foreach (DataGridViewColumn col in dgvBorrow.Columns)
            {
                if (col.Name != "書名") col.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            }
        }
        private void SetReserveGridColumnWidths()
        {
            if (dgvReserve.Columns.Contains("書名")) dgvReserve.Columns["書名"].Width = 250;
            foreach (DataGridViewColumn col in dgvReserve.Columns)
            {
                if (col.Name != "書名") col.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            }
        }
        private void SetComicGridColumnWidths()
        {
            if (dgvComic.Columns.Contains("書名")) dgvComic.Columns["書名"].Width = 250;
            foreach (DataGridViewColumn col in dgvComic.Columns)
            {
                if (col.Name != "書名") col.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            }
        }
        #endregion
    }
} 