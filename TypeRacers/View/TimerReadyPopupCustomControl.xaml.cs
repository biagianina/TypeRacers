using System.Windows;
using System.Windows.Controls;

namespace TypeRacers.View
{
    /// <summary>
    /// Interaction logic for TimerReadyPopupCustomControl.xaml
    /// </summary>
    public partial class TimerReadyPopupCustomControl : UserControl
    {
        public TimerReadyPopupCustomControl()
        {
            InitializeComponent();
        }

        public static DependencyProperty TitleProperty = DependencyProperty.Register(nameof(Title), typeof(string), typeof(TimerReadyPopupCustomControl));

        public string Title
        {
            get
            {
                return (string)GetValue(TitleProperty);
            }
            set
            {
                SetValue(TitleProperty, value);
            }
        }
    }
}