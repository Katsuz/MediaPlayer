using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace MediaPlayerProject.Converter
{
    internal class TimeSpanToText : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            TimeSpan input = (TimeSpan)value;
            int hours = input.Hours;
            int minutes = input.Minutes;
            int seconds = input.Seconds;
            string result = $"{hours}:{minutes}:{seconds}";
            return result;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
