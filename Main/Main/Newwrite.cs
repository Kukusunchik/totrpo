using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Main
{
    public partial class Newwrite : Form
    {
        DB db = new DB();
        string namescolumns = "";
        int columnsnumber;
        string columnsdatatypes = "";
        string tablename = "";
        public Newwrite(int columnnumber, string namecolumns, string datatypes, string tl)
        {
            InitializeComponent();
            columnsnumber = columnnumber - 1;
            dataGridView1.ColumnCount = columnsnumber;
            namescolumns = namecolumns;
            columnsdatatypes = datatypes;
            tablename = tl;
        }

        private void Newwrite_Load(object sender, EventArgs e)
        {
            dataGridView1.RowCount = 1;
            dataGridView1.ReadOnly = false;
            List<string> columns = new List<string>();
            columns = namescolumns.Split(' ').ToList();
            for (int i = 0; i < columnsnumber; i++)
            {
                dataGridView1.Columns[i].HeaderText = $"{columns[i + 1]}";
            }
            
        }

        private void Addbutton_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show(
               "Точно хотите добавить данные?",
               "Сообщение",
               MessageBoxButtons.YesNo,
               MessageBoxIcon.Information,
               MessageBoxDefaultButton.Button1,
               MessageBoxOptions.DefaultDesktopOnly);

            if (result == DialogResult.Yes)
            {
                List<string> datatypes = new List<string>(columnsnumber);
                List<string> datatypescheck = new List<string>(columnsnumber);
                bool checkdatatype = true;

                datatypes = columnsdatatypes.Split(' ').ToList();
                datatypes.Remove(datatypes[0]);
                datatypes.Remove(datatypes[datatypes.Count - 1]);

                for (int i = 0; i < datatypes.Count; i++)
                {
                    if (datatypes[i] == "varchar" || datatypes[i] == "nvarchar")
                        datatypes[i] = "string";
                    else if (datatypes[i] == "date")
                        datatypes[i] = "DateTime";
                }

                string[] formats = { "dd.MM.yyyy", "yyyy-MM-dd" };
                bool checkformats = true;
                try
                {
                    for (int i = 0; i < columnsnumber; i++)
                    {

                        if (int.TryParse(dataGridView1.Rows[0].Cells[i].Value.ToString(), out int res))
                        {
                            if (datatypes[i] == "string")
                            {
                                datatypescheck.Add("string");
                            }
                            else
                            {
                                datatypescheck.Add("int");
                            }

                        }
                        else if (bool.TryParse(dataGridView1.Rows[0].Cells[i].Value.ToString(), out bool res2))
                        {
                            datatypescheck.Add("bit");
                        }
                        else if (DateTime.TryParseExact(dataGridView1.Rows[0].Cells[i].Value.ToString(), formats, null, DateTimeStyles.None, out DateTime dob))
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
                    MessageBox.Show("Необходимо заполнить все поля", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error); checkformats = false;
                }
                if (checkformats)
                {
                    for (int i = 0; i < columnsnumber; i++)
                    {
                        if (!(datatypescheck[i] == datatypes[i]))
                        { MessageBox.Show($"Ошибка типа данных {i + 1} столбце"); checkdatatype = false; break; }
                    }
                    if (checkdatatype)
                    {
                        List<string> columns = new List<string>();
                        columns = namescolumns.Split(' ').ToList();
                        columns.Remove(columns[0]);
                        columns.Remove(columns[columns.Count - 1]);
                        string query_insert = $"insert into {tablename} ( ";
                        for (int i = 0; i < columnsnumber; i++)
                        {
                            query_insert += $"{columns[i]},";
                        }
                        query_insert = query_insert.Remove(query_insert.Length - 1);
                        query_insert += ") values (";
                        for (int i = 0; i < columnsnumber; i++)
                        {
                            if (datatypes[i] == "int")
                            {
                                query_insert += $"{dataGridView1.Rows[0].Cells[i].Value},";
                            }
                            else
                            {
                                query_insert += $"'{dataGridView1.Rows[0].Cells[i].Value}',";
                            }

                        }
                        query_insert = query_insert.Remove(query_insert.Length - 1);
                        query_insert += ")";

                        db.Openconnection();
                        try
                        {
                            SqlCommand sqlCommand = new SqlCommand(query_insert, db.Getconnection());
                            sqlCommand.ExecuteNonQuery();
                            MessageBox.Show("Запись успешно добавлена");
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
                db.Closeconnection();
            }
        }
    }
} 