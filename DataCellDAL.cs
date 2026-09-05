using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.SQLite;

namespace Attendance
{
    public class DataCellDAL : DatabaseConnection
    {
        public List<DataCellDTO> GetListColorCells()
        {
            List<DataCellDTO> list = new List<DataCellDTO>();

            using (SQLiteConnection? conn = GetOpenedConnection())
            {
                if (conn == null) return list; // Trả về danh sách trống nếu lỗi kết nối

                string sql = "SELECT Col, Row, Color FROM DataCells";

                using (SQLiteCommand cmd = new SQLiteCommand(sql, conn))
                {
                    using (SQLiteDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            DataCellDTO cell = new DataCellDTO
                            {
                                Col = Convert.ToInt32(reader["Col"]),
                                Row = Convert.ToInt32(reader["Row"]),
                                Color = reader["Color"].ToString() ?? "#000000"
                            };
                            list.Add(cell);
                        }
                    }
                }
            }
            return list;
        }
    }
}
