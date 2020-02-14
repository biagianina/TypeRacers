using System.Collections.Generic;
using System.Linq;

namespace Server
{
    public class Rooms
    {
        readonly List<Playroom> playrooms;

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

        public void AllocatePlayroom(Player player)
        {
            foreach (var playroom in playrooms)
            {
                if (!playroom.Join(player))
                {
                    CreateNewPlayroom();
                }
            }
        }

        private Playroom CreateNewPlayroom()
        {
            var newPlayroom = new Playroom();
            playrooms.Add(newPlayroom);
            return playrooms.Last();
        }
    }
}
