using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.ApplicationModel.Store;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Popups;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using System.Net.NetworkInformation;
using Windows.System.Profile;

namespace Full_Documentary
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    sealed partial class App : Application
    {
        private MainPage desktopMainPage;
        private MainPage_Mobile mobileMainPage;
        /// <summary>
        /// Initializes the singleton application object.  This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public App()
        {
            this.InitializeComponent();
            this.Suspending += OnSuspending;
        }

        /// <summary>
        /// Invoked when the application is launched normally by the end user.  Other entry points
        /// will be used such as when the application is launched to open a specific file.
        /// </summary>
        /// <param name="e">Details about the launch request and process.</param>
        protected async override void OnLaunched(LaunchActivatedEventArgs e)
        {
            
            if (!NetworkInterface.GetIsNetworkAvailable())
            {
                var mess = new MessageDialog("No Network Detected");

                mess.Commands.Add(new UICommand("Check Network Settings", async (command) =>
                {
                    await Windows.System.Launcher.LaunchUriAsync(new Uri("ms-settings-wifi:"));
                }));

                mess.Commands.Add(new UICommand("Close", (command) =>
                {
                    //to do on click of the CustomButton2
                }));

                await mess.ShowAsync();
            }

            AdvertisingClass.ISADBLOCKERACTIVE = CurrentApp.LicenseInformation.ProductLicenses["download_Video-ID"].IsActive;
            AdvertisingClass.DISPLAYADS =!AdvertisingClass.ISADBLOCKERACTIVE;
            

#if DEBUG
            if (System.Diagnostics.Debugger.IsAttached)
            {
                this.DebugSettings.EnableFrameRateCounter = true;
            }
#endif

            if(AnalyticsInfo.VersionInfo.DeviceFamily == "Windows.Mobile")
            {
                this.mobileMainPage = Window.Current.Content as MainPage_Mobile;
                Frame rootFrame = null;

                // Do not repeat app initialization when the Window already has content,
                // just ensure that the window is active
                if (this.mobileMainPage == null)
                {
                    // create new shell
                    this.mobileMainPage = new MainPage_Mobile();
                }

                if (rootFrame == null)
                {
                    // Create a Frame to act as the navigation context and navigate to the first page
                    rootFrame = new Frame();
                    // Set the default language
                    rootFrame.Language = Windows.Globalization.ApplicationLanguages.Languages[0];

                    rootFrame.NavigationFailed += OnNavigationFailed;

                    if (e.PreviousExecutionState == ApplicationExecutionState.Terminated)
                    {
                        //TODO: Load state from previously suspended application
                    }
                }

                mobileMainPage.DataContext = rootFrame;

                // Place the shell with frame as content in the current Window
                Window.Current.Content = mobileMainPage;

                if (rootFrame.Content == null)
                {
                    // When the navigation stack isn't restored navigate to the first page,
                    // configuring the new page by passing required information as a navigation
                    // parameter
                    rootFrame.Navigate(typeof(HomePage_Mobile), e.Arguments);
                }

                // Ensure the current window is active
                Window.Current.Activate(); 
            }

           else
            {
                this.desktopMainPage = Window.Current.Content as MainPage;
                Frame rootFrame = null;

                // Do not repeat app initialization when the Window already has content,
                // just ensure that the window is active
                if (this.desktopMainPage == null)
                {
                    // create new shell
                    this.desktopMainPage = new MainPage();
                }

                if (rootFrame == null)
                {
                    // Create a Frame to act as the navigation context and navigate to the first page
                    rootFrame = new Frame();
                    // Set the default language
                    rootFrame.Language = Windows.Globalization.ApplicationLanguages.Languages[0];

                    rootFrame.NavigationFailed += OnNavigationFailed;

                    if (e.PreviousExecutionState == ApplicationExecutionState.Terminated)
                    {
                        //TODO: Load state from previously suspended application
                    }
                }

                desktopMainPage.DataContext = rootFrame;

                // Place the shell with frame as content in the current Window
                Window.Current.Content = desktopMainPage;

                if (rootFrame.Content == null)
                {
                    // When the navigation stack isn't restored navigate to the first page,
                    // configuring the new page by passing required information as a navigation
                    // parameter
                    rootFrame.Navigate(typeof(HomePage), e.Arguments);
                }

                // Ensure the current window is active
                Window.Current.Activate();
            }
        }

      
        /// <summary>
        /// Invoked when Navigation to a certain page fails
        /// </summary>
        /// <param name="sender">The Frame which failed navigation</param>
        /// <param name="e">Details about the navigation failure</param>
        void OnNavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            throw new Exception("Failed to load Page " + e.SourcePageType.FullName);
        }

        /// <summary>
        /// Invoked when application execution is being suspended.  Application state is saved
        /// without knowing whether the application will be terminated or resumed with the contents
        /// of memory still intact.
        /// </summary>
        /// <param name="sender">The source of the suspend request.</param>
        /// <param name="e">Details about the suspend request.</param>
        private void OnSuspending(object sender, SuspendingEventArgs e)
        {
            var deferral = e.SuspendingOperation.GetDeferral();
            //TODO: Save application state and stop any background activity
            deferral.Complete();
        }
    }
}
