namespace Server
{
    internal class Player
    {
        public Player(string name)
        {
            Name = name;
        }

        public string Name { get; set; }
        public int PlayroomNumber { get; set; }
        public int WPMProgress { get; set; }
        public int CompletedTextPercentage { get; set; }

        public void UpdateInfo(int wpmProgress, int completedText, int roomNumber)
        {
            WPMProgress = wpmProgress;
            CompletedTextPercentage = completedText;
            PlayroomNumber = roomNumber;
        }
    }
}