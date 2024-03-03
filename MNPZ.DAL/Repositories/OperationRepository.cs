using MNPZ.DAL.Models;
using MNPZ.DAL.Repositories;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace MNPZ.DAO
{
    public class OperationRepository : BaseRepository
    {
        #region Получить список операций

        public List<Operation> SelectAllOperations(DateTime date)
        {
            throw new NotImplementedException();
            //var contextUser = new UserRepository();
            //var users = contextUser.SelectAllUsers(true);

            //var result = new List<Operation>();
            //var query = "select * from Operations";

            //var from = new DateTime(date.Year, date.Month, date.Day, 0, 0, 0, 0);
            //var to = from.AddDays(1);

            //query += " where DateOperation > '" + from.ToString() + "' AND DateOperation < '" + from.ToString() + "'";
            //con.Open();

            //SqlDataAdapter sda = new SqlDataAdapter(query, con);
            //SqlCommandBuilder builder = new SqlCommandBuilder(sda);
            //var ds = new DataSet();
            //sda.Fill(ds);
            //con.Close();

            //result = ds.Tables[0].AsEnumerable()
            //    .Select(x => new Operation
            //    {
            //        UserName = users.First(u => u.Id == x.Field<int>("UserId")).UserName,
            //        Date = x.Field<DateTime>("DateOperation"),
            //        Amount = Convert.ToDecimal(x.Field<double>("Cost")),
            //        CurrencyIn = x.Field<Currency>("Cur_in_num"),
            //        CurrencyOut = x.Field<Currency?>("Cur_out_num"),
            //        IsExchange = x.Field<bool>("IsExchange")
            //    }).ToList();

            //return result;
        }
        public IEnumerable<OperationInfoModel> SelectAllOperations()
        {
            var result = new List<OperationInfoModel>();
            var spName = "SELECT  * FROM  [dbo].[v_GetOperations]";

            using (var conn = new SqlConnection(_connectionString))
            {
                var command = new SqlCommand(spName, conn);
                conn.Open();
                SqlDataAdapter sda = new SqlDataAdapter(command);
                var ds = new DataSet();
                sda.Fill(ds);
                result = ds.Tables[0]
                    .AsEnumerable()
                    .Select(x => new OperationInfoModel
                    {
                        Date = x.Field<DateTime>("Date"),
                        InAmount = Convert.ToDecimal(x.Field<decimal>("InAmount")),
                        OutAmount = Convert.ToDecimal(x.Field<decimal>("OutAmount")),
                        Remainder = Convert.ToDecimal(x.Field<decimal>("Remainder")),
                        CurrencyIn = x.Field<Currency>("CurrencyIn"),
                        CurrencyOut = x.Field<Currency>("CurrencyOut"),
                        IsExchange = x.Field<bool>("IsExchange"),
                        ExchangeRate = x.Field<decimal>("ExchangeRate"),
                        UserName = x.Field<string>("Name"),
                    }).ToList();
            }

            return result;
        }

        public IEnumerable<OperationInfoModel> SelectAllOperations(int userId)
        {
            var result = new List<OperationInfoModel>();
            var spName = "[dbo].[sp_GetOperationsByUserID]";

            using (var conn = new SqlConnection(_connectionString))
            {
                var command = new SqlCommand(spName, conn);
                command.Parameters.Add(new SqlParameter()
                {
                    Value = userId,
                    DbType = DbType.Int32,
                    Direction = ParameterDirection.Input,
                    ParameterName = "@UserID"
                });
                command.CommandType = CommandType.StoredProcedure;
                conn.Open();
                SqlDataAdapter sda = new SqlDataAdapter(command);
                var ds = new DataSet();
                sda.Fill(ds);
                result = ds.Tables[0]
                    .AsEnumerable()
                    .Select(x => new OperationInfoModel
                    {
                        Date = x.Field<DateTime>("Date"),
                        InAmount = Convert.ToDecimal(x.Field<decimal>("InAmount")),
                        OutAmount = Convert.ToDecimal(x.Field<decimal>("OutAmount")),
                        Remainder = Convert.ToDecimal(x.Field<decimal>("Remainder")),
                        CurrencyIn = x.Field<Currency>("CurrencyIn"),
                        CurrencyOut = x.Field<Currency>("CurrencyOut"),
                        IsExchange = x.Field<bool>("IsExchange"),
                        ExchangeRate = x.Field<decimal>("ExchangeRate"),
                    }).ToList();
            }

            return result;
        }

        public SqlInfo AddFundsToUserBalance(int userId, IEnumerable<BaseBalanceItem> balances)
        {
            var spName = "[dbo].[sp_UpdateUserBalance]";

            DataTable dt = new DataTable();
            //Add columns  
            dt.Columns.Add(new DataColumn("Currency", typeof(int)));
            dt.Columns.Add(new DataColumn("Balance", typeof(decimal)));


            foreach (var bal in balances)
            {
                dt.Rows.Add((int)bal.Currency, bal.Balance);
            }

            try
            {
                using (var conn = new SqlConnection(_connectionString))
                {
                    var command = new SqlCommand(spName, conn);
                    command.Parameters.Add(new SqlParameter()
                    {
                        Value = userId,
                        DbType = DbType.Int32,
                        Direction = ParameterDirection.Input,
                        ParameterName = "@UserId"
                    });
                    command.Parameters.Add(new SqlParameter()
                    {
                        Value = dt,
                        Direction = ParameterDirection.Input,
                        ParameterName = "@Amounts"
                    });

                    command.CommandType = CommandType.StoredProcedure;
                    conn.Open();
                    command.ExecuteNonQuery();

                    return new SqlInfo()
                    {
                        IsError = false,
                        Message = "Баланс оновлен"
                    };
                }
            }
            catch
            {
                return new SqlInfo()
                {
                    IsError = false,
                    Message = "ошибка при обновлении баланса"
                };
            }

        }

        public IEnumerable<BaseBalanceItem> GetUserBalances(int userId)
        {
            var result = new List<BaseBalanceItem>();
            var spName = "[dbo].[sp_GetUserBalances]";

            using (var conn = new SqlConnection(_connectionString))
            {
                var command = new SqlCommand(spName, conn);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.Add(new SqlParameter()
                {
                    Value = userId,
                    DbType = DbType.Int32,
                    Direction = ParameterDirection.Input,
                    ParameterName = "@UserId"
                });
                command.CommandType = CommandType.StoredProcedure;

                conn.Open();
                SqlDataAdapter sda = new SqlDataAdapter(command);
                var ds = new DataSet();
                sda.Fill(ds);
                result = ds.Tables[0]
                    .AsEnumerable()
                    .Select(x => new BaseBalanceItem
                    {
                        Balance = Convert.ToDecimal(x.Field<decimal>("Balance")),
                        Currency = x.Field<Currency>("Currency")
                    }).ToList();
            }

            return result;
        }


        #endregion
        #region Внести в базу операции
        public SqlInfo InsertOperation(Operation op)
        {
            var spName = "[dbo].[sp_InsertOperation]";

            try
            {
                using (var conn = new SqlConnection(_connectionString))
                {
                    var command = new SqlCommand(spName, conn);
                    command.Parameters.AddWithValue("@UserId", op.UserId);
                    command.Parameters.AddWithValue("@Date", op.Date);
                    command.Parameters.AddWithValue("@InAmount", op.InAmount);
                    command.Parameters.AddWithValue("@OutAmount", op.OutAmount);
                    command.Parameters.AddWithValue("@Remainder", op.Remainder);
                    command.Parameters.AddWithValue("@CurrencyIn", (int)op.CurrencyIn);
                    command.Parameters.AddWithValue("@CurrencyOut", (int)op.CurrencyOut);
                    command.Parameters.AddWithValue("@ExchangeRate", op.ExchangeRate);
                    command.Parameters.AddWithValue("@IsExchange", op.IsExchange);

                    command.CommandType = CommandType.StoredProcedure;
                    conn.Open();
                    command.ExecuteNonQuery();

                    return new SqlInfo()
                    {
                        IsError = false,
                        Message = "Баланс оновлен"
                    };
                }
            }
            catch (Exception e)
            {
                return new SqlInfo()
                {
                    IsError = false,
                    Message = "ошибка при обновлении баланса"
                };
            }

        }
        #endregion
    }
}
