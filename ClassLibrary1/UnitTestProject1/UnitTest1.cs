using DatabaseController;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Runtime.InteropServices;

namespace UnitTestProject1
{
    [TestClass]
    public class UnitTest1
    {
        string sqlstr = @"Data Source=FIREFLYYYYY;Initial Catalog=lab_2;Integrated Security=true;";
        object result = null;
        [TestMethod]
        public void GetTableRowsCount()
        {
            DB db = new DB(sqlstr);
            string query = "staff";
            bool result = db.SelectAllFromTable(query);
            Assert.IsTrue(
            result
            );
        }
        [TestMethod]
        public void UpdateRowFromTable()
        {
            DB db = new DB(sqlstr);
            string tablename = "staff";
            int idname = 10;
            string login = "login";
            object inputvalue = "thisisthegreatestshow";
            bool result = db.UpdateRowDataBaseForOneIntPrimaryKey(tablename,idname,login,inputvalue);
            Assert.IsTrue(
            result
            );
        }

        [TestMethod]
        public void DeleteRowDataBaseForOneIntPrimaryKey()
        {
            DB db = new DB(sqlstr);
            string tablename = "staff";
            int idname = 10;
            bool result = db.DeleteRowDataBaseForOneIntPrimaryKey(tablename, idname);
            Assert.IsTrue(
            result
            );
        }
    }
}
