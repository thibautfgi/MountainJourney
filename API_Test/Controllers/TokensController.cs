
// using Microsoft.VisualBasic;
// using Models;
// using MySqlConnector;
// using System.Collections.Generic;
// using System.Linq;
// using System.Net;
// using System.Text;
// using System.Text.Json;

// namespace Controllers
// {
//     public class TokensController
//     {
//          private string connectionString = "Server=localhost;User ID=root;Password=azerty;Database=mj";
//         // mes identifiants pour me connect a mon mysql workbench

// // TODO: 
// // gener mieux la connection sql = 1 connection


//         public async Task<string> ProcessRequest(HttpListenerRequest request)
//         {
//             string responseString = "";

//             //GET

//             if (request.HttpMethod == "GET" && request.Url.PathAndQuery == "/api/tokens")
//             {
//                 var options = new JsonSerializerOptions { WriteIndented = true }; //cette ligne rend le json html jolie
//                 responseString = JsonSerializer.Serialize(await UsersFromToken(request), options);
//             }
//             else if (request.HttpMethod == "GET" && request.Url.PathAndQuery.StartsWith("/api/users"))
//             {
//                 responseString = "bad endpoint, Error =  " + (int)HttpStatusCode.BadRequest;
//             }
            
            
//             // POST

//             else if (request.HttpMethod == "POST" && request.Url.PathAndQuery == "/api/tokens")
//             {
//                 using (var reader = new StreamReader(request.InputStream, request.ContentEncoding))
//                 {
//                     string requestBody = reader.ReadToEnd(); // permet de lire le body de la requete postman json
//                     var data = JsonSerializer.Deserialize<Tokens>(requestBody); //ici data accede au body

//                     int userId = data.User_Id;
//                     string tokenValue = data.Token_Value;

//                     responseString = await HttpPostNewToken(userId, tokenValue);
//                 }
//             }
//             else if (request.HttpMethod == "POST" && request.Url.PathAndQuery.StartsWith("/api/users"))
//             {
//                 responseString = "bad endpoint, Error =  " + (int)HttpStatusCode.BadRequest;
//             }

//             //final return
//             return responseString;
//         }

//         private async Task<string> HttpPostNewToken(int userID, string tokenValue)
//         {
//             // sur postman, faire la requete avec un body contenant les infos ci dessus
//             try
//             {
//                 using (MySqlConnection connection = new MySqlConnection(connectionString))
//                 {
//                     await connection.OpenAsync();

//                     string SqlRequest = "INSERT INTO tokens (User_Id, Token_Value) VALUES (@UserId, @TokenValue)";

//                     using (MySqlCommand command = new MySqlCommand(SqlRequest, connection))
//                     { // lie les @ a une string
//                         command.Parameters.AddWithValue("@UserId", userID);
//                         command.Parameters.AddWithValue("@TokenValue", tokenValue);

//                         int rowsAffected = await command.ExecuteNonQueryAsync();

//                         if (rowsAffected > 0)
//                         {
//                             return "Its work! Post effectué! ";
//                         }
//                         else
//                         {
//                             return "Post failled, no row creat";
//                         }
//                     }
//                 }
//             }
//             catch (Exception ex)
//             {
//                 // gestion de l'erreur
//                 return $"Error during post: {ex.Message}";
//             }
//         }

//         private async Task<Users> UsersFromToken(HttpListenerRequest request)
//         {
//             Users user = null;

//             // Extract the bearer token from the Authorization header
//             string authorizationHeader = request.Headers["Authorization"];
//             if (string.IsNullOrEmpty(authorizationHeader) || !authorizationHeader.StartsWith("Bearer "))
//             {
//                 return user;
//             }

//             string token = authorizationHeader.Substring("Bearer ".Length);

//             // Database query to retrieve user information
//             using (MySqlConnection connection = new MySqlConnection(connectionString))
//             {
//                 await connection.OpenAsync();

//                 string sqlQuery = @"
//                     SELECT u.*
//                     FROM users u
//                     JOIN tokens t ON u.User_Id = t.User_Id
//                     WHERE t.Token_Value = @Token;
//                 ";

//                 // cette query renvoie le user corespondant au token données

//                 using (MySqlCommand command = new MySqlCommand(sqlQuery, connection))
//                 {
//                     command.Parameters.AddWithValue("@Token", token);

//                     using (MySqlDataReader reader = await command.ExecuteReaderAsync())
//                     {
//                         if (await reader.ReadAsync())
//                         {
//                             user = new Users
//                             {
//                                 User_Id = Convert.ToInt32(reader["User_Id"]),
//                                 User_FirstName = reader["User_FirstName"].ToString(),
//                                 User_LastName = reader["User_LastName"].ToString(),
//                                 User_Email = reader["User_Email"].ToString(),
//                                 User_Password = reader["User_Password"].ToString(),
//                                 User_Phone = reader["User_Phone"].ToString(),
//                             };
//                         }
//                     }
//                 }
//             }

//             // Return the user associated with the token
//             return user;
//         }

        
       

//     }
// }


