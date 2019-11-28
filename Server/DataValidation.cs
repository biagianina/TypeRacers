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
        private int charIndex = 0;
        StringBuilder newFormedWord = new StringBuilder(); //word formed after everytime a char is typed

        public DataValidation(string input)
        {
            if (input is null)
            {
                throw new ArgumentNullException("Input is null.");
            }

            originalText = input;
        }

        //validate only one word
        public bool ValidateWord(char currentInputChar, bool isBackspace)
        {
            bool charIndexIsInRange = charIndex != -1 && charIndex <= originalText.Length;

            if (!charIndexIsInRange)
            {
                return false;
            }

            // create new form word with the input char
            if (!isBackspace)
            {
                newFormedWord.Append(currentInputChar);
                charIndex++;
            }

            //check for backspace key
            if (isBackspace)
            {
                if (newFormedWord.Length > 0)
                {
                    newFormedWord.Remove(newFormedWord.Length - 1, 1);

                }

                //lowering the char index
                charIndex--;
            }



            string substringToCheck = string.Empty;

            //create the substring to compare with new formed word

            if (charIndexIsInRange)
            {
                substringToCheck = originalText.Substring(0, charIndex);
            }

            return CheckIfTwoStringsAreEqual(substringToCheck, newFormedWord.ToString());

        }


        private bool CheckIfTwoStringsAreEqual(string first, string second)
        {
            return first.Equals(second);
        }

    }
}
