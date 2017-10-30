using MyToolkit.Multimedia;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Content Dialog item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace Full_Documentary
{
    public sealed partial class AddVideos : ContentDialog
    {

        public AddVideos()
        {
            this.InitializeComponent();
        }

        private async void ContentDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
           
            
        }

        private async void ContentDialog_SecondaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
         
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            System.Text.RegularExpressions.Regex Youtube = new System.Text.RegularExpressions.Regex("youtu(?:\\.be|be\\.com)/(?:.*v(?:/|=)|(?:.*/)?)([a-zA-Z0-9-_]+)");
            System.Text.RegularExpressions.Match youtubeMatch = Youtube.Match(VideoTextBox.Text);
            if (youtubeMatch.Success) // makes sure the video can be found on youtube
            {
                string id = (genreComboBox.SelectedItem as TextBlock).Name.Replace("_", "-");
                MovieDataGroup data = await MovieDataSource.GetGroupAsync(id);
                int num = data.Items.Count;

                (await MovieDataSource.GetGroupAsync((genreComboBox.SelectedItem as TextBlock).Name.Replace("_", "-"))).Items.Add(new MovieDataItem(data.UniqueId + "Item-" + ++num, titleTextBox.Text, IMGTextBox.Text, descriptionTextBox.Text, VideoTextBox.Text));

                string jsonContents = JsonConvert.SerializeObject(new MovieDataSource().Groups);
                StorageFolder folder = Windows.Storage.ApplicationData.Current.LocalFolder;
                StorageFile sampleFile = await folder.CreateFileAsync("Data.json", CreationCollisionOption.ReplaceExisting);
                await Windows.Storage.FileIO.WriteTextAsync(sampleFile, jsonContents);
            }
            else
            {
                var mess = new MessageDialog("Video Not Supported");
                await mess.ShowAsync();
            }

        }
    }
}
