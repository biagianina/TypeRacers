using System;

namespace TypeRacers
{
    public class InputCharacterValidation
    {
        private string originalText = string.Empty;
        bool isValid;
        private string typedText;
        private InputCharacterValidation userInputValidator;
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
        private bool startReporting;
        bool typingAlert;

        public InputCharacterValidation(string input)
        {
            originalText = input;
        }

        //validate only one word
        public bool ValidateWord(string currentTypedWord, int currentCharIndex, int spaceIndex, out bool isWordCompleted, out bool isLastWord)
        {
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
    }
}