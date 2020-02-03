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
        public int PlayroomSize { get; set; }
        public int PlayroomNumber { get; set; }
        public Dictionary<string, Tuple<string, string, int>> Players { get; set; }
        public Dictionary<string, Tuple<bool, int>> Rank { get; set; }
        public string GameStartingTime { get; set; } = string.Empty;
        public string TimeToWaitForOpponents { get; set; }
        private int Place { get; set; } = 1;

        DateTime currentTime;
        public Playroom()
        {
            Players = new Dictionary<string, Tuple<string, string, int>>();
            Rank = new Dictionary<string, Tuple<bool, int>>();
            currentTime = DateTime.UtcNow;
            TimeToWaitForOpponents = string.Format("{0:MM/dd/yy H:mm:ss tt}", currentTime.AddSeconds(10));
        }
       
        public string CheckIfPlayersCanStart()
        {
            if (PlayroomSize == 3)
            {
                currentTime = DateTime.UtcNow;
                currentTime = currentTime.AddSeconds(10);
                GameStartingTime = string.Format("{0:H:mm:ss tt}", currentTime);
            }
            if (DateTime.Parse(TimeToWaitForOpponents) - DateTime.UtcNow <= TimeSpan.Zero && PlayroomSize == 2)
            {            
                currentTime = DateTime.UtcNow;
                currentTime = currentTime.AddSeconds(10);
                GameStartingTime = string.Format("{0:H:mm:ss tt}", currentTime);
            }
              
            if ((PlayroomSize == 1 || PlayroomSize == 0) && DateTime.Parse(TimeToWaitForOpponents) - DateTime.UtcNow <= TimeSpan.Zero)
            {
                ResetPlayroom();
            }

            return GameStartingTime;
        }
       
        public bool ExistsInPlayroom(string currentClientKey)
        {
            return Players.ContainsKey(currentClientKey);
        }
        public void RemovePlayer(string clientKey)
        {
            if (ExistsInPlayroom(clientKey))
            {
                Players.Remove(clientKey);
            }
            PlayroomSize--;
        }
       
        public bool AddPlayersToRoom(string currentClientKey, Tuple<string, string, int> clientInfo)
        {         
            if (Players.ContainsKey(currentClientKey))
            {
                Players[currentClientKey] = clientInfo;
                if (Rank[currentClientKey].Item1 == false && clientInfo.Item2.Equals("100"))
                {
                    Rank[currentClientKey] = new Tuple<bool, int>(true, Place);
                    Place += 1;
                }
                return false;
            }

            Console.WriteLine("adding: " + currentClientKey);
            Players.Add(currentClientKey, clientInfo);
            Rank.Add(currentClientKey, new Tuple<bool, int>(false, 0));

            PlayroomSize++;

            return true;
        }

        private void ResetPlayroom()
        {
            currentTime = DateTime.UtcNow;
            TimeToWaitForOpponents = string.Format("{0:MM/dd/yy H:mm:ss tt}", currentTime.AddSeconds(10));
        }
    }
}
