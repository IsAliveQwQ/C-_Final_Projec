using System;
using System.Data;
using MySql.Data.MySqlClient;
using System.Windows.Forms;

namespace WinFormsApp1
{
    public static class DBHelper
    {
        // 資料庫連接字串
        private static readonly string connectionString = "Server=interchange.proxy.rlwy.net;Port=59285;Database=comic_rental;Uid=root;Pwd=HYfcnilBuJjjGWkyAWaiSnnRqMyubIuS;Convert Zero Datetime=True;Allow Zero Datetime=True;Connection Timeout=30;CharSet=utf8mb4;";

        // 執行 SELECT 查詢並返回 DataTable
        public static DataTable ExecuteQuery(string sql, MySqlParameter[] parameters = null)
        {
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                using (MySqlCommand cmd = new MySqlCommand(sql, conn))
                {
                    if (parameters != null)
                    {
                        cmd.Parameters.AddRange(parameters);
                    }
                    DataTable dt = new DataTable();
                    using (MySqlDataAdapter adapter = new MySqlDataAdapter(cmd))
                    {
                        adapter.Fill(dt);
                    }
                    return dt;
                }
            }
        }

        // 執行非查詢操作 (INSERT, UPDATE, DELETE) 並返回受影響的行數
        public static int ExecuteNonQuery(string sql, MySqlParameter[] parameters = null)
        {
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                using (MySqlCommand cmd = new MySqlCommand(sql, conn))
                {
                    if (parameters != null)
                    {
                        cmd.Parameters.AddRange(parameters);
                    }
                    return cmd.ExecuteNonQuery();
                }
            }
        }

        // 執行單值查詢並返回結果
        public static object ExecuteScalar(string sql, MySqlParameter[] parameters = null)
        {
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                using (MySqlCommand cmd = new MySqlCommand(sql, conn))
                {
                    if (parameters != null)
                    {
                        cmd.Parameters.AddRange(parameters);
                    }
                    return cmd.ExecuteScalar();
                }
            }
        }

        // 取得目前資料庫連線字串（for debug）
        public static string GetConnectionString()
        {
            return connectionString;
        }

        // 新增一個擴展方法來處理時區轉換
        public static DateTime ToTaiwanTime(this DateTime utcTime)
        {
            // 確認 utcTime 是 UTC 時間
            if (utcTime.Kind == DateTimeKind.Unspecified)
            {
                 // 如果是未指定，假設它是 UTC
                 utcTime = DateTime.SpecifyKind(utcTime, DateTimeKind.Utc);
            }
            else if (utcTime.Kind == DateTimeKind.Local)
            {
                // 如果是本地時間，先轉換為 UTC
                utcTime = utcTime.ToUniversalTime();
            }
            
            try
            {
                // 嘗試尋找台灣時區
                TimeZoneInfo taiwanTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Taipei Standard Time");
                return TimeZoneInfo.ConvertTimeFromUtc(utcTime, taiwanTimeZone);
            }
            catch (TimeZoneNotFoundException)
            {
                // 如果找不到時區，退回使用 UTC+8 的偏移量
                TimeSpan offset = TimeSpan.FromHours(8);
                return utcTime + offset;
            }
        }

        public static DateTime ToUtcTime(this DateTime localTime)
        {
            // 確認 localTime 是本地時間
             if (localTime.Kind == DateTimeKind.Unspecified)
            {
                 // 如果是未指定，假設它是本地時間
                 localTime = DateTime.SpecifyKind(localTime, DateTimeKind.Local);
            }
            else if (localTime.Kind == DateTimeKind.Utc)
            {
                // 如果是 UTC 時間，直接返回 (因為目標是 UTC)
                return localTime;
            }

            try
            {
                // 嘗試尋找台灣時區
                TimeZoneInfo taiwanTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Taipei Standard Time");
                return TimeZoneInfo.ConvertTimeToUtc(localTime, taiwanTimeZone);
            }
            catch (TimeZoneNotFoundException)
            {
                 // 如果找不到時區，退回使用 UTC-8 的偏移量
                TimeSpan offset = TimeSpan.FromHours(-8);
                return localTime + offset;
            }
        }
    }
} 