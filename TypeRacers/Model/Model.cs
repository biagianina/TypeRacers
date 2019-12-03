using System;
using System.ComponentModel;
using TypeRacers.Client;

namespace TypeRacers.Model
{
    public class Model
    {
        // a class to hold the data

        //returns the text sent from the server
        public string TextFromServer
        {
            get
            {
                return new TypeRacersClient().GetMessageFromServer();
            }
        }
    }
}
