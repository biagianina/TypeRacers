namespace Common
{
    public interface IPlayer
    {
        string Name { get; set; }
        int Place { get; set; }
        bool Finnished { get; set; }
        int WPMProgress { get; set; }
        int CompletedTextPercentage { get; set; }

        void UpdateInfo(int wpmProgress, int completedText);

        void CheckReceivedData(string dataReceived);

        void Read();

        void Write(bool playerIsNew);

        void SendOpponents();
    }
}