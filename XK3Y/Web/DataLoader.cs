using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using System.Xml.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace XK3Y.Web
{
    public class DataLoader
    {
        public static bool DataLoaded;
        public static XK3Y Information;
        public static Store Store;
        public static List<Game> Games;
        public static ObservableCollection<GroupedObservableCollection<Game>> GroupedGames;
        private static readonly Random random = new Random();

        private static Timer _refreshTimer;

        public static void Reset()
        {
            StopTimer();

            DataLoaded = false;
            Information = null;
            Store = null;
            Games = null;
            GroupedGames = null;
        }

        private static bool PauseTimer { get; set; }

        private static void StartTimer()
        {
            int timeout = AppSettings.RefreshRate * 1000;
            _refreshTimer = new Timer(OnTimer, null, timeout, timeout);
        }

        private static void StopTimer()
        {
            if (_refreshTimer == null) return;
            _refreshTimer.Dispose();
            _refreshTimer = null;
        }

        public static void DownloadData(bool async = true, bool autoRefresh = true)
        {
            Uri uri = new Uri(String.Format("http://{0}/data.xml?t={1}", AppSettings.IPAddress, random.Next()));
            GetData(uri, async, OnDataOpened);

            if (!autoRefresh || _refreshTimer != null) return;

            StartTimer();
        }

        private static void OnTimer(object state)
        {
            if (PauseTimer) return;

            DownloadData(false, false);
        }

        private static void GetData(Uri uri, bool async, DownloadStringCompletedEventHandler handler)
        {
            // First, find the data.xml file, and apply it to the coverloader
            AutoResetEvent evt = async ? null : new AutoResetEvent(false);
            WebClient c = new WebClient();

            c.DownloadStringCompleted += handler;
            c.DownloadStringAsync(uri, evt);

            if (evt != null) evt.WaitOne();
        }

        private static void GetData(Uri uri, bool async, OpenReadCompletedEventHandler handler)
        {
            // First, find the data.xml file, and apply it to the coverloader
            AutoResetEvent evt = async ? null : new AutoResetEvent(false);
            WebClient c = new WebClient();
            c.Headers["Cache-Control"] = "no-cache";

            c.OpenReadCompleted += handler;
            c.OpenReadAsync(uri, evt);

            if (evt != null) evt.WaitOne();
        }

        public static void LaunchGame(string gameid, EventHandler launchGameComplete)
        {
            Uri uri = new Uri(string.Format("http://{0}/launchgame.sh?{1}&t={2}", AppSettings.IPAddress, gameid, random.Next()));
            GetData(uri, true, (sender, args) =>
                                   {
                                       OnDataOpened(sender, args);
                                       launchGameComplete(sender, args);
                                   });
        }

        private static void OnDataOpened(object sender, OpenReadCompletedEventArgs openReadCompletedEventArgs)
        {
            EventWaitHandle evt = openReadCompletedEventArgs.UserState as EventWaitHandle;
            if (openReadCompletedEventArgs.Error != null)
            {
                if (evt != null) evt.Set();
                return;
            }

            XmlSerializer deser = new XmlSerializer(typeof (XK3Y), new[] {typeof (Game), typeof (Directory)});
            XK3Y info = (XK3Y)deser.Deserialize(openReadCompletedEventArgs.Result);
            if (Information != null)
            {
                Information.TrayState = info.TrayState;
                Information.GuiState = info.GuiState;
                Information.Emergency = info.Emergency;
                Information.Active = info.Active;
                if (evt != null) evt.Set();
                return;
            }
            Information = info;

            // Set the game information to the source of the coverflow
            Games = new List<Game>();
            if (Information.Games.Hdds != null)
            {
                foreach (Hdd hdd in Information.Games.Hdds)
                {
                    if (hdd.DirectoryItems != null)
                    {
                        foreach (Directory directory in hdd.DirectoryItems.OfType<Directory>())
                        {
                            AddGames(directory);
                        }
                    }
                    if (hdd.Games != null) Games.AddRange(hdd.Games);
                }
            }
            GroupedGames = Games.ToGroupedOC(g => g.Key);
            DataLoaded = true;
            if (evt != null) evt.Set();
        }

        private static void AddGames(Directory directory)
        {
            if (directory.DirectoryItems != null)
            {
                foreach (Directory dir in directory.DirectoryItems.OfType<Directory>())
                {
                    AddGames(dir);
                }
            }
            if (directory.Games == null) return;
            Games.AddRange(directory.Games);
        }

        public static Store RetrieveSettings()
        {
            Uri uri = new Uri(String.Format("http://{0}/store.sh", AppSettings.IPAddress));
            GetData(uri, false, (sender, args) => {
                AutoResetEvent evt = args.UserState as AutoResetEvent;
                if (args.Error == null && !string.IsNullOrEmpty(args.Result))
                {
                    JObject j = JObject.Parse(args.Result);
                    Store = JsonConvert.DeserializeObject<Store>(args.Result);
                    Store.Remove("FavLists");
                    Store.FavLists = new FavLists();
                    if (j["FavLists"] != null)
                    {
                        // We can deserialize this list in one call with JsonConvert.DeserializeObject<FavLists>(j["FavLists"].ToString());
                        // but we want to correctly "bind" the games in the Favorite lists to the Games in the games list returned in
                        // data.xml
                        foreach (JProperty jProperty in j["FavLists"].OfType<JProperty>().Where(p => p.Count > 0))
                        {
                            List<Game> games = new List<Game>();
                            games.AddRange(
                                jProperty.First.Cast<JObject>().Select(
                                    game =>
                                    Games.FirstOrDefault(
                                        g =>
                                        g.ID.Equals(game["id"].ToString(), StringComparison.InvariantCultureIgnoreCase)))
                                    .Where(favGame => favGame != null));
                            Store.FavLists.Add(new FavList(games) {HtmlName = jProperty.Name});
                        }
                    }
                }
                if (evt != null) evt.Set();
            });
            return Store;
        }

        public static void SaveSettings()
        {
            JObject j = JObject.FromObject(Store);
            j.Add("FavLists", new JObject());
            if (Store.FavLists != null)
            {
                foreach (FavList favList in Store.FavLists)
                {
                    ((JObject) j["FavLists"]).Add(favList.HtmlName, JArray.FromObject(favList));
                }
            }

            Uri uri = new Uri(String.Format("http://{0}/store.sh", AppSettings.IPAddress));
            WebClient c = new WebClient();
            c.OpenWriteCompleted += (sender, args) =>
                    {
                        if (args.Error != null) return;
                        using (StreamWriter sw = new StreamWriter(args.Result))
                        {
                            sw.Write(j.ToString());
                        }
                    };
            c.OpenWriteAsync(uri);
        }
    }
}
