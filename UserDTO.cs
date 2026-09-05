using System;
using System.Collections.Generic;
using System.Text;

namespace Attendance
{
    public class UserDTO
    {
        // Gán sẵn chuỗi rỗng để không bị báo lỗi constructor
        public string ID { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Position { get; set; } = string.Empty;

        /// <summary>
        /// 1. Constructor mặc định (Không tham số)
        /// Bắt buộc phải giữ lại để không làm lỗi các đoạn code cũ sử dụng dạng: new UserDTO()
        /// </summary>
        public UserDTO()
        {
            // Để trống hoặc gán giá trị mặc định nếu muốn
        }

        /// <summary>
        /// 2. Constructor nâng cấp (Có tham số)
        /// Giúp bạn khởi tạo nhanh đối tượng và gán giá trị ngay lập tức
        /// </summary>
        /// <param name="username">Tên đăng nhập hoặc chuỗi ID định danh</param>
        /// <param name="password">Mật khẩu của tài khoản</param>
        public UserDTO(string userID, string password)
        {
            this.ID = userID;
            this.Password = password;
        }
    }
}
