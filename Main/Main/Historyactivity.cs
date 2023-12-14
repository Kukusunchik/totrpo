using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Main
{
    public partial class Historyactivity : Form
    {
        string path;
        bool Is_admin;
        public Historyactivity(string path, bool Is_admin)
        {
            InitializeComponent();
            this.path = path;
            this.Is_admin = Is_admin;
            textBox1.ScrollBars = ScrollBars.Vertical;
        }

        private void Historyactivity_Load(object sender, EventArgs e)
        {
            StreamReader sr = new StreamReader(path);
            textBox1.Text = sr.ReadToEnd();
            sr.Close();
        }

        private void Exitbutton_Click(object sender, EventArgs e)
        {
            Form1 form = new Form1(path, Is_admin);
            this.Hide();
            form.ShowDialog();
        }
    }
}
