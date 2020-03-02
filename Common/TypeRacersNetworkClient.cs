using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    public class TypeRacersNetworkClient : INetworkClient
    {
        // The real implementation
        private NetworkStream networkStream;
        private readonly TcpClient realTcpClient;
        public string DataReceieved { get; set; }

        public NetworkStream GetStream() => realTcpClient.GetStream();

        public TypeRacersNetworkClient()
        {
            realTcpClient = new TcpClient();
        }

        public TypeRacersNetworkClient(TcpClient client)
        {
            realTcpClient = client;
        }

        public void Close()
        {
            realTcpClient.Close();
        }

        public void Dispose()
        {
            realTcpClient.Dispose();
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
                var receivedMessage = new ReceivedMessage();
                receivedMessage.DecodeMessage(networkStream);

                return receivedMessage;
                //byte[] buffer = new byte[1024];
                //int bytesRead = networkStream.Read(buffer, 0, buffer.Length);
                //DataReceieved = Encoding.ASCII.GetString(buffer, 0, bytesRead);

                //while (!DataReceieved.Contains("#"))
                //{
                //    bytesRead = networkStream.Read(buffer, 0, 1024);
                //    DataReceieved += Encoding.ASCII.GetString(buffer, DataReceieved.Length, bytesRead);
                //}
                //string completeMessage = DataReceieved.Substring(0, DataReceieved.IndexOf('#'));
                //DataReceieved.Remove(0, completeMessage.Length - 1);

            }
            return default;
        }
    }
}
