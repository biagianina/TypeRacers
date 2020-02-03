using Server;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace TypeRacers.Server
{
    class ServerSetup
    {
        private bool newClient;
        private TcpListener server;
        private NetworkStream networkStream;
        private string currentClient;
        private List<Playroom> playrooms;
        private Playroom currentPlayroom;
        int playroomCount = 0;
        int currentPlayerPlayroomNumber;
        private object currentPlayroomStartingTime;
        readonly int maxPlayroomSize = 3;
        TcpClient client;


        //to avoid generating different texts from users in same competition
        public static string CompetitionText { get; set; } = ServerGeneratedText.GetText();
        public void Setup()
        {
            server = new TcpListener(IPAddress.IPv6Any, 80);
            server.Server.SetSocketOption(SocketOptionLevel.IPv6, SocketOptionName.IPv6Only, false);
            playrooms = new List<Playroom>();

            try
            {
                server.Start();
            }
            catch (Exception)
            {
                throw new Exception("Server setup failed");
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
                while (!dataReceived[bytesRead - 1].Equals('#'))
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
            CheckIfGameIsRestarted(dataReceived);

            //progress and slider progress
            string progress = dataReceived.Substring(0, dataReceived.IndexOf('$'));

            var progressInfoAndPlayerRoomInfo = progress.Split('&');

            //1 progress, 2 sliderprogress, 3 current client playroom, 
            Tuple<string, string, int> clientInfo = new Tuple<string, string, int>(progressInfoAndPlayerRoomInfo[0],
                progressInfoAndPlayerRoomInfo[1], Convert.ToInt32(progressInfoAndPlayerRoomInfo[2]));

            string username = dataReceived.Substring(dataReceived.IndexOf('$') + 1);
            
            currentClient = username.Substring(0, username.Length - 1);

            if (currentClient == string.Empty)
            {
                return;
            }

            currentPlayerPlayroomNumber = clientInfo.Item3;

            CheckIfClientLeftGame(currentClient);

            CheckCurrentPlayroom(currentClient, currentPlayerPlayroomNumber, clientInfo);
        }

        private void CheckIfClientLeftGame(string currentClient)
        {
            if (currentClient.Contains("_removed"))
            {
                string toRemove = currentClient.Substring(0, currentClient.IndexOf('_'));
                currentPlayroom.RemovePlayer(toRemove);
                return;
            }
        }

        private void CheckIfGameIsRestarted(string dataReceived)
        {
            if (dataReceived.Contains("_restart"))
            {
                Console.WriteLine("restart");
                Console.WriteLine("sent new time: " + currentPlayroom.TimeToWaitForOpponents);
                byte[] broadcastBytes = Encoding.ASCII.GetBytes(currentPlayroom.TimeToWaitForOpponents + "#");
                networkStream.Write(broadcastBytes, 0, broadcastBytes.Length);
                networkStream.Close();
                client.Close();
                return;
            }
        }

        public void CheckCurrentPlayroom(string currrentClient, int roomNumber, Tuple<string, string, int> clientInfo)
        {
            if (!playrooms.Any())
            {
                playrooms.Add(new Playroom());
                currentPlayroom = playrooms.Last();
            }

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

            newClient = currentPlayroom.AddPlayersToRoom(currrentClient, clientInfo);

            CheckNewClient(currentPlayroom.PlayroomNumber);
        }
        private void CheckNewClient(int roomNumber)
        {
            Console.WriteLine("playroom size: " + currentPlayroom.PlayroomSize);

            if (newClient)
            {
                GetNewCompetitionText();
                
                byte[] broadcastBytes = Encoding.ASCII.GetBytes(CompetitionText + "$" + roomNumber + "%" + currentPlayroom.TimeToWaitForOpponents + "*" + currentPlayroomStartingTime + "#"); //generates random text from text document
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
            if (string.IsNullOrEmpty(currentPlayroom.GameStartingTime))
            {
                currentPlayroomStartingTime = currentPlayroom.CheckIfPlayersCanStart();
            }

            string opponents = string.Empty;
            string rank = "!";


            opponents += playrooms[currentPlayerPlayroomNumber].Players.Aggregate(string.Empty, (localOpp, p) =>
            {
                if (!p.Key.Equals(currentClient))
                {
                    localOpp += (p.Key + ":" + p.Value.Item1 + "&" + p.Value.Item2 + "&" + p.Value.Item3 + "/");
                }

                return localOpp;
            });

            rank += playrooms[currentPlayerPlayroomNumber].Rank.Aggregate(string.Empty, (localRank, r) => localRank += r.Key + ":" + r.Value.Item1 + "&" + r.Value.Item2 + ";");

            opponents += "*" + currentPlayroomStartingTime + "/" + rank + "/";

            byte[] broadcastBytes = Encoding.ASCII.GetBytes(opponents + "#");
            networkStream.Write(broadcastBytes, 0, broadcastBytes.Length);
           
            networkStream.Close();
            client.Close();
        }

        public Playroom CreateNewPlayroom()
        {
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
