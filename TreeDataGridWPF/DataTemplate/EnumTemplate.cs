using System;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Markup;
using System.Xml.Linq;
using static System.Net.Mime.MediaTypeNames;

namespace TreeDataGridWPF.Controls
{
    public partial class TreeDataGrid
    {
        /// <summary>
        /// Dynamically creates a DataTemplate for enum properties.
        /// </summary>
        public static DataTemplate EnumTemplate(PropertyInfo prop, string name)
        {
            var enumType = prop.PropertyType;
            var enumNs = enumType.Namespace;                       // e.g. MyApp.Models
            var enumTypeName = enumType.Name;                      // e.g. Status
            var markupNs = "clr-namespace:TreeDataGridWPF.Controls"; // where EnumValuesExtension lives

            var xaml =
              "<DataTemplate xmlns='http://schemas.microsoft.com/winfx/2006/xaml/presentation' " +
              "              xmlns:x='http://schemas.microsoft.com/winfx/2006/xaml' " +
              $"              xmlns:enumNs='clr-namespace:{enumNs}' " +
              $"              xmlns:ext='{markupNs}'>" +
              $"  <ComboBox SelectedItem='{{Binding Model.{name}, Mode=TwoWay, UpdateSourceTrigger=PropertyChanged}}'>" +
              $"      <ComboBox.ItemsSource>" +
              $"          <ext:EnumValues EnumType='{{x:Type enumNs:{enumTypeName}}}' />" +
              $"      </ComboBox.ItemsSource>" +
              $"  </ComboBox>" +
              "</DataTemplate>";
            return (DataTemplate)System.Windows.Markup.XamlReader.Parse(xaml);
        }
    }

    [MarkupExtensionReturnType(typeof(Array))]
    public class EnumValuesExtension : MarkupExtension
    {
        public Type EnumType { get; set; }
        public override object ProvideValue(IServiceProvider serviceProvider)
            => EnumType != null ? Enum.GetValues(EnumType) : Array.Empty<object>();
    }
}
