using System;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Markup;
using static System.Net.Mime.MediaTypeNames;

namespace TreeDataGridWPF.Controls
{
    public partial class TreeDataGrid
    {
        public sealed class TemplateSelector<T> : DataTemplateSelector
        {
            private readonly PropertyInfo _prop;

            public TemplateSelector(PropertyInfo prop) => _prop = prop;

            public override DataTemplate SelectTemplate(object item, DependencyObject container)
            {
                if (item is TreeDataGridWPF.Models.TreeNode<T> node)
                {
                    var value = _prop.GetValue(node.Model);
                    return TypeTemplate(_prop, value);
                }
                // Fallback: read-only text
                return TypeTemplate(_prop);
            }
        }
    }
}

