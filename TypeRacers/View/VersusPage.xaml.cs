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
            vm = (VersusViewModel)this.Resources["VersusVM"];
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
    }
}
