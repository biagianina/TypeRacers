using System;
using System.Collections.Generic;
using TypeRacers.Client;

namespace TypeRacers
{
    //a class that handles the messages to and from the network
    public class NetworkHandler
    {
        private readonly TypeRacersClient client;
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
        public void SubscribeToSearchingOpponents(Action<Tuple<List<Tuple<string, Tuple<string, string, int>>>, Dictionary<string, Tuple<bool, int>>>> updateOpponentsList)
        {
            oponentsChangedEventHandler = new TypeRacersClient.OpponentsChangedEventHandler(updateOpponentsList);
            client.OpponentsChanged += new TypeRacersClient.OpponentsChangedEventHandler(oponentsChangedEventHandler);
        }

        public List<Tuple<string, Tuple<string, string, int>>> GetOpponents()
        {
            return client.GetOpponentsProgress();
        }

        public void StartServerCommunication()
        {
            client.StartServerCommunication();
        }

        public void GetTextToType()
        {
            client.GetTextToType();
        }

        public Dictionary<string, Tuple<bool, int>> GetRanking()
        {
            return client.Rank;
        }

        public string GetTextFromServer()
        {
            return client.GetTextToType();
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
            throw new NotImplementedException();
        }
    }
}