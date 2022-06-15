using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Unify
{
    public abstract class Packet
    {
        internal abstract byte[] GetBytes();
    }
}
