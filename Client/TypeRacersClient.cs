using System;
using System.Collections.Generic;
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
        static readonly int interval = 1000; // 1 second
        static readonly int totalTime = 30000; // 30 seconds or 30000 ms
        static int elapsedTime = 0; // Elapsed time in ms
        Timer timer;
        List<Tuple<string, string>> opponents;
        public delegate void TimerTickHandler(List<Tuple<string, string>> newOpponents);
        public event TimerTickHandler OpponentsChanged;
      
        private void SetOpponents(List<Tuple<string, string>> value)
        {
            opponents = value;
            OnOpponentsChanged(opponents);
        }

        private string LocalPlayerProgress { get; set; }
        public static string Name { get; set; }
        public void StartTimerForSearchingOpponents()
        {
            timer = new Timer(interval);
            timer.Elapsed += new ElapsedEventHandler(OnTimedEvent);
            timer.Enabled = true;

        }

        public void NameClient(string username)
        {
            Name = username;
        }

        void OnTimedEvent(object sender, ElapsedEventArgs e)
        {
            timer.Stop();
            if (elapsedTime > totalTime)
            {
                //we stop the timer after 30 seconds
                return;
            }
            else
            {
                // here I am performing the task
                //getting the opponents each second for 30 seconds from server through Client
                SetOpponents(GetOpponentsProgress());
                timer.Enabled = true;
            }

            elapsedTime += interval;
        }

        protected void OnOpponentsChanged(List<Tuple<string, string>> newOpponents)
        {
            if (newOpponents != null)
            {
                OpponentsChanged(newOpponents);
            }
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
            SetOpponents(GetOpponentsProgress());
            stream.Flush();
        }

        //receiving the opponents and their progress in a List
        public List<Tuple<string, string>> GetOpponentsProgress()
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

                var currentOpponents = text.Split('/').ToList();
                currentOpponents.Remove("#");
                List<Tuple<string, string>> opponents = new List<Tuple<string, string>>();
                foreach (var v in currentOpponents)
                {
                    var nameAndProgress = v.Split(':');
                    opponents.Add(new Tuple<string, string>(nameAndProgress.FirstOrDefault(), nameAndProgress.LastOrDefault()));
                }

                return opponents;
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
