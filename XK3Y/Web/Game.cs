using System;
using System.Net;
using System.Windows.Media.Imaging;
using System.Xml.Serialization;
using Newtonsoft.Json;

namespace XK3Y.Web
{
    public partial class Game
    {
        private readonly object _lockObject = new object();
        private bool _downloadingInformation;
        private bool _hasInformation;

        [XmlIgnore]
        [JsonIgnore]
        public override Uri ImageUri
        {
            get { return new Uri(string.Format("http://{0}/covers/{1}.jpg", AppSettings.IPAddress, ID)); }
        }

        [XmlIgnore]
        [JsonIgnore]
        public Uri XmlUri
        {
            get { return new Uri(string.Format("http://{0}/covers/{1}.xml", AppSettings.IPAddress, ID)); }
        }

        public event EventHandler OnDownloadCoverComplete;

        [XmlIgnore, JsonIgnore]
        public PlayInfo Info
        {
            get { return DataLoader.Store != null && DataLoader.Store.ContainsKey(ID) ? DataLoader.Store[ID] : null; }
            set 
            { 
                if (DataLoader.Store == null) DataLoader.Store = new Store();
                DataLoader.Store[ID] = value; 
            }
        }

        public void DownloadInfo()
        {
            if (Cover != null) return;
            Cover = new BitmapImage(new Uri("images\nocover.jpg", UriKind.Relative));

            WebClient c = new WebClient();
            c.OpenReadCompleted += OnInfoRetrieved;
            c.OpenReadAsync(XmlUri);
        }

        private string _summary;

        [XmlIgnore]
        [JsonIgnore]
        public string Summary
        {
            get
            {
                if (_hasInformation) return _summary;
                RetrieveInformation();
                return string.Empty;
            }
            set
            {
                if (_summary == value) return;
                _summary = value;
                RaisePropertyChanged("Summary");
            }
        }

        private InfoItem[] _info;

        [XmlIgnore]
        [JsonIgnore]
        public InfoItem[] InfoItems
        {
            get
            {
                if (_hasInformation) return _info;
                RetrieveInformation();
                return null;
            }
            set
            {
                if (_info != null) return;
                _info = value;
                RaisePropertyChanged("Info");
            }
        }

        private void RetrieveInformation()
        {
            lock (_lockObject)
            {
                if (_downloadingInformation || _hasInformation) return;

                _downloadingInformation = true;
                // Download the game.xml file
                WebClient c = new WebClient();
                c.OpenReadCompleted += OnInfoRetrieved;
                c.OpenReadAsync(new Uri(string.Format("http://{0}/covers/{1}.xml?t={2}", AppSettings.IPAddress, ID, new Random().Next())));
            }
        }

        private void OnInfoRetrieved(object sender, OpenReadCompletedEventArgs openReadCompletedEventArgs)
        {
            if (openReadCompletedEventArgs.Error != null) return;

            XmlSerializer deser = new XmlSerializer(typeof(GameInfo));
            GameInfo info = (GameInfo)deser.Deserialize(openReadCompletedEventArgs.Result);

            _hasInformation = true;
            _downloadingInformation = false;

            Cover = info.Boxart;

            if (info.Summary != null) Summary = info.Summary;
            if (info.Info != null) InfoItems = info.Info.Items;

            if (OnDownloadCoverComplete != null)
                OnDownloadCoverComplete(this, EventArgs.Empty);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != typeof (Game)) return false;
            return Equals((Game) obj);
        }

        public bool Equals(Game other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Equals(other.ID, ID);
        }

        public override int GetHashCode()
        {
            return (ID != null ? ID.GetHashCode() : 0);
        }
    }
}
