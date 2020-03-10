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
        private string remainingText;
        private bool isValid;
        private InputCharacterValidation userInputValidator;

        private int spaceIndex;
        private int correctChars;
        private int incorrectChars;
        private int incorrectTyping;
        private int correctTyping;
        private string textTyped;
        private bool alert;

        public InputCharacterValidation(string gameText)
        {
            originalText = gameText;
            remainingText = gameText;
        }

        public IEnumerable<Inline> TextToTypeStyles
        {
            get => new[] { new Run() { Text = originalText.Substring(0, SpaceIndex) , Foreground = Brushes.Salmon},
                new Run() { Text = originalText.Substring(SpaceIndex, correctChars), Foreground = Brushes.Salmon, TextDecorations = TextDecorations.Underline},
                new Run() { Text = originalText.Substring(correctChars + SpaceIndex, incorrectChars), TextDecorations = TextDecorations.Underline, Background = Brushes.Salmon},
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
                if (textTyped == value)
                    return;

                textTyped = value;
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
        public DateTime EndTime { get; private set; }
        public int Accuracy { get; private set; }
        public bool OpenFinishPopup { get; private set; }
        public bool Clear { get; set; }
        public int SpaceIndex { get; private set; }

        public int NumberOfCharactersTyped { get; set; }
        public int CurrentWordIndex { get; private set; }

        //validate only one word
        public void ValidateInput(string input)
        {
            Clear = false;
            TriggerPropertyChanged(nameof(Clear));

            CurrentInputText = input;
            bool charIndexIsInRange = CurrentInputText.Length != -1 && CurrentInputText.Length <= originalText.Length;

            if (!charIndexIsInRange)
            {
                isValid = false;
            }

            string substringToCheck = string.Empty;

            //create the substring to compare with the typed word
            if (charIndexIsInRange)
            {
                substringToCheck = remainingText.Substring(0, CurrentInputText.Length);
            }

            isValid = substringToCheck.Equals(CurrentInputText);

            CheckUserInput(CurrentInputText);
            HighlightText();
        }

        private void CheckUserInput(string text)
        {
            CheckIfInputIsCompleteWord(text);

            //checks if current word is the last one
            CheckIfIsLastWord();
        }
        private void CheckIfInputIsCompleteWord(string value)
        {
            if (isValid && value.EndsWith(" "))
            {
                SpaceIndex += value.Length;
                TriggerPropertyChanged(nameof(SpaceIndex));
                if (CurrentWordIndex < originalText.Split().Length - 1)
                {
                    CurrentWordIndex++;
                }

                remainingText = originalText.Substring(SpaceIndex);
                NumberOfCharactersTyped += CurrentInputText.Length;
                TriggerPropertyChanged(nameof(NumberOfCharactersTyped));
                textTyped = string.Empty;
                Clear = true;
                TriggerPropertyChanged(nameof(Clear));
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
                if (isValid)
                {
                    correctTyping++;
                    alert = false;
                    correctChars = textTyped.Length;
                    incorrectChars = 0;
                }

                if (!isValid)
                {
                    incorrectTyping++;
                    incorrectChars++;
                    if (CurrentWordLength - correctChars - incorrectChars < 0)
                    {
                        alert = true;
                        textTyped = textTyped.Substring(0, correctChars);
                        incorrectChars = 0;
                    }
                }
            }
            else
            {
                if (!isValid && !string.IsNullOrEmpty(textTyped) && CurrentWordLength - correctChars > 0)
                {
                    incorrectChars--;
                }
                else
                {
                    alert = false;
                    correctChars = textTyped.Length;
                    incorrectChars = 0;
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