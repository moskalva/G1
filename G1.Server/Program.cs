using Orleans.Runtime;

using G1.Model;
using G1.Server;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseOrleans(static siloBuilder =>
{
    siloBuilder.UseLocalhostClustering();
    siloBuilder.AddMemoryGrainStorage("agents");
});
builder.WebHost.UseUrls("http://+*:9080");
var app = builder.Build();

var webSocketOptions = new WebSocketOptions
{
    KeepAliveInterval = TimeSpan.FromMinutes(2)
};

app.UseWebSockets(webSocketOptions);

app.MapGet("/", () => "Hello World!");

var grainFactory = app.Services.GetRequiredService<IGrainFactory>();
app.Use(async (context, next) =>
{
    Console.WriteLine($"Connection attempt '{context.Request.Path}'");
    if (Helpers.IsClientConnection(context.Request.Path, out var id))
    {
        if (context.WebSockets.IsWebSocketRequest && WorldEntityId.TryParse(id, out var clientId))
        {
            using var webSocket = await context.WebSockets.AcceptWebSocketAsync();
            await ClientConnect.Connect(clientId,grainFactory, webSocket);
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
