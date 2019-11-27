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
        public MainWindow()
        {
            InitializeComponent();
            DataContext = new TypeViewModel(); //sets data context to be the ViewModel, in order to bind the text from the server
            KeyDown += new KeyEventHandler(EnterIsPressed);
        }

        //extra utilities to check whole text, event when enter is pressed
        private void EnterIsPressed(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                e.Handled = true;
            }

            if (e.Handled)
            {
                if (userInput.Text != "")
                {
                    if (userInput.Text.Equals(MyText.Text))
                    {
                        MyText.Foreground = Brushes.Green;
                        MessageBox.Show("Great!");
                    }
                    else
                    {
                        MyText.Foreground = Brushes.Red;
                        MessageBox.Show("Oops! Try again!");
                    }
                }

                else
                {
                    MessageBox.Show("Type something!");
                }
            }
        }
    }
}
