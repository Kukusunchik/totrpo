using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;

namespace Main
{
    public partial class Form1 : Form
    {
        DB db = new DB();
        int columnsnumber;
        string namecolumns = "";
        string namedatatypes = "";
        string tablename = "";
        int rowindexcelledit, columnindexcelledit;
        string path;
        bool Is_admin = false;

        public Form1(string path, bool Is_admin)
        {
            InitializeComponent();
            this.path = path;
            this.Is_admin = Is_admin;
            string query_tables_number = $"select table_name from information_schema.columns group by table_name";
            SqlCommand sqlCommand = new SqlCommand(query_tables_number, db.Getconnection());

            db.Openconnection();

            SqlDataReader reader = sqlCommand.ExecuteReader();

            while (reader.Read())
            {
                if (reader.GetString(0) != "sysdiagrams")
                {
                    if (!(!Is_admin && (reader.GetString(0) == "staff")))
                    {
                        comboBox1.Items.Add(reader.GetString(0));
                    }
                } 
            }
            if (!Is_admin)
                Historyactivitybutton.Visible = false;
            reader.Close();
            
            db.Closeconnection();
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            Newwritebutton.Visible = false;
            Editbutton.Visible = false;
            Deletebutton.Visible = false;
            Refreshbutton.Visible = false;
            Historyactivitybutton.Visible = false;
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            columnsnumber = 0;
            namecolumns = "";
            tablename = "";
            namedatatypes = "";

            string query_selectall = $"select * from {comboBox1.SelectedItem}";
            string query_selectcolumncount = $"select count(*) as number from information_schema.columns group by table_name having table_name = '{comboBox1.SelectedItem}'";
            string query_allcolumnsnames = $"select COLUMN_NAME from INFORMATION_SCHEMA.COLUMNS where TABLE_NAME = '{comboBox1.SelectedItem}'";
            string query_columns_data_types = $"select DATA_TYPE from INFORMATION_SCHEMA.COLUMNS where TABLE_NAME = '{comboBox1.SelectedItem}'";

            SqlCommand sqlCommand = new SqlCommand(query_selectall, db.Getconnection());
            SqlCommand sqlCommand1 = new SqlCommand(query_selectcolumncount, db.Getconnection());
            SqlCommand sqlCommand2 = new SqlCommand(query_allcolumnsnames, db.Getconnection());
            SqlCommand sqlCommand3 = new SqlCommand(query_columns_data_types, db.Getconnection());

            tablename = comboBox1.SelectedItem.ToString();

            db.Openconnection();

            SqlDataReader reader = sqlCommand.ExecuteReader();

            DataTable dt = new DataTable();

            dt.Load(reader);

            dataGridView1.DataSource = dt;

            reader.Close();

            db.Closeconnection();

            db.Openconnection();

            SqlDataReader reader1 = sqlCommand1.ExecuteReader();

            if (reader1.Read())
                columnsnumber = reader1.GetInt32(0);

            reader1.Close();

            db.Closeconnection();

            db.Openconnection();

            SqlDataReader reader2 = sqlCommand2.ExecuteReader();
            
            while (reader2.Read())
                namecolumns += reader2.GetString(0) + " ";

            namecolumns.Remove(namecolumns.Length - 1);
            db.Closeconnection();

            db.Openconnection();

            SqlDataReader reader3 = sqlCommand3.ExecuteReader();

            while (reader3.Read())
                namedatatypes += reader3.GetString(0) + " ";

            db.Closeconnection();

            if (Is_admin)
            {
                Newwritebutton.Visible = true;
                Editbutton.Visible = true;
                Deletebutton.Visible = true;
                Refreshbutton.Visible = true;
                Historyactivitybutton.Visible = true;
            }
            else
            {
                Newwritebutton.Visible = false;
                Editbutton.Visible = false;
                Deletebutton.Visible = false;
                Refreshbutton.Visible = false;
                Historyactivitybutton.Visible = false;
            }
        }

        private void Newwritebutton_Click(object sender, EventArgs e)
        {
            Newwrite Newwrite = new Newwrite(columnsnumber,namecolumns, namedatatypes, tablename);
            Newwrite.ShowDialog();
        }

        private void Deletebutton_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show(
              "Точно хотите удалить данные?",
              "Сообщение",
              MessageBoxButtons.YesNo,
              MessageBoxIcon.Information,
              MessageBoxDefaultButton.Button1,
              MessageBoxOptions.DefaultDesktopOnly);

            if (result == DialogResult.Yes)
            {
                try
                {
                    int index = dataGridView1.CurrentCell.RowIndex;
                    CurrencyManager currencyManager1 = (CurrencyManager)BindingContext[dataGridView1.DataSource];
                    currencyManager1.SuspendBinding();
                    dataGridView1.Rows[index].Visible = false;
                    currencyManager1.ResumeBinding();

                    db.Openconnection();

                    var idaccount = Convert.ToInt32(dataGridView1.Rows[index].Cells[0].Value);
                    var columnName = dataGridView1.Columns[0].Name;
                    var deleteQuery = $"delete from {comboBox1.SelectedItem} where {columnName} = '{idaccount}'";
                    var command = new SqlCommand(deleteQuery, db.Getconnection());

                    try
                    {
                        command.ExecuteNonQuery();
                        dataGridView1.Rows.RemoveAt(dataGridView1.Rows.Count - 1);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }

                    db.Closeconnection();
                }
                catch (Exception ex)
                {

                    MessageBox.Show(ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
        private void Editbutton_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show(
              "Точно хотите изменить данные?",
              "Сообщение",
              MessageBoxButtons.YesNo,
              MessageBoxIcon.Information,
              MessageBoxDefaultButton.Button1,
              MessageBoxOptions.DefaultDesktopOnly);

            if (result == DialogResult.Yes)
            {
                db.Openconnection();

                string changeQuery = $"update {comboBox1.SelectedItem} set ";
                List<string> columns = new List<string>();
                List<string> datatypes = new List<string>(columnsnumber);
                List<string> datatypescheck = new List<string>(columnsnumber);
                bool checkdatatype = true;

                datatypes = namedatatypes.Split(' ').ToList();
                datatypes.Remove(datatypes[0]);
                datatypes.Remove(datatypes[datatypes.Count - 1]);
                columns = namecolumns.Split(' ').ToList();
                columns.Remove(columns[columns.Count - 1]);

                for (int i = 0; i < datatypes.Count; i++)
                {
                    if (datatypes[i] == "varchar" || datatypes[i] == "nvarchar")
                        datatypes[i] = "string";
                    else if (datatypes[i] == "date")
                        datatypes[i] = "DateTime";
                    else if (datatypes[i] == "bit")
                        datatypes[i] = "bit";
                }

                string[] formats = { "dd.MM.yyyy h:mm:ss", "yyyy-MM-dd h:mm:ss", "dd.MM.yyyy", "yyyy-MM-dd" };
                string[] formatsadd = { "dd.MM.yyyy h:mm:ss", "dd.MM.yyyy" };
                bool checkformats = true;
                try
                {
                    for (int i = 0; i < datatypes.Count; i++)
                    {
                        if (int.TryParse(dataGridView1.Rows[rowindexcelledit].Cells[i + 1].Value.ToString(), out int res))
                        {
                            if (datatypes[i] == "bit")
                            {
                                datatypescheck.Add("bit");
                            }
                            else
                            {
                                if (DateTime.TryParseExact(dataGridView1.Rows[rowindexcelledit].Cells[i + 1].Value.ToString(), formats, null, DateTimeStyles.None, out DateTime dob))
                                {
                                    datatypescheck.Add("DateTime");
                                }
                                else
                                {
                                    if (datatypes[i] == "int")
                                    {
                                        datatypescheck.Add("int");
                                    }
                                    else
                                    {
                                        datatypescheck.Add("string");
                                    }
                                }
                            }
                        }
                        else if (bool.TryParse(dataGridView1.Rows[rowindexcelledit].Cells[i + 1].Value.ToString(), out bool res2))
                        {
                            datatypescheck.Add("bit");
                        }
                        else if (DateTime.TryParseExact(dataGridView1.Rows[rowindexcelledit].Cells[i + 1].Value.ToString(), formats, null, DateTimeStyles.None, out DateTime dob))
                        {
                            datatypescheck.Add("DateTime");
                        }
                        else
                        {
                            datatypescheck.Add("string");
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error); checkformats = false;
                }

                if (checkformats)
                {
                    for (int i = 0; i < datatypes.Count; i++)
                    {
                        if (!(datatypescheck[i] == datatypes[i]))
                        { MessageBox.Show($"Ошибка типа данных {i} столбце"); checkdatatype = false; break; }
                    }

                    if (checkdatatype)
                    {
                        for (int i = 0; i < datatypes.Count; i++)
                        {
                            if (datatypes[i] == "int")
                                changeQuery += columns[i + 1] + " = " + dataGridView1.Rows[rowindexcelledit].Cells[i + 1].Value.ToString() + ", ";
                            else
                            {
                                if (DateTime.TryParseExact(dataGridView1.Rows[rowindexcelledit].Cells[i + 1].Value.ToString(), formatsadd, null, DateTimeStyles.None, out DateTime dob))
                                {
                                    string date = dataGridView1.Rows[rowindexcelledit].Cells[i + 1].Value.ToString().Remove(dataGridView1.Rows[rowindexcelledit].Cells[i + 1].Value.ToString().Length - 8);
                                    List<string> add = date.Split('.').ToList();
                                    date = $"{add[2]}-{add[1]}-{add[0]}";
                                    changeQuery += columns[i + 1] + $" = '{date}' , ";
                                }
                                else
                                {
                                    changeQuery += columns[i + 1] + $" = '{dataGridView1.Rows[rowindexcelledit].Cells[i + 1].Value.ToString()}' , ";
                                }
                            }
                        }
                        changeQuery = changeQuery.Remove(changeQuery.Length - 2);
                        changeQuery += $" where {columns[0]} = {dataGridView1.Rows[rowindexcelledit].Cells[0].Value}";
                        try
                        {
                            var command = new SqlCommand(changeQuery, db.Getconnection());
                            command.ExecuteNonQuery();
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            Refreshbutton_Click(sender, e);
                        }
                    }
                }
                db.Closeconnection();
            }
                
        }
        private void Refreshbutton_Click(object sender, EventArgs e)
        {
            string query_selectall = $"select * from {comboBox1.SelectedItem}";

            SqlCommand sqlCommand = new SqlCommand(query_selectall, db.Getconnection());

            db.Openconnection();

            SqlDataReader reader = sqlCommand.ExecuteReader();

            DataTable dt = new DataTable();

            dt.Load(reader);

            dataGridView1.DataSource = dt;

            reader.Close();

            db.Closeconnection();
        }

        private void Exitbutton_Click(object sender, EventArgs e)
        {
            log log = new log();
            this.Hide();
            log.ShowDialog();
        }

        private void Historyactivitybutton_Click(object sender, EventArgs e)
        {
            Historyactivity ha = new Historyactivity(path, Is_admin);
            ha.ShowDialog();
            this.Hide();
        }

        private void dataGridView1_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            rowindexcelledit = e.RowIndex;
            columnindexcelledit = e.ColumnIndex;
        }

    }
}