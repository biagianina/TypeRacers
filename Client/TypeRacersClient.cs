using Common;
using System;
using System.Linq;
using System.Threading;

namespace TypeRacers.Client
{
    public class TypeRacersClient
    {
        public GameInfo gameInfo;

        public TypeRacersClient(Player player)
        {
            Player = player;
            gameInfo = new GameInfo();
            Player.SetPlayroom(gameInfo);
        }

        public Player Player { get; set; }

        public void StartCommunication()
        {
            Thread writeThread = new Thread(Write);
            Thread readThread = new Thread(Read);
            writeThread.Start();
            readThread.Start();
        }

        private void Read()
        {
            while (!Player.Removed)
            {
                var message = (ReceivedMessage)Player.Read();
                var data = message.GetData();
                if (Player.FirstTimeConnecting || Player.Restarting)
                {
                    gameInfo.SetGameInfo(data);
                    Player.FirstTimeConnecting = false;
                    Player.Restarting = false;
                }
                else
                {
                    SetGameStatus(data);
                }
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
            gameInfo.SetOpponentsAndTimers(infos);
        }

        private void Write()
        {
            while (true)
            {
                Player.Write(new PlayerMessage(Player.WPMProgress, Player.CompletedTextPercentage, Player.Name, Player.FirstTimeConnecting, Player.Restarting, Player.Removed));
                gameInfo.OnOpponentsChanged(gameInfo.Players);
                Thread.Sleep(1000);
                if (Player.Removed)
                {
                    Player.Write(new PlayerMessage(Player.WPMProgress, Player.CompletedTextPercentage, Player.Name, Player.FirstTimeConnecting, Player.Restarting, Player.Removed));
                    break;
                }
            }
        }
    }
}