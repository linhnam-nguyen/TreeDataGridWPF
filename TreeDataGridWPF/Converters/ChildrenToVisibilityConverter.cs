using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace TreeDataGridWPF.Converters
{
    public class ChildrenToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            int count = (int)value;
            return count > 0 ? Visibility.Visible : Visibility.Hidden;
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotSupportedException();
    }
}
