using System;
using System.Collections.Generic;
using System.Text;

namespace Attendance
{
    public class UserTimeBLL
    {
        private readonly UserTimeDAL _dal = new UserTimeDAL();

        /// <summary>
        /// Xử lý nghiệp vụ ghi nhận vân tay đầu tiên trong ngày cho một User
        /// </summary>
        public string SetFirstFingerPrintOfDay(string userIdStr)
        {
            // 1. Kiểm tra ID đầu vào
            if (string.IsNullOrWhiteSpace(userIdStr)) return "Vui lòng nhập ID người dùng!";
            if (!int.TryParse(userIdStr, out int id) || id <= 0) return "ID không hợp lệ!";

            // 2. Lấy thời gian hiện tại của hệ thống máy tính
            DateTime now = DateTime.Now;
            string todayStr = now.ToString("yyyy-MM-dd"); // Dùng để check trùng ngày
            string timeStr = now.ToString("HH:mm:ss"); // Dùng để lưu vào DB

            // 3. Chốt chặn nghiệp vụ: Kiểm tra xem hôm nay người này đã quét chưa
            if (_dal.IsFirstFingerPrintOfDay(id, todayStr))
            {
                return $"ID {id} đã ghi nhận quét vân tay đầu ngày vào hôm nay rồi!";
            }

            // 4. Nếu chưa quét, tiến hành gọi DAL để lưu vào SQLite
            if (_dal.SetFirstFingerPrintOfDay(id, todayStr, timeStr))
            {
                return $"SUCCESS|{now.ToString("HH:mm:ss")}"; // Trả về từ khóa thành công kèm theo giờ quét
            }

            return "Gặp lỗi hệ thống, không thể lưu dữ liệu!";
        }

        public string CheckInOut(string userID, string type)
        {
            // 1. Kiểm tra ID đầu vào
            if (string.IsNullOrWhiteSpace(userID)) return "Vui lòng nhập ID người dùng!";
            if (!int.TryParse(userID, out int id) || id <= 0) return "ID không hợp lệ!";

            // 2. Lấy thời gian hiện tại của hệ thống máy tính
            DateTime now = DateTime.Now;
            string date = now.ToString("yyyy-MM-dd"); // Dùng để check trùng ngày
            string time = now.ToString("HH:mm:ss"); // Dùng để lưu vào DB

            if ((type == "In" && _dal.IsFirstFingerPrintOfDay(id, date)) || type == "Out")
            {
                if (_dal.CheckInOut(id, date, time, type))
                    return ("SUCCESS|" + time);
            }else
            {
                return "Cảnh Báo|Chưa quét vân tay ở máy chấm công";
            }

            return "Lỗi|Gặp lỗi hệ thống, không thể lưu dữ liệu!";
        }
    }
}
