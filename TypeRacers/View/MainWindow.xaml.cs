using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using TypeRacers.ModelView;

namespace TypeRacers
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private int charIndex = 0;
        string originalText;
        bool isValidInput;
        DataValidation dataValidation;
        StringBuilder wordToCheck = new StringBuilder();
        bool backspacePressed;


        public MainWindow()
        {
            InitializeComponent();
            DataContext = new TypeViewModel(); //sets data context to be the ViewModel, in order to bind the text from the server
            originalText = displayText.Text;
            dataValidation = new DataValidation(originalText);
        }

        //this listener is used only for backspace key
        private void KeyListener(object sender, KeyEventArgs e)
        {
            if (e.Key != Key.Back)
            {
                backspacePressed = false;
            }

            //check for backspace
            if (e.Key == Key.Back)
            {
                if (userInput.Text != string.Empty)
                {
                    backspacePressed = true;
                    //remove one char at a time if length is greater than 0
                    if (wordToCheck.Length > 0)
                    {
                        wordToCheck.Remove(wordToCheck.Length - 1, 1);

                    }

                    //lowering the char index
                    charIndex--;
                }
            }
        }

        //listener when user types char by char
        private void UserInput_TextChanged(object sender, TextChangedEventArgs e)
        {
            char currentChar = default;
            // getting the current char from input
            if (userInput.Text != string.Empty)
            {
                currentChar = userInput.Text.Last();
            }

            //adding the current char to the formed word that will be validated
            if (!backspacePressed)
            {
                wordToCheck.Append(currentChar);
                charIndex++;
            }

            //validate formed word
            var result = dataValidation.ValidateWord(wordToCheck.ToString(), charIndex);

            isValidInput = result.Item1;
            var substringToCheck = result.Item2;

            validationCheck.Text = "word to check: " + wordToCheck.ToString() +
                " substring formed: " + substringToCheck +
                " char index: " + charIndex +
                " is valid: " + isValidInput;

        }
    }
}
