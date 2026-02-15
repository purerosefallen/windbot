using System;
using System.Net;
using System.Net.Sockets;

namespace YGOSharp.Network
{
    public interface INetworkClient
    {
        event Action Connected;
        event Action<Exception> Disconnected;
        event Action<byte[]> DataReceived;

        bool IsConnected { get; }
        IPAddress RemoteIPAddress { get; }

        void Initialize(Socket socket);
        void BeginConnect(IPAddress address, int port);
        void BeginSend(byte[] data);
        void BeginReceive();
        void Close(Exception error = null);
    }
}
