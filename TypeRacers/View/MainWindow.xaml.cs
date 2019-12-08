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
using System.Windows.Shapes;

namespace TypeRacers.View
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Contest_Click(object sender, RoutedEventArgs e)
        {
            MainMenu.Content = new VersusPage();
            HideElements();
        }

        private void HideElements()
        {
            txt_welcomingmessage.Visibility = Visibility.Collapsed;
            btnContest.Visibility = Visibility.Collapsed;
            btnPractice.Visibility = Visibility.Collapsed;
        }

        private void Practice_Click(object sender, RoutedEventArgs e)
        {
            MainMenu.Content = new PracticePage();
            HideElements();
        }
    }
}
