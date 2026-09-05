using Microsoft.VisualBasic.ApplicationServices;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Text;

namespace Attendance
{
    public class UserDAL : DatabaseConnection
    {
        public string AlreadyCheckIn(string userID)
        {
            using (SQLiteConnection? conn = GetOpenedConnection())
            {
                if (conn == null) return "";

                // Câu lệnh SQL lấy Id và Họ tên của tất cả thành viên
                string sql = "Select Time from UserTimeInOut  Where UserID = @id AND Type = 'In' AND Date = date('now', 'localtime')";

                using (SQLiteCommand cmd = new SQLiteCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@id", userID);

                    // ExecuteScalar dùng để lấy ra đúng 1 giá trị duy nhất (cột FullName)
                    object result = cmd.ExecuteScalar();

                    if (result != null && result != DBNull.Value)
                    {
                        return result.ToString() ?? string.Empty;
                    }
                }
            }

            return "";
        }

        public List<UserDTO> GetUsersPosition()
        {
            List<UserDTO> listPositions = new List<UserDTO>();

            using (SQLiteConnection? conn = GetOpenedConnection())
            {
                if (conn == null) return listPositions;

                // Câu lệnh SQL lấy Id và Họ tên của tất cả thành viên
                string sql = "SELECT Id, Position, FullName FROM Users Where Position != ''  ORDER BY Id ASC";

                using (SQLiteCommand cmd = new SQLiteCommand(sql, conn))
                {
                    using (SQLiteDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            UserDTO user = new UserDTO
                            {
                                ID = reader["Id"]?.ToString() ?? "",
                                // Phòng trường hợp dòng nào đó trong DB bị NULL cột FullName
                                Position = reader["Position"]?.ToString() ?? "",
                                FullName = reader["FullName"]?.ToString() ?? "Tên chưa cập nhật"
                            };
                            listPositions.Add(user);
                        }
                    }
                }
            }
            return listPositions;
        }

        /// <summary>
        /// 2. Cập nhật vị trí cột và hàng mới cho User theo số ID
        /// </summary>
        public bool SetPosition(string userID, int col, int row)
        {
            using (SQLiteConnection? conn = GetOpenedConnection())
            {
                if (conn == null) return false;

                string sql = "UPDATE Users SET Position = Concat(@col, ',', @row) WHERE Id = @id";
                using (SQLiteCommand cmd = new SQLiteCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@col", col);
                    cmd.Parameters.AddWithValue("@row", row);
                    cmd.Parameters.AddWithValue("@id", userID);

                    return cmd.ExecuteNonQuery() > 0; // Trả về true nếu cập nhật thành công
                }
            }
        }

        public bool IDIsExists(int id)
        {
            using (SQLiteConnection? conn = GetOpenedConnection())
            {
                if (conn == null) return false;

                string sql = "SELECT COUNT(*) FROM Users WHERE Id = @id";
                using (SQLiteCommand cmd = new SQLiteCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@id", id);
                    int count = Convert.ToInt32(cmd.ExecuteScalar());
                    return count > 0; // Trả về true nếu tìm thấy bản ghi khớp ID
                }
            }
        }

        /// <summary>
        /// Tìm kiếm họ tên của User dựa vào số ID
        /// </summary>
        public string GetNameByID(string id)
        {
            using (SQLiteConnection? conn = GetOpenedConnection())
            {
                if (conn == null) return string.Empty;

                string sql = "SELECT FullName FROM Users WHERE Id = @id";

                using (SQLiteCommand cmd = new SQLiteCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@id", id);

                    // ExecuteScalar dùng để lấy ra đúng 1 giá trị duy nhất (cột FullName)
                    object result = cmd.ExecuteScalar();

                    if (result != null && result != DBNull.Value)
                    {
                        return result.ToString() ?? string.Empty;
                    }
                }
            }
            return string.Empty; // Trả về chuỗi rỗng nếu không tìm thấy ID
        }
        public bool PasswordIsEmpty(string userID)
        {
            // Sử dụng hàm GetOpenedConnection() đã nâng cấp từ lớp cha
            using (SQLiteConnection? conn = GetOpenedConnection())
            {
                if (conn == null) return false;

                // Câu lệnh SQL kiểm tra sự tồn tại của cặp Username và Password
                string sql = "SELECT COUNT(*) FROM Users WHERE ID = @user AND Password = ''";

                using (SQLiteCommand cmd = new SQLiteCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@user", userID);

                    // ExecuteScalar trả về giá trị ở ô đầu tiên thu được từ câu lệnh SELECT
                    int count = Convert.ToInt32(cmd.ExecuteScalar());

                    return count > 0; // Nếu lớn hơn 0 nghĩa là tài khoản đúng
                }
            }
        }

        public bool CheckPassword(UserDTO user)
        {
            // Sử dụng hàm GetOpenedConnection() đã nâng cấp từ lớp cha
            using (SQLiteConnection? conn = GetOpenedConnection())
            {
                if (conn == null) return false;

                // Câu lệnh SQL kiểm tra sự tồn tại của cặp Username và Password
                string sql = "SELECT COUNT(*) FROM Users WHERE ID = @user AND Password = @pass";

                using (SQLiteCommand cmd = new SQLiteCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@user", user.ID);
                    cmd.Parameters.AddWithValue("@pass", user.Password);

                    // ExecuteScalar trả về giá trị ở ô đầu tiên thu được từ câu lệnh SELECT
                    int count = Convert.ToInt32(cmd.ExecuteScalar());

                    return count > 0; // Nếu lớn hơn 0 nghĩa là tài khoản đúng
                }
            }
        }


        public bool ChangePassword(int id, string hashedNewPassword)
        {
            using (SQLiteConnection? conn = GetOpenedConnection())
            {
                if (conn == null) return false;

                string sql = "UPDATE Users SET Password = @pass WHERE Id = @id";
                using (SQLiteCommand cmd = new SQLiteCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@pass", hashedNewPassword);
                    cmd.Parameters.AddWithValue("@id", id);

                    try
                    {
                        int rowsAffected = cmd.ExecuteNonQuery();

                        // Trường hợp lệnh chạy thành công nhưng không có dòng nào được cập nhật
                        if (rowsAffected == 0)
                        {
                            MessageBox.Show(
                                $"Lệnh chạy thành công nhưng KHÔNG tìm thấy User nào có Id = {id} trong hệ thống!",
                                "Cảnh Báo Dữ Liệu",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Warning
                            );
                        }

                        return rowsAffected > 0;
                    }
                    catch (SQLiteException ex)
                    {
                        // BẮT MÃ LỖI TẠI ĐÂY: Hiển thị chi tiết mã lỗi (ErrorCode) và thông điệp từ SQLite
                        MessageBox.Show(
                            $"SQLite gặp lỗi khi thực thi lệnh!\n\n" +
                            $"Mã lỗi (ErrorCode): {ex.ErrorCode}\n" +
                            $"Chi tiết: {ex.Message}",
                            "Lỗi Thực Thi SQL",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Error
                        );

                        return false;
                    }
                }
            }
        }

    }
}
