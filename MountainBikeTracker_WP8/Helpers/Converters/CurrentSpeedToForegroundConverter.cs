using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;

namespace MountainBikeTracker_WP8.Helpers.Converters
{
    public class CurrentSpeedToForegroundConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            double currentSpeed = (double)value;
            double averageThreshold = App.CurrentRideViewModel.CurrentTrail.AverageSpeed + 1;

            SolidColorBrush color = new SolidColorBrush(Colors.White);
            if (currentSpeed > averageThreshold)
            {
                color = new SolidColorBrush(Colors.Cyan);
            }
            else if (currentSpeed < averageThreshold)
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
