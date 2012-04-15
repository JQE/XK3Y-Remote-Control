
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using XK3Y.Web;

namespace XK3Y
{
    public partial class Wall
    {
        public Wall()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            if (NavigationContext.QueryString.ContainsKey("Structure"))
            {
                switch(NavigationContext.QueryString["Structure"])
                {
                    case "Folders":
                        ShowFolders();
                        break;
                    case "Favs":
                        ShowFavs();
                        break;
                    default:
                        if (NavigationService.CanGoBack) NavigationService.GoBack();
                        else NavigationService.Navigate(new Uri("/MainPage.xaml", UriKind.Relative));
                        return;
                }
            }
            else
                MainMenu.ItemsSource = DataLoader.Games;
        }

        private void ShowFolders()
        {
            // Which element is the current element?
            if (NavigationContext.QueryString.ContainsKey("Path"))
            {
                string[] parts = NavigationContext.QueryString["Path"].Split('/');
                DirectoryItem currentItem = parts.Aggregate<string, DirectoryItem>(null, (current, part) => current != null ? current.DirectoryItems.FirstOrDefault(d => d.Name == part) : DataLoader.Information.Games.Hdds.FirstOrDefault(h => h.Name == part));
                if (currentItem == null)
                {
                    NavigationService.Navigate(new Uri("/MainPage.xaml", UriKind.Relative));
                    return;
                }

                IEnumerable<NamedItem> childs = currentItem.DirectoryItems;
                if (currentItem is Directory) childs = childs == null ? ((Directory)currentItem).Games : childs.Union(((Directory)currentItem).Games);
                MainMenu.ItemsSource = childs;
            }
            else
                MainMenu.ItemsSource = DataLoader.Information.Games.Hdds;
        }

        private void ShowFavs()
        {
            // Which element is the current element?
            if (NavigationContext.QueryString.ContainsKey("Path") && DataLoader.Store.FavLists.Contains(NavigationContext.QueryString["Path"]))
            {
                FavList list = DataLoader.Store.FavLists[NavigationContext.QueryString["Path"]];
                if (list == null)
                {
                    if (NavigationService.CanGoBack) NavigationService.GoBack();
                    else NavigationService.Navigate(new Uri("/MainPage.xaml", UriKind.Relative));
                    return;
                }
                MainMenu.ItemsSource = list;
            }
            else
                MainMenu.ItemsSource = DataLoader.Store != null ? DataLoader.Store.FavLists : null;
        }

        private void Click(object sender, GestureEventArgs gestureEventArgs)
        {
            object obj = ((FrameworkElement) sender).DataContext;

            DirectoryItem directoryItem = obj as DirectoryItem;
            if (directoryItem != null)
            {
                // Navigate into that directoryitem
                string path = string.Empty;
                if (NavigationContext.QueryString.ContainsKey("Path"))
                    path = NavigationContext.QueryString["Path"] + "/";
                path += (directoryItem).Name;

                NavigationService.Navigate(new Uri("/Wall.xaml?Structure=Folders&Path=" + path, UriKind.Relative));
                return;
            }

            FavList favList = obj as FavList;
            if (favList != null)
            {
                NavigationService.Navigate(new Uri("/Wall.xaml?Structure=Favs&Path=" + favList.Name, UriKind.Relative));
                return;
            }

            Game game = obj as Game;
            if (game == null) return;

            NavigationService.Navigate(new Uri("/GameInfo.xaml?id=" + game.ID, UriKind.Relative));
        }
    }
}