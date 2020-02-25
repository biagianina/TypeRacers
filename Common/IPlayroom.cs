using System;
using System.Collections.Generic;

namespace Common
{
    public interface IPlayroom<Player>
    {
        string CompetitionText { get; }

        List<Player> Players { get; set; }
        DateTime GameStartingTime { get; set; }
        DateTime GameEndingTime { get; set; }
        DateTime TimeToWaitForOpponents { get; set; }
        int Place { get; set; }
    }
}