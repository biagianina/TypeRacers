using System;
using System.Net;
using System.Net.Sockets;

namespace echo
{
    public class Server
    {
        private TcpClient client;
        private IPAddress ip = Dns.GetHostEntry("localhost").AddressList[0];
        private TcpListener server;

        public void ServerSetup()
        {
            server = new TcpListener(ip, 80);
            try
            {
                server.Start();
            }
            catch (Exception exception)
            {
                throw new Exception(exception.Message);
            }

            Console.WriteLine("Server started");
            while (true)
            {
                client = server.AcceptTcpClient();
                Console.WriteLine("New clien accepted");
                HandleClient cl = new HandleClient(); //server sends a message to a new connected client
                cl.StartClient(client);
                Console.WriteLine("Handled client");
            }
        }
    }
}