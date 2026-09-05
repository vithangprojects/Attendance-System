using System;
using System.Data;
using System.Data.SQLite;
using System.Runtime.CompilerServices;
using System.Windows.Forms; // Dùng để hiển thị thông báo lỗi nếu cần

namespace Attendance
{

    // 1. Đổi từ 'internal' thành 'public' để các project layer khác (nếu chia nhỏ) có thể thấy.
    // 2. Thêm từ khóa 'abstract' để lớp này chỉ làm nhiệm vụ cho kế thừa, không cho phép tạo mới trực tiếp (new DatabaseConnection).
    public abstract class DatabaseConnection
    {
        protected static readonly string ConnectionString = "Data Source=MyData.db;Version=3;";

        /// <summary>
        /// Hàm dùng chung để khởi tạo và kiểm tra kết nối an toàn trước khi sử dụng dữ liệu
        /// </summary>
        /// <returns>Trả về một đối tượng SQLiteConnection đã được mở sẵn, hoặc null nếu lỗi</returns>
        protected SQLiteConnection? GetOpenedConnection()
        {
            SQLiteConnection conn = new SQLiteConnection(ConnectionString);

            try
            {
                // Kiểm tra nếu kết nối chưa mở thì tiến hành mở
                if (conn.State != ConnectionState.Open)
                {
                    conn.Open();
                }
                return conn;
            }
            catch (SQLiteException ex)
            {
                // Báo lỗi trực tiếp cho lập trình viên/người dùng biết nếu file db bị lỗi hoặc không tìm thấy
                MessageBox.Show($"Lỗi kết nối cơ sở dữ liệu: {ex.Message}", "Lỗi Hệ Thống", MessageBoxButtons.OK, MessageBoxIcon.Error);

                // Giải phóng tài nguyên nếu mở lỗi
                if (conn != null)
                {
                    conn.Dispose();
                }
                return null;
            }
        }
    }
}
