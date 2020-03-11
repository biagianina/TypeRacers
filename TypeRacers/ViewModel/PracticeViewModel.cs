using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;

namespace TypeRacers.ViewModel
{
    public class PracticeViewModel : ITextToType, INotifyPropertyChanged
    {
        private string typedText=string.Empty;
        private string textToType = string.Empty;
        private InputCharacterValidation userInputValidator;

        public PracticeViewModel()
        {
            StartTime = DateTime.UtcNow.AddSeconds(5);
            EndTime = StartTime.AddSeconds(90);
            SecondsToGetReady = (StartTime - DateTime.UtcNow).Seconds.ToString();
            GetReadyAlert = true;
            TriggerPropertyChanged(nameof(GetReadyAlert));
        }

        public IEnumerable<Inline> TextToTypeStyles => UserInputValidator.TextToTypeStyles;
        private InputCharacterValidation UserInputValidator { get => userInputValidator ?? new InputCharacterValidation(TextToType); set => userInputValidator = value; }

        public int SliderProgress
        {
            get
            {
                if (UserInputValidator.AllTextTyped || TextToType.Length == 0)
                {
                    return 100;
                }

                return UserInputValidator.SpaceIndex * 100 / TextToType.Length;
            }
        }

        public int WPMProgress
        {
            get
            {
                if (UserInputValidator.CurrentWordIndex == 0)
                {
                    return 0;
                }

                return (UserInputValidator.NumberOfCharactersTyped / 5) * 60 / ((int)(DateTime.UtcNow - StartTime).TotalSeconds);
            }
        }

        public bool AllTextTyped => UserInputValidator.AllTextTyped;

        //determines if a popup alert should apear, bindedin open property of popup xaml
        public bool TypingAlert => UserInputValidator.TypingAlert;

        public string InputBackgroundColor => UserInputValidator.InputBackgroundColor;

        public string TextToType
        {
            get => textToType;
            set
            {
                textToType = value;
                TriggerPropertyChanged(nameof(TextToType));
                TriggerPropertyChanged(nameof(SliderProgress));
                TriggerPropertyChanged(nameof(TextToTypeStyles));
                TriggerPropertyChanged(nameof(UserInputValidator));
            }
        }

        public string CurrentInputText
        {
            get => typedText;
            set
            {
                if (typedText == value)
                    return;
                
                if (userInputValidator == null)
                {
                    UserInputValidator = new InputCharacterValidation(TextToType);
                    TriggerPropertyChanged(nameof(UserInputValidator));
                }

                UserInputValidator.ValidateInput(value);
              
                TriggerPropertyChanged(nameof(TextToTypeStyles));
                TriggerPropertyChanged(nameof(CurrentInputText));
                TriggerPropertyChanged(nameof(Accuracy));
                TriggerPropertyChanged(nameof(ShowFinishResults));
                TriggerPropertyChanged(nameof(SliderProgress));
                TriggerPropertyChanged(nameof(WPMProgress));
                TriggerPropertyChanged(nameof(InputBackgroundColor));
                TriggerPropertyChanged(nameof(AllTextTyped));
                TriggerPropertyChanged(nameof(TypingAlert));

                typedText = UserInputValidator.CurrentInputText;
            }
        }

        public bool CanUserType { get; internal set; }
        public string SecondsInGame { get; internal set; } = "90 seconds";
        public bool GetReadyAlert { get; internal set; }
        public string SecondsToGetReady { get; internal set; }
        public int Accuracy => UserInputValidator?.Accuracy ?? 0;
        public bool ShowFinishResults => UserInputValidator?.OpenFinishPopup ?? false;
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
     
        //INotifyPropertyChanged code - basic
        public event PropertyChangedEventHandler PropertyChanged;

        public void TriggerPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}