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
        private TcpClient client;
        private NetworkStream stream;
        private Player player;

        public TypeRacersClient()
        { 
            client = new TcpClient("localhost", 80);
            stream = client.GetStream();
            player = new Player(client);
            player.SetPlayroom(new GameInfo());
        }

        public void StartServerCommunication()
        {
            while (true)
            {
                try
                {
                    Thread writeThread = new Thread(() =>
                    {
                        Thread.Sleep(2000);
                        player.Write();

                    });
                    writeThread.Start();
                    Thread readThread = new Thread(() =>
                    {
                        Thread.Sleep(2000);
                        if (player.FirstTimeConnecting)
                        {
                            player.Playroom.SetGameInfo(player.Read());
                        }
                        else
                        {
                            player.Playroom.SetOpponentsAndTimers(player.Read());
                        }

                    });
                    readThread.Start();
                }
                catch(Exception e)
                {
                    throw e;
                }
            }
            
        }

        public void NameClient(string username)
        {
            player.Name = username;
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