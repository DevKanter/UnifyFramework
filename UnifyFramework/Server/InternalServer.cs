using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unify.Common;
using Unify.Server;

namespace UnifyFramework.Server
{
    public abstract class InternalServer<T> where T : new()
    {
        private ClientListener<T> _listener;

        public InternalServer(int port)
        {
            _listener = new ClientListener<T>(System.Net.IPAddress.Any, port, OnConnect, OnReceive);
        }

        protected abstract void OnReceive(ByteBuffer buffer, Connection<T> connection);
        protected abstract void OnConnect(Connection<T> connection);
    }
}
