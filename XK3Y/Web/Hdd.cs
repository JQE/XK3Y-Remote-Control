using System;
using System.Xml.Serialization;

namespace XK3Y.Web
{
    public partial class Hdd
    {
        [XmlIgnore]
        public override Uri ImageUri
        {
            get { return new Uri("/Metro/hdd.png", UriKind.Relative); }
        }
    }
}
