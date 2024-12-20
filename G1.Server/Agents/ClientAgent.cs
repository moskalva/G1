using G1.Model;
using G1.Server.Models;
using Orleans.Concurrency;
using Orleans.Runtime;
using Orleans.Utilities;

namespace G1.Server.Agents;

public class ClientAgent : Grain, IClientAgent
{
    private ObserverManager<IWorldEventsReceiver> worldEvents;

    private readonly IPersistentState<ClientAgentState> storage;
    private readonly IPersistentState<PalyerShipType> shipInventory;

    private HashSet<Guid> visibleNeighbors = new HashSet<Guid>();

    public ClientAgent(
        ILogger<ClientAgent> logger,
        [PersistentState(stateName: "ship", storageName: "inventory")]
        IPersistentState<PalyerShipType> shipInventory,
        [PersistentState(stateName: "worldState", storageName: "agents")]
        IPersistentState<ClientAgentState> storage)
    {
        this.storage = storage;
        this.shipInventory = shipInventory;
        this.worldEvents = new ObserverManager<IWorldEventsReceiver>(TimeSpan.FromDays(1), logger);
    }

    public Task<ClientAgentState> GetState()
    {
        if (this.storage.RecordExists)
        {
            return Task.FromResult(this.storage.State);
        }

        throw new InvalidOperationException($"Ship was not found for '{this.GetPrimaryKey()}'");
    }

    public async Task CreateDefaultShip()
    {
        if (this.storage.RecordExists)
            return;

        var id = this.GetPrimaryKey();
        var newState = new ClientAgentState
        {
            Id = id,
            Position = ClientAgentState.Zero, // TODO: figure out initial position
            Velocity = Vector3D.Zero,
            Rotation = Vector3D.Zero,
            AngularVelocity = Vector3D.Zero,
        };

        await SaveState(newState);

        var shipType = new PalyerShipType()
        {
            Id = id,
            ShipType = ShipType.Mark1,
        };
        this.shipInventory.State = shipType;
        await this.shipInventory.WriteStateAsync();
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

    public async Task NeighborStateChanged(ClientAgentState neighborState)
    {
        Console.WriteLine($"NeighborStateChanged '{neighborState}'");

        var myState = await GetState();

        if (!this.shipInventory.RecordExists)
            throw new InvalidOperationException($"Ship was not found for '{this.GetPrimaryKey()}'");
        var shipModel = ModelFactory.GetShipModel(myState, this.shipInventory.State);

        neighborState = neighborState.Clone();
        neighborState.Position = WorldPositionTools.RelativePosition(myState.Position.SectorId, neighborState.Position);

        if (shipModel.CanSee(neighborState))
        {
            Console.WriteLine($"Can see neighbour '{neighborState.Id}'");
            visibleNeighbors.Add(neighborState.Id);
            await this.worldEvents.Notify(r => r.Notify(neighborState));
        }
        else
        {
            Console.WriteLine($"Can not see neighbour '{neighborState.Id}'");
            var isVisible = visibleNeighbors.Contains(neighborState.Id);
            if (isVisible)
            {
                await NeighborLeft(neighborState.Id);
                visibleNeighbors.Remove(neighborState.Id);
            }
        }
    }

    public async Task NeighborLeft(Guid clientId)
    {
        Console.WriteLine($"Neighbour left '{clientId}'");
        if (visibleNeighbors.Contains(clientId))
            await this.worldEvents.Notify(r => r.Leave(clientId));
    }

    public async Task UpdateState(ClientAgentState newState)
    {
        Console.WriteLine($"Update state called '{newState}'");

        var oldState = await GetState();
        if (newState.Id != oldState.Id)
            throw new InvalidOperationException($"State providded for wrong entity. This: '{oldState.Id}' other: '{newState.Id}'");

        if (WorldPositionTools.TryNormalizePosition(newState.Position, out var normalizedPosition))
        {
            newState.Position = normalizedPosition;
            await this.worldEvents.Notify(r => r.Notify(newState));
        }

        await NotifyNearSectors(newState, oldState);

        if (newState.Equals(oldState))
            return;

        Console.WriteLine($"State has changed '{newState}'");

        await SaveState(newState);
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
        Console.WriteLine($"NotifyNearSectors '{newState}'");
        var currentSectors = WorldPositionTools.GetNearSectors(newState.Position);
        var previousSectors = WorldPositionTools.GetNearSectors(oldState.Position);
        foreach (var nearSector in currentSectors)
        {
            var sectorAgent = this.GrainFactory.GetGrain<ISectorAgent>(nearSector.Raw);
            await sectorAgent.EntityUpdated(newState);
        }

        var oldSectors = previousSectors.Where(s => !currentSectors.Contains(s));
        foreach (var oldSector in oldSectors)
        {
            var sectorAgent = this.GrainFactory.GetGrain<ISectorAgent>(oldSector.Raw);
            await sectorAgent.EntityLeft(oldState.Id);
        }

        var newSectors = currentSectors.Where(s => !previousSectors.Contains(s));
        foreach (var newSector in newSectors)
        {
            var sectorAgent = this.GrainFactory.GetGrain<ISectorAgent>(newSector.Raw);
            var newNeighborIds = await sectorAgent.GetClients();
            foreach (var newNeighborId in newNeighborIds)
            {
                var neighbor = this.GrainFactory.GetGrain<IClientAgent>(newNeighborId);
                var state = await neighbor.GetState();
                await NeighborStateChanged(state);
            }
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