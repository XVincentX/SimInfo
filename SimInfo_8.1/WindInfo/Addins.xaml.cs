using Coding4Fun.Toolkit.Controls;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Notification;
using Microsoft.Phone.Shell;
using Newtonsoft.Json;
using StorageHelper.Apollo;
using StringResources;
using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using WindInfo.Code;
using Windows.ApplicationModel.Store;

namespace WindInfo
{
    public partial class Addins : PhoneApplicationPage
    {
        private ListingInformation prods;
        public Addins()
        {
            InitializeComponent();
        }

        private async void PhoneApplicationPage_Loaded(object sender, RoutedEventArgs e)
        {
            prods = await CurrentApp.LoadListingInformationAsync();
            products.ItemsSource = prods.ProductListings.Values.Select(x => new { x.Name, Status = CurrentApp.LicenseInformation.ProductLicenses[x.ProductId].IsActive ? "Purchased" : x.FormattedPrice, x.ImageUri, x.ProductId, BuyNowButtonVisible = CurrentApp.LicenseInformation.ProductLicenses[x.ProductId].IsActive ? System.Windows.Visibility.Collapsed : System.Windows.Visibility.Visible });
            progressRing.IsActive = false;

        }

        private async void buyProduct(object sender, EventArgs e)
        {

            MessageBox.Show(AppResources.AddinRedirect);
            var id = (sender as Button).Tag.ToString();
            try
            {
                var receipt = await CurrentApp.RequestProductPurchaseAsync(id);
                Fulfill(id, receipt);
                new ToastPrompt { Message = AppResources.PurchaseComplete }.Show();

            }
            catch (COMException)
            {
            }
        }

        private async void Fulfill(string item, PurchaseResults receipt)
        {
            var t = CurrentApp.LoadListingInformationAsync();
            int v;
            switch (item)
            {
                case IAPs.IAP_AdditionalLogin:
                    products.ItemsSource = null;
                    progressRing.IsActive = true;
                    var prevData = await Storage.LoadAsync<byte[]>(IAPs.IAP_AdditionalLogin);
                    if (prevData == null)
                    {
                        v = 2;
                    }
                    else
                    {
                        var dData = ProtectedData.Unprotect(prevData, null);
                        int value = BitConverter.ToInt32(dData, 0);
                        v = value + 1;
                    }

                    Save(v);
                    using (var c = new HttpClient())
                    {
                        var result = await c.PostAsync("https://wauth.apphb.com/api/AddPayment", new StringContent(JsonConvert.SerializeObject(new PayingUser { Username = (App.Current as App).currentInfo.Username, Count = v }), Encoding.UTF8, "application/json"));
                    }
                    progressRing.IsActive = false;
                    products.ItemsSource = prods.ProductListings.Values.Select(x => new { x.Name, Status = CurrentApp.LicenseInformation.ProductLicenses[x.ProductId].IsActive ? "Purchased" : x.FormattedPrice, x.ImageUri, x.ProductId, BuyNowButtonVisible = CurrentApp.LicenseInformation.ProductLicenses[x.ProductId].IsActive ? System.Windows.Visibility.Collapsed : System.Windows.Visibility.Visible });
                    break;
                case IAPs.IAP_PushNotification:
                    products.ItemsSource = null;
                    progressRing.IsActive = true;
                    var ch = WPUtils.ChannelStartup();
                    SystemTray.SetProgressIndicator(this, new ProgressIndicator { IsVisible = true, Text = AppResources.PushConnect, IsIndeterminate = true });
                    var tsk = WPUtils.UploadCurrentData();

                    while (ch.ConnectionStatus != ChannelConnectionStatus.Connected && ch.ChannelUri == null)
                        await Task.Delay(1000);

                    await WPUtils.PushNotificationSetUp(this);
                    SystemTray.SetProgressIndicator(this, null);

                    progressRing.IsActive = false;
                    products.ItemsSource = prods.ProductListings.Values.Select(x => new { x.Name, Status = CurrentApp.LicenseInformation.ProductLicenses[x.ProductId].IsActive ? "Purchased" : x.FormattedPrice, x.ImageUri, x.ProductId, BuyNowButtonVisible = CurrentApp.LicenseInformation.ProductLicenses[x.ProductId].IsActive ? System.Windows.Visibility.Collapsed : System.Windows.Visibility.Visible });
                    await tsk;
                    break;
                default:
                    break;
            }

            prods = await t;
            var prod = prods.ProductListings.Single(x => x.Value.ProductId == item);

            if (prod.Value.ProductType == ProductType.Consumable)
            {
                await CurrentApp.ReportConsumableFulfillmentAsync(item, receipt.TransactionId);
            }

        }


        public static void Save(int value)
        {
            var data = BitConverter.GetBytes(value);
            var edata = ProtectedData.Protect(data, null);
            Storage.SaveAsync(IAPs.IAP_AdditionalLogin, edata);
        }

    }

    public class PayingUser
    {
        public string Username { get; set; }
        public int Count { get; set; }
    }
}