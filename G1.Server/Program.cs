using G1.Model;
using G1.Server;

var builder = WebApplication.CreateBuilder(args);
builder.WebHost.UseUrls("http://localhost:9080");
var app = builder.Build();

var webSocketOptions = new WebSocketOptions
{
    KeepAliveInterval = TimeSpan.FromMinutes(2)
};

app.UseWebSockets(webSocketOptions);

app.MapGet("/", () => "Hello World!");

app.Use(async (context, next) =>
{
    Console.WriteLine($"Connection attempt '{context.Request.Path}'");
    if (Helpers.IsClientConnection(context.Request.Path, out var id))
    {
        if (context.WebSockets.IsWebSocketRequest && WorldEntityId.TryParse(id, out var clientId))
        {
            using var webSocket = await context.WebSockets.AcceptWebSocketAsync();
            await ClientConnect.Connect(clientId, webSocket);
        }
        else
        {
            context.Response.StatusCode = StatusCodes.Status400BadRequest;
        }
    }
    else
    {
        await next(context);
    }

});

app.Run();
