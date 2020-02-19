using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Common
{
    public class Message
    {
        public string GenerateClientInfo(int wpmProgress, int completedTextPercentage, string name)
        {
            return wpmProgress.ToString() + "&" + completedTextPercentage + "$" + name + "#";
        }

        public string GenerateGameInfo(string competitionText, DateTime timeToWaitForOpponents, DateTime gameStartingTime, DateTime gameEndingTime)
        {
            return competitionText + "$" + '%' + timeToWaitForOpponents.ToString() + "*" + gameStartingTime + "+" + gameEndingTime + "#";
        }

        public string GenerateOpponentsString(List<Player> players, DateTime startingTime, DateTime endingTime, string name)
        {
            string opponents = string.Empty;
            string rank = "!";
            opponents += players.Aggregate(string.Empty, (localOpp, p) =>
            {
                if (!p.Name.Equals(name))
                {
                    localOpp += (p.Name + ":" + p.WPMProgress + "&" + p.CompletedTextPercentage + "&0%");
                }

                return localOpp;
            });

            rank += players.Aggregate(string.Empty, (localRank, r) => localRank += r.Name + ":" + r.Finnished + "&" + r.Place + ";");

            opponents += "*" + startingTime.ToString() + "+" + endingTime.ToString() + "%" + rank + "%";

            return opponents;
        }
    }
}
