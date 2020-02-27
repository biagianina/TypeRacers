using System;
using System.Collections.Generic;
using TypeRacers.ViewModel;

namespace TypeRacers.Model
{
    public class Model
    {
        private readonly NetworkHandler networkHandler;

        public Model()
        {
            networkHandler = new NetworkHandler(MainViewModel.Name);
        }

        public List<Common.Player> GetOpponents()
        {
            return networkHandler.GetOpponents();
        }

        public void RestartSearch()
        {
            networkHandler.RestartSearch();
        }

        public DateTime GetStartingTime()
        {
            return networkHandler.GetStartingTime();
        }

        public DateTime GetEndingTime()
        {
            return networkHandler.GetEndingTime();
        }

        //public Dictionary<string, Tuple<bool, int>> GetRanking()
        //{
        //    return networkHandler.GetRanking();
        //}

        public DateTime GetWaitingTime()
        {
            return networkHandler.GetWaitingTime();
        }

        public void SubscribeToSearchingOpponents(Action<List<Common.Player>> updateOpponents)
        {
            networkHandler.SubscribeToSearchingOpponents(updateOpponents);
        }

        public void ReportProgress(int wpmProgress, int completedTextPercentageProgress)
        {
            networkHandler.SendProgressToServer(wpmProgress, completedTextPercentageProgress);
        }

        public string GetGeneratedTextToTypeLocally()
        {
            return LocalGeneratedText.GetText();
        }

        public string GetGeneratedTextToTypeFromServer()
        {
            return networkHandler.GetTextFromServer();
        }

        public void RemovePlayer()
        {
            networkHandler.RemovePlayer();
        }

        internal bool PlayerFinnished()
        {
            return networkHandler.PlayerFinnished();
        }

        internal string PlayerPlace()
        {
            return networkHandler.PlayerPlace();
        }
    }
}