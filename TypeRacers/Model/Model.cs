using System;
using System.ComponentModel;
using TypeRacers.Client;

namespace TypeRacers.Model
{
    public class Model
    {
        readonly NetworkHandler networkHandler = new NetworkHandler();

        public static void ReportProgressToServer(string message)
        {
            ////networkHandler.SendProgressToServer(message);
        }
        public static string GetGeneratedTextToTypeLocally()
        {
            return LocalGeneratedText.GetText();
        }

        public static string GetGeneratedTextToTypeFromServer()
        {
            throw new NotImplementedException();
        }
    }
}
