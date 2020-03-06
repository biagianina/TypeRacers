using Common;
using System;
using System.Text;

namespace Common
{
    public class GameMessage : IMessage
    {
        private string competitionText;
        private DateTime timeToWaitForOpponents;
        private DateTime gameStartingTime;
        private DateTime gameEndingTime;

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