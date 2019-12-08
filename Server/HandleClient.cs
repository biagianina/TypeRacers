using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace TypeRacers.Server
{
    public class HandleClient
    {
        TcpClient clientSocket;
        public void StartClient(TcpClient inClientSocket)
        {
            clientSocket = inClientSocket;
            DoChat();
        }

        //text is sent to client
        private void DoChat()
        {
            try
            {
                NetworkStream networkStream = clientSocket.GetStream();
                byte[] broadcastBytes = Encoding.ASCII.GetBytes(new ContestText().GetText()); //generates random text from text document
                networkStream.Write(broadcastBytes, 0, broadcastBytes.Length);//send the text to connected client
            }
            catch (Exception)
            {
                throw new Exception("unable to connect");
            }
        }
    }
}
