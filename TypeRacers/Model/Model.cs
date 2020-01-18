using System;
using System.Collections.Generic;
using System.ComponentModel;
using TypeRacers.Client;

namespace TypeRacers.Model
{
    public class Model
    {
        readonly static NetworkHandler networkHandler = new NetworkHandler();

        public List<Tuple<string, string>> GetOpponents()
        {
            return networkHandler.GetOpponents();
        }
        public void StartSearchingOpponents()
        {
            networkHandler.StartSearchingOpponents();
        }
        public void SubscribeToSearchingOpponents(Action<Tuple<List<Tuple<string, string>>, int>> updateOpponentsAndElapsedTime)
        {
            networkHandler.SubscribeToSearchingOpponentsTimer(updateOpponentsAndElapsedTime);
        }
        public void ReportProgress(int message)
        {
            networkHandler.SendProgressToServer(message.ToString());
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
    }
}
