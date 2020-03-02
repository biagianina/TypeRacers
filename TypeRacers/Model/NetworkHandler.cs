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
            client = new TcpClient();
            player = new Player(new TypeRacersNetworkClient(client))
            {
                Name = userName
            };
            typeRacersClient = new TypeRacersClient(player);
        }

        internal IPlayroom GameModel()
        {
            return player.Playroom;
        }

        internal Player PlayerModel()
        {
            return player;
        }

        internal void StartCommunication()
        {
            client.Connect("localhost", 80);
            typeRacersClient.StartServerCommunication();
        }

        public void SendProgressToServer(int wpmProgress, int completedTextPercentage)
        {
            //to implement finished and place
            typeRacersClient.Player.UpdateInfo(wpmProgress, completedTextPercentage);
        }

        public void RemovePlayer()
        {
            typeRacersClient.RemovePlayer();
        }

        public void RestartSearch()
        {
            typeRacersClient.RestartSearch();
        }

        public void SubscribeToSearchingOpponents(Action<List<Player>> updateOpponentsList)
        {
            oponentsChangedEventHandler = new TypeRacersClient.OpponentsChangedEventHandler(updateOpponentsList);
            typeRacersClient.OpponentsChanged += new TypeRacersClient.OpponentsChangedEventHandler(oponentsChangedEventHandler);
        }
    }
}