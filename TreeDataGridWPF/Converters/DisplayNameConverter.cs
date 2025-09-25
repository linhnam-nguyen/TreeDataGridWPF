using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using TreeDataGridWPF.Models;

namespace TreeDataGridWPF.Converters
{
    public class DisplayNameConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null) return "<null>";
            // You can re-use TypeStringFormatter here:
            return DataModel.IsLeaf(value) ? value : value.GetType().Name;
        }

        public object ConvertBack(object value, Type t, object p, System.Globalization.CultureInfo c) => value;
    }
}
