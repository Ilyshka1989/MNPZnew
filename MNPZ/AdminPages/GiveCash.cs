using MNPZ.DAL.Models;
using MNPZ.DAL.Repositories;
using MNPZ.DAO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace MNPZ.AdminPages
{
    public partial class GiveCash : Form
    {
        private readonly UserRepository _userRepository;
        private readonly OperationRepository _operationRepository;

        public List<User> Users { get; set; }

        public GiveCash()
        {
            _userRepository = new UserRepository();
            _operationRepository = new OperationRepository();
            InitializeComponent();
            InitialOperators();
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
                    var id = Users.First(x => x.UserName == comboBox1.SelectedValue.ToString()).Id;

                    var balances = new List<BaseBalanceItem>()
                    {
                        new BaseBalanceItem()
                        {
                            Balance = byn,
                            Currency = Currency.BYN
                        },
                        new BaseBalanceItem()
                        {
                            Balance = usd,
                            Currency = Currency.USD
                        } ,
                        new BaseBalanceItem()
                        {
                            Balance = eur,
                            Currency = Currency.EUR
                        } ,
                        new BaseBalanceItem()
                        {
                            Balance = rub,
                            Currency = Currency.RUB
                        }
                    };

                    _operationRepository.AddFundsToUserBalance(id, balances);

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
            Users = _userRepository.SelectAllUsers(true);

            var names = Users.Select(x => x.UserName).ToArray();
            comboBox1.DataSource = names;
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
