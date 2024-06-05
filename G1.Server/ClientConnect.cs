using System.Net.WebSockets;
using G1.Model;

namespace G1.Server;

public class ClientConnect
{
    // TODO:
    // Handle client silence as disconnect
    // Apply reusable in/out memory buffer
    public static async Task Connect(IClientAgent agent, WebSocket webSocket)
    {
        var buffer = new byte[1024 * 4];

        var initialState = await agent.GetState();
        var stateBytes = SerializerHelpers.Serialize(initialState);
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
            var updatedState = SerializerHelpers.Deserialize<WorldEntityState>(data);
            await agent.UpdateState(updatedState);
        }

        Console.WriteLine($"Closing connection. '{webSocket.CloseStatus}' : '{webSocket.CloseStatusDescription}'");
        await webSocket.CloseAsync(
            receiveResult.CloseStatus.Value,
            receiveResult.CloseStatusDescription,
            CancellationToken.None);
    }
}
