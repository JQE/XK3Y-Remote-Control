using System;
using System.IO;
using System.Windows.Media.Imaging;
using System.Xml.Serialization;

namespace XK3Y.Web
{
    [XmlRoot("gameinfo")]
    public class GameInfo
    {
        #region Xml Elements

        [XmlElement("banner")]
        public string BannerString { get; set; }

        [XmlElement("boxart")]
        public string BoxartString { get; set; }

        [XmlElement("title")]
        public string Title { get; set; }

        [XmlElement("summary")]
        public string Summary { get; set; }

        [XmlElement("info")]
        public Info Info { get; set; }

        #endregion

        private static BitmapImage GetImage(string content)
        {
            BitmapImage image;
            byte[] img = Convert.FromBase64String(content);
            using (MemoryStream ms = new MemoryStream(img))
            {
                image = new BitmapImage();
                image.SetSource(ms);
            }
            return image;
        }

        private BitmapImage _boxart;
        private BitmapImage _banner;

        public BitmapImage Boxart
        {
            get
            {
                if (_boxart == null && !string.IsNullOrEmpty(BoxartString))
                {
                    _boxart = GetImage(BoxartString);
                }
                return _boxart;
            }
        }

        public BitmapImage Banner
        {
            get
            {
                if (_banner == null && !string.IsNullOrEmpty(BannerString))
                {
                    _banner = GetImage(BannerString);
                }
                return _banner;
            }
        }
    }

    public class Info
    {
        [XmlElement("infoitem")]
        public InfoItem[] Items { get; set; }
    }

    public class InfoItem
    {
        [XmlAttribute("name")]
        public string Name { get; set; }

        [XmlText]
        public string Value { get; set; }
    }
}
