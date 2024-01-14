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
using System.Data.SqlClient;
using System.Web.UI.WebControls;
using MNPZ.AdminPages;
using MNPZ.DAO;

namespace MNPZ
{
    public partial class AdminPage : Form
    {
        public AdminPage()
        {
            InitializeComponent();
            DisplayUser();
            var user = (User)AppDomain.CurrentDomain.GetData("userTb");
            if (user == null)
            {
                LoginPage obj = new LoginPage();
                obj.Show();
                this.Hide();
            }
            else this.Text = "Администратор " +user.UserName;
        }
        UserContext userContext = new UserContext();
        private void Reset()
        {
            isOperator.Enabled = false;
            Name.Text = "";
            Login.Text = "";
            Password.Text = "";
        }
        private void DisplayUser()
        {
            var users = userContext.SelectAllUsers();
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
            if (Name.Text == "")
            {
                MessageBox.Show("Не указано имя пользователя!");
            } else if ( Login.Text == "")
            {
                MessageBox.Show("Не указан логин пользователя!");
            }
            else if (Password.Text == "")
            {
                MessageBox.Show("Не указан пароль!");
            }
            else
            {
                var insertUser = userContext.InsertUser(Login.Text,Name.Text,Password.Text, isOperator.Checked);

                MessageBox.Show(insertUser.Message);

                DisplayUser();
                Reset();
            }
        }


        int UserId = 0;
        private void OperatorsGDV_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            UserId = Convert.ToInt32(dataGridView1.SelectedRows[0].Cells[0].Value.ToString());
            Name.Text = dataGridView1.SelectedRows[0].Cells[1].Value.ToString();
            Login.Text = dataGridView1.SelectedRows[0].Cells[2].Value.ToString();
            Password.Text = dataGridView1.SelectedRows[0].Cells[3].Value.ToString();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (UserId == 0)
            {
                MessageBox.Show("Выбирете объект");
            }
            else
            {
                var deleteUser = userContext.DeleteUserById(UserId);
                MessageBox.Show(deleteUser.Message);
                DisplayUser();
                Reset();
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (Name.Text == "" || Login.Text == "" || Password.Text == "")
            {
                MessageBox.Show("Информация отсуствует");
            }
            else
            {
                var deleteUser = userContext.UpdateUserById(UserId,Login.Text,Name.Text,Password.Text,isOperator.Checked);
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
            AppDomain.CurrentDomain.SetData("userTb", null);
            LoginPage obj = new LoginPage();
            obj.Show();
            this.Hide();
        }
    } 
    }

