using System;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Phone.Controls;
using XK3Y.Web;

namespace XK3Y
{
    public partial class PickFavoriteList
    {
        private readonly PhoneApplicationPage Page;

        public PickFavoriteList(PhoneApplicationPage parent)
        {
            InitializeComponent();

            // Set the listpicker 
            SelectList.IsEnabled = DataLoader.Store != null && DataLoader.Store.FavLists != null &&
                                   DataLoader.Store.FavLists.Count > 0;
            ListSelect.ItemsSource = DataLoader.Store != null ? DataLoader.Store.FavLists : null;

            Page = parent;
            Page.OrientationChanged += ChangeSize;

            SetSize();
        }

        private void ChangeSize(object sender, OrientationChangedEventArgs e)
        {
            SetSize();
        }

        public bool IsPortrait
        {
            get
            {
                return Page.Orientation == PageOrientation.Portrait || Page.Orientation == PageOrientation.PortraitUp ||
                       Page.Orientation == PageOrientation.PortraitDown;
            }
        }

        public void SetSize()
        {
            LayoutRoot.Width = IsPortrait ? Application.Current.RootVisual.RenderSize.Width : Application.Current.RootVisual.RenderSize.Height;
            LayoutRoot.Height = IsPortrait ? Application.Current.RootVisual.RenderSize.Height : Application.Current.RootVisual.RenderSize.Width;
        }

        private void Ok_Click(object sender, RoutedEventArgs e)
        {
            if ((SelectList.IsChecked.GetValueOrDefault(false) && ListSelect.SelectedItem == null) ||
                (NewList.IsChecked.GetValueOrDefault(false) && string.IsNullOrEmpty(Listname.Text))) return;

            Result = MessageBoxResult.OK;
            CloseWindow();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            Result = MessageBoxResult.Cancel;
            CloseWindow();
        }

        public event EventHandler Close;

        private void CloseWindow()
        {
            Page.OrientationChanged -= ChangeSize;
            if (Close != null) Close(this, EventArgs.Empty);
        }

        public string SelectedList
        {
            get {
                return SelectList.IsChecked.GetValueOrDefault(false)
                           ? ListSelect.SelectedItem.ToString()
                           : Listname.Text;
            }
        }

        public MessageBoxResult Result { get; private set; }

        private void OnSelectChecked(object sender, RoutedEventArgs e)
        {
            bool ok = ((SelectList.IsChecked.GetValueOrDefault(false) && ListSelect.SelectedItem != null) ||
                       (NewList.IsChecked.GetValueOrDefault(false) && !string.IsNullOrEmpty(Listname.Text)));

            OkButton.IsEnabled = ok;
        }

        private void OnTextChanged(object sender, TextChangedEventArgs e)
        {
            bool ok = ((SelectList.IsChecked.GetValueOrDefault(false) && ListSelect.SelectedItem != null) ||
                       (NewList.IsChecked.GetValueOrDefault(false) && !string.IsNullOrEmpty(Listname.Text)));

            OkButton.IsEnabled = ok;
        }
    }
}
