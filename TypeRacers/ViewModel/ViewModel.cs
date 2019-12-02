using System.ComponentModel;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using TypeRacers.Model;

namespace TypeRacers.ModelView
{
    internal class ViewModel : INotifyPropertyChanged
    {
        string text;
        InputCharacterValidation dataValidation;
        bool isValid;
        int spaceIndex;
        Model.Model typeModel;
        

        //creates an instance of the model with the text received from the server
        public ViewModel()
        {
            typeModel = new Model.Model();
            TextToType = typeModel.TextFromServer;
            dataValidation = new InputCharacterValidation(TextToType);
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
                TriggerPropertyChanged(nameof(Color));
                TriggerPropertyChanged(nameof(InputBackgroundColor));
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
           
                if (isValid && value.EndsWith(" "))
                {
                    spaceIndex += text.Length;
                    dataValidation = new InputCharacterValidation(TextToType.Substring(spaceIndex));
                    text = "";
                }

                TriggerPropertyChanged(nameof(CurrentInputText));
            }
        }

        public string Color
        {
            get
            {
                if (string.IsNullOrEmpty(CurrentInputText))
                {                   
                    return "Black";
                }
                if (isValid)
                {
                    return "Green";
                    
                }
                else
                {
                    return "Red";
                }
            }
        }

        public string TextToType { get; }
        public string InputBackgroundColor
        { 
            get
            {
                if (string.IsNullOrEmpty(CurrentInputText))
                {
                    return default;
                }
                if (!isValid)
                {
                    return "Darkred";
                }

                return default;
            }
        }


        public event PropertyChangedEventHandler PropertyChanged;

        public void TriggerPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
