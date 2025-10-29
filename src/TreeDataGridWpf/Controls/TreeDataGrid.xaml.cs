using System;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Media;
using TreeDataGridWPF.Models;

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
            where T : DataModel
        {
            if (dataSource is null) throw new ArgumentNullException(nameof(dataSource));
            if (columns is null || columns.Length == 0) throw new ArgumentException("At least one column must be provided.", nameof(columns));

            // FlatList is ObservableCollection<TreeNode<T>>
            PART_DataGrid.ItemsSource = dataSource.FlatList;
            PART_DataGrid.Columns.Clear();
            var baseCellStyle = TryFindResource("TreeGridCellStyle") as Style;

            // First column: Expander + main property
            var firstCellStyle = new Style(typeof(DataGridCell), baseCellStyle);
            firstCellStyle.Setters.Add(new Setter(FontWeightProperty, FontWeights.SemiBold));
            firstCellStyle.Setters.Add(new Setter(FocusableProperty, false));

            var firstProp = columns[0];
            var firstColumn = new DataGridTemplateColumn
            {
                Header = firstProp.Name,
                Width = new DataGridLength(10, DataGridLengthUnitType.Auto),
                CellTemplate = ExpanderCellTemplate<T>(firstProp),
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

        public void Build<T>(TreeEngine.TreeListDataSource<T> dataSource, params ColumnSpec[] propertySet)
            where T : TreeTableModel
        {
            if (dataSource is null) throw new ArgumentNullException(nameof(dataSource));

            // rows
            PART_DataGrid.ItemsSource = dataSource.FlatList;
            PART_DataGrid.Columns.Clear();

            var baseCellStyle = TryFindResource("TreeGridCellStyle") as Style;

            // ---------------- First column: expander + EntityName ----------------
            var firstCellStyle = new Style(typeof(DataGridCell), baseCellStyle);
            firstCellStyle.Setters.Add(new Setter(FontWeightProperty, FontWeights.SemiBold));
            firstCellStyle.Setters.Add(new Setter(FocusableProperty, false));

            var nameProp = typeof(TreeTableModel).GetProperty(nameof(TreeTableModel.EntityName))
                           ?? throw new InvalidOperationException("Failed to locate EntityName property on TreeTableModel.");
            var firstColumn = new DataGridTemplateColumn
            {
                Header = nameof(TreeTableModel.EntityName),
                Width = new DataGridLength(10, DataGridLengthUnitType.Auto),
                CellTemplate = ExpanderCellTemplate<T>(nameProp),
                CellStyle = firstCellStyle,
                IsReadOnly = true
            };
            PART_DataGrid.Columns.Add(firstColumn);

            // --------------- Other columns: bind to Item.Columns[i].Value -------
            var objectCellStyle = new Style(typeof(DataGridCell), baseCellStyle);
            objectCellStyle.Setters.Add(new Setter(TextElement.FontStyleProperty, FontStyles.Italic));
            objectCellStyle.Setters.Add(new Setter(TextElement.ForegroundProperty, Brushes.Gray));

            // If no schema provided, infer count & headers from first row
            if ((propertySet == null || propertySet.Length == 0) && dataSource.FlatList.FirstOrDefault() is TreeTableModel first)
            {
                propertySet = first.Properties
                                   .Select((c, idx) => new ColumnSpec(c.Header, _ => c.Accessor))
                                   .ToArray();
            }
            propertySet ??= Array.Empty<ColumnSpec>();

            for (int i = 0; i < propertySet.Length; i++)
            {
                var header = propertySet[i].Header ?? $"Col {i + 1}";

                var col = typeof(Column).GetProperty(nameof(Column.Value))
                          ?? throw new InvalidOperationException("Failed to locate Column.Value property.");
                var selector = new TreeDataGrid.TemplateSelector<T>(col, i);
                var dataCol = new DataGridTemplateColumn
                {
                    Header = header,
                    CellTemplateSelector = selector,
                    CellEditingTemplateSelector = selector,
                    CanUserSort = false,
                    // Keep column editable; row-level accessor will decide in setter
                    IsReadOnly = false,
                    CellStyle = objectCellStyle
                };

                // Optional: visually indicate read-only rows (per-cell) via trigger
                //dataCol.CellStyle = dataCol.CellStyle ?? new Style(typeof(DataGridCell), baseCellStyle);
                //dataCol.CellStyle.Setters.Add(new Setter(TextElement.ForegroundProperty, Brushes.Gray));
                //var roTrigger = new DataTrigger
                //{
                //    Binding = new Binding($"Item.Properties[{i}].Accessor.CanWrite"),
                //    Value = false
                //};
                //roTrigger.Setters.Add(new Setter(TextElement.ForegroundProperty, Brushes.DarkGray));
                //roTrigger.Setters.Add(new Setter(TextElement.FontStyleProperty, FontStyles.Italic));
                //dataCol.CellStyle.Triggers.Add(roTrigger);

                PART_DataGrid.Columns.Add(dataCol);
            }
        }
    }
}
