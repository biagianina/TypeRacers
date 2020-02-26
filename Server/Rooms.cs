using System.Collections.Generic;
using System.Linq;
using System;
using Common;
using System.Threading;

namespace Server
{
    public class Rooms
    {
        private bool resendPlayroomInfo;
        private readonly List<IPlayroom> playrooms;
        private IPlayroom currentPlayroom;

        public Rooms()
        {
            playrooms = new List<IPlayroom>
            {
                new Playroom()
            };
            currentPlayroom = playrooms.Last();
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
                if (string.IsNullOrEmpty(dataRead))
                {
                    return;
                }
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
            player.FirstTimeConnecting = Convert.ToBoolean(infos[2]);
            player.UpdateInfo(int.Parse(infos[0]), int.Parse(infos[1]));

            if (currentPlayroom.CheckIfPlayerLeft(player))
            {
                return;
            }
            resendPlayroomInfo = currentPlayroom.CheckIfPlayerTriesToRestart(player);
            lock (currentPlayroom)
            {
                if (player.FirstTimeConnecting || resendPlayroomInfo)
                {
                    SendGameInfo(player);
                }
                else
                {
                    SendGamestatus(player);
                }

            }
        }

        private void SendGamestatus(Player player)
        {
            currentPlayroom = player.Playroom;
            currentPlayroom.TrySetGameStartingTime();
            player.TrySetRank();
            player.Write(currentPlayroom.GetGameStatus(player));
            Console.WriteLine("sending opponents");
        }

        private void SendGameInfo(Player player)
        {
            SetPlayroom(player);

            currentPlayroom = player.Playroom;
            currentPlayroom.TrySetGameStartingTime();
            player.Write(currentPlayroom.GameMessage());

            Console.WriteLine("sending game info");

            resendPlayroomInfo = false;
        }

        private void SetPlayroom(Player player)
        {
            if (!playrooms.Any(p => p.Join(player)))
            {
                CreateNewPlayroom();
                playrooms.Last().Join(player);
            }
            player.Playroom.TrySetGameStartingTime();
        }

        private void CreateNewPlayroom()
        {
            var newPlayroom = new Playroom();
            playrooms.Add(newPlayroom);
        }
    }
}