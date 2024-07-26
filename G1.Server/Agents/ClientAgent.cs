using G1.Model;
using Orleans.Runtime;
using Orleans.Utilities;

namespace G1.Server.Agents;

public class ClientAgent : Grain, IClientAgent
{
    private ObserverManager<IWorldEventsReceiver> worldEvents;

    private readonly IPersistentState<ClientAgentState> storage;

    public ClientAgent(
        ILogger<ClientAgent> logger,
        [PersistentState( stateName: "worldState", storageName: "agents")]
        IPersistentState<ClientAgentState> storage)
    {
        this.storage = storage;
        this.worldEvents = new ObserverManager<IWorldEventsReceiver>(TimeSpan.MaxValue, logger);
    }

    public async Task<ClientAgentState> GetState()
    {
        if(this.storage.RecordExists)
            return this.storage.State;

        var newState = new ClientAgentState
                     {
                         Id = this.GetPrimaryKey(),
                         Position = ClientAgentState.Zero, // TODO: figure out initial position
                         Velocity = AgentVelocity.Zero,
                     };
        
        await SaveState(newState);
        return newState;
    }

    public async Task Subscribe(IWorldEventsReceiver receiver)
    {
        Console.WriteLine("Subscribed to events");
        // notify subscriber with initial state
        var currentState = await GetState();
        await receiver.Notify(currentState);

        this.worldEvents.Subscribe(receiver, receiver);
    }

    public Task Unsubscribe(IWorldEventsReceiver receiver)
    {
        Console.WriteLine("Unsubscribed from events");
        this.worldEvents.Unsubscribe(receiver);
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

        var oldState = await GetState();
        if (newState.Id != oldState.Id)
            throw new InvalidOperationException($"State providded for wrong entity. This: '{oldState.Id}' other: '{newState.Id}'");
        
        if (WorldPositionTools.TryNormalizePosition(newState.Position, out var normalizedPosition))
        {
            newState.Position = normalizedPosition;
            // TODO: notify base sector change
        }

        await SaveState(newState);

        await NotifyNearSectors(newState, oldState);
        Console.WriteLine("========================");
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
            await sectorAgent.Leave(oldState.Id);
        }
    }

    private async Task SaveState(ClientAgentState newState)
    {
        this.storage.State = newState;

        await storage.WriteStateAsync();
    }

    public void Dispose()
    {
        // notify about deletion
    }
}