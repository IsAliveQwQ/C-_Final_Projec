using System;
using System.Data;
using MySql.Data.MySqlClient;
using System.Windows.Forms;

namespace WinFormsApp1
{
    public static class DBHelper
    {
        // 資料庫連接字串
        private static readonly string connectionString = "Server=interchange.proxy.rlwy.net;Port=59285;Database=comic_rental;Uid=root;Pwd=HYfcnilBuJjjGWkyAWaiSnnRqMyubIuS;";

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
    }
} 