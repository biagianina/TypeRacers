using System;
using System.Collections.Generic;
using System.Linq;
using TypeRacers.Server;
using TypeRacers;
using Common;
namespace Server
{
    public class Playroom : IPlayroom<Player>
    {
        public string CompetitionText { get; } = ServerGeneratedText.GetText();
        public bool GameHasStarted => GameStartingTime != DateTime.MinValue;
        public List<Player> Players { get; set; }


        public DateTime GameStartingTime { get; set; }
        public DateTime GameEndingTime { get; set; }
        public DateTime TimeToWaitForOpponents { get; set; }
        public int Place { get; set; } = 1;

        public Playroom()
        {
            Players = new List<Player>();
            TimeToWaitForOpponents = DateTime.UtcNow.AddSeconds(15);
        }

        public DateTime TrySetGameStartingTime()
        {
            if (Players.Count == 3 || TimeToWaitForOpponents - DateTime.UtcNow.AddSeconds(2) <= TimeSpan.Zero && Players.Count == 2)
            {
                GameStartingTime = DateTime.UtcNow.AddSeconds(10);
                GameEndingTime = GameStartingTime.AddSeconds(90);
            }

            if ((Players.Count == 1) && TimeToWaitForOpponents - DateTime.UtcNow.AddSeconds(2) <= TimeSpan.Zero)
            {
                ResetPlayroom();
            }

            return GameStartingTime;
        }

        public bool IsInPlayroom(string playerName)
        {
            return Players.Any(x => x.Name.Equals(playerName));
        }

        public Player GetPlayer(string name)
        {
            return Players.FirstOrDefault(x => x.Name.Equals(name));
        }

        public void Leave(string playerName)
        {
            if (IsInPlayroom(playerName))
            {
                Players.Remove(Players.FirstOrDefault(x => x.Name.Equals(playerName)));
            }

            if (Players.Count == 0)
            {
                ResetPlayroom();
            }
        }

        public bool Join(Player currentPlayer)
        {
            if (GameHasStarted || Players.Count == 3 )
            {
                return false;
            }

            if (!IsInPlayroom(currentPlayer.Name))
            {
                Players.Add(currentPlayer);

            }

            Console.WriteLine("adding player:" + currentPlayer.Name);
            currentPlayer.Playroom = this;

            return true;
        }

        private void ResetPlayroom()
        {
            TimeToWaitForOpponents = DateTime.UtcNow.AddSeconds(20);
        }
    }
}