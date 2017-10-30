using MyToolkit.Multimedia;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
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
    public sealed partial class HomePage : Page
    {

        private ObservableDictionary defaultViewModel = new ObservableDictionary();


        public ObservableDictionary DefaultViewModel
        {
            get { return this.defaultViewModel; }
        }

        public HomePage()
        {
            this.InitializeComponent();
        }

        protected async  override void OnNavigatedTo(NavigationEventArgs e)
        {
            try
            {
                string parameter = string.Empty;

                if (e.Parameter.ToString() != string.Empty)
                {
                    parameter = e.Parameter.ToString();
                }
                else
                    parameter = "Group-1";
               
                this.defaultViewModel["GenreItems"] = await MovieDataSource.GetGroupWithAds(parameter);
                this.defaultViewModel["GenreTitle"] = await MovieDataSource.GetGenreTitle(parameter);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }

        private void GridView_ItemClick(object sender, ItemClickEventArgs e)
        {
            if(e.ClickedItem is MovieDataItem)
            Frame.Navigate(typeof(VideoPage), (e.ClickedItem as MovieDataItem).VideoPath);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            (((sender) as Button).Flyout).ShowAt(sender as FrameworkElement);
        }

        private void ChangeFavorites_Clicked(object sender, RoutedEventArgs e)
        {
            (((sender as AppBarButton).DataContext) as MovieDataItem).ChangeFavorites();
        }

        
    }
}
