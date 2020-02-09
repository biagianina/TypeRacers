using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace Server
{
    class Playroom
    {
        public bool GameHasStarted { get; set; }
        public int PlayroomSize { get; set; }
        public int PlayroomNumber { get; set; }
        public List<Player> Players { get; set; }
        public Dictionary<string, Tuple<bool, int>> Rank { get; set; }
        public string GameStartingTime { get; set; } = string.Empty;
        public string GameEndingTime { get; set; } = string.Empty;
        public string TimeToWaitForOpponents { get; set; }
        private int Place { get; set; } = 1;

        DateTime currentTime;
        public Playroom()
        {
            Players = new List<Player>();
            Rank = new Dictionary<string, Tuple<bool, int>>();
            currentTime = DateTime.UtcNow;
            TimeToWaitForOpponents = string.Format("{0:MM/dd/yy H:mm:ss tt}", currentTime.AddSeconds(15));
        }

        public bool CheckIfPlayersCanStart()
        {
            if (PlayroomSize == 3 || (DateTime.Parse(TimeToWaitForOpponents) - DateTime.UtcNow.AddSeconds(2) <= TimeSpan.Zero && PlayroomSize == 2))
            {
                currentTime = DateTime.UtcNow;
                currentTime = currentTime.AddSeconds(10);
                GameStartingTime = string.Format("{0:H:mm:ss tt}", currentTime);
                GameEndingTime = string.Format("{0:H:mm:ss tt}", currentTime.AddSeconds(90));
                GameHasStarted = true;
                return true;
            }

            if ((PlayroomSize == 1) && DateTime.Parse(TimeToWaitForOpponents) - DateTime.UtcNow.AddSeconds(2) <= TimeSpan.Zero)
            {
                ResetPlayroom();
            }

            return false;
        }

        public bool ExistsInPlayroom(string playerName)
        {
            return Players.Any(x => x.Name.Equals(playerName));
        }

        public Player GetPlayer(string name)
        {
            return Players.FirstOrDefault(x => x.Name.Equals(name));
        }
        public bool RemovePlayer(string playerName)
        {
            if (ExistsInPlayroom(playerName))
            {
                Players.Remove(Players.FirstOrDefault(x=>x.Name.Equals(playerName)));
                PlayroomSize--;
                Rank.Remove(playerName);
                Console.WriteLine("Player removed, current size: " + PlayroomSize);
                return true;
            }

            if (PlayroomSize == 0)
            {
                ResetPlayroom();
            }
            return false;
        }

        public bool AddPlayersToRoom(Player currentPlayer)
        {
            if (ExistsInPlayroom(currentPlayer.Name))
            {
                GetPlayer(currentPlayer.Name).UpdateInfo(currentPlayer.WPMProgress, currentPlayer.CompletedTextPercentage, currentPlayer.PlayroomNumber);
                if (Rank[currentPlayer.Name].Item1 == false && currentPlayer.CompletedTextPercentage.Equals("100"))
                {
                    Rank[currentPlayer.Name] = new Tuple<bool, int>(true, Place);
                    Place += 1;
                }
                return false;
            }

            Console.WriteLine("adding: " + currentPlayer.Name);

            Players.Add(currentPlayer);
            if (!Rank.ContainsKey(currentPlayer.Name))
            {
                Rank.Add(currentPlayer.Name, new Tuple<bool, int>(false, 0));

            }

            PlayroomSize++;

            return true;
        }

        private void ResetPlayroom()
        {
            currentTime = DateTime.UtcNow;
            TimeToWaitForOpponents = string.Format("{0:MM/dd/yy H:mm:ss tt}", currentTime.AddSeconds(20));
        }
    }
}
