using MNPZ.AdminPages;
using MNPZ.DAL.Models;
using MNPZ.DAO;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace MNPZ
{
    public partial class SavedData : Form
    {
        private IEnumerable<Operation> operations;
        private readonly OperationRepository _opContext;

        public SavedData()
        {
            _opContext = new OperationRepository();
            InitializeComponent();
            InitialView();
            InitialDataGrid();
            var user = (User)AppDomain.CurrentDomain.GetData("User");
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
            dataGridView1.DataSource = operations;
        }
        private void InitialView()
        {
            operations = _opContext.SelectAllOperations();
        }

        private void label4_Click(object sender, EventArgs e)
        {
            AppDomain.CurrentDomain.SetData("User", null);
            LoginPage obj = new LoginPage();
            obj.Show();
            this.Hide();
        }
    }
}
