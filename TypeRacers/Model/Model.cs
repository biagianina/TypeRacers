using System;
using System.ComponentModel;
using TypeRacers.Client;

namespace TypeRacers.Model
{
    public static class Model
    {
        readonly static NetworkHandler networkHandler = new NetworkHandler();

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
