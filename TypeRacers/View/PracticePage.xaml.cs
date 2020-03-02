using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Common;
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

        public PracticeViewModel ViewModel => (PracticeViewModel)Resources["PracticeVM"];
        public string TextToType {
            get => ViewModel.TextToType;
            set
            {
                ViewModel.TextToType = value;
            }
        }

        public void Back_click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new MainPage());
        }
    }
}