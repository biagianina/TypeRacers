using System;

namespace Common
{
    public interface INetworkClient : IDisposable
    {
        void Write(IMessage message);

        IMessage Read();
    }
}