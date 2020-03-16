using Common;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Threading;

namespace TypeRacers.Client
{
    public class ClientReceivedInformationManager : IRecievedInformationManager
    {
        public Player Player { get; set; }
        public IPlayroom Playroom { get; set; }
        private GameInfo GameInfo { get; }

        public ClientReceivedInformationManager(Player player, IPlayroom playroom)
        {
            Player = player;
            Playroom = playroom;
            GameInfo = (GameInfo)playroom;
        }

        public void StartCommunication()
        {
            while (!Player.Removed)
            {
                SetData();
            }
        }

        public void SetData()
        {
            var message = (ReceivedMessage)Player.Read();

            if(message is null)
            {
                GameInfo.ConnectionLost = true;
                return;
            }

            var data = message.GetData();
            if (Player.FirstTimeConnecting || Player.Restarting)
            {
                GameInfo.SetGameInfo(data);
                Player.FirstTimeConnecting = false;
                Player.Restarting = false;
            }
            else
            {
                SetGameStatus(data);
            }
        }

        private void SetGameStatus(string data)
        {
            var infos = data.Split('%').ToList();

            infos.Remove("#");
            foreach (var i in infos)
            {
                if (i.StartsWith("!"))
                {
                    var rank = i.Split('/');
                    Player.Finnished = Convert.ToBoolean(rank.FirstOrDefault().Substring(1));
                    Player.Place = int.Parse(rank.LastOrDefault());
                    infos.Remove(i);
                    break;
                }
            }
            GameInfo.SetOpponentsAndTimers(infos);
        }

        public void Write()
        {
            while (true)
            {
                Player.Write(new PlayerMessage(Player.WPMProgress, Player.CompletedTextPercentage, Player.Name, Player.FirstTimeConnecting, Player.Restarting, Player.Removed));
                GameInfo.OnOpponentsChanged(GameInfo.Players);
                Thread.Sleep(1000);
                if (Player.Removed)
                {
                    Player.Write(new PlayerMessage(Player.WPMProgress, Player.CompletedTextPercentage, Player.Name, Player.FirstTimeConnecting, Player.Restarting, Player.Removed));
                }
            }
        }
    }
}