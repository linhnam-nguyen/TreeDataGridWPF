using System;
using System.Collections;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;
using TreeDataGridWPF.Models;

namespace TreeDataGridWPF.Converters
{
    public class DisplayNameConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null) return "<null>";
            if (DataModel.IsLeaf(value))
                return value;
            if (value is ICollection v)
                return $"{GetFriendlyTypeName(value.GetType())}[{v.Count}]";

            return GetFriendlyTypeName(value.GetType());
        }

        public object ConvertBack(object value, Type t, object p, System.Globalization.CultureInfo c) => value;

        private static string GetFriendlyTypeName(Type t)
        {
            if (!t.IsGenericType) return t.Name;
            var mainName = t.Name.Substring(0, t.Name.IndexOf('`'));
            return mainName;
        }

    }
}
