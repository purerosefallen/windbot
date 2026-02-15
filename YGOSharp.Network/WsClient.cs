using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Net.WebSockets;
using System.Threading;

namespace YGOSharp.Network
{
    public class WsClient : INetworkClient
    {
        public event Action Connected;
        public event Action<Exception> Disconnected;
        public event Action<byte[]> DataReceived;

        public bool IsConnected { get; private set; }

        public IPAddress RemoteIPAddress { get; private set; }

        private readonly Uri _uri;
        private readonly ClientWebSocket _socket;
        private int _receiveStarted;
        private bool _isClosed;
        private readonly object _closeLock = new object();

        public WsClient(Uri uri)
        {
            _uri = uri;
            _socket = new ClientWebSocket();
            RemoteIPAddress = IPAddress.None;
        }

        public void StartConnect()
        {
            ThreadPool.QueueUserWorkItem(delegate { ConnectInternal(); });
        }

        public void Initialize(Socket socket)
        {
            throw new NotSupportedException("WsClient does not support Initialize(Socket).");
        }

        public void BeginConnect(IPAddress address, int port)
        {
            throw new NotSupportedException("WsClient does not support BeginConnect(IPAddress, int).");
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
                _socket.Dispose();
                IsConnected = false;
                Disconnected?.Invoke(error);
            }
        }

        private void ConnectInternal()
        {
            try
            {
                _socket.ConnectAsync(_uri, CancellationToken.None).GetAwaiter().GetResult();
                IsConnected = true;
                try
                {
                    IPAddress parsedIp;
                    if (IPAddress.TryParse(_uri.Host, out parsedIp))
                        RemoteIPAddress = parsedIp;
                }
                catch
                {
                }
                Connected?.Invoke();
                BeginReceive();
            }
            catch (Exception ex)
            {
                Close(ex);
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
                                Close(new Exception("WsClient only supports binary frames."));
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
