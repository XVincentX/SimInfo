using Microsoft.Phone.Info;
using Microsoft.Phone.Net.NetworkInformation;
using Microsoft.Phone.Scheduler;
using Microsoft.Phone.Shell;
using StringResources;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using Tiles;
using WindDataLib;

namespace WindInfo.Code
{
    public class UpdateCreditAgent : ScheduledTaskAgent
    {
        /// <remarks>
        /// ScheduledAgent constructor, initializes the UnhandledException handler
        /// </remarks>
        static UpdateCreditAgent()
        {
            // Subscribe to the managed exception handler
            /*
            Deployment.Current.Dispatcher.BeginInvoke(delegate
            {
                Application.Current.UnhandledException += UnhandledException;
            });
             * */
        }

        public static string AgentName = "UpdateCredit";

        /// <summary>
        /// Agent that runs a scheduled task
        /// </summary>
        /// <param name="task">
        /// The invoked task
        /// </param>
        /// <remarks>
        /// This method is called when a periodic or resource intensive task is invoked
        /// </remarks>
        protected async override void OnInvoke(ScheduledTask task)
        {
            DebugOutputMemoryUsage("Agent start");
            try
            {
                var c = Utils.GetSavedState();
                var o = Utils.GetArraySavedState();

                var current = Enumerable.Repeat(c, 1).Concat(o).OrderByDescending(x => x.NumberInfos.First().LastUpdate).First();
                var nw = await CreditInfoRetriever.Get().RetrieveCreditInfo(current.Username, current.Password, current.Type, Guid.Empty);

                if (current == null || nw == null || string.IsNullOrEmpty(current.Password) || string.IsNullOrEmpty(current.Username) || !NetworkInterface.GetIsNetworkAvailable())
                {
                    //Nothing to do here.
                    NotifyComplete();
                    return;
                }


                DebugOutputMemoryUsage("Credit downloaded");
                current.Merge(nw);

                #region Toast display

                foreach (var number in current.NumberInfos.Where(n => n.NotifyEnabled))
                {

                    var toast = (new ShellToast { Title = AppResources.GenericLimit, NavigationUri = new Uri(string.Concat("/DataPage.xaml?number=", HttpUtility.UrlEncode(number.Number)), UriKind.Relative) });

                    if (number.CreditLimitReached && !number.clShowed)
                    {
                        toast.Content = string.Format(AppResources.CreditLimitReached, number.FriendlyName);
                        number.clShowed = true;
                        toast.Show();
                    }
                    else if (number.Credit > number.CreditLimit)
                        number.clShowed = false;

                    if (number.SMSLimitReached && !number.smsShowed)
                    {
                        toast.Content = string.Format(AppResources.SMSLimitReached, number.FriendlyName);
                        number.smsShowed = true;
                        toast.Show();
                    }
                    else if (number.SMS > number.SMSLimit)
                        number.smsShowed = false;

                    if (number.GigabytesLimitReached && !number.gigaShowed)
                    {
                        number.gigaShowed = true;
                        toast.Content = string.Format(AppResources.TrafficLimitReached, number.FriendlyName);
                        toast.Show();
                    }
                    else if (number.Gigabytes > number.GigabytesLimit)
                        number.gigaShowed = false;

                    if (number.MinutesLimitReached && !number.minShowed)
                    {
                        number.minShowed = true;
                        toast.Content = string.Format(AppResources.MinutesLimitReached, number.FriendlyName);
                        toast.Show();
                    }
                    else if (number.Minutes > number.MinutesLimit)
                        number.minShowed = false;
                }
                #endregion

                var tsk = Task.Factory.StartNew(new Action(() =>
                {
                    Utils.SaveCreditState(o);
                    Utils.SaveCreditState(c);
                }));

                const string baseDir = "/Shared/ShellContent/";
                DebugOutputMemoryUsage("Starting rendering");
                foreach (var item in current.NumberInfos)
                {
                    DebugOutputMemoryUsage("Before");
                    tsk.Wait();
                    Utils.RenderTiles(item.Number, item);
                    DebugOutputMemoryUsage("After");

                    if (ShellTile.ActiveTiles.Any(t => t.NavigationUri.ToString().Contains(item.Number)))
                    {
                        var smallpath = Path.Combine(baseDir, string.Format("{0}_{1}_{2}.jpg", 159, 159, item.Number));
                        var normalpath = Path.Combine(baseDir, string.Format("{0}_{1}_{2}.jpg", 336, 336, item.Number));
                        //var widepath = Path.Combine(baseDir, string.Format("{0}_{1}_{2}.jpg", 691, 336, item.Number));

                        FlipTileData tileData = new FlipTileData
                        {
                            Title = " ",
                            //WideBackgroundImage = new Uri("isostore:" + widepath, UriKind.Absolute),
                            BackgroundImage = new Uri("isostore:" + normalpath, UriKind.Absolute),
                            SmallBackgroundImage = new Uri("isostore:" + smallpath, UriKind.Absolute),
                            BackBackgroundImage = new Uri("isostore:" + normalpath, UriKind.Absolute),
                            //WideBackBackgroundImage = new Uri("isostore:" + widepath, UriKind.Absolute),
                        };
                        ShellTile.ActiveTiles.Single(t => t.NavigationUri.ToString().Contains(item.Number)).Update(tileData);
                    }
                }

                await tsk;

            }
            catch (Exception e)
            {
                if (e.Message.Contains("err") || e.Message.Contains("pass") || e.Message.Contains("user") || e.Message.Contains("valid"))
                {
                    ShellToast t = new ShellToast { Content = e.Message };
                    t.Show();
                }
            }
            finally
            {
                NotifyComplete();
            }


        }

        [Conditional("DEBUG")]
        protected static void DebugOutputMemoryUsage(string label = null)
        {
            var limit = DeviceStatus.ApplicationMemoryUsageLimit;
            var current = DeviceStatus.ApplicationCurrentMemoryUsage;
            var remaining = limit - current;
            var peak = DeviceStatus.ApplicationPeakMemoryUsage;
            var safetyMargin = limit - peak;

            if (label != null)
            {
                Debug.WriteLine(label);
            }
            Debug.WriteLine("Memory limit (bytes): " + limit);
            Debug.WriteLine("Current memory usage: {0} bytes ({1} bytes remaining)", current, remaining);
            Debug.WriteLine("Peak memory usage: {0} bytes ({1} bytes safety margin)", peak, safetyMargin);
        }


    }
}
