using System;
using System.Collections.Generic;

namespace Server
{
    public class Player
    {
        public Player(string name)
        {
            Name = name;
        }

        public string Name { get; set; }
        public int PlayroomNumber { get; set; }
        public int WPMProgress { get; set; }
        public int CompletedTextPercentage { get; set; }
        public Playroom Playroom { get; set; }

        public void UpdateInfo(int wpmProgress, int completedText, int roomNumber)
        {
            WPMProgress = wpmProgress;
            CompletedTextPercentage = completedText;
            PlayroomNumber = roomNumber;
        }

        //internal void Read()
        //{
        //    throw new NotImplementedException();
        //}

        //internal void SetGameInfo()
        //{
        //    throw new NotImplementedException();
        //}

        //internal void UpdateOpponents()
        //{
        //    throw new NotImplementedException();
        //}
    }
}