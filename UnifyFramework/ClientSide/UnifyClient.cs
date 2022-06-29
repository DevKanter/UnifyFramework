using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using Unify.Common;
using PacketLib.Enums;

namespace Unify.ClientSide
{
    public abstract partial class UnifyClient
    {
        private TcpClient _tcpClient;
        private Action<ByteBuffer>[][] _actions = new Action<ByteBuffer>[byte.MaxValue][];
        protected UnifyClient(string ipAddress, int port)
        {
            RegisterActions();
            ConnectToServer(ipAddress, port);  
        }
        private void ConnectToServer(string ipAddress,int port)
        {
            IPAddress ip = IPAddress.Parse(ipAddress);
            IPEndPoint remoteEndPoint = new IPEndPoint(ip, port);


            _tcpClient = new TcpClient();
            _tcpClient.Connect(remoteEndPoint);

            var t = new Thread(BeginAccept);
            t.Start();


        }
        private void RegisterAction(PacketCategory category, byte id, Action<ByteBuffer> action)
        {
            if (_actions[(int)category] == null)
                _actions[(int)category] = new Action<ByteBuffer>[byte.MaxValue];
            _actions[(int)category][id] = action;
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

                    OnReceive(data);
                }
            }
        }
        private void OnReceive(byte[] data)
        {
            var action = _actions[data[0]][data[1]];
            var buffer = new ByteBuffer(data);
            action(buffer);
        }
        public void Send(Packet packet)
        {
            _tcpClient.Client.Send(packet.GetBytes());
        }

    }
}
