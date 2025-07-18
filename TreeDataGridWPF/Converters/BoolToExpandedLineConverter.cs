﻿using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace TreeDataGridWPF.Converters
{
    public class BoolToExpandedLineConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
            => (value as bool? == true) ? Visibility.Visible : Visibility.Collapsed;

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotSupportedException();
    }
}
