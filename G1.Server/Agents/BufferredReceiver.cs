

using G1.Model;

namespace G1.Server.Agents;

public class BufferredReceiver : IWorldEventsReceiver
{
    private readonly HashSet<RemoteCommand> unique = new HashSet<RemoteCommand>();
    private readonly Queue<RemoteCommand> queue = new Queue<RemoteCommand>();
    
    public Task Notify(ClientAgentState state)
    {
        var stateChanged = new StateChange(Helpers.ToWorldState(state));
        if (unique.Add(stateChanged))
        {
            queue.Enqueue(stateChanged);
        }
        return Task.CompletedTask;
    }

    public Task<RemoteCommand?> GetNotification()
    {
        RemoteCommand? result = null;
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