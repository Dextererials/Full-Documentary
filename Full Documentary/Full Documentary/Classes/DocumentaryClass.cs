using Microsoft.Advertising.WinRT.UI;
using MyToolkit.Multimedia;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Windows.ApplicationModel.Store;
using Windows.Data.Json;
using Windows.Storage;
using Windows.UI;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;


namespace Full_Documentary
{
    public class DocumentaryDataList
    {
        public static DocumentaryDataSource DocumentarySource = new DocumentaryDataSource();
    }

    public  class Favorites
    {
        public static ObservableCollection<string> FavoritesList =  RequestFavoritesString();

        public  static  ObservableCollection<string>  RequestFavoritesString()
        {
            try
            {
                Windows.Storage.StorageFolder localFolder = Windows.Storage.ApplicationData.Current.LocalFolder;
                string results = string.Empty;

                StorageFile file = null;
                Task.Run(async () => { file = await localFolder.GetFileAsync("Favorites.txt"); results = await FileIO.ReadTextAsync(file); }).Wait();


                if (results != string.Empty)
                {
                    ObservableCollection<string> res = new ObservableCollection<string>();
                    foreach (string one in results.Split(new char[] { ',' }))
                    {
                        if (one != string.Empty)
                            res.Add(one);
                    }
                    return res;
                }
                else
                {
                    return new ObservableCollection<string>();
                }
            }
            catch
            {
                return new ObservableCollection<string>();
            }
            
        }

        public static ObservableCollection<object> RequestFavoritesList()
        {
            try
            {
                Windows.Storage.StorageFolder localFolder = Windows.Storage.ApplicationData.Current.LocalFolder;
                string results = string.Empty;

                StorageFile file = null;
                Task.Run(async () => { file = await localFolder.GetFileAsync("Favorites.txt"); results = await FileIO.ReadTextAsync(file); }).Wait();


                if (results != string.Empty)
                {
                    ObservableCollection<object> res = new ObservableCollection<object>();
                    int counter = 0;
                    foreach (string one in results.Split(new char[] { ',' }))
                    {
                        if (counter % 5 == 0 && AdvertisingClass.DISPLAYADS)
                            res.Add(true);

                        if (one != string.Empty)
                            Task.Run(async () => { res.Add(await MovieDataSource.GetItemAsync(one)); }).Wait();
                        counter++;
                    }
                    
                    return res;
                }
                else
                {
                    return new ObservableCollection<object>();
                }
            }
            catch
            {
                return new ObservableCollection<object>();
            }

        }

        public static void ChangeFavorites(string id)
        {
            if(!FavoritesList.Contains(id))
            {
                FavoritesList.Add(id);
                SaveFunction("Favorites.txt", string.Join(",", FavoritesList.ToArray()));
            }
            else if(FavoritesList.Contains(id))
            {
                FavoritesList.Remove(id);
                SaveFunction("Favorites.txt", string.Join(",", FavoritesList.ToArray()));
            }
            else
            {
                //do nothing 
            }
        }

        public static async void SaveFunction(string fileName, string content)
        {
           
            try
            {
                StorageFolder localFolder = Windows.Storage.ApplicationData.Current.LocalFolder;

                StorageFile sampleFile = await localFolder.CreateFileAsync(fileName, CreationCollisionOption.ReplaceExisting);

                await FileIO.WriteTextAsync(sampleFile, content);
               
            }
            catch(Exception)
            {
              
            }

         


        }

       
    }

    public class DocumentaryDataSource
    {
        public ObservableCollection<MovieDataGroup> Groups = new ObservableCollection<MovieDataGroup>();
        public DocumentaryDataSource()
        {
            ObservableCollection<MovieDataGroup> result = new ObservableCollection<MovieDataGroup>();
            Task.Run(async () => { result = await MovieDataSource.GetGroupsAsync(); }).Wait();  
        }
    }

    public class MovieDataItem : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string name)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(name));
            }
        }


        public string UniqueId { get; private set; }
        public string Title { get; private set; }
        public string Description { get; private set; }
        public string ImagePath { get; private set; }
        public string VideoPath { get; set; }

        public void ChangeFavorites()
        {
            Favorites.ChangeFavorites(this.UniqueId);
            OnPropertyChanged("IsFavorites");
        }
        public SolidColorBrush IsFavorites
        {
            get
            {
                if(Favorites.FavoritesList.Contains(this.UniqueId))
                {
                    return new SolidColorBrush(Colors.Yellow);
                }
                else
                {
                    return new SolidColorBrush(Colors.White);
                }
            }
        }



        public override string ToString()
        {
            return this.Title;
        }

        public MovieDataItem()
        {

        }

        public MovieDataItem(String uniqueId, String title, String imagePath, String description, String videoPath)
        {
            this.UniqueId = uniqueId;
            this.Title = title;
            this.Description = description;
            this.ImagePath = imagePath;
            this.VideoPath = videoPath;
        }




    }

    public class MovieDataGroup
    {
        public string UniqueId { get; private set; }
        public string Title { get; private set; }
        public string Description { get; private set; }
        public string ImagePath { get; private set; }
        public ObservableCollection<MovieDataItem> Items { get; private set; }

        public MovieDataGroup()
        {

        }

        public MovieDataGroup(String uniqueId, String title, String imagePath, String description)
        {
            this.UniqueId = uniqueId;
            this.Title = title;
            this.ImagePath = imagePath;
            this.Description = description;
            this.Items = new ObservableCollection<MovieDataItem>();
        }




        public override string ToString()
        {
            return this.Title;
        }


    }

    public class MovieDataSource
    {
        private static MovieDataSource _movieDataSource = new MovieDataSource();
        private ObservableCollection<MovieDataGroup> _groups = new ObservableCollection<MovieDataGroup>();

        private static int videoCounter = 1; // used to create a uniqueID for the items in MovieDataGroup


        public ObservableCollection<MovieDataGroup> Groups
        {
            get { return this._groups; }
        }

        /// <summary>
        /// Create XML file from interet
        /// </summary>
        /// 
        public static async void Create_XML_File_From_Internet()
        {
          

            MovieDataSource movieSource = new MovieDataSource();


            XDocument xdoc = await Internet.RequestContent("LINK");

            var genreList = xdoc.Descendants("article").Descendants("div").ToList(); // list for all the genre 

            int genreCounter = 1;

           
            foreach (var genre in genreList) // create a new group for each genre
            {
                videoCounter = 1;
                string genreName = genre.Descendants("h2").Descendants("a").FirstOrDefault().Value; // gets the name of the genre

                
                MovieDataGroup movieGroup = new MovieDataGroup("Group-" + genreCounter, genreName, "CREate GEneric Image path", "create Generic DDescription");

                string url = genre.Descendants("h2").Descendants("a").FirstOrDefault().Attribute("href").Value; //gets the url for each genre 

                XDocument genrePageXml = await Internet.RequestContent(url); // creates xml from genre url to get videos from the first page

                int maxPageNumber = 0;
                // get  a list of all the pages
                var pages = genrePageXml.Descendants("main").Descendants("div").Where(e => e.Descendants("span").FirstOrDefault() != null && e.Attribute("class").Value == "pagination module").Descendants("a").ToList();

                // find the maximum page number
                foreach (var page in pages)
                {
                    int hold = 0;
                    if (int.TryParse(page.Value, out hold) == true)
                    {
                        if (int.Parse(page.Value) > maxPageNumber)
                        {
                            //MaxPage = hold;
                            maxPageNumber = hold;
                        }
                    }
                }

                try
                {
                    var videoList = genrePageXml.Descendants("article").ToList();

                    ObservableCollection<MovieDataItem> movieDataGroup = await Create_Group_From_XElement(videoList, movieGroup.UniqueId);

                    foreach (MovieDataItem movie in movieDataGroup)
                    {
                        movieGroup.Items.Add(movie);
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                }

                string newURL;
                for (int i = 2; i <= maxPageNumber; i++)
                {
                    try
                    {
                        try
                        {

                            newURL = url + "page" + "/" + i + "/";
                            XDocument newXDocument = new XDocument();
                            List<XElement> newXElement = new List<XElement>();
                            ObservableCollection<MovieDataItem> movieDataGroup1 = new ObservableCollection<MovieDataItem>();
                            try
                            {

                                newXDocument = await Internet.RequestContent(newURL);
                            }
                            catch (Exception ex)
                            {
                                Debug.WriteLine(ex.Message);
                            }

                            try
                            {

                                newXElement = newXDocument.Descendants("article").ToList();
                            }
                            catch (Exception ex)
                            {
                                Debug.WriteLine(ex.Message);
                            }

                            try
                            {
                                movieDataGroup1 = await Create_Group_From_XElement(newXElement, movieGroup.UniqueId);
                            }
                            catch (Exception ex)
                            {
                                Debug.WriteLine(ex.Message);
                            }




                            try
                            {
                                foreach (MovieDataItem movie in movieDataGroup1)
                                {
                                    if(!MovieDataSource.RestrictedTitle(movie.Title)) // checks to make sure that the movie is appropriate for the user
                                     movieGroup.Items.Add(movie);
                                }
                            }
                            catch(Exception ex)
                            {
                                Debug.WriteLine(ex.Message);
                            }

                        }
                        catch(Exception ex)
                        {
                            Debug.WriteLine(ex.Message);
                        }



                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine(ex.Message);
                    }


                }




                movieSource.Groups.Add(movieGroup);
                genreCounter++;
            }

            // save the json file to computer

            string jsonContents = JsonConvert.SerializeObject(movieSource);

           
            StorageFolder folder = Windows.Storage.ApplicationData.Current.LocalFolder;
            StorageFile sampleFile = await folder.CreateFileAsync("Data.json", CreationCollisionOption.ReplaceExisting);
            await Windows.Storage.FileIO.WriteTextAsync(sampleFile, jsonContents);



        }


        public static bool RestrictedTitle(string title)
        {
            List<string> TitleCollection = new List<string>();
            TitleCollection.Add("Naked Citizens");
            TitleCollection.Add("Mom, Why Did You Circumcise Me?");
            TitleCollection.Add("Aleister Crowley: The Wickedest Man in the World");
            TitleCollection.Add("666 Revealed");
            if (!TitleCollection.Contains(title))
            {
                return false;
            }
            else
            {
                return true;
            }
                
            
        }

        public async static Task<ObservableCollection<MovieDataItem>> Create_Group_From_XElement(List<XElement> list, string uniqueID)
        {

            ObservableCollection<MovieDataItem> movieItemList = new ObservableCollection<MovieDataItem>();

            foreach (XElement one_video in list) // add each movieItem to group
            {
                bool isYoutube = false; // used to check if video is a youtube source
                string title = " ";
                string imagePath = " ";
                string videoPath = " ";
                string description = " ";
                XDocument movieXDOC = new XDocument();
                try
                {

                    title = one_video.Descendants("h2").Descendants("a").FirstOrDefault().Value;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                }

                try
                {

                    imagePath = one_video.Descendants("span").Descendants("a").Descendants("img").FirstOrDefault().Attribute("src").Value;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                    //to get video url and complete description you must visit the video website

                }


                try
                {
                    string GeneralVideoPath = one_video.Descendants("h2").Descendants("a").FirstOrDefault().Attribute("href").Value;
                    movieXDOC = await Internet.RequestContent(GeneralVideoPath);
                    videoPath = movieXDOC.Descendants("iframe").Attributes("src").Where(e => e != null).FirstOrDefault().Value;
                    if (videoPath.Contains("youtube"))
                    {
                        try
                        {
                            System.Text.RegularExpressions.Regex Youtube = new System.Text.RegularExpressions.Regex("youtu(?:\\.be|be\\.com)/(?:.*v(?:/|=)|(?:.*/)?)([a-zA-Z0-9-_]+)");
                            System.Text.RegularExpressions.Match youtubeMatch = Youtube.Match(videoPath);
                            string id;
                            if (youtubeMatch.Success) // makes sure the video can be found on youtube
                            {
                                id = youtubeMatch.Groups[1].Value;
                                isYoutube = true;
                            }
                        }
                        catch(Exception ex)
                        {
                            Debug.WriteLine(ex.Message);
                        }


                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                }









                description = "";
                try
                {
                    description = movieXDOC.Descendants("section").Where(e => e.Attribute("class").Value == "module single-h2-title").Descendants("p").FirstOrDefault().Value;

                }
                catch
                {
                    try
                    {
                        description = movieXDOC.Descendants("div").Where(x => x.Attribute("class") != null && x.Attribute("class").Value == "contentpic").Descendants("p").FirstOrDefault().Value;
                    }
                    catch
                    {
                        try
                        {
                            description = movieXDOC.Descendants("div").Where(x => x.Attribute("class") != null && x.Attribute("class").Value == "junk-contentpic").Descendants("p").FirstOrDefault().Value;
                        }
                        catch
                        {

                        }

                    }
                }

                if (isYoutube == true)
                {
                    MovieDataItem movieItem = new MovieDataItem(uniqueID + " Item-" + videoCounter, title, imagePath, description, videoPath);
                    movieItemList.Add(movieItem);
                    videoCounter++;
                }






            }
            return movieItemList;


        }


        /// <summary>
        /// gets the string json from the database online
        /// </summary>
        /// <returns></returns>
        public async Task<string> Get_String_From_WebsiteDatabase()
        {
            if(ApplicationData.Current.LocalSettings.Values["LastUpdateDate"] == null)
            {
                ApplicationData.Current.LocalSettings.Values["LastUpdateDate"] = DateTime.Today.ToString();
            }

            DateTime update = DateTime.Parse((string)ApplicationData.Current.LocalSettings.Values["LastUpdateDate"]);

            if((update - DateTime.Now).TotalDays < 30) // update the source code every 30 days
            {
                UpdateDataFile();
                ApplicationData.Current.LocalSettings.Values["LastUpdateDate"] = DateTime.Today;
            }

            try
            {
                    StorageFolder localFolder = ApplicationData.Current.LocalFolder; 
                    StorageFile sampleFile = await localFolder.GetFileAsync("Data.json");
                    return await FileIO.ReadTextAsync(sampleFile);
            }
            catch (Exception ex)
            {
                try
                {
                    HttpWebRequest request = (HttpWebRequest)WebRequest.Create(new Uri("https://od.lk/d/ODhfMTQzODU3OF8/DocumentaryList.json"));
                    HttpWebResponse response = (HttpWebResponse)await request.GetResponseAsync();
                    string xmlString = string.Empty;

                    xmlString = new StreamReader(response.GetResponseStream()).ReadToEnd();
                    request.Abort();
                    response.Dispose();

                    StorageFolder myfolder = Windows.Storage.ApplicationData.Current.LocalFolder;
                    StorageFile sampleFile = await myfolder.CreateFileAsync("Data.json", CreationCollisionOption.ReplaceExisting);// this line throws an exception
                    await FileIO.WriteTextAsync(sampleFile, xmlString);
                    Debug.WriteLine(ex.Message);
                    return xmlString;
                }
                catch(Exception eex)
                {
                    Debug.WriteLine(eex.Message);
                    var mess = new MessageDialog("Unable To Retrive Content\n Check Your Internet Connection.", "Data Status");
                    return string.Empty;
                }
               
            }

        }

        public async void UpdateDataFile()
        {
            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(new Uri("https://od.lk/d/ODhfMTQzODU3OF8/DocumentaryList.json"));
                HttpWebResponse response = (HttpWebResponse)await request.GetResponseAsync();
                string xmlString = string.Empty;

                xmlString = new StreamReader(response.GetResponseStream()).ReadToEnd();
                request.Abort();
                response.Dispose();

                StorageFolder myfolder = Windows.Storage.ApplicationData.Current.LocalFolder;
                StorageFile sampleFile = await myfolder.CreateFileAsync("Data.json", CreationCollisionOption.ReplaceExisting);// this line throws an exception
                await FileIO.WriteTextAsync(sampleFile, xmlString);
            }
            catch (Exception eex)
            {
                Debug.WriteLine(eex.Message);
                var mess = new MessageDialog("Unable To Retrive Content\n Check Your Internet Connection.", "Data Status");
                
            }
        }
        public static async Task<ObservableCollection<MovieDataGroup>> GetGroupsAsync()
        {
            await _movieDataSource.GetSampleDataAsync();

            return _movieDataSource.Groups;
        }


        public static async Task<MovieDataGroup> GetGroupAsync(string uniqueId)
        {
            await _movieDataSource.GetSampleDataAsync();
            // Simple linear search is acceptable for small data sets
            var matches = _movieDataSource.Groups.Where((group) => group.UniqueId.Equals(uniqueId));
            if (matches.Count() == 1) return matches.First();
            return null;
        }

        public static async Task<string> GetGenreTitle(string uniqueId)
        {
            await _movieDataSource.GetSampleDataAsync();
            // Simple linear search is acceptable for small data sets
            var matches = _movieDataSource.Groups.Where((group) => group.UniqueId.Equals(uniqueId));
            if (matches.Count() == 1) return matches.First().Title;
            return null;
        }

        public async static Task<ObservableCollection<object>> GetGroupWithAds(string uniqueId)
        {
            ObservableCollection<object> results = new ObservableCollection<object>();

            MovieDataGroup movieItem = await GetGroupAsync(uniqueId);

            int counter = 0;
            foreach(MovieDataItem one in movieItem.Items)
            {
                if(counter %5 == 0 && AdvertisingClass.DISPLAYADS)
                {
                    results.Add(true);
                   
                }
                results.Add(one);

                counter++;
            }

            return results;
        }


        public static async Task<MovieDataItem> GetItemAsync(string uniqueId)
        {
            await _movieDataSource.GetSampleDataAsync();
            // Simple linear search is acceptable for small data sets
            var matches = _movieDataSource.Groups.SelectMany(group => group.Items).Where((item) => item.UniqueId.Equals(uniqueId));
            if (matches.Count() == 1) return matches.First();
            return null;
        }


        private async Task GetSampleDataAsync()
        {
            if (this._groups.Count != 0)
                return;




            string file = await Get_String_From_WebsiteDatabase();


            JsonObject jsonObject = JsonObject.Parse(file);
            JsonArray jsonArray = jsonObject["Groups"].GetArray();

          

            foreach (JsonValue groupValue in jsonArray)
            {
                JsonObject groupObject = groupValue.GetObject();
                MovieDataGroup group = new MovieDataGroup(groupObject["UniqueId"].GetString(),
                                                            groupObject["Title"].GetString(),
                                                            groupObject["ImagePath"].GetString(),
                                                            groupObject["Description"].GetString());

                foreach (JsonValue itemValue in groupObject["Items"].GetArray())
                {
                    JsonObject itemObject = itemValue.GetObject();
                    group.Items.Add(new MovieDataItem(itemObject["UniqueId"].GetString(),
                                                       itemObject["Title"].GetString(),
                                                       itemObject["ImagePath"].GetString(),
                                                       itemObject["Description"].GetString(),
                                                       itemObject["VideoPath"].GetString())
                                                       );
                }

                if (group.Title != "Sexuality" && group.Title != "Military and War" && group.Title != "Drugs" && group.Title != "Crime")
                {
                    this.Groups.Add(group);
                }
               
            }

           

        }


  


    }
    
}
