
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
    public class UsersController
    {
        private string connectionString = "Server=localhost;User ID=root;Password=azerty;Database=mj";
        // mes identifiants pour me connect a mon mysql workbench
// TODO: 
// gener mieux la connection sql = 1 connection


        public async Task<string> ProcessRequest(HttpListenerRequest request)
        {
            string responseString = "";


            // GET

            if (request.HttpMethod == "GET" && request.Url.PathAndQuery == "/api/users")
            {
                var options = new JsonSerializerOptions { WriteIndented = true };

                // var verifiedUser = await TokenVerification.TokenVerify(request); //TOKEN TCHEK
                // if (verifiedUser == null)
                // {
                //     responseString = "Unauthorized access, wrong or empty token, please refer to the admin for obtain a valid key";
                // }
                // else
                // {
                responseString = JsonSerializer.Serialize(await HttpGetAllUsers(), options);
                // }
            }

            // GET all user
        
            else if (request.HttpMethod == "GET" && request.Url.PathAndQuery.StartsWith("/api/users/"))
            {
                string[] strings = request.Url.PathAndQuery.Split('/');
                string[] parts = strings; // separe notre url sur les "/"
                if (parts.Length == 4 && int.TryParse(parts[3], out int id))
                {
                    var options = new JsonSerializerOptions { WriteIndented = true }; //cette ligne rend le json html jolie
                    responseString = JsonSerializer.Serialize(await HttpGetUserById(id), options);
                    if (responseString == "null")
                    {
                    responseString = "Invalid id, Error =  " + (int)HttpStatusCode.BadRequest;
                    }

                }

                // GET comments by user id

                else if (parts.Length == 5 && parts[4] == "comments" && int.TryParse(parts[3], out int myId))
                {
                    var options = new JsonSerializerOptions { WriteIndented = true };
                    var comments = await HttpGetCommentsByUserId(myId);

                    if (comments.Any())
                    {
                        responseString = JsonSerializer.Serialize(comments, options);
                    }
                    else
                    {
                        responseString = "Invalid id or no comments found, Error = " + (int)HttpStatusCode.BadRequest;
                    }
                }

               // GET friendlists by user id


                else if (parts.Length == 5 && parts[4] == "friendlists" && int.TryParse(parts[3], out int myotherId))
                {
                    var options = new JsonSerializerOptions { WriteIndented = true };
                    var friendlists = await HttpGetFriendlistsByUserId(myotherId);

                    if (friendlists.Any())
                    {
                        responseString = JsonSerializer.Serialize(friendlists, options);
                    }
                    else
                    {
                        responseString = "Invalid id or no friendlists found, Error = " + (int)HttpStatusCode.BadRequest;
                    }
                }
                
                // GET likes by user id
                
                else if (parts.Length == 5 && parts[4] == "likes" && int.TryParse(parts[3], out int mythirdId))
                {
                    var options = new JsonSerializerOptions { WriteIndented = true };
                    var likes = await HttpGetLikesByUserId(mythirdId);

                    if (likes.Any())
                    {
                        responseString = JsonSerializer.Serialize(likes, options);
                    }
                    else
                    {
                        responseString = "Invalid id or no likes found, Error = " + (int)HttpStatusCode.BadRequest;
                    }
                }
                
               // GET maps by user id


                else if (parts.Length == 5 && parts[4] == "maps" && int.TryParse(parts[3], out int myquatreId))
                {
                    var options = new JsonSerializerOptions { WriteIndented = true };
                    var maps = await HttpGetMapsByUserId(myquatreId);

                    if (maps.Any())
                    {
                        responseString = JsonSerializer.Serialize(maps, options);
                    }
                    else
                    {
                        responseString = "Invalid id or no maps found, Error = " + (int)HttpStatusCode.BadRequest;
                    }
                }

                // GET marks by user id

                else if (parts.Length == 5 && parts[4] == "marks" && int.TryParse(parts[3], out int mycinqId))
                {
                    var options = new JsonSerializerOptions { WriteIndented = true };
                    var marks = await HttpGetMarksByUserId(mycinqId);

                    if (marks.Any())
                    {
                        responseString = JsonSerializer.Serialize(marks, options);
                    }
                    else
                    {
                        responseString = "Invalid id or no marks found, Error = " + (int)HttpStatusCode.BadRequest;
                    }
                }


                // GET routes by user id

                else if (parts.Length == 5 && parts[4] == "routes" && int.TryParse(parts[3], out int mylastId))
                {
                    var options = new JsonSerializerOptions { WriteIndented = true };
                    var routes = await HttpGetRoutesByUserId(mylastId);

                    if (routes.Any())
                    {
                        responseString = JsonSerializer.Serialize(routes, options);
                    }
                    else
                    {
                        responseString = "Invalid id or no routes found, Error = " + (int)HttpStatusCode.BadRequest;
                    }
                }
                                    
                else if (parts.Length > 4)
                {
                    responseString = "bad endpoint, Error =  " + (int)HttpStatusCode.BadRequest;
                }
                else if (request.Url.PathAndQuery == "/api/users/")
                {
                    responseString = "enter a id please, bad endpoint, Error =  " + (int)HttpStatusCode.BadRequest;
                }
                else
                {
                    string myEndPointString = parts[3];


                // GET user by email


                    if (myEndPointString.Contains("@"))
                    {
                        var options = new JsonSerializerOptions { WriteIndented = true }; //cette ligne rend le json html jolie
                        responseString = JsonSerializer.Serialize(await HttpGetUserByEmail(myEndPointString), options);

                        if (responseString == "null")
                        {
                        responseString = "Invalid Name or email, Error =  " + (int)HttpStatusCode.BadRequest;
                        }
                    } 
                    else
                    {

                // GET user by LastName


                        var options = new JsonSerializerOptions { WriteIndented = true }; //cette ligne rend le json html jolie
                        responseString = JsonSerializer.Serialize(await HttpGetUserByLastName(myEndPointString), options);
                        
                        if (responseString == "null")
                        {
                        responseString = "Invalid Name or email, Error =  " + (int)HttpStatusCode.BadRequest;
                        }
                    }

                    
                    
                }
            }

            // POST


           else if (request.HttpMethod == "POST" && request.Url.PathAndQuery == "/api/users")
            {
                using (var reader = new StreamReader(request.InputStream, request.ContentEncoding))
                {
                    string requestBody = await reader.ReadToEndAsync(); // asynchronously
                    var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true }; 
                    var data = JsonSerializer.Deserialize<Users>(requestBody, options); // Deserialize the request body into a Users object

                    if (data == null)
                    {
                        responseString = "Invalid user data, Error = " + (int)HttpStatusCode.BadRequest;
                    }
                    else
                    {
                        string firstName = data.User_FirstName;
                        string lastName = data.User_LastName;
                        string email = data.User_Email;
                        string password = data.User_Password;
                        string phone = data.User_Phone;

                        var verifiedUser = await TokenVerification.TokenVerify(request); // Token verification
                        if (verifiedUser == null)
                        {
                            responseString = "Unauthorized access, wrong or empty token, please refer to the admin for a valid key";
                        }
                        else
                        {
                            responseString = await HttpPostNewUser(firstName, lastName, email, password, phone);
                        }
                    }
                }
            }
            else if (request.HttpMethod == "POST" && request.Url.PathAndQuery.StartsWith("/api/users"))
            {
                responseString = "Bad endpoint, Error = " + (int)HttpStatusCode.BadRequest;
            }


        // PUT


        else if (request.HttpMethod == "PUT" && request.Url.PathAndQuery.StartsWith("/api/users/"))
        {
            string[] strings = request.Url.PathAndQuery.Split('/');
            string[] parts = strings; // Separate the URL by "/"

            if (parts.Length == 4 && int.TryParse(parts[3], out int id))
            {
                var verifiedUser = await TokenVerification.TokenVerify(request); // Token verification
                if (verifiedUser == null)
                {
                    responseString = "Unauthorized access, wrong or empty token, please refer to the admin for a valid key";
                }
                else
                {
                    try
                    {
                        using (var reader = new StreamReader(request.InputStream, request.ContentEncoding))
                        {
                            string requestBody = await reader.ReadToEndAsync(); // Read the request body asynchronously
                            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true, WriteIndented = true }; // Ignore case and prettify JSON
                            var data = JsonSerializer.Deserialize<Users>(requestBody, options); // Deserialize the request body into a Users object

                            if (data == null)
                            {
                                responseString = "Invalid user data, Error = " + (int)HttpStatusCode.BadRequest;
                            }
                            else
                            {
                                string firstName = data.User_FirstName;
                                string lastName = data.User_LastName;
                                string email = data.User_Email;
                                string password = data.User_Password;
                                string phone = data.User_Phone;

                                responseString = JsonSerializer.Serialize(await HttpPutUserById(id, firstName, lastName, email, password, phone), options);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        // Handle the error
                        responseString = $"No or bad body sent: {ex.Message}, Error = " + (int)HttpStatusCode.BadRequest;
                    }
                }
            }
            else if (parts.Length > 4)
            {
                responseString = "Bad endpoint, Error = " + (int)HttpStatusCode.BadRequest;
            }
            else if (request.Url.PathAndQuery == "/api/users/")
            {
                responseString = "Enter an ID please, bad endpoint, Error = " + (int)HttpStatusCode.BadRequest;
            }
            else
            {
                responseString = "Not an ID, Error = " + (int)HttpStatusCode.BadRequest;
            }
        }


            //DELETE

        else if (request.HttpMethod == "DELETE" && request.Url.PathAndQuery.StartsWith("/api/users/"))
        {
            string[] strings = request.Url.PathAndQuery.Split('/');
            string[] parts = strings; // Separate the URL by "/"

            if (parts.Length == 4 && int.TryParse(parts[3], out int id))
            {
                var verifiedUser = await TokenVerification.TokenVerify(request); // Token verification
                if (verifiedUser == null)
                {
                    responseString = "Unauthorized access, wrong or empty token, please refer to the admin for a valid key";
                }
                else
                {
                    try
                    {
                        var options = new JsonSerializerOptions { WriteIndented = true }; // jolie JSON
                        responseString = JsonSerializer.Serialize(await HttpDelUserById(id), options);
                    }
                    catch (Exception ex)
                    {
                        // Handle the error
                        responseString = $"Error deleting user: {ex.Message}, Error = " + (int)HttpStatusCode.InternalServerError;
                    }
                }
            }
            else if (parts.Length > 4)
            {
                responseString = "Bad endpoint, Error = " + (int)HttpStatusCode.BadRequest;
            }
            else if (request.Url.PathAndQuery == "/api/users/")
            {
                responseString = "Enter an ID please, bad endpoint, Error = " + (int)HttpStatusCode.BadRequest;
            }
            else
            {
                responseString = "Not an ID, Error = " + (int)HttpStatusCode.BadRequest;
            }
        }


            // HTTP PATCH


        else if (request.HttpMethod == "PATCH" && request.Url.PathAndQuery.StartsWith("/api/users/"))
        {
            string[] strings = request.Url.PathAndQuery.Split('/');
            string[] parts = strings; // Separate the URL by "/"

            if (parts.Length == 4 && int.TryParse(parts[3], out int id))
            {
                var verifiedUser = await TokenVerification.TokenVerify(request); // Token verification
                if (verifiedUser == null)
                {
                    responseString = "Unauthorized access, wrong or empty token, please refer to the admin for a valid key";
                }
                else
                {
                    try
                    {
                        using (var reader = new StreamReader(request.InputStream, request.ContentEncoding))
                        {
                            string requestBody = await reader.ReadToEndAsync(); // Read the request body
                            var data = JsonSerializer.Deserialize<Users>(requestBody); // Deserialize the JSON body into a Users object
                            
                            string firstName = data.User_FirstName;
                            string lastName = data.User_LastName;
                            string email = data.User_Email;
                            string password = data.User_Password;
                            string phone = data.User_Phone;

                            if (firstName == null && lastName == null && email == null && password == null && phone == null)
                            {
                                responseString = "Bad body, Error = " + (int)HttpStatusCode.BadRequest;
                            }
                            else
                            {
                                var options = new JsonSerializerOptions { WriteIndented = true };
                                responseString = JsonSerializer.Serialize(await HttpPatchUserById(id, firstName, lastName, email, password, phone), options);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        // Handle the error
                        responseString = $"No or bad body sent: {ex.Message}, Error = " + (int)HttpStatusCode.InternalServerError;
                    }
                }
            }
            else if (parts.Length > 4)
            {
                responseString = "Bad endpoint, Error = " + (int)HttpStatusCode.BadRequest;
            }
            else if (request.Url.PathAndQuery == "/api/users/")
            {
                responseString = "Enter an ID please, bad endpoint, Error = " + (int)HttpStatusCode.BadRequest;
            }
            else
            {
                responseString = "Not an ID, Error = " + (int)HttpStatusCode.BadRequest;
            }
        }


    //final return
        return responseString;
    }




        private async Task<string> HttpPutUserById(int id, string firstName, string lastName, string email, string password, string phone)
        {
            try
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    await connection.OpenAsync();

                    string sqlRequest = "UPDATE users SET User_FirstName = @FirstName, User_LastName = @LastName, User_Email = @Email, User_Password = @Password, User_Phone = @Phone WHERE User_Id = @Id";
                    
                    using (MySqlCommand command = new MySqlCommand(sqlRequest, connection))
                    {
                        command.Parameters.AddWithValue("@Id", id);
                        command.Parameters.AddWithValue("@FirstName", firstName);
                        command.Parameters.AddWithValue("@LastName", lastName);
                        command.Parameters.AddWithValue("@Email", email);
                        command.Parameters.AddWithValue("@Password", password); // Consider hashing the password before storing it
                        command.Parameters.AddWithValue("@Phone", phone);

                        int rowsAffected = await command.ExecuteNonQueryAsync();

                        if (rowsAffected > 0)
                        {
                            return "User successfully updated, Status = " + (int)HttpStatusCode.OK;
                        }
                        else
                        {
                            return "Failed to update user, Error = " + (int)HttpStatusCode.InternalServerError;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Handle the exception
                Console.WriteLine($"Error during user update: {ex.Message}");
                return "Error during user update: " + ex.Message;
            }
        }







        private async Task<IEnumerable<Friendlist>> HttpGetFriendlistsByUserId(int id)
        {
            List<Friendlist> friendlists = new List<Friendlist>();

            try
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    await connection.OpenAsync();

                    string sqlRequest = "SELECT * FROM friendlists WHERE User_Main_Id = @UserId"; 

                    using (MySqlCommand command = new MySqlCommand(sqlRequest, connection))
                    {
                        command.Parameters.AddWithValue("@UserId", id);

                        using (MySqlDataReader reader = await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                Friendlist friendlist = new Friendlist
                                {
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
                // Handle the exception
                Console.WriteLine($"Error during friendlist retrieval: {ex.Message}");
            }

            return friendlists;
        }





        private async Task<IEnumerable<Likes>> HttpGetLikesByUserId(int id)
        {
            List<Likes> likes = new List<Likes>();

            try
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    await connection.OpenAsync();

                    string sqlRequest = "SELECT * FROM likes WHERE User_Id = @UserId"; 

                    using (MySqlCommand command = new MySqlCommand(sqlRequest, connection))
                    {
                        command.Parameters.AddWithValue("@UserId", id);

                        using (MySqlDataReader reader = await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                Likes like = new Likes
                                {
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
                // Handle the exception
                Console.WriteLine($"Error during likes retrieval: {ex.Message}");
            }

            return likes;
        }







        private async Task<IEnumerable<Maps>> HttpGetMapsByUserId(int id)
        {
            List<Maps> maps = new List<Maps>();

            try
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    await connection.OpenAsync();

                    string sqlRequest = "SELECT * FROM maps WHERE User_Id = @UserId"; 

                    using (MySqlCommand command = new MySqlCommand(sqlRequest, connection))
                    {
                        command.Parameters.AddWithValue("@UserId", id);

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
            }
            catch (Exception ex)
            {
                // Handle the exception
                Console.WriteLine($"Error during maps retrieval: {ex.Message}");
            }

            return maps;
        }






        private async Task<IEnumerable<Marks>> HttpGetMarksByUserId(int id)
        {
            List<Marks> marks = new List<Marks>();

            try
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    await connection.OpenAsync();

                    string sqlRequest = "SELECT * FROM marks WHERE Map_Id IN (SELECT Map_Id FROM maps WHERE User_Id = @UserId)"; 

                    using (MySqlCommand command = new MySqlCommand(sqlRequest, connection))
                    {
                        command.Parameters.AddWithValue("@UserId", id);

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
                // Handle the exception
                Console.WriteLine($"Error during marks retrieval: {ex.Message}");
            }

            return marks;
        }





        private async Task<IEnumerable<Routes>> HttpGetRoutesByUserId(int id)
        {
            List<Routes> routes = new List<Routes>();

            try
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    await connection.OpenAsync();

                    string sqlRequest = "SELECT * FROM routes WHERE Mark_Start IN (SELECT Mark_Id FROM marks WHERE Map_Id IN (SELECT Map_Id FROM maps WHERE User_Id = @UserId))"; 

                    using (MySqlCommand command = new MySqlCommand(sqlRequest, connection))
                    {
                        command.Parameters.AddWithValue("@UserId", id);

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
                // Handle the exception
                Console.WriteLine($"Error during routes retrieval: {ex.Message}");
            }

            return routes;
        }




        private async Task<string> HttpPatchUserById(int id, string firstName, string lastName, string email, string password, string phone)
        {
            try
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    await connection.OpenAsync();

                    // cette requete permet de mettre a jour seulement les champs non vide
                    string SqlRequest = "UPDATE users SET ";

                    List<string> updates = new List<string>();

                    if (!string.IsNullOrEmpty(firstName))
                    {
                        updates.Add("User_FirstName = @FirstName");
                    }
                    if (!string.IsNullOrEmpty(lastName))
                    {
                        updates.Add("User_LastName = @LastName");
                    }
                    if (!string.IsNullOrEmpty(email))
                    {
                        updates.Add("User_Email = @Email");
                    }
                    if (!string.IsNullOrEmpty(password))
                    {
                        updates.Add("User_Password = @Password");
                    }
                    if (!string.IsNullOrEmpty(phone))
                    {
                        updates.Add("User_Phone = @Phone");
                    }

                    SqlRequest += string.Join(", ", updates);
                    SqlRequest += " WHERE User_Id = @UserId";

                    //ici on fait des collages pour avoir notre requete sql

                    Console.WriteLine(SqlRequest);

                    using (MySqlCommand command = new MySqlCommand(SqlRequest, connection))
                    {
                        command.Parameters.AddWithValue("@UserId", id);
                        command.Parameters.AddWithValue("@FirstName", firstName);
                        command.Parameters.AddWithValue("@LastName", lastName);
                        command.Parameters.AddWithValue("@Email", email);
                        command.Parameters.AddWithValue("@Password", password);
                        command.Parameters.AddWithValue("@Phone", phone);

                        int rowsAffected = await command.ExecuteNonQueryAsync();

                        if (rowsAffected > 0)
                        {
                            return "Patch success! User updated!";
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
                // Gérer l'erreur
                return $"Error during PATCH: {ex.Message}";
            }
        }


        private async Task<string> HttpPostNewUser(string firstName, string lastName, string email, string password, string phone)
        {
            try
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    await connection.OpenAsync();

                    string sqlRequest = "INSERT INTO users (User_FirstName, User_LastName, User_Email, User_Password, User_Phone) VALUES (@FirstName, @LastName, @Email, @Password, @Phone)";
                    
                    using (MySqlCommand command = new MySqlCommand(sqlRequest, connection))
                    {
                        command.Parameters.AddWithValue("@FirstName", firstName);
                        command.Parameters.AddWithValue("@LastName", lastName);
                        command.Parameters.AddWithValue("@Email", email);
                        command.Parameters.AddWithValue("@Password", password); // Consider hashing the password before storing it
                        command.Parameters.AddWithValue("@Phone", phone);

                        int rowsAffected = await command.ExecuteNonQueryAsync();

                        if (rowsAffected > 0)
                        {
                            return "User successfully created, Status = " + (int)HttpStatusCode.Created;
                        }
                        else
                        {
                            return "Failed to create user, Error = " + (int)HttpStatusCode.InternalServerError;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Handle the exception
                Console.WriteLine($"Error during user creation: {ex.Message}");
                return "Error during user creation: " + ex.Message;
            }
        }


//         // HttpPostNewUserWithId si on veux cree sur un Id specifique


        private async Task<IEnumerable<Users>> HttpGetAllUsers()
        {

        List<Users> users = new List<Users>(); //cree une liste vide


            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                await connection.OpenAsync();

                string SqlRequest = "SELECT * FROM users"; // recupère tt les users

                using (MySqlCommand command = new MySqlCommand(SqlRequest, connection))
                {
                    using (MySqlDataReader reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            Users user = new Users // retourne en object les données de ma base SQL
                            {
                                User_Id = Convert.ToInt32(reader["User_Id"]),
                                User_FirstName = reader["User_FirstName"].ToString(),
                                User_LastName = reader["User_LastName"].ToString(),
                                User_Email = reader["User_Email"].ToString(),
                                User_Password = reader["User_Password"].ToString(),
                                User_Phone = reader["User_Phone"].ToString(),
                            };
                            users.Add(user);
                        }
                    }
                }
            }
            return users;
        }



       private async Task<IEnumerable<Comments>> HttpGetCommentsByUserId(int id)
        {
            List<Comments> comments = new List<Comments>();

            try
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    await connection.OpenAsync();

                    string sqlRequest = "SELECT * FROM comments WHERE User_Id = @UserId"; 
                    // permet d'envoyé des données dans la query par un @ en C#

                    using (MySqlCommand command = new MySqlCommand(sqlRequest, connection))
                    {
                        command.Parameters.AddWithValue("@UserId", id);

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
                // Handle the exception
                Console.WriteLine($"Error during comments retrieval: {ex.Message}");
            }

            return comments;
        }
    




        private async Task<Users> HttpGetUserById(int id)
        {
            
        Users user = null;

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                await connection.OpenAsync();

                string SqlRequest = "SELECT * FROM users WHERE User_Id = @UserId"; // ma query SQL

                using (MySqlCommand command = new MySqlCommand(SqlRequest, connection))
                {
                    command.Parameters.AddWithValue("@UserId", id); 
                    // permet d'envoyé des données dans la query par un @ en C#

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




        private async Task<Users> HttpGetUserByLastName(string name)
        {
            
        Users user = null;

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                await connection.OpenAsync();

                string SqlRequest = "SELECT * FROM users WHERE User_LastName = @UserLastName"; // ma query SQL

                using (MySqlCommand command = new MySqlCommand(SqlRequest, connection))
                {
                    command.Parameters.AddWithValue("@UserLastName", name); 
                    // permet d'envoyé des données dans la query par un @ en C#

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




        private async Task<Users> HttpGetUserByEmail(string email)
        {
            
        Users user = null;

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                await connection.OpenAsync();

                string SqlRequest = "SELECT * FROM users WHERE User_Email = @UserEmail"; // ma query SQL

                using (MySqlCommand command = new MySqlCommand(SqlRequest, connection))
                {
                    command.Parameters.AddWithValue("@UserEmail", email); 
                    // permet d'envoyé des données dans la query par un @ en C#

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




        private async Task<string> HttpDelUserById(int id)
        {
            try
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    await connection.OpenAsync();

                    string sqlRequest = "DELETE FROM users WHERE User_Id = @Id";
                    
                    using (MySqlCommand command = new MySqlCommand(sqlRequest, connection))
                    {
                        command.Parameters.AddWithValue("@Id", id);

                        int rowsAffected = await command.ExecuteNonQueryAsync();

                        if (rowsAffected > 0)
                        {
                            return "User successfully deleted, Status = " + (int)HttpStatusCode.OK;
                        }
                        else
                        {
                            return "Failed to delete user, Error = " + (int)HttpStatusCode.NotFound;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Handle the exception
                Console.WriteLine($"Error during user deletion: {ex.Message}");
                return "Error during user deletion: " + ex.Message;
            }
        }






    }

}
