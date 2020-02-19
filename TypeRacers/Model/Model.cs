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
            networkHandler = new NetworkHandler();
            networkHandler.NameClient(MainViewModel.Name);
        }

        public List<Tuple<string, Tuple<string, string, int>>> GetOpponents()
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

        public void StartServerCommunication()
        {
            networkHandler.StartServerCommunication();
        }
        public void GetTextToType()
        {
            networkHandler.GetTextToType();
        }

        public Dictionary<string, Tuple<bool, int>> GetRanking()
        {
            return networkHandler.GetRanking();
        }

        public DateTime GetWaitingTime()
        {
            return networkHandler.GetWaitingTime();
        }
        public void SubscribeToSearchingOpponents(Action<Tuple<List<Tuple<string, Tuple<string, string, int>>>, Dictionary<string, Tuple<bool, int>>>> updateOpponents)
        {
            networkHandler.SubscribeToSearchingOpponents(updateOpponents);
        }

        public void ReportProgress(int progress, int sliderProgress)
        {
            string message = progress + "&" + sliderProgress;
            networkHandler.SendProgressToServer(message);
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

        internal void StartGameProgressReporting()
        {
            networkHandler.StartReportingGameProgress();
        }
    }
}