using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    public class Rooms
    {
        readonly List<Playroom> playrooms;
        private readonly int maxPlayroomSize = 3;
        private bool firstConnection;
        private int playroomsCount = 0;
        public Playroom LastAvailablePlayroom { get; set; }

        public Rooms()
        {
            playrooms = new List<Playroom>
            {
                new Playroom()
            };

            LastAvailablePlayroom = playrooms.Last();
        }

        public int GetNumberOfPlayrooms()
        {
            return playrooms.Count();
        }

        public void AllocatePlayroom(Player player)
        {
            if (LastAvailablePlayroom.GameHasStarted || LastAvailablePlayroom.Players.Count == maxPlayroomSize)
            {
                LastAvailablePlayroom = CreateNewPlayroom();
            }

            firstConnection = LastAvailablePlayroom.Join(player);

            player.Playroom = LastAvailablePlayroom;

            CheckIfIsFirstconnection(firstConnection, player);
        }

        private void CheckIfIsFirstconnection(bool newPlayer, Player player)
        {
            if (newPlayer)
            {
                player.SetGameInfo(); //first connection
            }
            else
            {
                player.UpdateOpponents();
            }
        }

        private Playroom CreateNewPlayroom()
        {
            var newPlayroom = new Playroom();
            newPlayroom.PlayroomNumber = playroomsCount++;
            playrooms.Add(newPlayroom);
            return playrooms.Last();
        }
    }
}
