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
using HttpClientExtras;
using System.Net.Http;
using Coding4Fun.Toolkit.Controls;

namespace WindInfo
{
    public partial class NotWorking : PhoneApplicationPage
    {
        public NotWorking()
        {
            InitializeComponent();
            HttpClientExtras.WP8.PlatformAdapters.Init();
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {

            if (!(textProblem.Text.Length > 0))
                MessageBox.Show(AppResources.MissingText);

            (FindName("Send") as Control).IsEnabled = false;
            SystemTray.SetProgressIndicator(this, new ProgressIndicator { IsVisible = true, IsIndeterminate = true, Text = "Sending..." });

            using (var webclient = new HttpClient(new OAuthProtectedResourceMessageHandler(t => { }) { ConsumerKey = "14a8a3b2e97716d92d21f1d66a58f6f3", ConsumerSecret = "f20f027623385fbafdb905622c78a5f18066ea00255390d3ffb61e16aa9b3dbe", AccessToken = "fd65690a317fbc6454dbbe15108160facc2b8d916889f2f88af9fd8656abe32a" }, true) { BaseAddress = new Uri("https://api.trello.com/1/") })
            {
                var result = await webclient.PostAsJsonAsync("cards",
                new
                {
                    name = textProblem.Text.Substring(0, 10),
                    desc = textProblem.Text.Substring(0, Math.Min(16384, textProblem.Text.Length)),
                    due = "null",
                    idList = "547c43fe357e27da9bf82d6c",
                    urlSource = "null",
                    labels = "blue"
                });

                try
                {
                    result.EnsureSuccessStatusCode();
                    var msg = new MessagePrompt() { Body = AppResources.RequestSent, Title = "SimInfo" };
                    msg.Completed += new EventHandler<PopUpEventArgs<string, PopUpResult>>((s, a) => { NavigationService.GoBack(); });
                    msg.Show();
                }
                catch (Exception)
                {
                    (new EmailComposeTask { To = "vincenz.chianese@yahoo.it", Subject = AppResources.TitleMailError, Body = textProblem.Text }).Show();
                }
                finally
                {
                    SystemTray.SetProgressIndicator(this, new ProgressIndicator { IsVisible = false });
                    (FindName("Send") as Control).IsEnabled = true;
                }

            }

        }

        private void Reset_Click(object sender, RoutedEventArgs e)
        {
            Utils.DeleteCreditState();
            ShellTile.ActiveTiles.Skip(1).ToList().ForEach(x => { if (x != null) x.Delete(); });
            NavigationService.Navigate(new Uri("/MainPage.xaml?FromData=1", UriKind.Relative));
        }
    }
}