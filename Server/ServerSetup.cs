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
        private static string CompetitionText { get; set; } = ServerGeneratedText.GetText();
        private bool newClient;
        private TcpListener server;
        private NetworkStream networkStream;
        private string currentClient;
        private List<Playroom> playrooms;
        private Playroom currentPlayroom;
        private int playroomCount = 0;
        private int currentPlayerPlayroomNumber;
        private DateTime currentPlayroomStartingTime;
        private readonly int maxPlayroomSize = 3;
        private TcpClient client;
        private Player currentPlayer;

        public void Setup()
        {
            server = new TcpListener(IPAddress.IPv6Any, 80);
            server.Server.SetSocketOption(SocketOptionLevel.IPv6, SocketOptionName.IPv6Only, false);
            playrooms = new List<Playroom>();

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
            currentPlayer.UpdateInfo(Convert.ToInt32(progressInfoAndPlayerRoomInfo[0]), Convert.ToInt32(progressInfoAndPlayerRoomInfo[1]), Convert.ToInt32(progressInfoAndPlayerRoomInfo[2]));

            currentPlayerPlayroomNumber = currentPlayer.PlayroomNumber;

            CheckIfClientLeftGame(currentClient);

            CheckCurrentPlayroom(currentClient, currentPlayerPlayroomNumber);
        }

        private bool CheckIfClientLeftGame(string currentClient)
        {
            if (currentClient.Contains("_removed"))
            {
                string toRemove = currentClient.Substring(0, currentClient.IndexOf('_'));
                currentPlayroom.RemovePlayer(toRemove);
                return true;
            }

            return false;
        }

        private bool CheckIfGameIsRestarted(string dataReceived)
        {
            if (dataReceived.Contains("_restart"))
            {
                Console.WriteLine("restart");
                Console.WriteLine("sent new time: " + currentPlayroom.TimeToWaitForOpponents);
                byte[] broadcastBytes = Encoding.ASCII.GetBytes(currentPlayroom.TimeToWaitForOpponents + "#");
                networkStream.Write(broadcastBytes, 0, broadcastBytes.Length);
                networkStream.Close();
                client.Close();
                return true;
            }

            return false;
        }

        private void CheckCurrentPlayroom(string currrentClient, int roomNumber)
        {
            if (!CheckIfClientLeftGame(currrentClient))
            {
                if (!playrooms.Any())
                {
                    playrooms.Add(new Playroom());
                    currentPlayroom = playrooms.Last();
                }

                if (roomNumber == -1)
                {
                    if (playrooms.Last().GameHasStarted || playrooms.Last().PlayroomSize == maxPlayroomSize)
                    {
                        currentPlayroom = CreateNewPlayroom();
                    }
                    else
                    {
                        currentPlayroom = playrooms.Last();
                    }
                }
                else
                {
                    currentPlayroom = playrooms[roomNumber];

                    if (currentPlayroom.PlayroomSize == maxPlayroomSize && !currentPlayroom.ExistsInPlayroom(currentClient))
                    {
                        if (playrooms.Last().PlayroomSize == maxPlayroomSize)
                        {
                            currentPlayroom = CreateNewPlayroom();
                        }
                        else
                        {
                            currentPlayroom = playrooms.Last();
                        }
                    }
                }

                newClient = currentPlayroom.AddPlayersToRoom(currentPlayer);

                CheckNewClient(currentPlayroom.PlayroomNumber);
            }
        }

        private void CheckNewClient(int roomNumber)
        {
            Console.WriteLine("playroom size: " + currentPlayroom.PlayroomSize);

            if (newClient)
            {
                byte[] broadcastBytes = Encoding.ASCII.GetBytes(CompetitionText + "$" + roomNumber + "%" + currentPlayroom.TimeToWaitForOpponents.ToString() + "*" + currentPlayroom.GameStartingTime + "+" + currentPlayroom.GameEndingTime + "#"); //generates random text from text document
                networkStream.Write(broadcastBytes, 0, broadcastBytes.Length);//send the text to connected client

                networkStream.Close();
                client.Close();
            }
            else
            {
                SendOpponents();
            }
        }

        private void SendOpponents()
        {
            if (currentPlayroom.GameStartingTime == DateTime.MinValue)
            {
                currentPlayroomStartingTime = currentPlayroom.TrySetGameStartingTime();
            }

            string opponents = string.Empty;
            string rank = "!";
            opponents += playrooms[currentPlayerPlayroomNumber].Players.Aggregate(string.Empty, (localOpp, p) =>
            {
                if (!p.Name.Equals(currentClient))
                {
                    localOpp += (p.Name + ":" + p.WPMProgress + "&" + p.CompletedTextPercentage + "&" + p.PlayroomNumber + "%");
                }

                return localOpp;
            });

            rank += playrooms[currentPlayerPlayroomNumber].Rank.Aggregate(string.Empty, (localRank, r) => localRank += r.Key + ":" + r.Value.Item1 + "&" + r.Value.Item2 + ";");

            opponents += "*" + currentPlayroomStartingTime.ToString() + "+" + currentPlayroom.GameEndingTime.ToString() + "%" + rank + "%";

            byte[] broadcastBytes = Encoding.ASCII.GetBytes(opponents + "#");
            networkStream.Write(broadcastBytes, 0, broadcastBytes.Length);

            networkStream.Close();
            client.Close();
        }

        private Playroom CreateNewPlayroom()
        {
            GetNewCompetitionText();
            playroomCount++;
            var newPlayroom = new Playroom
            {
                PlayroomNumber = playroomCount
            };
            playrooms.Add(newPlayroom);
            return playrooms.Last();
        }

        private void GetNewCompetitionText()
        {
            CompetitionText = ServerGeneratedText.GetText();
        }
    }
}