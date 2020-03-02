﻿using Common;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Server
{
    public class Rooms
    {
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
                var message = (ReceivedMessage)player.Read();
                var data = message?.GetData();
                if (string.IsNullOrEmpty(data))
                {
                    return;
                }

                var nameAndInfo = data.Split('$');
                var infos = nameAndInfo.FirstOrDefault()?.Split('&');
                player.Name = nameAndInfo.LastOrDefault();

                Console.WriteLine(data);

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

            lock (currentPlayroom)
            {
                if (player.FirstTimeConnecting || currentPlayroom.CheckIfPlayerTriesToRestart(player))
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