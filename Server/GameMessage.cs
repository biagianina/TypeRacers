using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common;
namespace Server
{
    public class GameMessage : IMessage
    {
        string competitionText;
        DateTime timeToWaitForOpponents;
        DateTime gameStartingTime;
        DateTime gameEndingTime;
        public GameMessage(string competitionText, DateTime timeToWaitForOpponents, DateTime gameStartingTime, DateTime gameEndingTime)
        {
            this.competitionText = competitionText;
            this.timeToWaitForOpponents = timeToWaitForOpponents;
            this.gameStartingTime = gameStartingTime;
            this.gameEndingTime = gameEndingTime;
        }

        public byte[] ToByteArray()
        {
            return Encoding.ASCII.GetBytes(competitionText + "$" + '%' + timeToWaitForOpponents.ToString() + "*" + gameStartingTime.ToString() + "+" + gameEndingTime.ToString() + "#");
        }
    }
}
