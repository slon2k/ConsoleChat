using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;

namespace ConsoleChat.Server
{
    public class ClientObject
    {
        protected internal string Id { get; private set; }
        protected internal NetworkStream Stream { get; private set; }
        string userName;
        TcpClient tcpClient;
        ServerObject server;

        public ClientObject(TcpClient client, ServerObject server)
        {
            Id = Guid.NewGuid().ToString();
            tcpClient = client;
            this.server = server;
            server.AddConnection(this);
        }

        public void Process()
        {
            try
            {
                Stream = tcpClient.GetStream();
                userName = GetMessage();
                string message = userName + " joined chat.";
                server.BroadcastMessage(message, Id);
                Console.WriteLine(message);

                while (true)
                {
                    try
                    {
                        message = $@"{userName}: {GetMessage()}";
                        server.BroadcastMessage(message, Id);
                        Console.WriteLine(message);
                    }
                    catch (Exception)
                    {
                        message = userName + " left chat.";
                        server.BroadcastMessage(message, Id);
                        Console.WriteLine(message);
                        break;
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            finally
            {
                server.RemoveConnection(Id);
                Close();
            }
        }

        private string GetMessage()
        {
            byte[] data = new byte[64];
            StringBuilder builder = new StringBuilder();
            int bytes;
            do
            {
                bytes = Stream.Read(data, 0, data.Length);
                builder.Append(Encoding.Unicode.GetString(data, 0, bytes));
            }
            while (Stream.DataAvailable);

            return builder.ToString();
        }

        protected internal void Close()
        {
            if (Stream != null)
            {
                Stream.Close();
            }
                
            if (tcpClient != null)
            {
                tcpClient.Close();
            }               
        }
    }
}
