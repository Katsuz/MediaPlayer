using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace MediaPlayerProject.Converter
{
    internal class TimeSpanToDouble : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            TimeSpan input= (TimeSpan)value;
            double result = input.TotalSeconds;
            return result;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            double input = (double)value;
            TimeSpan result = TimeSpan.FromSeconds(input);
            return result;
        }
    }
}
