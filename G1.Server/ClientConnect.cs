using System.Collections.Concurrent;
using System.Net.WebSockets;
using G1.Model;
using G1.Model.Serializers;
using G1.Server.Agents;

namespace G1.Server;

public class ClientConnect
{
    public static async Task Connect(WorldEntityId cientId, IGrainFactory grains, WebSocket webSocket)
    {
        using var agent = grains.GetGrain<IClientAgent>(cientId.Id);
        var cancellation = new CancellationTokenSource();
        var bufferredReceiver = new BufferredReceiver();
        var observer = grains.CreateObjectReference<IWorldEventsReceiver>(bufferredReceiver);
        await agent.Subscribe(observer);

        var reader = ReadLoop(async newState =>
        {
            var currentState = await agent.GetState();
            var agentState = FromWorldState(currentState.Position.SectorId,newState);
            await agent.UpdateState(agentState);
        }, webSocket, cancellation.Token);
        var writer = WriteLoop(async () =>
        {
            var worldEvent = await bufferredReceiver.GetNotification();
            return worldEvent is null ? null : ToWorldState(worldEvent);
        }, webSocket, cancellation.Token);

        await Task.WhenAny(reader, writer);

        cancellation.Cancel();
        await agent.Unsubscribe(observer);

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

    private static async Task WriteLoop(Func<Task<WorldEntityState?>> source, WebSocket webSocket, CancellationToken cancellation)
    {
        var buffer = new Memory<byte>(new byte[Settings.OutgoingConnectionBufferSize]);
        while (!cancellation.IsCancellationRequested && webSocket.State == WebSocketState.Open)
        {
            var state = await source.Invoke();
            if (state != null)
            {
                var data = SerializerHelpers.Serialize(state, buffer);
                await webSocket.SendAsync(data, WebSocketMessageType.Binary, true, cancellation);
            }
            else
            {
                await Task.Delay(Settings.EmptyWriteQueueTimeout);
            }
        }
    }

    private static async Task ReadLoop(Func<WorldEntityState, Task> action, WebSocket webSocket, CancellationToken cancellation)
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

    public static ClientAgentState FromWorldState(WorldSectorId sectorId, WorldEntityState state)
    {
        return new ClientAgentState
        {
            Id = state.Id.Id,
            Position = new AgentPosition
            {
                SectorId = sectorId,
                X = state.Position?.X ?? 0,
                Y = state.Position?.Y ?? 0,
                Z = state.Position?.Z ?? 0,
            },
            Velocity = new AgentVelocity
            {
                X = state.Velocity?.X ?? 0,
                Y = state.Velocity?.Y ?? 0,
                Z = state.Velocity?.Z ?? 0,
            },
        };
    }

    public static WorldEntityState ToWorldState(ClientAgentState agentState)
    {
        return new WorldEntityState
        {
            Id = new WorldEntityId { Id = agentState.Id },
            Type = WorldEntityType.Ship,
            Position = new World3dVector
            {
                X = agentState.Position.X,
                Y = agentState.Position.Y,
                Z = agentState.Position.Z
            },
            Velocity = new World3dVector
            {
                X = agentState.Velocity.X,
                Y = agentState.Velocity.Y,
                Z = agentState.Velocity.Z,
            },
        };
    }
}
