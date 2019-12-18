using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using TypeRacers.ViewModel;

namespace TypeRacers.View
{
    /// <summary>
    /// Interaction logic for BaseUserControl.xaml
    /// </summary>
    public partial class BaseUserControl : UserControl
    {
        public BaseUserControl()
        {
            InitializeComponent();
            TextFromServer = BaseViewModel.Inlines;
        }

        // dependecy properties from text recieved from server and styles, background color, and other properties from view model
        public IEnumerable<Inline> TextFromServer
        {
            get { return (IEnumerable<Inline>)GetValue(TextFromServerProperty); }
            set { SetValue(TextFromServerProperty, value); }
        }

        public static DependencyProperty TextFromServerProperty =
            DependencyProperty.Register("TextFromServer", typeof(IEnumerable<Inline>), 
                typeof(BaseUserControl), new FrameworkPropertyMetadata(null, new PropertyChangedCallback(OnTextFromServerPropertyChanged)));


        static void OnTextFromServerPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            BaseViewModel.Inlines = (IEnumerable<Inline>)e.NewValue;
        }













        //public string CurrentText
        //{
        //    get { return (string)GetValue(CurrentTextProperty); }
        //    set { SetValue(CurrentTextProperty, value); }
        //}

        //public static readonly DependencyProperty CurrentTextProperty =
        //    DependencyProperty.Register("CurrentText", typeof(string), typeof(BaseUserControl));




        //public string WholeTextTyped
        //{
        //    get { return (string)GetValue(WholeTextTypedProperty); }
        //    set { SetValue(WholeTextTypedProperty, value); }
        //}

        //public static readonly DependencyProperty WholeTextTypedProperty =
        //    DependencyProperty.Register("WholeTextTyped", typeof(string), typeof(BaseUserControl));



        //public string BackgroundColor
        //{
        //    get { return (string)GetValue(BackgroundColorProperty); }
        //    set { SetValue(BackgroundColorProperty, value); }
        //}


        //public static readonly DependencyProperty BackgroundColorProperty =
        //    DependencyProperty.Register("BackgroundColor", typeof(string), typeof(BaseUserControl), new FrameworkPropertyMetadata(""));



    }
}
