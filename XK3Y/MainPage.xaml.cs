using System;
using System.ComponentModel;
using System.Net;
using System.Windows;
using System.Windows.Controls.Primitives;
using Microsoft.Phone.Controls;
using XK3Y.Web;
using GestureEventArgs = System.Windows.Input.GestureEventArgs;

namespace XK3Y
{
    public partial class MainPage
    {
        private BackgroundWorker dataLoader;
        private Popup splashScreen;

        private NavigationOutTransition navigationOut;

        // Constructor
        public MainPage()
        {
            InitializeComponent();
        }

        protected override void OnNavigatingFrom(System.Windows.Navigation.NavigatingCancelEventArgs e)
        {
            base.OnNavigatingFrom(e);

            if (e.Uri.OriginalString == "/Config.xaml?Initial=1")
            {
                navigationOut = TransitionService.GetNavigationOutTransition(this);

                // Prevent page transistion, since that's ugly ;)
                TransitionService.SetNavigationOutTransition(this, new NavigationOutTransition());
            }
            else
                TransitionService.SetNavigationOutTransition(this, navigationOut);
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            if (AppSettings.IPAddress == null)
            {
                NavigationService.Navigate(new Uri("/Config.xaml?Initial=1", UriKind.Relative));
                return;
            }

            if (!DataLoader.DataLoaded)
            {
                StartDataLoading();
            }
            MainMenu.Visibility = Visibility.Visible;
        }

        private void StartDataLoading()
        {
            splashScreen = new Popup {IsOpen = true, Child = new AnimatedSplashScreen()};

            // Load the data.xml, and when complete, close the splash
            dataLoader = new BackgroundWorker();
            dataLoader.DoWork += LoadData;
            dataLoader.RunWorkerCompleted +=
                ((s, args) => Dispatcher.BeginInvoke(() =>
                    {
                        if (DataLoader.Information == null)
                        {
                            NavigationService.Navigate(new Uri("/Config.xaml", UriKind.Relative));
                        }
                        else if (DataLoader.Information.ActiveGame != null)
                        {
                            NavigationService.Navigate(
                                new Uri("/GameInfo.xaml?id=" +
                                        DataLoader.Information.ActiveGame.ID, UriKind.Relative));
                            MainMenu.Visibility = Visibility.Collapsed;
                        }
                        splashScreen.IsOpen = false;
                    }));
            dataLoader.RunWorkerAsync();
        }

        private void LoadData(object sender, DoWorkEventArgs doWorkEventArgs)
        {
            DataLoader.DownloadData(false);

            if (DataLoader.DataLoaded)
            {
                // Download all covers
                foreach (Game game in DataLoader.Games)
                {
                    Dispatcher.BeginInvoke(game.DownloadInfo);
                }
                DataLoader.RetrieveSettings();
            }
            else
            {
                // Probably an error occurred
                Dispatcher.BeginInvoke(() =>
                    {
                        splashScreen.IsOpen = false;
                        MessageBox.Show(
                            "An error occurred while downloading the gamelist. Verify that the IP address is correct, and that your XBOX360 is switched on.");
                        NavigationService.Navigate(new Uri("/Config.xaml", UriKind.Relative));
                    });
            }
        }

        private void OnWall(object sender, GestureEventArgs e)
        {
            NavigationService.Navigate(new Uri("/Wall.xaml", UriKind.Relative));
        }

        private void OnFolders(object sender, GestureEventArgs e)
        {
            NavigationService.Navigate(new Uri("/Wall.xaml?Structure=Folders", UriKind.Relative));
        }

        private void OnLists(object sender, GestureEventArgs e)
        {
            NavigationService.Navigate(new Uri("/List.xaml", UriKind.Relative));
        }

        private void OnFavs(object sender, GestureEventArgs e)
        {
            NavigationService.Navigate(new Uri("/Wall.xaml?Structure=Favs", UriKind.Relative));
        }

        private void OnSearch(object sender, GestureEventArgs e)
        {
            NavigationService.Navigate(new Uri("/Search.xaml", UriKind.Relative));
        }

        private void OnConfig(object sender, GestureEventArgs e)
        {
            NavigationService.Navigate(new Uri("/Config.xaml", UriKind.Relative));
        }

        private void OnInfo(object sender, GestureEventArgs e)
        {
            NavigationService.Navigate(new Uri("/About.xaml", UriKind.Relative));
        }
    }
}