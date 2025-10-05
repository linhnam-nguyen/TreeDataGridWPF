using System;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using TreeDataGridWPF.Models;

namespace TreeDataGridWPF.Controls
{
    public partial class TreeDataGrid
    {
        public sealed class TemplateSelector<T> : DataTemplateSelector
        {
            private readonly PropertyInfo _prop;
            private readonly int? _propertiesIndex;   // which Properties[i] to show
            private readonly string _propertiesHeader;// or lookup by header (optional)

            public TemplateSelector(PropertyInfo prop, int? propertiesIndex = null, string propertiesHeader = null)
            {
                _prop = prop ?? throw new ArgumentNullException(nameof(prop));
                _propertiesIndex = propertiesIndex;
                _propertiesHeader = propertiesHeader;
            }

            public override DataTemplate SelectTemplate(object item, DependencyObject container)
            {
                if (item is TreeNode<T> node)
                {
                    object value;

                    var isColumnValueProp = _prop.DeclaringType == typeof(Column) && _prop.Name == nameof(Column.Value);

                    if (typeof(TreeTableModel).IsAssignableFrom(typeof(T)) && node.Model is TreeTableModel row && isColumnValueProp)
                    {
                        Column col = null;
                        if (_propertiesIndex.HasValue && _propertiesIndex.Value >= 0 && _propertiesIndex.Value < row.Properties.Count)
                            col = row.Properties[_propertiesIndex.Value];
                        else if (!string.IsNullOrEmpty(_propertiesHeader))
                            col = row.Properties.FirstOrDefault(c => c.Header == _propertiesHeader);
                        if (col != null)
                        {
                            value = col.Value;
                            return TypeTemplate(_prop, _propertiesIndex.Value, value);
                        }
                    }

                    value = _prop.GetValue(node.Model);
                    return TypeTemplate(_prop, value);
                }
                // Fallback: read-only text
                return TypeTemplate(_prop);
            }
        }
    }
}

