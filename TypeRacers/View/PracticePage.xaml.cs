using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using TypeRacers.Model;
using TypeRacers.ViewModel;

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

        public Model.Model Model { get => ViewModel.Model; internal set => ViewModel.Model = value; }

        public PracticeViewModel ViewModel => (PracticeViewModel)Resources["PracticeVM"];

        public void Back_click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new MainPage());
        }
    }
}