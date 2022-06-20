using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unify.ClientSide;

namespace UnifyFramework.ClientSide
{
    public class UnifyClient : InternalClient
    {
        public UnifyClient(string ipAddress,int port)
        {
            ConnectToServer(ipAddress, port);
        }
    }
}
