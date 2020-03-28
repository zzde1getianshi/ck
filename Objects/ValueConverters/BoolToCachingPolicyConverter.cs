using System;
using System.Globalization;
using System.Windows.Data;
using Pixeval.Objects.Caching;

namespace Pixeval.Objects.ValueConverters
{
    public class BoolToCachingPolicyConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value is CachingPolicy.File;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value is true ? CachingPolicy.File : CachingPolicy.Memory;
        }
    }
}