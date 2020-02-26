﻿using System.Net.Sockets;
using System.Text;

namespace Common
{
    public class Player
    {
        private readonly TcpClient tcpClient;
        private NetworkStream networkStream;

        public Player(TcpClient tcpClient)
        {
            this.tcpClient = tcpClient;
        }

        public bool FirstTimeConnecting = true;
        public string Name { get; set; }
        public int Place { get; set; }
        public bool Restarting { get; set; }
        public bool Removed { get; set; }
        public bool Finnished { get; set; }
        public int WPMProgress { get; set; }
        public int CompletedTextPercentage { get; set; }
        public IPlayroom Playroom { get; set; }

        public void SetPlayroom(IPlayroom playroom)
        {
            Playroom = playroom;
        }

        public void UpdateInfo(int wpmProgress, int completedText)
        {
            WPMProgress = wpmProgress;
            CompletedTextPercentage = completedText;
        }

        public string Read()
        {
            networkStream = tcpClient.GetStream();

            byte[] buffer = new byte[tcpClient.ReceiveBufferSize];
            int bytesRead = networkStream.Read(buffer, 0, buffer.Length);

            var dataRecieved = Encoding.ASCII.GetString(buffer, 0, bytesRead);

            //solution to get complete messages
            //while (!dataRecieved.Last().Equals('#'))
            //{
            //    bytesRead = networkStream.Read(buffer, 0, tcpClient.ReceiveBufferSize);
            //    dataRecieved += Encoding.ASCII.GetString(buffer, dataRecieved.Length, bytesRead);
            //}

            return dataRecieved.Remove(dataRecieved.Length - 1);
        }

        public void TrySetRank()
        {
            if (CompletedTextPercentage == 100 && !Finnished)
            {
                Finnished = true;
                Place = Playroom.Place++;
            }
        }

        public void Write(IMessage message)
        {

            networkStream = tcpClient.GetStream();
            var toSend = message.ToByteArray();
            networkStream.Write(toSend, 0, toSend.Length);
        }
    }
}