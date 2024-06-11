
namespace G1.Server;

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

    public uint SystemId { get; set; }
    public int X { get; set; }
    public int Y { get; set; }
    public int Z { get; set; }
    public override string ToString() => $"{X},{Y},{Z}";
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