using System;
using System.Xml.Serialization;

namespace XK3Y.Web
{
    public partial class Directory
    {
        [XmlIgnore]
        public override Uri ImageUri
        {
            get { return new Uri("/Metro/folder.png", UriKind.Relative); }
        }
    }
}
