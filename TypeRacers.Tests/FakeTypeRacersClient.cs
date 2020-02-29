using System;
using System.Collections.Generic;
using System.Text;
using Common;
namespace TypeRacersFacts
{
    public class FakeTypeRacersClient : INetworkClient
    {
        public string DataReceieved { get; set; } = "just a text";
            
        public void Close()
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public string Read()
        {
            return DataReceieved;
        }

        public void Write(IMessage message)
        {
            var data = message.ToByteArray();
            DataReceieved = Encoding.ASCII.GetString(data, 0, data.Length);
        }
    }
}
