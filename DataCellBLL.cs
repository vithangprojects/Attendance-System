using System.Collections.Generic;

namespace Attendance
{
    public class DataCellBLL
    {
        private DataCellDAL dal = new DataCellDAL();

        public List<DataCellDTO> GetListColorCells()
        {
            // Gọi xuống lớp DAL để lấy dữ liệu thô từ Database
            return dal.GetListColorCells();
        }
    }
}
