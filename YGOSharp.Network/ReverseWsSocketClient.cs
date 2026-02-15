using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace YGOSharp.Network
{
    public class ReverseWsSocketClient : INetworkClient
    {
        public event Action Connected;
        public event Action<Exception> Disconnected;
        public event Action<byte[]> DataReceived;

        public bool IsConnected { get; private set; }
        public IPAddress RemoteIPAddress { get; private set; }

        private readonly Socket _socket;
        private readonly NetworkStream _stream;
        private readonly object _sendLock = new object();
        private readonly object _closeLock = new object();
        private bool _isClosed;
        private int _receiveStarted;
        private int _connectedRaised;

        private MemoryStream _fragmentBuffer;
        private byte _fragmentOpcode;

        public ReverseWsSocketClient(Socket socket)
        {
            _socket = socket;
            _stream = new NetworkStream(socket, false);
            IPEndPoint remote = socket.RemoteEndPoint as IPEndPoint;
            RemoteIPAddress = remote != null ? remote.Address : IPAddress.None;
            IsConnected = true;
        }

        public void Initialize(Socket socket)
        {
            throw new NotSupportedException("ReverseWsSocketClient does not support Initialize(Socket).");
        }

        public void BeginConnect(IPAddress address, int port)
        {
            throw new NotSupportedException("ReverseWsSocketClient does not support BeginConnect(IPAddress, int).");
        }

        public void BeginSend(byte[] data)
        {
            if (_isClosed)
                return;

            try
            {
                SendFrame(0x2, data ?? new byte[0]);
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

            IsConnected = false;
            try
            {
                SendFrame(0x8, new byte[0]);
            }
            catch
            {
            }

            try
            {
                _socket.Shutdown(SocketShutdown.Both);
            }
            catch
            {
            }

            try
            {
                _socket.Close();
            }
            catch
            {
            }

            Disconnected?.Invoke(error);
        }

        private void ReceiveLoop()
        {
            while (!_isClosed)
            {
                try
                {
                    int b0 = _stream.ReadByte();
                    if (b0 < 0)
                    {
                        Close();
                        return;
                    }
                    int b1 = _stream.ReadByte();
                    if (b1 < 0)
                    {
                        Close();
                        return;
                    }

                    bool fin = (b0 & 0x80) != 0;
                    byte opcode = (byte)(b0 & 0x0F);
                    bool masked = (b1 & 0x80) != 0;
                    ulong payloadLength = (ulong)(b1 & 0x7F);
                    if (payloadLength == 126)
                    {
                        byte[] ext = ReadExact(2);
                        payloadLength = (ulong)((ext[0] << 8) | ext[1]);
                    }
                    else if (payloadLength == 127)
                    {
                        byte[] ext = ReadExact(8);
                        payloadLength = ((ulong)ext[0] << 56) |
                                        ((ulong)ext[1] << 48) |
                                        ((ulong)ext[2] << 40) |
                                        ((ulong)ext[3] << 32) |
                                        ((ulong)ext[4] << 24) |
                                        ((ulong)ext[5] << 16) |
                                        ((ulong)ext[6] << 8) |
                                        ext[7];
                    }

                    if (payloadLength > int.MaxValue)
                    {
                        Close(new Exception("Reverse WS frame is too large."));
                        return;
                    }

                    byte[] maskKey = null;
                    if (masked)
                        maskKey = ReadExact(4);

                    byte[] payload = ReadExact((int)payloadLength);
                    if (masked && payloadLength > 0)
                    {
                        for (int i = 0; i < payload.Length; ++i)
                            payload[i] = (byte)(payload[i] ^ maskKey[i % 4]);
                    }

                    if (opcode == 0x8)
                    {
                        Close();
                        return;
                    }
                    if (opcode == 0x9)
                    {
                        SendFrame(0xA, payload);
                        continue;
                    }
                    if (opcode == 0xA)
                        continue;

                    if (opcode == 0x2)
                    {
                        if (fin)
                        {
                            DataReceived?.Invoke(payload);
                        }
                        else
                        {
                            _fragmentOpcode = opcode;
                            _fragmentBuffer = new MemoryStream();
                            _fragmentBuffer.Write(payload, 0, payload.Length);
                        }
                        continue;
                    }

                    if (opcode == 0x0)
                    {
                        if (_fragmentBuffer == null)
                        {
                            Close(new Exception("Invalid continuation frame."));
                            return;
                        }
                        _fragmentBuffer.Write(payload, 0, payload.Length);
                        if (fin)
                        {
                            if (_fragmentOpcode == 0x2)
                                DataReceived?.Invoke(_fragmentBuffer.ToArray());
                            _fragmentBuffer.Dispose();
                            _fragmentBuffer = null;
                            _fragmentOpcode = 0;
                        }
                        continue;
                    }

                    if (opcode == 0x1)
                    {
                        Close(new Exception("Reverse WS only supports binary frames."));
                        return;
                    }
                }
                catch (Exception ex)
                {
                    Close(ex);
                    return;
                }
            }
        }

        private byte[] ReadExact(int count)
        {
            byte[] data = new byte[count];
            int offset = 0;
            while (offset < count)
            {
                int read = _stream.Read(data, offset, count - offset);
                if (read <= 0)
                    throw new IOException("Socket closed.");
                offset += read;
            }
            return data;
        }

        private void SendFrame(byte opcode, byte[] payload)
        {
            if (payload == null)
                payload = new byte[0];

            lock (_sendLock)
            {
                if (_isClosed)
                    return;

                using (MemoryStream frame = new MemoryStream())
                {
                    frame.WriteByte((byte)(0x80 | (opcode & 0x0F)));

                    int len = payload.Length;
                    if (len <= 125)
                    {
                        frame.WriteByte((byte)len);
                    }
                    else if (len <= ushort.MaxValue)
                    {
                        frame.WriteByte(126);
                        frame.WriteByte((byte)((len >> 8) & 0xFF));
                        frame.WriteByte((byte)(len & 0xFF));
                    }
                    else
                    {
                        frame.WriteByte(127);
                        ulong l = (ulong)len;
                        frame.WriteByte((byte)((l >> 56) & 0xFF));
                        frame.WriteByte((byte)((l >> 48) & 0xFF));
                        frame.WriteByte((byte)((l >> 40) & 0xFF));
                        frame.WriteByte((byte)((l >> 32) & 0xFF));
                        frame.WriteByte((byte)((l >> 24) & 0xFF));
                        frame.WriteByte((byte)((l >> 16) & 0xFF));
                        frame.WriteByte((byte)((l >> 8) & 0xFF));
                        frame.WriteByte((byte)(l & 0xFF));
                    }

                    if (len > 0)
                        frame.Write(payload, 0, len);

                    byte[] bytes = frame.ToArray();
                    _stream.Write(bytes, 0, bytes.Length);
                    _stream.Flush();
                }
            }
        }
    }
}
