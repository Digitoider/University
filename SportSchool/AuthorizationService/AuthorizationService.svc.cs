using MySql.Data.MySqlClient;
using System;

namespace AuthorizationService
{
    public class AuthorizationService : IAuthorizationService
    {
        MySqlConnectionStringBuilder connectionString;
        public AuthorizationService()
        {
            connectionString = new MySqlConnectionStringBuilder();
            connectionString.Server = "localhost";//192.168.56.101
            connectionString.Port = 3306;
            connectionString.Database = "SportSchool";
            connectionString.UserID = "root";
            connectionString.Password = "1111";
            connectionString.CharacterSet = "utf8";
        }
        public string CheckUser(string login, string password)
        {
            if (login == "Admin" && password == "1111") return "Admin";
            using (MySqlConnection connection = new MySqlConnection(connectionString.ConnectionString))
            {
                string query = "SELECT Count(*) FROM Instructors WHERE Email=@email AND Password=@password";

                using (MySqlCommand cmd = new MySqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@email", login);
                    cmd.Parameters.AddWithValue("@password", password);
                    try
                    {
                        connection.Open();
                        int count = Convert.ToInt32(cmd.ExecuteScalar());
                        if (count == 1)
                        {
                            return "Instructor";
                        }
                    }
                    catch (MySqlException ex)
                    {
                    }
                    finally
                    {
                        connection.Close();
                    }
                    cmd.CommandText = "SELECT Count(*) FROM Learners WHERE Email=@email AND Password=@password";
                    cmd.Parameters.Clear();
                    cmd.Parameters.AddWithValue("@email", login);
                    cmd.Parameters.AddWithValue("@password", password);
                    try
                    {
                        connection.Open();
                        int count = Convert.ToInt32(cmd.ExecuteScalar());
                        if (count == 1)
                        {
                            return "Learner";
                        }
                    }
                    catch (MySqlException ex)
                    {
                    }
                    finally
                    {
                        connection.Close();
                    }
                }
            }
            return "Undefined";
        }
    }
}
