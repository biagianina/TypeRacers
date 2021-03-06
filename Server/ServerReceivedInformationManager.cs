﻿using Common;
using System;

namespace Server
{
    public class ServerReceivedInformationManager : IRecievedInformationManager
    {
        public Player Player { get; set; }
        public IPlayroom Playroom { get; set; }

        public ServerReceivedInformationManager(Player player, IPlayroom playroom)
        {
            Player = player;
            Playroom = playroom;
        }

        public void StartCommunication()
        {
            while (true)
            {
                ManageReceivedData();
            }
        }

        public void ManageReceivedData()
        {
            var message = (ReceivedMessage)Player.Read();

            if (message is null)
            {
                return;
            }
            var data = message.GetData();

            Player.UpdateInfo(data);

            if (Player.CheckIfLeft())
            {
                return;
            }

            if (Player.FirstTimeConnecting || Player.CheckIfTriesToRestart())
            {
                SendGameInfo();
            }
            else
            {
                SendGamestatus();
            }
        }

        private void SendGamestatus()
        {
            Playroom.TrySetStartingTime();
            Player.TrySetRank();
            Player.Write(GetGameStatus());
            Console.WriteLine("sending opponents");
        }

        private IMessage GetGameStatus()
        {
            return new OpponentsMessage(Playroom.Players, Playroom.GameStartingTime, Playroom.GameEndingTime, Player.Name, Player.Finnished, Player.Place);
        }

        private void SendGameInfo()
        {
            Playroom.TrySetStartingTime();
            Player.Write(GameMessage());
            Console.WriteLine("sending game info");
        }

        private IMessage GameMessage()
        {
            return new GameMessage(Playroom.CompetitionText, Playroom.TimeToWaitForOpponents, Playroom.GameStartingTime, Playroom.GameEndingTime);
        }
    }
}