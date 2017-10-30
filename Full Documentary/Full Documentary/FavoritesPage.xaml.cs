using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
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
    public sealed partial class FavoritesPage : Page
    {

        private ObservableDictionary defaultViewModel = new ObservableDictionary();


        public ObservableDictionary DefaultViewModel
        {
            get { return this.defaultViewModel; }
        }

        public FavoritesPage()
        {
            this.InitializeComponent();
        }

        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            try
            {
                ObservableCollection<object> results = Favorites.RequestFavoritesList();
                    
                if(results.Count == 0)
                {
                    var mess = new MessageDialog("No Items", "Items Status:");
                    await mess.ShowAsync();
                }
                else
                {
                    this.defaultViewModel["FavoritesItems"] = Favorites.RequestFavoritesList();
                }
                
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }

        private void GridView_ItemClick(object sender, ItemClickEventArgs e)
        {
            Frame.Navigate(typeof(VideoPage), (e.ClickedItem as MovieDataItem).VideoPath);
        }

        private void ChangeFavorites_Clicked(object sender, RoutedEventArgs e)
        {
            (((sender as AppBarButton).DataContext) as MovieDataItem).ChangeFavorites();
        }
    }
}
