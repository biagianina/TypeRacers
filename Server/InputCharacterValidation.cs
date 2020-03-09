using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;

namespace TypeRacers
{
    public class InputCharacterValidation
    {
        string typedText;
        private string originalText = string.Empty;
        bool isValid;
        private int spaceIndex;
        private int correctChars;
        private int incorrectChars;
        private int currentWordIndex;
        private bool wordIsCompletelyTyped;
        private bool isLastWord;
        private bool alert;
        private int numberOfCharactersTyped;
        private int incorrectTyping;
        private int correctTyping;
        bool typingAlert;

        public InputCharacterValidation(string input)
        {
            originalText = input;
        }

        public IEnumerable<Inline> TextToTypeStyles
        {
            get => new[] { new Run() { Text = originalText.Substring(0, spaceIndex) , Foreground = Brushes.Salmon},
                new Run() { Text = originalText.Substring(spaceIndex, correctChars), Foreground = Brushes.Salmon, TextDecorations = TextDecorations.Underline},
                new Run() { Text = originalText.Substring(correctChars + spaceIndex, incorrectChars), TextDecorations = TextDecorations.Underline, Background = Brushes.Salmon},
                new Run() {Text = originalText.Substring(spaceIndex + correctChars + incorrectChars, CurrentWordLength - correctChars - incorrectChars), TextDecorations = TextDecorations.Underline},
                new Run() {Text = originalText.Substring(spaceIndex + CurrentWordLength) }
                };
        }
 
        public int CurrentWordLength
        {
            get => originalText.Split()[currentWordIndex].Length;//length of current word
        }
 
        public string RemianingText { get; set; }
        //validate only one word
        public bool ValidateWord(string currentTypedWord, int spaceIndex, out bool isWordCompleted, out bool isLastWord)
        {
            typedText = currentTypedWord;
            var currentCharIndex = currentTypedWord.Length;
            bool charIndexIsInRange = currentCharIndex != -1 && currentCharIndex <= originalText.Length;

            if (!charIndexIsInRange)
            {
                isWordCompleted = false;
                isLastWord = false;
                return false;
            }

            string substringToCheck = string.Empty;

            //create the substring to compare with the typed word

            if (charIndexIsInRange)
            {
                substringToCheck = originalText.Substring(0, currentCharIndex);
            }

            isValid = substringToCheck.Equals(currentTypedWord);
            isWordCompleted = isValid && currentTypedWord.EndsWith(" ");
            isLastWord = isValid && currentTypedWord.Length + spaceIndex == originalText.Length;
            return isValid;
        }

        public bool GenerateHighlightInfo(bool isBackKeyPressed)
        {

            if (isBackKeyPressed && !isValid && !string.IsNullOrEmpty(typedText))
            {
                incorrectChars--;
            }

            else
            {
                if (isValid)
                {
                    correctTyping++;
                    typingAlert = false;
                    correctChars = typedText.Length;
                    incorrectChars = 0;
                }

                if (!isValid)
                {
                    incorrectTyping++;
                    incorrectChars++;
                    if (CurrentWordLength - correctChars - incorrectChars < 0)
                    {
                        typingAlert = true;
                        typedText.Substring(0, correctChars);
                        incorrectChars = 0;
                    }
                }
            }
            return typingAlert;
        }
    }
}