namespace Attendance
{
    internal static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            /*// To customize application configuration such as set high DPI settings or default font,
            // see https://aka.ms/applicationconfiguration.
            ApplicationConfiguration.Initialize();
            Application.Run(new Form1());*/

            ApplicationConfiguration.Initialize();

            // 1. Tạo và hiển thị FormLogin dưới dạng hộp thoại độc lập (ShowDialog)
            Login loginForm = new Login();
            DialogResult result = loginForm.ShowDialog();

            // 2. Kiểm tra nếu kết quả trả về từ FormLogin là OK (Đăng nhập đúng)
            if (result == DialogResult.OK)
            {
                // Lúc này FormLogin đã TỰ ĐỘNG ĐÓNG VÀ GIẢI PHÓNG khỏi RAM hoàn toàn
                // Ứng dụng bắt đầu chuyển quyền sinh tồn chính sang cho FormMain
                //Application.Run(new Form1(loginForm.text));
            }
            else
            {
                // Nếu người dùng bấm hủy hoặc tắt FormLogin, ứng dụng sẽ thoát luôn tại đây
                Application.Exit();
            }
        }
    }
}