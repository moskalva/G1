using System.Net.WebSockets;
using G1.Model;

namespace G1.Server;

public class ClientConnect
{
    // TODO:
    // Handle client silence as disconnect
    // Apply reusable in/out memory buffer
    public static async Task Connect(WorldEntityId clientId, WebSocket webSocket)
    {
        var buffer = new byte[1024 * 4];
        var dummyState = new WorldEntityState
        {
            Id = clientId,
            Type = WorldEntityType.Ship,
            Position = World3dVector.Zero,
            Velocity = World3dVector.Zero,
        };
        var stateBytes = SerializerHelpers.Serialize(dummyState);
        stateBytes.CopyTo(buffer, 0);
        var data = new ArraySegment<byte>(buffer, 0, stateBytes.Length);

        await webSocket.SendAsync(data, WebSocketMessageType.Binary, true, CancellationToken.None);

        var receiveResult = await webSocket.ReceiveAsync(
            new ArraySegment<byte>(buffer), CancellationToken.None);

        while (!receiveResult.CloseStatus.HasValue)
        {
            Console.WriteLine("Waiting for response");
            receiveResult = await webSocket.ReceiveAsync(
                new ArraySegment<byte>(buffer), CancellationToken.None);
            data = new ArraySegment<byte>(buffer, 0, receiveResult.Count);
            Log(data.ToArray());
        }

        Console.WriteLine($"Closing connection. '{webSocket.CloseStatus}' : '{webSocket.CloseStatusDescription}'");
        await webSocket.CloseAsync(
            receiveResult.CloseStatus.Value,
            receiveResult.CloseStatusDescription,
            CancellationToken.None);
    }

    private static void Log(byte[] data)
    {
        var s = SerializerHelpers.Deserialize<WorldEntityState>(data);
        Console.WriteLine("New message:\n========================");
        Console.WriteLine(s);
        Console.WriteLine("========================");
    }
}
