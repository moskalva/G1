using System;
using System.IO;
using ProtoBuf;

namespace G1.Model
{
    [ProtoContract]
    public enum WorldEntityType
    {
        Ship
    }

    [ProtoContract]
    public struct WorldEntityId
    {
        [ProtoMember(1)]
        public int Id { get; set; }

        public static WorldEntityId Create() => new WorldEntityId { Id = new Random().Next() };

        public override string ToString() => Id.ToString();
    }

    [ProtoContract]
    public struct WorldEntityState
    {
        [ProtoMember(1)]
        public WorldEntityType Type { get; set; }

        [ProtoMember(2)]
        public WorldEntityId Id { get; set; }

        public override string ToString() => $"WorldEntityState '{Id}' type '{Type}'";
    }

    public static class SerializerHelpers
    {
        public static byte[] Serialize<T>(T obj)
        {
            using (var stream = new MemoryStream())
            {
                Serializer.Serialize<T>(stream, obj);
                return stream.ToArray();
            }
        }

        public static T Deserialize<T>(byte[] data)
        {
            using (var stream = new MemoryStream())
            {
                stream.Write(data, 0, data.Length);
                return Serializer.Deserialize<T>(stream);
            }
        }
    }
}
