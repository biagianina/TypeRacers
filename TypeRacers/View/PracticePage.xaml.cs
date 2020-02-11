using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;

namespace TypeRacers.View
{
    /// <summary>
    /// Interaction logic for Practice.xaml
    /// </summary>
    public partial class PracticePage : Page
    {
        public PracticePage()
        {
            InitializeComponent();
        }

        public void Back_click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new MainPage());
        }
    }
}