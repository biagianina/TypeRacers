using Common;
using System;
using System.Collections.Generic;

namespace TypeRacers.Client
{
    public class GameInfo : IPlayroom<Player>
    {
        public string CompetitionText => throw new NotImplementedException();

        public List<Player> Players { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public DateTime GameStartingTime { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public DateTime GameEndingTime { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public DateTime TimeToWaitForOpponents { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public int Place { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public Player GetPlayer(string name)
        {
            throw new NotImplementedException();
        }
    }
}