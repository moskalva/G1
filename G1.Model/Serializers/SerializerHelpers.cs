using System;
using System.Buffers;
using System.IO;
using ProtoBuf;

namespace G1.Model.Serializers
{
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

        public static Memory<byte> Serialize<T>(T obj, Memory<byte> buffer)
        {
            var writer = new MemoryBufferWriter<byte>(buffer);
            Serializer.Serialize<T>(writer, obj);
            return buffer.Slice(0, writer.SizeWritten);
        }

        public static T Deserialize<T>(Span<byte> buffer)
        {
            return Serializer.Deserialize<T>(buffer);
        }

        public static T Deserialize<T>(byte[] data)
        {
            return Deserialize<T>(data.AsSpan());
        }
    }
}