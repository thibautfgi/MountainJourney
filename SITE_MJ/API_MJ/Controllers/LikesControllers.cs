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
    public class LikesController
    {
        private static string connectionString = "Server=172.16.238.3;User ID=api;Password=azerty;Database=MountainJourney";

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

            // GET all likes
            if (request.HttpMethod == "GET" && request.Url.PathAndQuery == "/api/likes")
            {
                var options = new JsonSerializerOptions { WriteIndented = true };
                responseString = JsonSerializer.Serialize(await HttpGetAllLikes(), options);
            }
            // GET likes by id
            else if (request.HttpMethod == "GET" && request.Url.PathAndQuery.StartsWith("/api/likes/"))
            {
                string[] parts = request.Url.PathAndQuery.Split('/');
                if (parts.Length == 4 && int.TryParse(parts[3], out int id))
                {
                    var options = new JsonSerializerOptions { WriteIndented = true };
                    responseString = JsonSerializer.Serialize(await HttpGetLikeById(id), options);
                    if (responseString == "null")
                    {
                        responseString = "Invalid id, Error = " + (int)HttpStatusCode.BadRequest;
                    }
                }
                else if (parts.Length > 4)
                {
                    responseString = "bad endpoint, Error = " + (int)HttpStatusCode.BadRequest;
                }
                else if (request.Url.PathAndQuery == "/api/likes/")
                {
                    responseString = "Enter an id please, bad endpoint, Error = " + (int)HttpStatusCode.BadRequest;
                }
            }
            // POST
            else if (request.HttpMethod == "POST" && request.Url.AbsolutePath == "/api/likes")
            {
                using (var reader = new StreamReader(request.InputStream, request.ContentEncoding))
                {
                    string requestBody = await reader.ReadToEndAsync();
                    var data = JsonSerializer.Deserialize<Likes>(requestBody);

                    string result = await HttpPostLike(data.User_Id, data.Map_Id);
                    responseString = result;
                }
            }
            // PUT
            else if (request.HttpMethod == "PUT" && request.Url.PathAndQuery.StartsWith("/api/likes/"))
            {
                string[] parts = request.Url.PathAndQuery.Split('/');
                if (parts.Length == 4 && int.TryParse(parts[3], out int likeId))
                {
                    using (var reader = new StreamReader(request.InputStream, request.ContentEncoding))
                    {
                        string requestBody = await reader.ReadToEndAsync();
                        var data = JsonSerializer.Deserialize<Likes>(requestBody);

                        string result = await HttpPutLikeById(likeId, data.User_Id, data.Map_Id);
                        responseString = result;
                    }
                }
                else
                {
                    responseString = "Invalid like ID, Error = " + (int)HttpStatusCode.BadRequest;
                }
            }
            // PATCH
            else if (request.HttpMethod == "PATCH" && request.Url.PathAndQuery.StartsWith("/api/likes/"))
            {
                string[] parts = request.Url.PathAndQuery.Split('/');
                if (parts.Length == 4 && int.TryParse(parts[3], out int likeId))
                {
                    using (var reader = new StreamReader(request.InputStream, request.ContentEncoding))
                    {
                        string requestBody = await reader.ReadToEndAsync();
                        var data = JsonSerializer.Deserialize<Likes>(requestBody);

                        string result = await HttpPatchLikeById(likeId, data.User_Id, data.Map_Id);
                        responseString = result;
                    }
                }
                else
                {
                    responseString = "Invalid like ID, Error = " + (int)HttpStatusCode.BadRequest;
                }
            }
            // DELETE
            else if (request.HttpMethod == "DELETE" && request.Url.PathAndQuery.StartsWith("/api/likes/"))
            {
                string[] parts = request.Url.PathAndQuery.Split('/');
                if (parts.Length == 4 && int.TryParse(parts[3], out int likeId))
                {
                    string result = await HttpDelLikeById(likeId);
                    responseString = result;
                }
                else
                {
                    responseString = "Invalid like ID, Error = " + (int)HttpStatusCode.BadRequest;
                }
            }

            return responseString;
        }

        private async Task<IEnumerable<Likes>> HttpGetAllLikes()
        {
            List<Likes> likes = new List<Likes>();

            try
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    await connection.OpenAsync();

                    string sqlRequest = "SELECT * FROM likes";

                    using (MySqlCommand command = new MySqlCommand(sqlRequest, connection))
                    {
                        using (MySqlDataReader reader = await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                Likes like = new Likes
                                {
                                    Like_Id = Convert.ToInt32(reader["Like_Id"]),
                                    User_Id = Convert.ToInt32(reader["User_Id"]),
                                    Map_Id = Convert.ToInt32(reader["Map_Id"])
                                };
                                likes.Add(like);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error during likes retrieval: {ex.Message}");
            }

            return likes;
        }

        private async Task<Likes> HttpGetLikeById(int id)
        {
            Likes like = null;

            try
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    await connection.OpenAsync();

                    string sqlRequest = "SELECT * FROM likes WHERE Like_Id = @LikeId";

                    using (MySqlCommand command = new MySqlCommand(sqlRequest, connection))
                    {
                        command.Parameters.AddWithValue("@LikeId", id);

                        using (MySqlDataReader reader = await command.ExecuteReaderAsync())
                        {
                            if (await reader.ReadAsync())
                            {
                                like = new Likes
                                {
                                    Like_Id = Convert.ToInt32(reader["Like_Id"]),
                                    User_Id = Convert.ToInt32(reader["User_Id"]),
                                    Map_Id = Convert.ToInt32(reader["Map_Id"])
                                };
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error during like retrieval: {ex.Message}");
            }

            return like;
        }

        private async Task<string> HttpPostLike(int userId, int mapId)
        {
            try
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    await connection.OpenAsync();

                    string sqlRequest = "INSERT INTO likes (User_Id, Map_Id) VALUES (@UserId, @MapId)";

                    using (MySqlCommand command = new MySqlCommand(sqlRequest, connection))
                    {
                        command.Parameters.AddWithValue("@UserId", userId);
                        command.Parameters.AddWithValue("@MapId", mapId);

                        await command.ExecuteNonQueryAsync();
                    }
                }
            }
            catch (Exception ex)
            {
                return $"Error during like creation: {ex.Message}";
            }

            return "Like created successfully";
        }

        private async Task<string> HttpPutLikeById(int likeId, int userId, int mapId)
        {
            try
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    await connection.OpenAsync();

                    string sqlRequest = "UPDATE likes SET User_Id = @UserId, Map_Id = @MapId WHERE Like_Id = @LikeId";

                    using (MySqlCommand command = new MySqlCommand(sqlRequest, connection))
                    {
                        command.Parameters.AddWithValue("@UserId", userId);
                        command.Parameters.AddWithValue("@MapId", mapId);
                        command.Parameters.AddWithValue("@LikeId", likeId);

                        int rowsAffected = await command.ExecuteNonQueryAsync();
                        if (rowsAffected > 0)
                        {
                            return "Like updated successfully";
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
                return $"Error during like update: {ex.Message}";
            }
        }

        private async Task<string> HttpPatchLikeById(int likeId, int userId, int mapId)
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

                    if (mapId != 0)
                    {
                        updates.Add("Map_Id = @MapId");
                        parameters["@MapId"] = mapId;
                    }

                    if (updates.Count == 0)
                    {
                        return "No fields to update.";
                    }

                    string sqlRequest = "UPDATE likes SET " + string.Join(", ", updates) + " WHERE Like_Id = @LikeId";
                    parameters["@LikeId"] = likeId;

                    using (MySqlCommand command = new MySqlCommand(sqlRequest, connection))
                    {
                        foreach (var param in parameters)
                        {
                            command.Parameters.AddWithValue(param.Key, param.Value);
                        }

                        int rowsAffected = await command.ExecuteNonQueryAsync();
                        if (rowsAffected > 0)
                        {
                            return "Like updated successfully";
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
                return $"Error during like patch update: {ex.Message}";
            }
        }

        private async Task<string> HttpDelLikeById(int id)
        {
            try
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    await connection.OpenAsync();

                    string sqlRequest = "DELETE FROM likes WHERE Like_Id = @LikeId";

                    using (MySqlCommand command = new MySqlCommand(sqlRequest, connection))
                    {
                        command.Parameters.AddWithValue("@LikeId", id);

                        int rowsAffected = await command.ExecuteNonQueryAsync();
                        if (rowsAffected > 0)
                        {
                            return "Like deleted successfully";
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
                return $"Error during like deletion: {ex.Message}";
            }
        }
    }
}
