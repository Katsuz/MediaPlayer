using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace MediaPlayerProject.Converter
{
    class DateTimeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            DateTime addedDate = (DateTime)value;
            var today = DateTime.Now;
            var diff = today.Subtract(addedDate);
            var res = String.Empty;
            if  (diff.Hours == 0)
            {
                res = String.Format("{0} minutes ago.", diff.Minutes);
            }
            else if (diff.Hours/24 == 0)
            {
                res = String.Format("{0} hours ago.", diff.Hours);
            }
            else
            {
                res = String.Format("{0} days ago.", diff.Hours/24);
            }

            return res;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
