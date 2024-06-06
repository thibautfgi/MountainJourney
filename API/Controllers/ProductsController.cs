
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
    public class ProductsController
    {
        private string connectionString = "Server=localhost;User ID=root;Password=azerty;Database=newschema";
        // mes identifiants pour me connect a mon mysql workbench

// TODO: 
// gener mieux la connection sql = 1 connection


        public async Task<string> ProcessRequest(HttpListenerRequest request)
        {
            string responseString = "";

            //GET

            if (request.HttpMethod == "GET" && request.Url.PathAndQuery == "/api/products")
            {
                var options = new JsonSerializerOptions { WriteIndented = true }; //cette ligne rend le json html jolie
                responseString = JsonSerializer.Serialize(await HttpGetAllProducts(), options);
            }
            else if (request.HttpMethod == "GET" && request.Url.PathAndQuery.StartsWith("/api/products/"))
            {
                string[] strings = request.Url.PathAndQuery.Split('/');
                string[] parts = strings; // separe notre url sur les "/"
                if (parts.Length == 4 && int.TryParse(parts[3], out int id))
                {
                    var options = new JsonSerializerOptions { WriteIndented = true }; //cette ligne rend le json html jolie
                    responseString = JsonSerializer.Serialize(await HttpGetProductById(id), options);
                    if (responseString == "null")
                    {
                    responseString = "Invalid id, Error =  " + (int)HttpStatusCode.BadRequest;
                    }

                }
                else if (parts.Length > 4)
                {
                    responseString = "bad endpoint, Error =  " + (int)HttpStatusCode.BadRequest;
                }
                else if (request.Url.PathAndQuery == "/api/products/")
                {
                    responseString = "enter a id please, bad endpoint, Error =  " + (int)HttpStatusCode.BadRequest;
                }
                else
                {
                    string myEndPointString = parts[3];

                        var options = new JsonSerializerOptions { WriteIndented = true }; //cette ligne rend le json html jolie

                        responseString = JsonSerializer.Serialize(await HttpGetProductByName(myEndPointString), options);


                        if (responseString == "null")
                        {
                            responseString = JsonSerializer.Serialize(await HttpGetAllProductByType(myEndPointString), options);    
                        }
                        
                        
                        if (responseString == "null")
                        {
                        responseString = "Invalid Name or Type of products, Error =  " + (int)HttpStatusCode.BadRequest;
                        } 
                    
                }
            }

            // POST


            else if (request.HttpMethod == "POST" && request.Url.PathAndQuery == "/api/products")
            {
                using (var reader = new StreamReader(request.InputStream, request.ContentEncoding))
                {
                    string requestBody = reader.ReadToEnd(); // permet de lire le body de la requete postman json
                    var data = JsonSerializer.Deserialize<Products>(requestBody); //ici data accede au body

                    string name = data.Product_Name;
                    string description = data.Product_Description;
                    string type = data.Product_Type;
                    int numberLeft = data.Product_NumberLeft;
                    int price = data.Product_Price;

                    responseString = await HttpPostNewProduct(name, description, type, numberLeft, price);
                }
            }

            else if (request.HttpMethod == "POST" && request.Url.PathAndQuery.StartsWith("/api/products"))
            {
                responseString = "bad endpoint, Error =  " + (int)HttpStatusCode.BadRequest;
            }


            //PUT

            else if (request.HttpMethod == "PUT" && request.Url.PathAndQuery.StartsWith("/api/products/"))
            {
                string[] strings = request.Url.PathAndQuery.Split('/');
                string[] parts = strings; // separe notre url sur les "/"
                if (parts.Length == 4 && int.TryParse(parts[3], out int id))
                {
                    try
                    {
                        using (var reader = new StreamReader(request.InputStream, request.ContentEncoding))
                        {
                            string requestBody = reader.ReadToEnd(); // permet de lire le body de la requete postman json
                            var data = JsonSerializer.Deserialize<Products>(requestBody); //ici data accede au body

                            string name = data.Product_Name;
                            string description = data.Product_Description;
                            string type = data.Product_Type;
                            int numberLeft = data.Product_NumberLeft;
                            int price = data.Product_Price;

                            
                            var options = new JsonSerializerOptions { WriteIndented = true }; //cette ligne rend le json html jolie
                            responseString = JsonSerializer.Serialize(await HttpPutProductById(id, name, description, type, numberLeft, price), options);
                        }
                    }
                    catch (Exception ex)
                    {
                        // Gérer l'erreur
                        return $"no or bad body send: {ex.Message}";
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
                    responseString = "not a Id, Error =  " + (int)HttpStatusCode.BadRequest;
                }
            }

            //DELETE

            else if (request.HttpMethod == "DELETE" && request.Url.PathAndQuery.StartsWith("/api/products/"))
            {
                string[] strings = request.Url.PathAndQuery.Split('/');
                string[] parts = strings; // separe notre url sur les "/"
                if (parts.Length == 4 && int.TryParse(parts[3], out int id))
                {
                    var options = new JsonSerializerOptions { WriteIndented = true }; //cette ligne rend le json html jolie
                    responseString = JsonSerializer.Serialize(await HttpDelProductById(id), options);

                }
                else if (parts.Length > 4)
                {
                    responseString = "bad endpoint, Error =  " + (int)HttpStatusCode.BadRequest;
                }
                else if (request.Url.PathAndQuery == "/api/products/")
                {
                    responseString = "enter a id please, bad endpoint, Error =  " + (int)HttpStatusCode.BadRequest;
                }
                else
                {
                    responseString = "not a Id, Error =  " + (int)HttpStatusCode.BadRequest;
                }
            }

            // HTTP PATCH


            else if (request.HttpMethod == "PATCH" && request.Url.PathAndQuery.StartsWith("/api/products/"))
            {
                string[] strings = request.Url.PathAndQuery.Split('/');
                string[] parts = strings; // sépare notre URL sur les "/"
                if (parts.Length == 4 && int.TryParse(parts[3], out int id))
                {

                    try
                    {
                    using (var reader = new StreamReader(request.InputStream, request.ContentEncoding))
                    {
                        string requestBody = reader.ReadToEnd(); // permet de lire le body de la requete postman json
                        var data = JsonSerializer.Deserialize<Products>(requestBody); //ici data accede au body
                        
                            string name = data.Product_Name;
                            string description = data.Product_Description;
                            string type = data.Product_Type;
                            int numberLeft = data.Product_NumberLeft;
                            int price = data.Product_Price;


                        if (name == null && description == null && type == null && numberLeft == null && price == null)
                        {
                            responseString = "bad body";
                        }
                        else
                        {
                            var options = new JsonSerializerOptions { WriteIndented = true };
                            responseString = JsonSerializer.Serialize(await HttpPatchProductById(id, name, description, type, numberLeft, price), options);
                        }
                    }
                    }
                    catch (Exception ex)
                    {
                        // Gérer l'erreur
                        return $"no or bad body send: {ex.Message}";
                    }
                }
                else if (parts.Length > 4)
                {
                    responseString = "bad endpoint, Error =  " + (int)HttpStatusCode.BadRequest;
                }
                else if (request.Url.PathAndQuery == "/api/products/")
                {
                    responseString = "enter an id please, bad endpoint, Error =  " + (int)HttpStatusCode.BadRequest;
                }
                else
                {
                    responseString = "not an Id, Error =  " + (int)HttpStatusCode.BadRequest;
                }
            }

            //final return
            return responseString;
        }



        private async Task<string> HttpPatchProductById(int id, string name, string description, string type, int numberLeft, int price)
        {
            try
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    await connection.OpenAsync();

                    // cette requete permet de mettre a jour seulement les champs non vide
                    string SqlRequest = "UPDATE products SET ";

                    List<string> updates = new List<string>();

                    string ToStringNumberLeft = numberLeft.ToString();
                    string ToStringPrice = price.ToString();

                    if (!string.IsNullOrEmpty(name))
                    {
                        updates.Add("Product_Name = @Name");
                    }
                    if (!string.IsNullOrEmpty(description))
                    {
                        updates.Add("Product_Description = @Description");
                    }
                    if (!string.IsNullOrEmpty(type))
                    {
                        updates.Add("Product_Type = @Type");
                    }
                    if (!string.IsNullOrEmpty(ToStringNumberLeft))
                    {
                        updates.Add("Product_NumberLeft = @NumberLeft");
                    }
                    if (!string.IsNullOrEmpty(ToStringPrice))
                    {
                        updates.Add("Product_Price = @Price");
                    }

                    SqlRequest += string.Join(", ", updates);
                    SqlRequest += " WHERE Product_Id = @ProductId";

                    //ici on fait des collages pour avoir notre requete sql

                    Console.WriteLine(SqlRequest);

                    using (MySqlCommand command = new MySqlCommand(SqlRequest, connection))
                    {
                        command.Parameters.AddWithValue("@ProductId", id);
                        command.Parameters.AddWithValue("@Name", name);
                        command.Parameters.AddWithValue("@Description", description);
                        command.Parameters.AddWithValue("@Type", type);
                        command.Parameters.AddWithValue("@NumberLeft", numberLeft);
                        command.Parameters.AddWithValue("@Price", price);

                        int rowsAffected = await command.ExecuteNonQueryAsync();

                        if (rowsAffected > 0)
                        {
                            return "Patch success! Product updated!";
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


        private async Task<string> HttpPostNewProduct(string name, string description, string type, int numberLeft , int price)
        {
            // sur postman, faire la requete avec un body contenant les infos ci dessus
            try
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    await connection.OpenAsync();

                    string SqlRequest = "INSERT INTO products (Product_Name, Product_Description, Product_Type, Product_NumberLeft, Product_Price) VALUES (@Name, @Description, @Type, @NumberLeft, @Price)";

                    using (MySqlCommand command = new MySqlCommand(SqlRequest, connection))
                    { // lie les @ a une string
                        command.Parameters.AddWithValue("@Name", name);
                        command.Parameters.AddWithValue("@Description", description);
                        command.Parameters.AddWithValue("@Type", type);
                        command.Parameters.AddWithValue("@NumberLeft", numberLeft);
                        command.Parameters.AddWithValue("@Price", price);

                        int rowsAffected = await command.ExecuteNonQueryAsync();

                        if (rowsAffected > 0)
                        {
                            return "Its work! Post effectué! ";
                        }
                        else
                        {
                            return "Post failled, no row creat";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // gestion de l'erreur
                return $"Error during post: {ex.Message}";
            }
        }


        private async Task<IEnumerable<Products>> HttpGetAllProducts()
        {

        List<Products> products = new List<Products>(); //cree une liste vide

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                await connection.OpenAsync();

                string SqlRequest = "SELECT * FROM products"; // recupère tt les products

                using (MySqlCommand command = new MySqlCommand(SqlRequest, connection))
                {
                    using (MySqlDataReader reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            Products product = new Products // retourne en object les données de ma base SQL
                            {
                                Product_Id = Convert.ToInt32(reader["Product_Id"]),
                                Product_Name = reader["Product_Name"].ToString(),
                                Product_Description = reader["Product_Description"].ToString(),
                                Product_Type = reader["Product_Type"].ToString(),
                                Product_NumberLeft = Convert.ToInt32(reader["Product_NumberLeft"]),
                                Product_Price = Convert.ToInt32(reader["Product_Price"]),
                            };
                            products.Add(product);
                        }
                    }
                }
            }
            return products;
        }


        private async Task<string> HttpPutProductById(int id, string name, string description, string type, int numberLeft, int price)
        {
            //Put = Update, ou crée si existe pas

            try
            {

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                await connection.OpenAsync();

                string SqlRequest = "UPDATE products SET Product_Name = @Name, Product_Description = @Description, Product_Type = @Type, Product_NumberLeft = @NumberLeft, Product_Price = @Price WHERE Product_Id = @ProductId"; // ma query SQL

                using (MySqlCommand command = new MySqlCommand(SqlRequest, connection))
                {
                    command.Parameters.AddWithValue("@ProductId", id);
                    command.Parameters.AddWithValue("@Name", name);
                    command.Parameters.AddWithValue("@Description", description);
                    command.Parameters.AddWithValue("@Type", type); 
                    command.Parameters.AddWithValue("@NumberLeft", numberLeft);
                    command.Parameters.AddWithValue("@Price", price);

                    // permet d'envoyé des données dans la query par un @ en C#

                   int rowsAffected = await command.ExecuteNonQueryAsync();

                        if (rowsAffected > 0)

                        {
                            return "Its work! Product mis a jour! ";
                        }
                        else
                        {
                            //cree un product sur le haut de la liste si aucun id atribué
                            await HttpPostNewProduct(name, description, type, numberLeft, price);
                            return "This Id is empty, New Product created";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // gestion de l'erreur
                return $"Error during PUT: {ex.Message}";
            }
        }


        private async Task<Products> HttpGetProductById(int id)
        {
            
        Products product = null;

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                await connection.OpenAsync();

                string SqlRequest = "SELECT * FROM products WHERE Product_Id = @ProductId"; // ma query SQL

                using (MySqlCommand command = new MySqlCommand(SqlRequest, connection))
                {
                    command.Parameters.AddWithValue("@ProductId", id); 
                    // permet d'envoyé des données dans la query par un @ en C#

                    using (MySqlDataReader reader = await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            product = new Products
                            {
                                Product_Id = Convert.ToInt32(reader["Product_Id"]),
                                Product_Name = reader["Product_Name"].ToString(),
                                Product_Description = reader["Product_Description"].ToString(),
                                Product_Type = reader["Product_Type"].ToString(),
                                Product_NumberLeft = Convert.ToInt32(reader["Product_NumberLeft"]),
                                Product_Price = Convert.ToInt32(reader["Product_Price"]),
                            };
                        }
                    }
                }
            }

            return product;
        }

        private async Task<Products> HttpGetProductByName(string name)
        {
            
        Products product = null;

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                await connection.OpenAsync();

                string SqlRequest = "SELECT * FROM products WHERE Product_Name = @ProductName"; // ma query SQL

                using (MySqlCommand command = new MySqlCommand(SqlRequest, connection))
                {
                    command.Parameters.AddWithValue("@ProductName", name); 
                    // permet d'envoyé des données dans la query par un @ en C#

                    using (MySqlDataReader reader = await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            product = new Products
                            {
                                Product_Id = Convert.ToInt32(reader["Product_Id"]),
                                Product_Name = reader["Product_Name"].ToString(),
                                Product_Description = reader["Product_Description"].ToString(),
                                Product_Type = reader["Product_Type"].ToString(),
                                Product_NumberLeft = Convert.ToInt32(reader["Product_NumberLeft"]),
                                Product_Price = Convert.ToInt32(reader["Product_Price"]),
                            };
                        }
                    }
                }
            }

            return product;
        }

        private async Task<IEnumerable<Products>> HttpGetAllProductByType(string type)
        {
            
        List<Products> products = new List<Products>();

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                await connection.OpenAsync();

                string SqlRequest = "SELECT * FROM products WHERE Product_Type = @ProductType"; // ma query SQL

                using (MySqlCommand command = new MySqlCommand(SqlRequest, connection))
                {
                    command.Parameters.AddWithValue("@ProductType", type); 
                    // permet d'envoyé des données dans la query par un @ en C#

                    using (MySqlDataReader reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                             Products product = new Products
                            {
                                Product_Id = Convert.ToInt32(reader["Product_Id"]),
                                Product_Name = reader["Product_Name"].ToString(),
                                Product_Description = reader["Product_Description"].ToString(),
                                Product_Type = reader["Product_Type"].ToString(),
                                Product_NumberLeft = Convert.ToInt32(reader["Product_NumberLeft"]),
                                Product_Price = Convert.ToInt32(reader["Product_Price"]),
                            };
                            products.Add(product);
                        }
                    }
                }
            }

            return products;
        }

        private async Task<string> HttpDelProductById(int id)
        {

            try
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    await connection.OpenAsync();

                    string SqlRequest = "DELETE FROM products WHERE Product_Id = @ProductId"; // ma query SQL

                    using (MySqlCommand command = new MySqlCommand(SqlRequest, connection))
                    {
                        command.Parameters.AddWithValue("@ProductId", id); 
                        // permet d'envoyé des données dans la query par un @ en C#

                        int rowsAffected = await command.ExecuteNonQueryAsync();

                        if (rowsAffected > 0)
                        {
                            return "Its work! Product supprimer! ";
                        }
                        else
                        {
                            return "Invalid id, Error =  " + (int)HttpStatusCode.BadRequest;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // gestion de l'erreur
                return $"Error during DEL: {ex.Message}";
            }
        }

    }

}
