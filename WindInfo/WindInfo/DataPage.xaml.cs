using BugSense;
using Cimbalino.Phone.Toolkit.Services;
using Coding4Fun.Toolkit.Controls;
using Coding4Fun.Toolkit.Controls.Common;
using Microsoft.Live;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Net.NetworkInformation;
using Microsoft.Phone.Notification;
using Microsoft.Phone.Scheduler;
using Microsoft.Phone.Shell;
using Newtonsoft.Json;
using StringResources;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using Tiles;
using WindDataLib;
using WindInfo.Code;
using Windows.ApplicationModel.Store;
using Windows.System;
using CimbalinoBeh = Cimbalino.Phone.Toolkit.Behaviors;

namespace WindInfo
{
    public sealed partial class DataPage : PhoneApplicationPage, IDisposable, IProgress<LiveOperationProgress>
    {
        private bool isUpdatding = false;
        private ShakeDetector _shakeSensor;
        PageOrientation lastOrientation;
        public DataPage()
        {
            InitializeComponent();

            Loaded += (sender, args) =>
            {
                _shakeSensor = new ShakeDetector();
                _shakeSensor.ShakeDetected += ShakeDetected;
                _shakeSensor.Start();

            };
            Unloaded += (sender, args) =>
            {
                _shakeSensor.ShakeDetected -= ShakeDetected;
                _shakeSensor.Stop();
                _shakeSensor.Dispose();
            };

            this.OrientationChanged += new EventHandler<OrientationChangedEventArgs>(OritantationChanged);

            lastOrientation = this.Orientation;
        }

        private void OritantationChanged(object sender, OrientationChangedEventArgs e)
        {
            PageOrientation newOrientation = e.Orientation;

            // Orientations are (clockwise) 'PortraitUp', 'LandscapeRight', 'LandscapeLeft'

            RotateTransition transitionElement = new RotateTransition();

            switch (newOrientation)
            {
                case PageOrientation.Landscape:
                case PageOrientation.LandscapeRight:
                    // Come here from PortraitUp (i.e. clockwise) or LandscapeLeft?
                    if (lastOrientation == PageOrientation.PortraitUp)
                        transitionElement.Mode = RotateTransitionMode.In90Counterclockwise;
                    else
                        transitionElement.Mode = RotateTransitionMode.In180Clockwise;
                    break;
                case PageOrientation.LandscapeLeft:
                    // Come here from LandscapeRight or PortraitUp?
                    if (lastOrientation == PageOrientation.LandscapeRight)
                        transitionElement.Mode = RotateTransitionMode.In180Counterclockwise;
                    else
                        transitionElement.Mode = RotateTransitionMode.In90Clockwise;
                    break;
                case PageOrientation.Portrait:
                case PageOrientation.PortraitUp:
                    // Come here from LandscapeLeft or LandscapeRight?
                    if (lastOrientation == PageOrientation.LandscapeLeft)
                        transitionElement.Mode = RotateTransitionMode.In90Counterclockwise;
                    else
                        transitionElement.Mode = RotateTransitionMode.In90Clockwise;
                    break;
                default:
                    break;
            }

            // Execute the transition
            PhoneApplicationPage phoneApplicationPage = (PhoneApplicationPage)(((PhoneApplicationFrame)Application.Current.RootVisual)).Content;
            ITransition transition = transitionElement.GetTransition(phoneApplicationPage);
            transition.Completed += delegate
            {
                transition.Stop();
            };
            transition.Begin();

            lastOrientation = newOrientation;
        }



        private void ShakeDetected(object sender, ShakeDetectedEventArgs e)
        {

            SafeDispatcher.Run(() =>
            {
                var pdc = new ProgressIndicator { IsIndeterminate = true, IsVisible = true, Text = string.Format(AppResources.UpdateSingleNumber, string.Empty) };
                SystemTray.SetProgressIndicator(this, pdc);
            });


            Task.Run(() =>
            {


                if (Update(RetrieveAgain: e.Retrieve))
                    SafeDispatcher.Run(() =>
                    {

                        var pdc = SystemTray.GetProgressIndicator(this);
                        pdc.IsVisible = false;
                        SystemTray.SetProgressIndicator(this, pdc);
                    });

            });


        }

        private bool Update(bool RetrieveAgain)
        {
            if (isUpdatding)
                return false;

            try
            {
                isUpdatding = true;
                var current = (App.Current as App).currentInfo;
                var currArray = (App.Current as App).currentInfoArray;

                if (currArray == null)
                    currArray = Enumerable.Empty<CreditInfo>().ToList();
                if (current == null)
                {
                    SafeDispatcher.Run(() =>
                    {
                        MessageBox.Show(AppResources.Whoops);
                        NavigationService.Navigate(new Uri("/NotWorking.xaml", UriKind.Relative));
                    });
                    return true;
                }

                int i = 0;
                int tot = currArray.Count + 1;

                foreach (var item in currArray.Concat(Enumerable.Repeat(current, 1)))
                {
                    if (item == null || string.IsNullOrEmpty(item.Password) || string.IsNullOrEmpty(item.Username) || !NetworkInterface.GetIsNetworkAvailable())
                    {
                        //Nothing to do here.
                        continue;
                    }

                    if (RetrieveAgain)
                    {
                        SafeDispatcher.Run(() =>
                        {
                            var pr = SystemTray.GetProgressIndicator(this);
                            pr.Text = string.Format(AppResources.UpdateSingleNumber, item.Username);
                            pr.IsIndeterminate = false;
                            pr.Value = (((double)i++) / (double)tot);

                        });
                        var tsk1 = CreditInfoRetriever.Get().RetrieveCreditInfo(item.Username, item.Password, item.Type, Guid.Empty);
                        tsk1.Wait();
                        var nw = tsk1.Result;
                        if (nw == null)
                            return true;
                        SafeDispatcher.Run(() => item.Merge(nw));



                        var Img = new ImageConverter();

                        foreach (var itm in item.NumberInfos)
                        {
                            Utils.RenderTiles(itm.Number, itm);

                            var smallpath = System.IO.Path.Combine(WPUtils.baseDir, string.Format("{0}_{1}_{2}.jpg", 159, 159, itm.Number));
                            var normalpath = System.IO.Path.Combine(WPUtils.baseDir, string.Format("{0}_{1}_{2}.jpg", 336, 336, itm.Number));
                            //var widepath = System.IO.Path.Combine(WPUtils.baseDir, string.Format("{0}_{1}_{2}.jpg", 691, 336, item.Number));

                            SafeDispatcher.Run(() =>
                            {
                                foreach (var str in new[] { new[] { "336", "336" }, new[] { "159", "159" }/*, new[] { "691", "336" }*/ })
                                {
                                    var hubtile = FindControl<HubTile>(this.pivotNumbers.ItemContainerGenerator.ContainerFromIndex(pivotNumbers.SelectedIndex), str[0]);
                                    if (hubtile != null)
                                    {
                                        hubtile.Source = Img.Convert(new[] { str[0], str[1], (pivotNumbers.SelectedItem as NumberInfo).Number }, null, null, null) as BitmapImage;
                                        hubtile.Source = Img.Convert(new[] { str[0], str[1], (pivotNumbers.SelectedItem as NumberInfo).Number }, null, null, null) as BitmapImage;
                                    }
                                }
                            });
                            if (ShellTile.ActiveTiles.Any(t => t.NavigationUri.ToString().Contains(itm.Number)))
                            {
                                FlipTileData tileData = new FlipTileData
                                {
                                    Title = " ",
                                    //WideBackgroundImage = new Uri("isostore:" + widepath, UriKind.Absolute),
                                    BackgroundImage = new Uri("isostore:" + normalpath, UriKind.Absolute),
                                    SmallBackgroundImage = new Uri("isostore:" + smallpath, UriKind.Absolute),
                                    BackBackgroundImage = new Uri("isostore:" + normalpath, UriKind.Absolute),
                                    //WideBackBackgroundImage = new Uri("isostore:" + widepath, UriKind.Absolute),
                                };
                                ShellTile.ActiveTiles.Single(t => t.NavigationUri.ToString().Contains(itm.Number)).Update(tileData);


                            }
                        }

                    }

                }

                Utils.SaveCreditState(current);
                Utils.SaveCreditState(currArray);

                return true;
            }
            catch (Exception e)
            {
                SafeDispatcher.Run(() => MessageBox.Show(e.Message));

            }
            finally
            {
                isUpdatding = false;
            }

            return true;

        }

        void prt_Completed(object sender, PopUpEventArgs<string, PopUpResult> e)
        {
            if (e.PopUpResult != PopUpResult.Cancelled)
            {
                IsolatedStorageSettings.ApplicationSettings[(sender as MessagePrompt).Tag.ToString()] = true;
                IsolatedStorageSettings.ApplicationSettings.Save();
            }

            if (e.PopUpResult == PopUpResult.Ok)
            {
                if (Application.Current.RootVisual as PhoneApplicationFrame != null)
                    (Application.Current.RootVisual as PhoneApplicationFrame).Navigate(new Uri("/Addins.xaml", UriKind.Relative));
            }
        }

        protected async override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {

            if (e.NavigationMode == System.Windows.Navigation.NavigationMode.New || NavigationContext.QueryString.ContainsKey("Reload"))
            {
                if ((App.Current as App).currentInfo == null) (App.Current as App).currentInfo = Utils.GetSavedState();
                (App.Current as App).currentInfoArray = Utils.GetArraySavedState();

                if (IsolatedStorageSettings.ApplicationSettings.Contains("AddinPub") == false)
                {
                    var p = new MessagePrompt { Body = new addinbanner(), Title = string.Empty, Tag = "AddinPub" };
                    p.Completed += prt_Completed;
                    p.Show();
                }

                if (IsolatedStorageSettings.ApplicationSettings.Contains("PushNotificationPub") == false)
                {
                    var p = new MessagePrompt { Body = new pushnotificationbanner(), Title = string.Empty, Tag = "PushNotificationPub" };
                    p.Completed += prt_Completed;
                    p.Show();
                }

                DataContext = new CreditInfo();
                (DataContext as CreditInfo).NumberInfos = new System.Collections.ObjectModel.ObservableCollection<NumberInfo>();

                if ((App.Current as App).currentInfo != null)
                {
                    foreach (var n in (App.Current as App).currentInfo.NumberInfos)
                    {
                        if (n != (object)null)
                            (DataContext as CreditInfo).NumberInfos.Add(n);
                    }

                    BugSenseHandler.Instance.UserIdentifier = (App.Current as App).currentInfo.Username;
                }
                else
                    NavigationService.Navigate(new Uri("/MainPage.xaml?FromData=1", UriKind.Relative));

                if ((App.Current as App).currentInfoArray != null)
                {
                    foreach (var n in (App.Current as App).currentInfoArray.SelectMany(x => x.NumberInfos))
                    {
                        if (n != (object)null)
                            (DataContext as CreditInfo).NumberInfos.Add(n);
                    }

                }


                if (NavigationContext.QueryString.ContainsKey("number"))
                {
                    var number = NavigationContext.QueryString["number"];
                    if (pivotNumbers.Items.Cast<NumberInfo>().Any(ni => ni.Number == number))
                        pivotNumbers.SelectedItem = pivotNumbers.Items.Cast<NumberInfo>().Single(ni => ni.Number == number);
                    else
                        NavigationService.Navigate(new Uri("/MainPage.xaml?FromData=1", UriKind.Relative));
                }

                NavigationService.RemoveBackEntry();

                if (NetworkInterface.GetIsNetworkAvailable())
                {
                    using (var http = new HttpClient())
                    {
                        var data = http.GetAsync("https://wauth.apphb.com/api/HowManyPayed/" + (App.Current as App).currentInfo.Username);

                        if (CurrentApp.LicenseInformation.ProductLicenses.ContainsKey(IAPs.IAP_PushNotification))
                        {
                            await WPUtils.UpdateFromCloud();
                            await WPUtils.PushNotificationSetUp(this).ContinueWith(q => WPUtils.RemoveAgent(UpdateCreditAgent.AgentName), TaskContinuationOptions.OnlyOnRanToCompletion);
                            Update(false);
                        }

                        IEnumerable<int> payed = JsonConvert.DeserializeObject<IEnumerable<int>>(await (await data).Content.ReadAsStringAsync());
                        if (payed.Any())
                            Addins.Save(payed.First());

                    }

                }

            }

        }
        private void pivotNumbers_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var itm = e.AddedItems[0] as NumberInfo;
            var olditm = e.RemovedItems[0] as NumberInfo;

            if (e.RemovedItems[0] != null)
                HubTileService.FreezeGroup(olditm.Number);
            if (e.AddedItems[0] != null)
                HubTileService.UnfreezeGroup(itm.Number);
        }

        public void Dispose()
        {
            if (_shakeSensor != null)
            {
                _shakeSensor.Dispose();
                _shakeSensor = null;
            }
        }

        private async void ApplicationBarIconButton_Click(object sender, EventArgs e)
        {
            if ((sender as CimbalinoBeh.ApplicationBarIconButton).Text.ToLower() == "about")
            {
                NavigationService.Navigate(new Uri("/About.xaml", UriKind.Relative));
            }

            else if ((sender as CimbalinoBeh.ApplicationBarIconButton).Text.ToLower() == "refresh")
            {
                (sender as CimbalinoBeh.ApplicationBarIconButton).IsEnabled = false;
                ShakeDetected(null, new ShakeDetectedEventArgs(true));
                (sender as CimbalinoBeh.ApplicationBarIconButton).IsEnabled = true;
            }
            else if ((sender as CimbalinoBeh.ApplicationBarIconButton).Text.ToLower() == "save")
            {
                Task task = null;

                if (CurrentApp.LicenseInformation.ProductLicenses.ContainsKey(IAPs.IAP_PushNotification))
                    task = WPUtils.UploadCurrentData();

                var c = (App.Current as App).currentInfo;
                var c2 = (App.Current as App).currentInfoArray;
                Utils.SaveCreditState(c);
                Utils.SaveCreditState(c2);

                foreach (var model in pivotNumbers.ItemsSource.Cast<NumberInfo>())
                {

                    Utils.RenderTiles(model.Number, model);
                    var tile = ShellTile.ActiveTiles.Where(t => t.NavigationUri.OriginalString.Contains(model.Number));
                    var smallpath = System.IO.Path.Combine(WPUtils.baseDir, string.Format("{0}_{1}_{2}.jpg", 159, 159, model.Number));
                    var normalpath = System.IO.Path.Combine(WPUtils.baseDir, string.Format("{0}_{1}_{2}.jpg", 336, 336, model.Number));
                    //var widepath = System.IO.Path.Combine(WPUtils.baseDir, string.Format("{0}_{1}_{2}.jpg", 691, 336, model.Number));

                    foreach (var item in tile)
                    {
                        item.Update(new FlipTileData
                        {
                            Title = " ",
                            //WideBackgroundImage = new Uri("isostore:" + widepath, UriKind.Absolute),
                            BackgroundImage = new Uri("isostore:" + normalpath, UriKind.Absolute),
                            SmallBackgroundImage = new Uri("isostore:" + smallpath, UriKind.Absolute),
                            BackBackgroundImage = new Uri("isostore:" + normalpath, UriKind.Absolute),
                            //WideBackBackgroundImage = new Uri("isostore:" + widepath, UriKind.Absolute),
                        });
                    }
                }

                var Img = new ImageConverter();

                SafeDispatcher.Run(() =>
                {
                    foreach (var str in new[] { "336", "159" })
                    {
                        for (int i = 0; i < pivotNumbers.Items.Count; i++)
                        {
                            var hubtile = FindControl<HubTile>(this.pivotNumbers.ItemContainerGenerator.ContainerFromIndex(i), str);
                            hubtile.Source = Img.Convert(new[] { str, str, (pivotNumbers.Items[i] as NumberInfo).Number }, null, null, null) as BitmapImage;
                            hubtile.Source = Img.Convert(new[] { str, str, (pivotNumbers.Items[i] as NumberInfo).Number }, null, null, null) as BitmapImage;
                        }
                    }

                    (new ToastPrompt { Message = AppResources.Saved, IsTimerEnabled = true, UseLayoutRounding = true, MillisecondsUntilHidden = 2500 }).Show();

                });

                if (task != null)
                    await task;
            }
        }

        void ThankYou(object sender, RoutedEventArgs e)
        {
            new MessagePrompt() { Title = "Thank you", Message = AppResources.ThankYou, IsCancelVisible = false, IsAppBarVisible = false }.Show();
        }


        private async void agent_enabledisable(object sender, EventArgs e)
        {
            string message = String.Empty;
            if (!CurrentApp.LicenseInformation.ProductLicenses.ContainsKey(IAPs.IAP_PushNotification))
            {
                var agent = ScheduledActionService.Find(UpdateCreditAgent.AgentName);


                if (agent != null)
                {
                    WPUtils.RemoveAgent(UpdateCreditAgent.AgentName);
                    message = AppResources.BADisabled;
                }
                else
                {
                    WPUtils.StartPeriodicAgent(UpdateCreditAgent.AgentName);
                    message = AppResources.BAEnabled;
                }
            }
            else
            {
                var hub = WPUtils.NotificationGet();
                var channel = HttpNotificationChannel.Find(Constants.CnName);

                if (channel != null && channel.ConnectionStatus == ChannelConnectionStatus.Connected)
                {
                    channel.Close();
                    await hub.UnregisterNativeAsync();
                    message = AppResources.BADisabled;
                }
                else
                {
                    await WPUtils.PushNotificationSetUp(this);
                    return;
                }
            }

            new ToastPrompt { Message = message }.Show();
        }

        /// <summary>
        /// Workaround for listpicker binding after loading.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void listPickerColor_Loaded(object sender, RoutedEventArgs e)
        {
            var obj = (sender as ListPicker);
            obj.SetBinding(ListPicker.SelectedItemProperty, new Binding("Brush") { Converter = new BrushToItemSource(), Mode = BindingMode.TwoWay });
        }

        private List<Control> AllChildren(DependencyObject parent)
        {
            var _List = new List<Control>();
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(parent); i++)
            {
                var _Child = VisualTreeHelper.GetChild(parent, i);
                if (_Child is Control)
                {
                    _List.Add(_Child as Control);
                }
                _List.AddRange(AllChildren(_Child));
            }
            return _List;
        }


        private T FindControl<T>(DependencyObject parentContainer, string controlName) where T : Control
        {
            if (FindControls<T>(parentContainer).Any(x => x.Name.Equals(controlName)))
                return FindControls<T>(parentContainer).Where(x => x.Name.Equals(controlName)).First();
            return null;
        }

        private IEnumerable<T> FindControls<T>(DependencyObject parentContainer) where T : Control
        {
            return AllChildren(parentContainer).OfType<T>();
        }

        private void WrapPanel_Loaded(object sender, RoutedEventArgs e)
        {
            var panel = sender as WrapPanel;
            var objs = panel.Children.Where(x => x.Visibility == System.Windows.Visibility.Collapsed).ToArray();

            for (int i = 0; i < objs.Count(); i++)
            {
                panel.Children.Remove(objs.ElementAt(i));
            }


        }

        private void ApplicationBarMenuItem_Click(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/Addins.xaml", UriKind.Relative));
        }

        private void GotoNumbers(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/Accounts.xaml", UriKind.Relative));
        }

        private async void OneDrive(object sender, EventArgs e)
        {
            try
            {

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

                    var current = (App.Current as App).currentInfo;

                    var t1 = Tuple.Create<string, string, string>(current.Username, current.Password, current.Type);
                    var data = (App.Current as App).currentInfoArray.Select(x => Tuple.Create<string, string, string>(x.Username, x.Password, x.Type)).Concat(Enumerable.Repeat(t1, 1));

                    string jsoned = JsonConvert.SerializeObject(data);

                    using (var ms = new MemoryStream())
                    {
                        using (var bw = new StreamWriter(ms) { AutoFlush = true })
                        {
                            await bw.WriteAsync(jsoned);
                            ms.Seek(0, SeekOrigin.Begin);
                            string folder = await CreateDirectoryAsync(client, "siminfo", "me/skydrive");
                            var result = await client.UploadAsync(folder, "data", ms, OverwriteOption.Overwrite, new System.Threading.CancellationToken(), this);
                        }
                    }

                    tray.IsVisible = false;
                    SystemTray.SetProgressIndicator(this, tray);
                    var p = new MessagePrompt { Body = AppResources.CloudSaved, Title = string.Empty };
                    p.Show();


                }
                else
                {
                    var p = new MessagePrompt { Body = AppResources.CloudReject, Title = string.Empty };
                    p.Show();

                }
            }
            catch (LiveAuthException)
            {
                var p = new MessagePrompt { Body = AppResources.CloudReject, Title = string.Empty };
                p.Show();

            }

        }

        private async void ApplicationBarMenuItem_Click_1(object sender, EventArgs e)
        {
            var op = await Launcher.LaunchUriAsync(new Uri("ms-settings-lock:"));
        }

        public static async Task<string> DownloadFileAsync(LiveConnectClient client, string directory, string fileName)
        {
            string skyDriveFolder = await CreateDirectoryAsync(client, directory, "me/skydrive");
            var result = await client.DownloadAsync(skyDriveFolder);

            var operation = await client.GetAsync(skyDriveFolder + "/files");

            var items = operation.Result["data"] as List<object>;
            string id = string.Empty;

            // Search for the file - add handling here if File Not Found
            foreach (object item in items)
            {
                IDictionary<string, object> file = item as IDictionary<string, object>;
                if (file["name"].ToString() == fileName)
                {
                    id = file["id"].ToString();
                    break;
                }
            }

            var downloadResult = await client.DownloadAsync(string.Format("{0}/content", id));

            var reader = new StreamReader(downloadResult.Stream);
            string text = await reader.ReadToEndAsync();
            return text;
        }
        public async static Task<string> CreateDirectoryAsync(LiveConnectClient client,
string folderName, string parentFolder)
        {
            string folderId = null;

            // Retrieves all the directories.
            var queryFolder = parentFolder + "/files?filter=folders,albums";
            var opResult = await client.GetAsync(queryFolder);
            dynamic result = opResult.Result;

            foreach (dynamic folder in result.data)
            {
                // Checks if current folder has the passed name.
                if (folder.name.ToLowerInvariant() == folderName.ToLowerInvariant())
                {
                    folderId = folder.id;
                    break;
                }
            }

            if (folderId == null)
            {
                // Directory hasn't been found, so creates it using the PostAsync method.
                var folderData = new Dictionary<string, object>();
                folderData.Add("name", folderName);
                opResult = await client.PostAsync(parentFolder, folderData);
                result = opResult.Result;

                // Retrieves the id of the created folder.
                folderId = result.id;
            }

            return folderId;
        }


        public void Report(LiveOperationProgress value)
        {
            var tray = SystemTray.GetProgressIndicator(this);

            if (value.ProgressPercentage == 100)
                tray.IsVisible = false;
            else
            {
                tray.IsVisible = true;
                tray.Value = value.ProgressPercentage;
                tray.IsIndeterminate = false;
            }

            SystemTray.SetProgressIndicator(this, tray);
        }
    }

}

