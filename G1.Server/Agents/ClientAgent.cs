using G1.Model;
using Orleans.Runtime;
using Orleans.Utilities;

namespace G1.Server.Agents;

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
        this.myState = storage.RecordExists
                     ? storage.State
                     : new ClientAgentState
                     {
                         Id = this.GetPrimaryKey(),
                         Position = ClientAgentState.Zero, // TODO: figure out initial position
                         Velocity = AgentVelocity.Zero,
                     }; 

        this.storage = storage;
        this.worldEvents = new ObserverManager<IWorldEventsReceiver>(TimeSpan.MaxValue, logger);
    }

    public Task<ClientAgentState> GetState() => Task.FromResult(this.myState);

    public Task Subscribe(IWorldEventsReceiver worldEvents)
    {
        // notify subscriber with initial state
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

    public Task Leave(Guid clientId)=>
        this.worldEvents.Notify(r => r.Leave(clientId));


    public async Task UpdateState(ClientAgentState newState)
    {
        Console.WriteLine("New message:\n========================");
        Console.WriteLine(newState);
        Console.WriteLine("========================");

        if (newState.Id != myState.Id)
            throw new InvalidOperationException($"State providded for wrong entity. This: '{myState.Id}' other: '{newState.Id}'");
        
        var oldState = this.myState;
        this.myState = newState;
        if (WorldPositionTools.TryNormalizePosition(myState.Position, out var normalizedPosition))
        {
            this.myState.Position = normalizedPosition;
            // TODO: notify base sector change
        }

        await SaveState(newState);

        await NotifyNearSectors(newState, oldState);
    }

    private async Task NotifyNearSectors(ClientAgentState newState, ClientAgentState oldState)
    {
        var nearSectors = WorldPositionTools.GetNearSectors(newState.Position);
        foreach (var nearSector in nearSectors)
        {
            var sectorAgent = this.GrainFactory.GetGrain<ISectorAgent>(nearSector.Raw);
            await sectorAgent.Notify(newState);
        }

        var oldSectors = WorldPositionTools.GetNearSectors(oldState.Position).Where(s => !nearSectors.Contains(s));
        foreach (var oldSector in oldSectors)
        {
            var sectorAgent = this.GrainFactory.GetGrain<ISectorAgent>(oldSector.Raw);
            await sectorAgent.Leave(this.myState.Id);
        }
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