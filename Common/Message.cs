using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Common
{
    public class Message
    {
        public Message(string messageType, params object[] messageInfo)
        {
            GenerateMessage(messageType, messageInfo);
        }
        public string GetMessage { get; private set; } = string.Empty;

        private void GenerateMessage(string messageType, params object[] messageInfo)
        {
            switch (messageType)
            {
                case "clientinfo":

                    if(messageInfo.Length == 3)
                    {
                        GenerateClientInfo((string)messageInfo[0], (string)messageInfo[1], (string)messageInfo[2]);
                    }
                    break;

                case "gameinfo":

                    if (messageInfo.Length == 4)
                    {
                        GenerateGameInfo((string)messageInfo[0], (DateTime)messageInfo[1], (DateTime)messageInfo[2], (DateTime)messageInfo[3]);
                    }
                    break;

                case "opponents":

                    if (messageInfo.Length == 4)
                    {
                        GenerateOpponentsString((List<Player>)messageInfo[0], (DateTime)messageInfo[1], (DateTime)messageInfo[2], (string)messageInfo[3]);
                    }
                    break;
            }
        }
        public void GenerateClientInfo(string wpmProgress, string completedTextPercentage, string name)
        {
            GetMessage = wpmProgress + "&" + completedTextPercentage + "$" + name + "#";
        }

        public void GenerateGameInfo(string competitionText, DateTime timeToWaitForOpponents, DateTime gameStartingTime, DateTime gameEndingTime)
        {
            GetMessage = competitionText + "$" + '%' + timeToWaitForOpponents.ToString() + "*" + gameStartingTime.ToString() + "+" + gameEndingTime.ToString() + "#";
        }

        public void GenerateOpponentsString(List<Player> players, DateTime startingTime, DateTime endingTime, string name)
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

            GetMessage = opponents;
        }
    }
}
