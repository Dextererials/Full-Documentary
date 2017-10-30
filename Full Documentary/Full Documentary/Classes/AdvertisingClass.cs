using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Store;
using Windows.Foundation;
using Windows.Storage;
using Windows.System.Profile;
using Windows.UI.Popups;
using Windows.UI.Xaml;

namespace Full_Documentary
{
    class AdvertisingClass : INotifyPropertyChanged
    {
        private static bool displayAds = true;
        private static bool isAdBlockerActive = false;
 

        public static void UpdateLicence()
        {
           if(CurrentApp.LicenseInformation.ProductLicenses["download_Video-ID"].IsActive)
            {
                ISADBLOCKERACTIVE = true;
                DISPLAYADS = false;
            }
        }


        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string name)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(name));
            }
        }
        public Visibility IsRemoveBannerAdsVisable
        {
            get
            {
                //use  download_Video - ID as key for banner ads as its not being used for anything else and i cant delete it
                return CurrentApp.LicenseInformation.ProductLicenses["download_Video-ID"].IsActive ? Visibility.Collapsed : Visibility.Visible;
            }
        }
        public Visibility IsRemoveVideoAdsVisable
        {
            get
            {
               return CurrentApp.LicenseInformation.ProductLicenses["VideoAds"].IsActive ? Visibility.Collapsed : Visibility.Visible;
            }
        }
      
        
        public static bool ISADBLOCKERACTIVE
        {
            get
            {
                return isAdBlockerActive;
            }
            set
            {
                isAdBlockerActive = value;
            }
        }
        public static bool DISPLAYADS
        {
            get
            {
                return displayAds;
            }
            set
            {
                displayAds = value;
            }
        }

        public void ChangeBannerAdVisability()
        {
            OnPropertyChanged("IsRemoveBannerAdsVisable");
        }

        public void ChangeVideoAdVisability()
        {
            OnPropertyChanged("IsRemoveVideoAdsVisable");
        }
        public async  void SaveRecipt(string fileName,string content)
        {
            string message = string.Empty;
            try
            {
                StorageFolder localFolder = KnownFolders.VideosLibrary;

                StorageFile sampleFile = await localFolder.CreateFileAsync(fileName, CreationCollisionOption.ReplaceExisting);

                await FileIO.WriteTextAsync(sampleFile, content);
                message = "Successfully Saved " + fileName + " In Your Videos Folder\n";
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                message = "Unable To Save " + fileName;
            }

            var mess = new MessageDialog(message, "Save File: Status");
            await mess.ShowAsync();
        }
    }
}
