using System;
using System.Data;
using MySql.Data.MySqlClient;
using System.Windows.Forms;

namespace WinFormsApp1
{
    public static class DBHelper
    {
        // Railway MySQL的連線字串，由於考量到資安疑慮，上傳至GitHub時已將部分字元替換為"$"。
        private static readonly string connectionString = "Server=interchange.proxy.rlwy.net;Port=59285;Database=comic_rental;Uid=root;Pwd=$YfcnilBuJ$jGWkyAWai$nnRq$yubI$S;Convert Zero Datetime=True;Allow Zero Datetime=True;Connection Timeout=30;CharSet=utf8mb4;";

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

        public static string GetConnectionString()
        {
            return connectionString;
        }

        public static DateTime ToTaiwanTime(this DateTime utcTime)
        {
            if (utcTime.Kind == DateTimeKind.Unspecified)
            {
                 utcTime = DateTime.SpecifyKind(utcTime, DateTimeKind.Utc);
            }
            else if (utcTime.Kind == DateTimeKind.Local)
            {
                utcTime = utcTime.ToUniversalTime();
            }
            
            try
            {
                TimeZoneInfo taiwanTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Taipei Standard Time");
                return TimeZoneInfo.ConvertTimeFromUtc(utcTime, taiwanTimeZone);
            }
            catch (TimeZoneNotFoundException)
            {
                TimeSpan offset = TimeSpan.FromHours(8);
                return utcTime + offset;
            }
        }

        public static DateTime ToUtcTime(this DateTime localTime)
        {
             if (localTime.Kind == DateTimeKind.Unspecified)
            {
                 localTime = DateTime.SpecifyKind(localTime, DateTimeKind.Local);
            }
            else if (localTime.Kind == DateTimeKind.Utc)
            {
                return localTime;
            }

            try
            {
                
                TimeZoneInfo taiwanTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Taipei Standard Time");
                return TimeZoneInfo.ConvertTimeToUtc(localTime, taiwanTimeZone);
            }
            catch (TimeZoneNotFoundException)
            {
                TimeSpan offset = TimeSpan.FromHours(-8);
                return localTime + offset;
            }
        }
    }
} 