
namespace G1.Server;

public interface IClientAgent : IGrainWithGuidKey, IWorldEventsReceiver, IDisposable
{
    public Task<ClientAgentState> GetState();

    public Task UpdateState(ClientAgentState newState);

    Task Subscribe(IWorldEventsReceiver worldEvents);
    Task Unsubscribe(IWorldEventsReceiver worldEvents);
}


