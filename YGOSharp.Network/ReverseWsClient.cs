using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Net.WebSockets;
using System.Threading;

namespace YGOSharp.Network
{
    public class ReverseWsClient : INetworkClient
    {
        public event Action Connected;
        public event Action<Exception> Disconnected;
        public event Action<byte[]> DataReceived;

        public bool IsConnected { get; private set; }

        public IPAddress RemoteIPAddress { get; private set; }

        private readonly WebSocket _socket;
        private int _receiveStarted;
        private int _connectedRaised;
        private bool _isClosed;
        private readonly object _closeLock = new object();

        public ReverseWsClient(WebSocket socket, IPAddress remoteIPAddress = null)
        {
            _socket = socket;
            RemoteIPAddress = remoteIPAddress ?? IPAddress.None;
            IsConnected = socket != null && socket.State == WebSocketState.Open;
        }

        public void Initialize(Socket socket)
        {
            throw new NotSupportedException("ReverseWsClient does not support Initialize(Socket).");
        }

        public void BeginConnect(IPAddress address, int port)
        {
            throw new NotSupportedException("ReverseWsClient does not support BeginConnect(IPAddress, int).");
        }

        public void BeginSend(byte[] data)
        {
            if (_isClosed)
                return;

            try
            {
                if (_socket.State != WebSocketState.Open)
                {
                    Close();
                    return;
                }
                _socket.SendAsync(new ArraySegment<byte>(data), WebSocketMessageType.Binary, true, CancellationToken.None)
                    .GetAwaiter().GetResult();
            }
            catch (Exception ex)
            {
                Close(ex);
            }
        }

        public void BeginReceive()
        {
            if (IsConnected && Interlocked.Exchange(ref _connectedRaised, 1) == 0)
                Connected?.Invoke();

            if (Interlocked.Exchange(ref _receiveStarted, 1) != 0)
                return;

            ThreadPool.QueueUserWorkItem(delegate { ReceiveLoop(); });
        }

        public void Close(Exception error = null)
        {
            lock (_closeLock)
            {
                if (_isClosed)
                    return;
                _isClosed = true;
            }

            try
            {
                if (_socket.State == WebSocketState.Open || _socket.State == WebSocketState.CloseReceived)
                {
                    _socket.CloseAsync(WebSocketCloseStatus.NormalClosure, "closed", CancellationToken.None)
                        .GetAwaiter().GetResult();
                }
                else
                {
                    _socket.Abort();
                }
            }
            catch
            {
            }
            finally
            {
                IsConnected = false;
                Disconnected?.Invoke(error);
            }
        }

        private void ReceiveLoop()
        {
            byte[] buffer = new byte[4096];
            while (!_isClosed)
            {
                try
                {
                    WebSocketReceiveResult result;
                    using (MemoryStream stream = new MemoryStream())
                    {
                        do
                        {
                            result = _socket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None)
                                .GetAwaiter().GetResult();

                            if (result.MessageType == WebSocketMessageType.Close)
                            {
                                Close();
                                return;
                            }

                            if (result.MessageType != WebSocketMessageType.Binary)
                            {
                                Close(new Exception("ReverseWsClient only supports binary frames."));
                                return;
                            }

                            if (result.Count > 0)
                                stream.Write(buffer, 0, result.Count);
                        }
                        while (!result.EndOfMessage);

                        DataReceived?.Invoke(stream.ToArray());
                    }
                }
                catch (Exception ex)
                {
                    Close(ex);
                    return;
                }
            }
        }
    }
}
