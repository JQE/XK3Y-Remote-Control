using System;
using System.Windows;
using Microsoft.Phone.Controls;

namespace XK3Y
{
    /// <summary>
    /// The same control as the HubTile control, expect the initial Visual State is SemiExanded. There is probably a better way ;)
    /// </summary>
    public class HubTileSemi : HubTile
    {
        private static readonly Random random = new Random();

        private static readonly string[] States = new[] {"Expanded", "Semiexpanded", "Collapsed"};

        public HubTileSemi()
        {
            Loaded += (sender, args) => VisualStateManager.GoToState(this, States[random.Next(0, States.Length)], true);
        }
    }
}
