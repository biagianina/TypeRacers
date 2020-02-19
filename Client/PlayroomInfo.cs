using Common;
using System;
using System.Collections.Generic;

namespace Client
{
    public class PlayroomInfo : IPlayroom<PlayerInfo>
    {
        public string CompetitionText => throw new NotImplementedException();

        public List<PlayerInfo> Players { get; set; }
        public DateTime GameStartingTime { get ; set ; }
        public DateTime GameEndingTime { get; set; }
        public DateTime TimeToWaitForOpponents { get; set ; }
        public int Place { get; internal set; }

        public PlayerInfo GetPlayer(string name)
        {
            throw new NotImplementedException();
        }
    }
}