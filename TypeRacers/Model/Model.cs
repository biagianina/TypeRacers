using System;
using System.Collections.Generic;
using System.ComponentModel;
using TypeRacers.Client;

namespace TypeRacers.Model
{
    public static class Model
    {
        readonly static NetworkHandler networkHandler = new NetworkHandler();

        public static List<Tuple<string, string>> GetOpponents()
        {
            return networkHandler.GetOpponents();
        }
        public static void StartSearchingOpponents()
        {
            networkHandler.StartSearchingOpponents();
        }
        public static void SubscribeToSearchingOpponents(Action<List<Tuple<string, string>>> updateFunction)
        {
            networkHandler.SubscribeToSearchingOpponentsTimer(updateFunction);
        }
        public static void ReportProgress(int message)
        {
            networkHandler.SendProgressToServer(message.ToString());
        }
        public static string GetGeneratedTextToTypeLocally()
        {
            return LocalGeneratedText.GetText();
        }

        public static string GetGeneratedTextToTypeFromServer()
        {
            return networkHandler.GetTextFromServer();
        }
        internal static void NameClient(string username)
        {
            networkHandler.NameClient(username);
        }
    }
}
