using Models;
using MySqlConnector;
using System.Net;
using System.Threading.Tasks;

namespace Tools
{
    public static class TokenVerification
    {
        private static string connectionString = "Server=localhost;User ID=root;Password=azerty;Database=mj";

        public static async Task<Users> TokenVerify(HttpListenerRequest request)
        {
            Users user = null;

            string authorizationHeader = request.Headers["Authorization"];
            if (string.IsNullOrEmpty(authorizationHeader) || !authorizationHeader.StartsWith("Bearer "))
            {
                return user;
            }

            string token = authorizationHeader.Substring("Bearer ".Length);

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                await connection.OpenAsync();

                string sqlQuery = @"
                    SELECT u.*
                    FROM users u
                    JOIN tokens t ON u.User_Id = t.User_Id
                    WHERE t.Token_Value = @Token;
                ";

                using (MySqlCommand command = new MySqlCommand(sqlQuery, connection))
                {
                    command.Parameters.AddWithValue("@Token", token);

                    using (MySqlDataReader reader = await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            user = new Users
                            {
                                User_Id = Convert.ToInt32(reader["User_Id"]),
                                User_FirstName = reader["User_FirstName"].ToString(),
                                User_LastName = reader["User_LastName"].ToString(),
                                User_Email = reader["User_Email"].ToString(),
                                User_Password = reader["User_Password"].ToString(),
                                User_Phone = reader["User_Phone"].ToString(),
                            };
                        }
                    }
                }
            }

            return user;
        }
    }
}
