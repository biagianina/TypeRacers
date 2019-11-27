using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace echo
{
    public class HandleClient
    {
        TcpClient clientSocket;
        public void StartClient(TcpClient inClientSocket)
        {
            this.clientSocket = inClientSocket;
            DoChat();
        }

        //text is sent to client
        private void DoChat()
        {
            try
            {
                NetworkStream networkStream = clientSocket.GetStream();
                byte[] broadcastBytes = null;
                
                broadcastBytes = Encoding.ASCII.GetBytes(new ContestText().GetText()); //generates random text from text document
                networkStream.Write(broadcastBytes, 0, broadcastBytes.Length);
                networkStream.Flush();
            }
            catch (Exception)
            {
                throw new Exception("unable to connect");
            }
        }
    }
}
