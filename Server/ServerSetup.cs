using Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace TypeRacers.Server
{
    internal class ServerSetup
    {
        private static string CompetitionText => ServerGeneratedText.GetText();

        private TcpListener server;
        private NetworkStream networkStream;
        private string currentClient;
        private Rooms playrooms;
        private DateTime currentPlayroomStartingTime;
        private TcpClient client;
        private Player currentPlayer;
        private bool newClient;

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
                //creates the stream
                networkStream = client.GetStream();
                //reads from stream
                byte[] buffer = new byte[client.ReceiveBufferSize];
                int bytesRead = networkStream.Read(buffer, 0, buffer.Length);

                string dataReceived = Encoding.ASCII.GetString(buffer, 0, bytesRead);

                //solution to get get complete messages
                while (!dataReceived.Last().Equals('#'))
                {
                    bytesRead = networkStream.Read(buffer, 0, client.ReceiveBufferSize);
                    dataReceived += Encoding.ASCII.GetString(buffer, dataReceived.Length, bytesRead);
                }

                CheckClientReceievedData(dataReceived);
                //check if reading from the stream has been done on the other end in order to close client

                Console.WriteLine("info: " + dataReceived);
                Console.WriteLine("Disconnected client");
            }
        }

        //to be moved to Player (happens each communication)
        private void CheckClientReceievedData(string dataReceived)
        {
            if (CheckIfGameIsRestarted(dataReceived) || CheckIfClientLeftGame(dataReceived))
            {
                return;
            }

            //progress and slider progress
            string progress = dataReceived.Substring(0, dataReceived.IndexOf('$'));

            var progressInfoAndPlayerRoomInfo = progress.Split('&');

            string username = dataReceived.Substring(dataReceived.IndexOf('$') + 1);

            currentClient = username.Substring(0, username.Length - 1);

            if (currentClient == string.Empty)
            {
                return;
            }

            currentPlayer = new Player(currentClient);
            newClient = true;
            currentPlayer.UpdateInfo(Convert.ToInt32(progressInfoAndPlayerRoomInfo[0]), Convert.ToInt32(progressInfoAndPlayerRoomInfo[1]), Convert.ToInt32(progressInfoAndPlayerRoomInfo[2]));

            CheckIfClientLeftGame(currentClient);
        }

        //to be moved to player
        private bool CheckIfClientLeftGame(string currentClient)
        {
            if (currentClient.Contains("_removed"))
            {
                string toRemove = currentClient.Substring(0, currentClient.IndexOf('_'));
                currentPlayer.Playroom.RemovePlayer(toRemove);
                return true;
            }

            return false;
        }

        //to be moved to player
        private bool CheckIfGameIsRestarted(string dataReceived)
        {
            if (dataReceived.Contains("_restart"))
            {
                Console.WriteLine("restart");
                byte[] broadcastBytes = Encoding.ASCII.GetBytes(currentPlayer.Playroom.TimeToWaitForOpponents + "#");
                networkStream.Write(broadcastBytes, 0, broadcastBytes.Length);
                networkStream.Close();
                client.Close();
                return true;
            }

            return false;
        }


        //to be moved to player (as first connection -> see playroom method)
        private void CheckNewClient(int roomNumber)
        {
            if (newClient) //player.FirstConnection() method
            {
                byte[] broadcastBytes = Encoding.ASCII.GetBytes(CompetitionText + "$" + roomNumber + "%" + currentPlayer.Playroom.TimeToWaitForOpponents.ToString() + "*" + currentPlayer.Playroom.GameStartingTime + "+" + currentPlayer.Playroom.GameEndingTime + "#"); //generates random text from text document
                networkStream.Write(broadcastBytes, 0, broadcastBytes.Length);//send the text to connected client

                networkStream.Close();
                client.Close();
            }
            else
            {
                SendOpponents();//player.UpdateOpponents() method
            }
        }

        //to be moved to player
        private void SendOpponents()
        {
            if (currentPlayer.Playroom.GameStartingTime == DateTime.MinValue)
            {
                currentPlayroomStartingTime = currentPlayer.Playroom.TrySetGameStartingTime();
            }

            string opponents = string.Empty;
            string rank = "!";
            opponents += currentPlayer.Playroom.Players.Aggregate(string.Empty, (localOpp, p) =>
            {
                if (!p.Name.Equals(currentClient))
                {
                    localOpp += (p.Name + ":" + p.WPMProgress + "&" + p.CompletedTextPercentage + "&" + p.PlayroomNumber + "%");
                }

                return localOpp;
            });

            rank += currentPlayer.Playroom.Rank.Aggregate(string.Empty, (localRank, r) => localRank += r.Key + ":" + r.Value.Item1 + "&" + r.Value.Item2 + ";");

            opponents += "*" + currentPlayroomStartingTime.ToString() + "+" + currentPlayer.Playroom.GameEndingTime.ToString() + "%" + rank + "%";

            byte[] broadcastBytes = Encoding.ASCII.GetBytes(opponents + "#");
            networkStream.Write(broadcastBytes, 0, broadcastBytes.Length);

            networkStream.Close();
            client.Close();
        }
    }
}