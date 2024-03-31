using MNPZ.AdminPages;
using MNPZ.DAL.Models;
using MNPZ.DAL.Repositories;
using System;
using System.Windows.Forms;

namespace MNPZ
{
    public partial class AdminPage : Form
    {
        public AdminPage()
        {
            InitializeComponent();
            DisplayUser();
            var user = (User)AppDomain.CurrentDomain.GetData("User");
            if (user == null)
            {
                LoginPage obj = new LoginPage();
                obj.Show();
                this.Hide();
            }
            else this.Text = "Администратор " + user.UserName;
        }
        UserRepository _userRepository = new UserRepository();
        private void Reset()
        {
            isOperator.Enabled = false;
            Namee.Text = "";
            Login.Text = "";
            Password.Text = "";
        }
        private void DisplayUser()
        {
            var users = _userRepository.SelectAllUsers();
            dataGridView1.DataSource = users.ToArray();
        }
        private void label2_Click(object sender, EventArgs e)
        {
            ExchangRates obj = new ExchangRates();
            obj.Show();
            this.Hide();
        }

        private void label3_Click(object sender, EventArgs e)
        {
            SavedData obj = new SavedData();
            obj.Show();
            this.Hide();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (Namee.Text == "")
            {
                MessageBox.Show("Не указано имя пользователя!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else if (Login.Text == "")
            {
                MessageBox.Show("Не указан логин пользователя!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else if (Password.Text == "")
            {
                MessageBox.Show("Не указан пароль!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                var insertUser = _userRepository.InsertUser(Login.Text, Namee.Text, Password.Text, isOperator.Checked);

                MessageBox.Show(insertUser.Message);

                DisplayUser();
                Reset();
            }
        }


        int UserId = 0;
        private void OperatorsGDV_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (UserId == 0)
            {
                MessageBox.Show("Выберете объект");
            }
            else
            {
                if (_userRepository.TryDeleteUser(UserId))
                {
                    MessageBox.Show("Пользователь удален");
                }
                else
                {
                    MessageBox.Show("Не удалось удалить пользователя", "Ошибка", MessageBoxButtons.OK);
                }

                DisplayUser();
                Reset();
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (Namee.Text == "" || Login.Text == "" || Password.Text == "")
            {
                MessageBox.Show("Информация отсуствует");
            }
            else
            {
                var deleteUser = _userRepository.UpdateUserById(UserId, Login.Text, Namee.Text, Password.Text, isOperator.Checked);
                MessageBox.Show(deleteUser.Message);
                DisplayUser();
                Reset();
            }
        }
        private void label9_Click(object sender, EventArgs e)
        {
            GiveCash obj = new GiveCash();
            obj.Show();
            this.Hide();
        }

        private void label4_Click(object sender, EventArgs e)
        {
            AppDomain.CurrentDomain.SetData("User", null);
            LoginPage obj = new LoginPage();
            obj.Show();
            this.Hide();
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            
        }

        private void dataGridView1_SelectionChanged(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count > 0)
            {
                UserId = Convert.ToInt32(dataGridView1.SelectedRows[0].Cells[0].Value.ToString());
                Namee.Text = dataGridView1.SelectedRows[0].Cells[1].Value.ToString();
                Login.Text = dataGridView1.SelectedRows[0].Cells[2].Value.ToString();
                Password.Text = dataGridView1.SelectedRows[0].Cells[3].Value.ToString();
            }
        }
    }
}

