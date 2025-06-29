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
        }

        /// <summary>
        /// Binds the TreeDataGrid to the supplied data source and configures columns.
        /// </summary>
        public void Build<T>(Data.TreeListDataSource<T> dataSource, params PropertyInfo[] columns)
        {
            // FlatList is ObservableCollection<TreeNode<T>>
            PART_DataGrid.ItemsSource = dataSource.FlatList;
            PART_DataGrid.Columns.Clear();

            // --- First column: Tree Expander + Main Property (e.g., Name) ---
            var cellStyle = new Style(typeof(DataGridCell));
            cellStyle.Setters.Add(new Setter(DataGridCell.FocusableProperty, false));

            var firstColumn = new DataGridTemplateColumn
            {
                Header = "Properties",
                Width = new DataGridLength(10, DataGridLengthUnitType.Auto),
                CellTemplate = BuildExpanderCellTemplate<T>(columns[0]),
                CellStyle = cellStyle
            };

            PART_DataGrid.Columns.Add(firstColumn);

            // --- Other columns: Show as text, editable if property has public setter ---
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
                    IsReadOnly = !col.CanWrite
                };
                PART_DataGrid.Columns.Add(textCol);
            }
        }

        private DataTemplate BuildExpanderCellTemplate2<T>(PropertyInfo prop)
        {
            // Create a DataTemplate in C#
            var template = new DataTemplate(typeof(TreeNode<T>));

            // Create StackPanel
            var stackPanelFactory = new FrameworkElementFactory(typeof(StackPanel));
            stackPanelFactory.SetValue(StackPanel.OrientationProperty, Orientation.Horizontal);

            // Create ToggleButton
            var toggleFactory = new FrameworkElementFactory(typeof(ToggleButton));
            toggleFactory.SetValue(ToggleButton.WidthProperty, 12.0);
            toggleFactory.SetValue(ToggleButton.HeightProperty, 12.0);
            toggleFactory.SetValue(ToggleButton.PaddingProperty, new Thickness(-8));

            // Bind Margin to Depth
            var marginBinding = new Binding("Depth") { Converter = new DepthToIndentConverter() };
            toggleFactory.SetBinding(ToggleButton.MarginProperty, marginBinding);

            // Bind IsChecked to IsExpanded (two-way)
            var isCheckedBinding = new Binding("IsExpanded") { Mode = BindingMode.TwoWay, UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged };
            toggleFactory.SetBinding(ToggleButton.IsCheckedProperty, isCheckedBinding);

            // Set ClickMode to Press (optional, for immediate response)
            toggleFactory.SetValue(ToggleButton.ClickModeProperty, ClickMode.Press);

            // Make ToggleButton focusable and bind its content to IsExpanded
            toggleFactory.SetValue(ToggleButton.FocusableProperty, true);

            // Bind Content to IsExpanded using a converter, uodate source trigger = PropertyChanged
            var contentBinding = new Binding("IsExpanded") { Converter = new IsExpandedToGlyphConverter()};
            toggleFactory.SetBinding(ToggleButton.ContentProperty, contentBinding);

            // Optional: Bind Visibility if you want, or leave always visible for debugging
            var visibilityBinding = new Binding("HasDummyChild") { Converter = new ChildrenToVisibilityConverter() };
            toggleFactory.SetBinding(ToggleButton.VisibilityProperty, visibilityBinding);

            // For debugging: give the button a color to make it obvious
            toggleFactory.SetValue(ToggleButton.BackgroundProperty, Brushes.LightBlue);
            //toggleFactory.AddHandler( UIElement.PreviewMouseLeftButtonDownEvent, new MouseButtonEventHandler((s, e) => { e.Handled = true; }));

            // Create TextBlock for the main property
            var textFactory = new FrameworkElementFactory(typeof(TextBlock));
            textFactory.SetBinding(TextBlock.TextProperty, new Binding($"Model.{prop.Name}"));
            textFactory.SetValue(TextBlock.VerticalAlignmentProperty, VerticalAlignment.Center);
            textFactory.SetValue(TextBlock.MarginProperty, new Thickness(5, 0, 0, 0));

            // Add children to StackPanel
            stackPanelFactory.AppendChild(toggleFactory);
            stackPanelFactory.AppendChild(textFactory);

            // Set visual tree of the template
            template.VisualTree = stackPanelFactory;
            return template;
        }

        /// <summary>
        /// Dynamically creates a DataTemplate for the first (tree/expander) column.
        /// </summary>
        private DataTemplate BuildExpanderCellTemplate<T>(PropertyInfo prop)
        {
            // For simplicity and to avoid converter issues, start with a basic ToggleButton.
            // Later, you can add margin/converters via code, not XAML.
            string xaml =
                "<DataTemplate xmlns='http://schemas.microsoft.com/winfx/2006/xaml/presentation' " +
                             "xmlns:x='http://schemas.microsoft.com/winfx/2006/xaml'>" +
                "  <StackPanel Orientation='Horizontal'>" +
                "    <ToggleButton " +
                "      Margin='{Binding Depth, Converter={StaticResource DepthToIndentConverter}}' " + // Enable this if your converter works
                "      IsChecked='{Binding IsExpanded, Mode=TwoWay, UpdateSourceTrigger=PropertyChanged}' " +
                "      ClickMode = 'Press'" +
                "      Focusable='True' " +
                "      Content='{Binding IsExpanded, Converter={StaticResource IsExpandedToGlyphConverter}}' " +
                "      Visibility='{Binding HasDummyChild, Converter={StaticResource ChildrenToVisibilityConverter}, UpdateSourceTrigger=PropertyChanged}' " +
                "      Width='14' Height='14' Padding='-8' />" +
                "    <TextBlock Text='{Binding Model." + prop.Name + "}' VerticalAlignment='Center' Margin='5,0,0,0' />" +
                "  </StackPanel>" +
                "</DataTemplate>";

             return (DataTemplate)System.Windows.Markup.XamlReader.Parse(xaml);
        }
    }
}
