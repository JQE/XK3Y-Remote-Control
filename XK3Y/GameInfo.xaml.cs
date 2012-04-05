using System;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Media.Imaging;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using XK3Y.Web;

namespace XK3Y
{
    public partial class GameInfo : INotifyPropertyChanged
    {
        public GameInfo()
        {
            InitializeComponent();
        }

        protected override void OnNavigatingFrom(System.Windows.Navigation.NavigatingCancelEventArgs e)
        {
            // Remove the events from DataLoader
            DataLoader.Information.PropertyChanged -= OnPropertyChanged;
            Game.PropertyChanged -= OnPropertyChanged;

            base.OnNavigatingFrom(e);
        }

        private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch(e.PropertyName)
            {
                case "TrayState":
//                    ((ApplicationBarIconButton)ApplicationBar.Buttons[0]).IsEnabled = IsTrayOpen;
                    break;
                case "FavoriteUri":
                    ((ApplicationBarIconButton)ApplicationBar.Buttons[1]).IconUri = FavoriteUri;
                    break;
                case "Active":
                    ((ApplicationBarIconButton)ApplicationBar.Buttons[0]).IsEnabled = !IsCurrentlyMounted;
                    RaisePropertyChanged("IsCurrentlyMounted");
                    break;
                case "Summary":
                    AddRemovePivotItem(g => !string.IsNullOrEmpty(Summary), SummaryPivot);
                    RaisePropertyChanged("Summary");
                    break;
                case "Info":
                    AddRemovePivotItem(g => Info != null, AdditionalInfoPivot);
                    RaisePropertyChanged("Info");
                    break;
            }
        }

        private void AddRemovePivotItem(Func<GameInfo, bool> add, PivotItem pivotItem)
        {
            if (add.Invoke(this))
            {
                if (!pivot.Items.Contains(pivotItem)) pivot.Items.Add(pivotItem);
            }
            else
                pivot.Items.Remove(pivotItem);
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            if (!NavigationContext.QueryString.ContainsKey("id"))
            {
                NavigationService.GoBack();
                return;
            }

            Game = DataLoader.Games.FirstOrDefault(g => g.ID == NavigationContext.QueryString["id"]);
            if (Game == null)
            {
                NavigationService.GoBack();
                return;
            }

            DataLoader.Information.PropertyChanged += OnPropertyChanged;
            Game.PropertyChanged += OnPropertyChanged;

            AddRemovePivotItem(g => !string.IsNullOrEmpty(Summary), SummaryPivot);
            AddRemovePivotItem(g => Info != null, AdditionalInfoPivot);

            ((ApplicationBarIconButton)ApplicationBar.Buttons[0]).IsEnabled = !IsCurrentlyMounted;
//            ((ApplicationBarIconButton)ApplicationBar.Buttons[0]).IsEnabled = IsTrayOpen;
            ((ApplicationBarIconButton)ApplicationBar.Buttons[1]).IconUri = FavoriteUri;
        }

        protected override void OnBackKeyPress(CancelEventArgs e)
        {
            if (pickList.IsOpen)
            {
                pickList.IsOpen = false;
                e.Cancel = true;
            }

            base.OnBackKeyPress(e);
        }

        public Game Game { get; private set; }

        public bool IsAnotherGameMounted
        {
            get { return DataLoader.Information.GuiState == GuiState.GameLoaded && DataLoader.Information.Active != Game.ID; }
        }

        public bool IsCurrentlyMounted
        {
            get { return DataLoader.Information.Active == Game.ID; }
        }

        public bool IsTrayOpen
        {
            get { return DataLoader.Information.TrayState == TrayState.Open; }
        }

        public bool IsFavorite
        {
            get { return DataLoader.Store != null && DataLoader.Store.FavLists.Any(f => f.Contains(Game)); }
        }

        public Uri FavoriteUri
        {
            get
            {
                bool dark = ((Visibility) Application.Current.Resources["PhoneDarkThemeVisibility"] ==
                             Visibility.Visible);
                string fav = string.Format("/Metro/{0}/appbar.favs.{1}rest.png", dark ? "dark" : "light", IsFavorite ? string.Empty : "addto.");
                return new Uri(fav, UriKind.Relative);
            }
        }

        public string Gamename
        {
            get { return Game.Name.ToLower(); }
        }

        public string Description
        {
            get { return Game.Name; }
        }

        public BitmapImage Cover
        {
            get { return Game.Cover; }
        }

        public InfoItem[] Info
        {
            get { return Game.InfoItems; }
        }

        public string Summary
        {
            get { return Game.Summary; }
        }

        private void Play(object sender, EventArgs e)
        {
            if (pickList.IsOpen) return;

            if (!IsTrayOpen)
            {
                if (IsAnotherGameMounted)
                    MessageBox.Show(
                        string.Format("The game '{0}' is already loaded. Please open the tray before loading another game.",
                        DataLoader.Information.ActiveGame.Name));
                else
                    MessageBox.Show("You'll have to open the tray before you can mount a game. Please open the tray, and try again.");
                return;
            }

            if (Game.Info == null) Game.Info = new PlayInfo();
            Game.Info.TimesPlayed++;
            Game.Info.LastPlayed = (long) (DateTime.UtcNow - new DateTime(1970, 1, 1).ToLocalTime()).TotalSeconds;
            DataLoader.UpdateData(Game.ID);
            DataLoader.SaveSettings();
        }
        
        private void Favorite(object sender, EventArgs e)
        {
            if (pickList.IsOpen) return;

            // If is added as favorite, we might undo that
            if (IsFavorite)
            {
                // Remove from the list
                FavList favList = DataLoader.Store.FavLists.FirstOrDefault(f => f.Contains(Game));
                if (favList != null)
                {
                    favList.Remove(Game);
                    if (favList.Count == 0)
                    {
                        DataLoader.Store.FavLists.Remove(favList);
                    }
                }
                DataLoader.SaveSettings();
                OnPropertyChanged(this, new PropertyChangedEventArgs("FavoriteUri"));
            }
            else
            {
                // Add to favorite list...sometime
                // Add a picklist for the favorite list, or allow ppl to create a new list
                PickFavoriteList pickFavList = new PickFavoriteList(this);
                pickFavList.Close += PicklistClosed;
                pickList.Child = pickFavList;
                pickList.IsOpen = true;
                ApplicationBar.IsVisible = false;
            }
        }

        private void PicklistClosed(object sender, EventArgs eventArgs)
        {
            if (pickList.IsOpen)
            {
                pickList.IsOpen = false;
            }
            ApplicationBar.IsVisible = true;

            PickFavoriteList list = (PickFavoriteList) sender;
            if (list.Result != MessageBoxResult.OK) return;

            // Fetch the new list name, or the selected one
            string name = list.SelectedList;
            FavList favList = DataLoader.Store != null
                                  ? DataLoader.Store.FavLists.FirstOrDefault(f => f.Name == name)
                                  : null;
            if (favList == null)
            {
                favList = new FavList(Enumerable.Empty<Game>()) {Name = name};
                if (DataLoader.Store == null) DataLoader.Store = new Store();
                if (DataLoader.Store.FavLists == null) DataLoader.Store.FavLists = new FavLists();
                DataLoader.Store.FavLists.Add(favList);
            }
            favList.Add(Game);
            DataLoader.SaveSettings();
            OnPropertyChanged(this, new PropertyChangedEventArgs("FavoriteUri"));
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void RaisePropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}