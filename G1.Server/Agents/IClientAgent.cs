
namespace G1.Server.Agents;

public interface IClientAgent : IGrainWithGuidKey, IWorldEventsReceiver, IDisposable
{
    public Task<ClientAgentState> GetState();

    public Task UpdateState(ClientAgentState newState);

    Task Subscribe(IWorldEventsReceiver worldEvents);
    Task Unsubscribe(IWorldEventsReceiver worldEvents);
}


