using System;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using XK3Y.Web;

namespace XK3Y
{
    public partial class Search
    {
        private CollectionViewSource gameCollection;

        public Search()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            gameCollection = new CollectionViewSource {Source = DataLoader.Games};
            GameList.ItemsSource = gameCollection.View;
        }

        private void FilterText_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            string filter = FilterText.Text.ToLower();
            gameCollection.View.Filter = f =>
                    {
                        Game g = f as Game;
                        return g != null && g.Name.ToLower().Contains(filter);
                    };
        }

        private void Click(object sender, GestureEventArgs e)
        {
            Game game = (Game)((FrameworkElement)sender).DataContext;
            if (game == null) return;

            NavigationService.Navigate(new Uri("/GameInfo.xaml?id=" + game.ID, UriKind.Relative));
        }
    }
}