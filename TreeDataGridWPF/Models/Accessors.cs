using System;
using System.Collections;
using System.Reflection;

namespace TreeDataGridWPF.Models
{
    public interface IAccessor
    {
        object Get();
        void Set(object value);      // may throw or be a no-op if not writable
        bool CanWrite { get; }
        object Owner { get; }
    }

    internal static class CoerceHelper
    {
        public static object Coerce(object value, Type targetType)
        {
            if (targetType == null) return value;

            if (value == null)
            {
                if (!targetType.IsValueType || Nullable.GetUnderlyingType(targetType) != null)
                    return null;
                return Activator.CreateInstance(targetType);
            }

            var srcType = value.GetType();
            if (targetType.IsAssignableFrom(srcType)) return value;

            // Nullable<T>
            var underlying = Nullable.GetUnderlyingType(targetType);
            if (underlying != null) return Coerce(value, underlying);

            // Enums
            if (targetType.IsEnum)
            {
                if (value is string s) return Enum.Parse(targetType, s, ignoreCase: true);
                return Enum.ToObject(targetType, Convert.ChangeType(value, Enum.GetUnderlyingType(targetType)));
            }

            // Common conversions
            try
            {
                if (targetType == typeof(Guid) && value is string gs) return Guid.Parse(gs);
                if (targetType == typeof(DateTime) && value is string ds) return DateTime.Parse(ds);
                if (targetType == typeof(TimeSpan) && value is string ts) return TimeSpan.Parse(ts);
                return Convert.ChangeType(value, targetType);
            }
            catch
            {
                return value; // fallback: leave as-is
            }
        }
    }

    internal sealed class LambdaAccessor : IAccessor
    {
        private readonly object _owner;
        private readonly Func<object, object> _getter;
        private readonly Action<object, object> _setter;

        public LambdaAccessor(object owner, Func<object, object> getter, Action<object, object> setter = null)
        {
            _owner = owner ?? throw new ArgumentNullException(nameof(owner));
            _getter = getter ?? throw new ArgumentNullException(nameof(getter));
            _setter = setter; // optional
        }

        public object Get()
        {
            try { return _getter(_owner); }
            catch { return "<unreadable>"; }
        }

        public void Set(object value)
        {
            if (!CanWrite) return;
            try { _setter(_owner, value); } catch { /* swallow or log */ }
        }

        public bool CanWrite => _setter != null;
        public object Owner => _owner;
    }

    internal sealed class PropertyAccessor : IAccessor
    {
        private readonly object _owner;
        private readonly PropertyInfo _prop;

        public PropertyAccessor(object owner, PropertyInfo prop) { _owner = owner; _prop = prop; }
        public object Get()
        {
            try { return _prop.GetValue(_owner, null); }
            catch { return "<unreadable>"; }
        }
        public void Set(object value)
        {
            if (!CanWrite) return;
            var coerced = CoerceHelper.Coerce(value, _prop.PropertyType);
            _prop.SetValue(_owner, coerced, null);
        }
        public bool CanWrite => _prop.CanWrite && _prop.SetMethod != null && _prop.SetMethod.IsPublic;
        public object Owner => _owner;
    }

    internal sealed class FieldAccessor : IAccessor
    {
        private readonly object _owner;
        private readonly FieldInfo _field;

        public FieldAccessor(object owner, FieldInfo field) { _owner = owner; _field = field; }
        public object Get()
        {
            try { return _field.GetValue(_owner); }
            catch { return "<unreadable>"; }
        }
        public void Set(object value)
        {
            if (!CanWrite) return;
            var coerced = CoerceHelper.Coerce(value, _field.FieldType);
            _field.SetValue(_owner, coerced);
        }
        public bool CanWrite => _field.IsPublic && !_field.IsInitOnly;
        public object Owner => _owner;
    }

    internal sealed class ListItemAccessor : IAccessor
    {
        private readonly IList _list;
        private readonly int _index;
        private readonly Type _itemType;   // best effort

        public ListItemAccessor(IList list, int index, Type itemType = null)
        {
            _list = list; _index = index; _itemType = itemType;
        }
        public object Get() => _index < _list.Count ? _list[_index] : "<out of range>";

        public void Set(object value)
        {
            if (!CanWrite || _index >= _list.Count) return;
            var targetType = _itemType ?? value?.GetType() ?? typeof(object);
            _list[_index] = CoerceHelper.Coerce(value, targetType);
        }
        public bool CanWrite => !_list.IsReadOnly;
        public object Owner => _list;
    }

    internal sealed class DictionaryEntryAccessor : IAccessor
    {
        private readonly IDictionary _dict;
        private readonly object _key;
        private readonly Type _valueType;  // best effort

        public DictionaryEntryAccessor(IDictionary dict, object key, Type valueType = null)
        {
            _dict = dict; _key = key; _valueType = valueType;
        }
        public object Get() => _dict.Contains(_key) ? _dict[_key] : "<missing key>";

        public void Set(object value)
        {
            if (!CanWrite) return;
            var t = _valueType ?? value?.GetType() ?? typeof(object);
            _dict[_key] = CoerceHelper.Coerce(value, t);
        }
        public bool CanWrite => !_dict.IsReadOnly;
        public object Owner => _dict;
    }
}
