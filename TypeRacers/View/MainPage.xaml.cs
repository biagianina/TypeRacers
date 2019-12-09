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

namespace TypeRacers.View
{
    /// <summary>
    /// Interaction logic for MainPage.xaml
    /// </summary>
    public partial class MainPage : Page
    {
        public MainPage()
        {
            InitializeComponent();
        }

        private void Contest_Click(object sender, RoutedEventArgs e)
        {
            MainMenu.NavigationService.Navigate(new VersusPage());
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
            MainMenu.NavigationService.Navigate(new PracticePage());
            HideElements();
        }
    }
}
