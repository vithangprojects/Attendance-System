using System.ComponentModel;
using System.Web;

namespace Attendance
{
    public partial class Form1 : Form
    {
        // Khai báo một thuộc tính công khai cho phép các Form khác gán giá trị vào
        private string _loginID = string.Empty;
        private readonly UserBLL _userBLL = new UserBLL();
        private readonly DataCellBLL _dataCellBLL = new DataCellBLL();
        private readonly UserTimeBLL _userTimeBLL = new UserTimeBLL();

        public Form1(string loginID)
        {
            InitializeComponent();
            _loginID = loginID;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            // Tự động kéo dãn độ rộng các cột cho khít toàn bộ chiều ngang của bảng
            //dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            // Không cho phép người dùng tự bấm vào dòng trống dưới cùng để thêm dòng bừa bãi
            dataGridView1.AllowUserToAddRows = false;
            for (int i = 0; i < 9; i++)
            {
                dataGridView1.Rows.Add();
                dataGridView1.Rows[dataGridView1.RowCount - 1].Height = 40;
            }

            for (int i = 0; i < dataGridView1.ColumnCount; i++)
                if (i < dataGridView1.ColumnCount)
                    dataGridView1.Columns[i].Width = 45;


            AutoResizeGridControl(dataGridView1);
            this.Width = dataGridView1.Width + 40;

            //button1.Location = new System.Drawing.Point(dataGridView1.Location.X + dataGridView1.Width - button1.Width, button1.Location.Y);

            SetColorCells();

            this.Text = "Attendance - " + DateTime.Now.ToString("yyyy-MM-dd") + ", " + _loginID + "." + _userBLL.GetNameByID(_loginID);

            foreach (UserDTO user in _userBLL.GetUsersPosition())
            {
                string[] fullName = user.FullName.Split(' ');
                string[] pos = user.Position.Split(',');
                int col = Convert.ToInt16(pos[0]);
                int row = Convert.ToInt16(pos[1]);
                dataGridView1[col, row].Value = fullName[fullName.Length - 1];
                dataGridView1[col, row].Style.BackColor = Color.LightGray;

                string checkIn = _userBLL.AlreadyCheckIn(user.ID);
                if (checkIn != "")
                {
                    dataGridView1[col, row].Style.BackColor = Color.LightBlue;
                    if (_loginID == user.ID) button1.Text = checkIn;
                }
            }
        }

        private void SetColorCells()
        {
            List<DataCellDTO> dsOMau = _dataCellBLL.GetListColorCells();

            // 3. Duyệt qua từng ô trong danh sách để tiến hành tô màu lên lưới
            foreach (var cell in dsOMau)
            {
                int c = cell.Col; // Lấy chỉ số cột
                int r = cell.Row; // Lấy chỉ số hàng

                // Kiểm tra điều kiện an toàn phòng trường hợp tọa độ trong DB vượt quá kích thước bảng
                if (r < dataGridView1.Rows.Count && c < dataGridView1.Columns.Count)
                {
                    // Chuyển mã chuỗi Hex (Ví dụ: '#000000') từ DB thành đối tượng Color của C#
                    Color colorFromDB = ColorTranslator.FromHtml(cell.Color);

                    // Tô màu trực tiếp cho ô dựa theo tọa độ (Cột c, Hàng r)
                    dataGridView1.Rows[r].Cells[c].Style.BackColor = colorFromDB;
                }
            }
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            // Đóng ứng dụng WinForms
            Application.Exit();

            // Ép hệ thống (Garbage Collector) dọn dẹp bộ nhớ RAM ngay lập tức
            GC.Collect();
            GC.WaitForPendingFinalizers();
        }
        private void AutoResizeGridControl(DataGridView dgv)
        {
            // 1. Tính toán chiều rộng tổng thể khít với các cột
            int totalWidth = 0;
            foreach (DataGridViewColumn col in dgv.Columns)
            {
                if (col.Visible)
                {
                    totalWidth += col.Width;
                }
            }

            // Cộng thêm phần viền của bảng và thanh cuộn dọc (nếu có)
            if (dgv.RowHeadersVisible) totalWidth += dgv.RowHeadersWidth;
            totalWidth += (dgv.BorderStyle == BorderStyle.None) ? 2 : 4;

            // Đặt lại chiều rộng cho DataGridView
            dgv.Width = totalWidth;

            // 2. Tính toán chiều cao tổng thể khít với các dòng
            int totalHeight = dgv.ColumnHeadersHeight; // Chiều cao thanh tiêu đề cột
            foreach (DataGridViewRow row in dgv.Rows)
            {
                if (row.Visible)
                {
                    totalHeight += row.Height;
                }
            }

            // Cộng thêm viền bảng
            totalHeight += (dgv.BorderStyle == BorderStyle.None) ? 2 : 4;

            // Đặt lại chiều cao cho DataGridView
            dgv.Height = totalHeight;
        }

        private void textBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                // Gọi hàm BLL đã nâng cấp chặt chẽ
                string result = _userBLL.GetNameByID(textBox1.Text);

                button2.Enabled = false;
                // Xử lý hiển thị dựa trên kết quả trả về từ BLL
                if (result == "Trống")
                {
                    textBox2.Text = string.Empty;
                }
                else if (result == "ID không hợp lệ!" || result == "ID chưa tồn tại!")
                {
                    textBox2.Text = result;
                    textBox2.ForeColor = Color.Red; // Đổi chữ thành màu ĐỎ để cảnh báo người dùng
                }
                else if (result == "Chưa cập nhật họ tên!")
                {
                    textBox2.Text = result;
                    textBox2.ForeColor = Color.Orange; // Màu CAM vì có ID nhưng thiếu tên
                }
                else
                {
                    // Đăng nhập đúng người -> Hiện họ tên bình thường
                    textBox2.Text = result;
                    textBox2.ForeColor = Color.Black; // Chữ đen mặc định
                    if (_loginID == "1") button2.Enabled = true;
                }
            }
        }


        private void button1_Click(object sender, EventArgs e)
        {
            if (button1.Text != "Check in")
            {
                MessageBox.Show("Bạn đã check in rồi !", "Thông tin", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            string[] result = _userTimeBLL.CheckInOut(_loginID, "In").Split('|');

            if (result[0] == "SUCCESS")
            {
                MessageBox.Show("Chúc ngày mới tốt lành", "Thông Tin", MessageBoxButtons.OK, MessageBoxIcon.Information);
                button1.Text = result[1];
            }
            else
            {
                MessageBox.Show(result[1], result[0], MessageBoxButtons.OK, result[0] == "Cảnh Báo" ? MessageBoxIcon.Warning : MessageBoxIcon.Error);
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            if (button1.Text == "Check in")
            {
                MessageBox.Show("Không thể check out khi chưa check in", "Cảnh Báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (MessageBox.Show("Bạn có muốn check out?", "Cảnh Báo", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning) == DialogResult.OK)
            {
                string[] result = _userTimeBLL.CheckInOut(_loginID, "Out").Split('|');

                if (result[0] == "SUCCESS")
                {
                    MessageBox.Show("Chào tạm biệt", "Thông Báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    this.Close();
                }
                else
                {
                    MessageBox.Show(result[1], result[0], MessageBoxButtons.OK, result[0] == "Cảnh Báo" ? MessageBoxIcon.Warning : MessageBoxIcon.Error);
                }
            }


        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            button2.Enabled = false;
            textBox2.Text = "";
        }

        private void button2_Click(object sender, EventArgs e)
        {
            // 1. Lấy danh sách tất cả các ô đang được bôi chọn (Selected Cells)
            DataGridViewSelectedCellCollection selectedCells = dataGridView1.SelectedCells;
            if (selectedCells.Count > 1)
            {
                MessageBox.Show("Không được chọn hơn 1 ô trống", "Thông Báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            if (selectedCells[0].Style.BackColor != Color.Empty)
            {
                MessageBox.Show("Chỉ được chọn ô trống", "Thông Báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            int col = selectedCells[0].ColumnIndex;
            int row = selectedCells[0].RowIndex;
            string result = _userBLL.SetPosition(textBox1.Text, col, row);
            if (result.IndexOf("SUCCESS") > -1)
            {
                MessageBox.Show("Cài đặt vị trí người dùng thành công", "Thông Tin", MessageBoxButtons.OK, MessageBoxIcon.Information);
                string[] fullName = textBox2.Text.Split(' ');
                dataGridView1[col, row].Value = fullName[fullName.Length - 1];
                dataGridView1[col, row].Style.BackColor = Color.LightGray;
            }
            else
                MessageBox.Show(result, "Thông báo Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            

        }
    }
}
