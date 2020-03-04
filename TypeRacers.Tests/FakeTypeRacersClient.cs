using Common;
using System.Text;

namespace TypeRacersFacts
{
    public class FakeTypeRacersClient : INetworkClient
    {
        public string DataReceieved { get; set; } = "just a text";

        public void Dispose()
        {
            DataReceieved = string.Empty;
        }

        public IMessage Read()
        {
            return new ReceivedMessage(DataReceieved);
        }

        public void Write(IMessage message)
        {
            var data = message.ToByteArray();
            DataReceieved = Encoding.ASCII.GetString(data, 0, data.Length);
        }
    }
}