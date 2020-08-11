using System;
using System.Threading;

namespace ConsoleChat.Server
{
    class Program
    {
        static ServerObject server;
        static Thread listenThread;

        static void Main(string[] args)
        {
            try
            {
                server = new ServerObject();
                listenThread = new Thread(new ThreadStart(server.Listen));
                listenThread.Start();
            }
            catch (Exception e)
            {
                server.Disconnect();
                Console.WriteLine(e.Message);
            }
        }
    }
}
