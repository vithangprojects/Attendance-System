using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Forms;

namespace Attendance
{
    public partial class Login : Form
    {
        public string UserID = "";

        public Login()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            CheckPassword();
        }

        private void Login_Load(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.None;
        }

        private void textBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
                CheckPassword();
        }

        private void CheckPassword()
        {
            // 1. Đóng gói dữ liệu nhập từ TextBox vào DTO
            UserDTO user = new UserDTO(textBox2.Text.Trim(), textBox1.Text.Trim());

            // 2. Gửi dữ liệu qua lớp BLL xử lý
            UserBLL bll = new UserBLL();
            string ketQuaBLL = bll.CheckPassword(user);

            // 3. Nhận kết quả từ BLL và hiển thị lên giao diện
            if (ketQuaBLL == "SUCCESS")
            {
                MessageBox.Show("Đăng nhập thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.DialogResult = DialogResult.OK;

                if (textBox2.Text == "1")
                {
                    UserTimeBLL bllUserTime = new UserTimeBLL();
                    bllUserTime.SetFirstFingerPrintOfDay("1");
                }

                this.Hide();
                Form1 mainForm = new Form1(textBox2.Text.Trim());
                mainForm.ShowDialog();
            }
            else
            {
                // Hiển thị các câu cảnh báo lỗi từ BLL (Trống ô, hoặc sai tài khoản)
                MessageBox.Show(ketQuaBLL, "Thông báo lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                textBox1.Focus();
                textBox1.SelectAll();
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            ChangePSW form = new ChangePSW();
            form.ShowDialog();
        }
    }
}
