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

        public List<Tuple<string, string>> GetOpponents()
        {
            return client.GetOpponentsProgress();
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
            client.Name = username;
        }
    }
}
