using System;
using System.Globalization;
using Xamarin.Forms;

namespace Fourplaces.Model
{
    public class IdToURLConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            int ImageID = (int)value;

            return "https://td-api.julienmialon.com/images/" + ImageID;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}