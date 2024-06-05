using G1.Model;

namespace G1.Server;


public interface IClientAgent : IDisposable
{
    public Task<WorldEntityState> GetState();

    public Task UpdateState(WorldEntityState newState);

    public IWorldEventSource EventSource { get; }
}