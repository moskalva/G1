

using G1.Model;

namespace G1.Server.Agents;

public class BufferredReceiver : IWorldEventsReceiver
{
    private readonly HashSet<ServerCommand> unique = new();
    private readonly Queue<ServerCommand> queue = new();

    public Task Notify(ClientAgentState state)
    {
        var stateChanged = Helpers.ToWorldState(state);
        if (unique.Add(stateChanged))
        {
            queue.Enqueue(stateChanged);
        }
        return Task.CompletedTask;
    }

    public Task<ServerCommand?> GetNotification()
    {
        ServerCommand? result = null;
        if (queue.TryDequeue(out var next))
        {
            unique.Remove(next);
            result = next;
        }

        return Task.FromResult(result);
    }

    public Task Leave(Guid clientId)
    {
        var state = new NeighborLeft
        {
            Id = new WorldEntityId { Id = clientId }
        };
        queue.Enqueue(state);
        return Task.CompletedTask;
    }
}