using System;
using System.ComponentModel;
using Client;

namespace TypeRacers.Model
{
    public class Model : INotifyPropertyChanged
    {
        // a class to hold the text to type received from the server
        private string textFromServer;

        public Model()
        {

            TextFromServer = new Client.Client().GetMessageFromServer();

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

            set
            {
                textFromServer = value;
                OnPropertyChanged("Text");
            }
        }
        private void OnPropertyChanged(string text)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(text));
        }


        public event PropertyChangedEventHandler PropertyChanged;
    }
}
