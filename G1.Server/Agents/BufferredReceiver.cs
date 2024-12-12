

using G1.Model;

namespace G1.Server.Agents;

public class BufferredReceiver : IWorldEventsReceiver
{
    private readonly Queue<RemoteCommand> queue = new Queue<RemoteCommand>();
    public Task Notify(ClientAgentState state)
    {
        var stateChanged = new StateChange(Helpers.ToWorldState(state));
        queue.Enqueue(stateChanged);
        return Task.CompletedTask;
    }

    public Task<RemoteCommand?> GetNotification()
    {
        return Task.FromResult(
            queue.TryDequeue(out var state) ? state : null);
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