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
    public class RoutesController
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

            // GET all routes
            if (request.HttpMethod == "GET" && request.Url.AbsolutePath == "/api/routes")
            {
                var options = new JsonSerializerOptions { WriteIndented = true };
                responseString = JsonSerializer.Serialize(await HttpGetAllRoutes(), options);
            }
            // GET route by id
            else if (request.HttpMethod == "GET" && request.Url.PathAndQuery.StartsWith("/api/routes/"))
            {
                string[] parts = request.Url.PathAndQuery.Split('/');
                if (parts.Length == 4 && int.TryParse(parts[3], out int id))
                {
                    var options = new JsonSerializerOptions { WriteIndented = true };
                    responseString = JsonSerializer.Serialize(await HttpGetRouteById(id), options);
                    if (responseString == "null")
                    {
                        responseString = "Invalid id, Error = " + (int)HttpStatusCode.BadRequest;
                    }
                }
                else if (parts.Length == 4)
                {
                    string myEndPointString = parts[3];
                    var options = new JsonSerializerOptions { WriteIndented = true };
                    var routesByName = await HttpGetRouteByName(myEndPointString);
                    responseString = JsonSerializer.Serialize(routesByName, options);

                    if (routesByName == null || !routesByName.Any())
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
            else if (request.HttpMethod == "POST" && request.Url.AbsolutePath == "/api/routes")
            {
                using (var reader = new StreamReader(request.InputStream, request.ContentEncoding))
                {
                    string requestBody = await reader.ReadToEndAsync();
                    var data = JsonSerializer.Deserialize<Routes>(requestBody);

                    string result = await HttpPostRoute(data.Mark_Start, data.Mark_End, data.Route_Name, data.Route_Description, data.Route_Distance);
                    responseString = result;
                }
            }
            // PUT
            else if (request.HttpMethod == "PUT" && request.Url.PathAndQuery.StartsWith("/api/routes/"))
            {
                string[] parts = request.Url.PathAndQuery.Split('/');
                if (parts.Length == 4 && int.TryParse(parts[3], out int routeId))
                {
                    using (var reader = new StreamReader(request.InputStream, request.ContentEncoding))
                    {
                        string requestBody = await reader.ReadToEndAsync();
                        var data = JsonSerializer.Deserialize<Routes>(requestBody);

                        string result = await HttpPutRouteById(routeId, data.Mark_Start, data.Mark_End, data.Route_Name, data.Route_Description, data.Route_Distance);
                        responseString = result;
                    }
                }
                else
                {
                    responseString = "Invalid route ID, Error = " + (int)HttpStatusCode.BadRequest;
                }
            }
            // PATCH
            else if (request.HttpMethod == "PATCH" && request.Url.PathAndQuery.StartsWith("/api/routes/"))
            {
                string[] parts = request.Url.PathAndQuery.Split('/');
                if (parts.Length == 4 && int.TryParse(parts[3], out int routeId))
                {
                    using (var reader = new StreamReader(request.InputStream, request.ContentEncoding))
                    {
                        string requestBody = await reader.ReadToEndAsync();
                        var data = JsonSerializer.Deserialize<Routes>(requestBody);

                        string result = await HttpPatchRouteById(routeId, data.Mark_Start, data.Mark_End, data.Route_Name, data.Route_Description, data.Route_Distance);
                        responseString = result;
                    }
                }
                else
                {
                    responseString = "Invalid route ID, Error = " + (int)HttpStatusCode.BadRequest;
                }
            }
            // DELETE
            else if (request.HttpMethod == "DELETE" && request.Url.PathAndQuery.StartsWith("/api/routes/"))
            {
                string[] parts = request.Url.PathAndQuery.Split('/');
                if (parts.Length == 4 && int.TryParse(parts[3], out int routeId))
                {
                    string result = await HttpDelRouteById(routeId);
                    responseString = result;
                }
                else
                {
                    responseString = "Invalid route ID, Error = " + (int)HttpStatusCode.BadRequest;
                }
            }
            else
            {
                responseString = "Invalid request method or endpoint, Error = " + (int)HttpStatusCode.BadRequest;
            }

            return responseString;
        }

        private async Task<IEnumerable<Routes>> HttpGetAllRoutes()
        {
            List<Routes> routes = new List<Routes>();

            try
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    await connection.OpenAsync();

                    string sqlRequest = "SELECT * FROM routes";

                    using (MySqlCommand command = new MySqlCommand(sqlRequest, connection))
                    {
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
                                    Route_Distance = reader.IsDBNull(reader.GetOrdinal("Route_Distance")) ? (float?)null : Convert.ToSingle(reader["Route_Distance"])
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

        private async Task<Routes> HttpGetRouteById(int id)
        {
            Routes route = null;

            try
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    await connection.OpenAsync();

                    string sqlRequest = "SELECT * FROM routes WHERE Route_Id = @RouteId";

                    using (MySqlCommand command = new MySqlCommand(sqlRequest, connection))
                    {
                        command.Parameters.AddWithValue("@RouteId", id);

                        using (MySqlDataReader reader = await command.ExecuteReaderAsync())
                        {
                            if (await reader.ReadAsync())
                            {
                                route = new Routes
                                {
                                    Route_Id = Convert.ToInt32(reader["Route_Id"]),
                                    Mark_Start = Convert.ToInt32(reader["Mark_Start"]),
                                    Mark_End = Convert.ToInt32(reader["Mark_End"]),
                                    Route_Name = reader["Route_Name"].ToString(),
                                    Route_Description = reader["Route_Description"].ToString(),
                                    Route_Distance = reader.IsDBNull(reader.GetOrdinal("Route_Distance")) ? (float?)null : Convert.ToSingle(reader["Route_Distance"])
                                };
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error during route retrieval: {ex.Message}");
            }

            return route;
        }

        private async Task<IEnumerable<Routes>> HttpGetRouteByName(string name)
        {
            List<Routes> routes = new List<Routes>();

            try
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    await connection.OpenAsync();

                    string sqlRequest = "SELECT * FROM routes WHERE Route_Name LIKE @RouteName";

                    using (MySqlCommand command = new MySqlCommand(sqlRequest, connection))
                    {
                        command.Parameters.AddWithValue("@RouteName", "%" + name + "%");

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
                                    Route_Distance = reader.IsDBNull(reader.GetOrdinal("Route_Distance")) ? (float?)null : Convert.ToSingle(reader["Route_Distance"])
                                };
                                routes.Add(route);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error during routes retrieval by name: {ex.Message}");
            }

            return routes;
        }


        private async Task<string> HttpPostRoute(int markStart, int markEnd, string routeName, string routeDescription, float? routeDistance)
        {
            try
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    await connection.OpenAsync();

                    string sqlRequest = "INSERT INTO routes (Mark_Start, Mark_End, Route_Name, Route_Description, Route_Distance) VALUES (@MarkStart, @MarkEnd, @RouteName, @RouteDescription, @RouteDistance)";

                    using (MySqlCommand command = new MySqlCommand(sqlRequest, connection))
                    {
                        command.Parameters.AddWithValue("@MarkStart", markStart);
                        command.Parameters.AddWithValue("@MarkEnd", markEnd);
                        command.Parameters.AddWithValue("@RouteName", routeName);
                        command.Parameters.AddWithValue("@RouteDescription", routeDescription);
                        command.Parameters.AddWithValue("@RouteDistance", routeDistance.HasValue ? (object)routeDistance.Value : DBNull.Value);

                        await command.ExecuteNonQueryAsync();
                    }
                }
                return "Route added successfully.";
            }
            catch (Exception ex)
            {
                return $"Error during route insertion: {ex.Message}";
            }
        }

        private async Task<string> HttpPutRouteById(int id, int markStart, int markEnd, string routeName, string routeDescription, float? routeDistance)
        {
            try
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    await connection.OpenAsync();

                    string sqlRequest = "UPDATE routes SET Mark_Start = @MarkStart, Mark_End = @MarkEnd, Route_Name = @RouteName, Route_Description = @RouteDescription, Route_Distance = @RouteDistance WHERE Route_Id = @RouteId";

                    using (MySqlCommand command = new MySqlCommand(sqlRequest, connection))
                    {
                        command.Parameters.AddWithValue("@RouteId", id);
                        command.Parameters.AddWithValue("@MarkStart", markStart);
                        command.Parameters.AddWithValue("@MarkEnd", markEnd);
                        command.Parameters.AddWithValue("@RouteName", routeName);
                        command.Parameters.AddWithValue("@RouteDescription", routeDescription);
                        command.Parameters.AddWithValue("@RouteDistance", routeDistance.HasValue ? (object)routeDistance.Value : DBNull.Value);

                        await command.ExecuteNonQueryAsync();
                    }
                }
                return "Route updated successfully.";
            }
            catch (Exception ex)
            {
                return $"Error during route update: {ex.Message}";
            }
        }

        private async Task<string> HttpPatchRouteById(int id, int? markStart, int? markEnd, string routeName, string routeDescription, float? routeDistance)
        {
            try
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    await connection.OpenAsync();

                    var updates = new List<string>();
                    if (markStart.HasValue) updates.Add("Mark_Start = @MarkStart");
                    if (markEnd.HasValue) updates.Add("Mark_End = @MarkEnd");
                    if (routeName != null) updates.Add("Route_Name = @RouteName");
                    if (routeDescription != null) updates.Add("Route_Description = @RouteDescription");
                    if (routeDistance.HasValue) updates.Add("Route_Distance = @RouteDistance");

                    string updateQuery = string.Join(", ", updates);

                    string sqlRequest = $"UPDATE routes SET {updateQuery} WHERE Route_Id = @RouteId";

                    using (MySqlCommand command = new MySqlCommand(sqlRequest, connection))
                    {
                        command.Parameters.AddWithValue("@RouteId", id);
                        if (markStart.HasValue) command.Parameters.AddWithValue("@MarkStart", markStart.Value);
                        if (markEnd.HasValue) command.Parameters.AddWithValue("@MarkEnd", markEnd.Value);
                        if (routeName != null) command.Parameters.AddWithValue("@RouteName", routeName);
                        if (routeDescription != null) command.Parameters.AddWithValue("@RouteDescription", routeDescription);
                        if (routeDistance.HasValue) command.Parameters.AddWithValue("@RouteDistance", routeDistance.Value);

                        await command.ExecuteNonQueryAsync();
                    }
                }
                return "Route patched successfully.";
            }
            catch (Exception ex)
            {
                return $"Error during route patch: {ex.Message}";
            }
        }

        private async Task<string> HttpDelRouteById(int id)
        {
            try
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    await connection.OpenAsync();

                    string sqlRequest = "DELETE FROM routes WHERE Route_Id = @RouteId";

                    using (MySqlCommand command = new MySqlCommand(sqlRequest, connection))
                    {
                        command.Parameters.AddWithValue("@RouteId", id);
                        await command.ExecuteNonQueryAsync();
                    }
                }
                return "Route deleted successfully.";
            }
            catch (Exception ex)
            {
                return $"Error during route deletion: {ex.Message}";
            }
        }
    }
}
