
using Cimbalino.Phone.Toolkit.Services;
using Coding4Fun.Toolkit.Controls;
using Coding4Fun.Toolkit.Controls.Common;
using Microsoft.Live;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Net.NetworkInformation;
using Microsoft.Phone.Scheduler;
using Microsoft.Phone.Shell;
using Microsoft.Phone.Tasks;
using Newtonsoft.Json;
using ReviewNotifier.Apollo;
using StorageHelper.Apollo;
using StringResources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Navigation;
using Tiles;
using WindDataLib;
using WindDataLib.Implementations;
using WindInfo.Code;

namespace WindInfo
{
    public partial class MainPage : PhoneApplicationPage
    {

        private ProgressIndicator progress = new ProgressIndicator
        {
            IsVisible = true,
            IsIndeterminate = true,
            Text = AppResources.DownloadingData
        };

        // Constructor
        public MainPage()
        {
            InitializeComponent();

            LLS.ItemsSource = new[] 
            {
                new OperatorType{ Text="Wind",Type="W",Image="Assets/wind.jpg"},
                new OperatorType{ Text="Vodafone",Type="V",Image ="Assets/vodafone.jpg"},
                new OperatorType{ Text="Tim", Type="T", Image="Assets/tim.jpg"},
                new OperatorType{Text="Tre",Type="H", Image="Assets/tre.jpg"},
                new OperatorType{Text="Noverca",Type="N", Image="Assets/Noverca.jpg"},
                new OperatorType{Text="Coop",Type="C", Image="Assets/coop.jpg"},
                new OperatorType{Text="Tiscali",Type="Z", Image="Assets/tiscali.jpg"},
                    new OperatorType{Text="Fastweb",Type="F", Image= "Assets/fastweb.jpg"}
            };


        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            //WPUtils.ChannelStartup();

            if (NavigationContext.QueryString.ContainsKey("FromData"))
            {
                while (NavigationService.RemoveBackEntry() != null)
                { }
            }
            else if (!NavigationContext.QueryString.ContainsKey("OtherLogin"))
            {
                (App.Current as App).currentInfo = Utils.GetSavedState();

                if ((App.Current as App).currentInfo != null && !string.IsNullOrEmpty((App.Current as App).currentInfo.Username) && !string.IsNullOrEmpty((App.Current as App).currentInfo.Password))
                {
                    NavigationService.Navigate(new Uri("/DataPage.xaml?paymentCheck=1", UriKind.Relative));
                }
            }



        }


        private void loginButton_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(usrtxt.Text) || string.IsNullOrEmpty(pwdtxt.Password))
            {
                usrtxt.Focus();
                return;
            }

            if (!NetworkInterface.GetIsNetworkAvailable())
            {
                MessageBox.Show(AppResources.RequiredInternet);
                return;
            }

            progress.IsVisible = true;
            SystemTray.SetProgressIndicator(this, progress);

            notworking.IsEnabled = LLS.IsEnabled = loginButton.IsEnabled = usrtxt.IsEnabled = pwdtxt.IsEnabled = livelogin.IsEnabled = false;

            string usr = usrtxt.Text;
            string pwd = pwdtxt.Password;
            TaskDownloadData(progress, usr, pwd, (LLS.SelectedItem as OperatorType).Type);
        }



        private void TaskDownloadData(ProgressIndicator progress, string usr, string pwd, string Type)
        {
            Task.Factory.StartNew(
              () =>
              {
                  try
                  {
                      if ((App.Current as App).currentInfo.NumberInfos == null)
                          (App.Current as App).currentInfo = CreateWindInformations(usr, pwd, Type);
                      else
                      {
                          var tslot = Storage.LoadAsync<byte[]>(IAPs.IAP_AdditionalLogin);
                          tslot.Wait();

                          var res = tslot.Result;

                          if (res != null && !CanAddNumber(res))
                          {
                              SafeDispatcher.Run(() =>
                              {
                                  progress.IsVisible = false; SystemTray.SetProgressIndicator(this, progress); notworking.IsEnabled = LLS.IsEnabled = livelogin.IsEnabled = loginButton.IsEnabled = usrtxt.IsEnabled = pwdtxt.IsEnabled = true;
                                  var msg = new MessagePrompt { Title = AppResources.NoAviableNumbersTitle, Message = AppResources.NoAviableNumbersMessage, IsCancelVisible = true };
                                  msg.Completed += addin_buy;
                                  msg.Show();
                                  return;
                              });
                              return;
                          }
                          else
                          {
                              var itm = CreateWindInformations(usr, pwd, Type);
                              (App.Current as App).currentInfoArray.Add(itm);
                              Utils.SaveCreditState((App.Current as App).currentInfoArray);

                              foreach (var item in itm.NumberInfos)
                              {
                                  Utils.RenderTiles(item.Number, item);
                              }

                              goto ok;
                          }

                      }


                      if ((App.Current as App).currentInfo.NumberInfos.Count == 0)
                          throw new Exception(AppResources.ErrorConnection);

                      Utils.SaveCreditState((App.Current as App).currentInfo);

                      foreach (var item in (App.Current as App).currentInfo.NumberInfos)
                      {
                          Utils.RenderTiles(item.Number, item);
                      }

                  ok:
                      SafeDispatcher.Run(() => NavigationService.Navigate(new Uri("/DataPage.xaml?paymentCheck=1", UriKind.Relative)));
                      WPUtils.StartPeriodicAgent(UpdateCreditAgent.AgentName);
                      return;
                  }
                  catch (Exception ex)
                  {
                      var msg = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                      SafeDispatcher.Run(() => { progress.IsVisible = false; SystemTray.SetProgressIndicator(this, progress); notworking.IsEnabled = LLS.IsEnabled = livelogin.IsEnabled = loginButton.IsEnabled = usrtxt.IsEnabled = pwdtxt.IsEnabled = true; new MessagePrompt { Message = msg, Title = "SimInfo" }.Show(); });
                      return;
                  }
              });
        }

        private bool CanAddNumber(byte[] res)
        {
            var dpData = ProtectedData.Unprotect(res, null);
            int val = BitConverter.ToInt32(dpData, 0);

            var cnt = (App.Current as App).currentInfoArray.Count;

            return cnt < val;
        }

        private void addin_buy(object sender, PopUpEventArgs<string, PopUpResult> e)
        {
            if (e.PopUpResult == PopUpResult.Ok)
                NavigationService.Navigate(new Uri("/Addins.xaml", UriKind.Relative));
        }

        private static CreditInfo CreateWindInformations(string usr, string pwd, string Type)
        {
            var tsk = CreditInfoRetriever.Get().RetrieveCreditInfo(usr, pwd, Type, Guid.Empty);
            tsk.Wait();
            var cd = tsk.Result;

            for (int i = 0; i < cd.NumberInfos.Count; i++)
            {
                cd.NumberInfos.ElementAt(i).Brush = string.Empty;
                cd.NumberInfos.ElementAt(i).FriendlyName = cd.NumberInfos.ElementAt(i).Number;
            }

            return cd;

        }

        private async void PhoneApplicationPage_Loaded(object sender, RoutedEventArgs e)
        {
            await ReviewNotification.TriggerAsync(AppResources.PleaseRateApp, AppResources.PleaseRateAppTitle, 5);
        }

        private void notworking_Click(object sender, RoutedEventArgs e)
        {
            /*
            var msg = (new MessagePrompt { Message = AppResources.NotWorkingMessage, IsCancelVisible = true, Title = "SimInfo" });
            msg.Completed += msg_Completed;
            msg.Show();
             * */
            NavigationService.Navigate(new Uri("/NotWorking.xaml", UriKind.Relative));
        }


        private void about_Click(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/About.xaml", UriKind.Relative));
        }

        void ThankYou(object sender, RoutedEventArgs e)
        {
            new MessagePrompt() { Title = "Thank you", Message = AppResources.ThankYou, IsCancelVisible = false, IsAppBarVisible = false }.Show();
        }

        private async void livelogin_Click(object sender, RoutedEventArgs e)
        {
            notworking.IsEnabled = LLS.IsEnabled = livelogin.IsEnabled = loginButton.IsEnabled = usrtxt.IsEnabled = pwdtxt.IsEnabled = false;
            var authClient = new LiveAuthClient("000000004812404D");
            LiveLoginResult loginresult = await authClient.InitializeAsync(new string[] { "wl.signin", "wl.skydrive", "wl.skydrive_update" });

            if (loginresult.Status != LiveConnectSessionStatus.Connected)
                loginresult = await authClient.LoginAsync(new string[] { "wl.signin", "wl.skydrive", "wl.skydrive_update" });

            if (loginresult.Status == LiveConnectSessionStatus.Connected)
            {
                var tray = new ProgressIndicator();
                tray.IsVisible = true;
                tray.IsIndeterminate = true;
                SystemTray.SetProgressIndicator(this, tray);

                var client = new LiveConnectClient(loginresult.Session);


                string folder = await DataPage.CreateDirectoryAsync(client, "siminfo", "me/skydrive");
                var result = await DataPage.DownloadFileAsync(client, "siminfo", "data");

                IEnumerable<Tuple<string, string, string>> cr = JsonConvert.DeserializeObject<IEnumerable<Tuple<string, string, string>>>(result);

                (App.Current as App).currentInfo = await WindJsonParsingClient.Get().RetrieveCreditInfo(cr.First().Item1, cr.First().Item2, cr.First().Item3, Guid.Empty);
                (App.Current as App).currentInfoArray = new CreditInfo[cr.Count() - 1];

                for (int i = 1; i < cr.Count(); i++)
                {
                    (App.Current as App).currentInfoArray[i - 1] = await WindJsonParsingClient.Get().RetrieveCreditInfo(cr.ElementAt(i).Item1, cr.ElementAt(i).Item2, cr.ElementAt(i).Item3, Guid.Empty);
                }

                Utils.SaveCreditState((App.Current as App).currentInfo);
                Utils.SaveCreditState((App.Current as App).currentInfoArray);


                tray.IsVisible = false;
                SystemTray.SetProgressIndicator(this, tray);
                notworking.IsEnabled = LLS.IsEnabled = livelogin.IsEnabled = loginButton.IsEnabled = usrtxt.IsEnabled = pwdtxt.IsEnabled = true;

            }
            else
            {
                var p = new MessagePrompt { Body = AppResources.CloudReject, Title = string.Empty };
                p.Show();
            }

            authClient.Logout();
            SafeDispatcher.Run(() => NavigationService.Navigate(new Uri("/DataPage.xaml?paymentCheck=1", UriKind.Relative)));
        }

    }

    public class OperatorType
    {
        public string Text { get; set; }
        public string Type { get; set; }
        public string Image { get; set; }
    }
}