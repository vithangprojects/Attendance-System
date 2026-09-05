using System;
using System.Collections.Generic;
using System.Text;

namespace Attendance
{
    public class DataCellDTO
    {
        public int Col { get; set; }
        public int Row { get; set; }
        public string Color { get; set; } = "#000000"; // Mặc định là chuỗi mã Hex

        public DataCellDTO() { }

        public DataCellDTO(int col, int row, string color)
        {
            this.Col = col;
            this.Row = row;
            this.Color = color;
        }
    }
}

