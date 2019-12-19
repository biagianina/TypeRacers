using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;

namespace TypeRacers.View
{
    /// <summary>
    /// Interaction logic for Page1.xaml
    /// </summary>
    public partial class FirstPage : Page
    {
        public FirstPage()
        {
            InitializeComponent();
        }

        private void start_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(username.Text))
            {
                NavigationService nav = NavigationService.GetNavigationService(this);
                nav.Navigate(new Uri("View/MainPage.xaml", UriKind.RelativeOrAbsolute));
            }
            else
            {
                MessageBox.Show("Please enter a username!");
            }
        }
    }
}
