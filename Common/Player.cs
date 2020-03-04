namespace Common
{
    public class Player
    {
        public Player(INetworkClient tcpClient)
        {
            NetworkClient = tcpClient;
        }

        public bool FirstTimeConnecting { get; set; } = true;
        public string Name { get; set; }
        public int Place { get; set; }
        public bool Restarting { get; set; }
        public bool Removed { get; set; }
        public bool Finnished { get; set; }
        public int WPMProgress { get; set; }
        public int CompletedTextPercentage { get; set; }
        public IPlayroom Playroom { get; set; }
        public INetworkClient NetworkClient { get; }

        public void SetPlayroom(IPlayroom playroom)
        {
            Playroom = playroom;
        }

        public void UpdateInfo(int wpmProgress, int completedText)
        {
            WPMProgress = wpmProgress;
            CompletedTextPercentage = completedText;
        }

        public void TrySetRank()
        {
            if (CompletedTextPercentage == 100 && !Finnished)
            {
                Finnished = true;
                Place = Playroom.Place++;
            }
        }

        public IMessage Read()
        {
            return NetworkClient.Read();
        }

        public void Write(IMessage message)
        {
            NetworkClient.Write(message);
        }
    }
}