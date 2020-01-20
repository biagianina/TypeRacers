using System;
using System.Collections.Generic;
using TypeRacers.Client;

namespace TypeRacers
{
    //a class that handles the messages to and from the network
    public class NetworkHandler : INetworkHandler
    {
        readonly TypeRacersClient client;
        public NetworkHandler()
        {
            client = new TypeRacersClient();
        }

        public void StartSearchingOpponents()
        {
            client.StartTimerForSearchingOpponents();
        }

        public void SubscribeToSearchingOpponentsTimer(Action<Tuple<List<Tuple<string, string, bool>>, int>> updateOpponentsListAndElapsedTime)
        {
            client.OpponentsChanged += new TypeRacersClient.TimerTickHandler(updateOpponentsListAndElapsedTime);
        }
        public List<Tuple<string, string, bool>> GetOpponents()
        {
            return client.Opponents;
        }
        public string GetTextFromServer()
        {
            return client.GetMessageFromServer();
        }
      
        public void SendProgressToServer(string progress, bool userIsInGame)
        {
            client.SendProgressToServer(progress, userIsInGame);
        }

        public void NameClient(string username)
        {
            client.NameClient(username);
        }
    }
}
