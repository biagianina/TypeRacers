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
        private string textToType;
        private bool isValid;
        private int spaceIndex;
        private int correctChars;
        private int incorrectChars;
        private int currentWordIndex;
        private bool alert;
        private int numberOfCharactersTyped;
        private int correctTyping;
        private int incorrectTyping;
        private InputCharacterValidation userInputValidator;

        public PracticeViewModel()
        {
            StartTime = DateTime.UtcNow.AddSeconds(5);
            EndTime = StartTime.AddSeconds(90);
            SecondsToGetReady = (StartTime - DateTime.UtcNow).Seconds.ToString();
            GetReadyAlert = true;
            TriggerPropertyChanged(nameof(GetReadyAlert));
        }

        public IEnumerable<Inline> TextToTypeStyles
        {
            get => new[] { new Run() { Text = TextToType.Substring(0, spaceIndex) , Foreground = Brushes.Salmon},
                new Run() { Text = TextToType.Substring(spaceIndex, correctChars), Foreground = Brushes.Salmon, TextDecorations = TextDecorations.Underline},
                new Run() { Text = TextToType.Substring(correctChars + spaceIndex, incorrectChars), TextDecorations = TextDecorations.Underline, Background = Brushes.Salmon},
                new Run() {Text = TextToType.Substring(spaceIndex + correctChars + incorrectChars, CurrentWordLength - correctChars - incorrectChars), TextDecorations = TextDecorations.Underline},
                new Run() {Text = TextToType.Substring(spaceIndex + CurrentWordLength) }
                };
        }

        private InputCharacterValidation UserInputValidator { get => userInputValidator ?? new InputCharacterValidation(TextToType); set => userInputValidator = value; }

        public bool ValidateInput
        {
            get => isValid;

            set
            {
                if (isValid == value)
                    return;

                isValid = value;
                TriggerPropertyChanged(nameof(ValidateInput));
                TriggerPropertyChanged(nameof(InputBackgroundColor));
            }
        }

        public int SliderProgress
        {
            get
            {
                if (AllTextTyped || TextToType.Length == 0)
                {
                    return 100;
                }

                return spaceIndex * 100 / TextToType.Length;
            }
        }

        public int WPMProgress
        {
            get
            {
                if (currentWordIndex == 0)
                {
                    return 0;
                }

                return (numberOfCharactersTyped / 5) * 60 / ((int)(DateTime.UtcNow - StartTime).TotalSeconds);
            }
        }

        public int CurrentWordLength
        {
            get => TextToType.Split()[currentWordIndex].Length;//length of current word
        }

        public bool AllTextTyped { get; set; }

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

        public string InputBackgroundColor
        {
            get
            {
                if (string.IsNullOrEmpty(CurrentInputText))
                {
                    return default;
                }
                if (!ValidateInput)
                {
                    return "Salmon";
                }

                return default;
            }
        }

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
            get => textToType;
            set
            {
                // return because we dont need to execute logic if the input text has not changed
                if (textToType == value)
                    return;

                textToType = value;

                //validate current word
               // ValidateInput = UserInputValidator.ValidateWord(CurrentInputText, CurrentInputText.Length);

                CheckUserInput(textToType);

                TriggerPropertyChanged(nameof(CurrentWordLength));//moves to next word

                //determine number of characters that are valid/invalid to form substrings
                HighlightText();

                TriggerPropertyChanged(nameof(CurrentInputText));
            }
        }

        public bool CanUserType { get; internal set; }
        public string SecondsInGame { get; internal set; } = "90 seconds";
        public bool GetReadyAlert { get; internal set; }
        public string SecondsToGetReady { get; internal set; }
        public int Accuracy { get; private set; }
        public bool ShowFinishResults { get; private set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }

        private void CheckUserInput(string value)
        {
            //checks if current word is typed, clears textbox, reintializes remaining text to the validation, sends progress
            CheckIfInputIsCompleteWord(value);
            //checks if current word is the last one
            CheckIfInputIsLastWord();
        }

        private void CheckIfInputIsLastWord()
        {
            if (ValidateInput && textToType.Length + spaceIndex == TextToType.Length)
            {
                AllTextTyped = true;
                TriggerPropertyChanged(nameof(AllTextTyped));

                EndTime = DateTime.UtcNow;
                TriggerPropertyChanged(nameof(EndTime));

                ShowFinishResults = true;
                TriggerPropertyChanged(nameof(ShowFinishResults));

                Accuracy = 100 - (incorrectTyping * 100 / correctTyping);
                TriggerPropertyChanged(nameof(Accuracy));

                TriggerPropertyChanged(nameof(WPMProgress));
                TriggerPropertyChanged(nameof(SliderProgress));
            }
        }

        private void CheckIfInputIsCompleteWord(string value)
        {
            if (ValidateInput && value.EndsWith(" "))
            {
                spaceIndex += textToType.Length;

                if (currentWordIndex < TextToType.Split().Length - 1)
                {
                    currentWordIndex++;
                }

                userInputValidator = new InputCharacterValidation(TextToType.Substring(spaceIndex));
                numberOfCharactersTyped += CurrentInputText.Length;
                textToType = string.Empty;
                TriggerPropertyChanged(nameof(SliderProgress));
                TriggerPropertyChanged(nameof(WPMProgress));//recalculates progress
            }
        }

        private void HighlightText()
        {
            if (!Keyboard.IsKeyDown(Key.Back))
            {
                if (ValidateInput)
                {
                    TypingAlert = false;
                    correctChars = textToType.Length;
                    correctTyping++;
                    incorrectChars = 0;
                }

                if (!ValidateInput)
                {
                    incorrectTyping++;
                    incorrectChars++;
                    if (CurrentWordLength - correctChars - incorrectChars < 0)
                    {
                        TypingAlert = true;
                        textToType = textToType.Substring(0, correctChars);
                        incorrectChars = 0;
                    }
                }
            }
            else
            {
                if (!ValidateInput && !string.IsNullOrEmpty(textToType))
                {
                    incorrectChars--;
                }
                else
                {
                    TypingAlert = false;
                    correctChars = textToType.Length;
                    incorrectChars = 0;
                }
            }

            TriggerPropertyChanged(nameof(TextToTypeStyles)); //new Inlines formed at each char in input
        }

        //INotifyPropertyChanged code - basic
        public event PropertyChangedEventHandler PropertyChanged;

        public void TriggerPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}