using System;
using System.Collections.Generic;
using TypeRacers.Client;

namespace TypeRacers
{
    //a class that handles the messages to and from the network
    public class NetworkHandler
    {
        private readonly TypeRacersClient client;
        private TypeRacersClient.TimerTickHandler timerTickHandler;

        public NetworkHandler()
        {
            client = new TypeRacersClient();
        }

        public DateTime GetStartingTime()
        {
            return client.PlayersStartingTime;
        }

        public DateTime GetEndingTime()
        {
            return client.PlayersEndingTime;
        }

        public DateTime GetWaitingTime()
        {
            return client.WaitingTime;
        }

        public void RestartSearch()
        {
            client.RestartSearch();
        }

        public void StartSearchingOpponents()
        {
            client.StartTimerForSearchingOpponents();
        }

        public void SubscribeToSearchingOpponents(Action<Tuple<List<Tuple<string, Tuple<string, string, int>>>, Dictionary<string, Tuple<bool, int>>>> updateOpponentsList)
        {
            timerTickHandler = new TypeRacersClient.TimerTickHandler(updateOpponentsList);
            client.OpponentsChanged += new TypeRacersClient.TimerTickHandler(timerTickHandler);
        }

        public List<Tuple<string, Tuple<string, string, int>>> GetOpponents()

        {
            return client.GetOpponentsProgress();
        }

        public Dictionary<string, Tuple<bool, int>> GetRanking()
        {
            return client.Rank;
        }

        public string GetTextFromServer()
        {
            return client.FirstTimeConnectingToServer();
        }

        public void SendProgressToServer(string progress)
        {
            client.LocalPlayerProgress = progress;
        }

        public void NameClient(string username)
        {
            client.NameClient(username);
        }

        public void RemovePlayer()
        {
            client.RemovePlayerFromRoom();
        }

        public void StartReportingGameProgress()
        {
            client.StartTimerForGameProgressReports();
        }
    }
}