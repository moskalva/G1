using G1.Model;

namespace G1.Server;

public class ClientAgent : IClientAgent
{
    private readonly WorldEntityId id;
    private WorldEntityState myState;

    public ClientAgent(WorldEntityId id)
    {
        this.id = id;
        this.myState = new WorldEntityState
        {
            Id = id,
            Type = WorldEntityType.Ship,
            Position = World3dVector.Zero,
            Velocity = World3dVector.Zero,
        };

        this.EventSource.Push(myState);
    }

    public IWorldEventSource EventSource { get; } = new WorldEventSource();

    public async Task<WorldEntityState> GetState() => this.myState;

    public async Task UpdateState(WorldEntityState newState)
    {
        Console.WriteLine("New message:\n========================");
        Console.WriteLine(newState);
        Console.WriteLine("========================");

        if (newState.Id != myState.Id)
            throw new InvalidOperationException($"State providded for wrong entity. This: '{myState.Id}' other: '{newState.Id}'");

        this.myState = newState;
    }

    public void Dispose() { }
}