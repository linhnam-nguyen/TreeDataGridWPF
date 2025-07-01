using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

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
                CellTemplate = ExpanderCellTemplate<T>(columns[0]),
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
    }
}
