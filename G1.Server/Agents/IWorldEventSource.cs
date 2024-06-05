using G1.Model;

namespace G1.Server;

public interface IWorldEventSource
{
    void Push(WorldEntityState entityState);
    WorldEntityState? Get();
}