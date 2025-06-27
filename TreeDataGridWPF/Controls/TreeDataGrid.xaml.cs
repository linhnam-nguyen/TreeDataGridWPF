using System;
using System.Collections.ObjectModel;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using TreeDataGridWPF.Models;

namespace TreeDataGridWPF.Controls
{
    public partial class TreeDataGrid<T> : UserControl
    {

        public TreeDataGrid()
        {
            InitializeComponent();
        }

        public static readonly DependencyProperty FlatRowsProperty =
            DependencyProperty.Register("FlatRows", typeof(ObservableCollection<TreeNode<T>>), typeof(TreeDataGrid<T>), new PropertyMetadata(null));

        public ObservableCollection<object> FlatRows
        {
            get { return (ObservableCollection<object>)GetValue(FlatRowsProperty); }
            set { SetValue(FlatRowsProperty, value); }
        }

        public void Build<T>(Data.TreeListDataSource<T> dataSource, params PropertyInfo[] columns)
        {
            FlatRows = new ObservableCollection<object>(dataSource.FlatList);
            PART_DataGrid.Columns.Clear();

            // First column: Tree/Expander + Name
            var firstColumn = new DataGridTemplateColumn
            {
                Header = columns[0].Name,
                Width = new DataGridLength(1, DataGridLengthUnitType.Auto),
                CellTemplate = BuildExpanderCellTemplate(columns[0])
            };
            PART_DataGrid.Columns.Add(firstColumn);

            // Other columns
            for (int i = 1; i < columns.Length; ++i)
            {
                var col = columns[i];
                var textCol = new DataGridTextColumn
                {
                    Header = col.Name,
                    Binding = new Binding($"Model.{col.Name}") { Mode = col.CanWrite ? BindingMode.TwoWay : BindingMode.OneWay }
                };
                if (!col.CanWrite) textCol.IsReadOnly = true;
                PART_DataGrid.Columns.Add(textCol);
            }
        }

        private DataTemplate BuildExpanderCellTemplate(PropertyInfo prop)
        {
            string xaml =
                "<DataTemplate xmlns='http://schemas.microsoft.com/winfx/2006/xaml/presentation' " +
                             "xmlns:x='http://schemas.microsoft.com/winfx/2006/xaml'>" +
                "  <StackPanel Orientation='Horizontal'>" +
                "    <ToggleButton " +
                "      Margin='{Binding Depth, Converter={StaticResource DepthToIndentConverter}}' " +
                "      IsChecked='{Binding IsExpanded, Mode=TwoWay}' " +
                "      Visibility='{Binding Children.Count, Converter={StaticResource ChildrenToVisibilityConverter}}' " +
                "      Width='14' Height='14' Padding='0' />" +
                "    <TextBlock Text='{Binding Model." + prop.Name + "}' VerticalAlignment='Center' Margin='2,0,0,0' />" +
                "  </StackPanel>" +
                "</DataTemplate>";
            return (DataTemplate)System.Windows.Markup.XamlReader.Parse(xaml);
        }
    }
}
