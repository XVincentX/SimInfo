using Coding4Fun.Toolkit.Controls;
using Coding4Fun.Toolkit.Controls.Common;
using Microsoft.Phone.Notification;
using Microsoft.Phone.Scheduler;
using Newtonsoft.Json;
using StringResources;
using System;
using System.IO;
using System.IO.IsolatedStorage;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Xml;
using Tiles;
using System.Linq;
using Windows.Phone.System.Power;
using System.Net;
using System.Collections.ObjectModel;
using Microsoft.WindowsAzure.Messaging;
using WindDataLib.Model;
using System.Collections.Generic;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System.Windows.Media;
using Microsoft.Live;


namespace WindInfo.Code
{
    public static class WPUtils
    {

        public const string baseDir = "/Shared/ShellContent/";

        public static string GetAppAttributeValue(string attributeName)
        {
            var xmlReaderSettings = new XmlReaderSettings
            {
                XmlResolver = new XmlXapResolver()
            };
            using (var xmlReader = XmlReader.Create("WMAppManifest.xml", xmlReaderSettings))
            {
                xmlReader.ReadToDescendant("App");

                return xmlReader.GetAttribute(attributeName);
            }
        }
        public static void RemoveAgent(string name)
        {
            try
            {
                ScheduledActionService.Remove(name);
            }
            catch (Exception)
            {
            }
        }
        public static void StartPeriodicAgent(string Name)
        {

            var periodicTask = ScheduledActionService.Find(Name) as PeriodicTask;

            // If the task already exists and background agents are enabled for the
            // application, you must remove the task and then add it again to update 
            // the schedule
            if (periodicTask != null)
            {
                RemoveAgent(Name);
            }

            periodicTask = new PeriodicTask(Name) { Description = AppResources.TaskDescription };

            try
            {
                ScheduledActionService.Add(periodicTask);

#if DEBUG
                ScheduledActionService.LaunchForTest(Name, TimeSpan.FromSeconds(10));
#endif

            }
            catch (InvalidOperationException exception)
            {

                if (exception.Message.Contains("BNS Error: The action is disabled"))
                {
                }

                if (exception.Message.Contains("BNS Error: The maximum number of ScheduledActions of this type have already been added."))
                {
                }
            }
            catch (SchedulerServiceException)
            {
            }
        }

        public static bool CheckBatterySaverState()
        {
            // The minimum phone version that supports the PowerSavingModeEnabled property
            Version TargetVersion = new Version(8, 0, 10492);

            if (Environment.OSVersion.Version >= TargetVersion)
            {
                // Use reflection to get the PowerSavingModeEnabled value
                bool powerSaveOn = (bool)
                    typeof(PowerManager).GetProperty("PowerSavingModeEnabled").GetValue(null, null);

                return powerSaveOn;
            }

            return false;
        }

        public static async Task PushNotificationSetUp(DependencyObject page)
        {

            var channel = ChannelStartup();
            channel.ChannelUriUpdated += channel_UploadUri;


            if (channel.ChannelUri != null)
                await UploadUri(channel.ChannelUri);
        }

        public static HttpNotificationChannel ChannelStartup()
        {
            var channel = HttpNotificationChannel.Find(Constants.CnName);
            if (channel == null)
                channel = new HttpNotificationChannel(Constants.CnName);

            channel.ShellToastNotificationReceived += channel_ShellToastNotificationReceived;
            channel.ErrorOccurred += channel_ErrorOccurred;

            if (channel.ConnectionStatus == ChannelConnectionStatus.Disconnected)
                try
                {
                    channel.Open();
                }
                catch
                {
                    //Already opening. Let's wait.
                }

            if (!channel.IsShellTileBound)
            {
                var uris = new Collection<Uri>();
                uris.Add(new Uri("http://wauth.apphb.com"));
                channel.BindToShellTile(uris);
            }

            if (!channel.IsShellToastBound)
                channel.BindToShellToast();

            return channel;
        }

        public static async Task UpdateFromCloud()
        {
            var userList = (App.Current as App).currentInfoArray.Concat(Enumerable.Repeat((App.Current as App).currentInfo, 1));
            var table = App.mobileClient.GetTable<PlainNumberInfo>();

            foreach (var u in userList)
            {
                foreach (var item in u.NumberInfos)
                {
                    IEnumerable<PlainNumberInfo> data = await table.Where(x => x.Number == item.Number).ToEnumerableAsync();
                    if (data.Count() != 0)
                    {
                        var d = data.First();
                        item.Credit = d.Credit;
                        item.Minutes = d.Minutes;
                        item.Gigabytes = d.Gigabytes;
                        item.SMS = d.SMS;
                        item.LastUpdate = d.LastUpdate;
                    }

                }
            }

            Utils.SaveCreditState((App.Current as App).currentInfo);
            Utils.SaveCreditState((App.Current as App).currentInfoArray);

        }
        static void channel_ErrorOccurred(object sender, NotificationChannelErrorEventArgs e)
        {
            SafeDispatcher.Run(() => { new ToastPrompt { Message = e.Message }.Show(); new ToastPrompt { Message = e.ErrorType.ToString() }.Show(); });
        }

        static void channel_ShellToastNotificationReceived(object sender, NotificationEventArgs e)
        {

            SafeDispatcher.Run(() => { var toast = new ToastPrompt { Title = e.Collection.First().Value, Message = e.Collection.Skip(1).First().Value, IsTimerEnabled = true, MillisecondsUntilHidden = 3000 }; toast.Show(); });
        }

        private static async void channel_UploadUri(object sender, NotificationChannelUriEventArgs e)
        {
            await UploadUri(e.ChannelUri);
            await UploadCurrentData();
        }

        static async Task UploadUri(Uri uri)
        {

            var hub = NotificationGet();
            var userList = (App.Current as App).currentInfoArray.Concat(Enumerable.Repeat((App.Current as App).currentInfo, 1));
            var jsData = userList.Select(x => string.Join("_", x.Username, x.Password, x.Type)).ToArray();
            var registration = await hub.RegisterNativeAsync(uri.ToString(), jsData);
            WPUtils.RemoveAgent(UpdateCreditAgent.AgentName);
            SafeDispatcher.Run(() => { new ToastPrompt { Message = "Push notification ok!", Background = new SolidColorBrush(Colors.Green), IsTimerEnabled = true, MillisecondsUntilHidden = 2000 }.Show(); });

        }

        public static NotificationHub NotificationGet()
        {
            return new NotificationHub("siminfohub", "Endpoint=sb://siminfohub-ns.servicebus.windows.net/;SharedAccessKeyName=DefaultListenSharedAccessSignature;SharedAccessKey=VWLZBn5G9omKPPLb51MY8SJagsZBPkbzbl2fMiIf580=");
        }

        public static async Task UploadCurrentData()
        {
            var userPrefs = App.mobileClient.GetTable<PlainNumberInfo>();

            var userList = (App.Current as App).currentInfoArray.Concat(Enumerable.Repeat((App.Current as App).currentInfo, 1));
            List<Task> tk = new List<Task>();

            foreach (var item in userList.SelectMany(x => x.NumberInfos))
            {
                IEnumerable<PlainNumberInfo> itm = await userPrefs.Where(p => p.Number == item.Number).ToEnumerableAsync();

                if (itm.Count() == 0)
                {
                    var instance = new PlainNumberInfo { Number = item.Number, Brush = item.Brush, CreditLimit = item.CreditLimit, FriendlyName = item.FriendlyName, GigabytesLimit = item.GigabytesLimit, SMSLimit = item.SMSLimit, smsShowed = false, clShowed = false, gigaShowed = false, minShowed = false, MinutesLimit = item.MinutesLimit, NotifyEnabled = item.NotifyEnabled };
                    tk.Add(userPrefs.InsertAsync(instance));
                }
                else
                {
                    var instance = itm.First();
                    instance.CreditLimit = item.CreditLimit;
                    instance.GigabytesLimit = item.GigabytesLimit;
                    instance.MinutesLimit = item.MinutesLimit;
                    instance.SMSLimit = item.SMSLimit;
                    instance.FriendlyName = item.FriendlyName;
                    instance.Brush = item.Brush;
                    instance.NotifyEnabled = item.NotifyEnabled;
                    tk.Add(userPrefs.UpdateAsync(instance));
                }

            }

            foreach (var item in tk)
                await item;
        }
    }
}
