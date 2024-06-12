
namespace G1.Server.Agents;

[GenerateSerializer, Alias(nameof(ClientAgentState))]
public class ClientAgentState
{
    public static AgentPosition Zero => new AgentPosition();
    [Id(0)]
    public Guid Id { get; set; }

    [Id(1)]
    public AgentPosition Position { get; set; }

    [Id(2)]
    public AgentVelocity Velocity { get; set; }

    public override string ToString() => $"WorldEntityState '{Id}', position: {Position}', velocity: '{Velocity}'";
}

[GenerateSerializer]
public struct WorldSectorId
{
    [Id(0)]
    public Guid Raw { get; set; }

    public uint SystemId { get; }
    public int X { get; }
    public int Y { get; }
    public int Z { get; }
    public override string ToString() => $"{X},{Y},{Z}";

    public static AgentPosition GetSectorPosition(WorldSectorId baseSector, WorldSectorId sector)
    {
        if (baseSector.SystemId != sector.SystemId)
            throw new InvalidOperationException($"Cannot calculate relative position of sectors in different systems");

        throw new NotImplementedException();
    }
}

[GenerateSerializer]
public struct AgentPosition
{
    [Id(0)]
    public WorldSectorId SectorId { get; set; }
    [Id(1)]
    public float X { get; set; }
    [Id(2)]
    public float Y { get; set; }
    [Id(3)]
    public float Z { get; set; }

    public override string ToString() => $"Sector: '{SectorId}'| {X},{Y},{Z}";

    public static bool TryGetUpdatedSector(AgentPosition currentPosition,out WorldSectorId updatedSector)
    {
        throw new NotImplementedException();
    }

    public static AgentPosition RelativePosition(WorldSectorId baseSector, AgentPosition currentPosition)
    {
        throw new NotImplementedException();
    }

    public static bool TryNormalizePosition(AgentPosition currentPosition,out AgentPosition newPosition)
    {
        throw new NotImplementedException();
    }

    public static WorldSectorId[] GetNearSectors(AgentPosition currentPosition)
    {
        throw new NotImplementedException();
    }
}

[GenerateSerializer]
public struct AgentVelocity
{
    public static AgentVelocity Zero => new AgentVelocity();

    [Id(0)]
    public float X { get; set; }
    [Id(1)]
    public float Y { get; set; }
    [Id(2)]
    public float Z { get; set; }
    public override string ToString() => $"{X},{Y},{Z}";
}