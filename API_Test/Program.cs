using Models;
using MySqlConnector;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;

class SimpleHttpServer
{
    private HttpListener listener;
    private List<Users> user;
    private string connectionString = "Server=localhost;User ID=root;Password=azerty;Database=mj";

    public SimpleHttpServer(string prefixes)
    {
        listener = new HttpListener();
        listener.Prefixes.Add(prefixes);
    }

    public async Task Start()
    {
        listener.Start();
        Console.WriteLine("Server test started. Listening for requests...");
        Console.WriteLine("http://localhost:8080/");

        while (true)
        {
            try
            {
                HttpListenerContext context = await listener.GetContextAsync().ConfigureAwait(false);
                await MyProcessRequest(context);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error handling request: {ex.Message}");
            }
        }
    }

    private async Task MyProcessRequest(HttpListenerContext context)
    {
        HttpListenerRequest request = context.Request;
        HttpListenerResponse response = context.Response;

        string responseString = "";

        if (request.HttpMethod == "GET" && request.Url.PathAndQuery == "/api/status")
        {
            responseString = await HttpTestConnectionDb();
        }

                else if (request.Url.AbsolutePath.StartsWith("/api/users")) // tchek si l'url commence avec api/users
        {
            responseString = await new Controllers.UsersController().ProcessRequest(request);
        }

        else if (request.Url.AbsolutePath.StartsWith("/api/comments")) // tchek si l'url commence avec api/comments
        {
            responseString = await new Controllers.CommentsController().ProcessRequest(request);
        }

        else if (request.Url.AbsolutePath.StartsWith("/api/friendlists")) // tchek si l'url commence avec api/friendlists
        {
            responseString = await new Controllers.FriendlistsController().ProcessRequest(request);
        }

        else if (request.Url.AbsolutePath.StartsWith("/api/likes")) // tchek si l'url commence avec api/likes
        {
            responseString = await new Controllers.LikesController().ProcessRequest(request);
        }

        // else if (request.Url.AbsolutePath.StartsWith("/api/maps")) // tchek si l'url commence avec api/maps
        // {
        //     responseString = await new Controllers.MapsController().ProcessRequest(request);
        // }

        else if (request.Url.AbsolutePath.StartsWith("/api/marks")) // tchek si l'url commence avec api/marks
        {
            responseString = await new Controllers.MarksController().ProcessRequest(request);
        }


        else if (request.Url.AbsolutePath.StartsWith("/api/routes")) // tchek si l'url commence avec api/routes
        {
            responseString = await new Controllers.RoutesController().ProcessRequest(request);
        }

        else if (request.Url.AbsolutePath.StartsWith("/api/tokens")) // tchek si l'url commence avec api/tokens
        {
            responseString = await new Controllers.TokensController().ProcessRequest(request);
        }

        else
        {
            responseString = "Invalid endpoint, Error = " + (int)HttpStatusCode.NotFound;
            response.StatusCode = (int)HttpStatusCode.NotFound;
        }

        byte[] buffer = System.Text.Encoding.UTF8.GetBytes(responseString);
        response.ContentLength64 = buffer.Length;
        using (Stream output = response.OutputStream)
        {
            await output.WriteAsync(buffer, 0, buffer.Length);
        }
    }

    private async Task<string> HttpTestConnectionDb()
    {
        try
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                await connection.OpenAsync();
                return "Database connection successful! Mountain Journey is alive!";
            }
        }
        catch (Exception ex)
        {
            return $"Database connection failed: {ex.Message}";
        }
    }
}

class Program
{
    static async Task Main()
    {
        string prefixes = "http://localhost:8080/";
        SimpleHttpServer server = new SimpleHttpServer(prefixes);

        await server.Start();
    }
}
