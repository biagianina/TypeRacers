using Common;
using Server;
using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace TypeRacers.Server
{
    internal class ServerSetup
    {
        private TcpListener server;
        private Rooms playrooms;
        private TcpClient client;

        public void Setup()
        {
            server = new TcpListener(IPAddress.IPv6Any, 80);
            server.Server.SetSocketOption(SocketOptionLevel.IPv6, SocketOptionName.IPv6Only, false);
            playrooms = new Rooms();

            try
            {
                server.Start();
            }
            catch (Exception)
            {
                throw;
            }

            Console.WriteLine("Server started");

            CommunicationSetup();
        }

        private void CommunicationSetup()
        {
            while (true)
            {
                Thread thread = new Thread(() =>
                {
                    client = server.AcceptTcpClient();
                    Player newConnectedClient = new Player(new TypeRacersNetworkClient(client));
                    playrooms.AllocatePlayroom(newConnectedClient);
                });
                thread.Start();
            }
        }
    }
}