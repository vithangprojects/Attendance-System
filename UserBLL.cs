using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace Attendance
{
    internal class UserBLL
    {
        private UserDAL _dal = new UserDAL();

        public string AlreadyCheckIn(string userID)
        {
            return _dal.AlreadyCheckIn(userID);
        }

        public List<UserDTO> GetUsersPosition()
        {
            // Chuyển tiếp danh sách lấy từ DAL lên cho lớp GUI
            return _dal.GetUsersPosition();
        }

        /// <summary>
        /// Xử lý nghiệp vụ đặt vị trí chỗ ngồi cho User
        /// </summary>
        public string SetPosition(string userID, int col, int row)
        {
            // 3. Nếu mọi điều kiện hợp lệ, gọi DAL cập nhật tọa độ vào SQLite
            bool result = _dal.SetPosition(userID, col, row);
            if (result)
            {
                return "SUCCESS";
            }

            return "Cập nhật vị trí thất bại!";
        }

        /// <summary>
        /// Hàm xử lý nghiệp vụ: Kiểm tra và lấy họ tên người dùng từ chuỗi ID
        /// </summary>
        public string GetNameByID(string idStr)
        {
            // Trường hợp 1: ID bị bỏ trống hoặc chỉ toàn dấu cách
            if (string.IsNullOrWhiteSpace(idStr))
            {
                return "Trống"; // Trả về trạng thái để GUI xóa ô họ tên
            }

            // Trường hợp 2: ID không hợp lệ (Người dùng cố tình gõ chữ bậy bạ, số âm, số quá lớn)
            if (!int.TryParse(idStr, out int id) || id <= 0)
            {
                return "ID không hợp lệ!";
            }

            // --- Đủ điều kiện an toàn, tiến hành gọi xuống DAL quét SQLite ---

            // Bước 1: Kiểm tra xem ID này có tồn tại trong hệ thống chưa
            bool isIdExist = _dal.IDIsExists(id);
            if (!isIdExist)
            {
                // Trường hợp 3: Số ID hợp lệ nhưng chưa được đăng ký trong Database
                return "ID chưa tồn tại!";
            }

            // Bước 2: Nếu có ID, tiến hành lấy tên
            string fullName = _dal.GetNameByID(idStr);

            // Trường hợp 4: Có ID trong hệ thống nhưng cột FullName bị bỏ trống hoặc NULL
            if (string.IsNullOrWhiteSpace(fullName))
            {
                return "Chưa cập nhật họ tên!";
            }

            // Tất cả các bước đều vượt qua thành công -> Trả về họ tên chuẩn
            return fullName;
        }

        public string CheckPassword(UserDTO user)
        {
            // 1. Kiểm tra logic nghiệp vụ (Ràng buộc dữ liệu đầu vào)
            if (string.IsNullOrWhiteSpace(user.ID))
            {
                return "Tên đăng nhập không được để trống!";
            }

            if (string.IsNullOrWhiteSpace(user.Password))
            {
                return "Mật khẩu không được để trống!";
            }

            user.Password = ToSHA256(user.Password);
            // 2. Gọi xuống lớp DAL để kiểm tra dưới Database SQLite
            bool ketQua = _dal.CheckPassword(user);

            if (ketQua)
            {
                return "SUCCESS"; // Đăng nhập đúng, trả về từ khóa thành công
            }
            else
            {
                return "Tên đăng nhập hoặc mật khẩu không chính xác!";
            }
        }

        /// <summary>
        /// Logic 2: Thực hiện đổi mật khẩu mới
        /// </summary>
        public string ChangePassword(string idStr, string newPass, string confirmPass)
        {
            if (!int.TryParse(idStr, out int id)) return "ID không hợp lệ!";

            // Yêu cầu 3: Kiểm tra mật khẩu mới và xác nhận có trùng không
            if (string.IsNullOrWhiteSpace(newPass)) return "Mật khẩu mới không được để trống!";
            if (newPass != confirmPass) return "Mật khẩu mới và Xác nhận mật khẩu không trùng khớp!";

            // Mã hóa mật khẩu mới trước khi lưu
            string hashedNewPass = ToSHA256(newPass);

            bool ketQua = _dal.ChangePassword(id, hashedNewPass);
            if (ketQua) return "SUCCESS";

            return "Đổi mật khẩu thất bại!";
        }

        /// <summary>
        /// Hàm băm chuỗi mật khẩu thường thành chuỗi mã hóa SHA-256
        /// </summary>
        private string ToSHA256(string password)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                // Chuyển chuỗi mật khẩu thành mảng byte
                byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));

                // Chuyển mảng byte thu được thành chuỗi Hexadecimal (chuỗi chữ và số)
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2"));
                }
                return builder.ToString();
            }
        }
    }
}
