using System;
using System.Collections.Generic;
using System.Linq;
using TypeRacers.Server;

namespace Server
{
    public class Playroom
    {
        public static string CompetitionText => ServerGeneratedText.GetText();
        public bool GameHasStarted => GameStartingTime != DateTime.MinValue;
        public List<Player> Players { get; set; }
        public int PlayroomNumber { get; set; }

        public Dictionary<string, Tuple<bool, int>> Rank { get; set; }
        public DateTime GameStartingTime { get; set; }
        public DateTime GameEndingTime { get; set; }
        public DateTime TimeToWaitForOpponents { get; set; }
        private int Place { get; set; } = 1;

        public Playroom()
        {
            Players = new List<Player>();
            Rank = new Dictionary<string, Tuple<bool, int>>();
            TimeToWaitForOpponents = DateTime.UtcNow.AddSeconds(15);
        }

        public DateTime TrySetGameStartingTime()
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

            return GameStartingTime;
        }

        public bool IsInPlayroom(string playerName)
        {
            return Players.Any(x => x.Name.Equals(playerName));
        }

        public Player GetPlayer(string name)
        {
            return Players.FirstOrDefault(x => x.Name.Equals(name));
        }

        public void Leave(string playerName)
        {
            if (IsInPlayroom(playerName))
            {
                Players.Remove(Players.FirstOrDefault(x => x.Name.Equals(playerName)));
                Rank.Remove(playerName);
            }

            if (Players.Count == 0)
            {
                ResetPlayroom();
            }
        }

        public bool Join(Player currentPlayer)
        {
            if (IsInPlayroom(currentPlayer.Name))
            {
                GetPlayer(currentPlayer.Name).UpdateInfo(currentPlayer.WPMProgress, currentPlayer.CompletedTextPercentage);

                if (!Rank[currentPlayer.Name].Item1 && currentPlayer.CompletedTextPercentage.Equals("100"))
                {
                    Rank[currentPlayer.Name] = new Tuple<bool, int>(true, Place);
                    Place++;
                }
                return false;
            }

            Console.WriteLine("adding: " + currentPlayer.Name + " room number: " + PlayroomNumber);

            Players.Add(currentPlayer);

            if (!Rank.ContainsKey(currentPlayer.Name))
            {
                Rank.Add(currentPlayer.Name, new Tuple<bool, int>(false, 0));
            }

            currentPlayer.Playroom = this;

            return true;
        }

        private void ResetPlayroom()
        {
            TimeToWaitForOpponents = DateTime.UtcNow.AddSeconds(20);
        }
    }
}