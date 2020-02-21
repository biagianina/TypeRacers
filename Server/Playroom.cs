using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using TypeRacers.Server;

namespace Server
{
    public class Playroom : IPlayroom<Player>
    {
        public Playroom()
        {
            Players = new List<Player>();
            TimeToWaitForOpponents = DateTime.UtcNow.AddSeconds(50);
        }

        public string CompetitionText { get; } = ServerGeneratedText.GetText();
        public bool GameHasStarted => GameStartingTime != DateTime.MinValue;
        public List<Player> Players { get; set; }
        private int PlayroomSize { get; set; }
        public DateTime GameStartingTime { get; set; }
        public DateTime GameEndingTime { get; set; }
        public DateTime TimeToWaitForOpponents { get; set; }
        public int Place { get; set; } = 1;
        string IPlayroom<Player>.CompetitionText { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

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
            return Players.Find(x => x.Name.Equals(name));
        }

        public void Leave(string playerName)
        {
            if (IsInPlayroom(playerName))
            {
                Players.Remove(Players.Find(x => x.Name.Equals(playerName)));
            }

            if (Players.Count == 0)
            {
                ResetPlayroom();
            }
        }

        public bool Join(Player currentPlayer)
        {
            if (GameHasStarted || Players.Count == 3)
            {
                return false;
            }

            if (!IsInPlayroom(currentPlayer.Name))
            {
                PlayroomSize++;
                Players.Add(currentPlayer);
                Console.WriteLine("adding player:" + currentPlayer.Name + ", playroom size: " + PlayroomSize);
            }
            return true;
        }

        private void ResetPlayroom()
        {
            TimeToWaitForOpponents = DateTime.UtcNow.AddSeconds(20);
        }

        public void SetGameInfo(string v)
        {
            throw new NotImplementedException();
        }

        public void SetOpponentsAndTimers(string v)
        {
            throw new NotImplementedException();
        }
    }
}