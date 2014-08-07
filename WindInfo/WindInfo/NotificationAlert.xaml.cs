using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;

namespace WindInfo
{
    public partial class NotificationAlert : PhoneApplicationPage
    {
        public NotificationAlert()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (NavigationContext.QueryString.Count > 0)
            {
                title.Text = NavigationContext.QueryString["title"];
                message.Text = NavigationContext.QueryString["message"];
            }
            else
            {
                NavigationService.Navigate(new Uri("/DataPage.xaml", UriKind.Relative));
            }
        }
    }
}