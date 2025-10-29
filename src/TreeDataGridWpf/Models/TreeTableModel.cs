using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Reflection;
using System.Linq.Expressions;

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

        private static TreeTableModel BuildChildren( object obj, Func<object, System.Collections.IEnumerable> childrenSelector, ColumnSpec[] columnSpecs, HashSet<object>? visited = null)
        {
            visited ??= new HashSet<object>(ReferenceEqualityComparer.Instance);

            if (obj != null && !visited.Add(obj))
                return new TreeTableModel { EntityName = obj.GetType().Name + "(loop)"};

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
                    node.Children.Add(BuildChildren(child, childrenSelector, columnSpecs,visited));
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
                if (owner is null) throw new ArgumentNullException(nameof(owner));
                var t = owner.GetType();
                var name = propertyName ?? header;
                var key = (t, name);

                if (!_getPCache.TryGetValue(key, out var getter))
                {
                    var pi = t.GetProperty(name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                             ?? throw new InvalidOperationException($"Property '{name}' not found on {t.Name}.");

                    // build Expression<Func<object, object?>>
                    var pObj = Expression.Parameter(typeof(object), "o");
                    var cast = Expression.Convert(pObj, t);
                    var read = Expression.Property(cast, pi);
                    var box = Expression.Convert(read, typeof(object));
                    getter = Expression.Lambda<Func<object, object?>>(box, pObj).Compile();
                    _getPCache[key] = getter;

                    if (pi.CanWrite)
                    {
                        var pVal = Expression.Parameter(typeof(object), "v");
                        var unbox = Expression.Convert(pVal, pi.PropertyType);
                        var set = Expression.Call(cast, pi.SetMethod!, unbox);
                        var setter = Expression.Lambda<Action<object, object?>>(set, pObj, pVal).Compile();
                        _setPCache[key] = setter;
                    }
                }

                _setPCache.TryGetValue(key, out var setDel);
                return new LambdaAccessor(owner, getter, setDel);
            });

        public static ColumnSpec ForField(string header, string fieldName = null)
            => new(header, owner =>
            {

                if (owner is null) throw new ArgumentNullException(nameof(owner));
                var t = owner.GetType();
                var name = fieldName ?? header;
                var key = (t, name);

                var getter = _getFCache.GetOrAdd(key, _ =>
                {
                    var fi = t.GetField(name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                             ?? throw new InvalidOperationException($"Field '{name}' not found on {t.Name}.");

                    // (object o) => (object?)((T)o).Field
                    var pObj = Expression.Parameter(typeof(object), "o");
                    var cast = Expression.Convert(pObj, t);
                    var read = Expression.Field(cast, fi);
                    var box = Expression.Convert(read, typeof(object));
                    return Expression.Lambda<Func<object, object?>>(box, pObj).Compile();
                });

                // compile setter only if field is writable and owner is ref type
                if (!_setFCache.TryGetValue(key, out var setter))
                {
                    var fi = t.GetField(name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)!;

                    var canWrite = !(fi.IsInitOnly || fi.IsLiteral) && !t.IsValueType; // avoid boxed struct issue
                    if (canWrite)
                    {
                        var pObj = Expression.Parameter(typeof(object), "o");
                        var pVal = Expression.Parameter(typeof(object), "v");
                        var cast = Expression.Convert(pObj, t);
                        var val = Expression.Convert(pVal, fi.FieldType);
                        var assign = Expression.Assign(Expression.Field(cast, fi), val);
                        setter = Expression.Lambda<Action<object, object?>>(assign, pObj, pVal).Compile();
                        _setFCache[key] = setter;
                    }
                }

                return new LambdaAccessor(owner, getter, setter); // setter may be null => read-only
            });

        public static ColumnSpec ForLambda(string header, Func<object, object> getter, Action<object, object> setter = null)
            => new(header, owner => new LambdaAccessor(owner, getter, setter));

        static readonly System.Collections.Concurrent.ConcurrentDictionary<(Type, string), Func<object, object?>> _getPCache = new();
        static readonly System.Collections.Concurrent.ConcurrentDictionary<(Type, string), Action<object, object?>> _setPCache = new();
        static readonly System.Collections.Concurrent.ConcurrentDictionary<(Type, string), Func<object, object?>> _getFCache = new();
        static readonly System.Collections.Concurrent.ConcurrentDictionary<(Type, string), Action<object, object?>> _setFCache = new();
    }

}
