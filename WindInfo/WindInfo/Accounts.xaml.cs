using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System.Collections;
using WindInfo.Code;
using System.Security.Cryptography;
using Coding4Fun.Toolkit.Controls;
using StringResources;
using WindDataLib;
using StorageHelper.Apollo;

namespace WindInfo
{
    public partial class Accounts : PhoneApplicationPage
    {
        public Accounts()
        {
            InitializeComponent();
            if ((App.Current as App).currentInfoArray != null)
                NumberList.ItemsSource = (IList)(new List<CreditInfo>((App.Current as App).currentInfoArray));

            NumberList.ItemsSource.Add((App.Current as App).currentInfo);
        }

        private void StackPanel_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {

            NavigationService.Navigate(new Uri(string.Format("/DataPage.xaml?number={0}", HttpUtility.UrlEncode((sender as FrameworkElement).Tag.ToString())), UriKind.Relative));
        }

        private async void Add_Click(object sender, EventArgs e)
        {
            var prevData = await Storage.LoadAsync<byte[]>(IAPs.IAP_AdditionalLogin);
            if (prevData == null)
            {
                var msg = new MessagePrompt { Title = AppResources.NoAviableNumbersTitle, Message = AppResources.NoAviableNumbersMessage, IsCancelVisible = true };
                msg.Completed += msg_Completed;
                msg.Show();
                return;
            }
            var acc = BitConverter.ToInt32(ProtectedData.Unprotect(prevData, null), 0);

            if (acc <= 1 + (App.Current as App).currentInfoArray.Count())
            {
                var msg = new MessagePrompt { Title = AppResources.NoAviableNumbersTitle, Message = AppResources.NoAviableNumbersMessage, IsCancelVisible = true };
                msg.Completed += msg_Completed;
                msg.Show();
                return;
            }


            NavigationService.Navigate(new Uri("/MainPage.xaml?OtherLogin=1", UriKind.Relative));
        }

        void msg_Completed(object sender, PopUpEventArgs<string, PopUpResult> e)
        {
            if (e.PopUpResult == PopUpResult.Ok)
                NavigationService.Navigate(new Uri("/Addins.xaml", UriKind.Relative));
        }

        private void MenuItem_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            var msg = new MessagePrompt { Tag = (sender as FrameworkElement).Tag, Title = AppResources.LogoutTitle, Message = AppResources.LogoutMessage, IsAppBarVisible = false, IsCancelVisible = true };
            msg.Completed += logout_message;
            msg.Show();
        }

        private void logout_message(object sender, PopUpEventArgs<string, PopUpResult> e)
        {
            if (e.PopUpResult == PopUpResult.Ok)
            {
                string number = (sender as FrameworkElement).Tag.ToString();

                var current = Utils.GetSavedState();
                var arr = Utils.GetArraySavedState();

                if (current.NumberInfos.Count(x => x.Number == number) > 0)
                {
                    if (arr.Count > 0)
                    {
                        current = arr.First();
                        arr.RemoveAt(0);
                        Utils.SaveCreditState(current);
                        Utils.SaveCreditState(arr);
                    }
                    else
                    {
                        Utils.DeleteCreditState();
                        (App.Current as App).currentInfoArray = Enumerable.Empty<CreditInfo>().ToList();
                        (App.Current as App).currentInfo = new CreditInfo();

                        NavigationService.Navigate(new Uri("/MainPage.xaml?FromData=1", UriKind.Relative));
                    }

                    foreach (var n in current.NumberInfos)
                        foreach (var sh in ShellTile.ActiveTiles.Where(s => s.NavigationUri.ToString().Contains(n.Number)))
                            sh.Delete();

                }
                else
                {
                    //Delete the tiles.
                    var elem = arr.First(x => x.NumberInfos.Count(y => y.Number == number) > 0);

                    foreach (var n in elem.NumberInfos)
                    {
                        foreach (var sh in ShellTile.ActiveTiles.Where(s => s.NavigationUri.ToString().Contains(n.Number)))
                            sh.Delete();
                    }
                    arr.Remove(elem);


                }

                Utils.SaveCreditState(arr);
                (App.Current as App).currentInfoArray = arr;
                (App.Current as App).currentInfo = current;

                NumberList.ItemsSource = (IList)(new List<CreditInfo>((App.Current as App).currentInfoArray));
                NumberList.ItemsSource.Add((App.Current as App).currentInfo);

            }
        }
    }
}