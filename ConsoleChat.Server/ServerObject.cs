using ConsoleChat.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace ConsoleChat.Server
{
     
    
    public class ServerObject
    {
        static TcpListener tcpListener;
        private readonly List<ClientObject> clients = new List<ClientObject>();
        private const int port = Config.Port;

        protected internal void AddConnection(ClientObject client)
        {
            clients.Add(client);
        }

        protected internal void RemoveConnection(string id)
        {
            var client = clients.FirstOrDefault(c => c.Id == id);
            if (client != null)
            {
                clients.Remove(client);
            }
        }

        protected internal void Listen()
        {
            try
            {
                tcpListener = new TcpListener(IPAddress.Any, port);
                tcpListener.Start();
                Console.WriteLine("Waiting...");
                while (true)
                {
                    TcpClient tcpClient = tcpListener.AcceptTcpClient();

                    ClientObject clientObject = new ClientObject(tcpClient, this);
                    Thread clientThread = new Thread(new ThreadStart(clientObject.Process));
                    clientThread.Start();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Disconnect();
            }
        }
        
        protected internal void BroadcastMessage(string messsage, string id)
        {
            var audience = clients.FindAll(c => c.Id != id);
            foreach (var client in audience)
            {
                var data = messsage.Encode();
                client.Stream.Write(data, 0, data.Length);
            }
        } 

        protected internal void Disconnect()
        {
            tcpListener.Stop();
            foreach (var client in clients)
            {
                client.Close();
            }
            Environment.Exit(0);
        }
    }
}