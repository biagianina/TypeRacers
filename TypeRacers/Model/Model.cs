using System;
using System.ComponentModel;
using TypeRacers.Client;

namespace TypeRacers.Model
{
    public class Model
    {
        //readonly NetworkHandler networkHandler = new NetworkHandler();
     
        public void ReportProgress(string message)
        {
            ////networkHandler.SendProgressToServer(message);
        }

        public string GetGeneratedTextToTypeLocally()
        {
            return LocalGeneratedText.GetText();
        }

        public string GetGeneratedTextToTypeFromServer()
        {
            throw new NotImplementedException();
        }
    }
}
