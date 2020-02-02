using System;
using System.Collections.Generic;
using TypeRacers.Client;

namespace TypeRacers
{
    //a class that handles the messages to and from the network
    public class NetworkHandler : INetworkHandler
    {
        readonly TypeRacersClient client;
        TypeRacersClient.TimerTickHandler timerTickHandler;
        public NetworkHandler()
        {
            client = new TypeRacersClient();
        }

        public string GetStartingTime()
        {
            return client.PlayersStartingTime;
        }
        public int GetWaitingTime()
        {
            return client.TimeToSearchForOpponents;
        }

        public void RestartSearch()
        {
            client.RestartSearch = true;
        }
        public void StartSearchingOpponents()
        {
            client.StartTimerForSearchingOpponents();
        }
        public void SubscribeToSearchingOpponentsTimer(Action<Tuple<List<Tuple<string, Tuple<string, string, int>>>, int, Dictionary<string, Tuple<bool, int>>>> updateOpponentsListAndElapsedTime)
        {
            timerTickHandler = new TypeRacersClient.TimerTickHandler(updateOpponentsListAndElapsedTime);
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
            return client.GetMessageFromServer();
        }
      
        public void SendProgressToServer(string progress)
        {
            client.SendProgressToServer(progress);
        }

        public void NameClient(string username)
        {
            client.NameClient(username);
        }

        public void RemovePlayer()
        {
           client.RemovePlayerFromRoom();
        }
    }
}
