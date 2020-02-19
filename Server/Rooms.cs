using System.Collections.Generic;
using System.Linq;
using Server;
using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using Common;

namespace Server
{
    public class Rooms
    {
        private readonly List<Playroom> playrooms;
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
            return playrooms.Count;
        }

        public void AllocatePlayroom(Player player)
        {
            if (!LastAvailablePlayroom.Join(player))
            {
                LastAvailablePlayroom = CreateNewPlayroom();
            }

            player.SetPlayroom(LastAvailablePlayroom);
        }

        public bool PlayerIsNew(Player player)
        {
            return !playrooms.Any(x => x.IsInPlayroom(player.Name));
        }

        private Playroom CreateNewPlayroom()
        {
            var newPlayroom = new Playroom();
            playrooms.Add(newPlayroom);
            return playrooms.Last();
        }
    }
}