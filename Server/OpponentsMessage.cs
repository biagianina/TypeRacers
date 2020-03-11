using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Server
{
    public class OpponentsMessage : IMessage
    {
        private List<Player> players;
        private DateTime startingTime;
        private DateTime endingTime;
        private string name;
        private bool finnished;
        private int place;

        public OpponentsMessage(List<Player> players, DateTime startingTime, DateTime endingTime, string name, bool finnished, int place)
        {
            this.players = players;
            this.startingTime = startingTime;
            this.endingTime = endingTime;
            this.name = name;
            this.place = place;
            this.finnished = finnished;
        }

        public byte[] ToByteArray()
        {
            return Encoding.ASCII.GetBytes(GeneratePlayersString());
        }

        private string GeneratePlayersString()
        {
            string opponents = string.Empty;
            opponents += players.Aggregate(string.Empty, (localOpp, p) =>
            {
                if (!p.Name.Equals(name))
                {
                    localOpp += (p.Name + ":" + p.WPMProgress + "&" + p.CompletedTextPercentage + "&" + p.Finnished + "&" + p.Place + "%");
                }

                return localOpp;
            });

            opponents += "*" + startingTime.ToString() + "+" + endingTime.ToString() + "%!" + finnished + "/" + place + "#";

            return opponents;
        }
    }
}