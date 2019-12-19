using System;
using System.ComponentModel;
using TypeRacers.Client;

namespace TypeRacers.Model
{
    public static class Model
    {
        readonly static NetworkHandler networkHandler = new NetworkHandler();
     
        public static void ReportProgress(string message)
        {
            networkHandler.SendProgressToServer(message);
        }

        internal static void NameClient(string username)
        {
            networkHandler.NameClient(username);
        }
    }
}
