using System;
using System.Windows.Data;
using System.Windows.Media;

namespace MountainBikeTracker_WP8.Helpers.Converters
{
    public class CurrentSpeedToForegroundConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            double currentSpeed = (double)value;
            double averageThreshold = App.CurrentRideViewModel.AverageSpeed;

            SolidColorBrush color = new SolidColorBrush(Colors.White);

            if (currentSpeed > (averageThreshold + 1))
            {
                color = new SolidColorBrush(Colors.Cyan);
            }
            else if (currentSpeed < (averageThreshold - 1))
            {
                color = new SolidColorBrush(Colors.Red);
            }

            return color;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}