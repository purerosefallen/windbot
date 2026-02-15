using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using YGOSharp.Network;

namespace WindBot
{
    internal class ServerRunRequest
    {
        public WindBotInfo Info;
        public YGOClient Connection;
    }

    internal delegate void ParseServerInfoCallback(string rawUrl, out WindBotInfo info, out string port);

    internal static class ServerModeHost
    {
        internal static void Run(int serverPort, ParameterizedThreadStart runCallback, ParseServerInfoCallback parseInfoCallback)
        {
            TcpListener mainServer = new TcpListener(IPAddress.Any, serverPort);
            mainServer.Start();
            while (true)
            {
#if !DEBUG
    try
    {
#endif
                Socket socket = mainServer.AcceptSocket();
                ThreadPool.QueueUserWorkItem(delegate { HandleServerSocket(socket, runCallback, parseInfoCallback); });
#if !DEBUG
    }
    catch (Exception ex)
    {
        Logger.WriteErrorLine("Accept Socket Error: " + ex);
    }
#endif
            }
        }

        private static void HandleServerSocket(Socket socket, ParameterizedThreadStart runCallback, ParseServerInfoCallback parseInfoCallback)
        {
            bool handoffToBot = false;
            try
            {
                string method;
                string rawUrl;
                Dictionary<string, string> headers;
                if (!TryReadHttpRequest(socket, out method, out rawUrl, out headers))
                    return;

                WindBotInfo info;
                string port;
                parseInfoCallback(rawUrl, out info, out port);
                if (info == null)
                    info = new WindBotInfo();

                string upgradeHeader = GetHeader(headers, "upgrade");
                string connectionHeader = GetHeader(headers, "connection");
                string webSocketKey = GetHeader(headers, "sec-websocket-key");
                string webSocketVersion = GetHeader(headers, "sec-websocket-version");
                bool isWebSocketRequest =
                    string.Equals(method, "GET", StringComparison.OrdinalIgnoreCase) &&
                    !string.IsNullOrEmpty(webSocketKey) &&
                    !string.IsNullOrEmpty(upgradeHeader) &&
                    upgradeHeader.Equals("websocket", StringComparison.OrdinalIgnoreCase) &&
                    !string.IsNullOrEmpty(connectionHeader) &&
                    connectionHeader.IndexOf("upgrade", StringComparison.OrdinalIgnoreCase) >= 0;

                if (isWebSocketRequest)
                {
                    if (info.Name == null)
                        info.Name = "WindBot";
                    if (info.Host == null)
                    {
                        IPEndPoint remote = socket.RemoteEndPoint as IPEndPoint;
                        info.Host = remote != null ? remote.Address.ToString() : "reverse-ws";
                    }

                    if (!string.IsNullOrEmpty(webSocketVersion) && webSocketVersion != "13")
                    {
                        WriteHttpResponse(socket, 426, "Upgrade Required", "Unsupported WebSocket Version");
                        return;
                    }

                    string acceptKey = ComputeWebSocketAccept(webSocketKey);
                    StringBuilder response = new StringBuilder();
                    response.Append("HTTP/1.1 101 Switching Protocols\r\n");
                    response.Append("Upgrade: websocket\r\n");
                    response.Append("Connection: Upgrade\r\n");
                    response.Append("Sec-WebSocket-Accept: ").Append(acceptKey).Append("\r\n\r\n");
                    byte[] responseBytes = Encoding.ASCII.GetBytes(response.ToString());
                    socket.Send(responseBytes);

                    ReverseWsSocketClient reverseWsClient = new ReverseWsSocketClient(socket);
                    YGOClient ygoClient = new YGOClient(reverseWsClient);

                    ServerRunRequest request = new ServerRunRequest();
                    request.Info = info;
                    request.Connection = ygoClient;

                    Thread workThread = new Thread(runCallback);
                    workThread.Start(request);
                    handoffToBot = true;
                    return;
                }

                bool canUseHostWithoutPort = false;
                if (info.Host != null && info.Host.IndexOf("//", StringComparison.Ordinal) >= 0)
                {
                    Uri uri;
                    if (Uri.TryCreate(info.Host, UriKind.Absolute, out uri))
                    {
                        string scheme = uri.Scheme.ToLowerInvariant();
                        canUseHostWithoutPort = (scheme == "ws" || scheme == "wss") || (scheme == "tcp" && !uri.IsDefaultPort);
                    }
                }

                if (info.Name == null || info.Host == null || (port == null && !canUseHostWithoutPort))
                {
                    WriteHttpResponse(socket, 400, "Bad Request", "");
                    return;
                }

#if !DEBUG
        try
        {
#endif
                Thread normalWorkThread = new Thread(runCallback);
                normalWorkThread.Start(info);
#if !DEBUG
        }
        catch (Exception ex)
        {
            Logger.WriteErrorLine("Start Thread Error: " + ex);
        }
#endif
                WriteHttpResponse(socket, 200, "OK", "");
#if !DEBUG
    }
    catch (Exception ex)
    {
        Logger.WriteErrorLine("Handle Socket Request Error: " + ex);
    }
#endif
            finally
            {
                if (!handoffToBot)
                {
                    try
                    {
                        socket.Shutdown(SocketShutdown.Both);
                    }
                    catch
                    {
                    }
                    socket.Close();
                }
            }
        }

        private static bool TryReadHttpRequest(Socket socket, out string method, out string rawUrl, out Dictionary<string, string> headers)
        {
            method = null;
            rawUrl = null;
            headers = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

            NetworkStream stream = new NetworkStream(socket, false);
            MemoryStream headerBuffer = new MemoryStream();
            int state = 0;
            while (headerBuffer.Length < 65536)
            {
                int b = stream.ReadByte();
                if (b < 0)
                    break;
                headerBuffer.WriteByte((byte)b);
                if (state == 0)
                    state = (b == '\r') ? 1 : 0;
                else if (state == 1)
                    state = (b == '\n') ? 2 : 0;
                else if (state == 2)
                    state = (b == '\r') ? 3 : 0;
                else if (state == 3)
                {
                    if (b == '\n')
                        break;
                    state = 0;
                }
            }

            string requestText = Encoding.ASCII.GetString(headerBuffer.ToArray());
            if (string.IsNullOrEmpty(requestText))
                return false;

            string[] lines = requestText.Split(new[] { "\r\n" }, StringSplitOptions.None);
            if (lines.Length == 0 || string.IsNullOrWhiteSpace(lines[0]))
                return false;

            string[] requestLineParts = lines[0].Split(' ');
            if (requestLineParts.Length < 3)
                return false;

            method = requestLineParts[0];
            rawUrl = requestLineParts[1];

            for (int i = 1; i < lines.Length; ++i)
            {
                string line = lines[i];
                if (string.IsNullOrEmpty(line))
                    break;
                int colon = line.IndexOf(':');
                if (colon <= 0)
                    continue;
                string key = line.Substring(0, colon).Trim();
                string value = line.Substring(colon + 1).Trim();
                headers[key] = value;
            }
            return true;
        }

        private static string GetHeader(Dictionary<string, string> headers, string key)
        {
            string value;
            if (headers.TryGetValue(key, out value))
                return value;
            return null;
        }

        private static void WriteHttpResponse(Socket socket, int statusCode, string statusText, string body)
        {
            if (body == null)
                body = "";

            byte[] bodyBytes = Encoding.UTF8.GetBytes(body);
            StringBuilder builder = new StringBuilder();
            builder.Append("HTTP/1.1 ").Append(statusCode).Append(' ').Append(statusText).Append("\r\n");
            builder.Append("Connection: close\r\n");
            builder.Append("Content-Type: text/plain; charset=utf-8\r\n");
            builder.Append("Content-Length: ").Append(bodyBytes.Length).Append("\r\n\r\n");
            byte[] headerBytes = Encoding.ASCII.GetBytes(builder.ToString());
            socket.Send(headerBytes);
            if (bodyBytes.Length > 0)
                socket.Send(bodyBytes);
        }

        private static string ComputeWebSocketAccept(string secWebSocketKey)
        {
            string combined = secWebSocketKey + "258EAFA5-E914-47DA-95CA-C5AB0DC85B11";
            byte[] bytes = Encoding.ASCII.GetBytes(combined);
            using (SHA1 sha1 = SHA1.Create())
            {
                return Convert.ToBase64String(sha1.ComputeHash(bytes));
            }
        }
    }
}
