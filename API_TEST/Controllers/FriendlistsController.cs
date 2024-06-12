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
    public class FriendlistsController
    {
        private static string connectionString = "Server=localhost;User ID=root;Password=azerty;Database=mj";

        public async Task<string> ProcessRequest(HttpListenerRequest request)
        {
            string responseString = "";

            // Token verification for POST, PUT, PATCH, and DELETE methods
            if (request.HttpMethod == "POST" || request.HttpMethod == "PUT" || request.HttpMethod == "PATCH" || request.HttpMethod == "DELETE")
            {
                var verifiedUser = await TokenVerification.TokenVerify(request);
                if (verifiedUser == null)
                {
                    responseString = "Unauthorized access, wrong or empty token, please refer to the admin for a valid key";
                    return responseString;
                }
            }

            // GET all friend lists
            if (request.HttpMethod == "GET" && request.Url.PathAndQuery == "/api/friendlists")
            {
                var options = new JsonSerializerOptions { WriteIndented = true };
                responseString = JsonSerializer.Serialize(await HttpGetAllFriendlists(), options);
            }
            // GET friend list by id
            else if (request.HttpMethod == "GET" && request.Url.PathAndQuery.StartsWith("/api/friendlists/"))
            {
                string[] parts = request.Url.PathAndQuery.Split('/');
                if (parts.Length == 4 && int.TryParse(parts[3], out int id))
                {
                    var options = new JsonSerializerOptions { WriteIndented = true };
                    responseString = JsonSerializer.Serialize(await HttpGetFriendlistById(id), options);
                    if (responseString == "null")
                    {
                        responseString = "Invalid id, Error = " + (int)HttpStatusCode.BadRequest;
                    }
                }
                else if (parts.Length > 4)
                {
                    responseString = "bad endpoint, Error = " + (int)HttpStatusCode.BadRequest;
                }
                else if (request.Url.PathAndQuery == "/api/friendlists/")
                {
                    responseString = "Enter an id please, bad endpoint, Error = " + (int)HttpStatusCode.BadRequest;
                }
            }

            // POST
            else if (request.HttpMethod == "POST" && request.Url.AbsolutePath == "/api/friendlists")
            {
                using (var reader = new StreamReader(request.InputStream, request.ContentEncoding))
                {
                    string requestBody = await reader.ReadToEndAsync();
                    var data = JsonSerializer.Deserialize<Friendlists>(requestBody);

                    string result = await HttpPostFriendlist(data.User_Main_Id, data.User_Id);
                    responseString = result;
                }
            }
            // PUT
            else if (request.HttpMethod == "PUT" && request.Url.PathAndQuery.StartsWith("/api/friendlists/"))
            {
                string[] parts = request.Url.PathAndQuery.Split('/');
                if (parts.Length == 4 && int.TryParse(parts[3], out int friendlistId))
                {
                    using (var reader = new StreamReader(request.InputStream, request.ContentEncoding))
                    {
                        string requestBody = await reader.ReadToEndAsync();
                        var data = JsonSerializer.Deserialize<Friendlists>(requestBody);

                        string result = await HttpPutFriendlistById(friendlistId, data.User_Main_Id, data.User_Id);
                        responseString = result;
                    }
                }
                else
                {
                    responseString = "Invalid friendlist ID, Error = " + (int)HttpStatusCode.BadRequest;
                }
            }
            // PATCH
            else if (request.HttpMethod == "PATCH" && request.Url.PathAndQuery.StartsWith("/api/friendlists/"))
            {
                string[] parts = request.Url.PathAndQuery.Split('/');
                if (parts.Length == 4 && int.TryParse(parts[3], out int friendlistId))
                {
                    using (var reader = new StreamReader(request.InputStream, request.ContentEncoding))
                    {
                        string requestBody = await reader.ReadToEndAsync();
                        var data = JsonSerializer.Deserialize<Friendlists>(requestBody);

                        string result = await HttpPatchFriendlistById(friendlistId, data.User_Main_Id, data.User_Id);
                        responseString = result;
                    }
                }
                else
                {
                    responseString = "Invalid friendlist ID, Error = " + (int)HttpStatusCode.BadRequest;
                }
            }
            // DELETE
            else if (request.HttpMethod == "DELETE" && request.Url.PathAndQuery.StartsWith("/api/friendlists/"))
            {
                string[] parts = request.Url.PathAndQuery.Split('/');
                if (parts.Length == 4 && int.TryParse(parts[3], out int friendlistId))
                {
                    string result = await HttpDelFriendlistById(friendlistId);
                    responseString = result;
                }
                else
                {
                    responseString = "Invalid friendlist ID, Error = " + (int)HttpStatusCode.BadRequest;
                }
            }

            return responseString;
        }

        private async Task<IEnumerable<Friendlists>> HttpGetAllFriendlists()
        {
            List<Friendlists> friendlists = new List<Friendlists>();

            try
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    await connection.OpenAsync();

                    string sqlRequest = "SELECT * FROM friendlists";

                    using (MySqlCommand command = new MySqlCommand(sqlRequest, connection))
                    {
                        using (MySqlDataReader reader = await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                Friendlists friendlist = new Friendlists
                                {
                                    Friendlist_Id = Convert.ToInt32(reader["Friendlist_Id"]),
                                    User_Main_Id = Convert.ToInt32(reader["User_Main_Id"]),
                                    User_Id = Convert.ToInt32(reader["User_Id"])
                                };
                                friendlists.Add(friendlist);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error during friendlists retrieval: {ex.Message}");
            }

            return friendlists;
        }

        private async Task<Friendlists> HttpGetFriendlistById(int id)
        {
            Friendlists friendlist = null;

            try
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    await connection.OpenAsync();

                    string sqlRequest = "SELECT * FROM friendlists WHERE Friendlist_Id = @FriendlistId";

                    using (MySqlCommand command = new MySqlCommand(sqlRequest, connection))
                    {
                        command.Parameters.AddWithValue("@FriendlistId", id);

                        using (MySqlDataReader reader = await command.ExecuteReaderAsync())
                        {
                            if (await reader.ReadAsync())
                            {
                                friendlist = new Friendlists
                                {
                                    Friendlist_Id = Convert.ToInt32(reader["Friendlist_Id"]),
                                    User_Main_Id = Convert.ToInt32(reader["User_Main_Id"]),
                                    User_Id = Convert.ToInt32(reader["User_Id"])
                                };
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error during friendlist retrieval: {ex.Message}");
            }

            return friendlist;
        }

        

        private async Task<string> HttpPostFriendlist(int userMainId, int userId)
        {
            try
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    await connection.OpenAsync();

                    string sqlRequest = "INSERT INTO friendlists (User_Main_Id, User_Id) VALUES (@UserMainId, @UserId)";

                    using (MySqlCommand command = new MySqlCommand(sqlRequest, connection))
                    {
                        command.Parameters.AddWithValue("@UserMainId", userMainId);
                        command.Parameters.AddWithValue("@UserId", userId);

                        await command.ExecuteNonQueryAsync();
                    }
                }
            }
            catch (Exception ex)
            {
                return $"Error during friendlist creation: {ex.Message}";
            }

            return "Friendlist created successfully";
        }

        private async Task<string> HttpPutFriendlistById(int friendlistId, int userMainId, int userId)
        {
            try
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    await connection.OpenAsync();

                    string sqlRequest = "UPDATE friendlists SET User_Main_Id = @UserMainId, User_Id = @UserId WHERE Friendlist_Id = @FriendlistId";

                    using (MySqlCommand command = new MySqlCommand(sqlRequest, connection))
                    {
                        command.Parameters.AddWithValue("@UserMainId", userMainId);
                        command.Parameters.AddWithValue("@UserId", userId);
                        command.Parameters.AddWithValue("@FriendlistId", friendlistId);

                        int rowsAffected = await command.ExecuteNonQueryAsync();
                        if (rowsAffected > 0)
                        {
                            return "Friendlist updated successfully";
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
                return $"Error during friendlist update: {ex.Message}";
            }
        }

        private async Task<string> HttpPatchFriendlistById(int friendlistId, int userMainId, int userId)
        {
            try
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    await connection.OpenAsync();

                    var updates = new List<string>();
                    var parameters = new Dictionary<string, object>();

                    if (userMainId != 0)
                    {
                        updates.Add("User_Main_Id = @UserMainId");
                        parameters["@UserMainId"] = userMainId;
                    }

                    if (userId != 0)
                    {
                        updates.Add("User_Id = @UserId");
                        parameters["@UserId"] = userId;
                    }

                    if (updates.Count == 0)
                    {
                        return "No fields to update.";
                    }

                    string sqlRequest = "UPDATE friendlists SET " + string.Join(", ", updates) + " WHERE Friendlist_Id = @FriendlistId";
                    parameters["@FriendlistId"] = friendlistId;

                    using (MySqlCommand command = new MySqlCommand(sqlRequest, connection))
                    {
                        foreach (var param in parameters)
                        {
                            command.Parameters.AddWithValue(param.Key, param.Value);
                        }

                        int rowsAffected = await command.ExecuteNonQueryAsync();
                        if (rowsAffected > 0)
                        {
                            return "Friendlist updated successfully";
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
                return $"Error during friendlist patch update: {ex.Message}";
            }
        }

        private async Task<string> HttpDelFriendlistById(int id)
        {
            try
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    await connection.OpenAsync();

                    string sqlRequest = "DELETE FROM friendlists WHERE Friendlist_Id = @FriendlistId";

                    using (MySqlCommand command = new MySqlCommand(sqlRequest, connection))
                    {
                        command.Parameters.AddWithValue("@FriendlistId", id);

                        int rowsAffected = await command.ExecuteNonQueryAsync();
                        if (rowsAffected > 0)
                        {
                            return "Friendlist deleted successfully";
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
                return $"Error during friendlist deletion: {ex.Message}";
            }
        }
    }
}
