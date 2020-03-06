using Common;
using System;
using System.Linq;
using System.Threading;

namespace TypeRacers.Client
{
    public class TypeRacersClient
    {
        private GameInfo gameInfo;
        private Player player;

        public TypeRacersClient(Player player)
        {
            this.player = player;
            gameInfo = new GameInfo();
            player.SetPlayroom(gameInfo);
        }

        public void StartCommunication()
        {
            var communicator = new ReceivedInformationManager(player, gameInfo);
            Thread receiveCommunication = new Thread(()=> { communicator.StartCommunication(); });
            receiveCommunication.Start();
            Thread sendCommunication = new Thread(() => { communicator.Write(); });
            sendCommunication.Start();
        }
    }
}