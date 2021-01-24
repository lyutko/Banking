using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace Server
{
    public class ServerObject
    {
        private const int port = 30000;
        static TcpListener tcpListener;
        List<ClientObject> clients = new List<ClientObject>();



        protected internal void AddConnection(ClientObject clientObject) => clients.Add(clientObject);
        protected internal void RemoveConnection(string id)
        {
            ClientObject client = clients.FirstOrDefault(c => c.Id == id);
            if (client != null)
                clients.Remove(client);
        }
        protected internal bool UserConnectionIsRepeated(string userName, ClientObject curClient)
        {
            ClientObject client = clients.FirstOrDefault(c => c.UserName == userName);
            return client != null && client != curClient;
        }
        protected internal ClientObject GetAccountFoOrherOperation(string accounNumber, ClientObject curClient)
        {
            foreach (var client in clients)
            {
                var account = client.Accounts?.FirstOrDefault(c => c.Number == accounNumber);
                if (account != null && client != curClient)
                    return client;
            }
            return null;
        }



        protected internal void Listen()
        {
            try
            {
                tcpListener = new TcpListener(IPAddress.Any, port);
                tcpListener.Start();
                Console.WriteLine("Server is run. Wait for connections...\n");

                while (true)
                {
                    TcpClient tcpClient = tcpListener.AcceptTcpClient();

                    ClientObject clientObject = new ClientObject(tcpClient, this, new DbHelper());
                    Thread clientThread = new Thread(async () => await clientObject.Process());
                    clientThread.Start();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Disconnect();
            }
        }



        protected internal void BroadcastMessage(string message, ClientObject client)
        {
            byte[] data = Encoding.Unicode.GetBytes(message);
            client.Stream.Write(data, 0, data.Length);
        }

        protected internal void BroadcastOperationResult(string result, ClientObject fromClient, ClientObject toClient = null)
        {
            byte[] data = Encoding.Unicode.GetBytes(result);
            fromClient.Stream.Write(data, 0, data.Length);
            toClient?.Stream.Write(data, 0, data.Length);
        }



        protected internal void Disconnect()
        {
            tcpListener.Stop();
            for (int i = 0; i < clients.Count; i++)
                clients[i].Close();
            Environment.Exit(0);
        }
    }
}
