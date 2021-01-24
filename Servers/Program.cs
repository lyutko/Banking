using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace Server
{
    struct TransferObject
    {
        public Socket Socket { get; set; }
        public byte[] Buffer { get; set; }
        public static readonly int size = 4096;
    }

    class Program
    {
        private static IPAddress ip;
        private static ServerObject server;
        static Thread listenThread;

        static void Main(string[] args)
        {
            StartServer();
        }

        private static void StartServer()
        {
            ip = Dns.GetHostEntry(Dns.GetHostName()).AddressList.LastOrDefault(a => a.AddressFamily == AddressFamily.InterNetwork);
            Console.Title = "Server: " + ip;
            try
            {
                server = new ServerObject();
                listenThread = new Thread(new ThreadStart(server.Listen));
                listenThread.Start();
            }
            catch (Exception ex)
            {
                server.Disconnect();
                Console.WriteLine("Connection failed: " + ex.Message);
            }
        }
    }
}
