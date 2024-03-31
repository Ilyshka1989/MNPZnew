using MNPZ.DAL.Repositories;
using System;
using System.Linq;
using System.Windows.Forms;

namespace MNPZ
{
    public partial class LoginPage : Form
    {
        private readonly UserRepository _userRepository;

        public LoginPage()
        {
            _userRepository = new UserRepository();
            InitializeComponent();
            CheckUsers();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var user = _userRepository.SelectUserBy(true, login.Text);
            if (user == null)
            {
                MessageBox.Show("Неправильный логин или пароль", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                if (user.Password != pass.Text)
                {
                    MessageBox.Show("Неправильный логин или пароль", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    AppDomain.CurrentDomain.SetData("User", user);
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
            var users = _userRepository.SelectAllUsers();
            if (users.Count() == 0)
            {
                var check = _userRepository.InsertUser("q", "root Admin", "q", false);
            }
        }
    }
}
