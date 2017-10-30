using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Xml.Linq;
using Windows.ApplicationModel.Store;
using Windows.Data.Xml.Dom;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.UI.Core;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace Full_Documentary
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = AppViewBackButtonVisibility.Visible;
            SystemNavigationManager.GetForCurrentView().BackRequested += MainPage_BackRequested;
            this.InitializeComponent();

            Start();
        }

        private async void Start()
        {
            await ((ContentDialog)new AddVideos()).ShowAsync();
        }
        private void MainPage_BackRequested(object sender, BackRequestedEventArgs e)
        {
            var frame = this.DataContext as Frame;
            if (frame != null && frame.CanGoBack)
            {
                frame.GoBack();
            }
        }

   

        private void AppBarButton_Click(object sender, RoutedEventArgs e)
        {
            if(SplitView.IsPaneOpen)
            {
                SplitView.IsPaneOpen = false;
                GenreListview.Visibility = Visibility.Collapsed;
                favoritesButton.Visibility = Visibility.Collapsed;
            }
            else
            {
                SplitView.IsPaneOpen = true;
                GenreListview.Visibility = Visibility.Visible;
                favoritesButton.Visibility = Visibility.Visible;
            }
        }

        private async void RemoveBannerAds_Clicked(object sender, RoutedEventArgs e)
        {
            var a = CurrentApp.LoadListingInformationAsync();
            if(!CurrentApp.LicenseInformation.ProductLicenses["download_Video-ID"].IsActive)
            {
              PurchaseResults results =  await CurrentApp.RequestProductPurchaseAsync("download_Video-ID");
                if(results.Status == ProductPurchaseStatus.Succeeded)
                {
                    var mess = new MessageDialog("Successfully Purchased IAP.", "IAP Results");
                    mess.Commands.Add(new UICommand("Save Recipt", (command) =>
                    {
                     ((sender as HyperlinkButton).DataContext as AdvertisingClass).SaveRecipt("BannerAd_IAPRecipt.txt", results.ReceiptXml);
                       
                    }));

                    mess.Commands.Add(new UICommand("Close", (command) =>
                    {
                    }));
                    
                    await mess.ShowAsync();
                }
              

            }

            else
            {
                var mess = new MessageDialog("You already Purchased This IAP");
                ((sender as HyperlinkButton).DataContext as AdvertisingClass).ChangeBannerAdVisability();
                await mess.ShowAsync();
                

            }
        }

        private async void RemoveVideoAds_Clicked(object sender, RoutedEventArgs e)
        {
          
            if (!CurrentApp.LicenseInformation.ProductLicenses["VideoAds"].IsActive)
            {
                PurchaseResults results = await CurrentApp.RequestProductPurchaseAsync("VideoAds");
                

                if (results.Status == ProductPurchaseStatus.Succeeded)
                {
                    var mess = new MessageDialog("Successfully Purchased IAP.", "IAP Results");
                    mess.Commands.Add(new UICommand("Save Recipt", (command) =>
                    {
                        
                        ((sender as HyperlinkButton).DataContext as AdvertisingClass).SaveRecipt("VideoAd_IAPRecipt.txt", results.ReceiptXml);

                    }));

                    mess.Commands.Add(new UICommand("Close", (command) =>
                    {
                    }));
                    
                    await mess.ShowAsync();
                }
                

            }

            else
            {
                var mess = new MessageDialog("You already Purchased This IAP");
                ((sender as HyperlinkButton).DataContext as AdvertisingClass).ChangeVideoAdVisability();
                await mess.ShowAsync();


            }
        }

        private void GenreListview_ItemClick(object sender, ItemClickEventArgs e)
        {
            string id = ((e.ClickedItem) as TextBlock).Name.Replace("_", "-");

            var frame = this.DataContext as Frame;
            if (frame != null)
            {
                frame.Navigate(typeof(HomePage), id);
            }

        }

        private void Favorites_Clicked(object sender, RoutedEventArgs e)
        {
            var frame = this.DataContext as Frame;
            if (frame != null)
            {
                frame.Navigate(typeof(FavoritesPage));
            }
        }

        private void About_Clicked(object sender, RoutedEventArgs e)
        {
            var frame = this.DataContext as Frame;
            if (frame != null )
            {
                frame.Navigate(typeof(AboutPage));
            }
        }

        private async void Support_Us_Clicked(object sender, RoutedEventArgs e)
        {
            if (!CurrentApp.LicenseInformation.ProductLicenses["Support"].IsActive)
            {
                PurchaseResults results = await CurrentApp.RequestProductPurchaseAsync("Support");
            }

            else
            {
                var mess = new MessageDialog("We appreciate your support.\n It looks like you have already supported us.\n Would you like to donate again? ");

                mess.Commands.Add(new UICommand("Yes",async (command) =>
                {
                    PurchaseResults results = await CurrentApp.RequestProductPurchaseAsync("Support");
                }));

                mess.Commands.Add(new UICommand("No", (command) =>
                {
                    //to do on click of the CustomButton2

                }));
                await mess.ShowAsync();


            }
        }

       
            
    }

}
