using System.Data;
using System.Data.SqlClient;

namespace QLSV
{
    public class DBConnection
    {
        public static SqlConnection conn;

        // Phương thức kết nối tới cơ sở dữ liệu
        public static void Connect()
        {
            conn = new SqlConnection();
            conn.ConnectionString = @"Data Source=LAPTOP-028H0UE8;Initial Catalog=qlsv_2;Integrated Security=True;";
            conn.Open();
        }

        // Phương thức ngắt kết nối cơ sở dữ liệu
        public static void Disconnect()
        {
            if (conn.State == ConnectionState.Open)
            {
                conn.Close();
                conn.Dispose();
                conn = null;
            }
        }

    }
}