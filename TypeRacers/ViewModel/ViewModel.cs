using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;

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
        string progress ="";

        int currentWordIndex;
        private bool alert = false;

        public ViewModel()
        {
            model = new Model.Model();
            TextToType = model.GetMessageFromServer();
            dataValidation = new InputCharacterValidation(TextToType);
        }

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

        //property that uses the extensions from TextBlockExtensions class
        //(https://stackoverflow.com/questions/10623850/text-from-code-code-behind-into-textblock-with-different-font-style
        //    this would be the code behind, using extensions binded to xaml you do the same but in ViewModel)
        public IEnumerable<Inline> Inlines
        {
            get => new[] { new Run() { Text = TextToType.Substring(0, spaceIndex) , Foreground = Brushes.Green},
                new Run() { Text = TextToType.Substring(spaceIndex, correctChars), Foreground = Brushes.Green, TextDecorations = TextDecorations.Underline},
                new Run() { Text = TextToType.Substring(correctChars + spaceIndex, incorrectChars) ,TextDecorations = TextDecorations.Underline, Background = Brushes.IndianRed},
                new Run() {Text = TextToType.Substring(spaceIndex + correctChars + incorrectChars, CurrentWordLength - correctChars - incorrectChars), TextDecorations = TextDecorations.Underline},
                new Run() {Text = TextToType.Substring(spaceIndex + CurrentWordLength) }
                };
        }

        public string Progress
        {
            get
            {
                return progress = (spaceIndex * 100 / TextToType.Length).ToString() + "%";
            }
        }

        public int CurrentWordLength
        {
            get => TextToType.Split()[currentWordIndex].Length;
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

                //clears at space, holds space indexes, initialize validation with the substring remained after typing some valid characters/words
                if (isValid && value.EndsWith(" ") || isValid && text.Length + spaceIndex == TextToType.Length)
                {
                    spaceIndex += text.Length;
                    
                    TriggerPropertyChanged(nameof(Progress));
                    if (currentWordIndex < TextToType.Split().Length - 1)
                    {
                        currentWordIndex++;
                    }

                    dataValidation = new InputCharacterValidation(TextToType.Substring(spaceIndex));
                    text = "";

                    ReportProgress();
                    
                }
                
                if (spaceIndex == TextToType.Length)
                {
                    AllTextTyped = true;
                    TriggerPropertyChanged(nameof(AllTextTyped));

                }

                TriggerPropertyChanged(nameof(CurrentWordLength));
                //determine number o characters taht are valid/invalid to form substrings
                HighlightText();

                //makes textbox readonly when all text is typed

                if (spaceIndex == TextToType.Length)
                {
                    AllTextTyped = true;
                    TriggerPropertyChanged(nameof(AllTextTyped));
                }


                TriggerPropertyChanged(nameof(CurrentInputText));
            }
        }

        private void ReportProgress()
        {
            model.ReportProgress(progress);
        }

        public bool AllTextTyped { get; private set; }

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
