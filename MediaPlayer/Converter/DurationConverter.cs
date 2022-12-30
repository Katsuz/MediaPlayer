using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace MediaPlayerProject.Converter
{
    class DurationConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            TimeSpan Duration = (TimeSpan)value;
            int hours = Duration.Hours;
            int minutes = Duration.Minutes;
            int seconds = Duration.Seconds;
            string result = $"{hours}:{minutes}:{seconds}";

            return result;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
