using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Windows;

namespace TreeDataGridWPF.Controls
{

    public partial class TreeDataGrid
    {
        /// <summary>
        /// Dynamically creates a DataTemplate for the first (tree/expander) column.
        /// </summary>
        public static DataTemplate TypeTemplate(PropertyInfo prop, object value = null)
        {
            string templateString;
            string xaml;
            if (value is string s && Regex.IsMatch(s, @"^\s*<[^>]+>\s*$"))
            {
                xaml = TemplateBuilder(TypeTemplate(prop.Name));
                return (DataTemplate)System.Windows.Markup.XamlReader.Parse(xaml);
            }

            var type = value?.GetType() ?? prop.PropertyType;
            var key = (prop, type);
            if (_cache.TryGetValue(key, out var dt)) return dt;

            if (type.IsEnum)
            {
                dt = EnumTemplate(prop, value);
                _cache[key] = dt;
                return dt;
            }

            templateString = value is not null
                ? TypeTemplate(value as dynamic, prop.Name)
                : DefaultTemplate(type, prop.Name);

            xaml = TemplateBuilder(templateString);

            dt = (DataTemplate)System.Windows.Markup.XamlReader.Parse(xaml);
            _cache[key] = dt;
            return dt;
        }

        public static string TypeTemplate(string value, string name)
            => $"  <TextBox Text='{{Binding Model.{name}, Mode=TwoWay, UpdateSourceTrigger=PropertyChanged}}' />";

        public static string TypeTemplate(int value, string name)
            => $"  <TextBox Text='{{Binding Model.{name}, Mode=TwoWay, UpdateSourceTrigger=PropertyChanged}}' />";

        public static string TypeTemplate(double value, string name)
            => $"  <TextBox Text='{{Binding Model.{name}, Mode=TwoWay, UpdateSourceTrigger=PropertyChanged}}' />";

        public static string TypeTemplate(bool value, string name)
            => $"  <CheckBox IsChecked='{{Binding Model.{name}, Mode=TwoWay, UpdateSourceTrigger=PropertyChanged}}' />";

        public static string TypeTemplate(DateTime value, string name)
            => $"  <DatePicker SelectedDate='{{Binding Model.{name}, Mode=TwoWay, UpdateSourceTrigger=PropertyChanged}}' />";

        public static string TypeTemplate(string name) // default read-only
            => $"  <TextBlock Text='{{Binding Model.{name}, Mode=OneWay}}' />";


        // Route nulls by PropertyType so editors are still editable for empty values
        public static string DefaultTemplate(Type type, string name)
        {
            if (type == typeof(string)) return TypeTemplate(default(string), name);
            if (type == typeof(bool) || type == typeof(bool?))
                return TypeTemplate(default(bool), name);

            if (type == typeof(DateTime) || type == typeof(DateTime?))
                return TypeTemplate(default(DateTime), name);

            if (type == typeof(int) || type == typeof(int?) ||
                type == typeof(double) || type == typeof(double?) ||
                type == typeof(float) || type == typeof(float?) ||
                type == typeof(decimal) || type == typeof(decimal?) ||
                type == typeof(long) || type == typeof(long?) ||
                type == typeof(short) || type == typeof(short?))
                return TypeTemplate(default(double), name); // any numeric overload (they return TextBox)

            return TypeTemplate(name);
        }

        private static readonly Dictionary<(PropertyInfo prop, Type type), DataTemplate> _cache = new();

        private static string TemplateBuilder(string templateString)
            => "<DataTemplate xmlns='http://schemas.microsoft.com/winfx/2006/xaml/presentation' " +
            "              xmlns:x='http://schemas.microsoft.com/winfx/2006/xaml'>" +
                templateString +
            "</DataTemplate>";
    }
}
