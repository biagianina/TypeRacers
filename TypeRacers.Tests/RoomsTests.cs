using System;
using Xunit;

namespace Server
{
    public class RoomsTests
    {
        readonly Rooms rooms = new Rooms();
        readonly Player player1 = new Player("Bianca");
        readonly Player player2 = new Player("Alin");
        readonly Player player3 = new Player("Gianina");
        readonly Player player4 = new Player("Ionut");

        [Fact]
        public void ConstructorCreatesOnePlayroomByDefault()
        {
            Rooms rooms = new Rooms();
            Assert.Equal(1, rooms.GetNumberOfPlayrooms());
        }

        [Fact]
        public void FirstPlayerAddedToRoom()
        {
            rooms.Join(player1);
            Assert.Equal(rooms.LastAvailablePlayroom, player1.Playroom);
        }

        [Fact]
        public void ThreePlayersAddedToRoom()
        {
            rooms.Join(player1);
            Assert.Equal(rooms.LastAvailablePlayroom, player1.Playroom);

            rooms.Join(player2);
            Assert.Equal(rooms.LastAvailablePlayroom, player2.Playroom);

            rooms.Join(player3);
            Assert.Equal(rooms.LastAvailablePlayroom, player3.Playroom);
        }

        [Fact]
        public void ConstructorCreatesAnotherPlayroomWhenOneIsFull()
        {
            rooms.Join(player1);
            rooms.Join(player2);
            rooms.Join(player3);
            rooms.Join(player4);
            Assert.Equal(2, rooms.GetNumberOfPlayrooms());
        }

        [Fact]
        public void ConstructorCreatesAnotherPlayroomWhenOneGameHasStarted()
        {
            rooms.Join(player1);
            rooms.Join(player2);
            Assert.Contains(player1, rooms.LastAvailablePlayroom.Players);
            Assert.Contains(player2, rooms.LastAvailablePlayroom.Players);
            rooms.LastAvailablePlayroom.GameStartingTime = DateTime.UtcNow;
            rooms.Join(player3);
            Assert.Equal(2, rooms.GetNumberOfPlayrooms());
            Assert.Contains(player3, rooms.LastAvailablePlayroom.Players);
        }
    }
}
