using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using TypeRacers.Server;

namespace Server
{
    public class Playroom : IPlayroom
    {
        public Playroom()
        {
            Players = new List<Player>();
            CompetitionText = ServerGeneratedText.GetText();
            TimeToWaitForOpponents = DateTime.UtcNow.AddSeconds(20);
        }

        public bool GameHasStarted => GameStartingTime != DateTime.MinValue;
        public List<Player> Players { get; set; }
        public DateTime GameStartingTime { get; set; }
        public DateTime GameEndingTime { get; set; }
        public DateTime TimeToWaitForOpponents { get; set; }
        public int Place { get; set; } = 1;
        public string CompetitionText { get; set; }

        public void TrySetGameStartingTime()
        {
            if (!GameHasStarted)
            {
                if (Players.Count == 3 || (TimeToWaitForOpponents - DateTime.UtcNow.AddSeconds(2) <= TimeSpan.Zero && Players.Count == 2))
                {
                    GameStartingTime = DateTime.UtcNow.AddSeconds(10);
                    GameEndingTime = GameStartingTime.AddSeconds(90);
                }

                if ((Players.Count == 1) && TimeToWaitForOpponents - DateTime.UtcNow.AddSeconds(2) <= TimeSpan.Zero)
                {
                    Reset();
                }
            }
        }

        public bool IsInPlayroom(string playerName)
        {
            return Players.Any(x => x.Name.Equals(playerName));
        }

        public Player GetPlayer(string name)
        {
            return Players.Find(x => x.Name.Equals(name));
        }

        public void Leave(string playerName)
        {
            if (IsInPlayroom(playerName))
            {
                Players.Remove(Players.Find(x => x.Name.Equals(playerName)));
            }

            if (Players.Count == 0)
            {
                Reset();
            }
        }

        public bool Join(Player currentPlayer)
        {
            if (GameHasStarted || Players.Count == 3)
            {
                return false;
            }

            if (!IsInPlayroom(currentPlayer.Name))
            {
                Players.Add(currentPlayer);
                currentPlayer.Playroom = this;
                Console.WriteLine("adding player:" + currentPlayer.Name + ", playroom size: " + Players.Count);
                StartCommunication(currentPlayer);
                return true;
            }
            return false;
        }

        private void StartCommunication(Player currentPlayer)
        {
            while (true)
            {
                var message = (ReceivedMessage)currentPlayer.Read();
                var data = message?.GetData();
                if (string.IsNullOrEmpty(data))
                {
                    return;
                }

                var nameAndInfo = data.Split('$');
                var infos = nameAndInfo.FirstOrDefault()?.Split('&');
                currentPlayer.Name = nameAndInfo.LastOrDefault();

                Console.WriteLine(data);

                ManagePlayerReceivedData(currentPlayer, infos);
            }
        }
        private void ManagePlayerReceivedData(Player player, string[] infos)
        {
            player.FirstTimeConnecting = Convert.ToBoolean(infos[2]);
            player.UpdateInfo(int.Parse(infos[0]), int.Parse(infos[1]));
            if (CheckIfPlayerLeft(player))
            {
                return;
            }

            if (player.FirstTimeConnecting || CheckIfPlayerTriesToRestart(player))
            {
                SendGameInfo(player);
            }
            else
            {
                SendGamestatus(player);
            }

        }

        private void SendGamestatus(Player player)
        {
            player.TrySetRank();
            player.Write(GetGameStatus(player));
            Console.WriteLine("sending opponents");
        }

        private void SendGameInfo(Player player)
        {
            TrySetGameStartingTime();
            player.Write(GameMessage());
            Console.WriteLine("sending game info");
        }

        private void Reset()
        {
            TimeToWaitForOpponents = DateTime.UtcNow.AddSeconds(20);
        }

        public IMessage GameMessage()
        {
            return new GameMessage(CompetitionText, TimeToWaitForOpponents, GameStartingTime, GameEndingTime);
        }

        public IMessage GetGameStatus(Player player)
        {
            return new OpponentsMessage(Players, GameStartingTime, GameEndingTime, player.Name, player.Finnished, player.Place);
        }

        public bool CheckIfPlayerLeft(Player player)
        {
            if (player.Name.Contains("_removed") && GetPlayer(player.Name) != null)
            {
                Leave(player.Name);
                Console.WriteLine("REMOVED: " + player.Name);
                Console.WriteLine("Playroom size: " + Players.Count);
                player.NetworkClient.Dispose();
                return true;
            }

            return false;
        }

        public bool CheckIfPlayerTriesToRestart(Player player)
        {
            return player.Name.Contains("_restart");
        }
    }
}