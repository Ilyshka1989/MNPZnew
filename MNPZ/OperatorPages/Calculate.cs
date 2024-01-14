
using MNPZ.DAO;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace MNPZ
{
    public partial class Calculate : Form
    {

        User user = new User();
        List<Operation> operations = new List<Operation>();
        OperationContext operationContext = new OperationContext();
        List<string> comboSelIn = new List<string>();
        List<string> comboSelOut = new List<string>();
        List<Money> moneyList = new List<Money>();
        MoneyContext moneyContext = new MoneyContext();
        public Calculate()
        {
            InitializeComponent();
            user = (User)AppDomain.CurrentDomain.GetData("userTb");
            if (user == null)
            {
                LoginPage obj = new LoginPage();
                obj.Show();
                this.Hide();
            }
            else this.Text = "Оператор " + user.UserName;
            Clear();
            LoadOperationsDb();
            LoadExchangeDb();
            comboBox4.DataSource = comboSelIn;
            comboBox1.DataSource = comboSelOut;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            decimal inputCost = 0, outputCost = 0, ostatok = 0;
            CurrencyNumber curIn;
            Enum.TryParse<CurrencyNumber>(comboBox4.SelectedValue.ToString(), out curIn);
            CurrencyNumber curOut;
            Enum.TryParse<CurrencyNumber>(comboBox1.SelectedValue.ToString(), out curOut);
            if(InputSel.Text.Length > 0)
                inputCost = Convert.ToDecimal(InputSel.Text);
            if (wannaCost.Text.Length > 0)
                outputCost = Convert.ToDecimal(wannaCost.Text);
            if (inputCost > 0)
            {
                if (outputCost > 0)
                {
                    var checkCur = moneyList.LastOrDefault(x => x.Cur_num_in == curIn && x.Cur_num_out == curOut);
                    if (curIn == curOut)
                    {
                        if (inputCost > outputCost)
                        {
                            ostatok = inputCost - outputCost;
                            SaveOperation(curIn, curOut, inputCost, outputCost);
                            ClearInput();
                        }
                        else
                        {
                            MessageBox.Show("Невозможно совершить обмен с меньшей суммы на большую!");
                        }
                    }
                    if (checkCur == null)
                    {
                        checkCur = moneyList.LastOrDefault(x => x.Cur_num_in == curOut && x.Cur_num_out == curIn);
                        if (checkCur == null)
                        {
                            MessageBox.Show("Невозможно совершить обмен без указаных курсов! Обратитесь к Администратору");
                        }
                        else
                        {
                            var valueIn = inputCost * checkCur.Cur_in;
                            if (valueIn > outputCost)
                            {
                                MessageBox.Show("Неверно указанны суммы, проверьте значения!");
                            }
                            else
                            {
                                ostatok = outputCost - valueIn;
                                SaveOperation(curIn, curOut,inputCost,valueIn);
                                ClearInput();
                            }
                        }
                        }
                    else
                    {
                        var valueIn = outputCost * checkCur.Cur_out;
                        if (valueIn > inputCost)
                        {
                            MessageBox.Show("Неверно указанны суммы, проверьте значения!");
                        }
                        else
                        {
                            ostatok = inputCost - valueIn;
                            SaveOperation(curIn, curOut, valueIn, outputCost);
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

            if (byn > 0)
                SaveOperation(CurrencyNumber.BYN, null, -byn, null, isExch: false);
            if (eur > 0)
                SaveOperation(CurrencyNumber.EUR, null, -eur, null, isExch: false);
            if (usd > 0)
                SaveOperation(CurrencyNumber.USD, null, -usd, null, isExch: false);
            if (rub > 0)
                SaveOperation(CurrencyNumber.RUB, null, -rub, null, isExch: false);
            ClearInput();
        }
        private void LoadOperationsDb()
        {
            operations = operationContext.SelectAllOperations(user.Id); 
            if (operations.Count != 0)
            {
                var opers = operations.Where(x => x.Cur_in_num == CurrencyNumber.BYN);
                OstBYN.Text = (operations.Where(x => x.Cur_in_num == CurrencyNumber.BYN).Sum(x => x.Cost)).ToString();
                OstEUR.Text = (operations.Where(x => x.Cur_in_num == CurrencyNumber.EUR).Sum(x => x.Cost)).ToString();
                OstUsd.Text = (operations.Where(x => x.Cur_in_num == CurrencyNumber.USD).Sum(x => x.Cost)).ToString();
                OstRUB.Text = (operations.Where(x => x.Cur_in_num == CurrencyNumber.RUB).Sum(x => x.Cost)).ToString();
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
            moneyList = moneyContext.SelectAllMoneyNoVm();
            NotCanExchange();
        }
        private void SaveOperation(
            CurrencyNumber curIn,
            CurrencyNumber? curOut,
            decimal costIn,
            decimal? costOut,
            bool isExch = true)
        {
            var op = new Operation();
            var opEx = new OperationEx();

            op.UserId = user.Id;
            op.DateOperation = DateTime.Now;
            op.Cur_in_num = curIn;
            op.Cur_out_num = curOut;
            op.Cost = costIn;
            op.IsExchange = isExch;
            opEx.SaveOperation(op);
            if (costOut.HasValue)
            {
                op.UserId = user.Id;
                op.DateOperation = DateTime.Now;

                if (curOut.HasValue)
                op.Cur_in_num = curOut.Value;

                op.Cur_out_num = curIn;
                op.Cost = -costOut.Value;
                op.IsExchange = isExch;
                opEx.SaveOperation(op);
            }
            
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
        private string EnumName(CurrencyNumber cur)
        {
            return Enum.GetName(typeof(CurrencyNumber), cur);
        }
        private string MakeMessage()
        {
            string message = "";
            decimal inputCost = 0, outputCost = 0, ostatok = 0;
            CurrencyNumber curIn;
            Enum.TryParse<CurrencyNumber>(comboBox4.SelectedValue.ToString(), out curIn);
            CurrencyNumber curOut;
            Enum.TryParse<CurrencyNumber>(comboBox1.SelectedValue.ToString(), out curOut);
            if (InputSel.Text.Length > 0)
                inputCost = Convert.ToDecimal(InputSel.Text);
            if (wannaCost.Text.Length > 0)
                outputCost = Convert.ToDecimal(wannaCost.Text);
            if (inputCost > 0)
            {
                if (outputCost > 0)
                {
                    var checkCur = moneyList.LastOrDefault(x => x.Cur_num_in == curIn && x.Cur_num_out == curOut);
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
                        checkCur = moneyList.LastOrDefault(x => x.Cur_num_in == curOut && x.Cur_num_out == curIn);
                        if (checkCur == null)
                        {
                            message = "Курсы для данной валюты не установлены!";
                        }
                        else
                        {
                            var valueIn = inputCost * checkCur.Cur_in;
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
                        var valueIn = outputCost * checkCur.Cur_out;
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
                return message;
        }
        private void DisableInput()
        {
            CurrencyNumber curIn;
            Enum.TryParse<CurrencyNumber>(comboBox4.SelectedValue.ToString(), out curIn);
            var curCheckUsd = moneyList.LastOrDefault(x => x.Cur_num_in == curIn && x.Cur_num_out == CurrencyNumber.USD);

            var curCheckEur = moneyList.LastOrDefault(x => x.Cur_num_in == curIn && x.Cur_num_out == CurrencyNumber.EUR);
            var curCheckRub = moneyList.LastOrDefault(x => x.Cur_num_in == curIn && x.Cur_num_out == CurrencyNumber.RUB);
        }

        private void label7_Click(object sender, EventArgs e)
        {
            AppDomain.CurrentDomain.SetData("userTb", null);
            LoginPage obj = new LoginPage();
            obj.Show();
            this.Hide();
        }
    }
}
