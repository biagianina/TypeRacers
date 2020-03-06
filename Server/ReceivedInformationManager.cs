using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common;

namespace Server
{
    public class ReceivedInformationManager : IRecievedInformationManager
    {
        public Player Player { get; set; }
        public IPlayroom Playroom { get; set; }

        public ReceivedInformationManager(Player player, IPlayroom playroom)
        {
            Player = player;
            Playroom = playroom;
        }
        public void StartCommunication()
        {
            while (true)
            {
                var message = (ReceivedMessage)Player.Read();
                var data = message?.GetData();
                if (string.IsNullOrEmpty(data))
                {
                    return;
                }

                ManageReceivedData(data);
            }
        }

        public void ManageReceivedData(string data)
        {
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
