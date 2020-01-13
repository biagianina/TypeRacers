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

namespace TypeRacers.View
{
    /// <summary>
    /// Interaction logic for CarAndProgressUserControl.xaml
    /// </summary>
    public partial class CarAndProgressUserControl : UserControl
    {
        public CarAndProgressUserControl()
        {
            InitializeComponent();
        }


        public static DependencyProperty CAPUCProgressProperty = DependencyProperty.Register("CAPUCProgress", typeof(int), typeof(CarAndProgressUserControl));

        public int CAPUCProgress
        {
            get { return (int)GetValue(CAPUCProgressProperty); }
            set
            {
                SetValue(CAPUCProgressProperty, value);
            }
        }
        protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            base.OnPropertyChanged(e);
        }

        public static DependencyProperty CAPUCSliderStyleProperty = DependencyProperty.Register("CAPUCSliderStyle", typeof(Style), typeof(CarAndProgressUserControl));

        public Style CAPUCSliderStyle
        {
            get { return (Style)GetValue(CAPUCSliderStyleProperty); }
            set
            {
                SetValue(CAPUCSliderStyleProperty, value);
            }
        }
    }
}
