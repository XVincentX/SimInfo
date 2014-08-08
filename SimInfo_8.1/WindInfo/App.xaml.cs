using Cimbalino.Phone.Toolkit.Services;
using Coding4Fun.Toolkit.Controls;
using Coding4Fun.Toolkit.Controls.Common;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Net.NetworkInformation;
using Microsoft.Phone.Shell;
using Microsoft.Phone.Tasks;
using ReviewNotifier.Apollo;
using StringResources;
using System;
using System.IO.IsolatedStorage;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Navigation;
using Tiles;
using WindDataLib;
using WindInfo.Code;
using Windows.ApplicationModel.Store;
using Windows.Storage;
using System.Linq;
using System.Collections.Generic;
using Microsoft.WindowsAzure.MobileServices;
using BugSense;
using BugSense.Core.Model;

namespace WindInfo
{
    public partial class App : Application
    {
        /// <summary>
        /// Provides easy access to the root frame of the Phone Application.
        /// </summary>
        /// <returns>The root frame of the Phone Application.</returns>
        public PhoneApplicationFrame RootFrame { get; private set; }

        public CreditInfo currentInfo { get; set; }
        public IList<CreditInfo> currentInfoArray { get; set; }

        public static MobileServiceClient mobileClient = new MobileServiceClient("https://siminfo.azure-mobile.net/", "ymzJInYzOBINEMfCskSfESqhClBeMZ75");


        /// <summary>
        /// Constructor for the Application object.
        /// </summary>
        public App()
        {

            currentInfoArray = new List<CreditInfo>();

            BugSenseHandler.Instance.InitAndStartSession(new ExceptionManager(Current), RootFrame, "w8c3ad1e");
            BugSenseHandler.Instance.HandleWhileDebugging = true;

            // Standard Silverlight initialization
            InitializeComponent();

            // Phone-specific initialization
            InitializePhoneApplication();

            // Show graphics profiling information while debugging.
            if (System.Diagnostics.Debugger.IsAttached)
            {
                // Display the current frame rate counters.
                Application.Current.Host.Settings.EnableFrameRateCounter = true;

                // Show the areas of the app that are being redrawn in each frame.
                //Application.Current.Host.Settings.EnableRedrawRegions = true;

                // Enable non-production analysis visualization mode, 
                // which shows areas of a page that are handed off to GPU with a colored overlay.
                //Application.Current.Host.Settings.EnableCacheVisualization = true;

                // Disable the application idle detection by setting the UserIdleDetectionMode property of the
                // application's PhoneApplicationService object to Disabled.
                // Caution:- Use this under debug mode only. Application that disables user idle detection will continue to run
                // and consume battery power when the user is not using the phone.
                PhoneApplicationService.Current.UserIdleDetectionMode = IdleDetectionMode.Disabled;
            }

        }

        // Code to execute when the application is launching (eg, from Start)
        // This code will not execute when the application is reactivated
        private async void Application_Launching(object sender, LaunchingEventArgs e)
        {
            ThemeManager.ToDarkTheme();

            await ReviewNotification.InitializeAsync();

            if (NetworkInterface.GetIsNetworkAvailable() && NetworkInterface.NetworkInterfaceType == NetworkInterfaceType.Wireless80211)
            {
                try
                {
                    var upd = await CheckIsThereUpdate();
                    if (upd)
                    {
                        var msg = new MessagePrompt { Title = AppResources.UpdateTitle, Message = AppResources.UpdateMessage, IsCancelVisible = true };
                        msg.Completed += msg_Completed;
                        msg.Show();

                    }
                }
                catch (Exception)
                {
                }

            }           
        }



        // Code to execute when the application is activated (brought to foreground)
        // This code will not execute when the application is first launched
        private void Application_Activated(object sender, ActivatedEventArgs e)
        {
        }

        // Code to execute when the application is deactivated (sent to background)
        // This code will not execute when the application is closing
        private void Application_Deactivated(object sender, DeactivatedEventArgs e)
        {
        }

        // Code to execute when the application is closing (eg, user hit Back)
        // This code will not execute when the application is deactivated
        private void Application_Closing(object sender, ClosingEventArgs e)
        {
        }

        // Code to execute if a navigation fails
        private void RootFrame_NavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            if (System.Diagnostics.Debugger.IsAttached)
            {
                // A navigation has failed; break into the debugger
                System.Diagnostics.Debugger.Break();
            }
        }

        // Code to execute on Unhandled Exceptions
        private void Application_UnhandledException(object sender, ApplicationUnhandledExceptionEventArgs e)
        {
            if (System.Diagnostics.Debugger.IsAttached)
            {
                // An unhandled exception has occurred; break into the debugger
                System.Diagnostics.Debug.WriteLine(e.ExceptionObject.Message);
                System.Diagnostics.Debugger.Break();
            }
            SafeDispatcher.Run(() => MessageBox.Show(e.ExceptionObject.Message));
        }

        #region Phone application initialization

        // Avoid double-initialization
        private bool phoneApplicationInitialized = false;

        // Do not add any additional code to this method
        private void InitializePhoneApplication()
        {
            if (phoneApplicationInitialized)
                return;

            // Create the frame but don't set it as RootVisual yet; this allows the splash
            // screen to remain active until the application is ready to render.
            RootFrame = new TransitionFrame();
            RootFrame.Navigated += CompleteInitializePhoneApplication;

            // Handle navigation failures
            RootFrame.NavigationFailed += RootFrame_NavigationFailed;

            // Ensure we don't initialize again
            phoneApplicationInitialized = true;
        }

        // Do not add any additional code to this method
        private void CompleteInitializePhoneApplication(object sender, NavigationEventArgs e)
        {
            // Set the root visual to allow the application to render
            if (RootVisual != RootFrame)
                RootVisual = RootFrame;

            // Remove this handler since it is no longer needed
            RootFrame.Navigated -= CompleteInitializePhoneApplication;
        }

        #endregion

        private static async Task<bool> CheckIsThereUpdate()
        {

            IMarketplaceInformationService mis = new MarketplaceInformationService();
            var data = await mis.GetAppInformationAsync();
            var currentVersion = new Version(WPUtils.GetAppAttributeValue("Version"));
            return (new Version(data.Entry.Version) > currentVersion);
        }

        void msg_Completed(object sender, PopUpEventArgs<string, PopUpResult> e)
        {
            if (e.PopUpResult == PopUpResult.Ok)
                new MarketplaceDetailTask().Show();
        }
    }
}