using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;

namespace Server
{
    public class Player
    {
        TcpClient tcpClient;
        NetworkStream networkStream;
        private bool newClient;
        DateTime currentPlayroomStartingTime;
        public Player(TcpClient tcpClient)
        {
            Playroom = new Playroom();
            this.tcpClient = tcpClient;
            networkStream = tcpClient.GetStream();
        }

        public string Name { get; set; }
        public int PlayroomNumber { get; set; }
        public int WPMProgress { get; set; }
        public int CompletedTextPercentage { get; set; }
        public Playroom Playroom { get; set; }

        public void UpdateInfo(int wpmProgress, int completedText, int roomNumber)
        {
            WPMProgress = wpmProgress;
            CompletedTextPercentage = completedText;
            PlayroomNumber = roomNumber;
        }

        private void CheckReceivedData(string dataReceived)
        {
            if (CheckIfGameIsRestarted(dataReceived) || CheckIfClientLeftGame(dataReceived))
            {
                return;
            }

            //progress and slider progress
            string progress = dataReceived.Substring(0, dataReceived.IndexOf('$'));

            var progressInfoAndPlayerRoomInfo = progress.Split('&');

            string username = dataReceived.Substring(dataReceived.IndexOf('$') + 1);

            Name = username.Substring(0, username.Length - 1);

            if (Playroom != null)
            {
                newClient = Playroom.Join(this);
            }

            UpdateInfo(Convert.ToInt32(progressInfoAndPlayerRoomInfo[0]), Convert.ToInt32(progressInfoAndPlayerRoomInfo[1]), Convert.ToInt32(progressInfoAndPlayerRoomInfo[2]));

            if (Playroom != null)
            {
                CheckIfClientLeftGame(Name);

                SetGameInfo();
            }

        }

        private bool CheckIfClientLeftGame(string currentClient)
        {
            if (currentClient.Contains("_removed"))
            {
                string toRemove = currentClient.Substring(0, currentClient.IndexOf('_'));
                Playroom.Leave(toRemove);
                return true;
            }

            return false;
        }
        private bool CheckIfGameIsRestarted(string dataReceived)
        {
            if (dataReceived.Contains("_restart"))
            {
                Console.WriteLine("restart");
                byte[] broadcastBytes = Encoding.ASCII.GetBytes(Playroom.TimeToWaitForOpponents + "#");
                networkStream.Write(broadcastBytes, 0, broadcastBytes.Length);
                return true;
            }

            return false;
        }
        //to be moved to player (as first connection -> see playroom method)
        internal void SetGameInfo()
        {
            if (newClient) //player.FirstConnection() method
            {
                byte[] broadcastBytes = Encoding.ASCII.GetBytes(Playroom.CompetitionText + "$" + "%" + Playroom.TimeToWaitForOpponents.ToString() + "*" + Playroom.GameStartingTime + "+" + Playroom.GameEndingTime + "#"); //generates random text from text document
                networkStream.Write(broadcastBytes, 0, broadcastBytes.Length);//send the text to connected client
            }
            else
            {
                UpdateOpponents();//player.UpdateOpponents() method
            }
        }
        internal void Read()
        {
            byte[] buffer = new byte[tcpClient.ReceiveBufferSize];
            int bytesRead = networkStream.Read(buffer, 0, buffer.Length);

            string dataReceived = Encoding.ASCII.GetString(buffer, 0, bytesRead);

            //solution to get get complete messages
            while (!dataReceived.Last().Equals('#'))
            {
                bytesRead = networkStream.Read(buffer, 0, tcpClient.ReceiveBufferSize);
                dataReceived += Encoding.ASCII.GetString(buffer, dataReceived.Length, bytesRead);
            }

            CheckReceivedData(dataReceived);
        }


        internal void UpdateOpponents()
        {

            if (Playroom.GameStartingTime == DateTime.MinValue)
            {
                currentPlayroomStartingTime = Playroom.TrySetGameStartingTime();
            }

            string opponents = string.Empty;
            string rank = "!";
            opponents += Playroom.Players.Aggregate(string.Empty, (localOpp, p) =>
            {
                if (!p.Name.Equals(this.Name))
                {
                    localOpp += (p.Name + ":" + p.WPMProgress + "&" + p.CompletedTextPercentage + "&" + p.PlayroomNumber + "%");
                }

                return localOpp;
            });

            rank += Playroom.Rank.Aggregate(string.Empty, (localRank, r) => localRank += r.Key + ":" + r.Value.Item1 + "&" + r.Value.Item2 + ";");

            opponents += "*" + currentPlayroomStartingTime.ToString() + "+" + Playroom.GameEndingTime.ToString() + "%" + rank + "%";

            byte[] broadcastBytes = Encoding.ASCII.GetBytes(opponents + "#");
            networkStream.Write(broadcastBytes, 0, broadcastBytes.Length);

            networkStream.Close();
            tcpClient.Close();


        }
    }
}
