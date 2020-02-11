using Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace TypeRacers.Server
{
    internal class ServerSetup
    {
        private static string CompetitionText => ServerGeneratedText.GetText();

        private TcpListener server;
        private NetworkStream networkStream;

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
            catch (Exception e)
            {
                throw e;
            }

            Console.WriteLine("Server started");
            CommunicationSetup();//separated the communication from server starter
        }

        //to implement thread with start func players.Allocate method
        private void CommunicationSetup()
        {
            while (true)
            {

                client = server.AcceptTcpClient();
                networkStream = client.GetStream();
                Thread thread = new Thread(() =>
                {
                    Player newPlayer = new Player(client, networkStream);
                    playrooms.AllocatePlayroom(newPlayer);
                });
                thread.Start();
            }
        }
    }
}