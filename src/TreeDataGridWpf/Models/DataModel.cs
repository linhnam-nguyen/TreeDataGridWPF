using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;

namespace TreeDataGridWPF.Models
{
    public class DataModel : INotifyPropertyChanged
    {
        public string Name { get; init; } = string.Empty;
        public IAccessor Accessor { get; init; } = default!;
        public object? Value
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

        public ObservableCollection<DataModel> Children { get; private set; } = new();

        // ------------ Parse (no copy) ------------

        public static ObservableCollection<DataModel> ParseDataModel(object? data)
        {
            if (data is ObservableCollection<DataModel> already)
                return already;

            var visited = new HashSet<object>(System.Collections.Generic.ReferenceEqualityComparer.Instance);
            var root = ForConstant(MakeRootName(data), data);
            root.Children = BuildChildren(data, visited);
            return new ObservableCollection<DataModel> { root };
        }

        private static DataModel ForConstant(string name, object? value) =>
            new DataModel { Name = name, Accessor = new ConstantAccessor(value) };

        private static ObservableCollection<DataModel> BuildChildren(object? obj, HashSet<object> visited)
        {
            var result = new ObservableCollection<DataModel>();
            if (obj == null) return result;

            var t = obj.GetType();
            if (!IsValueTypeOrString(t))
            {
                if (!visited.Add(obj)) return result; // cycle guard
            }

            // IDictionary
            if (obj is IDictionary dict)
            {
                var valueType = TryGetDictionaryValueType(dict.GetType());
                foreach (DictionaryEntry e in dict)
                {
                    var name = $"[{SafeKey(e.Key)}]";
                    var dm = new DataModel
                    {
                        Name = name,
                        Accessor = new DictionaryEntryAccessor(dict, e.Key, valueType)
                    };
                    AttachChildrenIfComplex(dm, visited);
                    result.Add(dm);
                }
                return result;
            }

            // IList (settable items)
            if (obj is IList list)
            {
                var itemType = TryGetListItemType(list.GetType());
                for (int i = 0; i < list.Count; i++)
                {
                    var dm = new DataModel
                    {
                        Name = $"[{i}]",
                        Accessor = new ListItemAccessor(list, i, itemType)
                    };
                    AttachChildrenIfComplex(dm, visited);
                    result.Add(dm);
                }
                return result;
            }

            // IEnumerable (read-only sequence) — materialize as constants
            if (obj is IEnumerable seq && obj is not string)
            {
                int j = 0;
                foreach (var item in seq)
                {
                    var dm = ForConstant($"[{j}]", item);
                    AttachChildrenIfComplex(dm, visited);
                    result.Add(dm);
                    j++;
                }
                return result;
            }

            // Plain object: properties (and public fields)
            foreach (var p in t.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                               .Where(p => p.CanRead && p.GetIndexParameters().Length == 0))
            {
                DataModel dm;
                try
                {
                    dm = new DataModel
                    {
                        Name = p.Name,
                        Accessor = new PropertyAccessor(obj, p)
                    };
                }
                catch
                {
                    dm = ForConstant(p.Name, "<unreadable>");
                }
                AttachChildrenIfComplex(dm, visited);
                result.Add(dm);
            }

            foreach (var f in t.GetFields(BindingFlags.Public | BindingFlags.Instance))
            {
                var dm = new DataModel
                {
                    Name = f.Name,
                    Accessor = new FieldAccessor(obj, f)
                };
                AttachChildrenIfComplex(dm, visited);
                result.Add(dm);
            }

            return result;
        }

        private static void AttachChildrenIfComplex(DataModel node, HashSet<object> visited)
        {
            var val = node.Value;
            if (!IsLeaf(val)) node.Children = BuildChildren(val, visited);
            else node.Children = new ObservableCollection<DataModel>();
        }

        // ------------ Helpers ------------

        public static bool IsLeaf(object? obj)
        {
            if (obj == null) return true;
            var t = obj.GetType();
            if (t.IsEnum || IsSimple(t)) return true;

            var nt = Nullable.GetUnderlyingType(t);
            return nt != null && IsSimple(nt);
        }

        public static bool IsSimple(Type t) =>
            t.IsPrimitive ||
            t == typeof(string) ||
            t == typeof(decimal) ||
            t == typeof(DateTime) ||
#if NET6_0_OR_GREATER
            t == typeof(DateOnly) || t == typeof(TimeOnly) ||
#endif
            t == typeof(TimeSpan) ||
            t == typeof(Guid);

        private static bool IsValueTypeOrString(Type t) => t.IsValueType || t == typeof(string);
        
        private static string SafeKey(object? key) => key?.ToString() ?? "null";

        private static string MakeRootName(object? data)
        {
            if (data == null) return "Root (null)";
            if (data is IEnumerable && data is not string) return data.GetType().Name;
            return data.GetType().Name;
        }

        private static Type? TryGetListItemType(Type listType) =>
            listType.IsGenericType ? listType.GetGenericArguments().FirstOrDefault() : null;

        private static Type? TryGetDictionaryValueType(Type dictType) =>
            dictType.IsGenericType && dictType.GetGenericArguments().Length == 2
                ? dictType.GetGenericArguments()[1]
                : null;

        // constant (read-only) nodes
        private sealed class ConstantAccessor : IAccessor
        {
            private readonly object? _value;
            public ConstantAccessor(object? value) { _value = value; }
            public object? Get() => _value;
            public void Set(object? value) { /* no-op */ }
            public bool CanWrite => false;
            public object? Owner => null;
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        private void OnPropertyChanged(string name) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}

