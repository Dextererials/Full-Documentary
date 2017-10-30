using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel.Store;
using Windows.Data.Xml.Dom;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Foundation.Metadata;
using Windows.Phone.UI.Input;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace Full_Documentary
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage_Mobile : Page
    {
        public MainPage_Mobile()
        {
            this.InitializeComponent();

           
         HardwareButtons.BackPressed += HardwareButtons_BackPressed;
          
        }

        private void HardwareButtons_BackPressed(object sender,BackPressedEventArgs e)
        {
            var frame = this.DataContext as Frame;
            if (frame != null && frame.CanGoBack)
            {
                e.Handled = true;
                frame.GoBack();
            }
        }

        private void Favorites_Clicked(object sender, RoutedEventArgs e)
        {
            var frame = this.DataContext as Frame;
            if (frame != null)
            {
                frame.Navigate(typeof(FavoritesPage_Mobile));
            }
        }

      
       
        private async void RemoveBannerAds_Clicked(object sender, RoutedEventArgs e)
        {
            if (!CurrentApp.LicenseInformation.ProductLicenses["download_Video-ID"].IsActive)
            {
                PurchaseResults results = await CurrentApp.RequestProductPurchaseAsync("download_Video-ID");

                if (results.Status == ProductPurchaseStatus.Succeeded)
                {
                    var mess = new MessageDialog("Successfully Purchased IAP.", "IAP Results");
                    mess.Commands.Add(new UICommand("Save Recipt", (command) =>
                    {
                        XmlDocument xDoc = new XmlDocument();
                        xDoc.LoadXml(results.ReceiptXml);
                        ((sender as HyperlinkButton).DataContext as AdvertisingClass).SaveRecipt("BannerAd_IAPRecipt.txt", xDoc.InnerText);

                    }));

                    mess.Commands.Add(new UICommand("Close", (command) =>
                    {
                    }));

                    try
                    {
                        await mess.ShowAsync();
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine(ex.Message);
                    }
                }


              


            }

            else
            {
                var mess = new MessageDialog("You already Purchased This IAP");
                ((sender as HyperlinkButton).DataContext as AdvertisingClass).ChangeBannerAdVisability();

                try
                {
                    await mess.ShowAsync();
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                }



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
                        XmlDocument xDoc = new XmlDocument();
                        xDoc.LoadXml(results.ReceiptXml);
                        ((sender as HyperlinkButton).DataContext as AdvertisingClass).SaveRecipt("VideoAd_IAPRecipt.txt", xDoc.InnerText);

                    }));

                    mess.Commands.Add(new UICommand("Close", (command) =>
                    {
                    }));
                    
                    try
                    {
                        await mess.ShowAsync();
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine(ex.Message);
                    }
                }


                
              

            }

            else
            {
                var mess = new MessageDialog("You already Purchased This IAP");
                ((sender as HyperlinkButton).DataContext as AdvertisingClass).ChangeVideoAdVisability();

                try
                {
                    await mess.ShowAsync();
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                }



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

                mess.Commands.Add(new UICommand("Yes", async (command) =>
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

        private void About_Clicked(object sender, RoutedEventArgs e)
        {
            var frame = this.DataContext as Frame;
            if (frame != null)
            {
                frame.Navigate(typeof(AboutPage));
            }
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string id = ((sender as ComboBox).SelectedItem as TextBlock).Name.Replace("_", "-");

            var frame = this.DataContext as Frame;
            if (frame != null)
            {
                frame.Navigate(typeof(HomePage_Mobile), id);
            }
        }
    }
}
