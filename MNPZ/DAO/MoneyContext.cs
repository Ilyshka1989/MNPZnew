using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MNPZ.DAO
{
    public class MoneyContext: MainDataContext
    {
        SqlConnection con = new SqlConnection(connectDb);
        #region Получить список курсов
        public List<MoneyVm> SelectAllMoney()
        {
            var result = new List<MoneyVm>();
            var query = "select * from Money";

            con.Open();

            SqlDataAdapter sda = new SqlDataAdapter(query, con);
            SqlCommandBuilder builder = new SqlCommandBuilder(sda);
            var ds = new DataSet();
            sda.Fill(ds);
            con.Close();

            result = ds.Tables[0].AsEnumerable()
                .Select(x => new MoneyVm
                {
                    Сумма_In = Convert.ToDecimal(x.Field<double>("Cur_in")),
                    Сумма_Out = Convert.ToDecimal(x.Field<double>("Cur_out")),
                    Валюта_In = Enum.GetName(typeof(CurrencyNumber), x.Field<CurrencyNumber>("Cur_num_in")),
                    Валюта_Out = Enum.GetName(typeof(CurrencyNumber), x.Field<CurrencyNumber>("Cur_num_out")),
                }).ToList();

            return result;
        }public List<Money> SelectAllMoneyNoVm()
        {
            var result = new List<Money>();
            var query = "select * from Money";

            con.Open();

            SqlDataAdapter sda = new SqlDataAdapter(query, con);
            SqlCommandBuilder builder = new SqlCommandBuilder(sda);
            var ds = new DataSet();
            sda.Fill(ds);
            con.Close();

            result = ds.Tables[0].AsEnumerable()
                .Select(x => new Money
                {
                    Cur_in = Convert.ToDecimal(x.Field<double>("Cur_in")),
                    Cur_out = Convert.ToDecimal(x.Field<double>("Cur_out")),
                    Cur_num_in = x.Field<CurrencyNumber>("Cur_num_in"),
                    Cur_num_out =  x.Field<CurrencyNumber>("Cur_num_out"),
                }).ToList();

            return result;
        }
        #endregion
        #region Получить курс по валютам
        public Money SelectMoneyByCurrency1(CurrencyNumber curIn, CurrencyNumber curOut)
        {
            var result = new Money();
            var query = "select * from Money";
            query += " where Cur_num_in = " + (int)curIn;
            query += " AND Cur_num_out = " + (int)curOut;

            con.Open();

            SqlDataAdapter sda = new SqlDataAdapter(query, con);
            SqlCommandBuilder builder = new SqlCommandBuilder(sda);
            var ds = new DataSet();
            sda.Fill(ds);
            con.Close();

            result = ds.Tables[0].AsEnumerable()
                .Select(x => new Money
                {
                    Cur_in = Convert.ToDecimal(x.Field<double>("Cur_in")),
                    Cur_out = Convert.ToDecimal(x.Field<double>("Cur_out")),
                    Cur_num_in = x.Field<CurrencyNumber>("Cur_num_in"),
                    Cur_num_out = x.Field<CurrencyNumber>("Cur_num_out"),
                }).ToList().FirstOrDefault();

            return result;
        }
        public Money SelectMoneyByCurrency2(CurrencyNumber curIn, CurrencyNumber curOut)
        {
            var result = new Money();
            var query = "select * from Money";
            query += " where Cur_num_in = " + (int)curOut;
            query += " and Cur_num_out = " + (int)curIn;

            con.Open();

            SqlDataAdapter sda = new SqlDataAdapter(query, con);
            SqlCommandBuilder builder = new SqlCommandBuilder(sda);
            var ds = new DataSet();
            sda.Fill(ds);
            con.Close();

            result = ds.Tables[0].AsEnumerable()
                .Select(x => new Money
                {
                    Cur_in = Convert.ToDecimal(x.Field<double>("Cur_in")),
                    Cur_out = Convert.ToDecimal(x.Field<double>("Cur_out")),
                    Cur_num_in = x.Field<CurrencyNumber>("Cur_num_in"),
                    Cur_num_out = x.Field<CurrencyNumber>("Cur_num_out"),
                }).ToList().FirstOrDefault();

            return result;
        }
        public Money SelectMoneyByCurrency(CurrencyNumber curIn, CurrencyNumber curOut)
        {
            var result = new Money();
            result = SelectMoneyByCurrency1(curIn,curOut);
            if (result == null)
                result = SelectMoneyByCurrency2(curIn,curOut);

            return result;
        }
        #endregion
        #region Внести в базу курсы
        public SqlInfo InsertMoney(Money money)
        {
            var result = new SqlInfo();
            result.IsError = false;

            var checkMoney = SelectMoneyByCurrency(money.Cur_num_in, money.Cur_num_out.Value);
            if (checkMoney != null)
            {
                return UpdateMoney(money,checkMoney);
            }

            string query = "insert into Money(Cur_in, Cur_out, Cur_num_in, Cur_num_out) values(@N, @L, @P, @O)";

            con.Open();
            SqlCommand cmd = new SqlCommand(query, con);
            cmd.Parameters.AddWithValue("@N", money.Cur_in);
            cmd.Parameters.AddWithValue("@L", money.Cur_out);
            cmd.Parameters.AddWithValue("@P", money.Cur_num_in);
            cmd.Parameters.AddWithValue("@O", money.Cur_num_out);

            cmd.ExecuteNonQuery();
            con.Close();
            result.Message = "Данные изменены!";

            return result;
        }
        #endregion
        #region Обновить базу
        public SqlInfo UpdateMoney(Money money,Money oldMoney)
        {
            var result = new SqlInfo();
            result.IsError = false;

            string query = "update Money set Cur_in=@Q, Cur_out=@W WHERE  Cur_num_in = @E AND Cur_num_out=@R";
            con.Open();
            SqlCommand cmd = new SqlCommand(query, con);
            cmd.Parameters.AddWithValue("@Q", money.Cur_in);
            cmd.Parameters.AddWithValue("@W", money.Cur_out);
            cmd.Parameters.AddWithValue("@E", oldMoney.Cur_num_in);
            cmd.Parameters.AddWithValue("@R", oldMoney.Cur_num_out);
            cmd.ExecuteNonQuery();
            con.Close();
            
            result.Message = "Данные изменены!";

            return result;
        }
        #endregion
        #region Удалить курсы
        public SqlInfo DeleteMoney(CurrencyNumber curIn, CurrencyNumber curOut)
        {
            var result = new SqlInfo();
            result.IsError = false;

            var checkMoney = SelectMoneyByCurrency(curIn,curOut);
            if (checkMoney != null)
            {
                result.IsError = true;
                result.Message = "Таких курсов не существует!";
            }

            string query = "delete from Money where Cur_in="+ checkMoney.Cur_in+" AND Cur_out=" + checkMoney.Cur_out;
            con.Open();
            SqlCommand cmd = new SqlCommand(query, con);

            cmd.ExecuteNonQuery();
            con.Close();
            result.Message = "Пользователь удалён!";

            return result;
        }
        #endregion
    }
}
