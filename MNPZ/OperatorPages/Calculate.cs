using MNPZ.DAL.Models;
using MNPZ.DAL.Repositories;
using MNPZ.DAO;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace MNPZ
{
    public partial class Calculate : Form
    {
        User user = new User();
        readonly OperationRepository _operationRepository;
        List<string> comboSelIn = new List<string>();
        List<string> comboSelOut = new List<string>();
        IEnumerable<Rate> moneyList = new List<Rate>();
        RateRepository moneyContext = new RateRepository();
        private IEnumerable<BaseBalanceItem> balances;

        public Calculate()
        {
            _operationRepository = new OperationRepository();
            InitializeComponent();
            user = (User)AppDomain.CurrentDomain.GetData("User");
            if (user == null)
            {
                LoginPage obj = new LoginPage();
                obj.Show();
                this.Hide();
            }
            else this.Text = $"Оператор {user.UserName}";
            Clear();
            LoadOperationsDb();
            LoadExchangeDb();
            comboBox4.DataSource = comboSelIn;
            comboBox1.DataSource = comboSelOut;


        }

        private void button3_Click(object sender, EventArgs e)
        {
            decimal inputCost = 0, outputCost = 0, ostatok = 0;
            Currency curIn;
            Enum.TryParse<Currency>(comboBox4.SelectedValue.ToString(), out curIn);
            Currency curOut;
            Enum.TryParse<Currency>(comboBox1.SelectedValue.ToString(), out curOut);
            if (InputSel.Text.Length > 0)
                inputCost = Convert.ToDecimal(InputSel.Text);
            if (wannaCost.Text.Length > 0)
                outputCost = Convert.ToDecimal(wannaCost.Text);
            if (inputCost > 0)
            {
                if (outputCost > 0)
                {
                    var checkCur = moneyList.LastOrDefault(x => x.CurIn == curIn && x.CurOut == curOut);
                    if (curIn == curOut)
                    {
                        if (inputCost > outputCost)
                        {
                            ostatok = inputCost - outputCost;
                            SaveOperation(curIn, curOut, inputCost, outputCost, ostatok, checkCur.CurOutAmount);
                            ClearInput();
                        }
                        else
                        {
                            MessageBox.Show("Невозможно совершить обмен с меньшей суммы на большую!");
                        }
                    }
                    if (checkCur == null)
                    {
                        checkCur = moneyList.LastOrDefault(x => x.CurIn == curOut && x.CurOut == curIn);
                        if (checkCur == null)
                        {
                            MessageBox.Show("Невозможно совершить обмен без указаных курсов! Обратитесь к Администратору");
                        }
                        else
                        {
                            var valueIn = inputCost * checkCur.CurInAmount;
                            if (valueIn > outputCost)
                            {
                                MessageBox.Show("Неверно указанны суммы, проверьте значения!");
                            }
                            else
                            {
                                ostatok = outputCost - valueIn;
                                SaveOperation(curIn, curOut, inputCost, valueIn, ostatok, checkCur.CurOutAmount);
                                ClearInput();
                            }
                        }
                    }
                    else
                    {
                        var valueIn = outputCost * checkCur.CurOutAmount;
                        if (valueIn > inputCost)
                        {
                            MessageBox.Show("Неверно указанны суммы, проверьте значения!");
                        }
                        else
                        {
                            ostatok = inputCost - valueIn;
                            SaveOperation(curIn, curOut, inputCost, outputCost, ostatok, checkCur.CurOutAmount);
                            ClearInput();
                        }
                    }
                }
                if (ostatok > 0)
                {
                    freeMoney.Text = ostatok.ToString();
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            var byn = Convert.ToDecimal(OstBYN.Text);
            var eur = Convert.ToDecimal(OstEUR.Text);
            var usd = Convert.ToDecimal(OstUsd.Text);
            var rub = Convert.ToDecimal(OstRUB.Text);

            var balances = new List<BaseBalanceItem>()
            {
                new BaseBalanceItem()
                {
                    Balance = -byn,
                    Currency = Currency.BYN
                },
                new BaseBalanceItem()
                {
                    Balance = -usd,
                    Currency = Currency.USD
                } ,
                new BaseBalanceItem()
                {
                    Balance = -eur,
                    Currency = Currency.EUR
                } ,
                new BaseBalanceItem()
                {
                    Balance = -rub,
                    Currency = Currency.RUB
                }
            };

            _operationRepository.AddFundsToUserBalance(user.Id, balances);
            ClearInput();
        }
        private void LoadOperationsDb()
        {
            var balances = _operationRepository.GetUserBalances(user.Id);

            if (balances != null && balances.Any())
            {
                OstBYN.Text = balances.FirstOrDefault(x => x.Currency == Currency.BYN)?.Balance.ToString() ?? string.Empty;
                OstEUR.Text = balances.FirstOrDefault(x => x.Currency == Currency.EUR)?.Balance.ToString() ?? string.Empty;
                OstUsd.Text = balances.FirstOrDefault(x => x.Currency == Currency.USD)?.Balance.ToString() ?? string.Empty;
                OstRUB.Text = balances.FirstOrDefault(x => x.Currency == Currency.RUB)?.Balance.ToString() ?? string.Empty;
            }
        }
        private void NotCanExchange()
        {
            if (OstBYN.Text == "0")
                OstBYN.ForeColor = Color.Red;
            else
            {
                OstBYN.ForeColor = Color.Green;
                comboSelIn.Add("BYN");
                comboSelOut.Add("BYN");
            }
            if (OstRUB.Text == "0")
                OstRUB.ForeColor = Color.Red;
            else
            {
                OstRUB.ForeColor = Color.Green;
                comboSelIn.Add("RUB");
                comboSelOut.Add("RUB");
            }
            if (OstUsd.Text == "0")
                OstUsd.ForeColor = Color.Red;
            else
            {
                OstUsd.ForeColor = Color.Green;
                comboSelIn.Add("USD");
                comboSelOut.Add("USD");
            }
            if (OstEUR.Text == "0")
                OstEUR.ForeColor = Color.Red;
            else
            {
                OstEUR.ForeColor = Color.Green;
                comboSelIn.Add("EUR");
                comboSelOut.Add("EUR");
            }
        }
        private void Clear()
        {
            OstBYN.Text = "0";
            OstEUR.Text = "0";
            OstUsd.Text = "0";
            OstRUB.Text = "0";
        }
        private void ClearInput()
        {
            InputSel.Text = "0";
            wannaCost.Text = "0";
            freeMoney.Text = "0";
            Clear();
            LoadOperationsDb();
            LoadExchangeDb();
        }
        private void LoadExchangeDb()
        {
            moneyList = moneyContext.SelectAllRates();
            NotCanExchange();
        }
        private void SaveOperation(
            Currency curIn,
            Currency curOut,
            decimal costIn,
            decimal costOut,
            decimal remainder,
            decimal rate,
            bool isExch = true)
        {
            var op = new Operation()
            {
                Date = DateTime.Now,
                CurrencyOut = curOut,
                OutAmount = costOut,
                Remainder = remainder,
                ExchangeRate = rate,
                IsExchange = isExch,
                InAmount = costIn,
                CurrencyIn = curIn,
                UserId = user.Id

            };

            _operationRepository.InsertOperation(op);



        }

        private void InputSel_TextChanged(object sender, EventArgs e)
        {
            OutRate.Text = MakeMessage();
        }

        private void UsdInput_TextChanged(object sender, EventArgs e)
        {
            OutRate.Text = MakeMessage();
        }

        private void EurInput_TextChanged(object sender, EventArgs e)
        {
            OutRate.Text = MakeMessage();
        }

        private void RubInput_TextChanged(object sender, EventArgs e)
        {
            OutRate.Text = MakeMessage();
        }
        private string EnumName(Currency cur)
        {
            return Enum.GetName(typeof(Currency), cur);
        }
        private string MakeMessage()
        {
            string message = "";
            decimal inputCost = 0, outputCost = 0, ostatok = 0;

            if (!string.IsNullOrEmpty(comboBox4.SelectedValue?.ToString()) &&
                !string.IsNullOrEmpty(comboBox1.SelectedValue?.ToString()) &&
                !string.IsNullOrEmpty(InputSel.Text) &&
                !string.IsNullOrEmpty(wannaCost.Text))
            {
                Enum.TryParse<Currency>(comboBox4.SelectedValue.ToString(), out var curIn);
                Enum.TryParse<Currency>(comboBox1.SelectedValue.ToString(), out var curOut);

                if (InputSel.Text.Length > 0)
                    inputCost = Convert.ToDecimal(InputSel.Text);
                if (wannaCost.Text.Length > 0)
                    outputCost = Convert.ToDecimal(wannaCost.Text);

                if (inputCost > 0)
                {
                    if (outputCost > 0)
                    {
                        var checkCur = moneyList.LastOrDefault(x => x.CurIn == curIn && x.CurOut == curOut);
                        if (curIn == curOut)
                        {
                            if (inputCost > outputCost)
                            {
                                message = "Размен возможен";

                            }
                            else
                            {
                                MessageBox.Show("Невозможно совершить обмен с меньшей суммы на большую!");
                            }
                        }
                        if (checkCur == null)
                        {
                            checkCur = moneyList.LastOrDefault(x => x.CurIn == curOut && x.CurOut == curIn);
                            if (checkCur == null)
                            {
                                message = "Курсы для данной валюты не установлены!";
                            }
                            else
                            {
                                var valueIn = inputCost * checkCur.CurInAmount;
                                if (valueIn > outputCost)
                                {
                                    message = "Недостаточно средств для обмена!";
                                }
                                else
                                {
                                    ostatok = outputCost - valueIn;
                                    message = "Обмен возможен, сдача будет равна: " + ostatok.ToString();
                                }
                            }
                        }
                        else
                        {
                            var valueIn = outputCost * checkCur.CurOutAmount;
                            if (valueIn > inputCost)
                            {
                                message = "Недостаточно средств для обмена!";
                            }
                            else
                            {
                                ostatok = inputCost - valueIn;
                                message = "Обмен возможен, сдача будет равна: " + ostatok.ToString();
                            }
                        }
                    }
                    if (ostatok > 0)
                    {
                        freeMoney.Text = ostatok.ToString();
                    }
                }
            }

            return message;
        }
        private void DisableInput()
        {
            Currency curIn;
            Enum.TryParse<Currency>(comboBox4.SelectedValue.ToString(), out curIn);
            var curCheckUsd = moneyList.LastOrDefault(x => x.CurIn == curIn && x.CurOut == Currency.USD);

            var curCheckEur = moneyList.LastOrDefault(x => x.CurIn == curIn && x.CurOut == Currency.EUR);
            var curCheckRub = moneyList.LastOrDefault(x => x.CurIn == curIn && x.CurOut == Currency.RUB);
        }

        private void label7_Click(object sender, EventArgs e)
        {
            AppDomain.CurrentDomain.SetData("User", null);
            LoginPage obj = new LoginPage();
            obj.Show();
            this.Hide();
        }
    }
}
