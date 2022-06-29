using Unify.Common;

namespace Unify
{
    public abstract class Packet : ISizeable
    {
        protected Packet() { }
        protected Packet(ByteBuffer buffer) { }
        public byte[] GetBytes()
        {
            var buffer = new ByteBuffer(GetSize());
            GetBytes(buffer);
            return buffer.GetData();
        }
        public abstract void GetBytes(ByteBuffer buffer);
        public abstract int GetSize();
    }
    
}
