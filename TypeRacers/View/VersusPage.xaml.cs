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
        private double timeTillStart;
        public VersusPage()
        {
            InitializeComponent();
            vm = (VersusViewModel)Resources["VersusVM"];
        }
        public void BackBtn_Click(object sender, RoutedEventArgs e)
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

            timeTillStart = (int)(vm.StartTime - DateTime.UtcNow).TotalSeconds;
            vm.SecondsToGetReady = timeTillStart.ToString();
            vm.TriggerPropertyChanged(nameof(vm.SecondsToGetReady));
        }
       
        private void Timer_Tick(object sender, EventArgs e)
        {
            timeTillStart--;

            if (timeTillStart < 0)
            {
                timer.Stop();
                vm.EnableGetReadyAlert = false;
                vm.TriggerPropertyChanged(nameof(vm.EnableGetReadyAlert));
                vm.CanUserType = true;
                vm.StartGameTimer = true;
                StartGameTimer();
                vm.TriggerPropertyChanged(nameof(vm.CanUserType));
            }

            vm.SecondsToGetReady = timeTillStart.ToString();
            vm.TriggerPropertyChanged(nameof(vm.SecondsToGetReady));

            if (timeTillStart == 0)
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
