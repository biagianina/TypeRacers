using Common;
using System.Net.Sockets;
using TypeRacers.Client;

namespace TypeRacers
{
    //a class that handles the messages to and from the network
    public class NetworkHandler
    {
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
    }
}