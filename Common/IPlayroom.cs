using System;
using System.Collections.Generic;

namespace Common
{
    public interface IPlayroom
    {
        string CompetitionText { get; set; }
        DateTime GameStartingTime { get; set; }
        DateTime GameEndingTime { get; set; }
        DateTime TimeToWaitForOpponents { get; set; }
        int Place { get; set; }
        bool Join(Player player);

        Player GetPlayer(string name);

        List<Player> Players { get; set; }
    }
}