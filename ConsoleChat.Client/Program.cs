using ConsoleChat.Common;
using System;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace ConsoleChat.Client
{
    class Program
    {
        static string userName;
        private const string host = "localhost";
        private const int port = Config.Port;
        static TcpClient client;
        static NetworkStream stream;
        
        static void Main(string[] args)
        {
            Console.Write("Input your name: ");
            userName = Console.ReadLine();
            client = new TcpClient();
            try
            {
                client.Connect(host, port);
                stream = client.GetStream();
                var data = userName.Encode();
                stream.Write(data, 0, data.Length);

                Thread receiveThread = new Thread(new ThreadStart(ReceiveMessage));
                receiveThread.Start();
                Console.WriteLine(@$"Welcome, {userName}");
                SendMessage();

            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            finally
            {
                Disconnect();               
            }
        }

        static void SendMessage()
        {
            Console.WriteLine("Input new message:");
            while (true)
            {
                string message = Console.ReadLine();
                var data = message.Encode();
                stream.Write(data, 0, data.Length);
            }
        }

        static void ReceiveMessage()
        {
            try
            {
                byte[] data = new byte[64];
                StringBuilder builder = new StringBuilder();
                int bytes;
                
                do
                {
                    bytes = stream.Read(data, 0, data.Length);
                    builder.Append(Encoding.UTF8.GetString(data, 0, bytes));
                } while (stream.DataAvailable);

                Console.WriteLine(builder.ToString());
            }
            catch (Exception)
            {
                Console.WriteLine("Connection was lost");
                Console.ReadLine();
                Disconnect();
            }
        }

        private static void Disconnect()
        {
            if (stream != null)
            {
                stream.Close();
            }

            if (client != null)
            {
                client.Close();
            }

            Environment.Exit(0);
        }
    }
}
