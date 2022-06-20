using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Unify.Common;
using UnifyFramework.ClientSide;

namespace Unify.ClientSide
{
    public abstract class InternalClient
    {
        private TcpClient _tcpClient;

        protected void ConnectToServer(string ipAddress,int port)
        {
            IPAddress ip = IPAddress.Parse(ipAddress);
            IPEndPoint remoteEndPoint = new IPEndPoint(ip, port);


            _tcpClient = new TcpClient();
            _tcpClient.Connect(remoteEndPoint);

            var t = new Thread(BeginAccept);
            t.Start();


        }
        private void BeginAccept()
        {
            var stream = _tcpClient.GetStream();
            var buffer = new byte[1024];
            while (true)
            {
                if (stream.DataAvailable)
                {
                    var count = stream.Read(buffer, 0, 1024);
                    var data = new byte[count];
                    Buffer.BlockCopy(buffer, 0, data, 0, count);

                    var byteBuffer = new ByteBuffer(data);
                    Console.WriteLine(byteBuffer.ToString());
                }
            }
        }

        public void Send(Packet packet)
        {
            _tcpClient.Client.Send(packet.GetBytes());
        }
    }
}
