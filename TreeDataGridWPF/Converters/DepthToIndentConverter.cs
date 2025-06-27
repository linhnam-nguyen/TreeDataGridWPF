using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace TreeDataGridWPF.Converters
{
    public class DepthToIndentConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            int depth = (int)value;
            return new Thickness(depth * 16, 0, 0, 0);
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotSupportedException();
    }
}
