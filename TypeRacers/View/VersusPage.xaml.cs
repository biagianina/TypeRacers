using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using System.Windows.Navigation;
using TypeRacers.ViewModel;

namespace TypeRacers.View
{
    /// <summary>
    /// Interaction logic for VersusWindow.xaml
    /// </summary>
    public partial class VersusPage : Page
    {
        readonly VersusViewModel vm;
        DispatcherTimer timer;
        public VersusPage()
        {
            InitializeComponent();
            vm = (VersusViewModel)Resources["VersusVM"];
        }
        public void Back_click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new MainPage());
        }

        private void GetReadyPopUp_Opened(object sender, EventArgs e)
        {
            timer = new DispatcherTimer()
            {
                Interval = TimeSpan.FromSeconds(1)
            };

            timer.Tick += Timer_Tick;

            timer.Start();
        }

        private int decrement = 5;
        private void Timer_Tick(object sender, EventArgs e)
        {
            decrement--;

            if (decrement < 0)
            {
                timer.Stop();
                vm.EnableGetReadyAlert = false;
                vm.TriggerPropertyChanged(nameof(vm.EnableGetReadyAlert));
                vm.CanUserType = true;
                vm.StartGameTimer = true;
                StartGameTimer();
                vm.TriggerPropertyChanged(nameof(vm.CanUserType));
            }

            vm.SecondsToGetReady = decrement.ToString();
            vm.TriggerPropertyChanged(nameof(vm.SecondsToGetReady));

            if (decrement == 0)
            {
                vm.SecondsToGetReady = "START!";
                vm.TriggerPropertyChanged(nameof(vm.SecondsToGetReady));
            }
        }

        private void StartGameTimer()
        {
            timer = new DispatcherTimer()
            {
                Interval = TimeSpan.FromSeconds(1)
            };

            timer.Tick += GameTimer_Tick;

            timer.Start();
        }

        private int secondsInGame = 90;
        private void GameTimer_Tick(object sender, EventArgs e)
        {
            secondsInGame--;

            if (secondsInGame < 0)
            {
                timer.Stop();
                vm.CanUserType = false;
                vm.TriggerPropertyChanged(nameof(vm.CanUserType));
            }

            if (secondsInGame > 0)
            {
                vm.SecondsInGame = secondsInGame.ToString() + " seconds";
                vm.TriggerPropertyChanged(nameof(vm.SecondsInGame));
            }

            if (secondsInGame == 0)
            {
                vm.SecondsInGame = "Time is up!";
                vm.TriggerPropertyChanged(nameof(vm.SecondsInGame));
            }
        }
    }
}
