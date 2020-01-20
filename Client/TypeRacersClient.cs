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
        Timer timer;
        readonly int interval = 1000; // 1 second
        readonly int totalTime = 30000; // 30 seconds or 30000 ms
        int elapsedTime = 0; // Elapsed time in ms
        List<Tuple<string, string, bool>> opponents;
        public delegate void TimerTickHandler(Tuple<List<Tuple<string, string, bool>>, int> opponentsAndElapsedTime);
        public event TimerTickHandler OpponentsChanged;

        public TypeRacersClient()
        {
            opponents = new List<Tuple<string, string, bool>>();
        }

        public List<Tuple<string, string, bool>> Opponents { get => opponents; private set => opponents = value; }
        public bool LocalPlayerIsInGame { get; set; }
        private string LocalPlayerProgress { get; set; }
        public static string Name { get; set; }
        private void SetOpponentsAndElapsedTime(Tuple<List<Tuple<string, string, bool>>, int> value)
        {
            opponents = value.Item1;
            elapsedTime = value.Item2;
            OnOpponentsChangedAndTimeChanged(value);
        }

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
                SetOpponentsAndElapsedTime(new Tuple<List<Tuple<string, string, bool>>, int>(Opponents, elapsedTime));
                timer.Enabled = true;
            }

            elapsedTime += interval;
        }

        protected void OnOpponentsChangedAndTimeChanged(Tuple<List<Tuple<string, string, bool>>, int> opponentsAndElapsedTime)
        {
            if (opponentsAndElapsedTime != null)
            {
                OpponentsChanged(opponentsAndElapsedTime);
            }
        }
        public void SendProgressToServer(string progress, bool userIsInGame)
        {
            //connecting to server
            client = new TcpClient("localhost", 80);
            stream = client.GetStream();

            //getting the info if the player is in game
            LocalPlayerIsInGame = userIsInGame;
            //getting the current progress from versus view model
            LocalPlayerProgress = progress;
            //writing the progress to stream
            byte[] bytesToSend = Encoding.ASCII.GetBytes(LocalPlayerProgress + "$" + Name + "*" + LocalPlayerIsInGame + "#");
            stream.Write(bytesToSend, 0, bytesToSend.Length);
            SetOpponentsAndElapsedTime(new Tuple<List<Tuple<string, string, bool>>, int>(Opponents, elapsedTime));
            stream.Flush();
        }

        //receiving the opponents and their progress in a List
        public void GetOpponentsProgress()
        {
            //connecting to server
            client = new TcpClient("localhost", 80);
            stream = client.GetStream();
            //writing the progress to stream
            byte[] bytesToSend = Encoding.ASCII.GetBytes(LocalPlayerProgress + "$" + Name + "*" + LocalPlayerIsInGame + "#");
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

                SetOpponents(text.Remove('#'));

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
                string userInfo = "0" + "$" + Name + "*" + LocalPlayerIsInGame + "#";
                byte[] bytesToSend = Encoding.ASCII.GetBytes(userInfo);
                stream.Write(bytesToSend, 0, bytesToSend.Length);

                byte[] inStream = new byte[client.ReceiveBufferSize];
                int read = stream.Read(inStream, 0, inStream.Length);
                string text = Encoding.ASCII.GetString(inStream, 0, read);

                while (!text[read - 1].Equals('#'))
                {
                    read = stream.Read(inStream, 0, inStream.Length);
                    text += Encoding.ASCII.GetString(inStream, text.Length, read);
                }
                text = text.Remove(text.Length - 1);
                client.Close();
                SetOpponents(text.Substring(text.IndexOf('$') + 1));
                return text.Substring(0, text.IndexOf('$'));
            }
            catch (Exception)
            {
                throw new Exception("Lost connection with server");
            }
        }

        private void SetOpponents(string opponentsList)
        {
            if (string.IsNullOrEmpty(opponentsList))
            {
                return;
            }

            var op = opponentsList.Split(';');
            foreach (var v in op)
            {
                var playerInfo = v.Split(':');
                string name = playerInfo[0];
                var progressAndIsInGame = playerInfo[1].Split('/');
                opponents.Add(new Tuple<string, string, bool>(name, progressAndIsInGame[0], Convert.ToBoolean(progressAndIsInGame[1])));
            }
        }
    }
}
