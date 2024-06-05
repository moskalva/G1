using G1.Model;

namespace G1.Server;

public class ClientAgent : IClientAgent
{
    private readonly WorldEntityId id;

    public ClientAgent(WorldEntityId id)
    {
        this.id = id;
    }

    public async Task<WorldEntityState> GetState()
    {
        var dummyState = new WorldEntityState
        {
            Id = id,
            Type = WorldEntityType.Ship,
            Position = World3dVector.Zero,
            Velocity = World3dVector.Zero,
        };
        return dummyState;
    }

    public async Task UpdateState(WorldEntityState newState)
    {
        Console.WriteLine("New message:\n========================");
        Console.WriteLine(newState);
        Console.WriteLine("========================");
    }

    public void Dispose() { }
}