using MNPZ.AdminPages;
using MNPZ.DAO;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Runtime.ConstrainedExecution;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;

namespace MNPZ
{
    public partial class SavedData : Form
    {
        List<User> users = new List<User>();
        List<OperationVm> operations = new List<OperationVm>();
        OperationContext opContext = new OperationContext();
        UserContext userContext = new UserContext();
        public SavedData()
        {
            InitializeComponent();
            InitialUsers();
            InitialView();
            InitialDataGrid();
            var user = (User)AppDomain.CurrentDomain.GetData("userTb");
            if (user == null)
            {
                LoginPage obj = new LoginPage();
                obj.Show();
                this.Hide();
            }
            else this.Text = "Администратор " + user.UserName;
        }

        private void label1_Click(object sender, EventArgs e)
        {
            AdminPage obj = new AdminPage();
            obj.Show();
            this.Hide();
        }

        private void label2_Click(object sender, EventArgs e)
        {
            ExchangRates obj = new ExchangRates();
            obj.Show();
            this.Hide();
        }

        private void label6_Click(object sender, EventArgs e)
        {
            GiveCash obj = new GiveCash();
            obj.Show();
            this.Hide();
        }
        private void InitialDataGrid()
        {
           dataGridView1.DataSource = operations.ToArray();
        }
        private void InitialView()
        {
            operations = opContext.SelectAllOperations();
        }
        private void InitialUsers()
        {
            users = userContext.SelectAllUsers(true);
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
