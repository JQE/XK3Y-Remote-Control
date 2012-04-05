using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Media;

namespace XK3Y
{
    public class GroupedObservableCollection<T> : ObservableCollection<T>
    {
        /// <summary>
        /// The Group Title
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Constructor ensure that a Group Title is included
        /// </summary>
        /// <param name="name">string to be used as the Group Title</param>
        public GroupedObservableCollection(string name)
        {
            Title = name;
        }

        /// <summary>
        /// Returns true if the group has a count more than zero
        /// </summary>
        public bool HasItems
        {
            get { return (Count != 0); }
        }

        public Brush Brush
        {
            get { return (Brush) Application.Current.Resources[HasItems ? "PhoneAccentBrush" : "PhoneChromeBrush"]; }
        }
    }

    public static class GroupedOCHelper
    {
        public static ObservableCollection<GroupedObservableCollection<T>> ToGroupedOC<T>(this IEnumerable<T> list, Func<T, char> expr)
        {
            ObservableCollection<GroupedObservableCollection<T>> collection = new ObservableCollection<GroupedObservableCollection<T>>();
            const string alpha = "#abcdefghijklmnopqrstuvwxyz";

            foreach (char c in alpha)
            {
                GroupedObservableCollection<T> group = new GroupedObservableCollection<T>(c.ToString(CultureInfo.InvariantCulture));
                foreach (T item in list.Where(item => expr.Invoke(item) == c))
                {
                    group.Add(item);
                }
                collection.Add(group);
            }
            return collection;
        }
    }
}
