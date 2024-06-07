
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

            if (request.HttpMethod == "GET" && request.Url.PathAndQuery == "/api/users")
            {
                var options = new JsonSerializerOptions { WriteIndented = true };

                var verifiedUser = await TokenVerification.TokenVerify(request);
                if (verifiedUser == null)
                {
                    responseString = "Unauthorized access, wrong token, please refer to the admin for obtain a valid key";
                }
                else
                {
                    responseString = JsonSerializer.Serialize(await HttpGetAllUsers(), options);
                }
            }
        
//         //     else if (request.HttpMethod == "GET" && request.Url.PathAndQuery.StartsWith("/api/users/"))
//         //     {
//         //         string[] strings = request.Url.PathAndQuery.Split('/');
//         //         string[] parts = strings; // separe notre url sur les "/"
//         //         if (parts.Length == 4 && int.TryParse(parts[3], out int id))
//         //         {
//         //             var options = new JsonSerializerOptions { WriteIndented = true }; //cette ligne rend le json html jolie
//         //             responseString = JsonSerializer.Serialize(await HttpGetUserById(id), options);
//         //             if (responseString == "null")
//         //             {
//         //             responseString = "Invalid id, Error =  " + (int)HttpStatusCode.BadRequest;
//         //             }

//         //         }

//         //         else if (parts.Length == 5 && parts[4] == "shoplists" && int.TryParse(parts[3], out int myId))
//         //         {
//         //             var options = new JsonSerializerOptions { WriteIndented = true };
//         //             var shoplists = await HttpGetShoplistByUserId(myId);

//         //             if (shoplists.Any())
//         //             {
//         //                 responseString = JsonSerializer.Serialize(shoplists, options);
//         //             }
//         //             else
//         //             {
//         //                 responseString = "Invalid id or no shoplists found, Error = " + (int)HttpStatusCode.BadRequest;
//         //             }
//         //         }

//         //         else if (parts.Length == 5 && parts[4] == "carts" && int.TryParse(parts[3], out int myotherId))
//         //         {
//         //             var options = new JsonSerializerOptions { WriteIndented = true };
//         //             var shoplists = await HttpGetCartsByUserId(myotherId);

//         //             if (shoplists.Any())
//         //             {
//         //                 responseString = JsonSerializer.Serialize(shoplists, options);
//         //             }
//         //             else
//         //             {
//         //                 responseString = "Invalid id or no shoplists found, Error = " + (int)HttpStatusCode.BadRequest;
//         //             }
//         //         }
                
//         //         else if (parts.Length == 5 && parts[4] == "commands" && int.TryParse(parts[3], out int mythirdId))
//         //         {
//         //             var options = new JsonSerializerOptions { WriteIndented = true };
//         //             var shoplists = await HttpGetCommandsByUserId(mythirdId);

//         //             if (shoplists.Any())
//         //             {
//         //                 responseString = JsonSerializer.Serialize(shoplists, options);
//         //             }
//         //             else
//         //             {
//         //                 responseString = "Invalid id or no commands found, Error = " + (int)HttpStatusCode.BadRequest;
//         //             }
//         //         }

//         //         else if (parts.Length == 5 && parts[4] == "invoices" && int.TryParse(parts[3], out int mylastId))
//         //         {
//         //             var options = new JsonSerializerOptions { WriteIndented = true };
//         //             var shoplists = await HttpGetInvoicesByUserId(mylastId);

//         //             if (shoplists.Any())
//         //             {
//         //                 responseString = JsonSerializer.Serialize(shoplists, options);
//         //             }
//         //             else
//         //             {
//         //                 responseString = "Invalid id or no invoices found, Error = " + (int)HttpStatusCode.BadRequest;
//         //             }
//         //         }
                                    
//         //         else if (parts.Length > 4)
//         //         {
//         //             responseString = "bad endpoint, Error =  " + (int)HttpStatusCode.BadRequest;
//         //         }
//         //         else if (request.Url.PathAndQuery == "/api/users/")
//         //         {
//         //             responseString = "enter a id please, bad endpoint, Error =  " + (int)HttpStatusCode.BadRequest;
//         //         }
//         //         else
//         //         {
//         //             string myEndPointString = parts[3];

//         //             if (myEndPointString.Contains("@"))
//         //             {
//         //                 var options = new JsonSerializerOptions { WriteIndented = true }; //cette ligne rend le json html jolie
//         //                 responseString = JsonSerializer.Serialize(await HttpGetUserByEmail(myEndPointString), options);

//         //                 if (responseString == "null")
//         //                 {
//         //                 responseString = "Invalid Name or email, Error =  " + (int)HttpStatusCode.BadRequest;
//         //                 }
//         //             } 
//         //             else
//         //             {
//         //                 var options = new JsonSerializerOptions { WriteIndented = true }; //cette ligne rend le json html jolie
//         //                 responseString = JsonSerializer.Serialize(await HttpGetUserByLastName(myEndPointString), options);
                        
//         //                 if (responseString == "null")
//         //                 {
//         //                 responseString = "Invalid Name or email, Error =  " + (int)HttpStatusCode.BadRequest;
//         //                 }
//         //             }

                    
                    
//         //         }
//         //     }

//         //     // POST


//         //     else if (request.HttpMethod == "POST" && request.Url.PathAndQuery == "/api/users")
//         //     {
//         //         using (var reader = new StreamReader(request.InputStream, request.ContentEncoding))
//         //         {
//         //             string requestBody = reader.ReadToEnd(); // permet de lire le body de la requete postman json
//         //             var data = JsonSerializer.Deserialize<Users>(requestBody); //ici data accede au body

//         //             string firstName = data.User_FirstName;
//         //             string lastName = data.User_LastName;
//         //             string email = data.User_Email;
//         //             string password = data.User_Password;
//         //             string phone = data.User_Phone;

//         //             responseString = await HttpPostNewUser(firstName, lastName, email, password, phone);
//         //         }
//         //     }
//         //     else if (request.HttpMethod == "POST" && request.Url.PathAndQuery.StartsWith("/api/users"))
//         //     {
//         //         responseString = "bad endpoint, Error =  " + (int)HttpStatusCode.BadRequest;
//         //     }


//         //     //PUT

//         //     else if (request.HttpMethod == "PUT" && request.Url.PathAndQuery.StartsWith("/api/users/"))
//         //     {
//         //         string[] strings = request.Url.PathAndQuery.Split('/');
//         //         string[] parts = strings; // separe notre url sur les "/"
//         //         if (parts.Length == 4 && int.TryParse(parts[3], out int id))
//         //         {
//         //             try
//         //             {
//         //                 using (var reader = new StreamReader(request.InputStream, request.ContentEncoding))
//         //                 {
//         //                     string requestBody = reader.ReadToEnd(); // permet de lire le body de la requete postman json
//         //                     var data = JsonSerializer.Deserialize<Users>(requestBody); //ici data accede au body

//         //                     string firstName = data.User_FirstName;
//         //                     string lastName = data.User_LastName;
//         //                     string email = data.User_Email;
//         //                     string password = data.User_Password;
//         //                     string phone = data.User_Phone;

                            
//         //                     var options = new JsonSerializerOptions { WriteIndented = true }; //cette ligne rend le json html jolie
//         //                     responseString = JsonSerializer.Serialize(await HttpPutUserById(id, firstName, lastName, email, password, phone), options);
//         //                 }
//         //             }
//         //             catch (Exception ex)
//         //             {
//         //                 // Gérer l'erreur
//         //                 return $"no or bad body send: {ex.Message}";
//         //             }
                    
//         //         }
//         //         else if (parts.Length > 4)
//         //         {
//         //             responseString = "bad endpoint, Error =  " + (int)HttpStatusCode.BadRequest;
//         //         }
//         //         else if (request.Url.PathAndQuery == "/api/users/")
//         //         {
//         //             responseString = "enter a id please, bad endpoint, Error =  " + (int)HttpStatusCode.BadRequest;
//         //         }
//         //         else
//         //         {
//         //             responseString = "not a Id, Error =  " + (int)HttpStatusCode.BadRequest;
//         //         }
//         //     }

//         //     //DELETE

//         //     else if (request.HttpMethod == "DELETE" && request.Url.PathAndQuery.StartsWith("/api/users/"))
//         //     {
//         //         string[] strings = request.Url.PathAndQuery.Split('/');
//         //         string[] parts = strings; // separe notre url sur les "/"
//         //         if (parts.Length == 4 && int.TryParse(parts[3], out int id))
//         //         {
//         //             var options = new JsonSerializerOptions { WriteIndented = true }; //cette ligne rend le json html jolie
//         //             responseString = JsonSerializer.Serialize(await HttpDelUserById(id), options);

//         //         }
//         //         else if (parts.Length > 4)
//         //         {
//         //             responseString = "bad endpoint, Error =  " + (int)HttpStatusCode.BadRequest;
//         //         }
//         //         else if (request.Url.PathAndQuery == "/api/users/")
//         //         {
//         //             responseString = "enter a id please, bad endpoint, Error =  " + (int)HttpStatusCode.BadRequest;
//         //         }
//         //         else
//         //         {
//         //             responseString = "not a Id, Error =  " + (int)HttpStatusCode.BadRequest;
//         //         }
//         //     }

//         //     // HTTP PATCH


//         //     else if (request.HttpMethod == "PATCH" && request.Url.PathAndQuery.StartsWith("/api/users/"))
//         //     {
//         //         string[] strings = request.Url.PathAndQuery.Split('/');
//         //         string[] parts = strings; // sépare notre URL sur les "/"
//         //         if (parts.Length == 4 && int.TryParse(parts[3], out int id))
//         //         {

//         //             try
//         //             {
//         //             using (var reader = new StreamReader(request.InputStream, request.ContentEncoding))
//         //             {
//         //                 string requestBody = reader.ReadToEnd(); // permet de lire le body de la requete postman json
//         //                 var data = JsonSerializer.Deserialize<Users>(requestBody); //ici data accede au body
                        
//         //                 string firstName = data.User_FirstName;
//         //                 string lastName = data.User_LastName;
//         //                 string email = data.User_Email;
//         //                 string password = data.User_Password;
//         //                 string phone = data.User_Phone;

//         //                 if (firstName == null && lastName == null && email == null && password == null && phone == null)
//         //                 {
//         //                     responseString = "bad body";
//         //                 }
//         //                 else
//         //                 {
//         //                     var options = new JsonSerializerOptions { WriteIndented = true };
//         //                     responseString = JsonSerializer.Serialize(await HttpPatchUserById(id, firstName, lastName, email, password, phone), options);
//         //                 }
//         //             }
//         //             }
//         //             catch (Exception ex)
//         //             {
//         //                 // Gérer l'erreur
//         //                 return $"no or bad body send: {ex.Message}";
//         //             }
//         //         }
//         //         else if (parts.Length > 4)
//         //         {
//         //             responseString = "bad endpoint, Error =  " + (int)HttpStatusCode.BadRequest;
//         //         }
//         //         else if (request.Url.PathAndQuery == "/api/users/")
//         //         {
//         //             responseString = "enter an id please, bad endpoint, Error =  " + (int)HttpStatusCode.BadRequest;
//         //         }
//         //         else
//         //         {
//         //             responseString = "not an Id, Error =  " + (int)HttpStatusCode.BadRequest;
//         //         }
//         //     }

//         //     //final return
              return responseString;
          }



//         private async Task<string> HttpPatchUserById(int id, string firstName, string lastName, string email, string password, string phone)
//         {
//             try
//             {
//                 using (MySqlConnection connection = new MySqlConnection(connectionString))
//                 {
//                     await connection.OpenAsync();

//                     // cette requete permet de mettre a jour seulement les champs non vide
//                     string SqlRequest = "UPDATE users SET ";

//                     List<string> updates = new List<string>();

//                     if (!string.IsNullOrEmpty(firstName))
//                     {
//                         updates.Add("User_FirstName = @FirstName");
//                     }
//                     if (!string.IsNullOrEmpty(lastName))
//                     {
//                         updates.Add("User_LastName = @LastName");
//                     }
//                     if (!string.IsNullOrEmpty(email))
//                     {
//                         updates.Add("User_Email = @Email");
//                     }
//                     if (!string.IsNullOrEmpty(password))
//                     {
//                         updates.Add("User_Password = @Password");
//                     }
//                     if (!string.IsNullOrEmpty(phone))
//                     {
//                         updates.Add("User_Phone = @Phone");
//                     }

//                     SqlRequest += string.Join(", ", updates);
//                     SqlRequest += " WHERE User_Id = @UserId";

//                     //ici on fait des collages pour avoir notre requete sql

//                     Console.WriteLine(SqlRequest);

//                     using (MySqlCommand command = new MySqlCommand(SqlRequest, connection))
//                     {
//                         command.Parameters.AddWithValue("@UserId", id);
//                         command.Parameters.AddWithValue("@FirstName", firstName);
//                         command.Parameters.AddWithValue("@LastName", lastName);
//                         command.Parameters.AddWithValue("@Email", email);
//                         command.Parameters.AddWithValue("@Password", password);
//                         command.Parameters.AddWithValue("@Phone", phone);

//                         int rowsAffected = await command.ExecuteNonQueryAsync();

//                         if (rowsAffected > 0)
//                         {
//                             return "Patch success! User updated!";
//                         }
//                         else
//                         {
//                             return "Invalid id or no rows affected.";
//                         }
//                     }
//                 }
//             }
//             catch (Exception ex)
//             {
//                 // Gérer l'erreur
//                 return $"Error during PATCH: {ex.Message}";
//             }
//         }


//         private async Task<string> HttpPostNewUser(string firstName, string lastName, string email, string password, string phone)
//         {
//             // sur postman, faire la requete avec un body contenant les infos ci dessus
//             try
//             {
//                 using (MySqlConnection connection = new MySqlConnection(connectionString))
//                 {
//                     await connection.OpenAsync();

//                     string SqlRequest = "INSERT INTO users (User_FirstName, User_LastName, User_Email, User_Password, User_Phone) VALUES (@FirstName, @LastName, @Email, @Password, @Phone)";

//                     using (MySqlCommand command = new MySqlCommand(SqlRequest, connection))
//                     { // lie les @ a une string
//                         command.Parameters.AddWithValue("@FirstName", firstName);
//                         command.Parameters.AddWithValue("@LastName", lastName);
//                         command.Parameters.AddWithValue("@Email", email);
//                         command.Parameters.AddWithValue("@Password", password);
//                         command.Parameters.AddWithValue("@Phone", phone);

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


//         private async Task<string> HttpPutUserById(int id, string firstName, string lastName, string email, string password, string phone)
//         {
//             //Put = Update, ou crée si existe pas

//             try
//             {

//              string result = await Task.Run(async () =>
//             {

//                 using (MySqlConnection connection = new MySqlConnection(connectionString))
//                 {
//                     await connection.OpenAsync();

//                     string SqlRequest = "UPDATE users SET User_FirstName = @FirstName, User_LastName = @LastName, User_Email = @Email, User_Password = @Password, User_Phone = @Phone WHERE User_Id = @UserId"; // ma query SQL

//                     using (MySqlCommand command = new MySqlCommand(SqlRequest, connection))
//                     {
//                         command.Parameters.AddWithValue("@UserId", id);
//                         command.Parameters.AddWithValue("@FirstName", firstName);
//                         command.Parameters.AddWithValue("@LastName", lastName);
//                         command.Parameters.AddWithValue("@Email", email); 
//                         command.Parameters.AddWithValue("@Password", password);
//                         command.Parameters.AddWithValue("@Phone", phone);

//                         // permet d'envoyé des données dans la query par un @ en C#

//                     int rowsAffected = await command.ExecuteNonQueryAsync();

//                             if (rowsAffected > 0)
//                             {
//                         return "Its work! User mis a jour! ";
//                             }
//                             else
//                             {
//                                 //cree un user sur le haut de la liste si aucun id atribué
//                                 await HttpPostNewUser(firstName, lastName, email, password, phone);
//                                 return "This Id is empty, New User created";
//                             }
//                         }
//                     }
//                 });

//             return result;

//             }
//             catch (Exception ex)
//             {
//                 // gestion de l'erreur
//                 return $"Error during PUT: {ex.Message}";
//             }
//         }


//         private async Task<IEnumerable<Shoplists>> HttpGetShoplistByUserId(int id)
//         {
        
//         List<Shoplists> shoplists = new List<Shoplists>();

//             try
//             {

//                 using (MySqlConnection connection = new MySqlConnection(connectionString))
//                 {
//                     await connection.OpenAsync();

//                     string SqlRequest = "SELECT * FROM shoplists WHERE User_Id = @UserId"; // ma query SQL

//                     using (MySqlCommand command = new MySqlCommand(SqlRequest, connection))
//                     {
//                         command.Parameters.AddWithValue("@UserId", id); 
//                         // permet d'envoyé des données dans la query par un @ en C#

//                         using (MySqlDataReader reader = await command.ExecuteReaderAsync())
//                         {
//                             while (await reader.ReadAsync())
//                             {
//                                 Shoplists shoplist = new Shoplists
//                                 {
//                                     Shoplist_Id = Convert.ToInt32(reader["Shoplist_Id"]),
//                                     User_Id = Convert.ToInt32(reader["User_Id"])
//                                 };
//                                 shoplists.Add(shoplist);
//                             }
//                         }
//                     }
//                 }
//             }
//             catch (Exception ex)
//             {
//                 // Handle the exception
//                 Console.WriteLine($"Error during shop list retrieval: {ex.Message}");
//             }

//         return shoplists;
//         }

//         private async Task<IEnumerable<Commands>> HttpGetCommandsByUserId(int userId)
//         {
//             List<Commands> commands = new List<Commands>();

//             using (MySqlConnection connection = new MySqlConnection(connectionString))
//             {
//                 await connection.OpenAsync();

//                 // Utilisez une requête SQL avec une jointure pour récupérer les commandes liées au User_Id

//                 //sql name broken sorry that why "c" "s"
//                 string sqlRequest = "SELECT c.* FROM commands c " +
//                                     "JOIN shoplists s ON c.Shoplist_Id = s.Shoplist_Id " +
//                                     "WHERE s.User_Id = @UserId";

//                 using (MySqlCommand command = new MySqlCommand(sqlRequest, connection))
//                 {
//                     command.Parameters.AddWithValue("@UserId", userId);

//                     using (MySqlDataReader reader = await command.ExecuteReaderAsync())
//                     {
//                         while (await reader.ReadAsync())
//                         {
//                             Commands commande = new Commands
//                             {
//                                 Command_Id = Convert.ToInt32(reader["Command_Id"]),
//                                 Shoplist_Id = Convert.ToInt32(reader["Shoplist_Id"]),
//                                 Command_OrderDate = Convert.ToString(reader["Command_OrderDate"])
//                             };
//                             commands.Add(commande);
//                         }
//                     }
//                 }
//             }

//             return commands;
//         }


//         private async Task<IEnumerable<Carts>> HttpGetCartsByUserId(int userId)
//         {
//             List<Carts> carts = new List<Carts>();

//             using (MySqlConnection connection = new MySqlConnection(connectionString))
//             {
//                 await connection.OpenAsync();

//                 // Utilisez une requête SQL avec une jointure pour récupérer les carts liés au User_Id

//                 //sql name broken sorry that why "c" "s"
//                 string sqlRequest = "SELECT c.* FROM carts c " +
//                                     "JOIN shoplists s ON c.Shoplist_Id = s.Shoplist_Id " +
//                                     "WHERE s.User_Id = @UserId";

//                 using (MySqlCommand command = new MySqlCommand(sqlRequest, connection))
//                 {
//                     command.Parameters.AddWithValue("@UserId", userId);

//                     using (MySqlDataReader reader = await command.ExecuteReaderAsync())
//                     {
//                         while (await reader.ReadAsync())
//                         {
//                             Carts cart = new Carts
//                             {
//                                 Shoplist_Id = Convert.ToInt32(reader["Shoplist_Id"]),
//                                 Product_Id = Convert.ToInt32(reader["Product_Id"]),
//                                 Cart_Total = Convert.ToInt32(reader["Cart_Total"]),
//                                 Cart_ProductCount = Convert.ToInt32(reader["Cart_ProductCount"])
//                             };
//                             carts.Add(cart);
//                         }
//                     }
//                 }
//             }

//             return carts;
//         }

//         private async Task<IEnumerable<Invoices>> HttpGetInvoicesByUserId(int userId)
//         {
//             List<Invoices> invoices = new List<Invoices>();

//             using (MySqlConnection connection = new MySqlConnection(connectionString))
//             {
//                 await connection.OpenAsync();

//                 // Utilisez une requête SQL avec une jointure pour récupérer les factures liées au User_Id

//                 //sql name broken sorry that why "c" "s"
//                 string sqlRequest = "SELECT i.* FROM invoices i " +
//                                     "JOIN commands c ON i.Command_Id = c.Command_Id " +
//                                     "JOIN shoplists s ON c.Shoplist_Id = s.Shoplist_Id " +
//                                     "WHERE s.User_Id = @UserId";

//                 using (MySqlCommand command = new MySqlCommand(sqlRequest, connection))
//                 {
//                     command.Parameters.AddWithValue("@UserId", userId);

//                     using (MySqlDataReader reader = await command.ExecuteReaderAsync())
//                     {
//                         while (await reader.ReadAsync())
//                         {
//                             Invoices invoice = new Invoices
//                             {
//                                 Invoices_Id = Convert.ToInt32(reader["Invoices_Id"]),
//                                 Command_Id = Convert.ToInt32(reader["Command_Id"]),
//                                 Invoice_Date = Convert.ToDateTime(reader["Invoice_Date"]).ToString("yyyy-MM-dd")
//                             };
//                             invoices.Add(invoice);
//                         }
//                     }
//                 }
//             }

//             return invoices;
//         }



//         private async Task<Users> HttpGetUserById(int id)
//         {
            
//         Users user = null;

//             using (MySqlConnection connection = new MySqlConnection(connectionString))
//             {
//                 await connection.OpenAsync();

//                 string SqlRequest = "SELECT * FROM users WHERE User_Id = @UserId"; // ma query SQL

//                 using (MySqlCommand command = new MySqlCommand(SqlRequest, connection))
//                 {
//                     command.Parameters.AddWithValue("@UserId", id); 
//                     // permet d'envoyé des données dans la query par un @ en C#

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

//             return user;
//         }

//         private async Task<Users> HttpGetUserByLastName(string name)
//         {
            
//         Users user = null;

//             using (MySqlConnection connection = new MySqlConnection(connectionString))
//             {
//                 await connection.OpenAsync();

//                 string SqlRequest = "SELECT * FROM users WHERE User_LastName = @UserLastName"; // ma query SQL

//                 using (MySqlCommand command = new MySqlCommand(SqlRequest, connection))
//                 {
//                     command.Parameters.AddWithValue("@UserLastName", name); 
//                     // permet d'envoyé des données dans la query par un @ en C#

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

//             return user;
//         }

//         private async Task<Users> HttpGetUserByEmail(string email)
//         {
            
//         Users user = null;

//             using (MySqlConnection connection = new MySqlConnection(connectionString))
//             {
//                 await connection.OpenAsync();

//                 string SqlRequest = "SELECT * FROM users WHERE User_Email = @UserEmail"; // ma query SQL

//                 using (MySqlCommand command = new MySqlCommand(SqlRequest, connection))
//                 {
//                     command.Parameters.AddWithValue("@UserEmail", email); 
//                     // permet d'envoyé des données dans la query par un @ en C#

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

//             return user;
//         }

//         private async Task<string> HttpDelUserById(int id)
//         {

//             try
//             {
//                 using (MySqlConnection connection = new MySqlConnection(connectionString))
//                 {
//                     await connection.OpenAsync();

//                     string SqlRequest = "DELETE FROM users WHERE User_Id = @UserId"; // ma query SQL

//                     using (MySqlCommand command = new MySqlCommand(SqlRequest, connection))
//                     {
//                         command.Parameters.AddWithValue("@UserId", id); 
//                         // permet d'envoyé des données dans la query par un @ en C#

//                         int rowsAffected = await command.ExecuteNonQueryAsync();

//                         if (rowsAffected > 0)
//                         {
//                             return "Its work! User supprimer! ";
//                         }
//                         else
//                         {
//                             return "Invalid id, Error =  " + (int)HttpStatusCode.BadRequest;
//                         }
//                     }
//                 }
//             }
//             catch (Exception ex)
//             {
//                 // gestion de l'erreur
//                 return $"Error during DEL: {ex.Message}";
//             }
//         }







    }

 }
