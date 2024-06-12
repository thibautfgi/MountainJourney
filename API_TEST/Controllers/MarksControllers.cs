using Models;
using MySqlConnector;
using System.Collections.Generic;
using System.Net;
using System.Text.Json;
using Tools;

namespace Controllers
{
    public class MarksController
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

            // GET all marks
            if (request.HttpMethod == "GET" && request.Url.AbsolutePath == "/api/marks")
            {
                var options = new JsonSerializerOptions { WriteIndented = true };
                responseString = JsonSerializer.Serialize(await HttpGetAllMarks(), options);
            }
            // GET mark by id or routes by mark id
            else if (request.HttpMethod == "GET" && request.Url.PathAndQuery.StartsWith("/api/marks/"))
            {
                string[] parts = request.Url.PathAndQuery.Split('/');
                if (parts.Length == 4 && int.TryParse(parts[3], out int id))
                {
                    var options = new JsonSerializerOptions { WriteIndented = true };
                    responseString = JsonSerializer.Serialize(await HttpGetMarkById(id), options);
                    if (responseString == "null")
                    {
                        responseString = "Invalid id, Error = " + (int)HttpStatusCode.BadRequest;
                    }
                }
                else if (parts.Length == 5 && parts[4] == "routes" && int.TryParse(parts[3], out int markId))
                {
                    var options = new JsonSerializerOptions { WriteIndented = true };
                    responseString = JsonSerializer.Serialize(await HttpGetRoutesByMarkId(markId), options);
                    if (responseString == "null")
                    {
                        responseString = "Invalid id, Error = " + (int)HttpStatusCode.BadRequest;
                    }
                }
                else if (parts.Length == 4)
                {
                    string myEndPointString = parts[3];
                    var options = new JsonSerializerOptions { WriteIndented = true };
                    var marksByName = await HttpGetMarksByName(myEndPointString);
                    responseString = JsonSerializer.Serialize(marksByName, options);

                    if (marksByName == null || !marksByName.Any())
                    {
                        responseString = "Invalid name, Error = " + (int)HttpStatusCode.BadRequest;
                    }
                }
                else
                {
                    responseString = "Bad endpoint, Error = " + (int)HttpStatusCode.BadRequest;
                }
            }
            // POST
            else if (request.HttpMethod == "POST" && request.Url.AbsolutePath == "/api/marks")
            {
                using (var reader = new StreamReader(request.InputStream, request.ContentEncoding))
                {
                    string requestBody = await reader.ReadToEndAsync();
                    var data = JsonSerializer.Deserialize<Marks>(requestBody);

                    string result = await HttpPostMark(data.Map_Id, data.Mark_Description, data.Mark_Name, data.Mark_Latitude, data.Mark_Longitude);
                    responseString = result;
                }
            }
            // PUT
            else if (request.HttpMethod == "PUT" && request.Url.PathAndQuery.StartsWith("/api/marks/"))
            {
                string[] parts = request.Url.PathAndQuery.Split('/');
                if (parts.Length == 4 && int.TryParse(parts[3], out int markId))
                {
                    using (var reader = new StreamReader(request.InputStream, request.ContentEncoding))
                    {
                        string requestBody = await reader.ReadToEndAsync();
                        var data = JsonSerializer.Deserialize<Marks>(requestBody);

                        string result = await HttpPutMarkById(markId, data.Map_Id, data.Mark_Description, data.Mark_Name, data.Mark_Latitude, data.Mark_Longitude);
                        responseString = result;
                    }
                }
                else
                {
                    responseString = "Invalid mark ID, Error = " + (int)HttpStatusCode.BadRequest;
                }
            }
            // PATCH
            else if (request.HttpMethod == "PATCH" && request.Url.PathAndQuery.StartsWith("/api/marks/"))
            {
                string[] parts = request.Url.PathAndQuery.Split('/');
                if (parts.Length == 4 && int.TryParse(parts[3], out int markId))
                {
                    using (var reader = new StreamReader(request.InputStream, request.ContentEncoding))
                    {
                        string requestBody = await reader.ReadToEndAsync();
                        var data = JsonSerializer.Deserialize<Marks>(requestBody);

                        string result = await HttpPatchMarkById(markId, data.Map_Id, data.Mark_Description, data.Mark_Name, data.Mark_Latitude, data.Mark_Longitude);
                        responseString = result;
                    }
                }
                else
                {
                    responseString = "Invalid mark ID, Error = " + (int)HttpStatusCode.BadRequest;
                }
            }
            // DELETE
            else if (request.HttpMethod == "DELETE" && request.Url.PathAndQuery.StartsWith("/api/marks/"))
            {
                string[] parts = request.Url.PathAndQuery.Split('/');
                if (parts.Length == 4 && int.TryParse(parts[3], out int markId))
                {
                    string result = await HttpDelMarkById(markId);
                    responseString = result;
                }
                else
                {
                    responseString = "Invalid mark ID, Error = " + (int)HttpStatusCode.BadRequest;
                }
            }
            else
            {
                responseString = "Invalid request method or endpoint, Error = " + (int)HttpStatusCode.BadRequest;
            }

            return responseString;
        }

        private async Task<IEnumerable<Marks>> HttpGetAllMarks()
        {
            List<Marks> marks = new List<Marks>();

            try
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    await connection.OpenAsync();

                    string sqlRequest = "SELECT * FROM marks";

                    using (MySqlCommand command = new MySqlCommand(sqlRequest, connection))
                    {
                        using (MySqlDataReader reader = await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                Marks mark = new Marks
                                {
                                    Mark_Id = Convert.ToInt32(reader["Mark_Id"]),
                                    Map_Id = Convert.ToInt32(reader["Map_Id"]),
                                    Mark_Description = reader["Mark_Description"].ToString(),
                                    Mark_Name = reader["Mark_Name"].ToString(),
                                    Mark_Latitude = Convert.ToSingle(reader["Mark_Latitude"]),
                                    Mark_Longitude = Convert.ToSingle(reader["Mark_Longitude"])
                                };
                                marks.Add(mark);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error during marks retrieval: {ex.Message}");
            }

            return marks;
        }

        private async Task<Marks> HttpGetMarkById(int id)
        {
            Marks mark = null;

            try
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    await connection.OpenAsync();

                    string sqlRequest = "SELECT * FROM marks WHERE Mark_Id = @MarkId";

                    using (MySqlCommand command = new MySqlCommand(sqlRequest, connection))
                    {
                        command.Parameters.AddWithValue("@MarkId", id);

                        using (MySqlDataReader reader = await command.ExecuteReaderAsync())
                        {
                            if (await reader.ReadAsync())
                            {
                                mark = new Marks
                                {
                                    Mark_Id = Convert.ToInt32(reader["Mark_Id"]),
                                    Map_Id = Convert.ToInt32(reader["Map_Id"]),
                                    Mark_Description = reader["Mark_Description"].ToString(),
                                    Mark_Name = reader["Mark_Name"].ToString(),
                                    Mark_Latitude = Convert.ToSingle(reader["Mark_Latitude"]),
                                    Mark_Longitude = Convert.ToSingle(reader["Mark_Longitude"])
                                };
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error during mark retrieval: {ex.Message}");
            }

            return mark;
        }

        private async Task<IEnumerable<Marks>> HttpGetMarksByName(string name)
        {
            List<Marks> marks = new List<Marks>();

            try
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    await connection.OpenAsync();

                    string sqlRequest = "SELECT * FROM marks WHERE Mark_Name LIKE @MarkName";

                    using (MySqlCommand command = new MySqlCommand(sqlRequest, connection))
                    {
                        command.Parameters.AddWithValue("@MarkName", "%" + name + "%");

                        using (MySqlDataReader reader = await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                Marks mark = new Marks
                                {
                                    Mark_Id = Convert.ToInt32(reader["Mark_Id"]),
                                    Map_Id = Convert.ToInt32(reader["Map_Id"]),
                                    Mark_Description = reader["Mark_Description"].ToString(),
                                    Mark_Name = reader["Mark_Name"].ToString(),
                                    Mark_Latitude = Convert.ToSingle(reader["Mark_Latitude"]),
                                    Mark_Longitude = Convert.ToSingle(reader["Mark_Longitude"])
                                };
                                marks.Add(mark);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error during marks retrieval by name: {ex.Message}");
            }

            return marks;
        }

        private async Task<IEnumerable<Routes>> HttpGetRoutesByMarkId(int id)
        {
            List<Routes> routes = new List<Routes>();

            try
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    await connection.OpenAsync();

                    string sqlRequest = "SELECT * FROM routes WHERE Mark_Start = @MarkId OR Mark_End = @MarkId";

                    using (MySqlCommand command = new MySqlCommand(sqlRequest, connection))
                    {
                        command.Parameters.AddWithValue("@MarkId", id);

                        using (MySqlDataReader reader = await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                Routes route = new Routes
                                {
                                    Route_Id = Convert.ToInt32(reader["Route_Id"]),
                                    Mark_Start = Convert.ToInt32(reader["Mark_Start"]),
                                    Mark_End = Convert.ToInt32(reader["Mark_End"]),
                                    Route_Name = reader["Route_Name"].ToString(),
                                    Route_Description = reader["Route_Description"].ToString(),
                                    Route_Distance = reader["Route_Distance"] != DBNull.Value ? (float?)Convert.ToSingle(reader["Route_Distance"]) : null
                                };
                                routes.Add(route);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error during routes retrieval: {ex.Message}");
            }

            return routes;
        }

        private async Task<string> HttpPostMark(int mapId, string description, string name, float latitude, float longitude)
        {
            try
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    await connection.OpenAsync();

                    string sqlRequest = @"INSERT INTO marks (Map_Id, Mark_Description, Mark_Name, Mark_Latitude, Mark_Longitude)
                                          VALUES (@MapId, @Description, @Name, @Latitude, @Longitude)";

                    using (MySqlCommand command = new MySqlCommand(sqlRequest, connection))
                    {
                        command.Parameters.AddWithValue("@MapId", mapId);
                        command.Parameters.AddWithValue("@Description", description);
                        command.Parameters.AddWithValue("@Name", name);
                        command.Parameters.AddWithValue("@Latitude", latitude);
                        command.Parameters.AddWithValue("@Longitude", longitude);

                        await command.ExecuteNonQueryAsync();
                    }
                }
            }
            catch (Exception ex)
            {
                return $"Error during mark creation: {ex.Message}";
            }

            return "Mark created successfully";
        }

        private async Task<string> HttpPutMarkById(int markId, int mapId, string description, string name, float latitude, float longitude)
        {
            try
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    await connection.OpenAsync();

                    string sqlRequest = "UPDATE marks SET Map_Id = @MapId, Mark_Description = @Description, Mark_Name = @Name, Mark_Latitude = @Latitude, Mark_Longitude = @Longitude WHERE Mark_Id = @MarkId";

                    using (MySqlCommand command = new MySqlCommand(sqlRequest, connection))
                    {
                        command.Parameters.AddWithValue("@MapId", mapId);
                        command.Parameters.AddWithValue("@Description", description);
                        command.Parameters.AddWithValue("@Name", name);
                        command.Parameters.AddWithValue("@Latitude", latitude);
                        command.Parameters.AddWithValue("@Longitude", longitude);
                        command.Parameters.AddWithValue("@MarkId", markId);

                        int rowsAffected = await command.ExecuteNonQueryAsync();
                        if (rowsAffected > 0)
                        {
                            return "Mark updated successfully";
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
                return $"Error during mark update: {ex.Message}";
            }
        }

        private async Task<string> HttpPatchMarkById(int markId, int mapId, string description, string name, float latitude, float longitude)
        {
            try
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    await connection.OpenAsync();

                    var updates = new List<string>();
                    var parameters = new Dictionary<string, object>();

                    if (mapId != 0)
                    {
                        updates.Add("Map_Id = @MapId");
                        parameters["@MapId"] = mapId;
                    }

                    if (!string.IsNullOrEmpty(description))
                    {
                        updates.Add("Mark_Description = @Description");
                        parameters["@Description"] = description;
                    }

                    if (!string.IsNullOrEmpty(name))
                    {
                        updates.Add("Mark_Name = @Name");
                        parameters["@Name"] = name;
                    }

                    if (latitude != 0)
                    {
                        updates.Add("Mark_Latitude = @Latitude");
                        parameters["@Latitude"] = latitude;
                    }

                    if (longitude != 0)
                    {
                        updates.Add("Mark_Longitude = @Longitude");
                        parameters["@Longitude"] = longitude;
                    }

                    if (updates.Count == 0)
                    {
                        return "No fields to update.";
                    }

                    string sqlRequest = "UPDATE marks SET " + string.Join(", ", updates) + " WHERE Mark_Id = @MarkId";
                    parameters["@MarkId"] = markId;

                    using (MySqlCommand command = new MySqlCommand(sqlRequest, connection))
                    {
                        foreach (var param in parameters)
                        {
                            command.Parameters.AddWithValue(param.Key, param.Value);
                        }

                        int rowsAffected = await command.ExecuteNonQueryAsync();
                        if (rowsAffected > 0)
                        {
                            return "Mark updated successfully";
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
                return $"Error during mark patch update: {ex.Message}";
            }
        }

        private async Task<string> HttpDelMarkById(int id)
        {
            try
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    await connection.OpenAsync();

                    string sqlRequest = "DELETE FROM marks WHERE Mark_Id = @MarkId";

                    using (MySqlCommand command = new MySqlCommand(sqlRequest, connection))
                    {
                        command.Parameters.AddWithValue("@MarkId", id);

                        int rowsAffected = await command.ExecuteNonQueryAsync();
                        if (rowsAffected > 0)
                        {
                            return "Mark deleted successfully";
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
                return $"Error during mark deletion: {ex.Message}";
            }
        }
    }
}
