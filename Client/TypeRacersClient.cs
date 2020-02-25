using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Timers;
using System.Windows.Forms;
using Common;
namespace TypeRacers.Client
{
    public class TypeRacersClient
    {
        public delegate void OpponentsChangedEventHandler(List<Player> updatedOpponents);

        public event OpponentsChangedEventHandler OpponentsChanged;

        public GameInfo gameInfo;

        public TypeRacersClient(Player player)
        {
            Player = player;
            gameInfo = new GameInfo();
            Player.SetPlayroom(gameInfo);
            StartServerCommunication();
        }

        public Player Player { get; set; }

        public void StartServerCommunication()
        {
            Thread writeThread = new Thread(Write);
            Thread readThread = new Thread(Read);
            writeThread.Start();
            readThread.Start();
        }

        private void Read()
        {
            while (true)
            {
                if (Player.FirstTimeConnecting)
                {
                    Player.Playroom.SetGameInfo(Player.Read());
                    Player.FirstTimeConnecting = false;
                }
                
                Player.Playroom.SetOpponentsAndTimers(Player.Read());

                Thread.Sleep(3000);
            }
        }
        private void Write()
        {
            while (true)
            {
                Player.Write(new PlayerMessage(Player.WPMProgress, Player.CompletedTextPercentage, Player.Name));
                OnOpponentsChanged(Player.Playroom.Players);
                Thread.Sleep(3000);
            }
        }

        public void NameClient(string username)
        {
            Player.Name = username;
        }
        protected void OnOpponentsChanged(List<Player> opponents)
        {
            if (opponents != null && OpponentsChanged != null)
            {
                OpponentsChanged(opponents);
            }
        }
        //public void RestartSearch()
        //{
        //    string toSend = Name + "_restart" + "#";

        //    var dataFromServer = SendDataToServer(toSend);
        //    var dataWithoutHashtag = dataFromServer.Remove(dataFromServer.Length - 1);
        //    WaitingTime = DateTime.Parse(dataWithoutHashtag);
        //}


        //public void RemovePlayerFromRoom()
        //{
        //    //connecting to server
        //    client = new TcpClient("localhost", 80);
        //    stream = client.GetStream();

        //    //writing the progress to stream

        //    string toSend = Name + "_removed" + "#";

        //    byte[] bytesToSend = Encoding.ASCII.GetBytes(toSend);
        //    stream.Write(bytesToSend, 0, bytesToSend.Length);
        //}
    }
}