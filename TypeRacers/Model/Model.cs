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
        public string GetStartingTime()
        {
            throw new NotImplementedException();
        }
        public void SubscribeToSearchingOpponents(Action<Tuple<List<Tuple<string, Tuple<string, string, int>>>, int>> updateOpponentsAndElapsedTime)

        {
            networkHandler.SubscribeToSearchingOpponentsTimer(updateOpponentsAndElapsedTime);
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
        internal static void NameClient(string username)
        {
            networkHandler.NameClient(username);
        }

        public void RemovePlayer()
        {
            networkHandler.RemovePlayer();
        }
    }
}
