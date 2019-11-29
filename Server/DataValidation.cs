using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TypeRacers
{
    public class DataValidation
    {
        private string originalText = string.Empty;

        public DataValidation(string input)
        {
            if (input is null)
            {
                throw new ArgumentNullException("Input is null.");
            }

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
