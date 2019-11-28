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
        string originalText;
        bool isValidInput;
        DataValidation dataValidation;
        bool backspacePressed;


        public MainWindow()
        {
            InitializeComponent();
            var context = new TypeViewModel();
            DataContext = context; //sets data context to be the ViewModel, in order to bind the text from the server
            originalText = context.Model.Text;
            dataValidation = new DataValidation(originalText);
        }

        //this listener is used only for backspace key
        private void KeyListener(object sender, KeyEventArgs e)
        {
            //check for backspace
            if (e.Key != Key.Back)
            {
                backspacePressed = false;
            }

            if (e.Key == Key.Back)
            {
                backspacePressed = true;
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


            isValidInput = dataValidation.ValidateWord(currentChar, backspacePressed);


            validationCheck.Text = "word to check: " + " is valid: " + isValidInput;

        }
    }
}
