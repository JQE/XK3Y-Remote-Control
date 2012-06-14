using System.ComponentModel;
using System.Net;
using System.Windows;
using Microsoft.Phone.Controls;
using XK3Y.Web;
using GestureEventArgs = System.Windows.Input.GestureEventArgs;

namespace XK3Y
{
    public partial class Config
    {
        private NavigationInTransition navigationIn;

        public Config()
        {
            InitializeComponent();

            if (AppSettings.IPAddress != null)
                ipAddress.Text = AppSettings.IPAddress.ToString();
        }

        protected override void OnNavigatingFrom(System.Windows.Navigation.NavigatingCancelEventArgs e)
        {
            base.OnNavigatingFrom(e);

            if (NavigationContext.QueryString.ContainsKey("Initial"))
            {
                navigationIn = TransitionService.GetNavigationInTransition(this);
                TransitionService.SetNavigationInTransition(this, new NavigationInTransition());
            }
            else
                TransitionService.SetNavigationInTransition(this, navigationIn);
        }

        private void CheckAddress(object sender, RoutedEventArgs e)
        {
            IPAddress ip;
            ipAddressError.Visibility = IPAddress.TryParse(ipAddress.Text, out ip) && !ip.Equals(IPAddress.None) &&
                                        !ip.Equals(IPAddress.Any) && !ip.Equals(IPAddress.Broadcast) && !IPAddress.IsLoopback(ip)
                                            ? Visibility.Collapsed
                                            : Visibility.Visible;
        }

        private bool HasChanges
        {
            get
            {
                IPAddress ip;
                return ((IPAddress.TryParse(ipAddress.Text, out ip) && !ip.Equals(AppSettings.IPAddress))
                        || AppSettings.RefreshRate != (int) refreshRate.Value);
            }
        }

        protected override void OnBackKeyPress(CancelEventArgs e)
        {
            if (ipAddressError.Visibility == Visibility.Visible)
            {
                MessageBox.Show("You have to enter a valid IP address first.");
                e.Cancel = true;
            }
            if (HasChanges)
            {
                if (MessageBox.Show("You have unsaved changes. Do you want to save them now?", "Unsaved changed", MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                {
                    OnSave(this, null);
                }
            }

            IPAddress ip;
            if (IPAddress.TryParse(ipAddress.Text, out ip) && !ip.Equals(AppSettings.IPAddress))
            {
                // Reload data
                DataLoader.Reset();

                AppSettings.IPAddress = ip;
                AppSettings.Save();
            }

            base.OnBackKeyPress(e);
        }

        private void OnSave(object sender, GestureEventArgs e)
        {
            AppSettings.RefreshRate = (int) refreshRate.Value;

            IPAddress ip;
            if (IPAddress.TryParse(ipAddress.Text, out ip) && !ip.Equals(AppSettings.IPAddress))
            {
                // Reload data
                DataLoader.Reset();
                AppSettings.IPAddress = ip;
            }
            AppSettings.Save();
        }
    }
}