using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using TypeRacers.ViewModel;

namespace TypeRacers.View
{
    /// <summary>
    /// Interaction logic for Practice.xaml
    /// </summary>
    public partial class PracticePage : Page
    {
        DispatcherTimer timer;
        PracticeViewModel vm;
        public PracticePage()
        {
            InitializeComponent();
            vm = (PracticeViewModel)Resources["PracticeVM"];
            StartGameTimer();
        }

        public void Back_click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new MainPage());
        }

        private void StartGameTimer()
        {
            timer = new DispatcherTimer()
            {
                Interval = TimeSpan.FromSeconds(1)
            };

            timer.Tick += GameTimer_Tick;
            vm.CanUserType = true;
            vm.TriggerPropertyChanged(nameof(vm.CanUserType));
            timer.Start();
        }

        private int secondsInGame = 90;
        private void GameTimer_Tick(object sender, EventArgs e)
        {
            secondsInGame--;
           
            if (secondsInGame < 0 || vm.AllTextTyped)
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
