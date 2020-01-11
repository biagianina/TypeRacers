using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Timers;

namespace TypeRacers.Client
{
    public class TypeRacersClient
    {
        TcpClient client;
        NetworkStream stream;
        private List<string> Opponents { get; set; }

        private string LocalPlayerProgress { get; set; }
        public string Name { get; set; }

        public TypeRacersClient()
        {
            Opponents = new List<string>();
        }
        public void SendProgressToServer(string progress)
        {
            //connecting to server

            client = new TcpClient("localhost", 80);
            stream = client.GetStream();

            //getting the current progress from versus view model
            LocalPlayerProgress = progress;
            //writing the progress to stream
            byte[] bytesToSend = Encoding.ASCII.GetBytes(LocalPlayerProgress + "$" + Name + "#");
            stream.Write(bytesToSend, 0, bytesToSend.Length);
            Opponents = GetOpponentsProgress();
            stream.Flush();
        }

        //receiving the opponents and their progress in a List
        public List<string> GetOpponentsProgress()
        {

            //connecting to server
            client = new TcpClient("localhost", 80);
            stream = client.GetStream();
            //writing the progress to stream
            byte[] bytesToSend = Encoding.ASCII.GetBytes(LocalPlayerProgress + "$" + Name + "#");
            stream.Write(bytesToSend, 0, bytesToSend.Length);

            try
            {
                byte[] inStream = new byte[client.ReceiveBufferSize];
                int read = stream.Read(inStream, 0, inStream.Length);
                string text = Encoding.ASCII.GetString(inStream, 0, read);

                while (!text[read - 1].Equals('#'))
                {
                    read = stream.Read(inStream, 0, inStream.Length);
                    text += Encoding.ASCII.GetString(inStream, text.Length, read);
                }
                client.Close();
                var currentOpponents = text.Split('/').ToList();
                currentOpponents.Remove("#");
                return currentOpponents;
            }
            catch (Exception)
            {
                throw new Exception("Lost connection with server");
            }

        }

        public string GetMessageFromServer()
        {
            //connecting to server
            client = new TcpClient("localhost", 80);
            stream = client.GetStream();

            try
            {
                byte[] bytesToSend = Encoding.ASCII.GetBytes("0" + "$" + Name + "#");
                stream.Write(bytesToSend, 0, bytesToSend.Length);

                byte[] inStream = new byte[client.ReceiveBufferSize];
                int read = stream.Read(inStream, 0, inStream.Length);
                string text = Encoding.ASCII.GetString(inStream, 0, read);

                while (!text[read - 1].Equals('#'))
                {
                    read = stream.Read(inStream, 0, inStream.Length);
                    text += Encoding.ASCII.GetString(inStream, text.Length, read);
                }
                client.Close();
                return text.Substring(0, text.IndexOf('#'));
            }
            catch (Exception)
            {
                throw new Exception("Lost connection with server");
            }
        }

    }
}
