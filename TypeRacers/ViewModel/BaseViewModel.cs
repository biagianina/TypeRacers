using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;

namespace TypeRacers.ViewModel
{
    //a class to hold the base logic
    internal class BaseViewModel : INotifyPropertyChanged
    {
        string text = "";
        InputCharacterValidation userInputValidator;
        bool isValid;
        int spaceIndex;
        int correctChars;
        int incorrectChars;
        int currentWordIndex;
        private bool alert = false;
        
        public BaseViewModel()
        {
            TextToType = new ContestText().GetText();
            userInputValidator = new InputCharacterValidation(TextToType);
        }
      
        //property that uses the extensions from TextBlockExtensions class
        //(https://stackoverflow.com/questions/10623850/text-from-code-code-behind-into-textblock-with-different-font-style
        //    this would be the code behind, using extensions binded to xaml you do the same but in ViewModel)        
        public IEnumerable<Inline> Inlines
        {
            get => new[] { new Run() { Text = TextToType.Substring(0, spaceIndex) , Foreground = Brushes.Green},
                new Run() { Text = TextToType.Substring(spaceIndex, correctChars), Foreground = Brushes.Green, TextDecorations = TextDecorations.Underline},
                new Run() { Text = TextToType.Substring(correctChars + spaceIndex, incorrectChars), TextDecorations = TextDecorations.Underline, Background = Brushes.IndianRed},
                new Run() {Text = TextToType.Substring(spaceIndex + correctChars + incorrectChars, CurrentWordLength - correctChars - incorrectChars), TextDecorations = TextDecorations.Underline},
                new Run() {Text = TextToType.Substring(spaceIndex + CurrentWordLength) }
                };
        }

        //holds the value of user input validation
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

        public string Progress
        {
            get
            {
                if (AllTextTyped)
                {
                    return "100%";
                }

                return (spaceIndex * 100 / TextToType.Length).ToString() + "%";
            }
        }

        public int CurrentWordLength
        {
            get => TextToType.Split()[currentWordIndex].Length;//length of current word
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
                IsValid = userInputValidator.ValidateWord(CurrentInputText, CurrentInputText.Length);
                
                CheckUserInput(text);

                TriggerPropertyChanged(nameof(CurrentWordLength));//moves to next word
               
                //determine number of characters that are valid/invalid to form substrings
                HighlightText();

                TriggerPropertyChanged(nameof(CurrentInputText));
            }
        }

        private void CheckUserInput(string value)
        {
            //checks if current word is typed, clears textbox, reintializes remaining text to the validation, sends progress 
            if (isValid && value.EndsWith(" "))
            {
                spaceIndex += text.Length;

                if (currentWordIndex < TextToType.Split().Length - 1)
                {
                    currentWordIndex++;
                }

                userInputValidator = new InputCharacterValidation(TextToType.Substring(spaceIndex));
                text = "";
                TriggerPropertyChanged(nameof(Progress));//recalculates progress 
                ReportProgress();
            }
            //checks if current word is the last one
            if (IsValid && text.Length + spaceIndex == TextToType.Length)
            {
                AllTextTyped = true;
                TriggerPropertyChanged(nameof(AllTextTyped));
                TriggerPropertyChanged(nameof(Progress));//recalculates progress 
                ReportProgress();
            }
        }

        public void ReportProgress()
        {
            Model.Model.ReportProgress(Progress);
        }

        public bool AllTextTyped { get; private set; }

        private void HighlightText()
        {
            if (!Keyboard.IsKeyDown(Key.Back))
            {
                if (isValid)
                {
                    TypingAlert = false;
                    correctChars = text.Length;
                    incorrectChars = 0;
                }

                if (!isValid)
                {
                    incorrectChars++;
                    if (CurrentWordLength - correctChars - incorrectChars < 0)
                    {
                        TypingAlert = true;
                        text = text.Substring(0, correctChars);
                        incorrectChars = 0;
                    }
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
                    TypingAlert = false;
                    correctChars = text.Length;
                    incorrectChars = 0;
                }
            }

             TriggerPropertyChanged(nameof(Inlines)); //new Inlines formed at each char in input
        }

        public string TextToType { get; }

        //determines if a popup alert should apear, bindedin open property of popup xaml
        public bool TypingAlert
        {
            get => alert;

            set
            {
                if (alert == value)
                {
                    return;
                }

                alert = value;
                TriggerPropertyChanged(nameof(TypingAlert));
            }
        }

        //property to color the background of the input textbox when invalid char is typed
        //binded in the input textbox (how to found in the tutorial WPF MVVM Step by Step (chanel .Net Interview Preparation)
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

        //INotifyPropertyChanged code - basic 
        public event PropertyChangedEventHandler PropertyChanged;

        public void TriggerPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
