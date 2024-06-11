
namespace G1.Server;

public interface IWorldEventsReceiver : IGrainObserver
{
    Task Notify(ClientAgentState state);
}