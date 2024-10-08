
using System.Buffers.Binary;
using System.Diagnostics.CodeAnalysis;

namespace G1.Server.Agents;

[GenerateSerializer, Alias(nameof(ClientAgentState))]
public class ClientAgentState : IEquatable<ClientAgentState>
{
    public static AgentPosition Zero => new AgentPosition();
    [Id(0)]
    public Guid Id { get; set; }

    [Id(1)]
    public AgentPosition Position { get; set; }

    [Id(2)]
    public AgentVelocity Velocity { get; set; }

    public bool Equals(ClientAgentState? other)
    {
        return other != null
            && this.Position.Equals(other.Position)
            && this.Velocity.Equals(other.Velocity);
    }

    public override string ToString() => $"WorldEntityState '{Id}', position: {Position}', velocity: '{Velocity}'";
}

[GenerateSerializer]
public struct WorldSectorId : IEquatable<WorldSectorId>
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

    public bool Equals(WorldSectorId other)
        => this.SystemId == other.SystemId
        && this.X == other.X
        && this.Y == other.Y
        && this.Z == other.Z;

    public override bool Equals([NotNullWhen(true)] object? obj) => obj is WorldSectorId other && Equals(other);
    public override int GetHashCode() => HashCode.Combine(SystemId, X, Y, Z);

    public static bool operator ==(WorldSectorId left, WorldSectorId right) => left.Equals(right);
    public static bool operator !=(WorldSectorId left, WorldSectorId right) => !left.Equals(right);

    private static Guid CreateRaw(uint systemId, int x, int y, int z)
    {
        Span<byte> bytes = stackalloc byte[16];
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
}

[GenerateSerializer]
public struct AgentPosition : IEquatable<AgentPosition>
{
    public AgentPosition(WorldSectorId sectorId, float x, float y, float z)
    {
        SectorId = sectorId;
        X = x;
        Y = y;
        Z = z;
    }

    [Id(0)]
    public WorldSectorId SectorId { get; set; }
    [Id(1)]
    public float X { get; set; }
    [Id(2)]
    public float Y { get; set; }
    [Id(3)]
    public float Z { get; set; }

    public bool Equals(AgentPosition other)
        => this.SectorId == other.SectorId
        && this.X == other.X
        && this.Y == other.Y
        && this.Z == other.Z;

    public override int GetHashCode() => HashCode.Combine(
        this.SectorId.GetHashCode(),
        this.X.GetHashCode(),
        this.Y.GetHashCode(),
        this.Z.GetHashCode());

    public override bool Equals([NotNullWhen(true)] object? obj) => obj is AgentPosition pos && Equals(pos);

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