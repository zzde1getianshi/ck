using System;
using System.Globalization;
using System.Windows.Data;
using Pixeval.Data.ViewModel;

namespace Pixeval.Objects.ValueConverters
{
    [ValueConversion(typeof(TrendType), typeof(string))]
    public class TrendsStatConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is TrendType obj)
            {
                return obj switch
                {
                    TrendType.AddIllust => StringResources.TrendsAddIllust,
                    TrendType.AddBookmark => StringResources.TrendsAddBookmark,
                    TrendType.AddFavorite => StringResources.TrendsAddFavorite,
                    _                               => string.Empty
                };
            }

            return string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}