using System;
using System.Collections.Generic;
using System.Net.Sockets;

namespace Unify.Server
{
    public class Connection<T> : IDisposable
    {
        public T IdentificationObj { get; }

        // Size of receive buffer.  
        public const int BUFFER_SIZE = 1024;

        // Receive buffer.  
        public byte[] Buffer = new byte[BUFFER_SIZE];

        // Client socket.
        public readonly Socket Socket;
        public readonly Guid ID;


        private readonly List<Action<Connection<T>>> _onCloseHandlers = new List<Action<Connection<T>>>();
        public Connection(Socket socket,T identificationObj)
        {
            ID = Guid.NewGuid();
            Socket = socket;
            IdentificationObj = identificationObj;
        }
        public void Send(Packet packet)
        {
            var bytes = packet.GetBytes();
            Socket.BeginSend(bytes, 0, bytes.Length, 0,
                new AsyncCallback(SendCallback), Socket);
        }

        public void Send(params Packet[] packets)
        {
            foreach (var packet in packets)
            {
                Send(packet);
            }
        }
        private void SendCallback(IAsyncResult ar)
        {
            try
            {
                // Retrieve the socket from the state object.  
                Socket handler = (Socket) ar.AsyncState;

                // Complete sending the data to the remote device.  
                handler.EndSend(ar);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
        public void AppendCloseHandler(Action<Connection<T>> onCloseAction)
        {
            _onCloseHandlers.Add(onCloseAction);
        }
        private bool IsConnected()
        {
            try
            {
                return !(Socket.Poll(1, SelectMode.SelectRead) && Socket.Available == 0);
            }
            catch (Exception)
            {
                return false;
            }
        }
        public void Dispose()                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                     
        {
            Socket?.Dispose();
        }
    }
}