using System;
using System.IO;
using System.Threading;
using System.Net;
using System.Web;
using WindBot.Game;
using WindBot.Game.AI;
using YGOSharp.Network;
using YGOSharp.OCGWrapper;

namespace WindBot
{
    public class Program
    {
        internal static Random Rand;

        internal static void Main(string[] args)
        {
            Logger.WriteLine("WindBot starting...");

            Config.Load(args);

            string databasePath = Config.GetString("DbPath", "cards.cdb");

            InitDatas(databasePath);

            bool serverMode = Config.GetBool("ServerMode", false);

            if (serverMode)
            {
                // Run in server mode, provide a http interface to create bot.
                int serverPort = Config.GetInt("ServerPort", 2399);
                RunAsServer(serverPort);
            }
            else
            {
                // Join the host specified on the command line.
                if (args.Length == 0)
                {
                    Logger.WriteErrorLine("=== WARN ===");
                    Logger.WriteLine("No input found, tring to connect to localhost YGOPro host.");
                    Logger.WriteLine("If it fail, the program will quit sliently.");
                }
                RunFromArgs();
            }
        }

        public static void InitDatas(string databasePath)
        {
            Rand = new Random();
            DecksManager.Init();
            string absolutePath = Path.GetFullPath(databasePath);
            if (!File.Exists(absolutePath))
                // In case windbot is placed in a folder under ygopro folder
                absolutePath = Path.GetFullPath("../" + databasePath);
            if (!File.Exists(absolutePath))
                // In case windbot is placed in a folder under ygopro2 folder
                absolutePath = Path.GetFullPath("../cdb/" + databasePath);
            if (!File.Exists(absolutePath))
            {
                Logger.WriteErrorLine("Can't find cards database file.");
                Logger.WriteErrorLine("Please place cards.cdb next to WindBot.exe or Bot.exe .");
                Logger.WriteLine("Press any key to quit...");
                Console.ReadKey();
                System.Environment.Exit(1);
            }
            NamedCardsManager.Init(absolutePath);
        }

        private static void RunFromArgs()
        {
            WindBotInfo Info = new WindBotInfo();
            Info.Name = Config.GetString("Name", Info.Name);
            Info.Deck = Config.GetString("Deck", Info.Deck);
            Info.DeckFile = Config.GetString("DeckFile", Info.DeckFile);
            Info.Dialog = Config.GetString("Dialog", Info.Dialog);
            Info.Host = Config.GetString("Host", Info.Host);
            Info.Port = Config.GetInt("Port", Info.Port);
            Info.HostInfo = Config.GetString("HostInfo", Info.HostInfo);
            Info.Version = Config.GetInt("Version", Info.Version);
            Info.Hand = Config.GetInt("Hand", Info.Hand);
            Info.Debug = Config.GetBool("Debug", Info.Debug);
            Info.Chat = Config.GetBool("Chat", Info.Chat);
            Info.DeckCode = Config.GetString("DeckCode", Info.DeckCode);
            Info.RealIP = Config.GetString("RealIP", Info.RealIP);
            Run(Info);
        }

        private static void RunAsServer(int ServerPort)
        {
                Logger.WriteLine("WindBot server start successed.");
                Logger.WriteLine("HTTP GET http://127.0.0.1:" + ServerPort + "/?name=WindBot&host=127.0.0.1&port=7911 to call the bot.");
                ServerModeHost.Run(ServerPort, new ParameterizedThreadStart(Run), delegate (string rawUrl, out WindBotInfo Info, out string port)
                {
                    Info = new WindBotInfo();
                    string RawUrl = Path.GetFileName(rawUrl);
                    Info.Name = HttpUtility.ParseQueryString(RawUrl).Get("name");
                    Info.Deck = HttpUtility.ParseQueryString(RawUrl).Get("deck");
                    Info.Host = HttpUtility.ParseQueryString(RawUrl).Get("host");
                    port = HttpUtility.ParseQueryString(RawUrl).Get("port");
                    if (port != null)
                        Info.Port = Int32.Parse(port);
                    string deckfile = HttpUtility.ParseQueryString(RawUrl).Get("deckfile");
                    if (deckfile != null)
                        Info.DeckFile = deckfile;
                    string dialog = HttpUtility.ParseQueryString(RawUrl).Get("dialog");
                    if (dialog != null)
                        Info.Dialog = dialog;
                    string version = HttpUtility.ParseQueryString(RawUrl).Get("version");
                    if (version != null)
                        Info.Version = Int16.Parse(version);
                    string password = HttpUtility.ParseQueryString(RawUrl).Get("password");
                    if (password != null)
                        Info.HostInfo = password;
                    string hand = HttpUtility.ParseQueryString(RawUrl).Get("hand");
                    if (hand != null)
                        Info.Hand = Int32.Parse(hand);
                    string debug = HttpUtility.ParseQueryString(RawUrl).Get("debug");
                    if (debug != null)
                        Info.Debug= bool.Parse(debug);
                    string chat = HttpUtility.ParseQueryString(RawUrl).Get("chat");
                    if (chat != null)
                        Info.Chat = bool.Parse(chat);
                    string deckcode = HttpUtility.ParseQueryString(RawUrl).Get("deckcode");
                    if (deckcode != null)
                        Info.DeckCode = deckcode;
                    string realIP = HttpUtility.ParseQueryString(RawUrl).Get("realip");
                    if (realIP != null)
                        Info.RealIP = realIP;
                });
        }

        private static void Run(object o)
        {
#if !DEBUG
    try
    {
    //all errors will be catched instead of causing the program to crash.
#endif
            WindBotInfo Info = o as WindBotInfo;
            YGOClient presetConnection = null;

            ServerRunRequest request = o as ServerRunRequest;
            if (request != null)
            {
                Info = request.Info;
                presetConnection = request.Connection;
            }
            GameClient client = (presetConnection == null) ? new GameClient(Info) : new GameClient(Info, presetConnection);
            client.Start();
            Logger.DebugWriteLine(client.Username + " started.");
            while (client.Connection.IsConnected)
            {
#if !DEBUG
        try
        {
#endif
                client.Tick();
                Thread.Sleep(30);
#if !DEBUG
        }
        catch (Exception ex)
        {
            Logger.WriteErrorLine("Tick Error: " + ex);
        }
#endif
            }
            Logger.DebugWriteLine(client.Username + " end.");
#if !DEBUG
    }
    catch (Exception ex)
    {
        Logger.WriteErrorLine("Run Error: " + ex);
    }
#endif
        }

        public static FileStream ReadFile(string directory, string filename, string extension)
        {
            string tryfilename = filename + "." + extension;
            string fullpath = Path.Combine(directory, tryfilename);
            if (!File.Exists(fullpath))
                fullpath = filename;
            if (!File.Exists(fullpath))
                fullpath = Path.Combine("../", filename);
            if (!File.Exists(fullpath))
                fullpath = Path.Combine("../deck/", filename);
            if (!File.Exists(fullpath))
                fullpath = Path.Combine("../", tryfilename);
            if (!File.Exists(fullpath))
                fullpath = Path.Combine("../deck/", tryfilename);
            return new FileStream(fullpath, FileMode.Open, FileAccess.Read);
        }
    }
}
