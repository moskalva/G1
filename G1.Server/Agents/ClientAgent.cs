using G1.Model;
using Orleans.Runtime;
using Orleans.Utilities;

namespace G1.Server;

public class ClientAgent : Grain, IClientAgent
{
    private ObserverManager<IWorldEventsReceiver> worldEvents;

    private readonly IPersistentState<ClientAgentState> storage;
    private ClientAgentState myState;

    public ClientAgent(
        ILogger<ClientAgent> logger,
        [PersistentState( stateName: "worldState", storageName: "agents")]
        IPersistentState<ClientAgentState> storage)
    {
        this.myState = new ClientAgentState
        {
            Id = this.GetPrimaryKey(),
            Position = ClientAgentState.Zero,
            Velocity = AgentVelocity.Zero,
        };

        this.storage = storage;
        this.worldEvents = new ObserverManager<IWorldEventsReceiver>(TimeSpan.MaxValue, logger);
    }

    public Task<ClientAgentState> GetState() => Task.FromResult(this.myState);


    public Task Subscribe(IWorldEventsReceiver worldEvents)
    {
        // notify subscriber about initial state
        worldEvents.Notify(myState);

        this.worldEvents.Subscribe(worldEvents, worldEvents);
        return Task.CompletedTask;
    }

    public Task Unsubscribe(IWorldEventsReceiver worldEvents)
    {
        this.worldEvents.Unsubscribe(worldEvents);
        return Task.CompletedTask;
    }

    public Task Notify(ClientAgentState entityState) =>
        this.worldEvents.Notify(r => r.Notify(entityState));


    public async Task UpdateState(ClientAgentState newState)
    {
        Console.WriteLine("New message:\n========================");
        Console.WriteLine(newState);
        Console.WriteLine("========================");

        if (newState.Id != myState.Id)
            throw new InvalidOperationException($"State providded for wrong entity. This: '{myState.Id}' other: '{newState.Id}'");

        this.myState = newState;

        await SaveState(newState);
    }

    private async Task SaveState(ClientAgentState newState)
    {
        this.storage.State.Position = newState.Position;
        this.storage.State.Velocity = newState.Velocity;

        await storage.WriteStateAsync();
    }

    public void Dispose()
    {
        // notify about deletion
    }
}