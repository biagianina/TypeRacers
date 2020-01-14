using System;
using System.Windows.Navigation;

namespace TypeRacers.ViewModel
{
    internal class MainViewModel
    {
        public MainViewModel()
        {
            ContestCommand = new CommandHandler(() => NavigateContest(), () => true);
            PracticeCommand = new CommandHandler(() => NavigatePractice(), () => true);
        }

        public CommandHandler ContestCommand { get; }

        public NavigationService ContestNavigation { get; set; }

        public CommandHandler PracticeCommand { get; }

        public NavigationService PracticeNavigation { get; set; }

        private void NavigateContest()
        {
            ContestNavigation.Navigate(new Uri("View/VersusPage.xaml", UriKind.RelativeOrAbsolute));
        }

        private void NavigatePractice()
        {
            PracticeNavigation.Navigate(new Uri("View/PracticePage.xaml", UriKind.RelativeOrAbsolute));
        }
    }
}