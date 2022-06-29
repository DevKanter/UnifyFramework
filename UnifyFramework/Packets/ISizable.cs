using Unify.Common;

namespace Unify
{
    public interface ISizeable
    {
        int GetSize();
        void GetBytes(ByteBuffer buffer);
       
    }
    
}
