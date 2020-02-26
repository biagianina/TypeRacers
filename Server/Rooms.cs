using System.Collections.Generic;
using System.Linq;
using System;
using Common;

namespace Server
{
    public class Rooms
    {
        private bool resendPlayroomInfo;
        private readonly List<IPlayroom<Player>> playrooms;

        public Rooms()
        {
            playrooms = new List<IPlayroom<Player>>
            {
                new Playroom()
            };
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
            if (player.Name.Contains("_restart"))
            {
                resendPlayroomInfo = true;
            }
            else if (player.Name.Contains("_removed"))
            {
                foreach (var current in playrooms)
                {
                    if (current.GetPlayer(player.Name) != null)
                    {
                        current.Players.Remove(current.GetPlayer(player.Name));
                        Console.WriteLine("REMOVED: " + player.Name);
                        Console.WriteLine("Playroom size: " + current.Players.Count());
                    }
                }
                return;
            }

            player.FirstTimeConnecting = Convert.ToBoolean(infos[2]);
            player.UpdateInfo(int.Parse(infos[0]), int.Parse(infos[1]));
            if (player.FirstTimeConnecting || resendPlayroomInfo)
            {
                SetPlayroom(player);
                player.Write(new GameMessage(player.Playroom.CompetitionText, player.Playroom.TimeToWaitForOpponents, player.Playroom.GameStartingTime, player.Playroom.GameEndingTime));
                Console.WriteLine("sending game info");

                if (resendPlayroomInfo)
                {
                    resendPlayroomInfo = false;
                }
            }
            else
            {
                player.Playroom.TrySetGameStartingTime();
                player.TrySetRank();
                player.Write(new OpponentsMessage(player.Playroom.Players, player.Playroom.GameStartingTime, player.Playroom.GameEndingTime, player.Name, player.Finnished, player.Place));
                Console.WriteLine("sending opponents");
            }
        }

        private void SetPlayroom(Player player)
        {
            if(!playrooms.Any(p => p.Join(player)))
            {
                CreateNewPlayroom();
                playrooms.Last().Join(player);
            }
            player.Playroom.TrySetGameStartingTime();
        }

        public bool PlayerIsNew(Player player)
        {
            return !playrooms.Any(x => x.IsInPlayroom(player.Name));
        }

        private void CreateNewPlayroom()
        {
            var newPlayroom = new Playroom();
            playrooms.Add(newPlayroom);
        }
    }
}