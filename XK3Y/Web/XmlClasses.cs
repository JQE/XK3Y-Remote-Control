using System.Xml.Serialization;
using Newtonsoft.Json;

namespace XK3Y.Web
{
    public enum GuiState
    {
        [XmlEnum(Name = "1")]
        NoGameLoaded,
        [XmlEnum(Name = "2")]
        GameLoaded
    }

    public enum TrayState
    {
        [XmlEnum(Name = "0")]
        Open,
        [XmlEnum(Name = "1")]
        Closed
    }

    [XmlRoot("XKEY")]
    public partial class XK3Y
    {
        private GuiState _guiState;

        [XmlElement("GUISTATE")]
        public GuiState GuiState
        {
            get { return _guiState; }
            set
            {
                if (_guiState == value) return;
                _guiState = value;
                RaisePropertyChanged("GuiState");
            }
        }

        private TrayState _trayState;

        [XmlElement("TRAYSTATE")]
        public TrayState TrayState
        {
            get { return _trayState; }
            set
            {
                if (_trayState == value) return;
                _trayState = value;
                RaisePropertyChanged("TrayState");
            }
        }

        private int _emergency;

        [XmlElement("EMERGENCY")]
        public int Emergency
        {
            get { return _emergency; }
            set
            {
                if (_emergency == value) return;
                _emergency = value;
                RaisePropertyChanged("Emergency");
            }
        }

        private string _active;

        [XmlElement("ACTIVE")]
        public string Active
        {
            get { return _active; }
            set
            {
                if (_active == value) return;
                _active = value;
                RaisePropertyChanged("Active");
            }
        }

        [XmlElement("GAMES")]
        public Games Games { get; set; }

        [XmlElement("ABOUT")]
        public About About { get; set; }
    }

    public class Games
    {
        [XmlElement("MOUNT")]
        public Hdd[] Hdds { get; set; }
    }

    public abstract partial class NamedItem
    {
        [XmlIgnore]
        public abstract string Name { get; set; }
    }

    public abstract class DirectoryItem : NamedItem
    {
        [XmlElement("MOUNT", Type = typeof(Hdd))]
        [XmlElement("DIR", Type = typeof(Directory))]
        public DirectoryItem[] DirectoryItems { get; set; }
    }

    public partial class Hdd : Directory
    {
    }

    public partial class Directory : DirectoryItem
    {
        [XmlAttribute("NAME")]
        public override string Name { get; set; }

        [XmlElement("ISO")]
        public Game[] Games { get; set; }
    }

    public partial class Game : NamedItem
    {
        [XmlElement("TITLE")]
        [JsonProperty("name")]
        public override string Name { get; set; }

        [XmlElement("ID")]
        [JsonProperty("id")]
        public string ID { get; set; }
    }

    public class About
    {
        [XmlElement("ITEM")]
        public Key[] Keys { get; set; }
    }

    public class Key
    {
        [XmlAttribute("NAME")]
        public string Name { get; set; }

        [XmlText]
        public string Value { get; set; }
    }
}
