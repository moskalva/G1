using System;
using System.Runtime.CompilerServices;
using ProtoBuf;

namespace G1.Model
{
    [ProtoContract]
    public struct WorldEntityLocationAndSpeed
    {
        [ProtoMember(1)]
        public World3dVector Position { get; set; }

        [ProtoMember(2)]
        public World3dVector Velocity { get; set; }

        [ProtoMember(3)]
        public World3dVector Rotation { get; set; }

        [ProtoMember(4)]
        public World3dVector AngularVelocity { get; set; }

    }

    [ProtoContract]
    [ProtoInclude(1, typeof(ClientHeartBeat))]
    [ProtoInclude(2, typeof(ClientStateChange))]
    public class ClientCommand
    {
    }

    [ProtoContract]
    public class ClientHeartBeat : ClientCommand, IEquatable<ClientHeartBeat>
    {
        [ProtoMember(1)]
        public WorldEntityId Id { get; set; }

        public bool Equals(ClientHeartBeat other) => this.Id.Equals(other.Id);
        public override bool Equals(object obj) => obj is ClientHeartBeat x ? Equals(x) : false;
        public override int GetHashCode() => this.Id.Id.GetHashCode();
    }

    [ProtoContract]
    public class ClientStateChange : ClientCommand, IEquatable<ClientStateChange>
    {
        [ProtoMember(1)]
        public WorldEntityId Id { get; set; }

        [ProtoMember(2)]
        public WorldEntityLocationAndSpeed PositionAndSpeed { get; set; }

        [ProtoMember(3)]
        public Emissions Emissions { get; set; }

        public bool Equals(ClientStateChange other)
            => this.Id.Equals(other.Id)
            && this.PositionAndSpeed.Equals(other.PositionAndSpeed)
            && this.Emissions.Equals(other.Emissions);

        public override bool Equals(object? other) => other is ClientStateChange change && this.Equals(change);

        public override int GetHashCode()
            => HashCode.Combine(
            this.Id,
            this.PositionAndSpeed,
            this.Emissions);
        public override string ToString() => this.Stringify();
    }
}
