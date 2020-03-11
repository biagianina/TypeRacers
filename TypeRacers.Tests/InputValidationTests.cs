using Xunit;
using TypeRacers;
using System.Windows;
using System.Threading;

namespace TypeRacersFacts
{
    public class InputValidationTests
    {
        [Fact]
        public void FirstCharValidator()
        {
            Thread validate = new Thread(()=> { var validator = new InputCharacterValidation("This is the text.");
                validator.ValidateInput("T");
                Assert.True(validator.IsValid);
                Assert.False(validator.TypingAlert);
                Assert.Equal(4, validator.CurrentWordLength);
                Assert.Equal(0, validator.CurrentWordIndex);
                Assert.False(validator.AllTextTyped);
                Assert.False(validator.OpenFinishPopup);
                Assert.False(validator.Clear);
                Assert.Equal(1, validator.NumberOfCharactersTyped);
            });
            validate.Start();
        }

        [Fact]
        public void FirstWordValidator()
        {
            Thread validate = new Thread(() => {
                var validator = new InputCharacterValidation("This is the text.");
                validator.ValidateInput("This ");
                Assert.True(validator.IsValid);
                Assert.False(validator.TypingAlert);
                Assert.Equal(2, validator.CurrentWordLength);
                Assert.Equal(1, validator.CurrentWordIndex);
                Assert.False(validator.AllTextTyped);
                Assert.False(validator.OpenFinishPopup);
                Assert.True(validator.Clear);
                Assert.Equal(5, validator.NumberOfCharactersTyped);
            });
            validate.Start();
        }

        [Fact]
        public void OneIncorectCharValidator()
        {
            Thread validate = new Thread(() => {
                var validator = new InputCharacterValidation("This is the text.");
                validator.ValidateInput("Thiz");
                Assert.False(validator.IsValid);
                Assert.Equal("salmon", validator.InputBackgroundColor);
                Assert.True(validator.TypingAlert);
                Assert.Equal(4, validator.CurrentWordLength);
                Assert.Equal(0, validator.CurrentWordIndex);
                Assert.False(validator.AllTextTyped);
                Assert.False(validator.OpenFinishPopup);
                Assert.False(validator.Clear);
                Assert.Equal(4, validator.NumberOfCharactersTyped);
            });
            validate.Start();
        }

        [Fact]
        public void WholeTextValidator()
        {
            Thread validate = new Thread(() => {
                var validator = new InputCharacterValidation("This is the text.");
                validator.ValidateInput("This is the text.");
                Assert.True(validator.IsValid);
                Assert.False(validator.TypingAlert);
                Assert.Equal(5, validator.CurrentWordLength);
                Assert.Equal(3, validator.CurrentWordIndex);
                Assert.True(validator.AllTextTyped);
                Assert.True(validator.OpenFinishPopup);
                Assert.False(validator.Clear);
                Assert.Equal(17, validator.NumberOfCharactersTyped);
            });
            validate.Start();
        }
    }
}
