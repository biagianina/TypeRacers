using System;
using System.Collections.Generic;
using System.Text;
using Common;
namespace TypeRacersFacts
{
    public class FakeTypeRacersClient : INetworkClient
    {
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
            return "just a text";
        }

        public void Write(IMessage message)
        {
            throw new NotImplementedException();
        }
    }
}
