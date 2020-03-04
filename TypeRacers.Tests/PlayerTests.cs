using Common;
using TypeRacers.Client;
using Xunit;

namespace TypeRacersFacts
{
    public class PlayerTests
    {
        [Fact]
        public void TestPlayerRead()
        {
            var player = new Player(new FakeTypeRacersClient());

            var message = (ReceivedMessage)player.Read();
            Assert.Equal("just a text", message.GetData());
        }

        [Fact]
        public void TestPlayerWrite()
        {
            var player = new Player(new FakeTypeRacersClient());

            player.Write(new PlayerMessage(4, 3, "george", true, false, false));
            var message = (ReceivedMessage)player.Read();
            Assert.Equal("4&3&True$george#", message.GetData());
        }

        [Fact]
        public void TestUpdateInfo()
        {
            var player = new Player(new FakeTypeRacersClient());

            player.UpdateInfo(3, 5);

            Assert.Equal(3, player.WPMProgress);
            Assert.Equal(5, player.CompletedTextPercentage);
        }
    }
}