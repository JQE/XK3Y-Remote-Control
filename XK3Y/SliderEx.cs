using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace XK3Y
{
    /// <summary>
    /// A slider which snaps to integer positions only
    /// </summary>
    public class SliderEx : Slider
    {
        protected override void OnValueChanged(double oldValue, double newValue)
        {
            int val = Convert.ToInt32(Math.Round(newValue));

            Thumb ElementHorizontalThumb = GetTemplateChild("HorizontalThumb") as Thumb;

            double maximum = Maximum;
            double minimum = Minimum;

            double num3 = val;
            double num4 = 1.0 - ((maximum - num3) / (maximum - minimum));

            RepeatButton ElementHorizontalLargeDecrease = GetTemplateChild("HorizontalTrackLargeChangeDecreaseRepeatButton") as RepeatButton;
            RepeatButton ElementHorizontalLargeIncrease = GetTemplateChild("HorizontalTrackLargeChangeIncreaseRepeatButton") as RepeatButton;

            Grid grid = GetTemplateChild("HorizontalTemplate") as Grid;

            if (grid != null)
            {

                if ((grid.ColumnDefinitions != null) && (grid.ColumnDefinitions.Count == 3))
                {
                    grid.ColumnDefinitions[0].Width = new GridLength(1.0, GridUnitType.Auto);
                    grid.ColumnDefinitions[2].Width = new GridLength(1.0, GridUnitType.Star);

                    if (ElementHorizontalLargeDecrease != null)
                    {
                        ElementHorizontalLargeDecrease.SetValue(Grid.ColumnProperty, 0);
                    }
                    if (ElementHorizontalLargeIncrease != null)
                    {
                        ElementHorizontalLargeIncrease.SetValue(Grid.ColumnProperty, 2);
                    }
                }
                if ((ElementHorizontalLargeDecrease != null) && (ElementHorizontalThumb != null))
                {
                    ElementHorizontalLargeDecrease.Width = Math.Max(0.0, num4 * (ActualWidth - ElementHorizontalThumb.ActualWidth));
                }
            }
        }

    }
}
