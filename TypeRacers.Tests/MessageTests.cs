using System.Text;
using TypeRacers.Client;
using Xunit;

namespace TypeRacersFacts
{
    public class MessageTests
    {
        [Fact]
        public void ReturnsCorrectSimplePlayerMessage()
        {
            var byteArray = new PlayerMessage(0, 0, "Alin", true, false, false).ToByteArray();

            var generatedMessage = Encoding.ASCII.GetString(byteArray, 0, byteArray.Length);

            Assert.Equal("0&0&True$Alin#", generatedMessage);
        }

        [Fact]
        public void ReturnsCorrectRestartedPlayerMessage()
        {
            var byteArray = new PlayerMessage(0, 0, "Alin", true, true, false).ToByteArray();

            var generatedMessage = Encoding.ASCII.GetString(byteArray, 0, byteArray.Length);

            Assert.Equal("0&0&True$Alin_restart#", generatedMessage);
        }

        [Fact]
        public void ReturnsCorrectRemovedPlayerMessage()
        {
            var byteArray = new PlayerMessage(0, 0, "Alin", true, false, true).ToByteArray();

            var generatedMessage = Encoding.ASCII.GetString(byteArray, 0, byteArray.Length);

            Assert.Equal("0&0&True$Alin_removed#", generatedMessage);
        }
    }
}