namespace TypeRacers
{
    public class InputCharacterValidation
    {
        private string originalText = string.Empty;

        public InputCharacterValidation(string input)
        {
            originalText = input;
        }

        //validate only one word
        public bool ValidateWord(string currentTypedWord, int currentCharIndex)
        {
            bool charIndexIsInRange = currentCharIndex != -1 && currentCharIndex <= originalText.Length;

            if (!charIndexIsInRange)
            {
                return false;
            }

            string substringToCheck = string.Empty;

            //create the substring to compare with the typed word

            if (charIndexIsInRange)
            {
                substringToCheck = originalText.Substring(0, currentCharIndex);
            }

            return substringToCheck.Equals(currentTypedWord);
        }
    }
}