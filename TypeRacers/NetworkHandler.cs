using Common;
using System;
using System.Collections.Generic;
using System.Net.Sockets;
using TypeRacers.Client;


namespace TypeRacers
{
    //a class that handles the messages to and from the network
    public class NetworkHandler
    {
        private TypeRacersClient.OpponentsChangedEventHandler oponentsChangedEventHandler;
        private Player player;
        private readonly TcpClient client;
        private TypeRacersClient typeRacersClient;
        public NetworkHandler(string userName)
        {
            client = new TcpClient("localhost", 80);
            player = new Player(client)
            {
                Name = userName
            };
            typeRacersClient = new TypeRacersClient(player);
        }

        public DateTime GetStartingTime()
        {
            return typeRacersClient.gameInfo.GameStartingTime;
        }

        public DateTime GetEndingTime()
        {
            return typeRacersClient.gameInfo.GameEndingTime;
        }

        public DateTime GetWaitingTime()
        {
            return typeRacersClient.gameInfo.TimeToWaitForOpponents;
        }

        public List<Common.Player> GetOpponents()
        {
            return typeRacersClient.gameInfo.Players;
        }

        //public Dictionary<string, Tuple<bool, int>> GetRanking()
        //{
        //    return client.Rank;
        //}

        public string GetTextFromServer()
        {
            return typeRacersClient.gameInfo.CompetitionText;
        }

        public void SendProgressToServer(int wpmProgress, int completedTextPercentage)
        {
            //to implement finished and place
            typeRacersClient.Player.UpdateInfo(wpmProgress, completedTextPercentage);
        }

        public void NameClient(string username)
        {
            typeRacersClient.NameClient(username);
        }

        public void RemovePlayer()
        {
            throw new NotImplementedException();
        }

        public void StartReportingGameProgress()
        {
            throw new NotImplementedException();
        }

        internal bool PlayerFinnished()
        {
            return player.Finnished;
        }

        internal string PlayerPlace()
        {
            return player.Place.ToString();
        }

        public void RestartSearch()
        {
            //client.RestartSearch();
        }
        public void SubscribeToSearchingOpponents(Action<List<Common.Player>> updateOpponentsList)
        {
            oponentsChangedEventHandler = new TypeRacersClient.OpponentsChangedEventHandler(updateOpponentsList);
            typeRacersClient.OpponentsChanged += new TypeRacersClient.OpponentsChangedEventHandler(oponentsChangedEventHandler);
        }

    }
}