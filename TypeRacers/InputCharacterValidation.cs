using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;

namespace TypeRacers

{
    public class InputCharacterValidation : INotifyPropertyChanged
    {
        private readonly string originalText = string.Empty;
        private string remainingText = string.Empty;
        private bool isValid;
        private int correctChars;
        private int incorrectChars;
        private int incorrectTyping;
        private int correctTyping;
        private string textTyped = string.Empty;
        private bool alert;

        public InputCharacterValidation(string gameText)
        {
            originalText = gameText;
            remainingText = gameText;
        }

        public IEnumerable<Inline> TextToTypeStyles
        {
            get => new[] { new Run() { Text = originalText.Substring(0, SpaceIndex) , Foreground = Brushes.Salmon},
                new Run() {Text = originalText.Substring(SpaceIndex, correctChars), Foreground = Brushes.Salmon, TextDecorations = TextDecorations.Underline},
                new Run() {Text = originalText.Substring(correctChars + SpaceIndex, incorrectChars), TextDecorations = TextDecorations.Underline, Background = Brushes.Salmon},
                new Run() {Text = originalText.Substring(SpaceIndex + correctChars + incorrectChars, CurrentWordLength - correctChars - incorrectChars), TextDecorations = TextDecorations.Underline},
                new Run() {Text = originalText.Substring(SpaceIndex + CurrentWordLength) }
                };
        }

        public int CurrentWordLength
        {
            get => originalText.Split()[CurrentWordIndex].Length;//length of current word
        }

        public bool IsValid
        {
            get => isValid;
            set
            {
                isValid = value;
                TriggerPropertyChanged(nameof(IsValid));
                TriggerPropertyChanged(nameof(InputBackgroundColor));
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
                if (!isValid)
                {
                    return "Salmon";
                }

                return default;
            }
        }

        public string CurrentInputText
        {
            get => textTyped;
            set
            {
                // return because we dont need to execute logic if the input text has not changed
                if (textTyped.Equals(value))
                    return;

                textTyped = value;
                TriggerPropertyChanged(nameof(CurrentInputText));
                Clear = false;
                TriggerPropertyChanged(nameof(Clear));
            }
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
        public bool AllTextTyped { get; private set; }
        public int Accuracy { get; private set; }
        public bool OpenFinishPopup { get; private set; }
        public bool Clear { get; set; }
        public int SpaceIndex { get; private set; }
        public int NumberOfCharactersTyped { get; set; }
        public int CurrentWordIndex { get; private set; }

        //validate only one word
        public void ValidateInput(string input)
        {
            CurrentInputText = input;
            bool charIndexIsInRange = CurrentInputText.Length != -1 && CurrentInputText.Length <= originalText.Length;

            if (!charIndexIsInRange)
            {
                isValid = false;
                return;
            }
            else 
            {
                string substringToCheck = remainingText.Substring(0, CurrentInputText.Length);
                IsValid = substringToCheck.Equals(CurrentInputText);
                CheckIfInputIsCompleteWord();
                CheckIfIsLastWord();
                HighlightText();
            }
        }

        private void CheckIfInputIsCompleteWord()
        {
            if (IsValid && CurrentInputText.EndsWith(" "))
            {
                SpaceIndex += CurrentInputText.Length;
                TriggerPropertyChanged(nameof(SpaceIndex));

                UpdateCurrentWordIndex();

                remainingText = originalText.Substring(SpaceIndex);
                NumberOfCharactersTyped += CurrentInputText.Length;
                TriggerPropertyChanged(nameof(NumberOfCharactersTyped));
                CurrentInputText = string.Empty;
                Clear = true;
                TriggerPropertyChanged(nameof(Clear));
            }
        }

        private void UpdateCurrentWordIndex()
        {
            if (CurrentWordIndex < originalText.Split().Length - 1)
            {
                CurrentWordIndex++;
            }
        }

        private void CheckIfIsLastWord()
        {
            if (IsValid && textTyped.Length + SpaceIndex == originalText.Length)
            {
                AllTextTyped = true;
                TriggerPropertyChanged(nameof(AllTextTyped));

                Accuracy = 100 - (incorrectTyping * 100 / correctTyping);
                TriggerPropertyChanged(nameof(Accuracy));

                OpenFinishPopup = true;
                TriggerPropertyChanged(nameof(OpenFinishPopup));
            }
        }

        private void HighlightText()
        {
            if (!Keyboard.IsKeyDown(Key.Back))
            {
                if (IsValid)
                {
                    correctTyping++;
                    TypingAlert = false;
                    correctChars = CurrentInputText.Length;
                    incorrectChars = 0;
                }

                if (!IsValid)
                {
                    incorrectTyping++;
                    incorrectChars++;
                    if (CurrentWordLength - correctChars - incorrectChars < 0)
                    {
                        TypingAlert = true;
                        CurrentInputText = CurrentInputText.Substring(0, correctChars);
                        incorrectChars = 0;
                    }
                }
            }
            else
            {
                if (!isValid && !string.IsNullOrEmpty(CurrentInputText))
                {
                    incorrectChars--;
                }
                else
                {
                    incorrectChars = 0;
                    TypingAlert = false;
                    correctChars = CurrentInputText.Length;
                }
            }

            TriggerPropertyChanged(nameof(TextToTypeStyles)); //new Inlines formed at each char in input
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void TriggerPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}