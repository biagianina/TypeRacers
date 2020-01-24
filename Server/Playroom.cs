using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    class Playroom
    {

        public Playroom()
        {
            Players = new Dictionary<string, Tuple<string, string, int>>();

        }

        public Dictionary<string, Tuple<string, string, int>> Players { get; set; }

        public int PlayroomSize { get; set; }
        public int PlayroomNumber { get; set; }

        public bool ExistsInPlayroom(string currentClientKey)
        {
            return Players.ContainsKey(currentClientKey);
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
