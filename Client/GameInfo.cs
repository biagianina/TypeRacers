﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace TypeRacers.Client
{
    public class GameInfo : IPlayroom<Player>
    {
        public string CompetitionText { get; set; }

        public List<Player> Players { get; set ; }
        public DateTime GameStartingTime { get; set; }
        public DateTime GameEndingTime { get ; set; }
        public DateTime TimeToWaitForOpponents { get ; set ; }
        public int Place { get ; set ; }

        public Player GetPlayer(string name)
        {
            return Players.Find(p => p.Name.Equals(name));
        }

        public void SetGameInfo(string data)
        {
            CompetitionText = data.Substring(0, data.IndexOf('$'));

            var times = data.Substring(data.IndexOf('%') + 1);
            var gameTimers = times.Split('*');
            TimeToWaitForOpponents = DateTime.Parse(gameTimers.FirstOrDefault());
            var startAndEndTimes = gameTimers.LastOrDefault().Split('+');
            GameStartingTime = DateTime.Parse(startAndEndTimes.FirstOrDefault());
            GameEndingTime = DateTime.Parse(startAndEndTimes.LastOrDefault());
        }

        public void SetOpponentsAndTimers(string data)
        {
            var currentOpponents = data.Split('%').ToList();
            currentOpponents.Remove("#");

            foreach (var v in currentOpponents)
            {

                if (v.FirstOrDefault().Equals('*'))
                {
                    var times = v.Substring(1).Split('+');
                    GameStartingTime = DateTime.Parse(times.FirstOrDefault());
                    GameEndingTime = DateTime.Parse(times.LastOrDefault());
                }
                else
                {
                    var nameAndInfos = v.Split(':');
                    SetOpponents(nameAndInfos);
                }
            }

        }

        private void SetOpponents(string[] nameAndInfos)
        {
            var name = nameAndInfos.FirstOrDefault();
            var info = nameAndInfos.LastOrDefault().Split('&');
            var player = GetPlayer(name);

            if (player == default)
            {
                player.Name = name;
                player.UpdateInfo(int.Parse(info[0]), int.Parse(info[1]), false, 0);
                Players.Add(player);
            }
            else
            {
                player.UpdateInfo(int.Parse(info[0]), int.Parse(info[1]), false, 0);
            }
        }
    }
}