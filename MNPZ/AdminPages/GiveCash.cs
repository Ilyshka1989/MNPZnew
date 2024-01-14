using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI.WebControls;
using System.Windows.Forms;
using MNPZ;
using MNPZ.DAO;

namespace MNPZ.AdminPages
{
    public partial class GiveCash : Form
    {
        public GiveCash()
        {
            InitializeComponent();
            InitialOperators();
            var user = (User)AppDomain.CurrentDomain.GetData("userTb");
            if (user == null)
            {
                LoginPage obj = new LoginPage();
                obj.Show();
                this.Hide();
            }
            else this.Text = "Администратор " + user.UserName;
        }
        UserContext userContext = new UserContext();
        List<User> users = new List<User>();

        

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

        private void label3_Click(object sender, EventArgs e)
        {
            SavedData obj = new SavedData();
            obj.Show();
            this.Hide();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (textBox1.Text.Length > 0 &&
                textBox2.Text.Length > 0 &&
                textBox3.Text.Length > 0 &&
                textBox4.Text.Length > 0)
            {
                try
                {
                    var byn = Convert.ToDecimal(textBox1.Text);
                    var usd = Convert.ToDecimal(textBox2.Text);
                    var eur = Convert.ToDecimal(textBox3.Text);
                    var rub = Convert.ToDecimal(textBox4.Text);
                    var now = DateTime.Now;
                    var id = users.First(x => x.UserName == comboBox1.SelectedValue.ToString()).Id;

                    var op = new Operation();
                    var opEx = new OperationEx();

                    op.UserId = id;
                    op.DateOperation = DateTime.Now;
                    op.Cur_in_num = CurrencyNumber.BYN;
                    op.Cost = byn;
                    op.IsExchange = false;
                    opEx.SaveOperation(op);

                    op.Cur_in_num = CurrencyNumber.USD;
                    op.Cost = usd;
                    opEx.SaveOperation(op);

                    op.Cur_in_num = CurrencyNumber.EUR;
                    op.Cost = eur;
                    opEx.SaveOperation(op);

                    op.Cur_in_num = CurrencyNumber.RUB;
                    op.Cost = rub;
                    opEx.SaveOperation(op);


                    ClearTextBox();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }
        private void ClearTextBox()
        {
            textBox1.Text = string.Empty;
            textBox2.Text = string.Empty;
            textBox3.Text = string.Empty;
            textBox4.Text = string.Empty;
        }
        private void InitialOperators()
        {
            users = userContext.SelectAllUsers(true);

            var names = users.Select(x => x.UserName).ToArray();
            comboBox1.DataSource = names;
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
