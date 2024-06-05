using System;
using System.Buffers;

namespace G1.Model.Serializers
{
    public class MemoryBufferWriter<T> : IBufferWriter<T>
    {
        private readonly Memory<T> memory;
        private uint position;

        public MemoryBufferWriter(Memory<T> memory)
        {
            this.memory = memory;
        }

        public int SizeWritten => (int)this.position;

        public void Advance(int count)
        {
            if (count < 0)
                throw new ArgumentOutOfRangeException(nameof(count));

            position += (uint)count;
        }

        public Memory<T> GetMemory(int sizeHint = 0)
        {
            var memoryLeft = memory.Length - position;
            if (memoryLeft < sizeHint)
                throw new InvalidOperationException("Cannot provide enough memory");

            return this.memory.Slice((int)position);
        }

        public Span<T> GetSpan(int sizeHint = 0) => GetMemory(sizeHint).Span;
    }
}