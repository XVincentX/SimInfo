using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Microsoft.Phone.Tasks;
using StringResources;
using WindInfo.Code;

namespace WindInfo
{
    public partial class NotWorking : PhoneApplicationPage
    {
        public NotWorking()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (textProblem.Text.Length > 0)
                (new EmailComposeTask { To = "vincenz.chianese@yahoo.it", Subject = AppResources.TitleMailError, Body = textProblem.Text }).Show();
            else
                MessageBox.Show(AppResources.MissingText);
        }

        private void Reset_Click(object sender, RoutedEventArgs e)
        {
            Utils.DeleteCreditState();
            ShellTile.ActiveTiles.Skip(1).ToList().ForEach(x => { if (x != null) x.Delete(); });
            NavigationService.Navigate(new Uri("/MainPage.xaml?FromData=1", UriKind.Relative));
        }
    }
}