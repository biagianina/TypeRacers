using System.Collections.Generic;
using System.Linq;

namespace Server
{
    public class Rooms
    {
        readonly List<Playroom> playrooms;
        private int playroomsCount = 0;

        public Rooms()
        {
            playrooms = new List<Playroom>
            {
                new Playroom()
            };
        }

        public int GetNumberOfPlayrooms()
        {
            return playrooms.Count();
        }

        public void Join(Player player)
        {
            foreach (var playroom in playrooms)
            {
                if (!playroom.Join(player))
                {
                    CreateNewPlayroom();
                }
                else
                {
                    player.SetGameInfo();
                    break;
                }
            }
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
