using System;
using System.Collections.ObjectModel;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using TreeDataGridWPF.Converters;
using TreeDataGridWPF.Models;

namespace TreeDataGridWPF.Controls
{
    public partial class TreeDataGrid : UserControl
    {
        public TreeDataGrid()
        {
            InitializeComponent();
            // Try to apply the Office/TaskPane style if found
            var officeStyle = TryFindResource("OfficeDataGridStyle") as Style;
            if (officeStyle != null) PART_DataGrid.Style = officeStyle;
            // Set expander style as column template resource
            PART_DataGrid.Resources["ExpanderToggleStyle"] = TryFindResource("ExpanderToggleStyle") as Style;
            PART_DataGrid.ColumnHeaderHeight = 30;
        }

        /// <summary>
        /// Binds the TreeDataGrid to the supplied data source and configures columns.
        /// </summary>
        public void Build<T>(Data.TreeListDataSource<T> dataSource, params PropertyInfo[] columns)
        {
            // FlatList is ObservableCollection<TreeNode<T>>
            PART_DataGrid.ItemsSource = dataSource.FlatList;
            PART_DataGrid.Columns.Clear();

            // First column: Tree Expander + Main Property 
            var cellStyle = new Style(typeof(DataGridCell));
            cellStyle.Setters.Add(new Setter(DataGridCell.FocusableProperty, false));

            var firstColumn = new DataGridTemplateColumn
            {
                Header = "Properties",
                Width = new DataGridLength(10, DataGridLengthUnitType.Auto),
                CellTemplate = BuildExpanderCellTemplate<T>(columns[0]),
                CellStyle = cellStyle,
            };

            PART_DataGrid.Columns.Add(firstColumn);

            // Other columns: Show as text, editable if property has public setter
            for (int i = 1; i < columns.Length; ++i)
            {
                var col = columns[i];
                var textCol = new DataGridTextColumn
                {
                    Header = col.Name,
                    Binding = new Binding($"Model.{col.Name}")
                    {
                        Mode = col.CanWrite ? BindingMode.TwoWay : BindingMode.OneWay
                    },
                    IsReadOnly = !col.CanWrite,
                    CanUserSort = false,
                };
                PART_DataGrid.Columns.Add(textCol);
            }
        }

        /// <summary>
        /// Dynamically creates a DataTemplate for the first (tree/expander) column.
        /// </summary>
        private DataTemplate BuildExpanderCellTemplate<T>(PropertyInfo prop)
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
