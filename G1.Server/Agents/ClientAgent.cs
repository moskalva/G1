using G1.Model;
using Orleans.Runtime;

namespace G1.Server;

[GenerateSerializer, Alias(nameof(ClientAgentState))]
public class ClientAgentState
{
    [Id(0)]
    public Guid Id { get; set; }

    public World3dVector Position { get; set; }
    public World3dVector Velocity { get; set; }
}
public class ClientAgent : Grain, IClientAgent
{
    private readonly WorldEntityId id;

    private readonly IWorldEventSource eventSource = new WorldEventSource();
    private readonly IPersistentState<ClientAgentState> storage;
    private WorldEntityState myState;

    public ClientAgent(
        [PersistentState( stateName: "url", storageName: "urls")]
        IPersistentState<ClientAgentState> storage)
    {
        this.id = new WorldEntityId { Id = this.GetPrimaryKey() };
        this.myState = new WorldEntityState
        {
            Id = id,
            Type = WorldEntityType.Ship,
            Position = World3dVector.Zero,
            Velocity = World3dVector.Zero,
        };

        this.eventSource.Push(myState);
        this.storage = storage;
    }

    public Task<WorldEntityState> GetState() => Task.FromResult(this.myState);

    public async Task UpdateState(WorldEntityState newState)
    {
        Console.WriteLine("New message:\n========================");
        Console.WriteLine(newState);
        Console.WriteLine("========================");

        if (newState.Id != myState.Id)
            throw new InvalidOperationException($"State providded for wrong entity. This: '{myState.Id}' other: '{newState.Id}'");

        this.myState = newState;

        await SaveState(newState);
    }

    public Task Notify(WorldEntityState entityState)
    {
        eventSource.Push(entityState);
        return Task.CompletedTask;
    }

    public Task<WorldEntityState?> GetNotification()
    {
        return Task.FromResult(eventSource.Get());
    }

    private async Task SaveState(WorldEntityState newState)
    {
        if (newState.Position.HasValue)
        {
            this.storage.State.Position = newState.Position.Value;
        }

        if (newState.Velocity.HasValue)
        {
            this.storage.State.Velocity = newState.Velocity.Value;
        }

        await storage.WriteStateAsync();
    }

    public void Dispose() { }
}