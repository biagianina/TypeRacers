using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;

namespace TypeRacers.ViewModel
{
    //extensions for using inlines property of textblock outside xaml/xaml.cs code
    //binded in the xaml with code (<TextBlock vm:TextBlockExtensions.BindableInlines="{Binding Inlines, UpdateSourceTrigger=PropertyChanged}")
    //vm stands for the local resource, Inlines stands for the property in ViewModel
    //Read more documentation about DependencyPropery and DependencyObjecy !!!!
    public class TextBlockExtensions
    {
        public static IEnumerable<Inline> GetBindableInlines(DependencyObject obj)
        {
            return (IEnumerable<Inline>)obj.GetValue(BindableInlinesProperty);
        }

        public static void SetBindableInlines(DependencyObject obj, IEnumerable<Inline> value)
        {
            obj.SetValue(BindableInlinesProperty, value);
        }

        public static readonly DependencyProperty BindableInlinesProperty =
            DependencyProperty.RegisterAttached("BindableInlines", typeof(IEnumerable<Inline>),
                typeof(TextBlockExtensions), new PropertyMetadata(null, OnBindableInlinesChanged));

        private static void OnBindableInlinesChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is TextBlock target)
            {
                target.Inlines.Clear();
                target.Inlines.AddRange((System.Collections.IEnumerable)e.NewValue);
            }
        }
    }
}