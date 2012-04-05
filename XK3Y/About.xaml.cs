using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using XK3Y.Web;

namespace XK3Y
{
    public partial class About
    {
        public About()
        {
            InitializeComponent();

            IEnumerable<Key> keys = new[]
                                        {
                                            new Key
                                                {
                                                    Name = "Created by",
                                                    Value = "r-win"
                                                },
                                            new Key
                                                {
                                                    Name = "Version",
                                                    Value = GetFileVersion()
                                                }
                                        };
            AuthorItems.ItemsSource = keys;
            AboutItems.ItemsSource = DataLoader.Information.About.Keys;
        }

        public static string GetFileVersion()
        {
            return Assembly.GetExecutingAssembly().GetCustomAttributes(false)
                .OfType<AssemblyFileVersionAttribute>()
                .First()
                .Version;
        }
    }
}