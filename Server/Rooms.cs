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
            while (true)
            {
                var dataRead = player.Read();
                dataRead.Remove(dataRead.Length - 1);

                var nameAndInfo = dataRead.Split('$');
                var infos = nameAndInfo.FirstOrDefault()?.Split('&');
                player.Name = nameAndInfo.LastOrDefault();
                Console.WriteLine(dataRead);

                ManagePlayerReceivedData(player, infos);
            }
        }

        private void ManagePlayerReceivedData(Player player, string[] infos)
        {
            if (PlayerIsNew(player))
            {
                if (!LastAvailablePlayroom.Join(player))
                {
                    LastAvailablePlayroom = CreateNewPlayroom();
                }
                player.SetPlayroom(LastAvailablePlayroom);
                player.UpdateInfo(int.Parse(infos[0]), int.Parse(infos[1]));
                LastAvailablePlayroom.TrySetGameStartingTime();
                player.Write(new GameMessage(LastAvailablePlayroom.CompetitionText, LastAvailablePlayroom.TimeToWaitForOpponents, LastAvailablePlayroom.GameStartingTime, LastAvailablePlayroom.GameEndingTime));
                Console.WriteLine("sending game info");
            }

            else
            {
                LastAvailablePlayroom = (Playroom)player.Playroom;
                player.UpdateInfo(int.Parse(infos[0]), int.Parse(infos[1]));
                LastAvailablePlayroom.TrySetGameStartingTime();
                if (int.Parse(infos[1]) == 100 && !player.Finnished)
                {
                    player.Finnished = true;
                    player.Place = LastAvailablePlayroom.Place++;
                }
                var toSend = new OpponentsMessage(LastAvailablePlayroom.Players, LastAvailablePlayroom.GameStartingTime, LastAvailablePlayroom.GameEndingTime, player.Name, player.Finnished, player.Place);
                player.Write(toSend);
                Console.WriteLine("sending opponents");
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