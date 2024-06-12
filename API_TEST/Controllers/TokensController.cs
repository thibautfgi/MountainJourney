using Microsoft.VisualBasic;
using Models;
using MySqlConnector;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.Json;
using Tools;

namespace Controllers
{
    public class TokensController
    {
        private static string connectionString = "Server=localhost;User ID=root;Password=azerty;Database=mj";

        public async Task<string> ProcessRequest(HttpListenerRequest request)
        {
            string responseString = "";

            // Token verification for all methods
            var verifiedUser = await TokenVerification.TokenVerify(request);
            if (verifiedUser == null)
            {
                responseString = "Unauthorized access, wrong or empty token, please refer to the admin for a valid key";
                return responseString;
            }

            // GET all tokens
            if (request.HttpMethod == "GET" && request.Url.PathAndQuery == "/api/tokens")
            {
                var options = new JsonSerializerOptions { WriteIndented = true };
                responseString = JsonSerializer.Serialize(await HttpGetAllTokens(), options);
            }
            // GET token by id
            else if (request.HttpMethod == "GET" && request.Url.PathAndQuery.StartsWith("/api/tokens/"))
            {
                string[] parts = request.Url.PathAndQuery.Split('/');
                if (parts.Length == 4 && int.TryParse(parts[3], out int id))
                {
                    var options = new JsonSerializerOptions { WriteIndented = true };
                    responseString = JsonSerializer.Serialize(await HttpGetTokenById(id), options);
                    if (responseString == "null")
                    {
                        responseString = "Invalid id, Error = " + (int)HttpStatusCode.BadRequest;
                    }
                }
                else if (parts.Length > 4)
                {
                    responseString = "Bad endpoint, Error = " + (int)HttpStatusCode.BadRequest;
                }
                else if (request.Url.PathAndQuery == "/api/tokens/")
                {
                    responseString = "Enter an id please, bad endpoint, Error = " + (int)HttpStatusCode.BadRequest;
                }
            }
            // POST
            else if (request.HttpMethod == "POST" && request.Url.AbsolutePath == "/api/tokens")
            {
                using (var reader = new StreamReader(request.InputStream, request.ContentEncoding))
                {
                    string requestBody = await reader.ReadToEndAsync();
                    var data = JsonSerializer.Deserialize<Tokens>(requestBody);

                    string result = await HttpPostToken(data.User_Id, data.Token_Value);
                    responseString = result;
                }
            }
            // PUT
            else if (request.HttpMethod == "PUT" && request.Url.PathAndQuery.StartsWith("/api/tokens/"))
            {
                string[] parts = request.Url.PathAndQuery.Split('/');
                if (parts.Length == 4 && int.TryParse(parts[3], out int tokenId))
                {
                    using (var reader = new StreamReader(request.InputStream, request.ContentEncoding))
                    {
                        string requestBody = await reader.ReadToEndAsync();
                        var data = JsonSerializer.Deserialize<Tokens>(requestBody);

                        string result = await HttpPutTokenById(tokenId, data.User_Id, data.Token_Value);
                        responseString = result;
                    }
                }
                else
                {
                    responseString = "Invalid token ID, Error = " + (int)HttpStatusCode.BadRequest;
                }
            }
            // PATCH
            else if (request.HttpMethod == "PATCH" && request.Url.PathAndQuery.StartsWith("/api/tokens/"))
            {
                string[] parts = request.Url.PathAndQuery.Split('/');
                if (parts.Length == 4 && int.TryParse(parts[3], out int tokenId))
                {
                    using (var reader = new StreamReader(request.InputStream, request.ContentEncoding))
                    {
                        string requestBody = await reader.ReadToEndAsync();
                        var data = JsonSerializer.Deserialize<Tokens>(requestBody);

                        string result = await HttpPatchTokenById(tokenId, data.User_Id, data.Token_Value);
                        responseString = result;
                    }
                }
                else
                {
                    responseString = "Invalid token ID, Error = " + (int)HttpStatusCode.BadRequest;
                }
            }
            // DELETE
            else if (request.HttpMethod == "DELETE" && request.Url.PathAndQuery.StartsWith("/api/tokens/"))
            {
                string[] parts = request.Url.PathAndQuery.Split('/');
                if (parts.Length == 4 && int.TryParse(parts[3], out int tokenId))
                {
                    string result = await HttpDelTokenById(tokenId);
                    responseString = result;
                }
                else
                {
                    responseString = "Invalid token ID, Error = " + (int)HttpStatusCode.BadRequest;
                }
            }

            return responseString;
        }

        private async Task<IEnumerable<Tokens>> HttpGetAllTokens()
        {
            List<Tokens> tokens = new List<Tokens>();

            try
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    await connection.OpenAsync();

                    string sqlRequest = "SELECT * FROM tokens";

                    using (MySqlCommand command = new MySqlCommand(sqlRequest, connection))
                    {
                        using (MySqlDataReader reader = await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                Tokens token = new Tokens
                                {
                                    Token_Id = Convert.ToInt32(reader["Token_Id"]),
                                    User_Id = Convert.ToInt32(reader["User_Id"]),
                                    Token_Value = reader["Token_Value"].ToString()
                                };
                                tokens.Add(token);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error during tokens retrieval: {ex.Message}");
            }

            return tokens;
        }

        private async Task<Tokens> HttpGetTokenById(int id)
        {
            Tokens token = null;

            try
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    await connection.OpenAsync();

                    string sqlRequest = "SELECT * FROM tokens WHERE Token_Id = @TokenId";

                    using (MySqlCommand command = new MySqlCommand(sqlRequest, connection))
                    {
                        command.Parameters.AddWithValue("@TokenId", id);

                        using (MySqlDataReader reader = await command.ExecuteReaderAsync())
                        {
                            if (await reader.ReadAsync())
                            {
                                token = new Tokens
                                {
                                    Token_Id = Convert.ToInt32(reader["Token_Id"]),
                                    User_Id = Convert.ToInt32(reader["User_Id"]),
                                    Token_Value = reader["Token_Value"].ToString()
                                };
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error during token retrieval: {ex.Message}");
            }

            return token;
        }

        private async Task<string> HttpPostToken(int userId, string tokenValue)
        {
            try
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    await connection.OpenAsync();

                    string sqlRequest = "INSERT INTO tokens (User_Id, Token_Value) VALUES (@UserId, @TokenValue)";

                    using (MySqlCommand command = new MySqlCommand(sqlRequest, connection))
                    {
                        command.Parameters.AddWithValue("@UserId", userId);
                        command.Parameters.AddWithValue("@TokenValue", tokenValue);

                        await command.ExecuteNonQueryAsync();
                    }
                }
            }
            catch (Exception ex)
            {
                return $"Error during token creation: {ex.Message}";
            }

            return "Token created successfully";
        }

        private async Task<string> HttpPutTokenById(int tokenId, int userId, string tokenValue)
        {
            try
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    await connection.OpenAsync();

                    string sqlRequest = "UPDATE tokens SET User_Id = @UserId, Token_Value = @TokenValue WHERE Token_Id = @TokenId";

                    using (MySqlCommand command = new MySqlCommand(sqlRequest, connection))
                    {
                        command.Parameters.AddWithValue("@UserId", userId);
                        command.Parameters.AddWithValue("@TokenValue", tokenValue);
                        command.Parameters.AddWithValue("@TokenId", tokenId);

                        int rowsAffected = await command.ExecuteNonQueryAsync();
                        if (rowsAffected > 0)
                        {
                            return "Token updated successfully";
                        }
                        else
                        {
                            return "Invalid id or no rows affected.";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return $"Error during token update: {ex.Message}";
            }
        }

        private async Task<string> HttpPatchTokenById(int tokenId, int userId, string tokenValue)
        {
            try
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    await connection.OpenAsync();

                    var updates = new List<string>();
                    var parameters = new Dictionary<string, object>();

                    if (userId != 0)
                    {
                        updates.Add("User_Id = @UserId");
                        parameters["@UserId"] = userId;
                    }

                    if (!string.IsNullOrEmpty(tokenValue))
                    {
                        updates.Add("Token_Value = @TokenValue");
                        parameters["@TokenValue"] = tokenValue;
                    }

                    if (updates.Count == 0)
                    {
                        return "No fields to update.";
                    }

                    string sqlRequest = "UPDATE tokens SET " + string.Join(", ", updates) + " WHERE Token_Id = @TokenId";
                    parameters["@TokenId"] = tokenId;

                    using (MySqlCommand command = new MySqlCommand(sqlRequest, connection))
                    {
                        foreach (var param in parameters)
                        {
                            command.Parameters.AddWithValue(param.Key, param.Value);
                        }

                        int rowsAffected = await command.ExecuteNonQueryAsync();
                        if (rowsAffected > 0)
                        {
                            return "Token updated successfully";
                        }
                        else
                        {
                            return "Invalid id or no rows affected.";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return $"Error during token patch update: {ex.Message}";
            }
        }

        private async Task<string> HttpDelTokenById(int id)
        {
            try
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    await connection.OpenAsync();

                    string sqlRequest = "DELETE FROM tokens WHERE Token_Id = @TokenId";

                    using (MySqlCommand command = new MySqlCommand(sqlRequest, connection))
                    {
                        command.Parameters.AddWithValue("@TokenId", id);

                        int rowsAffected = await command.ExecuteNonQueryAsync();
                        if (rowsAffected > 0)
                        {
                            return "Token deleted successfully";
                        }
                        else
                        {
                            return "Invalid id or no rows affected.";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return $"Error during token deletion: {ex.Message}";
            }
        }
    }
}
