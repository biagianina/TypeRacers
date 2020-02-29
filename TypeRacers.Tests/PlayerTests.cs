using System;
using System.Collections.Generic;
using System.Text;
using Xunit;
using Common;
using TypeRacers.Client;
namespace TypeRacersFacts
{
    public class PlayerTests
    {
        [Fact]
        public void TestPlayerRead()
        {
            var player = new Player(new FakeTypeRacersClient());

            Assert.Equal("just a text", player.Read());
        }

        [Fact]
        public void TestPlayerWrite()
        {
            var player = new Player(new FakeTypeRacersClient());

            player.Write(new PlayerMessage(4, 3, "george", true, false, false));
            Assert.Equal("4&3&True$george#", player.Read());
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
