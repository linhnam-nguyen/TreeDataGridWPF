using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace TreeDataGridWPF.Controls
{
    public partial class TreeDataGrid
    {
        /// <summary>
        /// Dynamically creates a DataTemplate for the first (tree/expander) column.
        /// </summary>
        public DataTemplate ExpanderCellTemplate<T>(PropertyInfo prop)
        {
            string xaml =
                "<DataTemplate xmlns='http://schemas.microsoft.com/winfx/2006/xaml/presentation' " +
                             "xmlns:x='http://schemas.microsoft.com/winfx/2006/xaml'>" +
                "  <StackPanel Orientation='Horizontal'>" +
                "    <ToggleButton " +
                "      Style='{StaticResource ExpanderToggleStyle}' " +
                "      Margin='{Binding Depth, Converter={StaticResource DepthToIndentConverter}}' " + // Enable this if your converter works
                "      IsChecked='{Binding IsExpanded, Mode=TwoWay, UpdateSourceTrigger=PropertyChanged}' " +
                "      ClickMode = 'Press'" +
                "      Focusable='True' " +
                "      Visibility='{Binding HasDummyChild, Converter={StaticResource ChildrenToVisibilityConverter}, UpdateSourceTrigger=PropertyChanged}' " +
                "      /> " +
                "    <TextBlock Text='{Binding Model." + prop.Name + "}' VerticalAlignment='Center' Margin='5,0,0,0' />" +
                "  </StackPanel>" +
                "</DataTemplate>";

             return (DataTemplate)System.Windows.Markup.XamlReader.Parse(xaml);
        }
    }
}
