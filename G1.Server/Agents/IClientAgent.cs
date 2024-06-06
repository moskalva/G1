using G1.Model;

namespace G1.Server;


public interface IClientAgent : IGrainWithGuidKey, IDisposable
{
    public Task<WorldEntityState> GetState();

    public Task UpdateState(WorldEntityState newState);

    
    Task Notify(WorldEntityState entityState);
    Task<WorldEntityState?> GetNotification();
}