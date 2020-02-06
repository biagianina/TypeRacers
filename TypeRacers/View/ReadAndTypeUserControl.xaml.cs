using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media.Animation;
using System.Windows.Threading;
using TypeRacers.ViewModel;

namespace TypeRacers.View
{
    /// <summary>
    /// Interaction logic for IOUserControl.xaml
    /// </summary>
    public partial class ReadAndTypeUserControl : UserControl, INotifyPropertyChanged
    {
        DispatcherTimer timer;
        public ReadAndTypeUserControl()
        {
            InitializeComponent();
        }

        //dp for typed text properties
        public readonly static DependencyProperty RATUCInlinesProperty = DependencyProperty.Register("RATUCInlines", typeof(IEnumerable<Inline>), typeof(ReadAndTypeUserControl));

        public IEnumerable<Inline> RATUCInlines
        {
            get { return (IEnumerable<Inline>)GetValue(RATUCInlinesProperty); }
            set
            {
                SetValue(RATUCInlinesProperty, value);
            }
        }

        //dp for textbox text
        public readonly static DependencyProperty RATUCCurrentInputTextProperty = DependencyProperty.Register("RATUCCurrentInputText", typeof(string), typeof(ReadAndTypeUserControl));

        public string RATUCCurrentInputText
        {
            get { return (string)GetValue(RATUCCurrentInputTextProperty); }
            set
            {
                SetValue(RATUCCurrentInputTextProperty, value);
            }
        }

        //dp for all text typed
        public readonly static DependencyProperty RATUCAllTextTypedProperty = DependencyProperty.Register("RATUCAllTextTyped", typeof(bool), typeof(ReadAndTypeUserControl));

        public bool RATUCAllTextTyped
        {
            get { return (bool)GetValue(RATUCAllTextTypedProperty); }
            set
            {
                SetValue(RATUCAllTextTypedProperty, value);
            }
        }

        //dp for background color in text box
        public readonly static DependencyProperty RATUCBackgroundColorProperty = DependencyProperty.Register("RATUCBackgroundColor", typeof(string), typeof(ReadAndTypeUserControl));

        public string RATUCBackgroundColor
        {
            get { return (string)GetValue(RATUCBackgroundColorProperty); }
            set
            {
                SetValue(RATUCBackgroundColorProperty, value);
            }
        }


        public readonly static DependencyProperty RATUCTypingAlertProperty = DependencyProperty.Register("RATUCTypingAlert", typeof(string), typeof(ReadAndTypeUserControl));

        public string RATUCTypingAlert
        {
            get { return (string)GetValue(RATUCTypingAlertProperty); }
            set
            {
                SetValue(RATUCTypingAlertProperty, value);
            }
        }

        public readonly static DependencyProperty RATUCSecondsToStartProperty = DependencyProperty.Register("RATUCSecondsToStart", typeof(string), typeof(ReadAndTypeUserControl));

        public string RATUCSecondsToStart
        {
            get { return (string)GetValue(RATUCSecondsToStartProperty); }
            set
            {
                SetValue(RATUCSecondsToStartProperty, value);
            }
        }

        public readonly static DependencyProperty RATUCSecondsInGameProperty = DependencyProperty.Register("RATUCSecondsInGame", typeof(string), typeof(ReadAndTypeUserControl));

        public string RATUCSecondsInGame
        {
            get { return (string)GetValue(RATUCSecondsInGameProperty); }
            set
            {
                SetValue(RATUCSecondsInGameProperty, value);
            }
        }

        public readonly static DependencyProperty RATUCGetReadyAlertProperty = DependencyProperty.Register("RATUCGetReadyAlert", typeof(bool), typeof(ReadAndTypeUserControl));

        public bool RATUCGetReadyAlert
        {
            get { return (bool)GetValue(RATUCGetReadyAlertProperty); }
            set
            {
                SetValue(RATUCGetReadyAlertProperty, value);
            }
        }

        public readonly static DependencyProperty RATUCCanTypeProperty = DependencyProperty.Register("RATUCCanType", typeof(bool), typeof(ReadAndTypeUserControl));

        public bool RATUCCanType
        {
            get { return (bool)GetValue(RATUCCanTypeProperty); }
            set
            {
                SetValue(RATUCCanTypeProperty, value);
            }
        }

        public readonly static DependencyProperty RATUCSearchingForOpponentsProperty = DependencyProperty.Register("RATUCSearchingForOpponents", typeof(bool), typeof(ReadAndTypeUserControl));

        public bool RATUCSearchingForOpponents
        {
            get { return (bool)GetValue(RATUCSearchingForOpponentsProperty); }
            set
            {
                SetValue(RATUCSearchingForOpponentsProperty, value);
            }
        }

        public readonly static DependencyProperty RATUCStartTimeProperty = DependencyProperty.Register("RATUCStartTime", typeof(int), typeof(ReadAndTypeUserControl));

        public int RATUCStartTime
        {
            get { return (int)GetValue(RATUCStartTimeProperty); }
            set
            {
                SetValue(RATUCStartTimeProperty, value);
            }
        }

        public DateTime RATUCStartingTime
        {
            get { return (DateTime)GetValue(RATUCStartingTimeProperty); }
            set { SetValue(RATUCStartingTimeProperty, value); }
        }

        public static readonly DependencyProperty RATUCStartingTimeProperty =
            DependencyProperty.Register("RATUCStartingTime", typeof(DateTime), typeof(ReadAndTypeUserControl));

        public DateTime RATUCEndingTime
        {
            get { return (DateTime)GetValue(RATUCEndingTimeProperty); }
            set { SetValue(RATUCEndingTimeProperty, value); }
        }

        public static readonly DependencyProperty RATUCEndingTimeProperty =
            DependencyProperty.Register("RATUCEndingTime", typeof(DateTime), typeof(ReadAndTypeUserControl));



        public bool RATUCStartReportingProgress
        {
            get { return (bool)GetValue(RATUCStartReportingProgressProperty); }
            set { SetValue(RATUCStartReportingProgressProperty, value); }
        }

        // Using a DependencyProperty as the backing store for RATUCStartReportingProgress.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty RATUCStartReportingProgressProperty =
            DependencyProperty.Register("RATUCStartReportingProgress", typeof(bool), typeof(ReadAndTypeUserControl));



        protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            base.OnPropertyChanged(e);
        }

        private void GetReadyPopUp_Opened(object sender, EventArgs e)
        {
            timer = new DispatcherTimer()
            {
                Interval = TimeSpan.FromMilliseconds(300)
            };

            timer.Tick += Timer_Tick;

            timer.Start();

            RATUCStartTime = 3;

            if (RATUCStartingTime != null)
            {
                RATUCStartTime = (RATUCStartingTime - DateTime.UtcNow).Seconds;
            }
        }

       
        private void Timer_Tick(object sender, EventArgs e)
        {
            if (RATUCStartingTime - DateTime.UtcNow < TimeSpan.Zero)
            {
                timer.Stop();
                RATUCGetReadyAlert = false;
                TriggerPropertyChanged(nameof(RATUCGetReadyAlert));
                RATUCCanType = true;
                StartGameTimer();
                TriggerPropertyChanged(nameof(RATUCCanType));
            }

            RATUCSecondsToStart = (RATUCStartingTime - DateTime.UtcNow).Seconds.ToString();
            TriggerPropertyChanged(nameof(RATUCSecondsToStart));

            if (RATUCSecondsToStart.Equals("0"))
            {
                RATUCSecondsToStart = "START!";
                TriggerPropertyChanged(nameof(RATUCSecondsToStart));
            }
        }

        private void StartGameTimer()
        {
            timer = new DispatcherTimer()
            {
                Interval = TimeSpan.FromMilliseconds(500)
            };

            timer.Tick += GameTimer_Tick;

            timer.Start();
        }

        
        private void GameTimer_Tick(object sender, EventArgs e)
        {
            RATUCStartReportingProgress = true;
            TriggerPropertyChanged(nameof(RATUCStartReportingProgress));

            if (RATUCEndingTime - DateTime.UtcNow < TimeSpan.Zero)
            {
                timer.Stop();
                RATUCCanType = false;
                TriggerPropertyChanged(nameof(RATUCCanType));
                RATUCStartReportingProgress = false;
                TriggerPropertyChanged(nameof(RATUCStartReportingProgress));
                RATUCSecondsInGame = "You finnished!";
                TriggerPropertyChanged(nameof(RATUCSecondsInGame));
            }

            if (RATUCEndingTime - DateTime.UtcNow > TimeSpan.Zero)
            {
                RATUCSecondsInGame = ((int)(RATUCEndingTime - DateTime.UtcNow).TotalSeconds).ToString() + " seconds left";
                TriggerPropertyChanged(nameof(RATUCSecondsInGame));
            }

            if (RATUCSecondsInGame.Equals("0 seconds left"))
            {
                RATUCSecondsInGame = "Time is up!";
                TriggerPropertyChanged(nameof(RATUCSecondsInGame));
            }
        }
       
        public event PropertyChangedEventHandler PropertyChanged;

        public void TriggerPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
