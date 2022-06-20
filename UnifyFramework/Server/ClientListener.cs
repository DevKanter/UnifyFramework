using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using Unify.Common;

namespace Unify.Server
{
    internal class ClientListener<T> where T : new()

    {
        private readonly ManualResetEvent _allDone = new ManualResetEvent(false);
        private readonly Action<Connection<T>> _onConnect;
        private readonly Action<ByteBuffer, Connection<T>> _onReceive;

        public ClientListener(IPAddress address, int port, Action<Connection<T>> onConnect,
            Action<ByteBuffer, Connection<T>> onReceive)
        {
            _onConnect = onConnect;
            _onReceive = onReceive;
            Task.Factory.StartNew(() => StartListening(address, port));
        }


        private void StartListening(IPAddress ipAddress, int port)
        {
            // Bind the socket to the local endpoint and listen for incoming connections.  
            try
            {
                IPEndPoint localEndPoint = new IPEndPoint(IPAddress.Any, port);

                // Create a TCP/IP socket.  
                Socket listener = new Socket(ipAddress.AddressFamily,
                    SocketType.Stream, ProtocolType.Tcp);
                listener.Bind(localEndPoint);
                listener.Listen(int.MaxValue);

                while (true)
                {
                    _allDone.Reset();
                    listener.BeginAccept(AcceptCallback, listener);

                    _allDone.WaitOne();
                }
                // Start an asynchronous socket to listen for connections.  
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private void AcceptCallback(IAsyncResult ar)
        {
            _allDone.Set();
            // Get the socket that handles the client request.  
            Socket listener = (Socket) ar.AsyncState;
            Socket handler = listener.EndAccept(ar);
            try
            {
                // Create the state object.  
                var connection = new Connection<T>(handler, new T());


                _onConnect(connection);

                handler.BeginReceive(connection.Buffer, 0, Connection<T>.BUFFER_SIZE, 0,
                    ReadCallback, connection);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private void ReadCallback(IAsyncResult ar)
        {
            var connection = (Connection<T>) ar.AsyncState;
            Socket handler = connection.Socket;

            try
            {
                // Retrieve the state object and the handler socket  
                // from the asynchronous state object.  

                int bytesRead = handler.EndReceive(ar);
                if (bytesRead > 0)
                {
                    var rec = new byte[bytesRead];
                    Buffer.BlockCopy(connection.Buffer, 0, rec, 0, bytesRead);

                    _onReceive(new ByteBuffer(rec), connection);

                    handler.BeginReceive(connection.Buffer, 0, Connection<T>.BUFFER_SIZE, 0,
                        ReadCallback, connection);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"Connection[{connection.ID}] closed!");
            }
        }

    }
}