﻿using System;
using System.ComponentModel;
using System.Windows.Navigation;

namespace TypeRacers.ViewModel
{
    internal class MainViewModel : INotifyPropertyChanged
    {
        public MainViewModel()
        {
            ContestCommand = new CommandHandler(() => NavigateContest(), () => true);
            PracticeCommand = new CommandHandler(() => NavigatePractice(), () => true);
        }

        public bool UsernameEntered { get; set; }

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
                ContestNavigation.Navigate(new Uri("View/VersusPage.xaml", UriKind.RelativeOrAbsolute));
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
                PracticeNavigation.Navigate(new Uri("View/PracticePage.xaml", UriKind.RelativeOrAbsolute));
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