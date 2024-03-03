using MNPZ.AdminPages;
using MNPZ.DAL.Models;
using MNPZ.DAL.Repositories;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MNPZ
{
    public partial class ExchangRates : Form
    {
        public IEnumerable<Exchange> ratesBB { get; set; }
        private readonly RateRepository _rateRepository;

        public ExchangRates()
        {
            _rateRepository = new RateRepository();
            InitializeComponent();
            DisplayRates();
            comboBox1.DataSource = Enum.GetValues(typeof(Currency));
            comboBox2.DataSource = Enum.GetValues(typeof(Currency));
            var user = (User)AppDomain.CurrentDomain.GetData("User");
            if (user == null)
            {
                LoginPage obj = new LoginPage();
                obj.Show();
                this.Hide();
            }
            else this.Text = "Администратор " + user.UserName;

        }

        private void DisplayRates()
        {
            var moneyVm = _rateRepository.SelectAllRates();
            dataGridView1.DataSource = moneyVm.ToArray();
        }


        public async Task<IEnumerable<Exchange>> GetRates()
        {
            string url = "https://belarusbank.by/api/kursExchange?city=Мозырь";
            WebRequest request = WebRequest.Create(url);
            request.Credentials = new NetworkCredential();
            WebResponse ws = await request.GetResponseAsync();
            string jsonString = string.Empty;
            using (System.IO.StreamReader sreader = new System.IO.StreamReader(ws.GetResponseStream()))
            {
                jsonString = sreader.ReadToEnd();
            }

            var exch = JsonConvert.DeserializeObject<List<Exchange>>(jsonString);
            var rates = exch;

            return rates;
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

            if (textBox1.Text.Length < 1 || textBox2.Text.Length < 1)
            {

                MessageBox.Show("Введены не все значения!");
            }
            else if (!decimal.TryParse(textBox1.Text.Replace(',', '.'), out var inAmount) ||
                     !decimal.TryParse(textBox2.Text.Replace(',', '.'), out var outAmount) ||
                !Enum.TryParse<Currency>(comboBox1.SelectedValue.ToString(), out var combo1) ||
                !Enum.TryParse<Currency>(comboBox2.SelectedValue.ToString(), out var combo2))
            {
                MessageBox.Show("Неверный ввод");
            }
            else
            {
                decimal check1 = 0;
                if (rates[0] > inAmount)
                {
                    check1 = 100 - Math.Round((inAmount / rates[0]) * 100, 2);
                }
                else check1 = 100 - Math.Round((rates[0] / inAmount) * 100, 2);

                decimal check2 = 0;
                if (rates[1] > outAmount)
                {
                    check2 = 100 - Math.Round((outAmount / rates[1]) * 100, 2);
                }
                else
                {
                    check2 = 100 - Math.Round((rates[1] / outAmount) * 100, 2);
                }

                bool ratesShouldBeSaved = true;

                if (check1 > check2)
                {
                    if (check1 > 10)
                    {
                        var mes = "Разница введенных курсов равна: " + (check1) +
                                  "%; Вы уверены что хотите внести такие значения?";
                        var cap = "haha";
                        var result = MessageBox.Show(mes, cap,
                            MessageBoxButtons.YesNo,
                            MessageBoxIcon.Question);
                        if (result == DialogResult.No)
                        {
                            ratesShouldBeSaved = false;
                        }
                    }
                }
                else
                {
                    if (check2 > 10)
                    {
                        var mes = "Разница введенных курсов равна: " + (check2) +
                                  "%; Вы уверены что хотите внести такие значения?";
                        var cap = "haha";
                        var result = MessageBox.Show(mes, cap,
                            MessageBoxButtons.YesNo,
                            MessageBoxIcon.Question);
                        if (result == DialogResult.No)
                        {
                            ratesShouldBeSaved = false;
                        }
                    }
                }
                if (ratesShouldBeSaved)
                {
                    SaveRate(inAmount, outAmount, combo1, combo2);
                    DisplayRates();
                }

            }

        }

        private void SaveRate(decimal inAmount, decimal outAmount, Currency curIn, Currency curOut)
        {
            var money = new Rate
            {
                CurInAmount = inAmount,
                CurOutAmount = outAmount,
                CurIn = curIn,
                CurOut = curOut
            };

            var insertMoney = _rateRepository.InsertOrUpdateRate(money);
            MessageBox.Show(insertMoney.Message);
        }

        private string GetRateText()
        {
            var result = "";
            if (ratesBB != null)
            {
                result = "In: 1.0; Out: 1.0";
                var first = ratesBB.First();
                if (first != null)
                {
                    Currency combo1;
                    Enum.TryParse<Currency>(comboBox1.SelectedValue.ToString(), out combo1);
                    Currency combo2;
                    Enum.TryParse<Currency>(comboBox2.SelectedValue.ToString(), out combo2);

                    if (combo1 == combo2)
                        return result;
                    else
                    {
                        if (combo1 == (Currency.BYN))
                        {
                            if (combo2 == (Currency.USD))
                                result = "In: " + first.USD_in + "; Out:" + first.USD_out + ";";
                            else if (combo2 == (Currency.EUR))
                                result = "In: " + first.EUR_in + "; Out:" + first.EUR_out + ";";
                            else if (combo2 == (Currency.RUB))
                                result = "In: " + first.RUB_in + "; Out:" + first.RUB_out + ";";
                        }
                        else if (combo1 == (Currency.USD))
                        {
                            if (combo2 == (Currency.BYN))
                                result = "In: " + first.USD_in + "; Out:" + first.USD_out + ";";
                            else if (combo2 == (Currency.EUR))
                                result = "In: " + first.USD_EUR_in + "; Out:" + first.USD_EUR_out + ";";
                            else if (combo2 == (Currency.RUB))
                                result = "In: " + first.USD_RUB_in + "; Out:" + first.USD_RUB_out + ";";
                        }
                        else if (combo1 == (Currency.EUR))
                        {
                            if (combo2 == (Currency.BYN))
                                result = "In: " + first.EUR_in + "; Out:" + first.EUR_out + ";";
                            else if (combo2 == (Currency.USD))
                                result = "In: " + first.USD_EUR_in + "; Out:" + first.USD_EUR_out + ";";
                            else if (combo2 == (Currency.RUB))
                                result = "In: " + first.RUB_EUR_in + "; Out:" + first.RUB_EUR_out + ";";
                        }
                        else if (combo1 == (Currency.RUB))
                        {
                            if (combo2 == (Currency.BYN))
                                result = "In: " + first.RUB_in + "; Out:" + first.RUB_out + ";";
                            else if (combo2 == (Currency.EUR))
                                result = "In: " + first.RUB_EUR_in + "; Out:" + first.RUB_EUR_out + ";";
                            else if (combo2 == (Currency.USD))
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

            var first = ratesBB.First();
            if (first != null)
            {
                Currency combo1;
                Enum.TryParse(comboBox1.SelectedValue.ToString(), out combo1);
                Currency combo2;
                Enum.TryParse(comboBox2.SelectedValue.ToString(), out combo2);

                if (combo1 == combo2)
                {
                    return result;
                }
                else
                {
                    if (combo1 == (Currency.BYN))
                    {
                        if (combo2 == (Currency.USD))
                        {
                            result[0] = Convert.ToDecimal(first.USD_in.Replace('.', ','));
                            result[1] = Convert.ToDecimal(first.USD_out.Replace('.', ','));
                        }
                        else if (combo2 == (Currency.EUR))
                        {
                            result[0] = Convert.ToDecimal(first.EUR_in);
                            result[1] = Convert.ToDecimal(first.EUR_out);
                        }
                        else if (combo2 == (Currency.RUB))
                        {
                            result[0] = Convert.ToDecimal(first.RUB_in);
                            result[1] = Convert.ToDecimal(first.RUB_out);
                        }
                    }
                    else if (combo1 == (Currency.USD))
                    {
                        if (combo2 == (Currency.BYN))
                        {
                            result[0] = Convert.ToDecimal(first.USD_in.Replace('.', ','));
                            result[1] = Convert.ToDecimal(first.USD_out.Replace('.', ','));
                        }
                        else if (combo2 == (Currency.EUR))
                        {
                            result[0] = Convert.ToDecimal(first.USD_EUR_in);
                            result[1] = Convert.ToDecimal(first.USD_EUR_out);
                        }
                        else if (combo2 == (Currency.RUB))
                        {
                            result[0] = Convert.ToDecimal(first.USD_RUB_in.Replace('.', ','));
                            result[1] = Convert.ToDecimal(first.USD_RUB_out.Replace('.', ','));
                        }
                    }
                    else if (combo1 == (Currency.EUR))
                    {
                        if (combo2 == (Currency.BYN))
                        {
                            result[0] = Convert.ToDecimal(first.EUR_in);
                            result[1] = Convert.ToDecimal(first.EUR_out);
                        }
                        else if (combo2 == (Currency.USD))
                        {
                            result[0] = Convert.ToDecimal(first.USD_EUR_in);
                            result[1] = Convert.ToDecimal(first.USD_EUR_out);
                        }
                        else if (combo2 == (Currency.RUB))
                        {
                            result[0] = Convert.ToDecimal(first.RUB_EUR_in);
                            result[1] = Convert.ToDecimal(first.RUB_EUR_out);
                        }
                    }
                    else if (combo1 == (Currency.RUB))
                    {
                        if (combo2 == (Currency.BYN))
                        {
                            result[0] = Convert.ToDecimal(first.RUB_in);
                            result[1] = Convert.ToDecimal(first.RUB_out);
                        }
                        else if (combo2 == (Currency.EUR))
                        {
                            result[0] = Convert.ToDecimal(first.RUB_EUR_in);
                            result[1] = Convert.ToDecimal(first.RUB_EUR_out);
                        }
                        else if (combo2 == (Currency.USD))
                        {
                            result[0] = Convert.ToDecimal(first.USD_RUB_in);
                            result[1] = Convert.ToDecimal(first.USD_RUB_out);
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
            AppDomain.CurrentDomain.SetData("User", null);
            LoginPage obj = new LoginPage();
            obj.Show();
            this.Hide();
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        protected override async void OnLoad(EventArgs e)
        {
            ratesBB = await GetRates();
        }
    }
}
