using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using YGOSharp.Network;
using YGOSharp.Network.Enums;
using YGOSharp.Network.Utils;

namespace WindBot.Game
{
    public class GameClient
    {
        public YGOClient Connection { get; private set; }
        public string Username;
        public string Deck;
        public string DeckFile;
        public string DeckCode;
        public string Dialog;
        public int Hand;
        public bool Debug;
        public bool _chat;
        private string _serverHost;
        private int _serverPort;
        private string _realIP;
        private short _proVersion;

        private string _roomInfo;
        private readonly YGOClient _presetConnection;

        private GameBehavior _behavior;

        private enum ConnectionMode
        {
            Tcp,
            WebSocket
        }

        private class ConnectionTarget
        {
            public ConnectionMode Mode;
            public string ExternalHost;
            public string TcpHost;
            public int TcpPort;
            public Uri WebSocketUri;
        }

        public GameClient(WindBotInfo Info, YGOClient presetConnection = null)
        {
            Username = Info.Name;
            Deck = Info.Deck;
            DeckFile = Info.DeckFile;
            DeckCode = Info.DeckCode;
            Dialog = Info.Dialog;
            Hand = Info.Hand;
            Debug = Info.Debug;
            _chat = Info.Chat;
            _serverHost = Info.Host;
            _serverPort = Info.Port;
            _realIP = Info.RealIP;
            _roomInfo = Info.HostInfo;
            _proVersion = (short)Info.Version;
            _presetConnection = presetConnection;
        }

        public void Start()
        {
            bool shouldStartWsConnect = false;
            WsClient wsClient = null;
            IPAddress tcpAddress = null;

            if (_presetConnection != null)
            {
                Connection = _presetConnection;
            }
            else
            {
                ConnectionTarget target = ResolveConnectionTarget();
                _serverHost = target.ExternalHost;

                if (target.Mode == ConnectionMode.WebSocket)
                {
                    wsClient = new WsClient(target.WebSocketUri);
                    Connection = new YGOClient(wsClient);
                    shouldStartWsConnect = true;
                }
                else
                {
                    _serverPort = target.TcpPort;
                    Connection = new YGOClient();

                    try
                    {
                        tcpAddress = IPAddress.Parse(target.TcpHost);
                    }
                    catch (System.Exception)
                    {
                        IPHostEntry hostEntry = Dns.GetHostEntry(target.TcpHost);
                        tcpAddress = hostEntry.AddressList.FirstOrDefault(findIPv4 => findIPv4.AddressFamily == AddressFamily.InterNetwork);
                        if (tcpAddress == null)
                            throw new Exception("Can't resolve an IPv4 address for host '" + target.TcpHost + "'.");
                    }
                }
            }

            _behavior = new GameBehavior(this);
            RegisterConnectionEvents();

            if (shouldStartWsConnect)
            {
                wsClient.StartConnect();
                return;
            }

            if (_presetConnection == null)
                Connection.Connect(tcpAddress, _serverPort);
        }

        private void RegisterConnectionEvents()
        {
            Connection.Connected += OnConnected;
            Connection.PacketReceived += OnPacketReceived;
        }

        private ConnectionTarget ResolveConnectionTarget()
        {
            if (string.IsNullOrEmpty(_serverHost))
                throw new Exception("Host is empty.");

            if (_serverHost.IndexOf("//", StringComparison.Ordinal) < 0)
            {
                return new ConnectionTarget
                {
                    Mode = ConnectionMode.Tcp,
                    ExternalHost = _serverHost,
                    TcpHost = _serverHost,
                    TcpPort = _serverPort
                };
            }

            Uri uri;
            if (!Uri.TryCreate(_serverHost, UriKind.Absolute, out uri))
                throw new Exception("Invalid Host URL: '" + _serverHost + "'.");

            string scheme = uri.Scheme.ToLowerInvariant();
            if (scheme == "tcp")
            {
                if (string.IsNullOrEmpty(uri.Host))
                    throw new Exception("Invalid TCP Host URL: '" + _serverHost + "'.");

                int resolvedPort = _serverPort;
                if (!uri.IsDefaultPort)
                    resolvedPort = uri.Port;

                return new ConnectionTarget
                {
                    Mode = ConnectionMode.Tcp,
                    ExternalHost = uri.Host,
                    TcpHost = uri.Host,
                    TcpPort = resolvedPort
                };
            }

            if (scheme == "ws" || scheme == "wss")
            {
                return new ConnectionTarget
                {
                    Mode = ConnectionMode.WebSocket,
                    ExternalHost = uri.Host,
                    WebSocketUri = uri
                };
            }

            throw new Exception("Unsupported Host scheme '" + uri.Scheme + "'. Supported schemes: tcp, ws, wss.");
        }

        private uint ParseRealIpAsNetworkOrderInt32()
        {
            if (string.IsNullOrWhiteSpace(_realIP))
                return 0;

            string text = _realIP.Trim();
            if (text.StartsWith("::ffff:", StringComparison.OrdinalIgnoreCase))
                text = text.Substring("::ffff:".Length);

            IPAddress parsedIp;
            if (!IPAddress.TryParse(text, out parsedIp))
                throw new Exception("Invalid RealIP: '" + _realIP + "'.");

            if (parsedIp.AddressFamily == AddressFamily.InterNetworkV6 && parsedIp.IsIPv4MappedToIPv6)
                parsedIp = parsedIp.MapToIPv4();

            if (parsedIp.AddressFamily != AddressFamily.InterNetwork)
                throw new Exception("RealIP must be an IPv4 address: '" + _realIP + "'.");

            byte[] bytes = parsedIp.GetAddressBytes();
            uint networkOrdered = ((uint)bytes[0] << 24) | ((uint)bytes[1] << 16) | ((uint)bytes[2] << 8) | bytes[3];
            return unchecked((uint)IPAddress.NetworkToHostOrder((int)networkOrdered));
        }

        private void OnConnected()
        {
            BinaryWriter packet = GamePacketFactory.Create(CtosMessage.ExternalAddress);
            packet.Write(ParseRealIpAsNetworkOrderInt32());
            packet.WriteUnicodeAutoLength(_serverHost, 255);
            Connection.Send(packet);

            packet = GamePacketFactory.Create(CtosMessage.PlayerInfo);
            packet.WriteUnicode(Username, 20);
            Connection.Send(packet);

            byte[] junk = { 0xCC, 0xCC, 0x00, 0x00, 0x00, 0x00 };
            packet = GamePacketFactory.Create(CtosMessage.JoinGame);
            packet.Write(_proVersion);
            packet.Write(junk);
            packet.WriteUnicode(_roomInfo, 20);
            Connection.Send(packet);
        }

        public void Tick()
        {
            Connection.Update();
        }

        public void Chat(string message)
        {
            BinaryWriter chat = GamePacketFactory.Create(CtosMessage.Chat);
            chat.WriteUnicodeAutoLength(message, 255);
            Connection.Send(chat);
        }

        public void Surrender()
        {
            Connection.Send(CtosMessage.Surrender);
        }

        private void OnPacketReceived(BinaryReader reader)
        {
            _behavior.OnPacket(reader);
        }
    }
}
