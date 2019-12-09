using System;
using System.ComponentModel;
using TypeRacers.Client;

namespace TypeRacers.Model
{
    public class Model
    {
        NetworkHandler networkHandler = new NetworkHandler();

        public string GetMessageFromServer()
        {
            return networkHandler.GetMessageFromServer();
        }

        public void ReportProgress(string message)
        {
            networkHandler.SendProgressToServer(message);
        }
    }
}
