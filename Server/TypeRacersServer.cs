using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace TypeRacers.Server
{
    public class TypeRacersServer
    {
        static void Main()
        {
            //starting the server
            ServerSetup();
        }

        public static void ServerSetup()
        {
            IPAddress ip = Dns.GetHostEntry("localhost").AddressList[0];
            TcpListener server = new TcpListener(ip, 80);
            try
            {
                server.Start();
            }
            catch (Exception exception)
            {
                throw new Exception(exception.Message);
            }

            while (true)
            {
                TcpClient client = server.AcceptTcpClient();
                HandleClient cl = new HandleClient(); //server sends a message to a new connected client
                cl.StartClient(client);
            }
        }
    }
}