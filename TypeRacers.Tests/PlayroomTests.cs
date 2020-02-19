namespace Server
{
    public class PlayroomTests
    {
        private Playroom playroom = new Playroom();
        private readonly Player player1 = new Player("Bianca");
        private readonly Player player2 = new Player("Alin");
        private readonly Player player3 = new Player("Gianina");
        private readonly Player player4 = new Player("Ionut");

        [Fact]
        public void ConstructorGeneratesTimeToWaitForOpponents()
        {
            Playroom playroom = new Playroom();
            Assert.Equal(14, (playroom.TimeToWaitForOpponents - DateTime.UtcNow).Seconds);
        }

        [Fact]
        public void FirstConnectionClientReturnsTrue()
        {
            Assert.True(playroom.Join(player1));
        }

        [Fact]
        public void SecondConnectionClientReturnsTrue()
        {
            playroom.Join(player1);
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
            Assert.True(playroom.IsInPlayroom("Bianca"));
            Assert.True(playroom.IsInPlayroom("Alin"));
            Assert.True(playroom.IsInPlayroom("Gianina"));
            Assert.False(playroom.IsInPlayroom("Ionut"));
        }

        [Fact]
        public void RemovesPlayerCorrectlyByName()
        {
            playroom.Join(player1);
            playroom.Join(player2);
            playroom.Join(player3);
            Assert.True(playroom.IsInPlayroom("Bianca"));
            Assert.True(playroom.IsInPlayroom("Alin"));
            Assert.True(playroom.IsInPlayroom("Gianina"));
            playroom.RemovePlayer("Alin");
            Assert.False(playroom.IsInPlayroom("Alin"));
        }
    }
}