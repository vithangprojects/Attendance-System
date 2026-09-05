using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows.Forms;

namespace Attendance
{
    public partial class ChangePSW : Form
    {
        public ChangePSW()
        {
            InitializeComponent();
        }

        private void textBox1_Leave(object sender, EventArgs e)
        {
            // Yêu cầu 1: Khi ID mất focus, kiểm tra rỗng thì thông báo
            if (string.IsNullOrWhiteSpace(textBox1.Text))
            {
                MessageBox.Show("Vui lòng điền thông tin vào ô ID!", "Nhắc nhở", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                textBox1.Focus(); // Đưa con trỏ chuột quay lại ô ID bắt nhập
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            // Yêu cầu 3 & Thực hiện đổi: Kiểm tra trùng khớp mật khẩu mới và Lưu vào DB
            UserBLL bll = new UserBLL();
            string thongBao = bll.ChangePassword(textBox1.Text, textBox3.Text, textBox4.Text);

            if (thongBao == "SUCCESS")
            {
                MessageBox.Show("Thay đổi mật khẩu thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.Close(); // Đóng form đổi mật khẩu lại
            }
            else
            {
                MessageBox.Show(thongBao, "Lỗi dữ liệu", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void button3_MouseDown(object sender, MouseEventArgs e)
        {
            Button btn = (Button)sender;
            if (btn.Name == "button3") textBox2.UseSystemPasswordChar = false;
            else if (btn.Name == "button4") textBox3.UseSystemPasswordChar = false;
            else if (btn.Name == "button5") textBox4.UseSystemPasswordChar = false;
        }

        private void button3_MouseUp(object sender, MouseEventArgs e)
        {
            Button btn = (Button)sender;
            if (btn.Name == "button3") textBox2.UseSystemPasswordChar = true;
            else if (btn.Name == "button4") textBox3.UseSystemPasswordChar = true;
            else if (btn.Name == "button5") textBox4.UseSystemPasswordChar = true;
        }

        private void textBox3_Enter(object sender, EventArgs e)
        {
            // 1. Kiểm tra xem người dùng có bấm nút X (Close) hoặc đóng Form khôngp

            if (this.ActiveControl == null || this.ActiveControl.Name == "button2" || this.ActiveControl.Name == "button3")
            {
                return; // Thoát hàm luôn, không hiện thông báo lỗi
            }

            // Yêu cầu 2: Khi "mật khẩu mới" focus thì kiểm tra id đúng password cũ không
            UserBLL bll = new UserBLL();
            UserDTO user = new UserDTO(textBox1.Text.Trim(), textBox2.Text.Trim());
            string checkResult = bll.CheckPassword(user);

            if (checkResult != "SUCCESS")
            {
                MessageBox.Show(checkResult, "Lỗi xác thực", MessageBoxButtons.OK, MessageBoxIcon.Error);

                // Nếu sai, đẩy con trỏ chuột ngược lại ô Mật khẩu cũ không cho nhập mật khẩu mới
                textBox2.Focus();
                textBox2.SelectAll();
            }
            else
            {
                textBox3.ReadOnly = false;
            }

        }

        private void textBox4_Enter(object sender, EventArgs e)
        {
            if (textBox3.Text.Trim() == "")
            {
                MessageBox.Show("Mật khẩu không được để trống!", "Nhắc nhở", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                textBox3.Focus();
            }
        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {
            textBox4.ReadOnly = false;
        }

        private void textBox4_TextChanged(object sender, EventArgs e)
        {
            button1.Enabled = true;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
