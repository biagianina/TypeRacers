using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using TypeRacers.Model;
using TypeRacers.ViewModel;

namespace TypeRacers.ViewModel
{
    //a class to hold the logic
    internal class ViewModel : INotifyPropertyChanged
    {
        string text = "";
        InputCharacterValidation dataValidation;
        bool isValid;
        int spaceIndex;
        readonly Model.Model model;
        int correctChars;
        int incorrectChars;

        public ViewModel()
        {
            model = new Model.Model();
            TextToType = model.TextFromServer;
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
                TriggerPropertyChanged(nameof(IsValid));
                TriggerPropertyChanged(nameof(InputBackgroundColor));
            }
        }
        public IEnumerable<Inline> Inlines
        {
            get => new[] { new Run() { Text = TextToType.Substring(0, spaceIndex) , Foreground = Brushes.Green},
                new Run() { Text = TextToType.Substring(spaceIndex, correctChars) , Foreground = Brushes.Green},
                new Run() { Text = TextToType.Substring(correctChars + spaceIndex, incorrectChars) , Background = Brushes.IndianRed},
                new Run() { Text = TextToType.Substring(correctChars + incorrectChars + spaceIndex) , Foreground = Brushes.Black}
                };
        }


        public string CurrentInputText
        {
            get => text;
            set
            {
                // return because we dont need to execute logic if the input text has not changed
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

                HighlightText();


                if (spaceIndex + text.Length == TextToType.Length && isValid)
                {
                    MessageBox.Show("Congrats!");
                }

                TriggerPropertyChanged(nameof(CurrentInputText));
            }
        }

        private void HighlightText()
        {
            if (!Keyboard.IsKeyDown(Key.Back))
            {
                if (isValid)
                {
                    correctChars = text.Length;
                    incorrectChars = 0;
                }

                if (!isValid)
                {
                    incorrectChars++;
                }

            }
            else
            {
                if (!isValid && !string.IsNullOrEmpty(text))
                {
                    incorrectChars--;
                }
                else
                {
                    incorrectChars = 0;
                }
            }

            TriggerPropertyChanged(nameof(Inlines));
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
                    return "IndianRed";
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
