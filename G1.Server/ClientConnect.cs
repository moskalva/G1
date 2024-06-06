using System.Collections.Concurrent;
using System.Net.WebSockets;
using G1.Model;
using G1.Model.Serializers;

namespace G1.Server;

public class ClientConnect
{
    public static async Task Connect(IClientAgent agent, WebSocket webSocket)
    {
        var cancellation = new CancellationTokenSource();

        var reader = ReadLoop(agent.UpdateState, webSocket, cancellation.Token);

        var writer = WriteLoop(agent.GetNotification, webSocket, cancellation.Token);

        await Task.WhenAny(reader, writer);
        cancellation.Cancel();
        await Task.WhenAll(reader, writer);

        if (webSocket.State == WebSocketState.Open)
        {
            Console.WriteLine("Unexpected connection close");
            await webSocket.CloseAsync(WebSocketCloseStatus.InternalServerError, "Unexpected failure", CancellationToken.None);
        }
        else if(webSocket.State == WebSocketState.Aborted){
            Console.WriteLine("Connection aborted");
        }
        else
        {
            Console.WriteLine($"Connection closed by remote peer. '{webSocket.CloseStatus}' : '{webSocket.CloseStatusDescription}'");
            await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closed by client", CancellationToken.None);
        }
    }

    private static async Task WriteLoop(Func<Task<WorldEntityState?>> source, WebSocket webSocket, CancellationToken cancellation)
    {
        var buffer = new Memory<byte>(new byte[Settings.OutgoingConnectionBufferSize]);
        while (!cancellation.IsCancellationRequested && webSocket.State == WebSocketState.Open)
        {
            var state = await source.Invoke();
            if (state.HasValue)
            {
                var data = SerializerHelpers.Serialize(state.Value, buffer);
                await webSocket.SendAsync(data, WebSocketMessageType.Binary, true, cancellation);
            }
            else
            {
                await Task.Delay(Settings.EmptyWriteQueueTimeout);
            }
        }
    }

    private static async Task ReadLoop(Func<WorldEntityState,Task> action, WebSocket webSocket, CancellationToken cancellation)
    {
        var buffer = new Memory<byte>(new byte[Settings.IncomingConnectionBufferSize]);
        int bytesAllocated = 0;
        while (!cancellation.IsCancellationRequested && webSocket.State == WebSocketState.Open)
        {
            var receiveTask = webSocket.ReceiveAsync(buffer.Slice(bytesAllocated), cancellation).AsTask();
            var finishedTask = await Task.WhenAny(receiveTask, Task.Delay(Settings.IdleClientTimeout));
            if (finishedTask != receiveTask)
            {
                Console.WriteLine("Client heartbeat was not received. Cancelling...");
                return;
            }

            var receiveResult = await receiveTask;
            bytesAllocated += receiveResult.Count;
            if (receiveResult.EndOfMessage)
            {
                var data = buffer.Slice(0, bytesAllocated);
                var updatedState = SerializerHelpers.Deserialize<WorldEntityState>(data.Span);
                await action(updatedState);
                bytesAllocated = 0;
            }
        }
    }
}
