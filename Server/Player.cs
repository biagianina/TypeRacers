using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace Server
{
    public class Player : IPlayer
    {
        private readonly TcpClient tcpClient;
        private readonly NetworkStream networkStream;
        private DateTime currentPlayroomStartingTime;

        public Player(TcpClient tcpClient)
        {
            Playroom = new Playroom();
            this.tcpClient = tcpClient;
            networkStream = tcpClient.GetStream();
            Read();
        }

        bool FirstTimeConnecting = true;
        private string DataReceived { get; set; }
        public string Name { get; set; }
        public int Place { get; set; }
        public bool Finnished { get; set; }
        public int WPMProgress { get; set; }
        public int CompletedTextPercentage { get; set; }
        public Playroom Playroom { get; set; }
        public Dictionary<string, Tuple<bool, int>> Rank { get; }

        public void StartListening()
        {

            while (true)
            {
                Thread thread = new Thread(() =>
                {
                    Read();
                    if (!string.IsNullOrEmpty(DataReceived))
                    {
                        Write();
                        DataReceived = string.Empty;
                    }
                });
                thread.Start();
            }
        }
        public void SetPlayroom(Playroom playroom)
        {
            Playroom = playroom;
            Write();
            FirstTimeConnecting = false;
            StartListening();
        }

        public void UpdateInfo(int wpmProgress, int completedText)
        {
            WPMProgress = wpmProgress;
            CompletedTextPercentage = completedText;
            if (wpmProgress == 100)
            {
                Finnished = true;
                Place = Playroom.Place++;
            }
        }

        internal void SendGameInfo()
        {
            byte[] broadcastBytes = Encoding.ASCII.GetBytes(Playroom.CompetitionText + "$" + '%' + Playroom.TimeToWaitForOpponents.ToString() + "*" + Playroom.GameStartingTime + "+" + Playroom.GameEndingTime + "#"); //generates random text from text document
            networkStream.Write(broadcastBytes, 0, broadcastBytes.Length);//send the text to connected client
        }

        public void CheckReceivedData(string dataReceived)
        {
            //progress and slider progress
            string progress = dataReceived.Substring(0, dataReceived.IndexOf('$'));

            var progressInfoAndPlayerRoomInfo = progress.Split('&');

            string username = dataReceived.Substring(dataReceived.IndexOf('$') + 1);

            Name = username.Substring(0, username.Length - 1);
            UpdateInfo(Convert.ToInt32(progressInfoAndPlayerRoomInfo[0]), Convert.ToInt32(progressInfoAndPlayerRoomInfo[1]));
            Console.WriteLine(dataReceived);
        }

        public void Read()
        {
            byte[] buffer = new byte[tcpClient.ReceiveBufferSize];
            int bytesRead = networkStream.Read(buffer, 0, buffer.Length);

            DataReceived = Encoding.ASCII.GetString(buffer, 0, bytesRead);

            //solution to get get complete messages
            while (!DataReceived.Last().Equals('#'))
            {
                bytesRead = networkStream.Read(buffer, 0, tcpClient.ReceiveBufferSize);
                DataReceived += Encoding.ASCII.GetString(buffer, DataReceived.Length, bytesRead);
            }

            CheckReceivedData(DataReceived);
        }

        public void Write()
        {
            if (FirstTimeConnecting)
            {
                SendGameInfo();
            }
            else
            {
                SendOpponents();
            }
        }

        public void SendOpponents()
        {
            if (Playroom.GameStartingTime == DateTime.MinValue)
            {
                currentPlayroomStartingTime = Playroom.TrySetGameStartingTime();
            }

            string opponents = string.Empty;
            string rank = "!";
            opponents += Playroom.Players.Aggregate(string.Empty, (localOpp, p) =>
            {
                if (!p.Name.Equals(Name))
                {
                    localOpp += (p.Name + ":" + p.WPMProgress + "&" + p.CompletedTextPercentage + "&0%");
                }

                return localOpp;
            });

            rank += Playroom.Players.Aggregate(string.Empty, (localRank, r) => localRank += r.Name + ":" + r.Finnished + "&" + r.Place + ";");

            opponents += "*" + currentPlayroomStartingTime.ToString() + "+" + Playroom.GameEndingTime.ToString() + "%" + rank + "%";

            byte[] broadcastBytes = Encoding.ASCII.GetBytes(opponents + "#");
            networkStream.Write(broadcastBytes, 0, broadcastBytes.Length);
        }

        //private void CheckReceivedData(string dataReceived)
        //{
        //if (CheckIfGameIsRestarted(dataReceived) || CheckIfClientLeftGame(dataReceived))
        //{
        //    return;
        //}

        //progress and slider progress
        //string progress = dataReceived.Substring(0, dataReceived.IndexOf('$'));

        //var progressInfoAndPlayerRoomInfo = progress.Split('&');

        //string username = dataReceived.Substring(dataReceived.IndexOf('$') + 1);

        //Name = username.Substring(0, username.Length - 1);

        //if (Playroom != null)
        //{
        //    newClient = Playroom.Join(this);
        //}

        //UpdateInfo(Convert.ToInt32(progressInfoAndPlayerRoomInfo[0]), Convert.ToInt32(progressInfoAndPlayerRoomInfo[1]));

        //if (Playroom != null)
        //{
        //    CheckIfClientLeftGame(Name);

        //    SetGameInfo();
        //}
        //Console.WriteLine(dataReceived);
        //}

        //private bool CheckIfClientLeftGame(string currentClient)
        //{
        //    if (currentClient.Contains("_removed"))
        //    {
        //        string toRemove = currentClient.Substring(0, currentClient.IndexOf('_'));
        //        Playroom.Leave(toRemove);
        //        return true;
        //    }

        //    return false;
        //}
        //private bool CheckIfGameIsRestarted(string dataReceived)
        //{
        //    if (dataReceived.Contains("_restart"))
        //    {
        //        Console.WriteLine("restart");
        //        byte[] broadcastBytes = Encoding.ASCII.GetBytes(Playroom.TimeToWaitForOpponents + "#");
        //        networkStream.Write(broadcastBytes, 0, broadcastBytes.Length);
        //        return true;
        //    }

        //    return false;
        //}
        //to be moved to player (as first connection -> see playroom method)
    }
}