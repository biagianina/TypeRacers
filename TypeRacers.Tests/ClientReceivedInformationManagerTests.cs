using Xunit;
using TypeRacers.Client;
using Common;
using Server;
using System;

namespace TypeRacersFacts
{
    public class ClientReceivedInformationManagerTests
    {
        Player player;
        GameInfo gameInfo;
        ClientReceivedInformationManager communicator;
        
        [Fact]
        public void FirstConnectionReceivedMessage()
        {
            player = new Player(new FakeTypeRacersClient());
            gameInfo = new GameInfo();
            player.SetPlayroom(gameInfo);
            communicator = new ClientReceivedInformationManager(player, gameInfo);
            var waitingTime = DateTime.UtcNow;
            player.Write(new GameMessage("This is the text", waitingTime, DateTime.MinValue, DateTime.MinValue));
            communicator.StartCommunication();
            Assert.Equal("This is the text", gameInfo.CompetitionText);
            Assert.False(gameInfo.TimeToWaitForOpponents.Equals(DateTime.MinValue));
        }
    }
}
