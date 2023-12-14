using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;

namespace Main
{
    class DB
    {
        SqlConnection sqlConnection = new SqlConnection(@"Data Source=FIREFLYYYYY;Initial Catalog=lab_2;Integrated Security=True");

        public void Openconnection()
        {
            if (sqlConnection.State == System.Data.ConnectionState.Closed)
            {
                sqlConnection.Open();
            }
        }
        public void Closeconnection()
        {
            if (sqlConnection.State == System.Data.ConnectionState.Open)
            {
                sqlConnection.Close();
            }
        }

        public SqlConnection Getconnection()
        {
            return sqlConnection;
        }
    }
}
