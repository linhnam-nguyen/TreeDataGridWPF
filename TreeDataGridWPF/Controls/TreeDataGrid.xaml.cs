using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;

namespace TreeDataGridWPF.Controls
{
    public partial class TreeDataGrid : UserControl
    {
        public TreeDataGrid()
        {
            InitializeComponent();
            PART_DataGrid.SetResourceReference(StyleProperty, "DefaultDataGridStyle");
            PART_DataGrid.Resources["ExpanderToggleStyle"] = TryFindResource("ExpanderToggleStyle") as Style;

            //PART_DataGrid.SetResourceReference(FrameworkElement.StyleProperty, "Unused"); // no-op, just example
            PART_DataGrid.ColumnHeaderHeight = 25;
        }

        /// <summary>
        /// Binds the TreeDataGrid to the supplied data source and configures columns.
        /// </summary>
        public void Build<T>(TreeEngine.TreeListDataSource<T> dataSource, params PropertyInfo[] columns)
        {
            // FlatList is ObservableCollection<TreeNode<T>>
            PART_DataGrid.ItemsSource = dataSource.FlatList;
            PART_DataGrid.Columns.Clear();
            var baseCellStyle = TryFindResource("TreeGridCellStyle") as Style;

            // First column: Expander + main property
            var firstCellStyle = new Style(typeof(DataGridCell), baseCellStyle);
            firstCellStyle.Setters.Add(new Setter(FontWeightProperty, FontWeights.SemiBold));
            firstCellStyle.Setters.Add(new Setter(FocusableProperty, false));

            var firstColumn = new DataGridTemplateColumn
            {
                Header = columns[0].Name,
                Width = new DataGridLength(10, DataGridLengthUnitType.Auto),
                CellTemplate = ExpanderCellTemplate<T>(columns[0]),
                CellStyle = firstCellStyle,
            };

            PART_DataGrid.Columns.Add(firstColumn);

            // Other columns: Shown based on value type, editable if property has public setter
            var objectCellStyle = new Style(typeof(DataGridCell), baseCellStyle);
            objectCellStyle.Setters.Add(new Setter(FontStyleProperty, FontStyles.Italic));
            objectCellStyle.Setters.Add(new Setter(TextElement.ForegroundProperty, Brushes.Gray));
            for (int i = 1; i < columns.Length; ++i)
            {
                var col = columns[i];
                var selector = new TreeDataGrid.TemplateSelector<T>(col);
                var dataCol = new DataGridTemplateColumn
                {
                    Header = col.Name,
                    CellTemplateSelector = selector,
                    CellEditingTemplateSelector = selector,
                    CanUserSort = false,
                    IsReadOnly = !(col.CanWrite && col.SetMethod?.IsPublic == true),
                    CellStyle = objectCellStyle,
                };
                PART_DataGrid.Columns.Add(dataCol);
            }
        }
    }
}
