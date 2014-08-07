using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;

namespace Tiles
{
    public partial class _159_159 : UserControl
    {
        public _159_159()
        {
            InitializeComponent();
        }

        private void StackPanel_Loaded(object sender, RoutedEventArgs e)
        {
            var panel = sender as StackPanel;
#if WINDOWS_PHONE
            var objs = panel.Children.Where(x => x.Visibility == System.Windows.Visibility.Collapsed).ToArray();
#else
            var objs = panel.Children.Cast<UIElement>().Where(x => x.Visibility == System.Windows.Visibility.Collapsed).ToArray();
#endif

            for (int i = 0; i < objs.Count(); i++)
            {
                panel.Children.Remove(objs.ElementAt(i));
            }
        }

    }
}