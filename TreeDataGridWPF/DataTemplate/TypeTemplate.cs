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
            return $"  < TextBox Text = {{Binding Model.{Name}, Mode=TwoWay, UpdateSourceTrigger=PropertyChanged}} />";
        }

        public string TypeTemplate<T>(int value)
        {
            return $"  <TextBox Text = {{Binding Model.{Name}, Mode=TwoWay, UpdateSourceTrigger=PropertyChanged}} InputScope='Number' />";
        }

        public string TypeTemplate<T>(double value)
        {
            return "  <TextBox Text='{Binding Value, Mode=TwoWay, UpdateSourceTrigger=PropertyChanged}' InputScope='Number' />";
        }


        public string TypeTemplate<T>(bool value)
        {
            return "  <CheckBox IsChecked='{Binding Value, Mode=TwoWay, UpdateSourceTrigger=PropertyChanged}' />";
        }

        public string TypeTemplate<T>(DateTime value)
        { 
            return "  <DatePicker SelectedDate='{Binding Value, Mode=TwoWay, UpdateSourceTrigger=PropertyChanged}' />";
        }

        public string TypeTemplate<T>(Enum value)
        {
            string xaml =
                "  <ComboBox ItemsSource='{Binding Source={x:Static local:Enum.GetValues(" + typeof(T).FullName + ")}}' " +
                "            SelectedItem='{Binding Value, Mode=TwoWay, UpdateSourceTrigger=PropertyChanged}' />";
            return xaml;
        }
        public string TypeTemplate<T>(object value)
        { 
            return "  <TextBlock Text='{Binding Value, Mode=OneWay}' />";
        }
    }
}
