using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TypeRacers.ModelView
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
        public (bool, string) ValidateWord(string wordToCheck, int charIndex)
        {
            // char index is to create current substring

            bool charIndexIsInRange = charIndex != -1 && charIndex <= originalText.Length;

            string substringToCheck = string.Empty;

            //create the substring from current word to check with wordToCheck

            if (charIndexIsInRange)
            {
                substringToCheck = originalText.Substring(0, charIndex);
            }

            return (CheckIfTwoStringsAreEqual(substringToCheck, wordToCheck), substringToCheck);

        }


        private bool CheckIfTwoStringsAreEqual(string first, string second)
        {
            return first.Equals(second);
        }

    }
}
