using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Xml.Serialization;

namespace XK3Y.Web
{
    public partial class XK3Y : INotifyPropertyChanged
    {
        [XmlIgnore]
        public Game ActiveGame
        {
            get { return DataLoader.Games == null || string.IsNullOrEmpty(Active) ? null : DataLoader.Games.FirstOrDefault(g => g.ID == Active); }
        }

        private void RaisePropertyChanged(string propertyName)
        {
            // Make this thread execute in the current UI thread. If not, event handlers might throw an error
            if (!Deployment.Current.Dispatcher.CheckAccess())
            {
                Deployment.Current.Dispatcher.BeginInvoke(() => RaisePropertyChanged(propertyName));
            }
            else if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
