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
        public string WaitingTime { get; set; }
        public Dictionary<string, Tuple<bool, int>> Rank { get; set; } = new Dictionary<string, Tuple<bool, int>>();
        public string LocalPlayerProgress { get; set; } = "0&0";
        public string PlayersStartingTime { get; set; } = string.Empty;
        public string PlayersEndingTime { get; private set; }
        private int PlayroomNumber { get; set; } = -1;
        public string Name { get; set; }
        public bool GameStarted { get; private set; }

        TcpClient client;
        NetworkStream stream;
        Timer timer;
        readonly int interval = 1000; // 1 second
        int elapsedTime = 0; // Elapsed time in ms
        List<Tuple<string, Tuple<string, string, int>>> opponents;

        public delegate void TimerTickHandler(Tuple<List<Tuple<string, Tuple<string, string, int>>>, int, Dictionary<string, Tuple<bool, int>>> opponentsAndRankingAndElapsedTime);
        public event TimerTickHandler OpponentsChanged;

        public void NameClient(string username)
        {
            Name = username;
        }
        public void StartTimerForSearchingOpponents()
        {
            timer = new Timer(interval);
            timer.Elapsed += new ElapsedEventHandler(OnTimedEvent);
            timer.Enabled = true;
        }
        public void StartTimerForGameProgressReports()
        {
            timer = new Timer(3000);
            timer.Elapsed += new ElapsedEventHandler(OnTimedReportProgressEvent);
            timer.Enabled = true;
        }
        public string FirstTimeConnectingToServer()
        {
            //connecting to server
            client = new TcpClient("localhost", 80);
            stream = client.GetStream();

            try
            {
                byte[] bytesToSend = Encoding.ASCII.GetBytes(LocalPlayerProgress + "&" + PlayroomNumber + "$" + Name + "#");

                stream.Write(bytesToSend, 0, bytesToSend.Length);

                byte[] inStream = new byte[client.ReceiveBufferSize];
                int read = stream.Read(inStream, 0, inStream.Length);
                string recievedData = Encoding.ASCII.GetString(inStream, 0, read);

                while (!recievedData[read - 1].Equals('#'))
                {
                    read = stream.Read(inStream, 0, inStream.Length);
                    recievedData += Encoding.ASCII.GetString(inStream, recievedData.Length, read);
                }

                var dataWithoutHashtag = recievedData.Remove(recievedData.Length - 1);
                var textToType = dataWithoutHashtag.Substring(0, dataWithoutHashtag.IndexOf('$'));
                //getting room number

                var times = dataWithoutHashtag.Substring(dataWithoutHashtag.IndexOf('%') + 1);
                var gameTimers = times.Split('*');
                WaitingTime = gameTimers.FirstOrDefault();
                var startAndEndTimes = gameTimers.LastOrDefault().Split('+');
                PlayersStartingTime = startAndEndTimes.FirstOrDefault();
                PlayersEndingTime = startAndEndTimes.LastOrDefault();
                PlayroomNumber = Convert.ToInt32(dataWithoutHashtag.Substring(dataWithoutHashtag.IndexOf('$') + 1, (dataWithoutHashtag.Length - textToType.Length - times.Length - 2)));

                return textToType;
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public List<Tuple<string, Tuple<string, string, int>>> GetOpponentsProgress()
        {
            //connecting to server
            client = new TcpClient("localhost", 80);
            stream = client.GetStream();
            string toSend;

            //writing the progress to stream

            toSend = LocalPlayerProgress + "&" + PlayroomNumber + "$" + Name + "#";


            byte[] bytesToSend = Encoding.ASCII.GetBytes(toSend);
            stream.Write(bytesToSend, 0, bytesToSend.Length);

            try
            {
                var text = GetDataFromServer();

                var currentOpponents = text.Split('/').ToList();
                currentOpponents.Remove("#");

                opponents = new List<Tuple<string, Tuple<string, string, int>>>();
                SetInfoOrRanking(currentOpponents);

                return opponents;
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public void RestartSearch()
        {
            //connecting to server
            client = new TcpClient("localhost", 80);
            stream = client.GetStream();

            //writing the progress to stream

            string toSend = Name + "_restart" + "#";

            byte[] bytesToSend = Encoding.ASCII.GetBytes(toSend);
            stream.Write(bytesToSend, 0, bytesToSend.Length);

            try
            {
                byte[] inStream = new byte[client.ReceiveBufferSize];
                int read = stream.Read(inStream, 0, inStream.Length);
                string text = Encoding.ASCII.GetString(inStream, 0, read);
                var dataWithoutHashtag = text.Remove(text.Length - 1);
                WaitingTime = dataWithoutHashtag;

            }
            catch (Exception)
            {
                throw new Exception("Lost connection with server");
            }
        }
        public void SendProgressToServer(string progress)
        {
            //connecting to server
            client = new TcpClient("localhost", 80);
            stream = client.GetStream();


            //writing the progress to stream
            byte[] bytesToSend = Encoding.ASCII.GetBytes(progress + "&" + PlayroomNumber + "$" + Name + "#");
            stream.Write(bytesToSend, 0, bytesToSend.Length);

            SetOpponentsAndElapsedTime(new Tuple<List<Tuple<string, Tuple<string, string, int>>>, int, Dictionary<string, Tuple<bool, int>>>(GetOpponentsProgress(), elapsedTime, Rank));

            stream.Flush();
        }
        public void RemovePlayerFromRoom()
        {
            //connecting to server
            client = new TcpClient("localhost", 80);
            stream = client.GetStream();

            //writing the progress to stream

            string toSend = Name + "_removed" + "#";

            byte[] bytesToSend = Encoding.ASCII.GetBytes(toSend);
            stream.Write(bytesToSend, 0, bytesToSend.Length);

            timer.Stop();
        }
        void OnTimedEvent(object sender, ElapsedEventArgs e)
        {
            timer.Stop();
 
            if (DateTime.Parse(WaitingTime) - DateTime.UtcNow < TimeSpan.Zero)
            {
                elapsedTime = 0;
                //we stop the timer after 30 seconds
                return;
            }
            else
            {
                SetOpponentsAndElapsedTime(new Tuple<List<Tuple<string, Tuple<string, string, int>>>, int, Dictionary<string, Tuple<bool, int>>>(GetOpponentsProgress(), elapsedTime, Rank));
            }
            timer.Enabled = true;

            elapsedTime += interval;
        }
        private void OnTimedReportProgressEvent(object sender, ElapsedEventArgs e)
        {
            timer.Stop();

            if(DateTime.Parse(PlayersEndingTime) -  DateTime.UtcNow < TimeSpan.Zero)
            {
                return;
            }
            else
            {
                SendProgressToServer(LocalPlayerProgress);
            }

            timer.Enabled = true;
        }
        private void SetOpponentsAndElapsedTime(Tuple<List<Tuple<string, Tuple<string, string, int>>>, int, Dictionary<string, Tuple<bool, int>>> value)
        {
            opponents = value.Item1;
            elapsedTime = value.Item2;
            Rank = value.Item3;
            OnOpponentsChangedAndTimeChanged(value);
        }
        protected void OnOpponentsChangedAndTimeChanged(Tuple<List<Tuple<string, Tuple<string, string, int>>>, int, Dictionary<string, Tuple<bool, int>>> opponentsAndElapsedTime)
        {
            if (opponentsAndElapsedTime != null)
            {
                OpponentsChanged(opponentsAndElapsedTime);
            }
        }
       
        //receiving the opponents and their progress in a List
        private void SetInfoOrRanking(List<string> currentOpponents)
        {

            foreach (var v in currentOpponents)
            {
                if (v.FirstOrDefault().Equals('*'))
                {
                    var times = v.Substring(1).Split('+');
                    PlayersStartingTime = times.FirstOrDefault();
                    PlayersEndingTime = times.LastOrDefault();

                }
                else
                {
                    if (v.FirstOrDefault().Equals('!'))
                    {
                        var rank = v.Substring(1).Split(';');
                        SetRanking(rank);
                    }
                    else
                    {

                        var nameAndInfos = v.Split(':');
                        SetInfo(nameAndInfos);
                    }
                }
            }
        }
        private string GetDataFromServer()
        {
            byte[] inStream = new byte[client.ReceiveBufferSize];
            int read = stream.Read(inStream, 0, inStream.Length);
            string text = Encoding.ASCII.GetString(inStream, 0, read);

            while (!string.IsNullOrEmpty(text) && !text[read - 1].Equals('#'))
            {
                read = stream.Read(inStream, 0, inStream.Length);
                text += Encoding.ASCII.GetString(inStream, text.Length, read);
            }

           
            return text;
        }
        private void SetInfo(string[] nameAndInfos)
        {
            var progressInfoAndPlayroomInfo = nameAndInfos.LastOrDefault().Split('&');

            if (progressInfoAndPlayroomInfo.Count() == 3)
            {
                var wpmProgress = progressInfoAndPlayroomInfo[0];
                var carProgress = progressInfoAndPlayroomInfo[1];
                var playroomNumber = Convert.ToInt32(progressInfoAndPlayroomInfo[2]);
                var opponentInfo = new Tuple<string, string, int>(wpmProgress, carProgress, playroomNumber);
                opponents.Add(new Tuple<string, Tuple<string, string, int>>(nameAndInfos.FirstOrDefault(), opponentInfo));
            }
        }
        private void SetRanking(string[] rank)
        {
            rank.Aggregate(Rank, (Rank, r) =>
            {
                if (!string.IsNullOrEmpty(r))
                {
                    var nameAndRank = r.Split(':');
                    var name = nameAndRank[0];
                    var currentRank = nameAndRank[1].Split('&');
                    var rankToAdd = new Tuple<bool, int>(Convert.ToBoolean(currentRank[0]), Convert.ToInt32(currentRank[1]));
                    if (!Rank.ContainsKey(name))
                    {
                        Rank.Add(name, rankToAdd);
                    }
                    else
                    {
                        Rank[name] = rankToAdd;
                    }
                }
                return Rank;
            });
        }
    }
}
