using MNPZ.DAO;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Windows.Forms;

namespace MNPZ
{
    public partial class LoginPage : Form
    {
        public LoginPage()
        {
            InitializeComponent();
            CheckUsers();
        }
        UserContext userContext = new UserContext();
        private void button1_Click(object sender, EventArgs e)
        {
            var user = userContext.SelectUserBy(true, login.Text);
            if (user == null)
            {
                MessageBox.Show("Не правильный логин или пароль");
            }
            else
            {
                if (user.Password != pass.Text)
                {
                    MessageBox.Show("Не правильный логин или пароль");
                }
                else
                {
                    AppDomain.CurrentDomain.SetData("userTb", user);
                    if (user.IsOperator)
                    {
                        OperatorPage obj = new OperatorPage();
                        obj.Show();
                        this.Hide();
                    }
                    else
                    {
                        AdminPage obj = new AdminPage();
                        obj.Show();
                        this.Hide();
                    }
                }
            }
        }

        private void CheckUsers()
        {
            var users = userContext.SelectAllUsers();
            if (users.Count() == 0)
            {
                var check = userContext.InsertUser("q", "root Admin", "q", false);
            }
        }
    }
}
