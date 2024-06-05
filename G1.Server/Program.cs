using G1.Model;
using G1.Server;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<IClientAgentProvider,ClientAgentProvider>();

builder.WebHost.UseUrls("http://localhost:9080");
var app = builder.Build();

var webSocketOptions = new WebSocketOptions
{
    KeepAliveInterval = TimeSpan.FromMinutes(2)
};

app.UseWebSockets(webSocketOptions);

app.MapGet("/", () => "Hello World!");

var agentProvider = app.Services.GetRequiredService<IClientAgentProvider>();
app.Use(async (context, next) =>
{
    Console.WriteLine($"Connection attempt '{context.Request.Path}'");
    if (Helpers.IsClientConnection(context.Request.Path, out var id))
    {
        if (context.WebSockets.IsWebSocketRequest && WorldEntityId.TryParse(id, out var clientId))
        {
            var client = agentProvider.GetAgent(clientId);
            using var webSocket = await context.WebSockets.AcceptWebSocketAsync();
            await ClientConnect.Connect(client, webSocket);
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
