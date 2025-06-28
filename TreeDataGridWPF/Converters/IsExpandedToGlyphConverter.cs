using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace TreeDataGridWPF.Converters
{
    public class IsExpandedToGlyphConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool isExpanded = value as bool? ?? false;
            return isExpanded ? "−" : "+";
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Binding.DoNothing;
        }
    }
}
