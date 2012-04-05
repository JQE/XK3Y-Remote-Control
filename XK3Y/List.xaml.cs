using System;
using System.Windows;
using System.Windows.Input;
using XK3Y.Web;

namespace XK3Y
{
    public partial class List
    {
        public List()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            GameList.ItemsSource = DataLoader.GroupedGames;
        }

        private void Click(object sender, GestureEventArgs e)
        {
            Game game = (Game) ((FrameworkElement) sender).DataContext;
            if (game == null) return;

            NavigationService.Navigate(new Uri("/GameInfo.xaml?id=" + game.ID, UriKind.Relative));
        }
    }
}