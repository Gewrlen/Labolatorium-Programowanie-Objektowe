using Microsoft.Data.SqlClient;

namespace SystemOcenianiaSimple.Data
{
    public static class Db
    {
        public static SqlConnection Open()
        {
            var conn = new SqlConnection(DbConfig.GetConnectionString());
            conn.Open();
            return conn;
        }
    }
}
