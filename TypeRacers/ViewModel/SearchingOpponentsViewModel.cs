using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace TypeRacers.ViewModel
{
    class SearchingOpponentsViewModel : INotifyPropertyChanged
    {

        NetworkHandler networkHandler;

        public SearchingOpponentsViewModel()
        {
            networkHandler = new NetworkHandler();
            Timer timer = new Timer();
            timer.Interval = 5000;
            timer.Elapsed += OnTimedEvent;
            timer.AutoReset = true;

            timer.Enabled = true;

        }
        public int NumberOfOpponents { get; set; }

      
        private void OnTimedEvent(Object source, System.Timers.ElapsedEventArgs e)
        {

            NumberOfOpponents++;
            TriggerPropertyChanged(nameof(NumberOfOpponents));

        }


        public void CheckForOpponents()
        {
        }
        public event PropertyChangedEventHandler PropertyChanged;
        public void TriggerPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
