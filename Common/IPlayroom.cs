using System;
using System.Collections.Generic;

namespace Common
{
    public interface IPlayroom<T> where T : IPlayer
    {
        string CompetitionText { get; }
        List<T> Players { get; set; }

        T GetPlayer(string name);

        DateTime GameStartingTime { get; set; }
        DateTime GameEndingTime { get; set; }
        DateTime TimeToWaitForOpponents { get; set; }
    }
}