using System;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics.Eventing.Reader;
using System.Drawing;
using System.IO;
using System.Threading;
using System.Windows.Forms;

namespace Main
{
    public partial class log : Form
    {
        private string text = String.Empty;
        private string path = "C:\\Users\\Edisu\\Downloads\\practic\\lab_5\\Main\\log.txt";
        private int num = 0;
        DB db = new DB();
        public log()
        {
            InitializeComponent();
            pb1.Image = this.CreateImage(pb1.Width, pb1.Height);
            pbopen.Visible = false;
            pbclose.Visible = true;
            passwordtb.UseSystemPasswordChar = false;
            passwordtb.AutoSize = false;
            passwordtb.Height = 26;
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            Height = 180;
            pictureBox1.Image = Properties.Resources.logo;
        }
        private Bitmap CreateImage(int Width, int Height)
        {
            Random rnd = new Random();

            //Создадим изображение
            Bitmap result = new Bitmap(Width, Height);

            //Вычислим позицию текста
            int Xpos = rnd.Next(0, Width - 50);
            int Ypos = rnd.Next(15, Height - 15);

            //Добавим различные цвета
            Brush[] colors = { Brushes.Black,
                     Brushes.Red,
                     Brushes.RoyalBlue,
                     Brushes.AliceBlue };

            //Укажем где рисовать
            Graphics g = Graphics.FromImage((System.Drawing.Image)result);

            //Пусть фон картинки будет серым
            g.Clear(Color.Gray);

            //Сгенерируем текст
            text = String.Empty;
            string ALF = "1234567890QWERTYUIOPASDFGHJKLZXCVBNM";
            for (int i = 0; i < 4; ++i)
                text += ALF[rnd.Next(ALF.Length)];

            //Нарисуем сгенирируемый текст
            g.DrawString(text,
                         new Font("Arial", 15),
                         colors[rnd.Next(colors.Length)],
                         new PointF(Xpos, Ypos));

            //Добавим немного помех
            /////Линии из углов
            g.DrawLine(Pens.Black,
                       new Point(0, 0),
                       new Point(Width - 1, Height - 1));
            g.DrawLine(Pens.Black,
                       new Point(0, Height - 1),
                       new Point(Width - 1, 0));
            ////Белые точки
            for (int i = 0; i < Width; ++i)
                for (int j = 0; j < Height; ++j)
                    if (rnd.Next() % 20 == 0)
                        result.SetPixel(i, j, Color.White);

            return result;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            pb1.Image = this.CreateImage(pb1.Width, pb1.Height);
        }
        private bool Is_admin(string login, string password)
        {

            SqlDataAdapter adapter = new SqlDataAdapter();
            DataTable datatable = new DataTable();

            string querystring = $"select login,password from staff where login = '{login}' and password = '{password}' and Is_admin = 1";
            SqlCommand command = new SqlCommand(querystring, db.Getconnection());

            adapter.SelectCommand = command;
            adapter.Fill(datatable);

            if (datatable.Rows.Count == 1)
                return true;
            return false;
        }
        private void button2_Click(object sender, EventArgs e)
        {
            var loginUser = logintb.Text;
            var passUser = passwordtb.Text;
            bool success = false;

            SqlDataAdapter adapter = new SqlDataAdapter();
            DataTable datatable = new DataTable();

            string querystring = $"select login,password from staff where login = '{loginUser}' and password = '{passUser}'";

            SqlCommand command = new SqlCommand(querystring, db.Getconnection());

            adapter.SelectCommand = command;
            adapter.Fill(datatable);

            if (datatable.Rows.Count == 1)
            {
                if (!(num != 0) || textBox1.Text == text)
                {
                    MessageBox.Show("Вход успешно произведен");
                    FileInfo fileInfos = new FileInfo(path);
                    if (!fileInfos.Exists)
                        using (FileStream fs = File.Create(path)) { };
                    success = true;
                    var myData = new
                    {
                        datetime = DateTime.Now.ToString("MM/dd/yyyy h:mm tt"),
                        UserName = loginUser,
                        succes = success
                    };
                    StreamWriter file = new StreamWriter(path, true);
                    file.WriteLine($"{{\"data\":[\"{myData.datetime}\",\"{myData.UserName}\",\"{myData.succes}\"]}}");
                    file.Close();
                    Form1 form = new Form1(path, Is_admin(loginUser, passUser));
                    this.Hide();
                    form.ShowDialog();
                    
                }
                else
                    MessageBox.Show("Капча неверна");
            }
            else
                MessageBox.Show("Такого аккаунта нет");

            FileInfo fileInfo = new FileInfo(path);
            if (!fileInfo.Exists)
                using (FileStream fs = File.Create(path)) { };
            var mydata = new
            {
                datetime = DateTime.Now.ToString("MM/dd/yyyy h:mm tt"),
                UserName = loginUser,
                succes = success
            };
            StreamWriter files = new StreamWriter(path, true);
            files.WriteLine($"{{\"data\":[\"{mydata.datetime}\",\"{mydata.UserName}\",\"{mydata.succes}\"]}}");
            files.Close();
            if (num == 0)
            {
                Height = 380;
                pb1.Location = new Point(pb1.Location.X, pb1.Location.Y - 30);
                textBox1.Location = new Point(textBox1.Location.X, textBox1.Location.Y - 30);
                button1.Location = new Point(button1.Location.X, button1.Location.Y - 30);
                button2.Location = new Point(button2.Location.X, button2.Location.Y + 198);
            }
            else if (num == 1)
            {
                Thread.Sleep(1000 * 10);

            }
            else if (num == 2)
            {
                System.Windows.Forms.Application.Restart();
            }
            num += 1;

        }

        private void pbopen_Click(object sender, EventArgs e)
        {
            passwordtb.UseSystemPasswordChar = false;
            pbclose.Visible = true;
            pbopen.Visible = false;
        }

        private void pbclose_Click(object sender, EventArgs e)
        {
            passwordtb.UseSystemPasswordChar = true;
            pbclose.Visible = false;
            pbopen.Visible = true;
        }
    }
}
