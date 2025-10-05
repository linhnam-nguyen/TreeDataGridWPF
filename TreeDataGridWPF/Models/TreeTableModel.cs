using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace TreeDataGridWPF.Models
{
    public class Column : INotifyPropertyChanged
    {
        public string Header { get; }
        public IAccessor Accessor { get; init; }

        public Column(string header, IAccessor accessor)
        {
            Header = header ?? throw new ArgumentNullException(nameof(header));
            Accessor = accessor ?? throw new ArgumentNullException(nameof(accessor));
        }

        public object Value
        {
            get => Accessor?.Get();
            set
            {
                if (Accessor?.CanWrite == true)
                {
                    Accessor.Set(value);
                    OnPropertyChanged(nameof(Value));
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string name) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }

    public class TreeTableModel
    {
        public string EntityName { get; init; }
        public List<Column> Properties { get; init; } = new();
        public ObservableCollection<TreeTableModel> Children { get; init; } = new();


        public static ObservableCollection<TreeTableModel> ParseTreeTableModel(object data, Func<object, System.Collections.IEnumerable> childrenSelector, params ColumnSpec[] columnSpecs)
        {
            var result = new ObservableCollection<TreeTableModel>();
            if (data is System.Collections.IEnumerable seq && data is not string)
            {
                foreach (var item in seq)
                    result.Add(BuildChildren(item, childrenSelector, columnSpecs));
            }
            else
            {
                result.Add(BuildChildren(data, childrenSelector, columnSpecs));
            }
            return result;
        }

        private static TreeTableModel BuildChildren( object obj, Func<object, System.Collections.IEnumerable> childrenSelector, ColumnSpec[] columnSpecs)
        {
            var node = new TreeTableModel
            {
                EntityName = obj?.GetType().Name ?? "<null>",
            };

            // Materialize columns
            foreach (var spec in columnSpecs)
            {
                var acc = spec.CreateAccessor(obj);
                node.Properties.Add(new Column(spec.Header, acc));
            }

            // Recurse
            if (obj != null && childrenSelector != null)
            {
                foreach (var child in childrenSelector(obj) ?? Array.Empty<object>())
                {
                    node.Children.Add(BuildChildren(child, childrenSelector, columnSpecs));
                }
            }
            return node;
        }
    }

    public readonly struct ColumnSpec
    {
        public string Header { get; }
        private readonly Func<object, IAccessor> _factory;

        public ColumnSpec(string header, Func<object, IAccessor> factory)
        {
            Header = header ?? throw new ArgumentNullException(nameof(header));
            _factory = factory ?? throw new ArgumentNullException(nameof(factory));
        }

        public IAccessor CreateAccessor(object owner) => _factory(owner);

        // ---------- Helpers ----------

        public static ColumnSpec ForProperty(string header, string propertyName = null)
            => new(header, owner =>
            {
                var name = propertyName ?? header;
                var pi = owner?.GetType().GetProperty(name, System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic);
                if (pi == null)
                    throw new InvalidOperationException(
                        $"Property '{name}' not found on {owner?.GetType().Name ?? "<null>"}.");
                return new PropertyAccessor(owner, pi);
            });

        public static ColumnSpec ForField(string header, string fieldName = null)
            => new(header, owner =>
            {
                var name = fieldName ?? header;
                var fi = owner?.GetType().GetField(name, System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic);
                if (fi == null)
                    throw new InvalidOperationException(
                        $"Field '{name}' not found on {owner?.GetType().Name ?? "<null>"}.");
                return new FieldAccessor(owner, fi);
            });

        public static ColumnSpec ForLambda(string header, Func<object, object> getter, Action<object, object> setter = null)
            => new(header, owner => new LambdaAccessor(owner, getter, setter));
    }
}
