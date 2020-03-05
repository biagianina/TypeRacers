namespace TypeRacers
{
    public class InputCharacterValidation
    {
        string currentText = string.Empty;
        private string originalText = string.Empty;
        int spaceIndex = 0;
        bool isValid;
        public InputCharacterValidation(string input)
        {
            originalText = input;
        }
        
        //validate only one word
        public bool ValidateWord(string currentTypedWord, int currentCharIndex, ref int spaceIndex, ref int currentWordIndex, ref int numberOfCharactersTyped)
        {
            currentText = currentTypedWord;
            this.spaceIndex = spaceIndex;
            bool charIndexIsInRange = currentCharIndex != -1 && currentCharIndex <= originalText.Length;

            if (!charIndexIsInRange)
            {
                isValid = false;
                return false;
            }

            string substringToCheck = string.Empty;

            //create the substring to compare with the typed word

            if (charIndexIsInRange)
            {
                substringToCheck = originalText.Substring(0, currentCharIndex);
            }
            isValid = substringToCheck.Equals(currentTypedWord);
            CheckIfInputIsCompleteWord(ref spaceIndex, ref currentWordIndex, ref numberOfCharactersTyped, ref currentTypedWord);
            return isValid;
        }

        private void CheckIfInputIsCompleteWord(ref int spaceIndex, ref int currentWordIndex, ref int numberOfCharactersTyped, ref string currentInputText)
        {
            if (isValid && currentInputText.EndsWith(" "))
            {
                spaceIndex += currentInputText.Length;

                if (currentWordIndex < currentInputText.Split().Length - 1)
                {
                    currentWordIndex++;
                }

                numberOfCharactersTyped += currentInputText.Length;
                currentInputText = string.Empty;

            }
        }

        public bool IsLastWord()
        {
          return isValid && currentText.Length + spaceIndex == originalText.Length;            
        }

    }
}