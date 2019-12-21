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
    /// Interaction logic for IOUserControl.xaml
    /// </summary>
    public partial class IOUserControl : UserControl
    {
        public IOUserControl()
        {
            InitializeComponent();
        }

        //dp for typed text properties
        public static DependencyProperty IOUCInlinesProperty = DependencyProperty.Register("IOUCInlines", typeof(IEnumerable<Inline>), typeof(IOUserControl));

        public IEnumerable<Inline> IOUCInlines
        {
            get { return (IEnumerable<Inline>)GetValue(IOUCInlinesProperty); }
            set
            {
                SetValue(IOUCInlinesProperty, value);
            }
        }

        //dp for textbox text
        public static DependencyProperty IOUCCurrentInputTextProperty = DependencyProperty.Register("IOUCCurrentInputText", typeof(string), typeof(IOUserControl));

        public string IOUCCurrentInputText
        {
            get { return (string)GetValue(IOUCCurrentInputTextProperty); }
            set
            {
                SetValue(IOUCCurrentInputTextProperty, value);
            }
        }

        //dp for all text typed
        public static DependencyProperty IOUCAllTextTypedProperty = DependencyProperty.Register("IOUCAllTextTyped", typeof(bool), typeof(IOUserControl));

        public bool IOUCAllTextTyped
        {
            get { return (bool)GetValue(IOUCAllTextTypedProperty); }
            set
            {
                SetValue(IOUCAllTextTypedProperty, value);
            }
        }

        //dp for background color in text box
        public static DependencyProperty IOUCBackgroundColorProperty = DependencyProperty.Register("IOUCBackgroundColor", typeof(string), typeof(IOUserControl));

        public string IOUCBackgroundColor
        {
            get { return (string)GetValue(IOUCBackgroundColorProperty); }
            set
            {
                SetValue(IOUCBackgroundColorProperty, value);
            }
        }


        public static DependencyProperty IOUCTypingAlertProperty = DependencyProperty.Register("IOUCTypingAlert", typeof(string), typeof(IOUserControl));

        public string IOUCTypingAlert
        {
            get { return (string)GetValue(IOUCTypingAlertProperty); }
            set
            {
                SetValue(IOUCTypingAlertProperty, value);
            }
        }

        protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            base.OnPropertyChanged(e);
        }

    }
}
