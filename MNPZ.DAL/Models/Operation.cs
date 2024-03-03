using System;

namespace MNPZ.DAL.Models
{
    public class Operation
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public DateTime Date { get; set; }

        public decimal Remainder { get; set; }
        public decimal InAmount { get; set; }
        public decimal OutAmount { get; set; }

        public Currency CurrencyIn { get; set; }
        public Currency CurrencyOut { get; set; }

        public decimal ExchangeRate { get; set; }
        public bool IsExchange { get; set; }
    }
}
