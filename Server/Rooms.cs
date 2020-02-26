using System.Collections.Generic;
using System.Linq;
using System;
using Common;

namespace Server
{
    public class Rooms
    {
        private readonly List<IPlayroom<Player>> playrooms;
        public IPlayroom<Player> LastAvailablePlayroom { get; set; }

        public Rooms()
        {
            playrooms = new List<IPlayroom<Player>>
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
            lock (LastAvailablePlayroom)
            {
                if (PlayerIsNew(player))
                {
                    if (!playrooms.Any(p => p.Join(player)))
                    {
                        CreateNewPlayroom();
                        LastAvailablePlayroom.Join(player);
                    }
                    player.SetPlayroom(LastAvailablePlayroom);
                    player.UpdateInfo(int.Parse(infos[0]), int.Parse(infos[1]));
                    LastAvailablePlayroom.TrySetGameStartingTime();
                    player.Write(new GameMessage(LastAvailablePlayroom.CompetitionText, LastAvailablePlayroom.TimeToWaitForOpponents, LastAvailablePlayroom.GameStartingTime, LastAvailablePlayroom.GameEndingTime));
                    Console.WriteLine("sending game info");
                }

                else
                {                    
                    player.UpdateInfo(int.Parse(infos[0]), int.Parse(infos[1]));
                    player.Playroom.TrySetGameStartingTime();
                    if (int.Parse(infos[1]) == 100 && !player.Finnished)
                    {
                        player.Finnished = true;
                        player.Place = LastAvailablePlayroom.Place++;
                    }
                    var toSend = new OpponentsMessage(player.Playroom.Players, player.Playroom.GameStartingTime, player.Playroom.GameEndingTime, player.Name, player.Finnished, player.Place);
                    player.Write(toSend);
                    Console.WriteLine("sending opponents");
                }
            }
        }
        
        public bool PlayerIsNew(Player player)
        {
            return !playrooms.Any(x => x.IsInPlayroom(player.Name));
        }

        private void CreateNewPlayroom()
        {
            var newPlayroom = new Playroom();
            playrooms.Add(newPlayroom);
            LastAvailablePlayroom = playrooms.Last();
        }
    }
}