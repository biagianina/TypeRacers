using System;
using System.ComponentModel;
using System.Windows.Navigation;
using TypeRacers.View;

namespace TypeRacers.ViewModel
{
    internal class MainViewModel : INotifyPropertyChanged
    {
        private VersusPage race;
        private PracticePage practice;
        public MainViewModel()
        {
            ContestCommand = new CommandHandler(() => NavigateContest(), () => true);
            PracticeCommand = new CommandHandler(() => NavigatePractice(), () => true);
        }

        public bool UsernameEntered { get; set; }
        public Model.Model Model { get; set; }
        public bool EnterUsernameMessage { get; set; }
        public CommandHandler ContestCommand { get; }

        public NavigationService ContestNavigation { get; set; }

        public CommandHandler PracticeCommand { get; }

        public NavigationService PracticeNavigation { get; set; }

        public string Username
        {
            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    UsernameEntered = false;
                    TriggerPropertyChanged(nameof(UsernameEntered));
                    return;
                }

                UsernameEntered = true;
                TriggerPropertyChanged(nameof(UsernameEntered));
                Name = value;
            }
        }

        public static string Name { get; private set; }

        private void NavigateContest()
        {
            if (UsernameEntered)
            {
                Model = new Model.Model();
                Model.StartCommunication();
                race = new VersusPage
                {
                    Player = Model.GetPlayer(),
                    GameInfo = Model.GetGameInfo()
                };
                ContestNavigation.Navigate(race);
            }
            else
            {
                EnterUsernameMessage = true;
                TriggerPropertyChanged(nameof(EnterUsernameMessage));
            }
        }

        private void NavigatePractice()
        {
            if (UsernameEntered)
            {
                Model = new Model.Model();
                practice = new PracticePage
                {
                    TextToType = Model.GetGeneratedTextToTypeLocally()
                };
                
                PracticeNavigation.Navigate(practice);
            }
            else
            {
                EnterUsernameMessage = true;
                TriggerPropertyChanged(nameof(EnterUsernameMessage));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void TriggerPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}