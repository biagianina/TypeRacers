using System;
using System.Collections.Generic;
using System.ComponentModel;
using TypeRacers.Client;

namespace TypeRacers.Model
{
    public class Model
    {
        readonly static NetworkHandler networkHandler = new NetworkHandler();

        public List<Tuple<string, Tuple<string, string, int>>> GetOpponents()

        {
            return networkHandler.GetOpponents();
        }
        public void StartSearchingOpponents()
        {
            networkHandler.StartSearchingOpponents();
        }

        public void RestartSearch()
        {
            networkHandler.RestartSearch();
        }
        public string GetStartingTime()
        {
            return networkHandler.GetStartingTime();
        }
        public void SubscribeToSearchingOpponents(Action<Tuple<List<Tuple<string, Tuple<string, string, int>>>, int, Dictionary<string, Tuple<bool, int>>>> updateOpponentsAndElapsedTime)
        {
            networkHandler.SubscribeToSearchingOpponentsTimer(updateOpponentsAndElapsedTime);
        }

        public Dictionary<string, Tuple<bool, int>> GetRanking()
        {
            return networkHandler.GetRanking();
        }

        public int GetWaitingTime()
        {
            return networkHandler.GetWaitingTime();
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
        internal static void NameClient(string username)
        {
            networkHandler.NameClient(username);
        }

    }
}
