using MNPZ.DAL.Models;
using System;
using System.Windows.Forms;

namespace MNPZ
{
    public partial class OperatorPage : Form
    {
        public OperatorPage()
        {
            InitializeComponent();
            var user = (User)AppDomain.CurrentDomain.GetData("user");
            if (user == null)
            {
                LoginPage obj = new LoginPage();
                obj.Show();
                this.Hide();
            }
            else this.Text = "Оператор " + user.UserName;
        }

        private void label1_Click(object sender, EventArgs e)
        {
            AdminPage obj = new AdminPage();
            obj.Show();
            this.Hide();
        }

        private void label2_Click(object sender, EventArgs e)
        {
            Calculate obj = new Calculate();
            obj.Show();
            this.Hide();
        }

        private void label3_Click(object sender, EventArgs e)
        {
            SavedData obj = new SavedData();
            obj.Show();
            this.Hide();
        }

        private void label7_Click(object sender, EventArgs e)
        {
            AppDomain.CurrentDomain.SetData("user", null);
            LoginPage obj = new LoginPage();
            obj.Show();
            this.Hide();
        }

        private void label5_Click(object sender, EventArgs e)
        {

        }
    }
}
