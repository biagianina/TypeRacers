using Common;
using Server;
using System;
using TypeRacers.Client;
using Xunit;

namespace TypeRacersFacts
{
    public class RoomsTests
    {
        private readonly Rooms rooms = new Rooms();
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
        public void FirstPlayerAddedToRoom()
        {
            player1.Write(new PlayerMessage(0, 0, "Bianca", true, false, false));
            rooms.AllocatePlayroom(player1);
            Assert.Equal(rooms.currentPlayroom, player1.Playroom);
        }

        [Fact]
        public void ThreePlayersAddedToRoom()
        {
            rooms.AllocatePlayroom(player1);
            Assert.Equal(rooms.playrooms[0], player1.Playroom);

            rooms.AllocatePlayroom(player2);
            Assert.Equal(rooms.playrooms[0], player2.Playroom);

            rooms.AllocatePlayroom(player3);
            Assert.Equal(rooms.playrooms[0], player3.Playroom);
        }

        [Fact]
        public void ConstructorCreatesAnotherPlayroomWhenOneIsFull()
        {
            rooms.AllocatePlayroom(player1);
            rooms.AllocatePlayroom(player2);
            rooms.AllocatePlayroom(player3);
            rooms.AllocatePlayroom(player4);
            Assert.Equal(2, rooms.GetNumberOfPlayrooms());
        }

        [Fact]
        public void ConstructorCreatesAnotherPlayroomWhenOneGameHasStarted()
        {
            rooms.AllocatePlayroom(player1);
            rooms.AllocatePlayroom(player2);
            Assert.Contains(player1, rooms.playrooms[0].Players);
            Assert.Contains(player2, rooms.playrooms[0].Players);
            rooms.playrooms[0].GameStartingTime = DateTime.UtcNow;
            rooms.AllocatePlayroom(player3);
            Assert.Equal(2, rooms.GetNumberOfPlayrooms());
            Assert.Contains(player3, rooms.playrooms[1].Players);
        }
    }
}
