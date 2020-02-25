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

        public TypeRacersClient(Player player)
        {
            Player = player;
            Player.SetPlayroom(new GameInfo());
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
                if (Player.FirstTimeConnecting || Player.Restarting)
                {
                    Player.Playroom.SetGameInfo(Player.Read());
                    Player.FirstTimeConnecting = false;
                    if (Player.Restarting)
                    {
                        Player.Restarting = false;
                    }
                }

                Player.Playroom.SetOpponentsAndTimers(Player.Read());
                Thread.Sleep(1000);
            }
        }

        public void RestartSearch()
        {
            Player.Restarting = true;
        }
        private void Write()
        {
            while (true)
            {
                if (Player.Restarting)
                {
                    Player.Write(new PlayerMessage(Player.WPMProgress, Player.CompletedTextPercentage, Player.Name + "_restart"));
                }
                else
                {
                    Player.Write(new PlayerMessage(Player.WPMProgress, Player.CompletedTextPercentage, Player.Name));
                }
                OnOpponentsChanged(Player.Playroom.Players);
                Thread.Sleep(1000);
            }
        }

        protected void OnOpponentsChanged(List<Player> opponents)
        {
            if (opponents != null && OpponentsChanged != null && !Player.Restarting)
            {
                OpponentsChanged(opponents);
            }
        }


        public void NameClient(string username)
        {
            Player.Name = username;
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