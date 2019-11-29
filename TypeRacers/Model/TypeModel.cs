using System.ComponentModel;
using Client;

namespace TypeRacers.Model
{
    public class TypeModel : INotifyPropertyChanged 
    {
        // a class to hold the text to type received from the server
        private string _text;

        public TypeModel(string text)
        {
            TextFromServer = text;
        }

        public string TextFromServer
        {
            get
            {
                return _text;
            }

            set
            {
                _text = value;
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
