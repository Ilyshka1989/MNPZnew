using MNPZ.DAL.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace MNPZ.DAL.Repositories
{
    public class RateRepository : BaseRepository
    {
        #region Получить список курсов
        public IEnumerable<Rate> SelectAllRates()
        {
            var result = new List<Rate>();
            var query = "SELECT * FROM Rates";
            using (var conn = new SqlConnection(_connectionString))
            {
                conn.Open();

                SqlDataAdapter sda = new SqlDataAdapter(query, conn);
                SqlCommandBuilder builder = new SqlCommandBuilder(sda);
                var ds = new DataSet();
                sda.Fill(ds);


                result = ds.Tables[0].AsEnumerable()
                    .Select(x => new Rate
                    {
                        CurInAmount = Convert.ToDecimal(x.Field<decimal>("CurInAmount")),
                        CurOutAmount = Convert.ToDecimal(x.Field<decimal>("CurOutAmount")),
                        CurIn = x.Field<Currency>("CurIn"),
                        CurOut = x.Field<Currency>("CurOut"),
                        Id = x.Field<int>("Id")
                    }).ToList();
            }

            return result;
        }
        #endregion
        #region Получить курс по валютам
        public Rate SelectMoneyByCurrency(Currency curIn, Currency curOut)
        {
            var result = new Rate();
            var query = $"SELECT * FROM Rates where CurIn = {(int)curIn} AND CurOut = {(int)curOut}";

            using (var conn = new SqlConnection(_connectionString))
            {
                conn.Open();

                SqlDataAdapter sda = new SqlDataAdapter(query, conn);
                SqlCommandBuilder builder = new SqlCommandBuilder(sda);
                var ds = new DataSet();
                sda.Fill(ds);

                result = ds.Tables[0].AsEnumerable()
                    .Select(x => new Rate
                    {
                        CurInAmount = Convert.ToDecimal(x.Field<decimal>("CurInAmount")),
                        CurOutAmount = Convert.ToDecimal(x.Field<decimal>("CurOutAmount")),
                        CurIn = x.Field<Currency>("CurIn"),
                        CurOut = x.Field<Currency>("CurOut"),
                        Id = x.Field<int>("Id"),
                    }).ToList().FirstOrDefault();
            }

            return result;
        }
        #endregion

        #region Внести в базу курсы
        public SqlInfo InsertOrUpdateRate(Rate money)
        {
            var result = new SqlInfo();
            result.IsError = false;

            var checkMoney = SelectMoneyByCurrency(money.CurIn, money.CurOut);

            if (checkMoney != null)
            {
                return UpdateRate(checkMoney.Id, money);
            }

            string query = "INSERT INTO Rates (CurInAmount, CurOutAmount, CurIn, CurOut) values(@N, @L, @P, @O)";

            using (var conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@N", money.CurInAmount);
                cmd.Parameters.AddWithValue("@L", money.CurOutAmount);
                cmd.Parameters.AddWithValue("@P", money.CurIn);
                cmd.Parameters.AddWithValue("@O", money.CurOut);

                cmd.ExecuteNonQuery();
            }
            result.Message = "Данные изменены!";

            return result;
        }
        #endregion
        #region Обновить базу
        public SqlInfo UpdateRate(int id, Rate rate)
        {
            var result = new SqlInfo();
            result.IsError = false;

            string query = "UPDATE Rates SET CurInAmount=@InAmount, CurOutAmount=@OurAmount , CurIn = @CurIn , CurOut = @CurOut  WHERE Id = @Id";
            using (var conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@Id", id);
                cmd.Parameters.AddWithValue("@InAmount", rate.CurInAmount);
                cmd.Parameters.AddWithValue("@OurAmount", rate.CurOutAmount);
                cmd.Parameters.AddWithValue("@CurIn", (int)rate.CurIn);
                cmd.Parameters.AddWithValue("@CurOut", (int)rate.CurOut);
                cmd.ExecuteNonQuery();
            }

            result.Message = "Данные изменены!";

            return result;
        }
        #endregion
        #region Удалить курсы
        public SqlInfo DeleteMoney(Currency curIn, Currency curOut)
        {
            var result = new SqlInfo();
            result.IsError = false;

            var checkMoney = SelectMoneyByCurrency(curIn, curOut);
            if (checkMoney != null)
            {
                result.IsError = true;
                result.Message = "Таких курсов не существует!";
            }

            string query = "delete from Rates where CurIn=" + checkMoney.CurInAmount + " AND CurOut=" + checkMoney.CurOutAmount;
            using (var conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(query, conn);

                cmd.ExecuteNonQuery();
            }
            result.Message = "Пользователь удалён!";

            return result;
        }
        #endregion

        public bool TryDelereRate(int id)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string query = "DELETE FROM [Rates] WHERE Id = @ID";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@ID", id);

                    try
                    {
                        connection.Open();

                        int rowsAffected = command.ExecuteNonQuery();

                        return rowsAffected > 0;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Ошибка при удалении записи: " + ex.Message);
                        return false;
                    }
                    finally { connection.Close(); }
                }
            }
        }
    }
}
