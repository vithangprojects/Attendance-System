using Microsoft.VisualBasic.ApplicationServices;
using System;
using System.Data.SQLite;
using System.Timers;

namespace Attendance
{
    public class UserTimeDAL : DatabaseConnection
    {
        /// <summary>
        /// 1. Kiểm tra xem User đã có dữ liệu quét vân tay trong ngày hôm nay chưa
        /// </summary>
        public bool IsFirstFingerPrintOfDay(int Userid, string todayStr)
        {
            using (SQLiteConnection? conn = GetOpenedConnection())
            {
                if (conn == null) return false;

                // Sử dụng hàm date() của SQLite để lọc phần Ngày từ cột CheckTime dạng TEXT
                string sql = "SELECT COUNT(*) FROM FirstFingerPrint WHERE UserID = @id AND Date = @today";

                using (SQLiteCommand cmd = new SQLiteCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@id", Userid);
                    cmd.Parameters.AddWithValue("@today", todayStr); // Truyền chuỗi định dạng "YYYY-MM-DD"

                    int count = Convert.ToInt32(cmd.ExecuteScalar());
                    return count > 0; // Trả về true nếu đã tồn tại bản ghi trong ngày hôm nay
                }
            }
        }

        /// <summary>
        /// 2. Chèn dữ liệu chấm công mới với trọn gói Ngày và Giờ
        /// </summary>
        public bool SetFirstFingerPrintOfDay(int Userid, string dateStr, string timeStr)
        {
            using (SQLiteConnection? conn = GetOpenedConnection())
            {
                if (conn == null) return false;

                string sql = "INSERT INTO FirstFingerPrint (UserID, Date, Time) VALUES (@id, @Date, @Time)";

                using (SQLiteCommand cmd = new SQLiteCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@id", Userid);
                    cmd.Parameters.AddWithValue("@Date", dateStr); // Truyền chuỗi "YYYY-MM-DD"
                    cmd.Parameters.AddWithValue("@Time", timeStr); // Truyền chuỗi "HH:mm:ss"

                    int result = cmd.ExecuteNonQuery();
                    return result > 0;
                }
            }
        }

        public bool CheckInOut(int userID, string date, string time, string type)
        {
            using (SQLiteConnection? conn = GetOpenedConnection())
            {
                if (conn == null) return false;

                string sql = "INSERT INTO UserTimeInOut (UserID, Date, Time, Type) VALUES (@id, @Date, @Time, @Type)";

                using (SQLiteCommand cmd = new SQLiteCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@id", userID);
                    cmd.Parameters.AddWithValue("@Date", date); // Truyền chuỗi "YYYY-MM-DD"
                    cmd.Parameters.AddWithValue("@Time", time); // Truyền chuỗi "HH:mm:ss"
                    cmd.Parameters.AddWithValue("@Type", type); // Truyền chuỗi "In/Out"

                    int result = cmd.ExecuteNonQuery();
                    return result > 0;
                }
            }
        }
    }
}
