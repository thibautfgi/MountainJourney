using Models;
using MySqlConnector;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;
using Tools;

namespace Controllers
{
    public class MapsController
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

            // GET all maps
            if (request.HttpMethod == "GET" && request.Url.PathAndQuery == "/api/maps")
            {
                var options = new JsonSerializerOptions { WriteIndented = true };
                responseString = JsonSerializer.Serialize(await HttpGetAllMaps(), options);
            }
            // GET map by ID or map name
            else if (request.HttpMethod == "GET" && request.Url.PathAndQuery.StartsWith("/api/maps/"))
            {
                string[] parts = request.Url.PathAndQuery.Split('/');
                if (parts.Length == 4 && int.TryParse(parts[3], out int id))
                {
                    var options = new JsonSerializerOptions { WriteIndented = true };
                    responseString = JsonSerializer.Serialize(await HttpGetMapById(id), options);
                }
                else if (parts.Length == 4)
                {
                    string mapName = parts[3];
                    var options = new JsonSerializerOptions { WriteIndented = true };
                    var maps = await HttpGetMapsByName(mapName);
                    responseString = maps.Any()
                        ? JsonSerializer.Serialize(maps, options)
                        : "Invalid name or no maps found, Error = " + (int)HttpStatusCode.BadRequest;
                }
                // GET likes by map id
                else if (parts.Length == 5 && parts[4] == "likes" && int.TryParse(parts[3], out int mapId))
                {
                    var options = new JsonSerializerOptions { WriteIndented = true };
                    var likes = await HttpGetLikesByMapId(mapId);
                    responseString = likes.Any()
                        ? JsonSerializer.Serialize(likes, options)
                        : "Invalid id or no likes found, Error = " + (int)HttpStatusCode.BadRequest;
                }
                // GET marks by map id
                else if (parts.Length == 5 && parts[4] == "marks" && int.TryParse(parts[3], out int mapIdTwo))
                {
                    var options = new JsonSerializerOptions { WriteIndented = true };
                    var marks = await HttpGetMarksByMapId(mapIdTwo);
                    responseString = marks.Any()
                        ? JsonSerializer.Serialize(marks, options)
                        : "Invalid id or no marks found, Error = " + (int)HttpStatusCode.BadRequest;
                }
                // GET routes by map id
                else if (parts.Length == 5 && parts[4] == "routes" && int.TryParse(parts[3], out int mapIdThree))
                {
                    var options = new JsonSerializerOptions { WriteIndented = true };
                    var routes = await HttpGetRoutesByMapId(mapIdThree);
                    responseString = routes.Any()
                        ? JsonSerializer.Serialize(routes, options)
                        : "Invalid id or no routes found, Error = " + (int)HttpStatusCode.BadRequest;
                }
                // GET comments by map id
                else if (parts.Length == 5 && parts[4] == "comments" && int.TryParse(parts[3], out int mapIdFour))
                {
                    var options = new JsonSerializerOptions { WriteIndented = true };
                    var comments = await HttpGetCommentsByMapId(mapIdFour);
                    responseString = comments.Any()
                        ? JsonSerializer.Serialize(comments, options)
                        : "Invalid id or no comments found, Error = " + (int)HttpStatusCode.BadRequest;
                }
                else if (request.Url.PathAndQuery == "/api/maps/")
                {
                    responseString = "Enter a map id please, bad endpoint, Error = " + (int)HttpStatusCode.BadRequest;
                }
                else
                {
                    responseString = "Bad endpoint, Error = " + (int)HttpStatusCode.BadRequest;
                }
            }
            // POST map
            else if (request.HttpMethod == "POST" && request.Url.PathAndQuery == "/api/maps")
            {
                using (var reader = new StreamReader(request.InputStream, request.ContentEncoding))
                {
                    string requestBody = await reader.ReadToEndAsync();
                    var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                    var data = JsonSerializer.Deserialize<Maps>(requestBody, options);

                    if (data == null)
                    {
                        responseString = "Invalid map data, Error = " + (int)HttpStatusCode.BadRequest;
                    }
                    else
                    {
                        responseString = await HttpPostNewMap(data);
                    }
                }
            }
            // PUT map
            else if (request.HttpMethod == "PUT" && request.Url.PathAndQuery.StartsWith("/api/maps/"))
            {
                string[] parts = request.Url.PathAndQuery.Split('/');
                if (parts.Length == 4 && int.TryParse(parts[3], out int id))
                {
                    using (var reader = new StreamReader(request.InputStream, request.ContentEncoding))
                    {
                        string requestBody = await reader.ReadToEndAsync();
                        var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true, WriteIndented = true };
                        var data = JsonSerializer.Deserialize<Maps>(requestBody, options);

                        if (data == null)
                        {
                            responseString = "Invalid map data, Error = " + (int)HttpStatusCode.BadRequest;
                        }
                        else
                        {
                            responseString = JsonSerializer.Serialize(await HttpPutMapById(id, data), options);
                        }
                    }
                }
                else
                {
                    responseString = "Bad endpoint, Error = " + (int)HttpStatusCode.BadRequest;
                }
            }
            // DELETE map
            else if (request.HttpMethod == "DELETE" && request.Url.PathAndQuery.StartsWith("/api/maps/"))
            {
                string[] parts = request.Url.PathAndQuery.Split('/');
                if (parts.Length == 4 && int.TryParse(parts[3], out int id))
                {
                    responseString = JsonSerializer.Serialize(await HttpDelMapById(id), new JsonSerializerOptions { WriteIndented = true });
                }
                else
                {
                    responseString = "Bad endpoint, Error = " + (int)HttpStatusCode.BadRequest;
                }
            }
            // PATCH map
            else if (request.HttpMethod == "PATCH" && request.Url.PathAndQuery.StartsWith("/api/maps/"))
            {
                string[] parts = request.Url.PathAndQuery.Split('/');
                if (parts.Length == 4 && int.TryParse(parts[3], out int id))
                {
                    using (var reader = new StreamReader(request.InputStream, request.ContentEncoding))
                    {
                        string requestBody = await reader.ReadToEndAsync();
                        var data = JsonSerializer.Deserialize<Maps>(requestBody);
                        responseString = JsonSerializer.Serialize(await HttpPatchMapById(id, data), new JsonSerializerOptions { WriteIndented = true });
                    }
                }
                else
                {
                    responseString = "Bad endpoint, Error = " + (int)HttpStatusCode.BadRequest;
                }
            }

            return responseString;
        }

        private async Task<IEnumerable<Maps>> HttpGetAllMaps()
        {
            List<Maps> maps = new List<Maps>();
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                await connection.OpenAsync();
                string sqlRequest = "SELECT * FROM maps";
                using (MySqlCommand command = new MySqlCommand(sqlRequest, connection))
                {
                    using (MySqlDataReader reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            Maps map = new Maps
                            {
                                Map_Id = Convert.ToInt32(reader["Map_Id"]),
                                User_Id = Convert.ToInt32(reader["User_Id"]),
                                Map_Name = reader["Map_Name"].ToString(),
                                Map_Description = reader["Map_Description"].ToString(),
                                Map_LikeNumber = reader["Map_LikeNumber"] != DBNull.Value ? (int?)Convert.ToInt32(reader["Map_LikeNumber"]) : null,
                                Map_NumberCommentary = reader["Map_NumberCommentary"] != DBNull.Value ? (int?)Convert.ToInt32(reader["Map_NumberCommentary"]) : null,
                                Map_TravelTime = reader["Map_TravelTime"] != DBNull.Value ? (float?)Convert.ToSingle(reader["Map_TravelTime"]) : null,
                                Map_TotalDistance = reader["Map_TotalDistance"] != DBNull.Value ? (float?)Convert.ToSingle(reader["Map_TotalDistance"]) : null,
                                Map_Image = reader["Map_Image"].ToString(),
                                Map_Rating = reader["Map_Rating"] != DBNull.Value ? (int?)Convert.ToInt32(reader["Map_Rating"]) : null
                            };
                            maps.Add(map);
                        }
                    }
                }
            }
            return maps;
        }

        private async Task<Maps> HttpGetMapById(int id)
        {
            Maps map = null;
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                await connection.OpenAsync();
                string sqlRequest = "SELECT * FROM maps WHERE Map_Id = @MapId";
                using (MySqlCommand command = new MySqlCommand(sqlRequest, connection))
                {
                    command.Parameters.AddWithValue("@MapId", id);
                    using (MySqlDataReader reader = await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            map = new Maps
                            {
                                Map_Id = Convert.ToInt32(reader["Map_Id"]),
                                User_Id = Convert.ToInt32(reader["User_Id"]),
                                Map_Name = reader["Map_Name"].ToString(),
                                Map_Description = reader["Map_Description"].ToString(),
                                Map_LikeNumber = reader["Map_LikeNumber"] != DBNull.Value ? (int?)Convert.ToInt32(reader["Map_LikeNumber"]) : null,
                                Map_NumberCommentary = reader["Map_NumberCommentary"] != DBNull.Value ? (int?)Convert.ToInt32(reader["Map_NumberCommentary"]) : null,
                                Map_TravelTime = reader["Map_TravelTime"] != DBNull.Value ? (float?)Convert.ToSingle(reader["Map_TravelTime"]) : null,
                                Map_TotalDistance = reader["Map_TotalDistance"] != DBNull.Value ? (float?)Convert.ToSingle(reader["Map_TotalDistance"]) : null,
                                Map_Image = reader["Map_Image"].ToString(),
                                Map_Rating = reader["Map_Rating"] != DBNull.Value ? (int?)Convert.ToInt32(reader["Map_Rating"]) : null
                            };
                        }
                    }
                }
            }
            return map;
        }

        private async Task<IEnumerable<Likes>> HttpGetLikesByMapId(int id)
        {
            List<Likes> likes = new List<Likes>();
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                await connection.OpenAsync();
                string sqlRequest = "SELECT * FROM likes WHERE Map_Id = @MapId";
                using (MySqlCommand command = new MySqlCommand(sqlRequest, connection))
                {
                    command.Parameters.AddWithValue("@MapId", id);
                    using (MySqlDataReader reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            Likes like = new Likes
                            {
                                Like_Id = Convert.ToInt32(reader["Like_Id"]), // Correctly fetch Like_Id
                                User_Id = Convert.ToInt32(reader["User_Id"]),
                                Map_Id = Convert.ToInt32(reader["Map_Id"])
                            };
                            likes.Add(like);
                        }
                    }
                }
            }
            return likes;
        }


        private async Task<IEnumerable<Marks>> HttpGetMarksByMapId(int id)
        {
            List<Marks> marks = new List<Marks>();
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                await connection.OpenAsync();
                string sqlRequest = "SELECT * FROM marks WHERE Map_Id = @MapId";
                using (MySqlCommand command = new MySqlCommand(sqlRequest, connection))
                {
                    command.Parameters.AddWithValue("@MapId", id);
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
            return marks;
        }

        private async Task<IEnumerable<Routes>> HttpGetRoutesByMapId(int id)
        {
            List<Routes> routes = new List<Routes>();
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                await connection.OpenAsync();
                string sqlRequest = "SELECT * FROM routes WHERE Mark_Start IN (SELECT Mark_Id FROM marks WHERE Map_Id = @MapId)";
                using (MySqlCommand command = new MySqlCommand(sqlRequest, connection))
                {
                    command.Parameters.AddWithValue("@MapId", id);
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
            return routes;
        }

        private async Task<IEnumerable<Comments>> HttpGetCommentsByMapId(int id)
        {
            List<Comments> comments = new List<Comments>();
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                await connection.OpenAsync();
                string sqlRequest = "SELECT * FROM comments WHERE Map_Id = @MapId";
                using (MySqlCommand command = new MySqlCommand(sqlRequest, connection))
                {
                    command.Parameters.AddWithValue("@MapId", id);
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
            return comments;
        }

        private async Task<IEnumerable<Maps>> HttpGetMapsByName(string name)
        {
            List<Maps> maps = new List<Maps>();
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                await connection.OpenAsync();
                string sqlRequest = "SELECT * FROM maps WHERE Map_Name LIKE @MapName";
                using (MySqlCommand command = new MySqlCommand(sqlRequest, connection))
                {
                    command.Parameters.AddWithValue("@MapName", "%" + name + "%");
                    using (MySqlDataReader reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            Maps map = new Maps
                            {
                                Map_Id = Convert.ToInt32(reader["Map_Id"]),
                                User_Id = Convert.ToInt32(reader["User_Id"]),
                                Map_Name = reader["Map_Name"].ToString(),
                                Map_Description = reader["Map_Description"].ToString(),
                                Map_LikeNumber = reader["Map_LikeNumber"] != DBNull.Value ? (int?)Convert.ToInt32(reader["Map_LikeNumber"]) : null,
                                Map_NumberCommentary = reader["Map_NumberCommentary"] != DBNull.Value ? (int?)Convert.ToInt32(reader["Map_NumberCommentary"]) : null,
                                Map_TravelTime = reader["Map_TravelTime"] != DBNull.Value ? (float?)Convert.ToSingle(reader["Map_TravelTime"]) : null,
                                Map_TotalDistance = reader["Map_TotalDistance"] != DBNull.Value ? (float?)Convert.ToSingle(reader["Map_TotalDistance"]) : null,
                                Map_Image = reader["Map_Image"].ToString(),
                                Map_Rating = reader["Map_Rating"] != DBNull.Value ? (int?)Convert.ToInt32(reader["Map_Rating"]) : null
                            };
                            maps.Add(map);
                        }
                    }
                }
            }
            return maps;
        }

        private async Task<string> HttpPostNewMap(Maps map)
        {
            try
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    await connection.OpenAsync();
                    string sqlRequest = "INSERT INTO maps (User_Id, Map_Name, Map_Description, Map_LikeNumber, Map_NumberCommentary, Map_TravelTime, Map_TotalDistance, Map_Image, Map_Rating) VALUES (@UserId, @MapName, @MapDescription, @MapLikeNumber, @MapNumberCommentary, @MapTravelTime, @MapTotalDistance, @MapImage, @MapRating)";
                    using (MySqlCommand command = new MySqlCommand(sqlRequest, connection))
                    {
                        command.Parameters.AddWithValue("@UserId", map.User_Id);
                        command.Parameters.AddWithValue("@MapName", map.Map_Name);
                        command.Parameters.AddWithValue("@MapDescription", map.Map_Description);
                        command.Parameters.AddWithValue("@MapLikeNumber", map.Map_LikeNumber);
                        command.Parameters.AddWithValue("@MapNumberCommentary", map.Map_NumberCommentary);
                        command.Parameters.AddWithValue("@MapTravelTime", map.Map_TravelTime);
                        command.Parameters.AddWithValue("@MapTotalDistance", map.Map_TotalDistance);
                        command.Parameters.AddWithValue("@MapImage", map.Map_Image);
                        command.Parameters.AddWithValue("@MapRating", map.Map_Rating);

                        int rowsAffected = await command.ExecuteNonQueryAsync();
                        if (rowsAffected > 0)
                        {
                            return "Map successfully created, Status = " + (int)HttpStatusCode.Created;
                        }
                        else
                        {
                            return "Failed to create map, Error = " + (int)HttpStatusCode.InternalServerError;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error during map creation: {ex.Message}");
                return "Error during map creation: " + ex.Message;
            }
        }

        private async Task<string> HttpPutMapById(int id, Maps map)
        {
            try
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    await connection.OpenAsync();
                    string sqlRequest = "UPDATE maps SET User_Id = @UserId, Map_Name = @MapName, Map_Description = @MapDescription, Map_LikeNumber = @MapLikeNumber, Map_NumberCommentary = @MapNumberCommentary, Map_TravelTime = @MapTravelTime, Map_TotalDistance = @MapTotalDistance, Map_Image = @MapImage, Map_Rating = @MapRating WHERE Map_Id = @MapId";
                    using (MySqlCommand command = new MySqlCommand(sqlRequest, connection))
                    {
                        command.Parameters.AddWithValue("@MapId", id);
                        command.Parameters.AddWithValue("@UserId", map.User_Id);
                        command.Parameters.AddWithValue("@MapName", map.Map_Name);
                        command.Parameters.AddWithValue("@MapDescription", map.Map_Description);
                        command.Parameters.AddWithValue("@MapLikeNumber", map.Map_LikeNumber);
                        command.Parameters.AddWithValue("@MapNumberCommentary", map.Map_NumberCommentary);
                        command.Parameters.AddWithValue("@MapTravelTime", map.Map_TravelTime);
                        command.Parameters.AddWithValue("@MapTotalDistance", map.Map_TotalDistance);
                        command.Parameters.AddWithValue("@MapImage", map.Map_Image);
                        command.Parameters.AddWithValue("@MapRating", map.Map_Rating);

                        int rowsAffected = await command.ExecuteNonQueryAsync();
                        return rowsAffected > 0
                            ? "Map successfully updated, Status = " + (int)HttpStatusCode.OK
                            : "Failed to update map, Error = " + (int)HttpStatusCode.InternalServerError;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error during map update: {ex.Message}");
                return "Error during map update: " + ex.Message;
            }
        }

        private async Task<string> HttpPatchMapById(int id, Maps map)
        {
            try
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    await connection.OpenAsync();
                    string sqlRequest = "UPDATE maps SET ";

                    List<string> updates = new List<string>();
                    if (!string.IsNullOrEmpty(map.Map_Name))
                    {
                        updates.Add("Map_Name = @MapName");
                    }
                    if (!string.IsNullOrEmpty(map.Map_Description))
                    {
                        updates.Add("Map_Description = @MapDescription");
                    }
                    if (map.Map_LikeNumber.HasValue)
                    {
                        updates.Add("Map_LikeNumber = @MapLikeNumber");
                    }
                    if (map.Map_NumberCommentary.HasValue)
                    {
                        updates.Add("Map_NumberCommentary = @MapNumberCommentary");
                    }
                    if (map.Map_TravelTime.HasValue)
                    {
                        updates.Add("Map_TravelTime = @MapTravelTime");
                    }
                    if (map.Map_TotalDistance.HasValue)
                    {
                        updates.Add("Map_TotalDistance = @MapTotalDistance");
                    }
                    if (!string.IsNullOrEmpty(map.Map_Image))
                    {
                        updates.Add("Map_Image = @MapImage");
                    }
                    if (map.Map_Rating.HasValue)
                    {
                        updates.Add("Map_Rating = @MapRating");
                    }

                    sqlRequest += string.Join(", ", updates);
                    sqlRequest += " WHERE Map_Id = @MapId";

                    using (MySqlCommand command = new MySqlCommand(sqlRequest, connection))
                    {
                        command.Parameters.AddWithValue("@MapId", id);
                        command.Parameters.AddWithValue("@MapName", map.Map_Name);
                        command.Parameters.AddWithValue("@MapDescription", map.Map_Description);
                        command.Parameters.AddWithValue("@MapLikeNumber", map.Map_LikeNumber);
                        command.Parameters.AddWithValue("@MapNumberCommentary", map.Map_NumberCommentary);
                        command.Parameters.AddWithValue("@MapTravelTime", map.Map_TravelTime);
                        command.Parameters.AddWithValue("@MapTotalDistance", map.Map_TotalDistance);
                        command.Parameters.AddWithValue("@MapImage", map.Map_Image);
                        command.Parameters.AddWithValue("@MapRating", map.Map_Rating);

                        int rowsAffected = await command.ExecuteNonQueryAsync();
                        return rowsAffected > 0
                            ? "Map successfully updated"
                            : "Invalid id or no rows affected.";
                    }
                }
            }
            catch (Exception ex)
            {
                return $"Error during PATCH: {ex.Message}";
            }
        }

        private async Task<string> HttpDelMapById(int id)
        {
            try
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    await connection.OpenAsync();
                    string sqlRequest = "DELETE FROM maps WHERE Map_Id = @MapId";
                    using (MySqlCommand command = new MySqlCommand(sqlRequest, connection))
                    {
                        command.Parameters.AddWithValue("@MapId", id);
                        int rowsAffected = await command.ExecuteNonQueryAsync();
                        return rowsAffected > 0
                            ? "Map successfully deleted, Status = " + (int)HttpStatusCode.OK
                            : "Failed to delete map, Error = " + (int)HttpStatusCode.NotFound;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error during map deletion: {ex.Message}");
                return "Error during map deletion: " + ex.Message;
            }
        }
    }
}
