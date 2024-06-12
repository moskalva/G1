

namespace G1.Server.Agents;

public class BufferredReceiver : IWorldEventsReceiver
{
    private readonly Queue<ClientAgentState> queue = new Queue<ClientAgentState>();
    public Task Notify(ClientAgentState state)
    {
        queue.Enqueue(state);
        return Task.CompletedTask;
    }

    public Task<ClientAgentState?> GetNotification()
    {
        return Task.FromResult(
            queue.TryDequeue(out var state) ? state : null);
    }

    public Task Leave(Guid clientId)
    {
        throw new NotImplementedException();
    }
}