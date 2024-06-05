using System.Collections.Concurrent;
using G1.Model;

namespace G1.Server;

public class WorldEventSource : IWorldEventSource
{
    private readonly ConcurrentQueue<WorldEntityState> queue = new ConcurrentQueue<WorldEntityState>();

    public void Push(WorldEntityState entityState) => queue.Enqueue(entityState);

    public WorldEntityState? Get()=> queue.TryDequeue(out var entityState)? entityState: null;
}