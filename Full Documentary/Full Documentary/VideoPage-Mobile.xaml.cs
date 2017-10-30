using Microsoft.Advertising.WinRT.UI;
using MyToolkit.Multimedia;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel.Store;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
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
    public sealed partial class VideoPage_Mobile : Page
    {

        private ObservableDictionary defaultViewModel = new ObservableDictionary();
        public ObservableDictionary DefaultViewModel
        {
            get { return this.defaultViewModel; }
        }
        public VideoPage_Mobile()
        {
            this.InitializeComponent();
        }

        void RequestAD()
        {
            var video = new InterstitialAd();

            var MyAppId = "d25517cb-12d4-4699-8bdc-52040c712cab";
            var MyAdUnitId = "11389925";
            video.AdReady += (q, b) => { mediaElement.Pause(); video.Show();  };
            video.Cancelled += (c, d) => { mediaElement.Play();  };
            video.Completed += (e, f) => { mediaElement.Play();  };
            video.ErrorOccurred += (g, h) => { mediaElement.Play(); Debug.WriteLine("AD ERROR: =>:" + h.ErrorMessage); Debug.WriteLine("VIDEOSTATE: => " + video.State.ToString()); };
            video.RequestAd(AdType.Video, MyAppId, MyAdUnitId);
        }

        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            try
            {
                System.Text.RegularExpressions.Regex Youtube = new System.Text.RegularExpressions.Regex("youtu(?:\\.be|be\\.com)/(?:.*v(?:/|=)|(?:.*/)?)([a-zA-Z0-9-_]+)");
                System.Text.RegularExpressions.Match youtubeMatch = Youtube.Match(e.Parameter.ToString());
                string id = string.Empty;
                if (youtubeMatch.Success) // makes sure the video can be found on youtube
                {
                    id = youtubeMatch.Groups[1].Value;
                    var URL = await YouTube.GetVideoUriAsync(id, YouTubeQuality.QualityHigh);
                    this.defaultViewModel["VideoPath"] = URL.Uri.AbsoluteUri;
                }


            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }

        private void MediaElement_MediaOpened(object sender, RoutedEventArgs e)
        {
            if (!CurrentApp.LicenseInformation.ProductLicenses["VideoAds"].IsActive)
            {
                for (TimeSpan i = mediaElement.Position; i < mediaElement.NaturalDuration; i += new TimeSpan(0, 10, 0))
                {
                    mediaElement.Markers.Add(new TimelineMarker { Type = "PlayAd", Time = i });
                }
            }
            else
            {
                
            }
        }

        private void MediaElement_MarkerReached(object sender, TimelineMarkerRoutedEventArgs e)
        {
            if (e.Marker.Type == "PlayAd")
            {
                Debug.WriteLine("REQUESTING PLAYAD()");
                RequestAD();
            }

        }

        private async void MediaElement_MediaFailed(object sender, ExceptionRoutedEventArgs e)
        {
            var mess = new MessageDialog("Unable To Play The Video \n ", "Mediaplayer Status");
            await mess.ShowAsync();
        }
    }
}
