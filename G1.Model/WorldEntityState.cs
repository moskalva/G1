using System;
using ProtoBuf;

namespace G1.Model
{
    [ProtoContract]
    public enum WorldEntityType
    {
        Unknown = 0,
        Ship = 1,
    }

    [ProtoContract]
    public struct WorldEntityId : IEquatable<WorldEntityId>
    {
        [ProtoMember(1)]
        public Guid Id { get; set; }

        public override string ToString() => Id.ToString();

        public bool Equals(WorldEntityId other) => this.Id == other.Id;

        public override bool Equals(object obj)
            => obj is WorldEntityId other && Equals(other);

        public override int GetHashCode() => Id.GetHashCode();

        public static bool operator ==(WorldEntityId obj1, WorldEntityId obj2) => obj1.Equals(obj2);

        public static bool operator !=(WorldEntityId obj1, WorldEntityId obj2) => !obj1.Equals(obj2);

        public static WorldEntityId Create() => new WorldEntityId { Id = Guid.NewGuid() };

        public static bool TryParse(string str, out WorldEntityId clientId)
        {
            if (Guid.TryParse(str, out var id))
            {
                clientId = new WorldEntityId { Id = id };
                return true;
            }

            clientId = default;
            return false;
        }
    }

    [ProtoContract]
    public struct World3dVector
    {
        public static World3dVector Zero => new World3dVector();

        [ProtoMember(1)]
        public float X { get; set; }
        [ProtoMember(2)]
        public float Y { get; set; }
        [ProtoMember(3)]
        public float Z { get; set; }

        public override string ToString() => $"{X},{Y},{Z}";
    }

    [ProtoContract]
    public struct WorldEntityState
    {
        [ProtoMember(1)]
        public WorldEntityId Id { get; set; }
        [ProtoMember(2)]
        public WorldEntityType Type { get; set; }

        [ProtoMember(3)]
        public World3dVector? Position { get; set; }

        [ProtoMember(4)]
        public World3dVector? Velocity { get; set; }

        public override string ToString() => $"WorldEntityState '{Id}' type '{Type}', position: {Position}', velocity: '{Velocity}'";
    }    
}
