using System.Net.Sockets;

namespace Common
{
    public class TypeRacersNetworkClient : INetworkClient
    {
        // The real implementation
        private NetworkStream networkStream;

        private readonly TcpClient realTcpClient;

        public TypeRacersNetworkClient()
        {
            realTcpClient = new TcpClient();
        }

        public TypeRacersNetworkClient(TcpClient client)
        {
            realTcpClient = client;
        }

        public void Dispose()
        {
            networkStream.Dispose();
            realTcpClient.Dispose();
            realTcpClient.Close();
        }

        public void Write(IMessage message)
        {
            networkStream = realTcpClient.GetStream();
            var toSend = message.ToByteArray();
            networkStream.Write(toSend, 0, toSend.Length);
        }

        public IMessage Read()
        {
            if (realTcpClient.Connected)
            {
                networkStream = realTcpClient.GetStream();
                return new ReceivedMessage(realTcpClient);
            }
            return default;
        }
    }
}