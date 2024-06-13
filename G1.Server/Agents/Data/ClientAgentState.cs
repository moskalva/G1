
using System.Buffers.Binary;
using System.Runtime.InteropServices;

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
    public WorldSectorId(Guid raw)
    {
        var (systemId, x, y, z) = ExtractValues(raw);
        SystemId = systemId;
        X = x;
        Y = y;
        Z = z;
    }

    public WorldSectorId(uint systemId, int x, int y, int z)
    {
        SystemId = systemId;
        X = x;
        Y = y;
        Z = z;
    }

    [Id(0)]
    public Guid Raw
    {
        get => CreateRaw(SystemId, X, Y, Z);
        set
        {
            var (systemId, x, y, z) = ExtractValues(value);
            this.SystemId = systemId;
            this.X = x;
            this.Y = y;
            this.Z = z;
        }
    }

    public uint SystemId { get; private set; }
    public int X { get; private set; }
    public int Y { get; private set; }
    public int Z { get; private set; }

    public override string ToString() => $"{SystemId},{X},{Y},{Z}";

    private static Guid CreateRaw(uint systemId, int x, int y, int z)
    {
        var bytes = new byte[16].AsSpan();
        BinaryPrimitives.WriteUInt32BigEndian(bytes.Slice(0, 4), systemId);
        BinaryPrimitives.WriteInt32BigEndian(bytes.Slice(4, 4), x);
        BinaryPrimitives.WriteInt32BigEndian(bytes.Slice(8, 4), y);
        BinaryPrimitives.WriteInt32BigEndian(bytes.Slice(12, 4), z);
        return new Guid(bytes, false);
    }

    private static (uint systemId, int x, int y, int z) ExtractValues(Guid raw)
    {
        Span<byte> result = stackalloc byte[16];
        raw.TryWriteBytes(result);
        return (
            BinaryPrimitives.ReadUInt32BigEndian(result.Slice(0, 4)),
            BinaryPrimitives.ReadInt32BigEndian(result.Slice(4, 4)),
            BinaryPrimitives.ReadInt32BigEndian(result.Slice(8, 4)),
            BinaryPrimitives.ReadInt32BigEndian(result.Slice(12, 4))
        );
    }

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

    public static bool TryGetUpdatedSector(AgentPosition currentPosition, out WorldSectorId updatedSector)
    {
        throw new NotImplementedException();
    }

    public static AgentPosition RelativePosition(WorldSectorId baseSector, AgentPosition currentPosition)
    {
        throw new NotImplementedException();
    }

    public static bool TryNormalizePosition(AgentPosition currentPosition, out AgentPosition newPosition)
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