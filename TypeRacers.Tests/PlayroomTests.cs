using Common;
using Server;
using System;
using TypeRacers.Client;
using Xunit;

namespace TypeRacersFacts
{
    public class PlayroomTests
    {
        private readonly Player player1 = new Player(new FakeTypeRacersClient())
        {
            Name = "george"
        };

        private readonly Player player2 = new Player(new FakeTypeRacersClient())
        {
            Name = "anca"
        };

        private readonly Player player3 = new Player(new FakeTypeRacersClient())
        {
            Name = "ana"
        };

        [Fact]
        public void ConstructorGeneratesTimeToWaitForOpponents()
        {
            var playroom = new Playroom(new MockGeneratedText());
            Assert.True(playroom.TimeToWaitForOpponents != DateTime.MinValue);
        }

        [Fact]
        public void FirstConnectionClientReturnsTrue()
        {
            var playroom = new Playroom(new MockGeneratedText());
            player1.Write(new PlayerMessage(4, 3, "george", true, false, false));
            var joined = playroom.Join(player1, new MockInformationManager());
            Assert.True(joined);
        }

        [Fact]
        public void SecondConnectionClientReturnsFalse()
        {
            var playroom = new Playroom(new MockGeneratedText());
            var infoManager = new MockInformationManager();
            player1.Write(new PlayerMessage(4, 3, "george", true, false, false));
            playroom.Join(player1, infoManager);
            player1.Write(new PlayerMessage(4, 3, "george", false, false, false));
            Assert.False(playroom.Join(player1, infoManager));
        }

        [Fact]
        public void ThreePlayersAreConnectedSetsStartingAndEndingTimes()
        {
            var playroom = new Playroom(new MockGeneratedText());
            var infoManager = new MockInformationManager();
            player1.Write(new PlayerMessage(4, 3, "george", true, false, false));
            player2.Write(new PlayerMessage(4, 3, "anca", true, false, false));
            player3.Write(new PlayerMessage(4, 3, "ana", true, false, false));

            playroom.Join(player1, infoManager);
            playroom.Join(player2, infoManager);
            playroom.Join(player3, infoManager);
            playroom.TrySetStartingTime();
            Assert.False(playroom.GameStartingTime.Equals(DateTime.MinValue));
            Assert.False(playroom.GameEndingTime.Equals(DateTime.MinValue));
        }

        [Fact]
        public void ReturningPlayersByName()
        {
            var playroom = new Playroom(new MockGeneratedText());
            var infoManager = new MockInformationManager();
            player1.Write(new PlayerMessage(4, 3, "george", true, false, false));
            playroom.Join(player1, infoManager);
            Assert.Equal(player1, playroom.GetPlayer("george"));
        }

        [Fact]
        public void FindsCorrectlyIsPlayersAreInRoomByName()
        {
            var playroom = new Playroom(new MockGeneratedText());
            var infoManager = new MockInformationManager();
            player1.Write(new PlayerMessage(4, 3, "george", true, false, false));
            player2.Write(new PlayerMessage(4, 3, "anca", true, false, false));
            player3.Write(new PlayerMessage(4, 3, "ana", true, false, false));
            playroom.Join(player1, infoManager);
            playroom.Join(player2, infoManager);
            playroom.Join(player3, infoManager);
            Assert.Equal(player1, playroom.GetPlayer("george"));
            Assert.Equal(player2, playroom.GetPlayer("anca"));
            Assert.Equal(player3, playroom.GetPlayer("ana"));
            Assert.Equal(default, playroom.GetPlayer("Ionut"));
        }
    }
}