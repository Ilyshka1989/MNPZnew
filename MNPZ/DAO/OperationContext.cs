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
    public class OperationContext: MainDataContext
    {
        SqlConnection con = new SqlConnection(connectDb);
        #region Получить список операций
        
        public List<OperationVm> SelectAllOperations(DateTime date)
        {
            var contextUser = new UserContext();
            var users = contextUser.SelectAllUsers(true);

            var result = new List<OperationVm>();
            var query = "select * from Operations";

            var from = new DateTime(date.Year, date.Month, date.Day, 0, 0, 0, 0);
            var to = from.AddDays(1);

            query += " where DateOperation > '" + from.ToString() + "' AND DateOperation < '" + from.ToString() + "'";
            con.Open();

            SqlDataAdapter sda = new SqlDataAdapter(query, con);
            SqlCommandBuilder builder = new SqlCommandBuilder(sda);
            var ds = new DataSet();
            sda.Fill(ds);
            con.Close();

            result = ds.Tables[0].AsEnumerable()
                .Select(x => new OperationVm
                {
                    Имя_пользователя = users.First(u => u.Id == x.Field<int>("UserId")).UserName,
                    Дата = x.Field<DateTime>("DateOperation"),
                    Сумма = Convert.ToDecimal(x.Field<double>("Cost")),
                    Валюта_In = Enum.GetName(typeof(CurrencyNumber), x.Field<CurrencyNumber>("Cur_in_num")),
                    Валюта_Out = (x.Field<CurrencyNumber?>("Cur_out_num") == null ? "" : Enum.GetName(typeof(CurrencyNumber), x.Field<CurrencyNumber>("Cur_out_num"))),
                    Был_Ли_Обмен = x.Field<bool>("IsExchange")
                }).ToList();

            return result;
        }
        public List<OperationVm> SelectAllOperations()
        {
            var contextUser = new UserContext();
            var users = contextUser.SelectAllUsers(true);

            var result = new List<OperationVm>();
            var query = "select * from Operations";
            con.Open();

            SqlDataAdapter sda = new SqlDataAdapter(query, con);
            SqlCommandBuilder builder = new SqlCommandBuilder(sda);
            var ds = new DataSet();
            sda.Fill(ds);
            con.Close();

            result = ds.Tables[0].AsEnumerable()
                .Select(x => new OperationVm
                {
                    Имя_пользователя = users.First(u => u.Id == x.Field<int>("UserId")).UserName,
                    Дата = x.Field<DateTime>("DateOperation"),
                    Сумма = Convert.ToDecimal(x.Field<double>("Cost")),
                    Валюта_In = Enum.GetName(typeof(CurrencyNumber), x.Field<CurrencyNumber>("Cur_in_num")),
                    Валюта_Out = (x.Field<CurrencyNumber?>("Cur_out_num") == null ? "" : Enum.GetName(typeof(CurrencyNumber), x.Field<CurrencyNumber>("Cur_out_num"))),
                    Был_Ли_Обмен = x.Field<bool>("IsExchange")
                }).ToList();

            return result;
        }
         
        public List<Operation> SelectAllOperations(int userId)
        {
            var contextUser = new UserContext();
            
            var result = new List<Operation>();
            var query = "select * from Operations where UserId =" + userId;
            con.Open();

            SqlDataAdapter sda = new SqlDataAdapter(query, con);
            SqlCommandBuilder builder = new SqlCommandBuilder(sda);
            var ds = new DataSet();
            sda.Fill(ds);
            con.Close();

            result = ds.Tables[0].AsEnumerable()
                .Select(x => new Operation
                {
                    Id = x.Field<int>("Id"),
                    UserId = x.Field<int>("UserId"),
                    DateOperation = x.Field<DateTime>("DateOperation"),
                    Cost = Convert.ToDecimal((x.Field<double>("Cost")).ToString().Replace('.', ',')),
                    Cur_in_num = (CurrencyNumber)x.Field<int>("Cur_in_num"),
                    Cur_out_num = (CurrencyNumber?)x.Field<int?>("Cur_out_num"),
                    IsExchange = x.Field<bool>("IsExchange")
                }).ToList();

            return result;
        }
        #endregion
        #region Внести в базу операции
        public SqlInfo InsertOperation(Operation op)
        {
            var result = new SqlInfo();
            result.IsError = false;
            var query = "";
            if (op.IsExchange)
            {
                query = "insert into Operations(UserId, DateOperation, Cost, Cur_in_num, Cur_out_num, IsExchange)";
                query += "values(@userId, @d, @c, @in, @out, @is)";
            }
            else
            {
                query = "insert into Operations(UserId, DateOperation, Cost, Cur_in_num, IsExchange)";
                query += "values(@userId, @d, @c, @in, @is)";
            }

            con.Open();
            SqlCommand cmd = new SqlCommand(query, con);
            cmd.Parameters.AddWithValue("@userId", op.UserId);
            cmd.Parameters.AddWithValue("@d", op.DateOperation);
            cmd.Parameters.AddWithValue("@c", op.Cost);
            cmd.Parameters.AddWithValue("@in", op.Cur_in_num);
            if (op.IsExchange)
            {
                cmd.Parameters.AddWithValue("@out", op.Cur_out_num);
            }
            cmd.Parameters.AddWithValue("@is", op.IsExchange);
            cmd.ExecuteNonQuery();
            con.Close();

            return result;
        }
        #endregion
    }
}
