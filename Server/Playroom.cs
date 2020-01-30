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
        DateTime currentTime;
        public Playroom()
        {
            Players = new Dictionary<string, Tuple<string, string, int>>();
            currentTime = DateTime.UtcNow;
            TimeToWaitForOpponents = string.Format("{0:hh:mm:ss tt}", currentTime.AddSeconds(20));
        }

        public Dictionary<string, Tuple<string, string, int>> Players { get; set; }

        public string GameStartingTime { get; set; } = string.Empty;
        public string TimeToWaitForOpponents { get; set; }

        public void CheckIfPlayersCanStart()
        {
            if (PlayroomSize == 3 || DateTime.Parse(TimeToWaitForOpponents) - DateTime.UtcNow <= TimeSpan.Zero && PlayroomSize == 2)
            {
                currentTime = DateTime.UtcNow;
                currentTime = currentTime.AddSeconds(15);
                GameStartingTime = string.Format("{0:hh:mm:ss tt}", currentTime);
            }
        }
        public int PlayroomSize { get; set; }
        public int PlayroomNumber { get; set; }
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
        }
        public bool AddPlayersToRoom(string currentClientKey, Tuple<string, string, int> clientInfo)
        {

            if (Players.ContainsKey(currentClientKey))
            {
                Players[currentClientKey] = clientInfo;
                return false;
            }

            Players.Add(currentClientKey, clientInfo);
            PlayroomSize++;

            return true;
        }
    }
}
