using Common;
using Server;
using System;
using TypeRacers.Client;
using Xunit;

namespace TypeRacersFacts
{
    public class PlayroomTests
    {
        private IPlayroom playroom = new Playroom();
        private readonly Player player1 = new Player(new FakeTypeRacersClient())
        {
            Name = "Bianca"
        };
        private readonly Player player2 = new Player(new FakeTypeRacersClient())
        {
            Name = "Alin"
        };
        private readonly Player player3 = new Player(new FakeTypeRacersClient())
        {
            Name = "Gianina"
        };
        private readonly Player player4 = new Player(new FakeTypeRacersClient())
        {
            Name = "Ionut"
        };

        [Fact]
        public void ConstructorGeneratesTimeToWaitForOpponents()
        {            
            Assert.True(playroom.TimeToWaitForOpponents!= DateTime.MinValue);
        }

        [Fact]
        public void FirstConnectionClientReturnsTrue()
        {
            Assert.True(playroom.Join(player1));
        }

        [Fact]
        public void SecondConnectionClientReturnsFalse()
        {
            var playroom = new Playroom();
            playroom.Join(player1);
            player1.FirstTimeConnecting = false;
            Assert.False(playroom.Join(player1));
        }

        [Fact]
        public void ThreePlayersAreConnectedSetsStartingAndEndingTimes()
        {
            playroom.Join(player1);
            playroom.Join(player2);
            playroom.Join(player3);
            playroom.TrySetGameStartingTime();
            Assert.False(playroom.GameStartingTime.Equals(DateTime.MinValue));
            Assert.False(playroom.GameEndingTime.Equals(DateTime.MinValue));
        }

        [Fact]
        public void ReturningPlayersByName()
        {
            playroom.Join(player1);
            Assert.Equal(player1, playroom.GetPlayer("Bianca"));
        }

        [Fact]
        public void FindsCorrectlyIsPlayersAreInRoomByName()
        {
            playroom.Join(player1);
            playroom.Join(player2);
            playroom.Join(player3);
            Assert.Equal(player1, playroom.GetPlayer("Bianca"));
            Assert.Equal(player2, playroom.GetPlayer("Alin"));
            Assert.Equal(player3, playroom.GetPlayer("Gianina"));
            Assert.Equal(default, playroom.GetPlayer("Ionut"));
        }

        [Fact]
        public void RemovesPlayerCorrectlyByName()
        {
            playroom.Join(player1);
            playroom.Join(player2);
            playroom.Join(player3);
            Assert.Equal(player1, playroom.GetPlayer("Bianca"));
            Assert.Equal(player2, playroom.GetPlayer("Alin"));
            Assert.Equal(player3, playroom.GetPlayer("Gianina"));
            player2.Removed = true;
            player2.Name = "Alin_removed";
            playroom.CheckIfPlayerLeft(player2);
            Assert.Equal(default, playroom.GetPlayer("Alin"));
        }
    }
}
