using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unify.Common;
using Unify.Server;

namespace UnifyFramework.Server
{
    public class UnifyServer<T> : InternalServer<T> where T : new()
    {
        public UnifyServer(int port) : base(port)
        {
        }

        protected override void OnConnect(Connection<T> connection)
        {
            Console.WriteLine($"New Connection[{connection.ID}]");
            var packet = new PacketLib.S2CHello();
            connection.Send(packet);
        }

        protected override void OnReceive(ByteBuffer buffer, Connection<T> connection)
        {
            Console.WriteLine($"New data received: {buffer}");
        }
    }
}
