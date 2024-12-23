
using Orleans.Concurrency;

namespace G1.Server.Agents;

public interface IClientAgent : IGrainWithGuidKey, IDisposable
{
    Task CreateDefaultShip();
    public Task<ClientAgentState> GetState();

    public Task UpdateState(ClientAgentState newState);

    Task Subscribe(IWorldEventsReceiver worldEvents);
    Task Unsubscribe(IWorldEventsReceiver worldEvents);
    Task Disconnect();

    Task NeighborStateChanged(ClientAgentState state);
    Task NeighborLeft(Guid clientId);
}


