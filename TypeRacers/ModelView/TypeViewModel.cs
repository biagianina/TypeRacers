using System.ComponentModel;
using TypeRacers.Model;

namespace TypeRacers.ModelView
{
    internal class TypeViewModel : INotifyPropertyChanged
    {
        private string text;
        string originalText;
        DataValidation dataValidation;
        bool isValid;

        //creates an instance of the model with the text received from the server
        public TypeViewModel()
        {
            Model = new TypeModel(new Client.Client().GetMessageFromServer());
            originalText = Model.TextFromServer;
            dataValidation = new DataValidation(originalText);
        }

        public bool IsValid
        {
            get => isValid;

            set
            {
                if (isValid == value)
                    return;

                isValid = value;
                TriggerPropertyChanged(nameof(isValid));
            }
        }

        public string CurrentInputText
        {
            get => text;
            set
            {
                if (text == value)
                    return;

                text = value;

                //validate current word
                IsValid = dataValidation.ValidateWord(CurrentInputText, CurrentInputText.Length);
                TriggerPropertyChanged(nameof(CurrentInputText));
            }

        }

        public TypeModel Model { get; }

        public event PropertyChangedEventHandler PropertyChanged;

        public void TriggerPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
