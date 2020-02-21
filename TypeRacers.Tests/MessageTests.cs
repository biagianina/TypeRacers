using System;
using System.Collections.Generic;
using System.Text;
using Xunit;
using Common;

namespace TypeRacersFacts
{
    public class MessageTests
    {
        [Fact]
        public void ReturnCorrectClientInfoGeneratedMessage()
        {
            Message message = new Message("clientinfo", new object[] { "2" , "5", "alin" });

            Assert.Equal("2&5$alin#", message.GetMessage);
        }

        //[Fact]
        //public void ReturnCorrectGameInfoGeneratedMessage()
        //{
        //    Message message = new Message("gameinfo", new object[] { "type this text", DateTime.UtcNow.AddSeconds(15), DateTime.UtcNow.AddSeconds(30), DateTime.UtcNow.AddSeconds(45) });

        //    Assert.Equal("", message.GetMessage);
        //}

        [Fact]
        public void ReturnCorrectOpponentsGeneratedMessage()
        {
            Message message = new Message("clientInfo", new object[] { "2", "5", "alin" });

            Assert.Equal("2&5$alin#", message.GetMessage);
        }
    }
}
