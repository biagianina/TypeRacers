using System;
using System.Windows.Controls;
using System.Windows.Threading;

namespace TypeRacers.View
{
    /// <summary>
    /// Interaction logic for VersusWindow.xaml
    /// </summary>
    public partial class VersusPage : Page
    {
        public VersusPage()
        {
            InitializeComponent();
        }

        private void GetReadyPopUp_Opened(object sender, EventArgs e)
        {
            DispatcherTimer timer = new DispatcherTimer()
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
            Timer.Content = decrement.ToString();
            if (decrement == 0)
            {
                Timer.Content = "START!";
            }

            if (decrement < 0)
            {
                UserCTRL.inputTextbox.IsEnabled = true;
                GetReadyPopUp.IsOpen = false;
                GetReadyPopUp.IsEnabled = false;
            }
        }
    }
}
