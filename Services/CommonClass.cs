using System.Data;
using System.Data.SqlClient;

namespace PracticeCoreMVC.Services
{
    public class CommonClass
    {
        public readonly IConfiguration _configuration;
        public CommonClass(IConfiguration config) 
        {
            this._configuration = config;
        }
        public DataTable? GetDataByQuery(string query) 
        {
            if(!query.Trim().ToLower().StartsWith("select ")) 
            {
                throw new ArgumentException("Only SELECT queries are allowed.");
            }
            DataTable dt = new DataTable();
            try 
            {
                using(SqlConnection connection = new SqlConnection( _configuration.GetConnectionString("PracticeDB"))) 
                {
                    connection.Open();
                    using(SqlCommand cmd = new SqlCommand(query, connection))
                    {
                        dt.Load(cmd.ExecuteReader());
                    }
                }
                return dt;
            }
            catch(Exception) {
                return null;
            }
        }
    }
}