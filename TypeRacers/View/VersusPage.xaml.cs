using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using TypeRacers.ViewModel;

namespace TypeRacers.View
{
    /// <summary>
    /// Interaction logic for VersusWindow.xaml
    /// </summary>
    public partial class VersusPage : Page
    {
        VersusViewModel vm;
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
                timer = default;
                vm.EnableGameTimer = true;
                vm.TriggerPropertyChanged(nameof(vm.EnableGameTimer));
                StartTimerForGame();
                vm.EnableGetReadyAlert = false;
                vm.TriggerPropertyChanged(nameof(vm.EnableGetReadyAlert));
                vm.CanUserType = true;
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

        private void StartTimerForGame()
        {
            timer = new DispatcherTimer()
            {
                Interval = TimeSpan.FromSeconds(1)
            };

            timer.Tick += GameTimer_Tick;

            timer.Start();
        }

        private int gameSeconds = 90;
        private void GameTimer_Tick(object sender, EventArgs e)
        {
            gameSeconds--;

            if (gameSeconds < 0)
            {
                timer.Stop();
                timer = default;
               
                vm.CanUserType = false;
                vm.TriggerPropertyChanged(nameof(vm.CanUserType));
            }

            vm.SecondsInGame = gameSeconds.ToString() + " seconds";
            vm.TriggerPropertyChanged(nameof(vm.SecondsInGame));

            if (gameSeconds == 0)
            {
                vm.SecondsInGame = "End!";
                vm.TriggerPropertyChanged(nameof(vm.SecondsInGame));
            }
        }

        
    }
}
