using System;
using ProtoBuf;

namespace G1.Model
{
    [ProtoContract]
    [ProtoInclude(1, typeof(ServerHeartBeat))]
    [ProtoInclude(2, typeof(ServerStateChange))]
    [ProtoInclude(3, typeof(NeighborLeft))]
    public class ServerCommand
    {
    }

    [ProtoContract]
    public class ServerHeartBeat : ServerCommand, IEquatable<ServerHeartBeat>
    {
        [ProtoMember(1)]
        public WorldEntityId Id { get; set; }

        public bool Equals(ServerHeartBeat other) => this.Id.Equals(other.Id);
        public override bool Equals(object obj) => obj is ServerHeartBeat x ? Equals(x) : false;
        public override int GetHashCode() => this.Id.Id.GetHashCode();
    }

    [ProtoContract]
    public class ServerStateChange : ServerCommand, IEquatable<ServerStateChange>
    {
        [ProtoMember(1)]
        public WorldEntityId Id { get; set; }

        [ProtoMember(2)]
        public WorldEntityType Type { get; set; }

        [ProtoMember(3)]
        public uint SystemId { get; set; }

        [ProtoMember(4)]
        public WorldReferencePoint ReferencePoint { get; set; }

        [ProtoMember(5)]
        public WorldEntityLocationAndSpeed PositionAndSpeed { get; set; }

        [ProtoMember(6)]
        public Emissions Emissions { get; set; }

        public bool Equals(ServerStateChange other)
            => this.Id.Equals(other.Id)
            && this.Type.Equals(other.Type)
            && this.SystemId.Equals(other.SystemId)
            && this.ReferencePoint.Equals(other.ReferencePoint)
            && this.PositionAndSpeed.Equals(other.PositionAndSpeed)
            && this.Emissions.Equals(other.Emissions);

        public override bool Equals(object? other) => other is ServerStateChange change && this.Equals(change);

        public override int GetHashCode()
            => HashCode.Combine(
            this.Id,
            this.Type,
            this.SystemId,
            this.ReferencePoint,
            this.PositionAndSpeed,
            this.Emissions);
        public override string ToString() => this.Stringify();
    }

    [ProtoContract]
    public class NeighborLeft : ServerCommand, IEquatable<NeighborLeft>
    {
        [ProtoMember(1)]
        public WorldEntityId Id { get; set; }

        public bool Equals(NeighborLeft other) => this.Id.Equals(other.Id);
        public override bool Equals(object obj) => obj is NeighborLeft x ? Equals(x) : false;
        public override int GetHashCode() => this.Id.Id.GetHashCode();
    }

    [ProtoContract]
    public struct WorldReferencePoint
    {
        public static WorldReferencePoint Zero => new WorldReferencePoint();

        [ProtoMember(1)]
        public int X { get; set; }

        [ProtoMember(2)]
        public int Y { get; set; }

        [ProtoMember(3)]
        public int Z { get; set; }

        public override string ToString() => $"{X},{Y},{Z}";
    }
}