using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    public interface INetworkClient : IDisposable
    {
        void Write(IMessage message);
        string Read();
        void Close();
    }
}
