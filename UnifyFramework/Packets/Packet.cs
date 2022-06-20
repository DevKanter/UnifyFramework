using Unify.Common;

namespace Unify
{
    public abstract class Packet : ISizable
    {
        public byte[] GetBytes()
        {
            var buffer = new ByteBuffer(GetSize());
            GetBytes(buffer);
            return buffer.GetData();
        }
        public abstract void GetBytes(ByteBuffer buffer);
        public abstract int GetSize();
    }

    public interface ISizable
    {
        int GetSize();
    }
    
}
