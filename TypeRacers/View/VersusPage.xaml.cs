using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;

namespace TypeRacers.View
{
    /// <summary>
    /// Interaction logic for VersusWindow.xaml
    /// </summary>
    public partial class VersusPage : Page
    {
        public VersusPage()
        {
            InitializeComponent();
        }

        public void BackBtn_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new MainPage());
        }
    }
}