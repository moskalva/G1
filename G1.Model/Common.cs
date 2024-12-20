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
    public struct WorldEntityId
    {
        [ProtoMember(1)]
        public Guid Id { get; set; }

        public override string ToString() => Id.ToString();

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
    public struct Emissions
    {
        [ProtoMember(1)]
        public float ThermalEmission { get; set; }
        [ProtoMember(2)]
        public float EmEmission { get; set; }
        [ProtoMember(3)]
        public float ParticleEmission { get; set; }
    }
}