
using Microsoft.VisualBasic;
using Models;
using MySqlConnector;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.Json;

namespace Controllers
{
    public class CartsController
    {
        private string connectionString = "Server=localhost;User ID=root;Password=azerty;Database=newschema";
        // mes identifiants pour me connect a mon mysql workbench

// TODO: 
// gener mieux la connection sql = 1 connection


        public async Task<string> ProcessRequest(HttpListenerRequest request)
        {
            string responseString = "";

            // GET
            if (request.HttpMethod == "GET" && request.Url.PathAndQuery == "/api/carts")
            {
                var options = new JsonSerializerOptions { WriteIndented = true };
                responseString = JsonSerializer.Serialize(await HttpGetAllCarts(), options);
            }
            else if (request.HttpMethod == "GET" && request.Url.PathAndQuery.StartsWith("/api/carts"))
            {
                string[] strings = request.Url.PathAndQuery.Split('/');
                string[] parts = strings;
                if (parts.Length == 4 && int.TryParse(parts[3], out int id))
                {
                    var options = new JsonSerializerOptions { WriteIndented = true };
                    responseString = JsonSerializer.Serialize(await HttpGetCartById(id), options);
                    if (responseString == "null")
                    {
                        responseString = "Invalid id, Error =  " + (int)HttpStatusCode.BadRequest;
                    }
                }
                else if (parts.Length > 4)
                {
                    responseString = "bad endpoint, Error =  " + (int)HttpStatusCode.BadRequest;
                }
                else if (request.Url.PathAndQuery == "/api/carts/")
                {
                    responseString = "enter an id please, bad endpoint, Error =  " + (int)HttpStatusCode.BadRequest;
                }
                else
                {
                    responseString = "not an id, please enter a valid id";
                }
            }

            // POST
            else if (request.HttpMethod == "POST" && request.Url.PathAndQuery == "/api/carts")
            {
                using (var reader = new StreamReader(request.InputStream, request.ContentEncoding))
                {
                    string requestBody = reader.ReadToEnd();
                    var data = JsonSerializer.Deserialize<Carts>(requestBody);

                    int shoplistId = data.Shoplist_Id;
                    int productId = data.Product_Id;
                    int cartTotal = data.Cart_Total;
                    int cartProductCount = data.Cart_ProductCount;

                    responseString = await HttpPostNewCart(shoplistId, productId, cartTotal, cartProductCount);
                }
            }
            else if (request.HttpMethod == "POST" && request.Url.PathAndQuery.StartsWith("/api/carts"))
            {
                responseString = "bad endpoint, Error =  " + (int)HttpStatusCode.BadRequest;
            }

            // PUT
            //pas de put pour carts

            // DELETE
            else if (request.HttpMethod == "DELETE" && request.Url.PathAndQuery.StartsWith("/api/carts/"))
            {
                string[] strings = request.Url.PathAndQuery.Split('/');
                string[] parts = strings;
                if (parts.Length == 4 && int.TryParse(parts[3], out int id))
                {
                    var options = new JsonSerializerOptions { WriteIndented = true };
                    responseString = JsonSerializer.Serialize(await HttpDelCartById(id), options);
                }
                else if (parts.Length > 4)
                {
                    responseString = "bad endpoint, Error =  " + (int)HttpStatusCode.BadRequest;
                }
                else if (request.Url.PathAndQuery == "/api/carts/")
                {
                    responseString = "enter an id please, bad endpoint, Error =  " + (int)HttpStatusCode.BadRequest;
                }
                else
                {
                    responseString = "not an Id, Error =  " + (int)HttpStatusCode.BadRequest;
                }
            }

            // HTTP PATCH
            //pas de patch pour carts

            // final return
            return responseString;
        }



        //FIXME: pb d'exeption lors d'un mauvais body entre dans la nouvelle shoplist a gerer.
        private async Task<string> HttpPostNewCart(int shoplistId, int productId, int cartTotal, int cartProductCount)
        {
            try
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    await connection.OpenAsync();

                    string SqlRequest = "INSERT INTO carts (Shoplist_Id, Product_Id, Cart_Total, Cart_ProductCount) " +
                                        "VALUES (@ShoplistId, @ProductId, @CartTotal, @CartProductCount)";

                    using (MySqlCommand command = new MySqlCommand(SqlRequest, connection))
                    {
                        command.Parameters.AddWithValue("@ShoplistId", shoplistId);
                        command.Parameters.AddWithValue("@ProductId", productId);
                        command.Parameters.AddWithValue("@CartTotal", cartTotal);
                        command.Parameters.AddWithValue("@CartProductCount", cartProductCount);

                        int rowsAffected = await command.ExecuteNonQueryAsync();

                        if (rowsAffected > 0)
                        {
                            return "It works! Post effectué!";
                        }
                        else
                        {
                            return "Post failed, no row created";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Handle the error
                return $"Error during post: {ex.Message}";
            }
        }



        private async Task<IEnumerable<Carts>> HttpGetAllCarts()
        {
            List<Carts> carts = new List<Carts>();

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                await connection.OpenAsync();

                string SqlRequest = "SELECT * FROM carts";

                using (MySqlCommand command = new MySqlCommand(SqlRequest, connection))
                {
                    using (MySqlDataReader reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            Carts cart = new Carts
                            {
                                Shoplist_Id = Convert.ToInt32(reader["Shoplist_Id"]),
                                Product_Id = Convert.ToInt32(reader["Product_Id"]),
                                Cart_Total = Convert.ToInt32(reader["Cart_Total"]),
                                Cart_ProductCount = Convert.ToInt32(reader["Cart_ProductCount"])
                            };
                            carts.Add(cart);
                        }
                    }
                }
            }
            return carts;
        }



        private async Task<Carts> HttpGetCartById(int id)
        {
            Carts cart = null;

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                await connection.OpenAsync();

                string SqlRequest = "SELECT * FROM carts WHERE Shoplist_Id = @ShoplistId";

                using (MySqlCommand command = new MySqlCommand(SqlRequest, connection))
                {
                    command.Parameters.AddWithValue("@ShoplistId", id);

                    using (MySqlDataReader reader = await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            cart = new Carts
                            {
                                Shoplist_Id = Convert.ToInt32(reader["Shoplist_Id"]),
                                Product_Id = Convert.ToInt32(reader["Product_Id"]),
                                Cart_Total = Convert.ToInt32(reader["Cart_Total"]),
                                Cart_ProductCount = Convert.ToInt32(reader["Cart_ProductCount"])
                            };
                        }
                    }
                }
            }

            return cart;
        }


        private async Task<string> HttpDelCartById(int id)
        {
            try
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    await connection.OpenAsync();

                    string SqlRequest = "DELETE FROM carts WHERE Shoplist_Id = @ShoplistId"; // Ma requête SQL

                    using (MySqlCommand command = new MySqlCommand(SqlRequest, connection))
                    {
                        command.Parameters.AddWithValue("@ShoplistId", id);
                        // Permet d'envoyer des données dans la requête par un @ en C#

                        int rowsAffected = await command.ExecuteNonQueryAsync();

                        if (rowsAffected > 0)
                        {
                            return "Delete success! Cart supprimé!";
                        }
                        else
                        {
                            return "Invalid id, Error = " + (int)HttpStatusCode.BadRequest;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Gérer l'erreur
                return $"Erreur lors du DELETE : {ex.Message}";
            }
        }


    }

}
