using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace TreeDataGridWPF.Models
{
    public class DataModel
    {
        public string Name { get; set; }
        public object Value { get; set; }
        public ObservableCollection<DataModel> Children { get; set; }

        /// <summary>
        /// Entry point: parse any object into a tree of DataModel.
        /// - If data is an ObservableCollection<DataModel>, it is returned as-is.
        /// - Otherwise, the returned collection contains a single root node named from the type or "Root".
        /// </summary>
        public static ObservableCollection<DataModel> ParseData(object data)
        {
            if (data is ObservableCollection<DataModel> already)
                return already;

            var visited = new HashSet<object>(ReferenceEqualityComparer.Instance);
            var rootName = MakeRootName(data);
            var root = BuildNode(rootName, data, visited);
            return new ObservableCollection<DataModel> { root };
        }

        // ---------- Internals ----------

        private static DataModel BuildNode(string name, object obj, HashSet<object> visited)
        {
            // Leaf or null => no children
            if (IsLeaf(obj))
            {
                return new DataModel
                {
                    Name = name,
                    Value = obj,
                    Children = new ObservableCollection<DataModel>()
                };
            }

            // Cycle guard (reference types only)
            if (obj != null && !IsValueTypeOrString(obj.GetType()))
            {
                if (!visited.Add(obj))
                {
                    return new DataModel
                    {
                        Name = name,
                        Value = $"<circular reference: {obj.GetType().Name}>",
                        Children = new ObservableCollection<DataModel>()
                    };
                }
            }

            // Expand children
            var children = BuildChildren(obj, visited);

            return new DataModel
            {
                Name = name,
                Value = GetDisplayValueOrType(obj),
                Children = children
            };
        }

        private static ObservableCollection<DataModel> BuildChildren(object obj, HashSet<object> visited)
        {
            var result = new ObservableCollection<DataModel>();
            if (obj == null) return result;

            // IDictionary (non-generic or generic)
            if (obj is IDictionary dict)
            {
                foreach (DictionaryEntry entry in dict)
                {
                    var childName = $"[{SafeKey(entry.Key)}]";
                    result.Add(BuildNode(childName, entry.Value, visited));
                }
                return result;
            }

            // IEnumerable<KeyValuePair<,>> (e.g., Dictionary<,> surfaced as IEnumerable)
            var kvpIface = obj.GetType().GetInterfaces()
                .FirstOrDefault(t => t.IsGenericType &&
                                     t.GetGenericTypeDefinition() == typeof(IEnumerable<>) &&
                                     t.GetGenericArguments()[0].IsGenericType &&
                                     t.GetGenericArguments()[0].GetGenericTypeDefinition() == typeof(KeyValuePair<,>));
            if (kvpIface != null)
            {
                foreach (var kv in (IEnumerable)obj)
                {
                    var keyProp = kv.GetType().GetProperty("Key");
                    var valProp = kv.GetType().GetProperty("Value");
                    var key = keyProp?.GetValue(kv, null);
                    var val = valProp?.GetValue(kv, null);
                    var childName = $"[{SafeKey(key)}]";
                    result.Add(BuildNode(childName, val, visited));
                }
                return result;
            }

            // IEnumerable (but not string)
            if (obj is IEnumerable seq && obj is not string)
            {
                int i = 0;
                foreach (var item in seq)
                {
                    var childName = $"[{i}]";
                    result.Add(BuildNode(childName, item, visited));
                    i++;
                }
                return result;
            }

            // Plain object: expand readable public instance properties (skip indexers)
            var props = obj.GetType()
                           .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                           .Where(p => p.CanRead && p.GetIndexParameters().Length == 0);

            foreach (var p in props)
            {
                object value;
                try
                {
                    value = p.GetValue(obj, null);
                }
                catch
                {
                    value = "<unreadable>";
                }

                result.Add(BuildNode(p.Name, value, visited));
            }

            return result;
        }

        private static bool IsLeaf(object obj)
        {
            if (obj == null) return true;
            var t = obj.GetType();

            if (t.IsEnum) return true;
            if (IsSimple(t)) return true;               // primitives, decimal, string, DateTime, etc.

            // Treat nullable<T> of simple as leaf
            if (Nullable.GetUnderlyingType(t) is Type nt && IsSimple(nt))
                return true;

            return false;
        }

        private static bool IsSimple(Type t)
        {
            if (t.IsPrimitive) return true;
            if (t == typeof(string) ||
                t == typeof(decimal) ||
                t == typeof(DateTime) ||
                t == typeof(DateOnly) ||
                t == typeof(TimeOnly) ||
                t == typeof(TimeSpan) ||
                t == typeof(Guid))
                return true;
            return false;
        }

        private static bool IsValueTypeOrString(Type t)
            => t.IsValueType || t == typeof(string);

        private static string GetDisplayValueOrType(object obj)
        {
            if (obj == null) return "<null>";
            // For non-leaf complex objects, show type name; for leaf, show actual value
            return IsLeaf(obj) ? obj.ToString() : $"<{obj.GetType().Name}>";
        }

        private static string SafeKey(object key)
            => key == null ? "null" : key.ToString();

        private static string MakeRootName(object data)
        {
            if (data == null) return "Root (null)";
            if (data is IEnumerable && data is not string) return $"{data.GetType().Name}";
            return data.GetType().Name;
        }

        // Reference equality comparer to detect cycles
        private sealed class ReferenceEqualityComparer : IEqualityComparer<object>
        {
            public static readonly ReferenceEqualityComparer Instance = new ReferenceEqualityComparer();
            public new bool Equals(object x, object y) => ReferenceEquals(x, y);
            public int GetHashCode(object obj) => System.Runtime.CompilerServices.RuntimeHelpers.GetHashCode(obj);
        }
    }
}
