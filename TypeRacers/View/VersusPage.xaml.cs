using Common;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using TypeRacers.Client;
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

        public VersusViewModel VersusViewModel => (VersusViewModel)Resources["VersusVM"];

        public Player Player { get => VersusViewModel.Player; set => VersusViewModel.Player = value; }
        public GameInfo GameInfo { get => VersusViewModel.GameInfo; set => VersusViewModel.GameInfo = value; }

        public void BackBtn_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new MainPage());
        }
    }
}