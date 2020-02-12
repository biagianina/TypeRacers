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
        public DateTime WaitingTime { get; set; }
        public Dictionary<string, Tuple<bool, int>> Rank { get; set; } = new Dictionary<string, Tuple<bool, int>>();
        public string LocalPlayerProgress { get; set; } = "0&0";
        public DateTime PlayersStartingTime { get; set; }
        public DateTime PlayersEndingTime { get; private set; }
        private int PlayroomNumber { get; set; } = -1;
        public string Name { get; set; }
        public bool GameStarted { get; private set; }

        private TcpClient client;
        private NetworkStream stream;
        private Timer timer;
        private readonly int interval = 1000; // 1 second
        private int elapsedTime = 0; // Elapsed time in ms
        private List<Tuple<string, Tuple<string, string, int>>> opponents;

        public delegate void TimerTickHandler(Tuple<List<Tuple<string, Tuple<string, string, int>>>, Dictionary<string, Tuple<bool, int>>> opponentsAndRanking);

        public event TimerTickHandler OpponentsChanged;

        public void NameClient(string username)
        {
            Name = username;
        }

        public void StartTimerForSearchingOpponents()
        {
            timer = new Timer(interval);
            timer.Elapsed += OnTimedEvent;
            timer.Enabled = true;
        }

        public void StartTimerForGameProgressReports()
        {
            timer = new Timer(3000);
            timer.Elapsed += OnTimedReportProgressEvent;
            timer.Enabled = true;
        }

        public string FirstTimeConnectingToServer()
        {
            //connecting to server
            string dataToSend = LocalPlayerProgress + "&" + PlayroomNumber + "$" + Name + "#";
            var receivedData = SendDataToServer(dataToSend);

            var dataWithoutHashtag = receivedData.Remove(receivedData.Length - 1);
            var textToType = dataWithoutHashtag.Substring(0, dataWithoutHashtag.IndexOf('$'));

            var times = dataWithoutHashtag.Substring(dataWithoutHashtag.IndexOf('%') + 1);
            var gameTimers = times.Split('*');
            WaitingTime = DateTime.Parse(gameTimers.FirstOrDefault());
            var startAndEndTimes = gameTimers.LastOrDefault().Split('+');
            PlayersStartingTime = DateTime.Parse(startAndEndTimes.FirstOrDefault());
            PlayersEndingTime = DateTime.Parse(startAndEndTimes.LastOrDefault());

            return textToType;

        }

        public List<Tuple<string, Tuple<string, string, int>>> GetOpponentsProgress()
        {
            //connecting to server
            string toSend = LocalPlayerProgress + "&" + PlayroomNumber + "$" + Name + "#";

            var dataFromServer = SendDataToServer(toSend);

            var currentOpponents = dataFromServer.Split('%').ToList();
            currentOpponents.Remove("#");

            opponents = new List<Tuple<string, Tuple<string, string, int>>>();
            SetInfoOrRanking(currentOpponents);

            return opponents;

        }

        public void RestartSearch()
        {

            string toSend = Name + "_restart" + "#";

            var dataFromServer = SendDataToServer(toSend);
            var dataWithoutHashtag = dataFromServer.Remove(dataFromServer.Length - 1);
            WaitingTime = DateTime.Parse(dataWithoutHashtag);

        }

        public void SendProgressToServer(string progress)
        {

            //writing the progress to stream
            string toSend = progress + "&" + PlayroomNumber + "$" + Name + "#";

            SendDataToServer(toSend);
            SetOpponents(new Tuple<List<Tuple<string, Tuple<string, string, int>>>, Dictionary<string, Tuple<bool, int>>>(GetOpponentsProgress(), Rank));

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

        private void OnTimedEvent(object sender, ElapsedEventArgs e)
        {
            timer.Stop();

            if (WaitingTime - DateTime.UtcNow < TimeSpan.Zero)
            {
                elapsedTime = 0;
                //we stop the timer after 30 seconds
                return;
            }
            else
            {
                SetOpponents(new Tuple<List<Tuple<string, Tuple<string, string, int>>>, Dictionary<string, Tuple<bool, int>>>(GetOpponentsProgress(), Rank));
            }
            timer.Enabled = true;

            elapsedTime += interval;
        }

        private void OnTimedReportProgressEvent(object sender, ElapsedEventArgs e)
        {
            timer.Stop();

            if (PlayersEndingTime - DateTime.UtcNow < TimeSpan.Zero)
            {
                return;
            }
            else
            {
                SendProgressToServer(LocalPlayerProgress);
            }

            timer.Enabled = true;
        }

        private void SetOpponents(Tuple<List<Tuple<string, Tuple<string, string, int>>>, Dictionary<string, Tuple<bool, int>>> value)
        {
            opponents = value.Item1;
            Rank = value.Item2;
            OnOpponentsChanged(value);
        }

        protected void OnOpponentsChanged(Tuple<List<Tuple<string, Tuple<string, string, int>>>, Dictionary<string, Tuple<bool, int>>> opponents)
        {
            if (opponents != null)
            {
                OpponentsChanged(opponents);
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
                    PlayersStartingTime = DateTime.Parse(times.FirstOrDefault());
                    PlayersEndingTime = DateTime.Parse(times.LastOrDefault());
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

        private void SetInfo(string[] nameAndInfos)
        {
            var progressInfoAndPlayroomInfo = nameAndInfos.LastOrDefault()?.Split('&');

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

        private string SendDataToServer(string data)
        {
            client = new TcpClient("localhost", 80);
            stream = client.GetStream();

            try
            {
                byte[] bytesToSend = Encoding.ASCII.GetBytes(data);

                stream.Write(bytesToSend, 0, bytesToSend.Length);


                return ReceiveDataFromServer(client);

            }
            catch (Exception e)
            {
                throw e;
            }

        }

        private string ReceiveDataFromServer(TcpClient tcpClient)
        {
            byte[] inStream = new byte[tcpClient.ReceiveBufferSize];
            int read = stream.Read(inStream, 0, inStream.Length);
            string receivedData = Encoding.ASCII.GetString(inStream, 0, read);

            while (!receivedData[read - 1].Equals('#'))
            {
                read = stream.Read(inStream, 0, inStream.Length);
                receivedData += Encoding.ASCII.GetString(inStream, receivedData.Length, read);
            }

            return receivedData;
        }
    }
}