
namespace G1.Server.Agents;

public interface IWorldEventsReceiver : IGrainObserver
{
    Task Notify(ClientAgentState state);
    Task Leave(Guid clientId);
}