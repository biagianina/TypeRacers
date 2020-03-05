using TypeRacers;
using Xunit;

namespace TypeRacersFacts
{
    public class InputCharacterValidationTests
    {
        InputCharacterValidation characterValidation = new InputCharacterValidation("sample text");
        [Fact]
        public void ReturnsTrueWhenFirstWordIsCorrect()
        {
            var word = "sample";
            Assert.True(characterValidation.ValidateWord(word, word.Length));
        }

        [Fact]
        public void ReturnsFalseWhenSecondWordIsCorrect()
        {
            var word = "text";
            Assert.False(characterValidation.ValidateWord(word, word.Length));
        }

        [Fact]
        public void ReturnsTrueWhenWholeTextIsTypedCorrect()
        {
            var firstWord = "sample";
            Assert.True(characterValidation.ValidateWord(firstWord, firstWord.Length));
            var firstWordAndSpace = "sample ";
            Assert.True(characterValidation.ValidateWord(firstWordAndSpace, firstWordAndSpace.Length));
            var firstWordAndSecondWord = "sample text";
            Assert.True(characterValidation.ValidateWord(firstWordAndSecondWord, firstWordAndSecondWord.Length));
        }
    }
}
