using System.Net.Sockets;
using System.Text;

namespace Common
{
    public class Player
    {
        private NetworkStream networkStream;

        public Player(TcpClient tcpClient)
        {
            TcpClient = tcpClient;
        }

        public bool FirstTimeConnecting = true;
        public string Name { get; set; }
        public int Place { get; set; }
        public bool Restarting { get; set; }
        public bool Removed { get; set; }
        public bool Finnished { get; set; }
        public int WPMProgress { get; set; }
        public int CompletedTextPercentage { get; set; }
        public IPlayroom Playroom { get; set; }
        public TcpClient TcpClient { get;}

        public void SetPlayroom(IPlayroom playroom)
        {
            Playroom = playroom;
        }

        public void UpdateInfo(int wpmProgress, int completedText)
        {
            WPMProgress = wpmProgress;
            CompletedTextPercentage = completedText;
        }

        public string Read()
        {
            if (TcpClient.Connected)
            {
                networkStream = TcpClient.GetStream();
                byte[] buffer = new byte[TcpClient.ReceiveBufferSize];
                int bytesRead = networkStream.Read(buffer, 0, buffer.Length);
                var dataRecieved = Encoding.ASCII.GetString(buffer, 0, bytesRead);
                return dataRecieved.Remove(dataRecieved.Length - 1);
            }
            return string.Empty;
        }

        public void TrySetRank()
        {
            if (CompletedTextPercentage == 100 && !Finnished)
            {
                Finnished = true;
                Place = Playroom.Place++;
            }
        }

        public void Write(IMessage message)
        {
            networkStream = TcpClient.GetStream();
            var toSend = message.ToByteArray();
            networkStream.Write(toSend, 0, toSend.Length);
        }
    }
}