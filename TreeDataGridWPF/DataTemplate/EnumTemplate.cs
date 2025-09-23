using System;
using System.Reflection;
using System.Windows;
using System.Windows.Markup;

namespace TreeDataGridWPF.Controls
{
    public partial class TreeDataGrid
    {
        public static DataTemplate EnumTemplate(PropertyInfo prop, object value = null)
        {
            // Handle Nullable<Enum>
            var runtimeType = value?.GetType() ?? prop.PropertyType;
            var enumType = Nullable.GetUnderlyingType(runtimeType) ?? runtimeType;
            if (!enumType.IsEnum)
                throw new ArgumentException($"Property '{prop.Name}' is not an enum (got {enumType}).");

            var enumNs = enumType.Namespace;
            var enumAsm = enumType.Assembly.GetName().Name;

            var extType = typeof(EnumValuesExtension);
            var extNs = extType.Namespace;
            var extAsm = extType.Assembly.GetName().Name;

            var enumTypeName = enumType.Name;

            var xaml =
              "<DataTemplate xmlns='http://schemas.microsoft.com/winfx/2006/xaml/presentation' " +
              "              xmlns:x='http://schemas.microsoft.com/winfx/2006/xaml' " +
              $"              xmlns:enumNs='clr-namespace:{enumNs};assembly={enumAsm}' " +
              $"              xmlns:ext='clr-namespace:{extNs};assembly={extAsm}'> " +
              $"  <ComboBox SelectedItem='{{Binding Model.{prop.Name}, Mode=TwoWay, UpdateSourceTrigger=PropertyChanged}}'> " +
              $"      <ComboBox.ItemsSource> " +
              $"          <ext:EnumValues EnumType='{{x:Type enumNs:{enumTypeName}}}' /> " +
              $"      </ComboBox.ItemsSource> " +
              $"  </ComboBox> " +
              "</DataTemplate>";

            return (DataTemplate)XamlReader.Parse(xaml);
        }
    }

    [MarkupExtensionReturnType(typeof(Array))]
    public class EnumValuesExtension : MarkupExtension
    {
        public Type EnumType { get; set; }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            var t = EnumType;
            if (t == null)
                return Array.Empty<object>();

            // Unwrap Nullable<TEnum>
            var underlying = Nullable.GetUnderlyingType(t);
            if (underlying != null) t = underlying;

            if (!t.IsEnum)
                throw new ArgumentException($"Type provided must be an Enum, got: {t.FullName}", nameof(EnumType));

            return Enum.GetValues(t);
        }
    }
}
