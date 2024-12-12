
using Orleans.Concurrency;

namespace G1.Server.Agents;

public interface IClientAgent : IGrainWithGuidKey, IDisposable
{
    public Task<ClientAgentState> GetState();

    public Task UpdateState(ClientAgentState newState);

    Task Subscribe(IWorldEventsReceiver worldEvents);
    Task Unsubscribe(IWorldEventsReceiver worldEvents);
    Task Disconnect();


    [AlwaysInterleave]
    Task NeighbourStateChanged(ClientAgentState state);
    [AlwaysInterleave]
    Task NeighbourLeft(Guid clientId);
}


