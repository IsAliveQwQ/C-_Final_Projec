using System;
using System.Data;
using System.Windows.Forms;
using MySql.Data.MySqlClient;
using System.Threading.Tasks;
using System.Collections.Generic;
using Minio; 
using Minio.Exceptions; 

#nullable disable  

namespace WinFormsApp1
{
    
    public class ComicDetails
    {
        public string 書名 { get; set; }
        public string 作者 { get; set; }
        public string 出版社 { get; set; }
        public string 分類 { get; set; }
        public string ISBN { get; set; }
        public string 借閱狀態 { get; set; }
        public string 預約狀態 { get; set; }

        public string ImageUrl { get; set; }

        public string OfferDate { get; set; }         public string Pages { get; set; }         public string BookSummary { get; set; }     }

    public partial class UserForm : Form
    {
        private int loggedInUserId = 0;         private string loggedInUsername = "";         private string loggedInUserRole = "";         private int currentBorrowPage = 1;
        private int currentReservePage = 1;

        private const int PageSize = 10;         private int currentComicPage = 1;         private const int ComicPageSize = 10;         private Button btnComicPrev;         private Button btnComicNext;         private Label lblComicPage; 

        private DataGridView dgvUserComics;


        private System.Windows.Forms.TabControl tabUserMain;
        private System.Windows.Forms.TabPage tabPageHome;
        private System.Windows.Forms.TabPage tabPageBorrow;
        private System.Windows.Forms.TabPage tabPageReserve;


        private DataTable cachedBorrowRecords = null;
        private DataTable cachedReserveRecords = null;
        private DataTable cachedFavoriteRecords = null;
        private DateTime lastBorrowRefresh = DateTime.MinValue;
        private DateTime lastReserveRefresh = DateTime.MinValue;
        private DateTime lastFavoriteRefresh = DateTime.MinValue;
        private const int CACHE_DURATION_SECONDS = 30; 

        private const string MinioEndpoint = "bucket-production-63a9.up.railway.app";
        private const string MinioAccessKey = "dkF1y6M79nH7i8BTBfXuOmf6x6bNJ1rW";
        private const string MinioSecretKey = "Lq4ijpczUkbRznusbOAm0hOWiXLRBdQDb16fJQgbcPH3Q0Xn";
        private const string BucketName = "comicimage"; 
        private IMinioClient minioClient;


        private async Task InitializeMinioClientAsync()
        {
            try
            {
                minioClient = new MinioClient()
                    .WithEndpoint(MinioEndpoint)
                    .WithCredentials(MinioAccessKey, MinioSecretKey)
                    .Build();

                await CheckOrCreateBucketAsync(BucketName);
                Console.WriteLine("MinIO client initialized successfully and bucket checked/created.");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"初始化 MinIO 客戶端時發生錯誤: {ex.Message}", "MinIO 錯誤", MessageBoxButtons.OK, MessageBoxIcon.Error);
                minioClient = null;
            }
        }


        private async Task CheckOrCreateBucketAsync(string bucketName)
        {
            try
            {
                bool found = await minioClient.BucketExistsAsync(
                    new Minio.DataModel.Args.BucketExistsArgs().WithBucket(bucketName)
                );

                if (!found)
                {
                    await minioClient.MakeBucketAsync(
                        new Minio.DataModel.Args.MakeBucketArgs().WithBucket(bucketName)
                    );
                    Console.WriteLine($"Bucket '{bucketName}' created successfully.");
                }
                else
                {
                    Console.WriteLine($"Bucket '{bucketName}' already exists.");
                }
            }
            catch (MinioException ex)
            {
                Console.WriteLine($"MinIO Error while checking/creating bucket: {ex.Message}");
            }
            catch (Exception ex)
            {
                 Console.WriteLine($"Generic Error while checking/creating bucket: {ex.Message}");
            }
        }

        public UserForm(int userId, string userRole)
        {
            InitializeComponent();
            
            typeof(DataGridView).InvokeMember(
                "DoubleBuffered",
                System.Reflection.BindingFlags.SetProperty |
                System.Reflection.BindingFlags.Instance |
                System.Reflection.BindingFlags.NonPublic,
                null,
                dgvComics,
                new object[] { true });

            dgvComics.Font = new System.Drawing.Font("Microsoft JhengHei UI", 10F);
            dgvComics.ColumnHeadersDefaultCellStyle.Font = new System.Drawing.Font("Microsoft JhengHei UI", 10F, System.Drawing.FontStyle.Bold);
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

            string username = GetUsernameById(userId);
            this.Text = $"漫畫租書及預約系統 - 使用者介面（ID:{userId} 用戶名:{username}）";

            this.loggedInUserId = userId;
            this.loggedInUserRole = userRole; 
            var screen = Screen.PrimaryScreen.WorkingArea;
            this.Size = new Size(1242, (int)(screen.Height * 0.6)); 
            this.StartPosition = FormStartPosition.CenterScreen;


            this.btnSearch.Click += new System.EventHandler(this.btnSearch_Click);

            this.dgvComics.SelectionChanged += new System.EventHandler(this.dgvComics_SelectionChanged);

            this.btnLoginLogout.Click += new System.EventHandler(this.btnLoginLogout_Click);

            this.btnAdmin.Click += new System.EventHandler(this.btnAdmin_Click);


            this.btnBigRent.Click += new System.EventHandler(this.btnBigRent_Click);
            this.btnBigReserve.Click += new System.EventHandler(this.btnBigReserve_Click);
      
            panelComicActions.Visible = false;

            this.Load += new System.EventHandler(this.UserForm_Load);

        

            
            DisableButtonsForAdmin();

            this.btnBorrowPrev.Click += new EventHandler(btnBorrowPrev_Click);
            this.btnBorrowNext.Click += new EventHandler(btnBorrowNext_Click);
            this.btnReservePrev.Click += new EventHandler(btnReservePrev_Click);
            this.btnReserveNext.Click += new EventHandler(btnReserveNext_Click);

            
            InitializeUserTabs();

            
            dgvComics.DataBindingComplete += dgvComics_DataBindingComplete;
        }


        private void dgvComics_DataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
        {
            dgvComics.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            dgvComics.ColumnHeadersHeight = 24;
        }


        public UserForm() : this(0, "")
        {

             DisableButtonsForAdmin();
        }

        private void DisableButtonsForAdmin()
        {
            if (loggedInUserRole == "admin")
            {
                btnRent.Enabled = false;                 btnReserve.Enabled = false;                 btnBigRent.Enabled = false;                 btnBigReserve.Enabled = false;             }
            else
            {
                
                btnRent.Enabled = false;
                btnReserve.Enabled = false;
                btnBigRent.Enabled = false;
                btnBigReserve.Enabled = false;
            }
             
             btnAdmin.Visible = (loggedInUserRole == "admin");
             
             UpdateLoginStatus(loggedInUserId > 0, loggedInUserId, loggedInUsername, loggedInUserRole);
        }

        private async void UserForm_Load(object sender, EventArgs e)
        {
            SetupHomePageLayout();
            SetupBorrowPageLayout();
            SetupReservePageLayout();

            dgvComics.Font = new System.Drawing.Font("Microsoft JhengHei UI", 20F);
            dgvComics.ColumnHeadersDefaultCellStyle.Font = new System.Drawing.Font("Microsoft JhengHei UI", 20F, System.Drawing.FontStyle.Bold);
            dgvComics.RowTemplate.Height = 48;
            dgvComics.RowHeadersWidth = 60;
            SetComicsGridColumnSettings();

            this.Show();

            _ = Task.Run(async () =>
            {
                try
                {

                    await InitializeMinioClientAsync();


                    await this.InvokeAsync(() => RefreshUserComicsGrid("", "全部"));


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


        private async Task InitializeAllUserDataAsync()
        {
            await Task.WhenAll(
                RefreshUserComicsGrid("", "全部"),
                RefreshBorrowRecordsAsync(),
                RefreshReserveRecordsAsync()
            );
        }


        private void LoadComicsDataFromDatabase()
        {
            try
            {
                string sql = "SELECT comic_id, isbn, title, author, publisher, category FROM comic WHERE 1=1";
                DataTable dt = DBHelper.ExecuteQuery(sql);
                if (!dt.Columns.Contains("借閱狀態")) dt.Columns.Add("借閱狀態", typeof(string));
                if (!dt.Columns.Contains("預約狀態")) dt.Columns.Add("預約狀態", typeof(string));


                var borrowDict = new Dictionary<int, int>();
                var borrowDt = DBHelper.ExecuteQuery("SELECT comic_id, user_id FROM borrow_record WHERE return_date IS NULL");
                foreach (DataRow row in borrowDt.Rows)
                    borrowDict[Convert.ToInt32(row["comic_id"])] = Convert.ToInt32(row["user_id"]);


                var reserveDict = new Dictionary<int, int>();
                var reserveDt = DBHelper.ExecuteQuery("SELECT comic_id, user_id FROM reservation WHERE status = 'active' AND reservation_date > DATE_SUB(NOW(), INTERVAL 24 HOUR)");
                foreach (DataRow row in reserveDt.Rows)
                    reserveDict[Convert.ToInt32(row["comic_id"])] = Convert.ToInt32(row["user_id"]);

                foreach (DataRow row in dt.Rows)
                {
                    int comicId = Convert.ToInt32(row["comic_id"]);

                    bool isBorrowed = borrowDict.ContainsKey(comicId);
                    row["借閱狀態"] = isBorrowed ? "已被借" : "未被借";

                    string reserveStatus = isBorrowed ? "不可預約" :
                        reserveDict.ContainsKey(comicId) ? "已被預約" : "可預約";
                    row["預約狀態"] = reserveStatus;
                }
                dgvComics.DataSource = dt;
                dgvComics.ColumnHeadersVisible = true; 
                if (dgvComics.Columns.Contains("comic_id")) dgvComics.Columns["comic_id"].HeaderText = "書號";
                if (dgvComics.Columns.Contains("isbn")) dgvComics.Columns["isbn"].HeaderText = "ISBN";
                if (dgvComics.Columns.Contains("title")) dgvComics.Columns["title"].HeaderText = "書名";
                if (dgvComics.Columns.Contains("author")) dgvComics.Columns["author"].HeaderText = "作者";
                if (dgvComics.Columns.Contains("publisher")) dgvComics.Columns["publisher"].HeaderText = "出版社";
                if (dgvComics.Columns.Contains("category")) dgvComics.Columns["category"].HeaderText = "分類";
                if (dgvComics.Columns.Contains("借閱狀態")) dgvComics.Columns["借閱狀態"].HeaderText = "借閱狀態";
                if (dgvComics.Columns.Contains("預約狀態")) dgvComics.Columns["預約狀態"].HeaderText = "預約狀態";

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


        private void btnSearch_Click(object sender, EventArgs e)
        {
            DoSearch();
        }

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


                var borrowDict = new Dictionary<int, int>();
                var borrowDt = DBHelper.ExecuteQuery("SELECT comic_id, user_id FROM borrow_record WHERE return_date IS NULL");
                foreach (DataRow row in borrowDt.Rows)
                    borrowDict[Convert.ToInt32(row["comic_id"])] = Convert.ToInt32(row["user_id"]);


                var reserveDict = new Dictionary<int, int>();
                var reserveDt = DBHelper.ExecuteQuery("SELECT comic_id, user_id FROM reservation WHERE status = 'active' AND reservation_date > DATE_SUB(NOW(), INTERVAL 24 HOUR)");
                foreach (DataRow row in reserveDt.Rows)
                    reserveDict[Convert.ToInt32(row["comic_id"])] = Convert.ToInt32(row["user_id"]);

                foreach (DataRow row in dt.Rows)
                {
                    int comicId = Convert.ToInt32(row["comic_id"]);

                    bool isBorrowed = borrowDict.ContainsKey(comicId);
                    row["借閱狀態"] = isBorrowed ? "已被借" : "未被借";

                    string reserveStatus = isBorrowed ? "不可預約" :
                        reserveDict.ContainsKey(comicId) ? "已被預約" : "可預約";
                    row["預約狀態"] = reserveStatus;
                }
                dgvComics.DataSource = dt;
                dgvComics.ColumnHeadersVisible = true; 
                if (dgvComics.Columns.Contains("comic_id")) dgvComics.Columns["comic_id"].HeaderText = "書號";
                if (dgvComics.Columns.Contains("isbn")) dgvComics.Columns["isbn"].HeaderText = "ISBN";
                if (dgvComics.Columns.Contains("title")) dgvComics.Columns["title"].HeaderText = "書名";
                if (dgvComics.Columns.Contains("author")) dgvComics.Columns["author"].HeaderText = "作者";
                if (dgvComics.Columns.Contains("publisher")) dgvComics.Columns["publisher"].HeaderText = "出版社";
                if (dgvComics.Columns.Contains("category")) dgvComics.Columns["category"].HeaderText = "分類";
                if (dgvComics.Columns.Contains("借閱狀態")) dgvComics.Columns["借閱狀態"].HeaderText = "借閱狀態";
                if (dgvComics.Columns.Contains("預約狀態")) dgvComics.Columns["預約狀態"].HeaderText = "預約狀態";

                if (dgvComics.Columns.Contains("title")) dgvComics.Columns["title"].Width = 260;
                if (dgvComics.Columns.Contains("publisher")) dgvComics.Columns["publisher"].Width = 200;
                if (dgvComics.Columns.Contains("借閱狀態")) dgvComics.Columns["借閱狀態"].Width = 160;
                if (dgvComics.Columns.Contains("預約狀態")) dgvComics.Columns["預約狀態"].Width = 160;

                if (dt.Rows.Count > 0)
                {
                    dgvComics.ClearSelection();
                    dgvComics.Rows[0].Selected = true;
                    dgvComics_SelectionChanged(dgvComics, EventArgs.Empty);
                }
                else
                {

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


        private async void dgvComics_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvComics.CurrentRow != null && dgvComics.CurrentRow.Index >= 0 && dgvComics.CurrentRow.DataBoundItem != null)
            {
                DataGridViewRow selectedRow = dgvComics.CurrentRow;
                int comicId = Convert.ToInt32(selectedRow.Cells["書號"].Value);
                string isbn = selectedRow.Cells["ISBN"].Value.ToString();
                string title = selectedRow.Cells["書名"].Value.ToString();
                string author = selectedRow.Cells["作者"].Value.ToString();
                string publisher = selectedRow.Cells["出版社"].Value.ToString();
                string category = selectedRow.Cells["分類"].Value.ToString();


                lblDetailTitle.Text = "書名：" + title;
                lblDetailAuthor.Text = "作者：" + author;
                lblDetailCategory.Text = "分類：" + category;
                lblDetailPublisher.Text = "出版社：" + publisher;


                string sqlBorrow = "SELECT user_id FROM borrow_record WHERE comic_id = @cid AND return_date IS NULL";
                MySqlParameter[] p1 = { new MySql.Data.MySqlClient.MySqlParameter("@cid", comicId) };
                var dtBorrow = await Task.Run(() => DBHelper.ExecuteQuery(sqlBorrow, p1));
                int borrowedUserId = -1;
                string borrowStatus = "未被借";
                if (dtBorrow.Rows.Count > 0)
                {
                    borrowStatus = "已被借";
                    borrowedUserId = Convert.ToInt32(dtBorrow.Rows[0]["user_id"]);
                }

                lblDetailStatus.Text = "借閱狀態：" + borrowStatus;


                string reserveStatus = "可預約";
                int reservedUserId = -1;
                

                if (borrowStatus == "已被借")
                {
                    reserveStatus = "不可預約";
                }
                else
                {

                    string sqlReserve = "SELECT user_id FROM reservation WHERE comic_id = @cid AND status = 'active' AND reservation_date > DATE_SUB(NOW(), INTERVAL 24 HOUR)";
                    var dtReserve = await Task.Run(() => DBHelper.ExecuteQuery(sqlReserve, p1));
                    if (dtReserve.Rows.Count > 0)
                    {
                        reserveStatus = "已被預約";
                        reservedUserId = Convert.ToInt32(dtReserve.Rows[0]["user_id"]);
                    }
                }
                

                lblDetailReserve.Text = "預約狀態：" + reserveStatus;


                bool isCoolingBorrow = false;
                bool isCoolingReserve = false;
                DateTime? lastReturnTime = null;
                DateTime? lastReserveTime = null;

                if (loggedInUserId > 0)
                {

                    string sqlLastReturn = "SELECT return_date FROM borrow_record WHERE user_id = @uid AND comic_id = @cid AND return_date IS NOT NULL ORDER BY return_date DESC LIMIT 1";
                    var dtReturn = await Task.Run(() => DBHelper.ExecuteQuery(sqlLastReturn, new MySql.Data.MySqlClient.MySqlParameter[] {
                        new MySql.Data.MySqlClient.MySqlParameter("@uid", loggedInUserId),
                        new MySql.Data.MySqlClient.MySqlParameter("@cid", comicId)
                    }));
                    if (dtReturn.Rows.Count > 0)
                    {
                        lastReturnTime = Convert.ToDateTime(dtReturn.Rows[0]["return_date"]);
                        if ((DateTime.Now - lastReturnTime.Value).TotalHours < 24)
                        {
                            isCoolingBorrow = true;
                        }
                    }


                    string sqlLastReserve = "SELECT reservation_date FROM reservation WHERE user_id = @uid AND comic_id = @cid AND reservation_date > DATE_SUB(NOW(), INTERVAL 24 HOUR) ORDER BY reservation_date DESC LIMIT 1";
                    var dtLastReserve = await Task.Run(() => DBHelper.ExecuteQuery(sqlLastReserve, new MySql.Data.MySqlClient.MySqlParameter[] {
                        new MySql.Data.MySqlClient.MySqlParameter("@uid", loggedInUserId),
                        new MySql.Data.MySqlClient.MySqlParameter("@cid", comicId)
                    }));
                    if (dtLastReserve.Rows.Count > 0)
                    {
                        lastReserveTime = Convert.ToDateTime(dtLastReserve.Rows[0]["reservation_date"]);
                        if ((DateTime.Now - lastReserveTime.Value).TotalHours < 24)
                        {
                            isCoolingReserve = true;
                        }
                    }
                }

                
                await this.InvokeAsync(() => {
                    btnBigRent.Enabled = false;
                    btnBigReserve.Enabled = false;

                    if (loggedInUserRole != "admin" && loggedInUserId > 0)
                    {

                        if (borrowStatus == "已被借")
                        {

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

                        else
                        {

                            if (reserveStatus == "已被預約")
                            {

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
                            
                            else
                            {
                                
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

                   
                }); 
            }
            else
            {
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


        private async void btnBigRent_Click(object sender, EventArgs e)
        {
            if (dgvComics.CurrentRow == null) return;
            int comicId = Convert.ToInt32(dgvComics.CurrentRow.Cells["comic_id"].Value);
            try
            {

                string sqlBorrow = "SELECT user_id FROM borrow_record WHERE comic_id = @cid AND return_date IS NULL";
                var dtBorrow = DBHelper.ExecuteQuery(sqlBorrow, new MySql.Data.MySqlClient.MySqlParameter[] {
                    new MySql.Data.MySqlClient.MySqlParameter("@cid", comicId)
                });
                bool isBorrowed = dtBorrow.Rows.Count > 0;
                int borrowedUserId = isBorrowed ? Convert.ToInt32(dtBorrow.Rows[0]["user_id"]) : -1;
                if (isBorrowed && borrowedUserId == loggedInUserId)
                {

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

                    string sqlBorrowInsert = "INSERT INTO borrow_record (user_id, comic_id, borrow_date) VALUES (@uid, @cid, NOW())";
                    int rowsAffected = DBHelper.ExecuteNonQuery(sqlBorrowInsert, new MySql.Data.MySqlClient.MySqlParameter[] {
                        new MySql.Data.MySqlClient.MySqlParameter("@uid", loggedInUserId),
                        new MySql.Data.MySqlClient.MySqlParameter("@cid", comicId)
                    });
                    if (rowsAffected > 0)
                    {

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

                if (IsUserInCoolingPeriod(loggedInUserId, comicId))
                {
                    MessageBox.Show("您剛歸還此書，正在借閱冷卻期內，暫時無法預約。", "預約失敗", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }


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


                var activeReservation = await Task.Run(() => {
                    string sqlReserve = "SELECT user_id, reservation_id FROM reservation WHERE comic_id = @cid AND status = 'active' AND reservation_date > DATE_SUB(NOW(), INTERVAL 24 HOUR)";
                    var dtReserve = DBHelper.ExecuteQuery(sqlReserve, new MySql.Data.MySqlClient.MySqlParameter[] {
                        new MySql.Data.MySqlClient.MySqlParameter("@cid", comicId)
                    });
                    return dtReserve.Rows.Count > 0 ? new { UserId = Convert.ToInt32(dtReserve.Rows[0]["user_id"]), ReservationId = Convert.ToInt32(dtReserve.Rows[0]["reservation_id"]) } : null;
                });


                if (activeReservation != null && activeReservation.UserId == loggedInUserId)
                {

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

                else if (activeReservation != null)
                {
                    MessageBox.Show("此書已被預約，無法再次預約。", "預約失敗", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }


                if (IsUserInReservationCoolingPeriod(loggedInUserId, comicId))
                {
                    MessageBox.Show("您仍在預約冷卻期內，無法預約此書。", "預約失敗", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    await RefreshAllSectionsAsync();
                    return;
                }


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


        private void btnLoginLogout_Click(object sender, EventArgs e)
        {
            if (loggedInUserId > 0)             {
                Logout();
            }
            else             {
                using (Form1 loginForm = new Form1())
                {
                    if (loginForm.ShowDialog() == DialogResult.OK)
                    {

                        int userId = loginForm.LoggedInUserId; 
                        string userRole = loginForm.LoggedInUserRole; 


                        string username = GetUsernameById(userId);

                        UpdateLoginStatus(true, userId, username, userRole);
                        MessageBox.Show($"登入成功！歡迎，{username}", "成功", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {

                        MessageBox.Show("登入失敗或已取消", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            }
        }


        private void UpdateLoginStatus(bool isLoggedIn, int userId = 0, string username = "", string userRole = "")
        {
            loggedInUserId = userId;
            loggedInUsername = username;
            loggedInUserRole = userRole; 

            if (isLoggedIn)
            {
                lblUsername.Text = $"歡迎，{loggedInUsername}";
                btnLoginLogout.Text = "登出";


                btnAdmin.Visible = (loggedInUserRole == "admin");



                 if (dgvComics.SelectedRows.Count > 0)
                {
                    dgvComics_SelectionChanged(dgvComics, EventArgs.Empty);
                }


                btnRent.Enabled = (loggedInUserRole != "admin");
                btnReserve.Enabled = (loggedInUserRole != "admin");
            }
            else
            {
                lblUsername.Text = ""; 
                btnLoginLogout.Text = "登入";


                btnAdmin.Visible = false;
                btnRent.Enabled = false;
                btnReserve.Enabled = false;
            }
        }


        private void Logout()
        {
            UpdateLoginStatus(false);
            MessageBox.Show("您已登出", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);

            LoadComicsDataFromDatabase();
        }


        private void btnAdmin_Click(object sender, EventArgs e)
        {

            if (loggedInUserRole == "admin")
            {

                AdminForm adminForm = new AdminForm(loggedInUserId);

                adminForm.Show();
            }
            else
            {

                MessageBox.Show("您沒有權限訪問管理介面！", "警告", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }


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

            tabUserMain.SelectedTab = tabPageHome;

            await RefreshAllSectionsAsync();
        }


        private async Task RefreshUserRecordsAsync()
        {
            var borrowTask = RefreshBorrowRecordsAsync();
            var reserveTask = RefreshReserveRecordsAsync();
            await Task.WhenAll(borrowTask, reserveTask);
        }


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


        private async Task RefreshBorrowRecordsAsync(string searchTerm = "")
        {
            try
            {

                var dt = await Task.Run(() => {
                    string sql = @"SELECT br.borrow_id, c.title, c.isbn, br.borrow_date, br.expected_return_date, br.return_date, 
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


                cachedBorrowRecords = dt;
                lastBorrowRefresh = DateTime.Now;


                await this.InvokeAsync(() => {

                    dgvBorrowRecord.SuspendLayout();
                    

                    dgvBorrowRecord.DataSource = dt;
                    

                    SetBorrowGridSettingsAndButtonStatus();


                    lblBorrowPage.Text = $"第 {currentBorrowPage} 頁";
                    btnBorrowPrev.Enabled = currentBorrowPage > 1;
                    

                    dgvBorrowRecord.ResumeLayout();
                });


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


        private void DgvBorrowRecord_DataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
        {
            if (dgvBorrowRecord.Columns.Count == 0) return;


            var operationCol = dgvBorrowRecord.Columns.OfType<DataGridViewButtonColumn>().FirstOrDefault(c => c.Name == "操作");
            if (operationCol == null)
            {
                operationCol = new DataGridViewButtonColumn
                {
                    Name = "操作",
                    HeaderText = "操作",
                    UseColumnTextForButtonValue = false,                     
                    Width = 80
                };
                dgvBorrowRecord.Columns.Add(operationCol);
            }
            else
            {
                operationCol.UseColumnTextForButtonValue = false;             }


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


        private async Task RefreshReserveRecordsAsync(string searchTerm = "")
        {
            try
            {
                string sql = @"SELECT r.reservation_id, c.title, c.isbn, r.reservation_date,
                             DATE_ADD(r.reservation_date, INTERVAL 24 HOUR) as expiry_date,
                             CASE
                                 -- 檢查是否有對應的借閱記錄（表示已完成預約）
                                 WHEN br.borrow_id IS NOT NULL AND br.borrow_date >= r.reservation_date THEN '已完成'
                                 WHEN r.status = 'active' AND r.reservation_date > DATE_SUB(NOW(), INTERVAL 24 HOUR) THEN '預約中'
                                 WHEN r.status = 'canceled' THEN '已取消'
                                 WHEN r.reservation_date <= DATE_SUB(NOW(), INTERVAL 24 HOUR) THEN '已過期'
                                 ELSE '未知'
                             END as status
                             FROM reservation r
                             JOIN comic c ON r.comic_id = c.comic_id
                             LEFT JOIN borrow_record br ON r.user_id = br.user_id AND r.comic_id = br.comic_id AND br.borrow_date >= r.reservation_date
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
                paramList.Add(new MySqlParameter("@pageSize", PageSize));


                DataTable dt = await Task.Run(() => DBHelper.ExecuteQuery(sql, paramList.ToArray()));


                cachedReserveRecords = dt;
                lastReserveRefresh = DateTime.Now;


                await this.InvokeAsync(() => {

                    dgvReserveRecord.SuspendLayout();
                    
                    dgvReserveRecord.DataSource = dt;
                    SetReserveGridSettingsAndButtonStatus();
                    

                    lblReservePage.Text = $"第 {currentReservePage} 頁";
                    btnReservePrev.Enabled = currentReservePage > 1;
                    

                    dgvReserveRecord.ResumeLayout();
                });


                await Task.Run(async () => {
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
                        countParamList.Add(new MySqlParameter("@search", "%" + searchTerm + "%"));
                    }
                    long totalRecords = Convert.ToInt64(DBHelper.ExecuteScalar(countSql, countParamList.ToArray()));
                    

                    await this.InvokeAsync(() => {
                        btnReserveNext.Enabled = (currentReservePage * PageSize) < totalRecords;
                    });
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show("載入預約紀錄時發生錯誤：" + ex.Message, "錯誤", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        private void SetComicsGridColumnSettings()
        {
            dgvComics.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            dgvComics.ColumnHeadersHeight = 24;
            dgvComics.ColumnHeadersDefaultCellStyle.Font = new System.Drawing.Font("Microsoft JhengHei UI", 10F, System.Drawing.FontStyle.Bold);
            if (dgvComics.Columns.Contains("comic_id")) { var col = dgvComics.Columns["comic_id"]; col.HeaderText = "書號"; col.Width = 80; col.AutoSizeMode = DataGridViewAutoSizeColumnMode.None; }
            if (dgvComics.Columns.Contains("isbn")) { var col = dgvComics.Columns["isbn"]; col.HeaderText = "ISBN"; col.Width = 120; col.AutoSizeMode = DataGridViewAutoSizeColumnMode.None; }
            if (dgvComics.Columns.Contains("title")) { var col = dgvComics.Columns["title"]; col.HeaderText = "書名"; col.Width = 440; col.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill; }
            if (dgvComics.Columns.Contains("author")) { var col = dgvComics.Columns["author"]; col.HeaderText = "作者"; col.Width = 180; col.AutoSizeMode = DataGridViewAutoSizeColumnMode.None; }
            if (dgvComics.Columns.Contains("publisher")) { var col = dgvComics.Columns["publisher"]; col.HeaderText = "出版社"; col.Width = 200; col.AutoSizeMode = DataGridViewAutoSizeColumnMode.None; }
            if (dgvComics.Columns.Contains("category")) { var col = dgvComics.Columns["category"]; col.HeaderText = "分類"; col.Width = 140; col.AutoSizeMode = DataGridViewAutoSizeColumnMode.None; }
            if (dgvComics.Columns.Contains("借閱狀態")) { var col = dgvComics.Columns["借閱狀態"]; col.HeaderText = "借閱狀態"; col.Width = 100; col.AutoSizeMode = DataGridViewAutoSizeColumnMode.None; }
            if (dgvComics.Columns.Contains("預約狀態")) { var col = dgvComics.Columns["預約狀態"]; col.HeaderText = "預約狀態"; col.Width = 120; col.AutoSizeMode = DataGridViewAutoSizeColumnMode.None; }
        }

        private bool borrowGridSettingsApplied = false;


        private void SetBorrowGridSettingsAndButtonStatus()
        {
            if (dgvBorrowRecord == null || dgvBorrowRecord.Columns.Count == 0) return;

            dgvBorrowRecord.Font = new System.Drawing.Font("Microsoft JhengHei UI", 10F);
            dgvBorrowRecord.ColumnHeadersDefaultCellStyle.Font = new System.Drawing.Font("Microsoft JhengHei UI", 10F, System.Drawing.FontStyle.Bold);
            dgvBorrowRecord.RowTemplate.Height = 36;
            dgvBorrowRecord.RowHeadersWidth = 60;
            dgvBorrowRecord.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.EnableResizing;
            dgvBorrowRecord.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvBorrowRecord.EnableHeadersVisualStyles = false;
            dgvBorrowRecord.ColumnHeadersDefaultCellStyle.BackColor = System.Drawing.Color.WhiteSmoke;

            var columnSettings = new Dictionary<string, (string Header, int? Width, DataGridViewAutoSizeColumnMode Mode)>()
            {
                { "borrow_id", ("編號", 60, DataGridViewAutoSizeColumnMode.None) },
                { "title", ("書名", 200, DataGridViewAutoSizeColumnMode.None) },
                { "isbn", ("ISBN", 100, DataGridViewAutoSizeColumnMode.None) },
                { "borrow_date", ("借閱日期", null, DataGridViewAutoSizeColumnMode.Fill) },
                { "expected_return_date", ("預期歸還日", null, DataGridViewAutoSizeColumnMode.Fill) },
                { "return_date", ("實際歸還日", null, DataGridViewAutoSizeColumnMode.Fill) },
                { "status", ("狀態", 80, DataGridViewAutoSizeColumnMode.None) }
            };

            foreach (var setting in columnSettings)
            {
                if (dgvBorrowRecord.Columns.Contains(setting.Key))
                {
                    var col = dgvBorrowRecord.Columns[setting.Key];
                    col.HeaderText = setting.Value.Header;
                    col.AutoSizeMode = setting.Value.Mode;
                    if (setting.Value.Mode == DataGridViewAutoSizeColumnMode.None && setting.Value.Width.HasValue)
                    {
                        col.Width = setting.Value.Width.Value;
                    }

                    if (setting.Key == "borrow_date" || setting.Key == "expected_return_date" || setting.Key == "return_date")
                    {
                        col.DefaultCellStyle.Format = "yyyy/MM/dd HH:mm:ss";
                    }
                }
            }


            var operationCol = dgvBorrowRecord.Columns.OfType<DataGridViewButtonColumn>().FirstOrDefault(c => c.Name == "操作");
            if (operationCol == null)
            {
                operationCol = new DataGridViewButtonColumn
                {
                    Name = "操作",
                    HeaderText = "操作",
                    UseColumnTextForButtonValue = false,
                    Width = 80,                 };
                dgvReserveRecord.Columns.Add(operationCol);
            }
            else
            {
                operationCol.UseColumnTextForButtonValue = false;
                operationCol.Width = 80;             }


            foreach (DataGridViewRow row in dgvReserveRecord.Rows)
            {
                if (row.IsNewRow) continue;
                var statusCell = row.Cells["status"];
                var operationCell = row.Cells["操作"] as DataGridViewButtonCell;
                if (statusCell != null && operationCell != null)
                {
                    string status = statusCell.Value?.ToString() ?? "";
                    if (status == "預約中")
                    {

                        int reservationId = 0;
                        if (dgvReserveRecord.Columns.Contains("reservation_id") && row.Cells["reservation_id"] != null && row.Cells["reservation_id"].Value != null) {
                            int.TryParse(row.Cells["reservation_id"].Value.ToString(), out reservationId);
                        }
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
                            operationCell.Value = status;
                            operationCell.ReadOnly = true;
                            if (operationCell.Style != null) operationCell.Style.ForeColor = System.Drawing.Color.Gray;
                        }
                    }
                    else
                    {
                        operationCell.Value = status == "" ? "操作" : status;
                        operationCell.ReadOnly = true;
                        if (operationCell.Style != null) operationCell.Style.ForeColor = System.Drawing.Color.Gray;
                    }
                }
            }
        }


        private void dgvBorrowRecord_Paint(object sender, PaintEventArgs e)
        {
            if (dgvBorrowRecord.Rows.Count > 0 && !borrowGridSettingsApplied)
            {
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


                var operationCol = dgvBorrowRecord.Columns.OfType<DataGridViewButtonColumn>().FirstOrDefault(c => c.Name == "操作");
                 if (operationCol != null)
                 {
                     operationCol.UseColumnTextForButtonValue = true;                  }


                borrowGridSettingsApplied = true;
            }
        }


        private void DgvReserveRecord_DataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
        {
            SetReserveGridSettingsAndButtonStatus();
        }


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

        private async Task<DataTable> QueryBorrowRecordsAsync()
        {
            string sql = @"
                SELECT br.borrow_id, br.comic_id, c.title as comic_title, br.borrow_date, br.return_date,
                       CASE 
                           WHEN br.return_date IS NULL AND NOW() > br.return_date THEN '逾期'
                           WHEN br.return_date IS NULL THEN '借閱中'
                           ELSE '已歸還'
                       END as status
                FROM borrow_record br
                JOIN comic c ON br.comic_id = c.comic_id
                WHERE br.user_id = @uid
                ORDER BY br.borrow_date DESC
                LIMIT @offset, @pageSize";

            return await Task.Run(() => DBHelper.ExecuteQuery(sql, new MySql.Data.MySqlClient.MySqlParameter[] {
                new MySql.Data.MySqlClient.MySqlParameter("@uid", loggedInUserId),
                new MySql.Data.MySqlClient.MySqlParameter("@offset", (currentBorrowPage - 1) * PageSize),
                new MySql.Data.MySqlClient.MySqlParameter("@pageSize", PageSize)
            }));
        }

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

        private async Task RefreshAllSectionsAsync()
        {
            var borrowTask = QueryBorrowRecordsAsync();
            var reserveTask = QueryReserveRecordsAsync();
            await Task.WhenAll(borrowTask, reserveTask);
        }

        private bool IsUserInCoolingPeriod(int userId, int comicId)
        {
            string sql = "SELECT return_date FROM borrow_record WHERE user_id = @uid AND comic_id = @cid AND return_date IS NOT NULL AND return_date > DATE_SUB(NOW(), INTERVAL 24 HOUR) ORDER BY return_date DESC LIMIT 1";
            var dt = DBHelper.ExecuteQuery(sql, new MySql.Data.MySqlClient.MySqlParameter[] {
                new MySql.Data.MySqlClient.MySqlParameter("@uid", userId),
                new MySql.Data.MySqlClient.MySqlParameter("@cid", comicId)
            });
            return dt.Rows.Count > 0;
        }


        private bool IsUserInReservationCoolingPeriod(int userId, int comicId)
        {
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


        private async void tabUserMain_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                switch (tabUserMain.SelectedTab.Text)
                {
                    case "首頁":
                        break;
                    case "借閱紀錄":
                        if (cachedBorrowRecords == null || 
                            (DateTime.Now - lastBorrowRefresh).TotalSeconds > CACHE_DURATION_SECONDS)
                        {
                            await RefreshBorrowRecordsAsync();
                        }
                        else
                        {
                            dgvBorrowRecord.DataSource = cachedBorrowRecords;
                            SetBorrowGridSettingsAndButtonStatus();
                        }
                        break;
                    case "預約紀錄":
                        if (cachedReserveRecords == null || 
                            (DateTime.Now - lastReserveRefresh).TotalSeconds > CACHE_DURATION_SECONDS)
                        {
                            await RefreshReserveRecordsAsync();
                        }
                        else
                        {
                            dgvReserveRecord.DataSource = cachedReserveRecords;
                            SetReserveGridSettingsAndButtonStatus();
                        }
                        break;
                    case "收藏紀錄":
                        if (cachedFavoriteRecords == null || 
                            (DateTime.Now - lastFavoriteRefresh).TotalSeconds > CACHE_DURATION_SECONDS)
                        {
                            await RefreshFavoriteRecordsAsync();
                        }
                        else
                        {
                            dgvFavoriteRecord.DataSource = cachedFavoriteRecords;
                            SetUserComicsGridColumnSettingsForFavorite();
                        }
                        break;
                    case "會員中心":
                        break;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("切換分頁時發生錯誤：" + ex.Message, "錯誤", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        private void SetupHomePageLayout()
        {
            tabPageHome.Controls.Clear();
            var panelSearch = new Panel { Dock = DockStyle.Top, Height = 40, BackColor = System.Drawing.Color.WhiteSmoke };
            var labelSearch = new Label { Text = "搜尋：", Location = new System.Drawing.Point(10, 10), Size = new System.Drawing.Size(60, 20), Font = new System.Drawing.Font("Microsoft JhengHei UI", 10F) };
            var txtSearch = new TextBox { Name = "txtSearch", Location = new System.Drawing.Point(70, 6), Size = new System.Drawing.Size(180, 27), Font = new System.Drawing.Font("Microsoft JhengHei UI", 10F) };
            var cmbSearchType = new ComboBox { Name = "cmbSearchType", Location = new System.Drawing.Point(260, 6), Size = new System.Drawing.Size(80, 27), Font = new System.Drawing.Font("Microsoft JhengHei UI", 10F), DropDownStyle = ComboBoxStyle.DropDownList };
            cmbSearchType.Items.AddRange(new object[] { "全部", "書名", "ISBN", "作者", "出版社", "分類" });
            cmbSearchType.SelectedIndex = 0;
            var btnSearch = new Button { Name = "btnSearch", Text = "搜尋", Location = new System.Drawing.Point(350, 6), Size = new System.Drawing.Size(60, 27), Font = new System.Drawing.Font("Microsoft JhengHei UI", 10F) };
            btnSearch.Click += async (s, e) => await RefreshUserComicsGrid(txtSearch.Text, cmbSearchType.SelectedItem?.ToString());
            var btnRefresh = new Button { Name = "btnHomeRefresh", Text = "刷新", Location = new System.Drawing.Point(420, 6), Size = new System.Drawing.Size(60, 27), Font = new System.Drawing.Font("Microsoft JhengHei UI", 10F) };
            btnRefresh.Click += async (s, e) => await RefreshUserComicsGrid(txtSearch.Text, cmbSearchType.SelectedItem?.ToString());
            panelSearch.Controls.Add(labelSearch);
            panelSearch.Controls.Add(txtSearch);
            panelSearch.Controls.Add(cmbSearchType);
            panelSearch.Controls.Add(btnSearch);
            panelSearch.Controls.Add(btnRefresh);

            var panelPaging = new Panel { Dock = DockStyle.Bottom, Height = 40, BackColor = System.Drawing.Color.WhiteSmoke };
            btnComicPrev = new Button { Name = "btnComicPrev", Text = "上一頁", Location = new System.Drawing.Point(10, 6), Size = new System.Drawing.Size(80, 27), Font = new System.Drawing.Font("Microsoft JhengHei UI", 10F) };
            lblComicPage = new Label { Name = "lblComicPage", Text = "第 1 頁", Location = new System.Drawing.Point(100, 10), Size = new System.Drawing.Size(100, 20), Font = new System.Drawing.Font("Microsoft JhengHei UI", 10F), TextAlign = ContentAlignment.MiddleCenter };
            btnComicNext = new Button { Name = "btnComicNext", Text = "下一頁", Location = new System.Drawing.Point(210, 6), Size = new System.Drawing.Size(80, 27), Font = new System.Drawing.Font("Microsoft JhengHei UI", 10F) };
            btnComicPrev.Click += async (s, e) => {
                if (currentComicPage > 1)
                {
                    currentComicPage--;
                    await RefreshUserComicsGrid(txtSearch.Text, cmbSearchType.SelectedItem?.ToString());
                }
            };
            btnComicNext.Click += async (s, e) => {
                currentComicPage++;
                await RefreshUserComicsGrid(txtSearch.Text, cmbSearchType.SelectedItem?.ToString());
            };
            panelPaging.Controls.Add(btnComicPrev);
            panelPaging.Controls.Add(lblComicPage);
            panelPaging.Controls.Add(btnComicNext);

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

            SetUserComicsGridColumnSettings();
            ApplyUserComicsGridGlobalSettings(dgvUserComics);

            dgvUserComics.CellContentClick += DgvUserComics_CellContentClick;
            tabPageHome.Controls.Add(dgvUserComics);
            tabPageHome.Controls.Add(panelSearch);
            tabPageHome.Controls.Add(panelPaging);
        }

        private void SetupBorrowPageLayout()
        {
            tabPageBorrow.Controls.Clear();
            
            var panelSearch = new Panel { Dock = DockStyle.Top, Height = 40, BackColor = System.Drawing.Color.WhiteSmoke };
            var labelSearch = new Label { Text = "搜尋：", Location = new System.Drawing.Point(10, 10), Size = new System.Drawing.Size(60, 20), Font = new System.Drawing.Font("Microsoft JhengHei UI", 10F) };
            var txtSearch = new TextBox { Name = "txtBorrowSearch", Location = new System.Drawing.Point(70, 6), Size = new System.Drawing.Size(180, 27), Font = new System.Drawing.Font("Microsoft JhengHei UI", 10F) };
            var btnSearch = new Button { Name = "btnBorrowSearch", Text = "搜尋", Location = new System.Drawing.Point(260, 6), Size = new System.Drawing.Size(60, 27), Font = new System.Drawing.Font("Microsoft JhengHei UI", 10F) };
            btnSearch.Click += async (s, e) => await RefreshBorrowRecordsAsync(txtSearch.Text);
            var btnRefresh = new Button { Name = "btnBorrowRefresh", Text = "刷新", Location = new System.Drawing.Point(330, 6), Size = new System.Drawing.Size(60, 27), Font = new System.Drawing.Font("Microsoft JhengHei UI", 10F) };
            btnRefresh.Click += async (s, e) => await RefreshBorrowRecordsAsync(txtSearch.Text);
            panelSearch.Controls.Add(labelSearch);
            panelSearch.Controls.Add(txtSearch);
            panelSearch.Controls.Add(btnSearch);
            panelSearch.Controls.Add(btnRefresh);

            var panelPaging = new Panel { Dock = DockStyle.Bottom, Height = 40, BackColor = System.Drawing.Color.WhiteSmoke };
            var btnPrev = new Button { Name = "btnBorrowPrev", Text = "上一頁", Location = new System.Drawing.Point(10, 6), Size = new System.Drawing.Size(80, 27), Font = new System.Drawing.Font("Microsoft JhengHei UI", 10F) };
            var lblPage = new Label { Name = "lblBorrowPage", Text = "第 1 頁", Location = new System.Drawing.Point(100, 10), Size = new System.Drawing.Size(100, 20), Font = new System.Drawing.Font("Microsoft JhengHei UI", 10F), TextAlign = ContentAlignment.MiddleCenter };
            var btnNext = new Button { Name = "btnBorrowNext", Text = "下一頁", Location = new System.Drawing.Point(210, 6), Size = new System.Drawing.Size(80, 27), Font = new System.Drawing.Font("Microsoft JhengHei UI", 10F) };
            btnPrev.Click += btnBorrowPrev_Click;
            btnNext.Click += btnBorrowNext_Click;
            panelPaging.Controls.Add(btnPrev);
            panelPaging.Controls.Add(lblPage);
            panelPaging.Controls.Add(btnNext);

            dgvBorrowRecord = new DataGridView
            {
                Dock = DockStyle.Fill,
                Font = new System.Drawing.Font("Microsoft JhengHei UI", 10F),
                ColumnHeadersDefaultCellStyle = { Font = new System.Drawing.Font("Microsoft JhengHei UI", 10F, System.Drawing.FontStyle.Bold) },
                RowTemplate = { Height = 36 },
                AllowUserToAddRows = false,
                ReadOnly = true,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false,
                Name = "dgvBorrowRecord",
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize,
                EnableHeadersVisualStyles = false,
                ColumnHeadersVisible = true
            };

            dgvBorrowRecord.CellContentClick += DgvBorrowRecord_CellContentClick;
            dgvBorrowRecord.Paint += dgvBorrowRecord_Paint;
            dgvBorrowRecord.DataBindingComplete += DgvBorrowRecord_DataBindingComplete;

            tabPageBorrow.Controls.Add(dgvBorrowRecord);
            tabPageBorrow.Controls.Add(panelSearch);
            tabPageBorrow.Controls.Add(panelPaging);
        }

        private void SetupReservePageLayout()
        {
            tabPageReserve.Controls.Clear();

            var panelSearch = new Panel { Dock = DockStyle.Top, Height = 40, BackColor = System.Drawing.Color.WhiteSmoke };
            var labelSearch = new Label { Text = "搜尋：", Location = new System.Drawing.Point(10, 10), Size = new System.Drawing.Size(60, 20), Font = new System.Drawing.Font("Microsoft JhengHei UI", 10F) };
            var txtSearch = new TextBox { Name = "txtReserveSearch", Location = new System.Drawing.Point(70, 6), Size = new System.Drawing.Size(180, 27), Font = new System.Drawing.Font("Microsoft JhengHei UI", 10F) };
            var btnSearch = new Button { Name = "btnReserveSearch", Text = "搜尋", Location = new System.Drawing.Point(260, 6), Size = new System.Drawing.Size(60, 27), Font = new System.Drawing.Font("Microsoft JhengHei UI", 10F) };
            btnSearch.Click += async (s, e) => await RefreshReserveRecordsAsync(txtSearch.Text);
            panelSearch.Controls.Add(labelSearch);
            panelSearch.Controls.Add(txtSearch);
            panelSearch.Controls.Add(btnSearch);

            var panelPaging = new Panel { Dock = DockStyle.Bottom, Height = 40, BackColor = System.Drawing.Color.WhiteSmoke };
            btnReservePrev = new Button { Name = "btnReservePrev", Text = "上一頁", Location = new System.Drawing.Point(10, 6), Size = new System.Drawing.Size(80, 27), Font = new System.Drawing.Font("Microsoft JhengHei UI", 10F) };
            lblReservePage = new Label { Name = "lblReservePage", Text = "第 1 頁", Location = new System.Drawing.Point(100, 10), Size = new System.Drawing.Size(100, 20), Font = new System.Drawing.Font("Microsoft JhengHei UI", 10F), TextAlign = ContentAlignment.MiddleCenter };
            btnReserveNext = new Button { Name = "btnReserveNext", Text = "下一頁", Location = new System.Drawing.Point(210, 6), Size = new System.Drawing.Size(80, 27), Font = new System.Drawing.Font("Microsoft JhengHei UI", 10F) };
            btnReservePrev.Click += btnReservePrev_Click;
            btnReserveNext.Click += btnReserveNext_Click;
            panelPaging.Controls.Add(btnReservePrev);
            panelPaging.Controls.Add(btnReserveNext);
            panelPaging.Controls.Add(lblReservePage);

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
                BackgroundColor = System.Drawing.Color.White,
                ScrollBars = ScrollBars.Vertical             };
            dgvReserveRecord.EnableHeadersVisualStyles = false;
            dgvReserveRecord.DataBindingComplete += DgvReserveRecord_DataBindingComplete;

            dgvReserveRecord.CellContentClick += DgvReserveRecord_CellContentClick;
            tabPageReserve.Controls.Add(dgvReserveRecord);
            tabPageReserve.Controls.Add(panelPaging);
            tabPageReserve.Controls.Add(panelSearch);
        }


        private void SetupFavoritePageLayout()
        {
            tabPageFavorite.Controls.Clear();

            var panelSearch = new Panel { Dock = DockStyle.Top, Height = 40, BackColor = System.Drawing.Color.WhiteSmoke };
            var labelSearch = new Label { Text = "搜尋：", Location = new System.Drawing.Point(10, 10), Size = new System.Drawing.Size(60, 20), Font = new System.Drawing.Font("Microsoft JhengHei UI", 10F) };
            var txtSearch = new TextBox { Name = "txtFavoriteSearch", Location = new System.Drawing.Point(70, 6), Size = new System.Drawing.Size(180, 27), Font = new System.Drawing.Font("Microsoft JhengHei UI", 10F) };
            var btnSearch = new Button { Name = "btnFavoriteSearch", Text = "搜尋", Location = new System.Drawing.Point(260, 6), Size = new System.Drawing.Size(60, 27), Font = new System.Drawing.Font("Microsoft JhengHei UI", 10F) };
            btnSearch.Click += async (s, e) => { currentFavoritePage = 1; await RefreshFavoriteRecordsAsync(txtSearch.Text); };
            panelSearch.Controls.Add(labelSearch);
            panelSearch.Controls.Add(txtSearch);
            panelSearch.Controls.Add(btnSearch);


            var panelPaging = new Panel { Dock = DockStyle.Bottom, Height = 40, BackColor = System.Drawing.Color.WhiteSmoke };
            btnFavoritePrev = new Button { Name = "btnFavoritePrev", Text = "上一頁", Location = new System.Drawing.Point(10, 6), Size = new System.Drawing.Size(80, 27), Font = new System.Drawing.Font("Microsoft JhengHei UI", 10F) };
            lblFavoritePage = new Label { Name = "lblFavoritePage", Text = "第 1 頁", Location = new System.Drawing.Point(100, 10), Size = new System.Drawing.Size(100, 20), Font = new System.Drawing.Font("Microsoft JhengHei UI", 10F), TextAlign = ContentAlignment.MiddleCenter };
            btnFavoriteNext = new Button { Name = "btnFavoriteNext", Text = "下一頁", Location = new System.Drawing.Point(210, 6), Size = new System.Drawing.Size(80, 27), Font = new System.Drawing.Font("Microsoft JhengHei UI", 10F) };
            btnFavoritePrev.Click += async (s, e) => { if (currentFavoritePage > 1) { currentFavoritePage--; await RefreshFavoriteRecordsAsync(); } };
            btnFavoriteNext.Click += async (s, e) => { currentFavoritePage++; await RefreshFavoriteRecordsAsync(); };
            panelPaging.Controls.Add(btnFavoritePrev);
            panelPaging.Controls.Add(lblFavoritePage);
            panelPaging.Controls.Add(btnFavoriteNext);


            dgvFavoriteRecord = new DataGridView
            {
                Name = "dgvFavoriteRecord",
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
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,                 EnableHeadersVisualStyles = false,
                BackgroundColor = System.Drawing.Color.White,
                ScrollBars = ScrollBars.Vertical
            };
            typeof(DataGridView).InvokeMember(
                "DoubleBuffered",
                System.Reflection.BindingFlags.SetProperty |
                System.Reflection.BindingFlags.Instance |
                System.Reflection.BindingFlags.NonPublic,
                null,
                dgvFavoriteRecord,
                new object[] { true });

            dgvFavoriteRecord.CellContentClick += DgvFavoriteRecord_CellContentClick;
            tabPageFavorite.Controls.Add(dgvFavoriteRecord);
            tabPageFavorite.Controls.Add(panelPaging);
            tabPageFavorite.Controls.Add(panelSearch);
        }


        private int lastActionComicId = 0;

        private async Task RefreshUserComicsGrid(string searchTerm, string searchType)
        {
            try
            {

                string sql = @"SELECT 
                    c.comic_id AS 書號, 
                    c.isbn AS ISBN, 
                    c.title AS 書名, 
                    c.author AS 作者, 
                    c.publisher AS 出版社, 
                    c.category AS 分類,
                    c.image_path AS 圖片URL,
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


                sql += " ORDER BY c.comic_id LIMIT @offset, @pageSize";
                paramList.Add(new MySqlParameter("@offset", (currentComicPage - 1) * ComicPageSize));
                paramList.Add(new MySqlParameter("@pageSize", ComicPageSize));


                DataTable dt = await Task.Run(() => DBHelper.ExecuteQuery(sql, paramList.ToArray()));


                string countSql = "SELECT COUNT(*) FROM comic c WHERE 1=1";
                if (!string.IsNullOrWhiteSpace(searchTerm))
                {
                    if (searchType == "全部")
                    {
                        countSql += " AND (c.comic_id LIKE @kw OR c.title LIKE @kw OR c.author LIKE @kw OR c.category LIKE @kw OR c.isbn LIKE @kw OR c.publisher LIKE @kw)";
                    }
                    else if (searchType == "書名")
                    {
                        countSql += " AND c.title LIKE @kw";
                    }
                    else if (searchType == "ISBN")
                    {
                        countSql += " AND c.isbn LIKE @kw";
                    }
                    else if (searchType == "作者")
                    {
                        countSql += " AND c.author LIKE @kw";
                    }
                    else if (searchType == "出版社")
                    {
                        countSql += " AND c.publisher LIKE @kw";
                    }
                    else if (searchType == "分類")
                    {
                        countSql += " AND c.category LIKE @kw";
                    }
                }
                long totalRecords = await Task.Run(() => Convert.ToInt64(DBHelper.ExecuteScalar(countSql, paramList.ToArray())));


                await this.InvokeAsync(() => {
                    dgvUserComics.SuspendLayout();
                    dgvUserComics.DataSource = dt;                     SetUserComicsGridColumnSettings();                     ApplyUserComicsGridGlobalSettings(dgvUserComics);                     UpdateComicsButtonColumnStates();                     lblComicPage.Text = $"第 {currentComicPage} 頁";
                    btnComicPrev.Enabled = currentComicPage > 1;
                    btnComicNext.Enabled = (currentComicPage * ComicPageSize) < totalRecords;
                    dgvUserComics.ResumeLayout();
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




            var columnSettings = new List<(string Name, string Header, int Width)>()
            {
                ("書號", "編號", 70),
                ("ISBN", "ISBN", 70),
                ("書名", "書名", 175),
                ("作者", "作者", 80),
                ("出版社", "出版社", 90),                 ("分類", "分類", 59),                 ("詳情", "詳情", 58),                 ("收藏", "收藏", 58),                 ("借閱狀態", "借閱狀態", 76),
                ("預約狀態", "預約狀態", 76),
                ("借書", "借書", 76),
                ("預約", "預約", 76)
            };


            if (dgvUserComics.Columns["詳情"] == null)
            {
                var btnDetails = new DataGridViewButtonColumn
                {
                    Name = "詳情",
                    HeaderText = "詳情",
                    Text = "詳情",
                    UseColumnTextForButtonValue = false,
                    Width = 58                 };
                dgvUserComics.Columns.Add(btnDetails);
            }

            if (dgvUserComics.Columns["收藏"] == null)
            {
                var btnFavorite = new DataGridViewButtonColumn
                {
                    Name = "收藏",
                    HeaderText = "收藏",
                    Text = "收藏",                     UseColumnTextForButtonValue = false,                     Width = 58                 };
                dgvUserComics.Columns.Add(btnFavorite);
            }
            if (dgvUserComics.Columns["借書"] == null)
            {
                var btnRent = new DataGridViewButtonColumn
                {
                    Name = "借書",
                    HeaderText = "借書",
                    Text = "借書",                     UseColumnTextForButtonValue = false,                     Width = 76
                };
                dgvUserComics.Columns.Add(btnRent);
            }
            if (dgvUserComics.Columns["預約"] == null)
            {
                var btnReserve = new DataGridViewButtonColumn
                {
                    Name = "預約",
                    HeaderText = "預約",
                    Text = "預約",                     UseColumnTextForButtonValue = false,                     Width = 76
                };
                dgvUserComics.Columns.Add(btnReserve);
            }

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

                        if (!(col is DataGridViewButtonColumn))
                        {
                            col.DataPropertyName = setting.Name;                         }
                        col.DisplayIndex = displayIndex++;


                        if (setting.Name == "詳情" || setting.Name == "收藏" || setting.Name == "借書" || setting.Name == "預約")
                        {
                            col.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
                        }

                    }
                    catch (ArgumentOutOfRangeException ex)
                    {
                        System.Diagnostics.Debug.WriteLine($"Error setting DisplayIndex for column {setting.Name}: {ex.Message}");

                        col.DisplayIndex = dgvUserComics.Columns.Count > 0 ? dgvUserComics.Columns.Count - 1 : 0;                     }
                }
            }

            foreach (var col in currentColumns)
            {
                if (!columnSettings.Any(setting => setting.Name == col.Name))
                {
                    col.Visible = false;
                }
                else
                {
                    col.Visible = true;                 }
            }



        }


        private void UpdateComicsButtonColumnStates()
        {
            if (dgvUserComics == null || dgvUserComics.Rows.Count == 0) return;

            if (!dgvUserComics.Columns.Contains("借書") || !dgvUserComics.Columns.Contains("預約")) return;


            var borrowCoolingSet = GetUserBorrowCoolingComicIds();
            var reserveCoolingSet = GetUserReserveCoolingComicIds();
            var favoriteSet = GetUserFavoriteComicIds(); 
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


                var cellDetails = row.Cells["詳情"] as DataGridViewButtonCell;
                if (cellDetails != null)
                {
                    cellDetails.Value = "詳情";                     cellDetails.ReadOnly = false;                 }


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


                var cellFavorite = row.Cells["收藏"] as DataGridViewButtonCell;
                if (cellFavorite != null)
                {
                    if (favoriteSet.Contains(comicId))
                    {
                        cellFavorite.Value = "已收藏";
                        cellFavorite.Style.ForeColor = System.Drawing.Color.Red;
                    }
                    else
                    {
                        cellFavorite.Value = "收藏";
                        cellFavorite.Style.ForeColor = System.Drawing.Color.Black;
                    }
                }
            }


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


            if (dgvUserComics.Columns.Contains("借書"))
            {
                dgvUserComics.Columns["借書"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
                dgvUserComics.Columns["借書"].Width = 76;
            }
            if (dgvUserComics.Columns.Contains("預約"))
            {
                dgvUserComics.Columns["預約"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
                dgvUserComics.Columns["預約"].Width = 76;
            }
        }


        private async void DgvUserComics_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && e.ColumnIndex >= 0)
            {
                var columnName = dgvUserComics.Columns[e.ColumnIndex].Name;
                var comicId = Convert.ToInt32(dgvUserComics.Rows[e.RowIndex].Cells["書號"].Value);

                if (columnName == "詳情")                 {
                    var comic = GetComicById(comicId);
                    if (comic != null)
                    {

                        var detailsForm = new ComicDetailsForm(comic);


                        detailsForm.Size = new Size(this.Width / 2, this.Height);


                        detailsForm.ShowDialog(this);
                    }
                }
                else if (columnName == "收藏")
                {

                    bool isNowFavorite = false;

                    string checkSql = "SELECT 1 FROM user_favorites WHERE user_id = @uid AND comic_id = @cid LIMIT 1";
                    var dt = DBHelper.ExecuteQuery(checkSql, new MySql.Data.MySqlClient.MySqlParameter[] {
                        new MySql.Data.MySqlClient.MySqlParameter("@uid", loggedInUserId),
                        new MySql.Data.MySqlClient.MySqlParameter("@cid", comicId)
                    });
                    if (dt.Rows.Count > 0)
                    {

                        DBHelper.ExecuteNonQuery("DELETE FROM user_favorites WHERE user_id = @uid AND comic_id = @cid",
                            new MySql.Data.MySqlClient.MySqlParameter[] {
                                new MySql.Data.MySqlClient.MySqlParameter("@uid", loggedInUserId),
                                new MySql.Data.MySqlClient.MySqlParameter("@cid", comicId)
                            });
                        MessageBox.Show("取消收藏成功！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        isNowFavorite = false;
                    }
                    else
                    {

                        DBHelper.ExecuteNonQuery("INSERT INTO user_favorites (user_id, comic_id) VALUES (@uid, @cid)",
                            new MySql.Data.MySqlClient.MySqlParameter[] {
                                new MySql.Data.MySqlClient.MySqlParameter("@uid", loggedInUserId),
                                new MySql.Data.MySqlClient.MySqlParameter("@cid", comicId)
                            });
                        MessageBox.Show("收藏成功！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        isNowFavorite = true;
                    }

                    string checkAgainSql = "SELECT 1 FROM user_favorites WHERE user_id = @uid AND comic_id = @cid LIMIT 1";
                    var dtAgain = DBHelper.ExecuteQuery(checkAgainSql, new MySql.Data.MySqlClient.MySqlParameter[] {
                        new MySql.Data.MySqlClient.MySqlParameter("@uid", loggedInUserId),
                        new MySql.Data.MySqlClient.MySqlParameter("@cid", comicId)
                    });
                    var cellFavorite = dgvUserComics.Rows[e.RowIndex].Cells["收藏"] as DataGridViewButtonCell;
                    if (cellFavorite != null)
                    {
                        if (dtAgain.Rows.Count > 0)
                        {
                            cellFavorite.Value = "已收藏";
                            cellFavorite.Style.ForeColor = System.Drawing.Color.Red;
                        }
                        else
                        {
                            cellFavorite.Value = "收藏";
                            cellFavorite.Style.ForeColor = System.Drawing.Color.Black;
                        }
                    }

                    await RefreshFavoriteRecordsAsync();
                    return;
                }
            }
            else if (e.RowIndex < 0 || dgvUserComics.CurrentRow == null) return;
            var row = dgvUserComics.Rows[e.RowIndex];

            var clickedCell = row.Cells[e.ColumnIndex];

            if (clickedCell is DataGridViewButtonCell && clickedCell.OwningColumn.Name == "借書")
            {

                int comicId = 0;
                if (dgvUserComics.Columns.Contains("書號") && row.Cells["書號"] != null && row.Cells["書號"].Value != null)
                    int.TryParse(row.Cells["書號"].Value.ToString(), out comicId);
                if (comicId == 0) return;
                lastActionComicId = comicId; 
                string searchTerm = "";
                string searchType = "全部";

                var homeTabPage = tabUserMain.TabPages["tabPageHome"];
                var txtSearch = homeTabPage.Controls.OfType<Panel>().FirstOrDefault()?.Controls.OfType<TextBox>().FirstOrDefault(c => c.Name == "txtSearch");
                var cmbSearchType = homeTabPage.Controls.OfType<Panel>().FirstOrDefault()?.Controls.OfType<ComboBox>().FirstOrDefault(c => c.Name == "cmbSearchType");

                if (txtSearch != null) searchTerm = txtSearch.Text;
                if (cmbSearchType != null && cmbSearchType.SelectedItem != null) searchType = cmbSearchType.SelectedItem.ToString();

                await HandleRentAction(comicId);

                await RefreshUserComicsGrid(searchTerm, searchType); 
                await RefreshBorrowRecordsAsync();                 await RefreshReserveRecordsAsync();             }
            else if (clickedCell is DataGridViewButtonCell && clickedCell.OwningColumn.Name == "預約")
            {

                int comicId = 0;
                if (dgvUserComics.Columns.Contains("書號") && row.Cells["書號"] != null && row.Cells["書號"].Value != null)
                    int.TryParse(row.Cells["書號"].Value.ToString(), out comicId);
                if (comicId == 0) return;
                 lastActionComicId = comicId; 
                string searchTerm = "";
                string searchType = "全部";

                var homeTabPage = tabUserMain.TabPages["tabPageHome"];
                var txtSearch = homeTabPage.Controls.OfType<Panel>().FirstOrDefault()?.Controls.OfType<TextBox>().FirstOrDefault(c => c.Name == "txtSearch");
                var cmbSearchType = homeTabPage.Controls.OfType<Panel>().FirstOrDefault()?.Controls.OfType<ComboBox>().FirstOrDefault(c => c.Name == "cmbSearchType");

                if (txtSearch != null) searchTerm = txtSearch.Text;
                if (cmbSearchType != null && cmbSearchType.SelectedItem != null) searchType = cmbSearchType.SelectedItem.ToString();


                string reserveStatus = (dgvUserComics.Columns.Contains("預約狀態") && row.Cells["預約狀態"] != null) ? (row.Cells["預約狀態"].Value?.ToString() ?? "") : "";
                if (reserveStatus == "已被預約")
                {

                    string sql = "SELECT user_id FROM reservation WHERE comic_id = @cid AND status = 'active' AND reservation_date > DATE_SUB(NOW(), INTERVAL 24 HOUR)";
                    var dt = DBHelper.ExecuteQuery(sql, new MySql.Data.MySqlClient.MySqlParameter[] {
                        new MySql.Data.MySqlClient.MySqlParameter("@cid", comicId)
                    });
                    int reservedUserId = dt.Rows.Count > 0 ? Convert.ToInt32(dt.Rows[0]["user_id"]) : -1;

                    if (reservedUserId == loggedInUserId)
                    {
                await HandleReserveAction(comicId);

                        await RefreshUserComicsGrid(searchTerm, searchType);

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


                    await HandleReserveAction(comicId);

                    await RefreshUserComicsGrid(searchTerm, searchType);

                    await RefreshReserveRecordsAsync();
                    await RefreshAllSectionsAsync();
                }
                 else
                 {

                      MessageBox.Show("預約狀態異常，無法執行操作。", "錯誤", MessageBoxButtons.OK, MessageBoxIcon.Error);
                 }
            }
        }






        private void InitializeUserTabs()
        {

            tabPageHome = new TabPage("首頁");
            tabPageHome.Name = "tabPageHome";
            tabPageBorrow = new TabPage("借閱紀錄");
            tabPageBorrow.Name = "tabPageBorrow";
            tabPageReserve = new TabPage("預約紀錄");
            tabPageReserve.Name = "tabPageReserve";
            tabPageFavorite = new TabPage("收藏紀錄");             tabPageFavorite.Name = "tabPageFavorite";


            tabUserMain = new TabControl();
            tabUserMain.Name = "tabUserMain";
            tabUserMain.Dock = DockStyle.Fill;
            tabUserMain.TabPages.Add(tabPageHome);
            tabUserMain.TabPages.Add(tabPageBorrow);
            tabUserMain.TabPages.Add(tabPageReserve);
            tabUserMain.TabPages.Add(tabPageFavorite); 

            this.Controls.Add(tabUserMain);


            tabUserMain.SelectedIndexChanged += tabUserMain_SelectedIndexChanged;
            SetupHomePageLayout();
            SetupBorrowPageLayout();
            SetupReservePageLayout();
            SetupFavoritePageLayout();         }



        private async Task HandleRentAction(int comicId)
        {
            try
            {

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
                else if (!isBorrowed)                 {

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


                    using (var selectDateForm = new SelectReturnDateForm(DateTime.Now))
                    {
                        if (selectDateForm.ShowDialog() == DialogResult.OK)
                        {
                            DateTime selectedReturnDate = selectDateForm.SelectedDate;
                            

                            string sqlBorrowInsert = "INSERT INTO borrow_record (user_id, comic_id, borrow_date, expected_return_date) VALUES (@uid, @cid, NOW(), @returnDate)";
                            int rowsAffected = DBHelper.ExecuteNonQuery(sqlBorrowInsert, new MySql.Data.MySqlClient.MySqlParameter[] {
                                new MySql.Data.MySqlClient.MySqlParameter("@uid", loggedInUserId),
                                new MySql.Data.MySqlClient.MySqlParameter("@cid", comicId),
                                new MySql.Data.MySqlClient.MySqlParameter("@returnDate", selectedReturnDate)
                            });

                            if (rowsAffected > 0)
                            {

                                string sqlCancelReserve = "UPDATE reservation SET status = 'canceled' WHERE comic_id = @cid AND user_id = @uid AND status = 'active' AND reservation_date > DATE_SUB(NOW(), INTERVAL 24 HOUR)";
                                await Task.Run(() => DBHelper.ExecuteNonQuery(sqlCancelReserve, new MySql.Data.MySqlClient.MySqlParameter[] {
                                    new MySql.Data.MySqlClient.MySqlParameter("@cid", comicId),
                                    new MySql.Data.MySqlClient.MySqlParameter("@uid", loggedInUserId)
                                }));
                                MessageBox.Show("借書成功！請在 " + selectedReturnDate.ToString("yyyy/MM/dd HH:mm:ss") + " 前歸還。", "成功", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                

                                await RefreshUserComicsGrid("", "全部");                                 await RefreshAllSectionsAsync();                             }
                            else
                            {
                                MessageBox.Show("借書失敗！", "錯誤", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                        }
                        else
                        {

                            MessageBox.Show("已取消借書操作。", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
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


        private async void DgvBorrowRecord_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

            if (e.RowIndex < 0 || dgvBorrowRecord.Columns[e.ColumnIndex].Name != "操作") return;

            var row = dgvBorrowRecord.Rows[e.RowIndex];
            var statusCell = row.Cells["status"];
            var operationCell = row.Cells["操作"] as DataGridViewButtonCell;


            if (statusCell != null && operationCell != null)
            {
                string status = statusCell.Value?.ToString() ?? "";

                if (status == "未還")
                {

                    int borrowId = 0;
                    if (dgvBorrowRecord.Columns.Contains("borrow_id") && row.Cells["borrow_id"] != null && row.Cells["borrow_id"].Value != null)
                    {
                         int.TryParse(row.Cells["borrow_id"].Value.ToString(), out borrowId);
                    }
                    
                    if (borrowId == 0) return; 
                    try
                    {

                        string sql = "UPDATE borrow_record SET return_date = NOW() WHERE borrow_id = @bid";
                        int rowsAffected = await Task.Run(() => DBHelper.ExecuteNonQuery(sql, new MySql.Data.MySqlClient.MySqlParameter[] {
                            new MySql.Data.MySqlClient.MySqlParameter("@bid", borrowId)
                        }));

                        if (rowsAffected > 0)
                        {
                            MessageBox.Show("還書成功！", "成功", MessageBoxButtons.OK, MessageBoxIcon.Information);

                            await RefreshBorrowRecordsAsync();

                            await RefreshUserComicsGrid("", "全部");                         }
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

            }
        }


        private async void DgvReserveRecord_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

            if (e.RowIndex < 0 || dgvReserveRecord.Columns.Count <= e.ColumnIndex || dgvReserveRecord.Columns[e.ColumnIndex].Name != "操作") return;

            var row = dgvReserveRecord.Rows[e.RowIndex];
            var clickedCell = row.Cells[e.ColumnIndex] as DataGridViewButtonCell;


            if (clickedCell != null && clickedCell.Value?.ToString() == "取消預約")
            {

                int reservationId = 0;
                if (dgvReserveRecord.Columns.Contains("reservation_id") && row.Cells["reservation_id"] != null && row.Cells["reservation_id"].Value != null)
                {
                     int.TryParse(row.Cells["reservation_id"].Value.ToString(), out reservationId);
                }

                if (reservationId == 0) return; 
                try
                {

                    string sqlComicId = "SELECT comic_id FROM reservation WHERE reservation_id = @rid";
                    var dtComic = DBHelper.ExecuteQuery(sqlComicId, new MySql.Data.MySqlClient.MySqlParameter[] {
                        new MySql.Data.MySqlClient.MySqlParameter("@rid", reservationId)
                    });

                    if (dtComic.Rows.Count > 0)
                    {
                        int comicId = Convert.ToInt32(dtComic.Rows[0]["comic_id"]);


                        await HandleReserveAction(comicId);


                        await RefreshReserveRecordsAsync();
                        await RefreshUserComicsGrid("", "全部");                     }
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


        private ComicDetails GetComicById(int comicId)
        {
            try
            {
                string sql = @"SELECT 
                                c.comic_id, 
                                c.isbn AS ISBN, 
                                c.title AS 書名, 
                                c.author AS 作者, 
                                c.publisher AS 出版社, 
                                c.category AS 分類,
                                c.image_path AS 圖片URL,
                                c.offer_date AS 出版日,
                                c.pages AS 頁數,
                                c.book_summary AS 摘要,
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
                            ) r ON c.comic_id = r.comic_id
                            WHERE c.comic_id = @comicId";

                MySqlParameter[] parameters = {
                    new MySqlParameter("@comicId", comicId)
                };

                DataTable dt = DBHelper.ExecuteQuery(sql, parameters);

                if (dt.Rows.Count > 0)
                {
                    DataRow row = dt.Rows[0];
                    return new ComicDetails
                    {
                        書名 = row["書名"].ToString(),
                        作者 = row["作者"].ToString(),
                        出版社 = row["出版社"].ToString(),
                        分類 = row["分類"].ToString(),
                        ISBN = row["ISBN"].ToString(),
                        OfferDate = row["出版日"]?.ToString(),
                        Pages = row["頁數"]?.ToString(),
                        BookSummary = row["摘要"]?.ToString(),
                        ImageUrl = row["圖片URL"]?.ToString()
                    };
                }
                else
                {
                    return null;                 }
            }
            catch (Exception ex)
            {
                MessageBox.Show("獲取漫畫詳細資訊時發生錯誤：" + ex.Message, "錯誤", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;
            }
        }


        private HashSet<int> GetUserFavoriteComicIds()
        {
            var result = new HashSet<int>();
            string sql = "SELECT comic_id FROM user_favorites WHERE user_id = @uid";
            var dt = DBHelper.ExecuteQuery(sql, new MySql.Data.MySqlClient.MySqlParameter[] {
                new MySql.Data.MySqlClient.MySqlParameter("@uid", loggedInUserId)
            });
            foreach (DataRow row in dt.Rows)
            {
                result.Add(Convert.ToInt32(row["comic_id"]));
            }
            return result;
        }


        private async Task RefreshFavoriteRecordsAsync(string searchTerm = "")
        {
            try
            {

                var dt = await Task.Run(() => {
                    string sql = @"SELECT 
                        c.comic_id AS 書號, 
                        c.isbn AS ISBN, 
                        c.title AS 書名, 
                        c.author AS 作者, 
                        c.publisher AS 出版社, 
                        c.category AS 分類,
                        c.image_path AS 圖片URL,
                        CASE WHEN br.comic_id IS NOT NULL THEN '已被借' ELSE '未被借' END AS 借閱狀態,
                        CASE
                            WHEN br.comic_id IS NOT NULL THEN '不可預約'
                            WHEN r.comic_id IS NOT NULL THEN '已被預約'
                            ELSE '可預約'
                        END AS 預約狀態,
                        br.user_id AS borrowed_by,
                        r.user_id AS reserved_by
                    FROM user_favorites uf
                    JOIN comic c ON uf.comic_id = c.comic_id
                    LEFT JOIN borrow_record br ON c.comic_id = br.comic_id AND br.return_date IS NULL
                    LEFT JOIN (
                        SELECT comic_id, user_id 
                        FROM reservation 
                        WHERE status = 'active' 
                        AND reservation_date > DATE_SUB(NOW(), INTERVAL 24 HOUR)
                    ) r ON c.comic_id = r.comic_id
                    WHERE uf.user_id = @uid";
                    var paramList = new List<MySqlParameter> { new MySqlParameter("@uid", loggedInUserId) };
                    if (!string.IsNullOrWhiteSpace(searchTerm))
                    {
                        sql += " AND (c.comic_id LIKE @kw OR c.title LIKE @kw OR c.author LIKE @kw OR c.category LIKE @kw OR c.isbn LIKE @kw OR c.publisher LIKE @kw)";
                        paramList.Add(new MySqlParameter("@kw", "%" + searchTerm + "%"));
                    }
                    sql += " ORDER BY c.comic_id LIMIT @offset, @pageSize";
                    paramList.Add(new MySqlParameter("@offset", (currentFavoritePage - 1) * FavoritePageSize));
                    paramList.Add(new MySqlParameter("@pageSize", FavoritePageSize));
                    return DBHelper.ExecuteQuery(sql, paramList.ToArray());
                });
                cachedFavoriteRecords = dt;
                lastFavoriteRefresh = DateTime.Now;
                await this.InvokeAsync(() => {
                    dgvFavoriteRecord.SuspendLayout();                     dgvFavoriteRecord.DataSource = dt;
                    SetUserComicsGridColumnSettingsForFavorite();
                    UpdateFavoriteButtonColumnStates();                     lblFavoritePage.Text = $"第 {currentFavoritePage} 頁";
                    btnFavoritePrev.Enabled = currentFavoritePage > 1;
                    dgvFavoriteRecord.ResumeLayout();                 });

                await Task.Run(async () => {
                    string countSql = "SELECT COUNT(*) FROM user_favorites uf JOIN comic c ON uf.comic_id = c.comic_id WHERE uf.user_id = @uid";
                    var countParamList = new List<MySqlParameter> { new MySqlParameter("@uid", loggedInUserId) };
                    if (!string.IsNullOrWhiteSpace(searchTerm))
                    {
                        countSql += " AND (c.comic_id LIKE @kw OR c.title LIKE @kw OR c.author LIKE @kw OR c.category LIKE @kw OR c.isbn LIKE @kw OR c.publisher LIKE @kw)";
                        countParamList.Add(new MySqlParameter("@kw", "%" + searchTerm + "%"));
                    }
                    long totalRecords = Convert.ToInt64(DBHelper.ExecuteScalar(countSql, countParamList.ToArray()));
                    await this.InvokeAsync(() => {
                        btnFavoriteNext.Enabled = (currentFavoritePage * FavoritePageSize) < totalRecords;
                    });
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show("載入收藏紀錄時發生錯誤：" + ex.Message, "錯誤", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        private void SetUserComicsGridColumnSettingsForFavorite()
        {
            if (dgvFavoriteRecord == null || dgvFavoriteRecord.Columns.Count == 0) return;
            dgvFavoriteRecord.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill; 

            var columnSettings = new List<(string Name, string Header, int Width)>()
            {
                ("書號", "編號", 50),                 ("ISBN", "ISBN", 90),                 ("書名", "書名", -1),                 ("作者", "作者", 120),                 ("出版社", "出版社", 140),                 ("分類", "分類", 80),                 ("詳情", "詳情", 60),                 ("收藏", "收藏", 60),                 ("借閱狀態", "借閱狀態", 90),                 ("預約狀態", "預約狀態", 90),                 ("回首頁", "在首頁搜尋", 90)             };


            if (dgvFavoriteRecord.Columns["詳情"] == null)
            {
                var btnDetails = new DataGridViewButtonColumn
                {
                    Name = "詳情",
                    HeaderText = "詳情",
                    Text = "詳情",
                    UseColumnTextForButtonValue = false,
                    Width = 58
                };
                dgvFavoriteRecord.Columns.Add(btnDetails);
            }
            if (dgvFavoriteRecord.Columns["收藏"] == null)
            {
                var btnFavorite = new DataGridViewButtonColumn
                {
                    Name = "收藏",
                    HeaderText = "收藏",
                    Text = "收藏",
                    UseColumnTextForButtonValue = false,
                    Width = 58
                };
                dgvFavoriteRecord.Columns.Add(btnFavorite);
            }
            if (dgvFavoriteRecord.Columns["回首頁"] == null)
            {
                var btnJump = new DataGridViewButtonColumn
                {
                    Name = "回首頁",
                    HeaderText = "在首頁搜尋",                     Text = "在首頁搜尋",                     UseColumnTextForButtonValue = true,
                    Width = 90                 };
                dgvFavoriteRecord.Columns.Add(btnJump);
            }

            while (dgvFavoriteRecord.Columns["借書"] != null)
                dgvFavoriteRecord.Columns.Remove("借書");
            while (dgvFavoriteRecord.Columns["預約"] != null)
                dgvFavoriteRecord.Columns.Remove("預約");


            var currentColumns = dgvFavoriteRecord.Columns.OfType<DataGridViewColumn>().ToList();
            int displayIndex = 0;
            foreach (var setting in columnSettings)
            {
                var col = currentColumns.FirstOrDefault(c => c.Name == setting.Name);
                if (col != null)
                {
                    col.HeaderText = setting.Header;

                    if (setting.Width == -1)                     {
                         col.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                    }
                    else
                    {
                        col.AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
                        col.Width = setting.Width;
                    }
                    
                    try
                    {
                        if (!(col is DataGridViewButtonColumn))
                        {
                            col.DataPropertyName = setting.Name;
                        }
                        col.DisplayIndex = displayIndex++;
                        if (setting.Name == "詳情" || setting.Name == "收藏" || setting.Name == "回首頁")
                        {
                            col.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
                        }
                    }
                    catch (ArgumentOutOfRangeException ex)
                    {
                        System.Diagnostics.Debug.WriteLine($"Error setting DisplayIndex for column {setting.Name}: {ex.Message}");
                        col.DisplayIndex = dgvFavoriteRecord.Columns.Count > 0 ? dgvFavoriteRecord.Columns.Count - 1 : 0;
                    }
                }
            }

            foreach (var col in currentColumns)
            {
                if (!columnSettings.Any(setting => setting.Name == col.Name))
                {
                    col.Visible = false;
                }
                else
                {
                    col.Visible = true;
                }
            }
        }


        private void ApplyUserComicsGridGlobalSettings(DataGridView dgv)
        {
            dgv.Font = new System.Drawing.Font("Microsoft JhengHei UI", 10F);
            dgv.ColumnHeadersDefaultCellStyle.Font = new System.Drawing.Font("Microsoft JhengHei UI", 10F, System.Drawing.FontStyle.Bold);
            dgv.RowTemplate.Height = 36;
            dgv.RowHeadersWidth = 60;
            dgv.ColumnHeadersHeight = 24;
            dgv.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.EnableResizing;
            dgv.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgv.EnableHeadersVisualStyles = false;
            dgv.ColumnHeadersDefaultCellStyle.BackColor = System.Drawing.Color.WhiteSmoke;
            dgv.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgv.MultiSelect = false;
            dgv.ReadOnly = true;
            dgv.AllowUserToAddRows = false;
            dgv.AllowUserToDeleteRows = false;
            dgv.AllowUserToOrderColumns = false;
            dgv.StandardTab = true;
            dgv.TabStop = true;
            dgv.TabIndex = 0;
        }


        private void UpdateFavoriteButtonColumnStates()
        {
            if (dgvFavoriteRecord == null || dgvFavoriteRecord.Rows.Count == 0) return;

            foreach (DataGridViewRow row in dgvFavoriteRecord.Rows)
            {
                if (row.IsNewRow) continue;


                var cellDetails = row.Cells["詳情"] as DataGridViewButtonCell;
                if (cellDetails != null)
                {
                    cellDetails.Value = "詳情";
                    cellDetails.ReadOnly = false;
                }


                var cellFavorite = row.Cells["收藏"] as DataGridViewButtonCell;
                if (cellFavorite != null)
                {
                    cellFavorite.Value = "已收藏";
                    cellFavorite.Style.ForeColor = System.Drawing.Color.Red;
                    cellFavorite.ReadOnly = false;
                }
            }
        }


        private async void DgvFavoriteRecord_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex < 0) return; 
            var dgv = sender as DataGridView;
            if (dgv == null) return;

            var columnName = dgv.Columns[e.ColumnIndex].Name;

            if (!(dgv.Rows[e.RowIndex].Cells[e.ColumnIndex] is DataGridViewButtonCell)) return;

            int comicId = 0;
            if (dgv.Columns.Contains("書號") && dgv.Rows[e.RowIndex].Cells["書號"] != null && dgv.Rows[e.RowIndex].Cells["書號"].Value != null)
                int.TryParse(dgv.Rows[e.RowIndex].Cells["書號"].Value.ToString(), out comicId);

            if (comicId == 0) return; 
            try
            {

                if (columnName == "收藏")
                {

                    DBHelper.ExecuteNonQuery("DELETE FROM user_favorites WHERE user_id = @uid AND comic_id = @cid",
                        new MySql.Data.MySqlClient.MySqlParameter[] {
                            new MySql.Data.MySqlClient.MySqlParameter("@uid", loggedInUserId),
                            new MySql.Data.MySqlClient.MySqlParameter("@cid", comicId)
                        });
                    MessageBox.Show("取消收藏成功！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);


                    await RefreshFavoriteRecordsAsync();
                    await RefreshUserComicsGrid("", "全部");                 }

                else if (columnName == "詳情")
                {
                    var comic = GetComicById(comicId);
                    if (comic != null)
                    {

                        var detailsForm = new ComicDetailsForm(comic);


                        detailsForm.ShowDialog(this);


                        tabUserMain.SelectedTab = tabPageHome;


                        var homeTabPage = tabUserMain.TabPages["tabPageHome"];
                        var txtSearch = homeTabPage.Controls.OfType<Panel>().FirstOrDefault()?.Controls.OfType<TextBox>().FirstOrDefault(c => c.Name == "txtSearch");
                        var cmbSearchType = homeTabPage.Controls.OfType<Panel>().FirstOrDefault()?.Controls.OfType<ComboBox>().FirstOrDefault(c => c.Name == "cmbSearchType");

                        if (txtSearch != null && cmbSearchType != null)
                        {

                            txtSearch.Text = comic.書名;
                            cmbSearchType.SelectedItem = "書名";


                            await RefreshUserComicsGrid(comic.書名, "書名");
                        }
                    }
                }
                else if (columnName == "回首頁")                 {
                    string comicTitle = dgv.Rows[e.RowIndex].Cells["書名"].Value?.ToString() ?? "";
                    if (!string.IsNullOrEmpty(comicTitle))
                    {

                        tabUserMain.SelectedTab = tabPageHome;


                        var homeTabPage = tabUserMain.TabPages["tabPageHome"];
                        var txtSearch = homeTabPage.Controls.OfType<Panel>().FirstOrDefault()?.Controls.OfType<TextBox>().FirstOrDefault(c => c.Name == "txtSearch");
                        var cmbSearchType = homeTabPage.Controls.OfType<Panel>().FirstOrDefault()?.Controls.OfType<ComboBox>().FirstOrDefault(c => c.Name == "cmbSearchType");

                        if (txtSearch != null && cmbSearchType != null)
                        {

                            txtSearch.Text = comicTitle;
                            cmbSearchType.SelectedItem = "書名";


                            await RefreshUserComicsGrid(comicTitle, "書名");
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show("操作失敗：" + ex.Message, "錯誤", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        private void SetReserveGridSettingsAndButtonStatus()
        {
            if (dgvReserveRecord == null || dgvReserveRecord.Columns.Count == 0) return;

            dgvReserveRecord.Font = new System.Drawing.Font("Microsoft JhengHei UI", 10F);
            dgvReserveRecord.ColumnHeadersDefaultCellStyle.Font = new System.Drawing.Font("Microsoft JhengHei UI", 10F, System.Drawing.FontStyle.Bold);
            dgvReserveRecord.RowTemplate.Height = 36;
            dgvReserveRecord.RowHeadersWidth = 60;
            dgvReserveRecord.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.EnableResizing;
            dgvReserveRecord.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;             dgvReserveRecord.EnableHeadersVisualStyles = false;
            dgvReserveRecord.ColumnHeadersDefaultCellStyle.BackColor = System.Drawing.Color.WhiteSmoke;

            var columnSettings = new Dictionary<string, (string Header, int? Width, DataGridViewAutoSizeColumnMode Mode)>()
            {
                { "reservation_id", ("編號", 60, DataGridViewAutoSizeColumnMode.None) },                 { "title", ("書名", 200, DataGridViewAutoSizeColumnMode.None) },                 { "isbn", ("ISBN", 100, DataGridViewAutoSizeColumnMode.None) },                 { "reservation_date", ("預約日期", null, DataGridViewAutoSizeColumnMode.Fill) },                 { "expiry_date", ("到期日期", null, DataGridViewAutoSizeColumnMode.Fill) },                 { "status", ("狀態", 80, DataGridViewAutoSizeColumnMode.None) }             };

            foreach (var setting in columnSettings)
            {
                if (dgvReserveRecord.Columns.Contains(setting.Key))
                {
                    var col = dgvReserveRecord.Columns[setting.Key];
                    col.HeaderText = setting.Value.Header;
                    col.AutoSizeMode = setting.Value.Mode;                     if (setting.Value.Mode == DataGridViewAutoSizeColumnMode.None && setting.Value.Width.HasValue)
                    {
                        col.Width = setting.Value.Width.Value;                     }


                    if (setting.Key == "reservation_date" || setting.Key == "expiry_date")
                    {
                        col.DefaultCellStyle.Format = "yyyy/MM/dd HH:mm:ss";
                    }
                }
            }


            var operationCol = dgvReserveRecord.Columns.OfType<DataGridViewButtonColumn>().FirstOrDefault(c => c.Name == "操作");
            if (operationCol == null)
            {
                operationCol = new DataGridViewButtonColumn
                {
                    Name = "操作",
                    HeaderText = "操作",
                    UseColumnTextForButtonValue = false,
                    Width = 80,                 };
                dgvReserveRecord.Columns.Add(operationCol);
            }
            else
            {
                operationCol.UseColumnTextForButtonValue = false;
                operationCol.Width = 80;             }


            foreach (DataGridViewRow row in dgvReserveRecord.Rows)
            {
                if (row.IsNewRow) continue;
                var statusCell = row.Cells["status"];
                var operationCell = row.Cells["操作"] as DataGridViewButtonCell;
                if (statusCell != null && operationCell != null)
                {
                    string status = statusCell.Value?.ToString() ?? "";
                    if (status == "預約中")
                    {

                        int reservationId = 0;
                        if (dgvReserveRecord.Columns.Contains("reservation_id") && row.Cells["reservation_id"] != null && row.Cells["reservation_id"].Value != null) {
                            int.TryParse(row.Cells["reservation_id"].Value.ToString(), out reservationId);
                        }
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
                            operationCell.Value = status;
                            operationCell.ReadOnly = true;
                            if (operationCell.Style != null) operationCell.Style.ForeColor = System.Drawing.Color.Gray;
                        }
                    }
                    else
                    {
                        operationCell.Value = status == "" ? "操作" : status;
                        operationCell.ReadOnly = true;
                        if (operationCell.Style != null) operationCell.Style.ForeColor = System.Drawing.Color.Gray;
                    }
                }
            }
        }
    }
} 

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

