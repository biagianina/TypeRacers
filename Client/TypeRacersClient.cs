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
        public int TimeToSearchForOpponents { get; set; }
        public Dictionary<string, Tuple<bool, int>> Rank { get; set; } = new Dictionary<string, Tuple<bool, int>>();
        public string LocalPlayerProgress { get; set; } = "0&0";
        public string PlayersStartingTime { get; set; } = string.Empty;
        private int PlayroomNumber { get; set; }
        public string Name { get; set; }
        public bool GameStarted { get; private set; }


        TcpClient client;
        NetworkStream stream;
        Timer timer;
        int interval = 1000; // 1 second
        int elapsedTime = 0; // Elapsed time in ms
        List<Tuple<string, Tuple<string, string, int>>> opponents;
        bool playerIsRemoved;

        public delegate void TimerTickHandler(Tuple<List<Tuple<string, Tuple<string, string, int>>>, int, Dictionary<string, Tuple<bool, int>>> opponentsAndRankingAndElapsedTime);
        public event TimerTickHandler OpponentsChanged;

        private void SetOpponentsAndElapsedTime(Tuple<List<Tuple<string, Tuple<string, string, int>>>, int, Dictionary<string, Tuple<bool, int>>> value)
        {
            opponents = value.Item1;
            elapsedTime = value.Item2;
            Rank = value.Item3;
            OnOpponentsChangedAndTimeChanged(value);
        }
       
        public void StartTimerForSearchingOpponents()
        {
            timer = new Timer(interval);
            timer.Elapsed += new ElapsedEventHandler(OnTimedEvent);
            timer.Enabled = true;
        }
      
        void OnTimedEvent(object sender, ElapsedEventArgs e)
        {
            timer.Stop();

            if (elapsedTime > TimeToSearchForOpponents)
            {
                elapsedTime = 0;
                //we stop the timer after 30 seconds
                return;
            }
            else
            {
                // here I am performing the task
                //getting the opponents each second for 30 seconds from server through Client
                if (GameStarted)
                {
                    SendProgressToServer(LocalPlayerProgress);
                }
                else
                {
                    SetOpponentsAndElapsedTime(new Tuple<List<Tuple<string, Tuple<string, string, int>>>, int, Dictionary<string, Tuple<bool, int>>>(GetOpponentsProgress(), elapsedTime, Rank));
                }
                timer.Enabled = true;
            }

            elapsedTime += interval;
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

            }
            catch (Exception)
            {
                throw new Exception("Lost connection with server");
            }
            client.Close();

        }
        public void NameClient(string username)
        {
            Name = username;
        }

        public void RemovePlayerFromRoom()
        {
            playerIsRemoved = true;
        }

        protected void OnOpponentsChangedAndTimeChanged(Tuple<List<Tuple<string, Tuple<string, string, int>>>, int, Dictionary<string, Tuple<bool, int>>> opponentsAndElapsedTime)
        {
            if (opponentsAndElapsedTime != null)
            {
                OpponentsChanged(opponentsAndElapsedTime);
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
        //receiving the opponents and their progress in a List

        public List<Tuple<string, Tuple<string, string, int>>> GetOpponentsProgress()
        {
            //connecting to server
            client = new TcpClient("localhost", 80);
            stream = client.GetStream();
            string toSend;

            //writing the progress to stream

            if (playerIsRemoved)
            {
                var removedName  = Name + "_removed";

                toSend = LocalPlayerProgress + "&" + PlayroomNumber + "$" + removedName + "#";
                playerIsRemoved = false;
            }
            else
            {
                toSend = LocalPlayerProgress + "&" + PlayroomNumber + "$" + Name + "#";

            }

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

        private void SetInfoOrRanking(List<string> currentOpponents)
        {
            foreach (var v in currentOpponents)
            {
                if (v.First().Equals('*'))
                {
                    PlayersStartingTime = v.Substring(1);
                    if (!string.IsNullOrEmpty(PlayersStartingTime))
                    {
                        GameStarted = true;
                        interval = 3000;
                        TimeToSearchForOpponents += 90000 + (DateTime.Parse(PlayersStartingTime) - DateTime.UtcNow).Milliseconds;
                    }
                }
                else
                {
                    if (v.First().Equals('!'))
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

            while (!text[read - 1].Equals('#'))
            {
                read = stream.Read(inStream, 0, inStream.Length);
                text += Encoding.ASCII.GetString(inStream, text.Length, read);
            }

            if (stream.DataAvailable)
            {
                client.Close();
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
            foreach (var r in rank)
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
            }
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
                var timers = times.Split('*');
                TimeToSearchForOpponents = (DateTime.Parse(timers.FirstOrDefault()) - DateTime.UtcNow).Seconds * 1000;
                PlayersStartingTime = timers.LastOrDefault();
                PlayroomNumber = Convert.ToInt32(dataWithoutHashtag.Substring(dataWithoutHashtag.IndexOf('$') + 1, (dataWithoutHashtag.Length - textToType.Length - times.Length - 2)));
               
                return textToType;
            }
            catch (Exception e)
            {
                throw e;
            }
        }
    }
}
