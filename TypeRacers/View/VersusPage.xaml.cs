using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using TypeRacers.Model;
using TypeRacers.ViewModel;

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

        public Model.Model Model { get => VersusViewModel.Model; set => VersusViewModel.Model = value; }

        public VersusViewModel VersusViewModel => (VersusViewModel)Resources["VersusVM"];

        public void BackBtn_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new MainPage());
        }
    }
}