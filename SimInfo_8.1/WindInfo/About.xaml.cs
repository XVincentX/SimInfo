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
using Coding4Fun.Toolkit.Controls;
using System.Windows.Input;
using Tiles;
using System.Globalization;
using StringResources;
using WindInfo.Code;

namespace WindInfo
{
    public partial class About : PhoneApplicationPage
    {
        public About()
        {
            InitializeComponent();
        }

        private void HubTile_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            e.Handled = true;
            new WebBrowserTask() { Uri = new Uri((sender as FrameworkElement).Tag.ToString(), UriKind.Absolute) }.Show();
        }

        private async void Donate_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            var InSc = new System.Windows.Input.InputScope();
            InputScopeName name = new InputScopeName();

            name.NameValue = InputScopeNameValue.Number;
            InSc.Names.Add(name);
            var initialValue = (2.0f).ToString("N2");

            const string DonationItem = "Donazione libera";

            PayPal.Checkout.BuyNow purchase = new PayPal.Checkout.BuyNow("vincenz.chianese@yahoo.it") { UseSandbox = false, DisplayShipping = 1, ConfirmedShippingOnly = false, Currency = "EUR" };
            var item = new PayPal.Checkout.ItemBuilder(DonationItem)
                .Name(DonationItem)
                .Description(DonationItem)
                .ID("DON")
                .Price(float.Parse(initialValue).ToString(new NumberFormatInfo { CurrencyDecimalDigits = 2, NumberDecimalDigits = 2, CurrencyDecimalSeparator = ".", NumberDecimalSeparator = ".", NumberGroupSeparator = ",", CurrencyGroupSeparator = "," }))
                .Tax("0.00")
                //.AddMetadata("type", "Digital") //It must be included. Modify paypal account to allow this
                .Quantity(1);

            purchase.AddItem(item);


            purchase.Error += new EventHandler<PayPal.Checkout.Event.ErrorEventArgs>((source, eventArg) =>
            {
                new MessagePrompt { IsCancelVisible = false, Title = "SimInfo", Message = string.Format(AppResources.DonateError, eventArg.Message) }.Show();
                SystemTray.SetProgressIndicator(this, new ProgressIndicator() { Text = "", IsIndeterminate = true, IsVisible = false });
            });
            purchase.Complete += new EventHandler<PayPal.Checkout.Event.CompleteEventArgs>((source, eventArg) =>
            {
                new MessagePrompt { IsCancelVisible = false, Title = "SimInfo", Message = AppResources.DonateSuccess }.Show();
                SystemTray.SetProgressIndicator(this, new ProgressIndicator() { Text = "", IsIndeterminate = true, IsVisible = false });
            });
            purchase.Cancel += new EventHandler<PayPal.Checkout.Event.CancelEventArgs>((source, eventArg) =>
            {
                new MessagePrompt { IsCancelVisible = false, Title = "SimInfo", Message = AppResources.DonateCancel }.Show();
                SystemTray.SetProgressIndicator(this, new ProgressIndicator() { Text = "", IsIndeterminate = true, IsVisible = false });
            });
            purchase.Start += new EventHandler<PayPal.Checkout.Event.StartEventArgs>((source, eventArg) =>
            {
                SystemTray.SetProgressIndicator(this, new ProgressIndicator() { Text = AppResources.PaypalLoading, IsIndeterminate = true, IsVisible = true });

            });
            purchase.Auth += new EventHandler<PayPal.Checkout.Event.AuthEventArgs>((source, eventArg) =>
            {
                SystemTray.SetProgressIndicator(this, new ProgressIndicator() { Text = string.Format(AppResources.PayPalAuth, eventArg.Token), IsIndeterminate = true, IsVisible = false });
            });

            bool res = await purchase.Execute();

        }


        private void TextBlock_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            NavigationService.Navigate(new Uri((e.OriginalSource as TextBlock).Tag.ToString(), UriKind.Relative));
        }

        private void TextBlock_Tap_1(object sender, System.Windows.Input.GestureEventArgs e)
        {
            new MarketplaceReviewTask().Show();
        }

        private void TextBlock_Tap_2(object sender, System.Windows.Input.GestureEventArgs e)
        {
            new EmailComposeTask { To = "vincenz.chianese@yahoo.it", Subject = AppResources.MailSubject }.Show();
        }

        private void TextBlock_Tap_3(object sender, System.Windows.Input.GestureEventArgs e)
        {
            new MessagePrompt { Title = "Thank you", Message = AppResources.ThankYou }.Show();
        }

        private void TextBlock_Tap_4(object sender, System.Windows.Input.GestureEventArgs e)
        {
            NavigationService.Navigate(new Uri("/news.xaml", UriKind.Relative));
        }
    }
}