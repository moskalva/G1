using System.Net.WebSockets;

namespace G1.Server;

public class ClientConnect
{
    public static async Task Echo(WebSocket webSocket)
    {
        var buffer = new byte[1024 * 4];
        var receiveResult = await webSocket.ReceiveAsync(
            new ArraySegment<byte>(buffer), CancellationToken.None);

        while (!receiveResult.CloseStatus.HasValue)
        {
            var data = new ArraySegment<byte>(buffer, 0, receiveResult.Count);
            Log(data);
            Console.WriteLine("Waiting data sent");
            await webSocket.SendAsync(
                data,
                receiveResult.MessageType,
                receiveResult.EndOfMessage,
                CancellationToken.None);
            Console.WriteLine("Waiting for response");
            receiveResult = await webSocket.ReceiveAsync(
                new ArraySegment<byte>(buffer), CancellationToken.None);
        }

        await webSocket.CloseAsync(
            receiveResult.CloseStatus.Value,
            receiveResult.CloseStatusDescription,
            CancellationToken.None);
    }

    private static void Log(ArraySegment<byte> data)
    {
        var s = System.Text.Encoding.UTF8.GetString( data.ToArray());
        Console.WriteLine("New message:\n========================");
        Console.WriteLine(s);
        Console.WriteLine("========================");
    }
}
