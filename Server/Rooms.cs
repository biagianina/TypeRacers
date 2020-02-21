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
            var dataRead = player.Read();
            dataRead.Remove(dataRead.Length - 1);
            var nameAndInfo = dataRead.Split('$');
            var infos = nameAndInfo.FirstOrDefault().Split('&');
            var name = nameAndInfo.LastOrDefault();

            player.Name = name;
            Console.WriteLine(dataRead);

            if (PlayerIsNew(player))
            {
                if (!LastAvailablePlayroom.Join(player))
                {
                    LastAvailablePlayroom = CreateNewPlayroom();
                }

                player.SetPlayroom(LastAvailablePlayroom);
                player.UpdateInfo(int.Parse(infos[0]), int.Parse(infos[1]), false, 0);
                player.Write(new Message("gameinfo", new object[] { LastAvailablePlayroom.CompetitionText, LastAvailablePlayroom.TimeToWaitForOpponents, LastAvailablePlayroom.GameStartingTime, LastAvailablePlayroom.GameEndingTime }));
                player.FirstTimeConnecting = false;
            }
            else
            {
                player.UpdateInfo(int.Parse(infos[0]), int.Parse(infos[1]), false, 0);
                player.Write(new Message("opponents", new object[] { LastAvailablePlayroom.Players, LastAvailablePlayroom.GameStartingTime, LastAvailablePlayroom.GameEndingTime }));
            }
           
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