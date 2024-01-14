using MNPZ.AdminPages;
using MNPZ.DAO;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using System.Web.UI.WebControls;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Window;

namespace MNPZ
{
    public partial class ExchangRates : Form
    {
        public Rates ratesBB { get; set; }
        public ExchangRates()
        {
            InitializeComponent();
            DisplayMoney();
            comboBox1.DataSource = Enum.GetValues(typeof(CurrencyNumber));
            comboBox2.DataSource = Enum.GetValues(typeof(CurrencyNumber));
            var user = (User)AppDomain.CurrentDomain.GetData("userTb");
            if (user == null)
            {
                LoginPage obj = new LoginPage();
                obj.Show();
                this.Hide();
            }
            else this.Text = "Администратор " + user.UserName;
            ratesBB = GetRates();
        }
        MoneyContext moneyContext = new MoneyContext();
        private void DisplayMoney()
        {
            var moneyVm = moneyContext.SelectAllMoney();
            dataGridView1.DataSource = moneyVm.ToArray();
        }
        
        public Rates GetRates()
        {
            string url = "https://belarusbank.by/api/kursExchange?city=Мозырь";
            WebRequest request = WebRequest.Create(url);
            request.Credentials = new NetworkCredential();
            WebResponse ws = request.GetResponse();
            string jsonString = string.Empty;
            using (System.IO.StreamReader sreader = new System.IO.StreamReader(ws.GetResponseStream()))
            {
                jsonString = sreader.ReadToEnd();
            }
                
            var exch = JsonConvert.DeserializeObject<List<Exchange>>(jsonString);
            var rates = new Rates();
            rates.exchang = exch;

            if(rates == null | rates.exchang == null)
            {
                return null;
            }
            else
            {
                return rates;
            }
        }

        private void label3_Click(object sender, EventArgs e)
        {
            SavedData obj = new SavedData();
            obj.Show();
            this.Hide();
        }

        private void label1_Click(object sender, EventArgs e)
        {
            AdminPage obj = new AdminPage();
            obj.Show();
            this.Hide();
        }
        private void button1_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Курсы валют удалены");
        }

        private void label5_Click(object sender, EventArgs e)
        {
            GiveCash obj = new GiveCash();
            obj.Show();
            this.Hide();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            var rates = GetRateInt();
            decimal check1 = 0, check2 = 0;
            if (textBox1.Text.Length < 1 || textBox2.Text.Length < 1)
                MessageBox.Show("Введены не все значения!");
            else
            {
                if (rates[0] > Convert.ToDecimal(textBox1.Text))
                {
                    check1 = 100 - Math.Round((Convert.ToDecimal(textBox1.Text) / rates[0]) * 100, 2);
                }
                else check1 = 100 - Math.Round((rates[0] / Convert.ToDecimal(textBox1.Text)) * 100, 2);
                if (rates[1] > Convert.ToDecimal(textBox2.Text))
                {
                    check2 = 100 - Math.Round((Convert.ToDecimal(textBox2.Text) / rates[1]) * 100, 2);
                }
                else check2 = 100 - Math.Round((rates[1] / Convert.ToDecimal(textBox2.Text)) * 100, 2);

                if (check1 > check2)
                {
                    if (check1 > 10)
                    {
                        var mes = "Разница введенных курсов равна: " + (check1) + "%; Вы уверены что хотите внести такие значения?";
                        var cap = "haha";
                        var result = MessageBox.Show(mes, cap,
                                MessageBoxButtons.YesNo,
                                MessageBoxIcon.Question);
                        if (result == DialogResult.Yes)
                        {
                            SaveRate();
                            DisplayMoney();
                        }
                    }
                    else
                    {
                        SaveRate();
                        DisplayMoney();
                    }
                }
                else
                {
                    if (check2 > 10)
                    {
                        var mes = "Разница введенных курсов равна: " + (check2) + "%; Вы уверены что хотите внести такие значения?";
                        var cap = "haha";
                        var result = MessageBox.Show(mes, cap,
                                MessageBoxButtons.YesNo,
                                MessageBoxIcon.Question);
                        if (result == DialogResult.Yes)
                        {

                            SaveRate();
                            DisplayMoney();
                        }
                    }
                    else
                    {
                        SaveRate();
                        DisplayMoney();
                    }
                }
            }

        }
        private void SaveRate()
        {
            CurrencyNumber combo1;
            Enum.TryParse<CurrencyNumber>(comboBox1.SelectedValue.ToString(), out combo1);
            CurrencyNumber combo2;
            Enum.TryParse<CurrencyNumber>(comboBox2.SelectedValue.ToString(), out combo2);

            var money = new Money
            {
                Cur_in = Convert.ToDecimal(textBox1.Text.Replace('.', ',')),
                Cur_out = Convert.ToDecimal(textBox2.Text.Replace('.', ',')),
                Cur_num_in = combo1,
                Cur_num_out = combo2
            };

            var insertMoney = moneyContext.InsertMoney(money);
            MessageBox.Show(insertMoney.Message);
        }
        private string GetRateText()
        {
            var result = "";
            if (ratesBB != null)
            {
                result = "In: 1.0; Out: 1.0";
                var first = ratesBB.exchang.First();
                if (first != null)
                {
                    CurrencyNumber combo1;
                    Enum.TryParse<CurrencyNumber>(comboBox1.SelectedValue.ToString(), out combo1);
                    CurrencyNumber combo2;
                    Enum.TryParse<CurrencyNumber>(comboBox2.SelectedValue.ToString(), out combo2);

                    if (combo1 == combo2)
                        return result;
                    else
                    {
                        if (combo1 == (CurrencyNumber.BYN))
                        {
                            if (combo2 == (CurrencyNumber.USD))
                                result = "In: " + first.USD_in + "; Out:" + first.USD_out + ";";
                            else if (combo2 == (CurrencyNumber.EUR))
                                result = "In: " + first.EUR_in + "; Out:" + first.EUR_out + ";";
                            else if (combo2 == (CurrencyNumber.RUB))
                                result = "In: " + first.RUB_in + "; Out:" + first.RUB_out + ";";
                        }
                        else if (combo1 == (CurrencyNumber.USD))
                        {
                            if (combo2 == (CurrencyNumber.BYN))
                                result = "In: " + first.USD_in + "; Out:" + first.USD_out + ";";
                            else if (combo2 == (CurrencyNumber.EUR))
                                result = "In: " + first.USD_EUR_in + "; Out:" + first.USD_EUR_out + ";";
                            else if (combo2 == (CurrencyNumber.RUB))
                                result = "In: " + first.USD_RUB_in + "; Out:" + first.USD_RUB_out + ";";
                        }
                        else if (combo1 == (CurrencyNumber.EUR))
                        {
                            if (combo2 == (CurrencyNumber.BYN))
                                result = "In: " + first.EUR_in + "; Out:" + first.EUR_out + ";";
                            else if (combo2 == (CurrencyNumber.USD))
                                result = "In: " + first.USD_EUR_in + "; Out:" + first.USD_EUR_out + ";";
                            else if (combo2 == (CurrencyNumber.RUB))
                                result = "In: " + first.RUB_EUR_in + "; Out:" + first.RUB_EUR_out + ";";
                        }
                        else if (combo1 == (CurrencyNumber.RUB))
                        {
                            if (combo2 == (CurrencyNumber.BYN))
                                result = "In: " + first.RUB_in + "; Out:" + first.RUB_out + ";";
                            else if (combo2 == (CurrencyNumber.EUR))
                                result = "In: " + first.RUB_EUR_in + "; Out:" + first.RUB_EUR_out + ";";
                            else if (combo2 == (CurrencyNumber.USD))
                                result = "In: " + first.USD_RUB_in + "; Out:" + first.USD_RUB_out + ";";
                        }
                    }
                }
            }
            return result;
        }
        private decimal[] GetRateInt()
        {
            var result = new decimal[2];
            result[0] = 1;
            result[1] = 1;

            var first = ratesBB.exchang.First();
            if (first != null)
            {
                CurrencyNumber combo1;
                Enum.TryParse<CurrencyNumber>(comboBox1.SelectedValue.ToString(), out combo1);
                CurrencyNumber combo2;
                Enum.TryParse<CurrencyNumber>(comboBox2.SelectedValue.ToString(), out combo2);

                if (combo1 == combo2)
                {
                    return result;
                }
                else
                {
                    if (combo1 == (CurrencyNumber.BYN))
                    {
                        if (combo2 == (CurrencyNumber.USD))
                        {
                            result[0] = Convert.ToDecimal(first.USD_in.Replace('.', ','));
                            result[1] = Convert.ToDecimal(first.USD_out.Replace('.', ','));
                        }
                        else if (combo2 == (CurrencyNumber.EUR))
                        {
                            result[0] = Convert.ToDecimal(first.EUR_in.Replace('.', ','));
                            result[1] = Convert.ToDecimal(first.EUR_out.Replace('.', ','));
                        }
                        else if (combo2 == (CurrencyNumber.RUB))
                        {
                            result[0] = Convert.ToDecimal(first.RUB_in.Replace('.', ','));
                            result[1] = Convert.ToDecimal(first.RUB_out.Replace('.', ','));
                        }
                    }
                    else if (combo1 == (CurrencyNumber.USD))
                    {
                        if (combo2 == (CurrencyNumber.BYN))
                        {
                            result[0] = Convert.ToDecimal(first.USD_in.Replace('.', ','));
                            result[1] = Convert.ToDecimal(first.USD_out.Replace('.', ','));
                        }
                        else if (combo2 == (CurrencyNumber.EUR))
                        {
                            result[0] = Convert.ToDecimal(first.USD_EUR_in.Replace('.', ','));
                            result[1] = Convert.ToDecimal(first.USD_EUR_out.Replace('.', ','));
                        }
                        else if (combo2 == (CurrencyNumber.RUB))
                        {
                            result[0] = Convert.ToDecimal(first.USD_RUB_in.Replace('.', ','));
                            result[1] = Convert.ToDecimal(first.USD_RUB_out.Replace('.', ','));
                        }
                    }
                    else if (combo1 == (CurrencyNumber.EUR))
                    {
                        if (combo2 == (CurrencyNumber.BYN))
                        {
                            result[0] = Convert.ToDecimal(first.EUR_in.Replace('.', ','));
                            result[1] = Convert.ToDecimal(first.EUR_out.Replace('.', ','));
                        }
                        else if (combo2 == (CurrencyNumber.USD))
                        {
                            result[0] = Convert.ToDecimal(first.USD_EUR_in.Replace('.', ','));
                            result[1] = Convert.ToDecimal(first.USD_EUR_out.Replace('.', ','));
                        }
                        else if (combo2 == (CurrencyNumber.RUB))
                        {
                            result[0] = Convert.ToDecimal(first.RUB_EUR_in.Replace('.', ','));
                            result[1] = Convert.ToDecimal(first.RUB_EUR_out.Replace('.', ','));
                        }
                    }
                    else if (combo1 == (CurrencyNumber.RUB))
                    {
                        if (combo2 == (CurrencyNumber.BYN))
                        {
                            result[0] = Convert.ToDecimal(first.RUB_in.Replace('.', ','));
                            result[1] = Convert.ToDecimal(first.RUB_out.Replace('.', ','));
                        }
                        else if (combo2 == (CurrencyNumber.EUR))
                        {
                            result[0] = Convert.ToDecimal(first.RUB_EUR_in.Replace('.', ','));
                            result[1] = Convert.ToDecimal(first.RUB_EUR_out.Replace('.', ','));
                        }
                        else if (combo2 == (CurrencyNumber.USD))
                        {
                            result[0] = Convert.ToDecimal(first.USD_RUB_in.Replace('.', ','));
                            result[1] = Convert.ToDecimal(first.USD_RUB_out.Replace('.', ','));
                        }
                    }
                }
            }
        return result;
        }
        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            rateBB.Text = GetRateText();
        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            rateBB.Text = GetRateText();
        }

        private void label12_Click(object sender, EventArgs e)
        {
            AppDomain.CurrentDomain.SetData("userTb", null);
            LoginPage obj = new LoginPage();
            obj.Show();
            this.Hide();
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }
    }
}
