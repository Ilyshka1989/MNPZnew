using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web.UI.WebControls;
using System.Windows.Forms;
using System.Xml.Linq;

namespace MNPZ.DAO
{
    public class UserContext: MainDataContext
    {
        SqlConnection con = new SqlConnection(connectDb);

        #region Получить список пользователей
        public List<User> SelectAllUsers(string[] logins, string[] userNames, bool? isOperators)
        {
            var result = new List<User>();
            var query = "select * from UserTb";
            string andSql = " AND ";

            if (logins != null || userNames != null || isOperators != null)
            {
                query += " where ";
            }

            if (logins != null)
            {
                andSql = " AND ";
                for (int i = 0; i < logins.Length; i++)
                {
                    if (i == 0)
                        andSql = "";

                    var row = "Login = '" + logins[i] + "'" + andSql;
                    query += row;
                }
            }

            if (userNames != null)
            {
                andSql = " AND ";
                for (int i = 0; i < userNames.Length; i++)
                {
                    if (i == 0)
                        andSql = "";

                    var row = "UserName = '" + userNames[i] + "'" + andSql;
                    query += row;
                }
            }

            if (isOperators != null)
            {
                if (andSql == "")
                    andSql = " AND ";

                if (!isOperators.Value)
                    query += andSql + "IsOperator = 0";
                else query += andSql + "IsOperator = 1";
            }

            con.Open();

            SqlDataAdapter sda = new SqlDataAdapter(query, con);
            SqlCommandBuilder builder = new SqlCommandBuilder(sda);
            var ds = new DataSet();
            sda.Fill(ds);
            con.Close();

            result = ds.Tables[0].AsEnumerable()
                .Select(x => new User
                {
                    UserName = x.Field<string>("UserName"),
                    Id = x.Field<int>("Id"),
                    Login = x.Field<string>("Login"),
                    Password = x.Field<string>("Password"),
                    IsOperator = x.Field<bool>("IsOperator")
                }).ToList();    

            return result;
        }
        public List<User> SelectAllUsers(bool isOperator)
        {
            var result = new List<User>();
            var query = "select * from UserTb ";
            
                if (!isOperator)
                    query += "where IsOperator = 0";
                else query += "where IsOperator = 1";

            con.Open();

            SqlDataAdapter sda = new SqlDataAdapter(query, con);
            SqlCommandBuilder builder = new SqlCommandBuilder(sda);
            var ds = new DataSet();
            sda.Fill(ds);
            con.Close();

            result = ds.Tables[0].AsEnumerable()
                .Select(x => new User
                {
                    UserName = x.Field<string>("UserName"),
                    Id = x.Field<int>("Id"),
                    Login = x.Field<string>("Login"),
                    Password = x.Field<string>("Password"),
                    IsOperator = x.Field<bool>("IsOperator")
                }).ToList();

            return result;
        }
        public List<User> SelectAllUsers()
        {
            var result = new List<User>();
            var query = "select * from UserTb";
           
            con.Open();

            SqlDataAdapter sda = new SqlDataAdapter(query, con);
            SqlCommandBuilder builder = new SqlCommandBuilder(sda);
            var ds = new DataSet();
            sda.Fill(ds);
            con.Close();

            result = ds.Tables[0].AsEnumerable()
                .Select(x => new User
                {
                    UserName = x.Field<string>("UserName"),
                    Id = x.Field<int>("Id"),
                    Login = x.Field<string>("Login"),
                    Password = x.Field<string>("Password"),
                    IsOperator = x.Field<bool>("IsOperator")
                }).ToList();

            return result;
        }
        #endregion
        #region Получить пользователя по айди или логину или имени
        public User SelectUserBy(int id)
        {
            var result = new User();
            var query = "select * from UserTb";
                query += " where Id = " + id.ToString();

            con.Open();

            SqlDataAdapter sda = new SqlDataAdapter(query, con);
            SqlCommandBuilder builder = new SqlCommandBuilder(sda);
            var ds = new DataSet();
            sda.Fill(ds);
            con.Close();

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

            return result;
        }
        public User SelectUserBy(bool isLogin, string row)
        {
            var result = new User();
            var query = "select * from UserTb";
            if (isLogin)
            {
                query += " where Login = '" + row + "'";
            }
            else
            {
                query += " where UserName = '" + row + "'";
            }
            con.Open();

            SqlDataAdapter sda = new SqlDataAdapter(query, con);
            SqlCommandBuilder builder = new SqlCommandBuilder(sda);
            var ds = new DataSet();
            sda.Fill(ds);
            con.Close();

            result = ds.Tables[0].AsEnumerable()
                .Select(x => new User
                {
                    UserName = x.Field<string>("UserName"),
                    Id = x.Field<int>("Id"),
                    Login = x.Field<string>("Login"),
                    Password = x.Field<string>("Password"),
                    IsOperator = x.Field<bool>("IsOperator"),
                })
                .ToList()
                .FirstOrDefault();

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

            string query = "insert into UserTb(UserName, Login, Password, IsOperator) values(@N, @L, @P, @O)";
            con.Open();
            SqlCommand cmd = new SqlCommand(query, con);
            cmd.Parameters.AddWithValue("@N", userName);
            cmd.Parameters.AddWithValue("@L", login);
            cmd.Parameters.AddWithValue("@P", password);
            if (isOperator.HasValue)
            {
                if (isOperator.Value)
                    cmd.Parameters.AddWithValue("@O", 1);
                else cmd.Parameters.AddWithValue("@O", 0);
            }
            else
            {
                cmd.Parameters.AddWithValue("@O", 0);
            }

            cmd.ExecuteNonQuery();
            con.Close();
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

            string query = "update UserTb set UserName=@N, Login=@L, Password=@P, IsOperator=@O where Id=@Id";
            con.Open();
            SqlCommand cmd = new SqlCommand(query, con);
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
            con.Close();
            result.Message = "Данные сохранены";

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

            string query = "delete from UserTb where Id = " + id.ToString();
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
