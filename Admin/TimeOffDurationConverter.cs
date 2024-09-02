using System;
using System.Globalization;
using System.Windows.Data;

namespace AIS.Admin
{
    public class TimeOffDurationConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is DateTime endDate)
            {
                TimeSpan duration = endDate - ((DateTime)value).Date;
                return duration.Days;
            }

            return 0; 
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException(); 
        }
    }
}