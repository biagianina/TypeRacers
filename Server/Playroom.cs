﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
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
            StartingTime = (string.Format("{0:hh:mm:ss tt}", currentTime.AddSeconds(30)));
        }

        public Dictionary<string, Tuple<string, string, int>> Players { get; set; }
        public string StartingTime { get; set; }
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
