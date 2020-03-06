using System;
using System.Linq;
using System.Threading;

namespace Common
{
    public class Player
    {
        public Player(INetworkClient tcpClient)
        {
            NetworkClient = tcpClient;
        }

        public bool FirstTimeConnecting { get; set; } = true;
        public string Name { get; set; }
        public int Place { get; set; }
        public bool Restarting { get; set; }
        public bool Removed { get; set; }
        public bool Finnished { get; set; }
        public int WPMProgress { get; set; }
        public int CompletedTextPercentage { get; set; }
        public IPlayroom Playroom { get; set; }
        public INetworkClient NetworkClient { get; }

        public void SetPlayroom(IPlayroom playroom)
        {
            Playroom = playroom;
        }

        public void UpdateProgress(int wpmProgress, int completedText)
        {
            WPMProgress = wpmProgress;
            CompletedTextPercentage = completedText;
        }

        public void TrySetRank()
        {
            if (CompletedTextPercentage == 100 && !Finnished)
            {
                Finnished = true;
                Place = Playroom.Place++;
            }
        }

        public IMessage Read()
        {
            return NetworkClient.Read();
        }

        public void Write(IMessage message)
        {
            NetworkClient.Write(message);
        }
     
        public void StartCommunication()
        {
            while (true)
            {
                var message = (ReceivedMessage)Read();
                var data = message?.GetData();
                if (string.IsNullOrEmpty(data))
                {
                    return;
                }

                var nameAndInfo = data.Split('$');
                var infos = nameAndInfo.FirstOrDefault()?.Split('&');
                Name = nameAndInfo.LastOrDefault();

                Console.WriteLine(data);

                ManagePlayerReceivedData(infos);
            }
        }
        private void ManagePlayerReceivedData(string[] infos)
        {
            FirstTimeConnecting = Convert.ToBoolean(infos[2]);
            UpdateProgress(int.Parse(infos[0]), int.Parse(infos[1]));
            if (CheckIfLeft())
            {
                return;
            }

            if (FirstTimeConnecting || CheckIfTriesToRestart())
            {
                SendGameInfo();
            }
            else
            {
                SendGamestatus();
            }
        }

        private void SendGamestatus()
        {
            Playroom.TrySetStartingTime();
            TrySetRank();
            Write(GetGameStatus());
            Console.WriteLine("sending opponents");
        }

        private IMessage GetGameStatus()
        {
            return new OpponentsMessage(Playroom.Players, Playroom.GameStartingTime, Playroom.GameEndingTime, Name, Finnished, Place);
        }

        private void SendGameInfo()
        {
            Playroom.TrySetStartingTime();
            Write(GameMessage());
            Console.WriteLine("sending game info");
        }

        private IMessage GameMessage()
        {
            return new GameMessage(Playroom.CompetitionText, Playroom.TimeToWaitForOpponents, Playroom.GameStartingTime, Playroom.GameEndingTime);
        }

        private bool CheckIfTriesToRestart()
        {
            return Name.Contains("_restart");
        }

        private bool CheckIfLeft()
        {
            if (Name.Contains("_removed") && Name != null)
            {
                Playroom.Leave(Name);
                NetworkClient.Dispose();
                return true;
            }

            return false;
        }
    }
}