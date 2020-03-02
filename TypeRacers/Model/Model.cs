using System;
using System.Collections.Generic;
using TypeRacers.ViewModel;

namespace TypeRacers.Model
{
    public class Model
    {
        private readonly NetworkHandler networkHandler;
        private string textToType = LocalGeneratedText.GetText();

        public Model()
        {
            networkHandler = new NetworkHandler(MainViewModel.Name);
        }

        public void StartCommunication()
        {
            networkHandler.StartCommunication();
        }

        public Common.Player GetPlayerModel()
        {
            return networkHandler.PlayerModel();
        }

        public Common.IPlayroom GetGameModel()
        {
            return networkHandler.GameModel();
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
            return textToType;
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