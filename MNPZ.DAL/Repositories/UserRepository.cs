using MNPZ.DAL.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace MNPZ.DAL.Repositories
{
    public class UserRepository : BaseRepository
    {

        #region Получить список пользователей
        public IEnumerable<User> SelectAllUsers(bool? isOperators)
        {
            var isOperatorBit = isOperators.HasValue && isOperators.Value ? 1 : 0;

            var query = $@"SELECT * from [User] WHERE IsOperator = {isOperatorBit}";
            var result = new List<User>();

            using (var conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                SqlDataAdapter sda = new SqlDataAdapter(query, conn);
                var ds = new DataSet();
                sda.Fill(ds);

                result = ds.Tables[0].AsEnumerable()
                    .Select(x => new User
                    {
                        UserName = x.Field<string>("UserName"),
                        Id = x.Field<int>("Id"),
                        Login = x.Field<string>("Login"),
                        Password = x.Field<string>("Password"),
                        IsOperator = x.Field<bool>("IsOperator")
                    }).ToList();
            }

            return result;
        }
        public List<User> SelectAllUsers(bool isOperator)
        {
            var result = new List<User>();
            var query = "select * from [User] ";

            if (!isOperator)
                query += "where IsOperator = 0";
            else query += "where IsOperator = 1";

            using (var conn = new SqlConnection(_connectionString))
            {
                conn.Open();

                SqlDataAdapter sda = new SqlDataAdapter(query, conn);
                SqlCommandBuilder builder = new SqlCommandBuilder(sda);
                var ds = new DataSet();
                sda.Fill(ds);

                result = ds.Tables[0].AsEnumerable()
                    .Select(x => new User
                    {
                        UserName = x.Field<string>("Name"),
                        Id = x.Field<int>("Id"),
                        Login = x.Field<string>("Login"),
                        Password = x.Field<string>("Password"),
                        IsOperator = x.Field<bool>("IsOperator")
                    }).ToList();
            }

            return result;
        }

        public bool TryDeleteUser(int id)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string query = "DELETE FROM [User] WHERE Id = @ID";

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

        public List<User> SelectAllUsers()
        {
            var result = new List<User>();
            var query = "SELECT * FROM dbo.[User]";

            using (var conn = new SqlConnection(_connectionString))
            {
                conn.Open();

                SqlDataAdapter sda = new SqlDataAdapter(query, conn);
                SqlCommandBuilder builder = new SqlCommandBuilder(sda);
                var ds = new DataSet();
                sda.Fill(ds);

                result = ds.Tables[0].AsEnumerable()
                    .Select(x => new User
                    {
                        UserName = x.Field<string>("Name"),
                        Id = x.Field<int>("Id"),
                        Login = x.Field<string>("Login"),
                        Password = x.Field<string>("Password"),
                        IsOperator = x.Field<bool>("IsOperator")
                    }).ToList();
            }

            return result;
        }
        #endregion
        #region Получить пользователя по айди или логину или имени
        public User SelectUserBy(int id)
        {
            var result = new User();
            var query = "select * from [User]";
            query += " where Id = " + id.ToString();

            using (var conn = new SqlConnection(_connectionString))
            {
                conn.Open();

                SqlDataAdapter sda = new SqlDataAdapter(query, conn);
                SqlCommandBuilder builder = new SqlCommandBuilder(sda);
                var ds = new DataSet();
                sda.Fill(ds);
                conn.Close();

                result = ds.Tables[0].AsEnumerable()
                    .Select(x => new User
                    {
                        UserName = x.Field<string>("UserName"),
                        Id = x.Field<int>("Id"),
                        Login = x.Field<string>("Login"),
                        Password = x.Field<string>("Password"),
                        IsOperator = x.Field<bool>("IsOperator")
                    })
                    .ToList()
                    .FirstOrDefault();
            }

            return result;
        }
        public User SelectUserBy(bool isLogin, string row)
        {
            var result = new User();
            var query = "SELECT * FROM [User]";
            if (isLogin)
            {
                query += " where Login = '" + row + "'";
            }
            else
            {
                query += " where UserName = '" + row + "'";
            }

            using (var conn = new SqlConnection(_connectionString))
            {
                conn.Open();

                SqlDataAdapter sda = new SqlDataAdapter(query, conn);
                SqlCommandBuilder builder = new SqlCommandBuilder(sda);
                var ds = new DataSet();
                sda.Fill(ds);
                result = ds.Tables[0].AsEnumerable()
                    .Select(x => new User
                    {
                        UserName = x.Field<string>("Name"),
                        Id = x.Field<int>("Id"),
                        Login = x.Field<string>("Login"),
                        Password = x.Field<string>("Password"),
                        IsOperator = x.Field<bool>("IsOperator"),
                    })
                    .ToList()
                    .FirstOrDefault();

            }

            return result;
        }
        #endregion
        #region Внести в базу пользователя
        public SqlInfo InsertUser(string login, string userName, string password, bool? isOperator)
        {
            var result = new SqlInfo();
            result.IsError = false;

            var checkUser = SelectUserBy(true, login);
            if (checkUser != null)
            {
                result.IsError = true;
                result.Message = "Пользователь с таким логином уже существует!";
                return result;
            }

            string query = "INSERT INTO [User] ([Name], Login, Password, IsOperator) values(@N, @L, @P, @O)";
            using (var conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@N", userName);
                cmd.Parameters.AddWithValue("@L", login);
                cmd.Parameters.AddWithValue("@P", password);
                if (isOperator.Value)
                    cmd.Parameters.AddWithValue("@O", 1);
                else cmd.Parameters.AddWithValue("@O", 0);

                cmd.ExecuteNonQuery();
            }

            result.Message = "Данные изменены!";

            return result;
        }
        #endregion
        #region Обновить базу
        public SqlInfo UpdateUserById(int id, string login, string userName, string password, bool? isOperator)
        {
            var result = new SqlInfo();
            result.IsError = false;

            var checkUser = SelectUserBy(id);
            if (checkUser == null)
            {
                result.IsError = true;
                result.Message = "Пользователь не найден!";
                return result;
            }

            string query = "update [USER] set UserName=@N, Login=@L, Password=@P, IsOperator=@O where Id=@Id";
            using (var conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@N", userName);
                cmd.Parameters.AddWithValue("@L", login);
                cmd.Parameters.AddWithValue("@P", password);
                cmd.Parameters.AddWithValue("@Id", id);
                if (isOperator.HasValue)
                {
                    if (isOperator.Value)
                        cmd.Parameters.AddWithValue("@O", 1);
                    else cmd.Parameters.AddWithValue("@O", 0);
                }
                else
                {
                    cmd.Parameters.AddWithValue("@O", checkUser.IsOperator);
                }

                cmd.ExecuteNonQuery();
                result.Message = "Данные сохранены";
            }

            return result;
        }
        #endregion
        #region Удалить пользователя
        public SqlInfo DeleteUserById(int id)
        {
            var result = new SqlInfo();
            result.IsError = false;

            var checkUser = SelectUserBy(id);
            if (checkUser != null)
            {
                result.IsError = true;
                result.Message = "Такого пользователя не существует!";
            }

            string query = "delete from [User] where Id = " + id.ToString();
            using (var conn = new SqlConnection(_connectionString))
            {
                SqlCommand cmd = new SqlCommand(query, conn);

                cmd.ExecuteNonQuery();
                result.Message = "Пользователь удалён!";
            }

            return result;
        }
        #endregion

    }
}
