using System;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using static System.Net.Mime.MediaTypeNames;

namespace TreeDataGridWPF.Controls
{
    public partial class TreeDataGrid
    {


        /// <summary>
        /// Dynamically creates a DataTemplate for the first (tree/expander) column.
        /// </summary>


        public DataTemplate TypeTemplate<T>(PropertyInfo prop)
        {
            object value = prop.GetValue(default(T), null);
            string xaml =
                "<DataTemplate xmlns='http://schemas.microsoft.com/winfx/2006/xaml/presentation' " +
                             "xmlns:x='http://schemas.microsoft.com/winfx/2006/xaml'>" +
                TypeTemplate<T>(value as dynamic, prop.Name) +
                "</DataTemplate>";
            return (DataTemplate)System.Windows.Markup.XamlReader.Parse(xaml);
        }

        public string TypeTemplate<T>(string value, string name)
        {
            return $"  <TextBox Text='{{Binding Model.{name}, Mode=TwoWay, UpdateSourceTrigger=PropertyChanged}}' />";
        }

        public string TypeTemplate<T>(int value, string name)
        {
            return $"  <TextBox Text='{{Binding Model.{name}, Mode=TwoWay, UpdateSourceTrigger=PropertyChanged}}' InputScope='Number' />";
        }

        public string TypeTemplate<T>(double value, string name)
        {
            return $"  <TextBox Text='{{Binding Model.{name}, Mode=TwoWay, UpdateSourceTrigger=PropertyChanged}}' InputScope='Number' />";
        }

        public string TypeTemplate<T>(bool value, string name)
        {
            return $"  <CheckBox IsChecked='{{Binding Model.{name}, Mode=TwoWay, UpdateSourceTrigger=PropertyChanged}}' />";
        }

        public string TypeTemplate<T>(DateTime value, string name)
        {
            return $"  <DatePicker SelectedDate='{{Binding Model.{name}, Mode=TwoWay, UpdateSourceTrigger=PropertyChanged}}' />";
        }

        public string TypeTemplate<T>(Enum value, string name)
        {
            string xaml =
                $"  <ComboBox ItemsSource='{{Binding Source={{x:Static local:Enum.GetValues({typeof(T).FullName})}}}}' " +
                $"            SelectedItem='{{Binding Model.{name}, Mode=TwoWay, UpdateSourceTrigger=PropertyChanged}}' />";
            return xaml;
        }

        public string TypeTemplate<T>(object value, string name)
        {
            return $"  <TextBlock Text='{{Binding Model.{name}, Mode=OneWay}}' />";
        }
    }
}
