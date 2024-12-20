using System.Collections.Concurrent;
using System.Diagnostics;
using System.Net.WebSockets;
using G1.Model;
using G1.Model.Serializers;
using G1.Server.Agents;

namespace G1.Server;

public class ClientConnect
{
    public static async Task Connect(WorldEntityId clientId, IGrainFactory grains, WebSocket webSocket)
    {
        using var agent = grains.GetGrain<IClientAgent>(clientId.Id);
        await agent.CreateDefaultShip();
        var cancellation = new CancellationTokenSource();
        var bufferredReceiver = new BufferredReceiver();
        var observer = grains.CreateObjectReference<IWorldEventsReceiver>(bufferredReceiver);
        await agent.Subscribe(observer);

        var reader = ReadLoop(async command =>
        {
            try
            {
                switch (command)
                {
                    case ClientStateChange stateChange:
                        Console.WriteLine($"Client state received '{stateChange}'");
                        var currentState = await agent.GetState();
                        var agentState = Helpers.FromWorldState(currentState, stateChange);
                        await agent.UpdateState(agentState);
                        break;
                    case ClientHeartBeat:
                        Console.WriteLine($"HeartBeat received");
                        var previous = await agent.GetState();
                        // we assume nothing has changed so just notify same state
                        await agent.UpdateState(previous);
                        break;
                    default:
                        Console.WriteLine($"Unknown command received '{command}'");
                        break;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception \n{ex}");
            }

        }, webSocket, cancellation.Token);
        var writer = WriteLoop(async () =>
        {
            try
            {
                var worldEvent = await bufferredReceiver.GetNotification();
                return worldEvent;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception \n{ex}");
                return null;
            }
        }, clientId, webSocket, cancellation.Token);

        await Task.WhenAny(reader, writer);

        cancellation.Cancel();
        await agent.Unsubscribe(observer);
        await agent.Disconnect();

        await Task.WhenAll(reader, writer);
        if (webSocket.State == WebSocketState.Open)
        {
            Console.WriteLine("Unexpected connection close");
            await webSocket.CloseAsync(WebSocketCloseStatus.InternalServerError, "Unexpected failure", CancellationToken.None);
        }
        else if (webSocket.State == WebSocketState.Aborted)
        {
            Console.WriteLine("Connection aborted");
        }
        else
        {
            Console.WriteLine($"Connection closed by remote peer. '{webSocket.CloseStatus}' : '{webSocket.CloseStatusDescription}'");
            await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closed by client", CancellationToken.None);
        }
    }

    private static async Task WriteLoop(Func<Task<ServerCommand?>> source, WorldEntityId clientId, WebSocket webSocket, CancellationToken cancellation)
    {
        var buffer = new Memory<byte>(new byte[Settings.OutgoingConnectionBufferSize]);
        var heartBeatWatch = Stopwatch.StartNew();
        var heartBeat = SerializerHelpers.Serialize(new ServerHeartBeat() { Id = clientId });
        while (!cancellation.IsCancellationRequested && webSocket.State == WebSocketState.Open)
        {
            var command = await source.Invoke();
            if (command != null)
            {
                Console.WriteLine($"Sending notification to client '{command}'");
                var data = SerializerHelpers.Serialize(command, buffer);
                Console.WriteLine($"Sending some data '{command.GetType().Name}' '{data.Length}' bytes");
                await webSocket.SendAsync(data, WebSocketMessageType.Binary, true, cancellation);
                heartBeatWatch.Restart();
            }
            else if (heartBeatWatch.Elapsed >= Settings.HeartBeatInterval)
            {
                await webSocket.SendAsync(heartBeat, WebSocketMessageType.Binary, true, cancellation);
                heartBeatWatch.Restart();
            }
            else
            {
                await Task.Delay(Settings.EmptyWriteQueueTimeout);
            }
        }
    }

    private static async Task ReadLoop(Func<ClientCommand, Task> action, WebSocket webSocket, CancellationToken cancellation)
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
            Console.WriteLine("Received some data");
            if (receiveResult.EndOfMessage)
            {
                Console.WriteLine($"Received '{bytesAllocated}' bytes message");
                var data = buffer.Slice(0, bytesAllocated);
                var updatedState = SerializerHelpers.Deserialize<ClientCommand>(data.Span);
                await action(updatedState);
                bytesAllocated = 0;
            }
        }
    }
}
