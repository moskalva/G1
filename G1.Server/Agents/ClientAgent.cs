using G1.Model;
using Orleans.Concurrency;
using Orleans.Runtime;
using Orleans.Utilities;

namespace G1.Server.Agents;

public class ClientAgent : Grain, IClientAgent
{
    private ObserverManager<IWorldEventsReceiver> worldEvents;

    private readonly IPersistentState<ClientAgentState> storage;

    public ClientAgent(
        ILogger<ClientAgent> logger,
        [PersistentState(stateName: "worldState", storageName: "agents")]
        IPersistentState<ClientAgentState> storage)
    {
        this.storage = storage;
        this.worldEvents = new ObserverManager<IWorldEventsReceiver>(TimeSpan.FromDays(1), logger);
    }

    public async Task<ClientAgentState> GetState()
    {
        Console.WriteLine("GetState\n========================");
        if (this.storage.RecordExists)
        {
            var state = this.storage.State;

            Console.WriteLine($"State is '{state}'");
            Console.WriteLine("========================");
            return state;
        }

        var newState = new ClientAgentState
        {
            Id = this.GetPrimaryKey(),
            Position = ClientAgentState.Zero, // TODO: figure out initial position
            Velocity = Vector3D.Zero,
            Rotation = Vector3D.Zero,
            AngularVelocity = Vector3D.Zero,
        };

        await SaveState(newState);
        Console.WriteLine("========================");
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

    public Task NeighborStateChanged(ClientAgentState entityState) =>
        this.worldEvents.Notify(r => r.Notify(entityState));

    public Task NeighborLeft(Guid clientId) =>
        this.worldEvents.Notify(r => r.Leave(clientId));


    public async Task UpdateState(ClientAgentState newState)
    {
        Console.WriteLine(newState);

        var oldState = await GetState();
        if (newState.Id != oldState.Id)
            throw new InvalidOperationException($"State providded for wrong entity. This: '{oldState.Id}' other: '{newState.Id}'");

        if (WorldPositionTools.TryNormalizePosition(newState.Position, out var normalizedPosition))
        {
            newState.Position = normalizedPosition;
            // TODO: notify base sector change
        }
        if (newState != oldState)
        {
            Console.WriteLine($"State has changed '{newState}'");
        }

        await SaveState(newState);

        await NotifyNearSectors(newState, oldState);
    }

    public async Task Disconnect()
    {
        var state = await GetState();
        var sectors = WorldPositionTools.GetNearSectors(state.Position);
        foreach (var oldSector in sectors)
        {
            var sectorAgent = this.GrainFactory.GetGrain<ISectorAgent>(oldSector.Raw);
            await sectorAgent.EntityLeft(state.Id);
        }
    }

    private async Task NotifyNearSectors(ClientAgentState newState, ClientAgentState oldState)
    {
        var nearSectors = WorldPositionTools.GetNearSectors(newState.Position);
        foreach (var nearSector in nearSectors)
        {
            var sectorAgent = this.GrainFactory.GetGrain<ISectorAgent>(nearSector.Raw);
            await sectorAgent.EntityUpdated(newState);
        }

        var oldSectors = WorldPositionTools.GetNearSectors(oldState.Position).Where(s => !nearSectors.Contains(s));
        foreach (var oldSector in oldSectors)
        {
            var sectorAgent = this.GrainFactory.GetGrain<ISectorAgent>(oldSector.Raw);
            await sectorAgent.EntityLeft(oldState.Id);
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