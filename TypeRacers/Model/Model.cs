using System;
using System.ComponentModel;
using Client;

namespace TypeRacers.Model
{
    public class Model
    {
        // a class to hold the data
        private readonly string textFromServer;

        public Model()
        {
            //creates an instance of the client with the text received from the server
            textFromServer = new Client.Client().GetMessageFromServer();

            if(TextFromServer == null)
            {
                throw new ArgumentNullException("Text not found");
            }
        }

        public string TextFromServer
        {
            get
            {
                return textFromServer;
            }
        }
    }
}
