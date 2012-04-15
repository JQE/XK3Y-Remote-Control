using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Runtime.Serialization;
using System.Windows.Media.Imaging;
using Newtonsoft.Json;

namespace XK3Y.Web
{
    public class Store : Dictionary<string, PlayInfo> // key = gameid
    {
        public FavLists FavLists; // Key = listname

        [OnDeserialized]
        public void OnDeserialized(StreamingContext context)
        {
            foreach (KeyValuePair<string, PlayInfo> pair in this)
            {
                Game game = DataLoader.Games.FirstOrDefault(g => g.ID == pair.Key);
                if (game != null) game.Info = pair.Value;
            }
        }
    }

    public class FavLists : List<FavList>
    {
        public FavList this[string name]
        {
            get { return this.FirstOrDefault(f => f.Name == name || f.HtmlName == name); }
        }

        public bool Contains(string name)
        {
            return this[name] != null;
        }
    }

    [JsonArray]
    public class FavList : List<Game>
    {
        public FavList(IEnumerable<Game> games)
            : base(games)
        {
        }

        [JsonIgnore]
        public string Name 
        {
            get { return HttpUtility.UrlDecode(HtmlName); }
            set { HtmlName = HttpUtility.UrlEncode(value); }
        }

        [JsonProperty("name")]
        public string HtmlName { get; set; }

        public Uri ImageUri
        {
            get { return new Uri("/Metro/fav.png", UriKind.Relative); }
        }

        private BitmapImage cover;

        public BitmapImage Cover
        {
            get { return cover ?? (cover = new BitmapImage(ImageUri)); }
        }

        public override string ToString()
        {
            return Name;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != typeof (FavList)) return false;
            return Equals((FavList) obj);
        }

        public bool Equals(FavList other)
        {
            if (ReferenceEquals(null, other)) return false;
            return ReferenceEquals(this, other) || Equals(other.HtmlName, HtmlName);
        }

        public override int GetHashCode()
        {
            return (HtmlName != null ? HtmlName.GetHashCode() : 0);
        }
    }

    public class PlayInfo
    {
        [JsonProperty("lastPlayed")]
        public long LastPlayed;
        [JsonProperty("timesPlayed")]
        public int TimesPlayed;
    }
}
