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
    public class CommentsController
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

            // GET all comments
            if (request.HttpMethod == "GET" && request.Url.PathAndQuery == "/api/comments")
            {
                var options = new JsonSerializerOptions { WriteIndented = true };
                responseString = JsonSerializer.Serialize(await HttpGetAllComments(), options);
            }
            // GET comments by id
            else if (request.HttpMethod == "GET" && request.Url.PathAndQuery.StartsWith("/api/comments/"))
            {
                string[] parts = request.Url.PathAndQuery.Split('/');
                if (parts.Length == 4 && int.TryParse(parts[3], out int id))
                {
                    var options = new JsonSerializerOptions { WriteIndented = true };
                    responseString = JsonSerializer.Serialize(await HttpGetCommentById(id), options);
                    if (responseString == "null")
                    {
                        responseString = "Invalid id, Error = " + (int)HttpStatusCode.BadRequest;
                    }
                }
                else if (parts.Length > 4)
                {
                    responseString = "bad endpoint, Error = " + (int)HttpStatusCode.BadRequest;
                }
                else if (request.Url.PathAndQuery == "/api/comments/")
                {
                    responseString = "Enter an id please, bad endpoint, Error = " + (int)HttpStatusCode.BadRequest;
                }
            }
            // POST
            else if (request.HttpMethod == "POST" && request.Url.AbsolutePath == "/api/comments")
            {
                using (var reader = new StreamReader(request.InputStream, request.ContentEncoding))
                {
                    string requestBody = await reader.ReadToEndAsync();
                    var data = JsonSerializer.Deserialize<Comments>(requestBody);

                    string result = await HttpPostComment(data.User_Id, data.Map_Id, data.Comment_Content, data.Comment_Date);
                    responseString = result;
                }
            }
            // PUT
            else if (request.HttpMethod == "PUT" && request.Url.PathAndQuery.StartsWith("/api/comments/"))
            {
                string[] parts = request.Url.PathAndQuery.Split('/');
                if (parts.Length == 4 && int.TryParse(parts[3], out int commentId))
                {
                    using (var reader = new StreamReader(request.InputStream, request.ContentEncoding))
                    {
                        string requestBody = await reader.ReadToEndAsync();
                        var data = JsonSerializer.Deserialize<Comments>(requestBody);

                        string result = await HttpPutCommentById(commentId, data.User_Id, data.Map_Id, data.Comment_Content, data.Comment_Date);
                        responseString = result;
                    }
                }
                else
                {
                    responseString = "Invalid comment ID, Error = " + (int)HttpStatusCode.BadRequest;
                }
            }
            // PATCH
            else if (request.HttpMethod == "PATCH" && request.Url.PathAndQuery.StartsWith("/api/comments/"))
            {
                string[] parts = request.Url.PathAndQuery.Split('/');
                if (parts.Length == 4 && int.TryParse(parts[3], out int commentId))
                {
                    using (var reader = new StreamReader(request.InputStream, request.ContentEncoding))
                    {
                        string requestBody = await reader.ReadToEndAsync();
                        var data = JsonSerializer.Deserialize<Comments>(requestBody);

                        string result = await HttpPatchCommentById(commentId, data.User_Id, data.Map_Id, data.Comment_Content, data.Comment_Date);
                        responseString = result;
                    }
                }
                else
                {
                    responseString = "Invalid comment ID, Error = " + (int)HttpStatusCode.BadRequest;
                }
            }
            // DELETE
            else if (request.HttpMethod == "DELETE" && request.Url.PathAndQuery.StartsWith("/api/comments/"))
            {
                string[] parts = request.Url.PathAndQuery.Split('/');
                if (parts.Length == 4 && int.TryParse(parts[3], out int commentId))
                {
                    string result = await HttpDelCommentById(commentId);
                    responseString = result;
                }
                else
                {
                    responseString = "Invalid comment ID, Error = " + (int)HttpStatusCode.BadRequest;
                }
            }

            return responseString;
        }




        private async Task<IEnumerable<Comments>> HttpGetAllComments()
        {
            List<Comments> comments = new List<Comments>();

            try
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    await connection.OpenAsync();

                    string sqlRequest = "SELECT * FROM comments";

                    using (MySqlCommand command = new MySqlCommand(sqlRequest, connection))
                    {
                        using (MySqlDataReader reader = await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                Comments comment = new Comments
                                {
                                    Comment_Id = Convert.ToInt32(reader["Comment_Id"]),
                                    User_Id = Convert.ToInt32(reader["User_Id"]),
                                    Map_Id = Convert.ToInt32(reader["Map_Id"]),
                                    Comment_Content = reader["Comment_Content"].ToString(),
                                    Comment_Date = reader["Comment_Date"].ToString()
                                };
                                comments.Add(comment);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error during comments retrieval: {ex.Message}");
            }

            return comments;
        }

        private async Task<Comments> HttpGetCommentById(int id)
        {
            Comments comment = null;

            try
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    await connection.OpenAsync();

                    string sqlRequest = "SELECT * FROM comments WHERE Comment_Id = @CommentId";

                    using (MySqlCommand command = new MySqlCommand(sqlRequest, connection))
                    {
                        command.Parameters.AddWithValue("@CommentId", id);

                        using (MySqlDataReader reader = await command.ExecuteReaderAsync())
                        {
                            if (await reader.ReadAsync())
                            {
                                comment = new Comments
                                {
                                    Comment_Id = Convert.ToInt32(reader["Comment_Id"]),
                                    User_Id = Convert.ToInt32(reader["User_Id"]),
                                    Map_Id = Convert.ToInt32(reader["Map_Id"]),
                                    Comment_Content = reader["Comment_Content"].ToString(),
                                    Comment_Date = reader["Comment_Date"].ToString()
                                };
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error during comment retrieval: {ex.Message}");
            }

            return comment;
        }

        private async Task<string> HttpPostComment(int userId, int mapId, string content, string date)
        {
            try
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    await connection.OpenAsync();

                    string sqlRequest = "INSERT INTO comments (User_Id, Map_Id, Comment_Content, Comment_Date) VALUES (@UserId, @MapId, @Content, @Date)";

                    using (MySqlCommand command = new MySqlCommand(sqlRequest, connection))
                    {
                        command.Parameters.AddWithValue("@UserId", userId);
                        command.Parameters.AddWithValue("@MapId", mapId);
                        command.Parameters.AddWithValue("@Content", content);
                        command.Parameters.AddWithValue("@Date", date);

                        await command.ExecuteNonQueryAsync();
                    }
                }
            }
            catch (Exception ex)
            {
                return $"Error during comment creation: {ex.Message}";
            }

            return "Comment created successfully";
        }

               private async Task<string> HttpPutCommentById(int commentId, int userId, int mapId, string content, string date)
        {
            try
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    await connection.OpenAsync();

                    string sqlRequest = "UPDATE comments SET User_Id = @UserId, Map_Id = @MapId, Comment_Content = @Content, Comment_Date = @Date WHERE Comment_Id = @CommentId";

                    using (MySqlCommand command = new MySqlCommand(sqlRequest, connection))
                    {
                        command.Parameters.AddWithValue("@UserId", userId);
                        command.Parameters.AddWithValue("@MapId", mapId);
                        command.Parameters.AddWithValue("@Content", content);
                        command.Parameters.AddWithValue("@Date", date);
                        command.Parameters.AddWithValue("@CommentId", commentId);

                        int rowsAffected = await command.ExecuteNonQueryAsync();
                        if (rowsAffected > 0)
                        {
                            return "Comment updated successfully";
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
                return $"Error during comment update: {ex.Message}";
            }
        }

        private async Task<string> HttpPatchCommentById(int commentId, int userId, int mapId, string content, string date)
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

                    if (!string.IsNullOrEmpty(content))
                    {
                        updates.Add("Comment_Content = @Content");
                        parameters["@Content"] = content;
                    }

                    if (!string.IsNullOrEmpty(date))
                    {
                        updates.Add("Comment_Date = @Date");
                        parameters["@Date"] = date;
                    }

                    if (updates.Count == 0)
                    {
                        return "No fields to update.";
                    }

                    string sqlRequest = "UPDATE comments SET " + string.Join(", ", updates) + " WHERE Comment_Id = @CommentId";
                    parameters["@CommentId"] = commentId;

                    using (MySqlCommand command = new MySqlCommand(sqlRequest, connection))
                    {
                        foreach (var param in parameters)
                        {
                            command.Parameters.AddWithValue(param.Key, param.Value);
                        }

                        int rowsAffected = await command.ExecuteNonQueryAsync();
                        if (rowsAffected > 0)
                        {
                            return "Comment updated successfully";
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
                return $"Error during comment patch update: {ex.Message}";
            }
        }

        private async Task<string> HttpDelCommentById(int id)
        {
            try
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    await connection.OpenAsync();

                    string sqlRequest = "DELETE FROM comments WHERE Comment_Id = @CommentId";

                    using (MySqlCommand command = new MySqlCommand(sqlRequest, connection))
                    {
                        command.Parameters.AddWithValue("@CommentId", id);

                        int rowsAffected = await command.ExecuteNonQueryAsync();
                        if (rowsAffected > 0)
                        {
                            return "Comment deleted successfully";
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
                return $"Error during comment deletion: {ex.Message}";
            }
        }
    }
}
