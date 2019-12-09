using System;
using System.ComponentModel;
using TypeRacers.Client;

namespace TypeRacers.Model
{
    public class Model
    {
        NetworkHandler nwCommunication = new NetworkHandler();

        public string GetMessageFromServer()
        {
            return nwCommunication.GetMessageFromServer();
        }

        public void ReportProgress(string message)
        {
            nwCommunication.SendProgressToServer(message);
        }
    }
}
