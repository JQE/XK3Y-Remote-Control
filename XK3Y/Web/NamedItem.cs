using System;
using System.ComponentModel;
using System.Windows.Media.Imaging;
using System.Xml.Serialization;
using Newtonsoft.Json;

namespace XK3Y.Web
{
    public partial class NamedItem : INotifyPropertyChanged
    {
        [XmlIgnore]
        [JsonIgnore]
        public abstract Uri ImageUri { get; }

        [XmlIgnore]
        [JsonIgnore]
        private BitmapImage cover;

        [XmlIgnore]
        [JsonIgnore]
        public BitmapImage Cover
        {
            get { return cover; }
            protected set
            {
                cover = value;
                RaisePropertyChanged("Cover");
            }
        }

        [XmlIgnore]
        [JsonIgnore]
        private char? key;

        [XmlIgnore]
        [JsonIgnore]
        public char Key
        {
            get
            {
                if (key == null)
                {
                    key = char.ToLower(Name[0]);
                    if (key < 'a' && key > 'z') key = '#';
                }
                return key.Value;
            }
        }


        public event PropertyChangedEventHandler PropertyChanged;

        // Create the OnPropertyChanged method to raise the event
        protected void RaisePropertyChanged(string name)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(name));
            }
        }
    }
}
