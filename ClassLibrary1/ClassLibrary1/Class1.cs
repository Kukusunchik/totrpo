using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace DatabaseController
{
    public class DB
    {
        SqlConnection sqlConnection;
        public DB(string sqlconnect)
        {
            sqlConnection = new SqlConnection(@sqlconnect);
        }

        internal void Openconnection()
        {
            if (sqlConnection.State == System.Data.ConnectionState.Closed)
            {
                sqlConnection.Open();
            }
        }
        internal void Closeconnection()
        {
            if (sqlConnection.State == System.Data.ConnectionState.Open)
            {
                sqlConnection.Close();
            }
        }

        internal SqlConnection Getconnection()
        {
            return sqlConnection;
        }

        public bool SelectAllFromTable(string tablename)
        {
            try
            {
                string query_selectall = $"select * from {tablename}";

                SqlCommand sqlCommand = new SqlCommand(query_selectall, Getconnection());

                Openconnection();

                SqlDataReader reader = sqlCommand.ExecuteReader();

                DataTable dt = new DataTable();

                dt.Load(reader);

                reader.Close();

                Closeconnection();

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public bool UpdateRowDataBaseForOneIntPrimaryKey(string tablename, int idname, string columnname, object inputvalue)
        {
            try
            {
                Openconnection();
                var columnget = $"select top 1 column_name  from information_schema.columns  where TABLE_NAME = '{tablename}'";
                var sqlCommand = new SqlCommand(columnget, Getconnection());

                SqlDataReader reader = sqlCommand.ExecuteReader();

                DataTable dt = new DataTable();

                dt.Load(reader);

                reader.Close();
                string firstcolumn = "";
                foreach (DataRow row in dt.Rows)
                {
                    firstcolumn = row[0].ToString();
                }
                var updateQuery = $"update {tablename} set {columnname} = '{inputvalue}' where {firstcolumn} = {idname}";
                sqlCommand = new SqlCommand(updateQuery, Getconnection());

                reader = sqlCommand.ExecuteReader();

                dt = new DataTable();

                dt.Load(reader);

                reader.Close();

                Closeconnection();

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        
        public bool DeleteRowDataBaseForOneIntPrimaryKey(string tablename, int idname)
        {
            try
            {
                Openconnection();
                var columnget = $"select top 1 column_name  from information_schema.columns  where TABLE_NAME = '{tablename}'";
                var sqlCommand = new SqlCommand(columnget, Getconnection());

                SqlDataReader reader = sqlCommand.ExecuteReader();

                DataTable dt = new DataTable();

                dt.Load(reader);

                reader.Close();
                string firstcolumn = "";
                foreach (DataRow row in dt.Rows)
                {
                    firstcolumn = row[0].ToString();
                }
                columnget = $"delete from {tablename} where {firstcolumn} = {idname}";
                sqlCommand = new SqlCommand(columnget, Getconnection());

                reader = sqlCommand.ExecuteReader();

                Closeconnection();

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}