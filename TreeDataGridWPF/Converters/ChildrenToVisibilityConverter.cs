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
            bool isTrue = (bool)value;
            return isTrue ? Visibility.Visible : Visibility.Hidden;
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotSupportedException();
    }
}
