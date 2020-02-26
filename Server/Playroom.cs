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
            TimeToWaitForOpponents = DateTime.UtcNow.AddSeconds(60);
        }

        public bool GameHasStarted => GameStartingTime != DateTime.MinValue;
        public List<Player> Players { get; set; }
        public DateTime GameStartingTime { get; set; }
        public DateTime GameEndingTime { get; set; }
        public DateTime TimeToWaitForOpponents { get; set; }
        public int Place { get; set; } = 1;
        public string CompetitionText { get; }

        public void TrySetGameStartingTime()
        {
            if (!GameHasStarted)
            {
                if (Players.Count == 3 || TimeToWaitForOpponents - DateTime.UtcNow.AddSeconds(2) <= TimeSpan.Zero && Players.Count == 2)
                {
                    GameStartingTime = DateTime.UtcNow.AddSeconds(10);
                    GameEndingTime = GameStartingTime.AddSeconds(90);
                }

                if ((Players.Count == 1) && TimeToWaitForOpponents - DateTime.UtcNow.AddSeconds(2) <= TimeSpan.Zero)
                {
                    ResetPlayroom();
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
                ResetPlayroom();
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
            }
            return true;
        }

        internal void CheckRanking(Player player, string completedTextPerecentage)
        {
            var currentPlayer = GetPlayer(player.Name);
            if (completedTextPerecentage.Equals(100))
            {
                currentPlayer.Finnished = true;
                currentPlayer.Place = Place++;
            }
        }

        private void ResetPlayroom()
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

        public void Remove(Player player)
        {
            Players.Remove(player);
        }

        public void CheckIfPlayerLeft(Player player)
        {
            if (player.Name.Contains("_removed"))
            {
                if (GetPlayer(player.Name) != null)
                {
                    Remove(GetPlayer(player.Name));
                    Console.WriteLine("REMOVED: " + player.Name);
                    Console.WriteLine("Playroom size: " + Players.Count());
                }
            }               
        }
        public bool CheckIfPlayerTriesToRestart(Player player)
        {
            if (player.Name.Contains("_restart"))
            {
                return true;
            }
            return false;
        }
    }
}