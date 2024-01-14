using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MNPZ
{
    public class Rates
    {
        public IList<Exchange> exchang;
    }
    public class Exchange
    {
        public string USD_in { get; set; }
        public string USD_out { get; set; }
        public string EUR_in { get; set; }
        public string EUR_out { get; set; }
        public string RUB_in { get; set; }
        public string RUB_out { get; set; }
        public string GBP_in { get; set; }
        public string GBP_out { get; set; }
        public string CAD_in { get; set; }
        public string CAD_out { get; set; }
        public string PLN_in { get; set; }
        public string PLN_out { get; set; }
        public string SEK_in { get; set; }
        public string SEK_out { get; set; }
        public string CHF_in { get; set; }
        public string CHF_out { get; set; }
        public string USD_EUR_in { get; set; }
        public string USD_EUR_out { get; set; }
        public string USD_RUB_in { get; set; }
        public string USD_RUB_out { get; set; }
        public string RUB_EUR_in { get; set; }
        public string RUB_EUR_out { get; set; }
        public string JPY_in { get; set; }
        public string JPY_out { get; set; }
        public string CNY_in { get; set; }
        public string CNY_out { get; set; }
        public string CNY_EUR_in { get; set; }
        public string CNY_EUR_out { get; set; }
        public string CNY_USD_in { get; set; }
        public string CNY_USD_out { get; set; }
        public string CNY_RUB_in { get; set; }
        public string CNY_RUB_out { get; set; }
        public string CZK_in { get; set; }
        public string CZK_out { get; set; }
        public string NOK_in { get; set; }
        public string NOK_out { get; set; }
        public string filial_id { get; set; }
        public string sap_id { get; set; }
        public string info_worktime { get; set; }
        public string street_type { get; set; }
        public string street { get; set; }
        public string filials_text { get; set; }
        public string home_number { get; set; }
        public string name { get; set; }
        public string name_type { get; set; }
    }
    public enum CurrencyNumber
    {
        BYN = 0,
        USD = 1,
        EUR = 2,
        RUB = 3
    }
    public class User
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public string Login { get; set; }
        public string Password { get; set; }
        public bool IsOperator { get; set; } = false;
    }
    public class Operation
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string UserName { get; set; }
        public DateTime DateOperation { get; set; }
        public decimal Cost { get; set; }
        public CurrencyNumber Cur_in_num { get; set; }
        public CurrencyNumber? Cur_out_num { get; set; }
        public bool IsExchange { get; set; }
    }
    public class OperationVm
    {
        public string Имя_пользователя { get; set; }
        public DateTime Дата { get; set; }
        public decimal Сумма { get; set; }
        public string Валюта_In { get; set; }
        public string Валюта_Out { get; set; }
        public bool Был_Ли_Обмен { get; set; }
    }
    public class Money
    {
        public decimal Cur_in { get; set; }
        public decimal Cur_out { get; set; }
        public CurrencyNumber Cur_num_in { get; set; }
        public CurrencyNumber? Cur_num_out { get; set; }
    }
    public class MoneyVm
    {
        public decimal Сумма_In { get; set; }
        public decimal Сумма_Out { get; set; }
        public string Валюта_In { get; set; }
        public string Валюта_Out { get; set; }
    }
    public class SqlInfo
    {
        public bool IsError { get; set; }
        public string Message { get; set; }
    }
}
