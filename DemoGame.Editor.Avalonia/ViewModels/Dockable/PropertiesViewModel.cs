using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using CommunityToolkit.Mvvm.ComponentModel;

namespace DemoGame.Editor.Avalonia.ViewModels.Dockable
{
    /// <summary>
    /// ViewModel for Properties Panel - shows properties of selected objects
    /// Replaces WinForms PropertyGrid
    /// </summary>
    public partial class PropertiesViewModel : ViewModelBase
    {
        [ObservableProperty]
        private object? _selectedObject;

        [ObservableProperty]
        private string _selectedObjectType = "No selection";

        [ObservableProperty]
        private ObservableCollection<PropertyItem> _properties = new();

        public PropertiesViewModel()
        {
            // In full implementation, this would wire up to GlobalState selection events
            // For now, shows placeholder
        }

        /// <summary>
        /// Updates the selected object to display properties for
        /// </summary>
        public void SetSelectedObject(object? obj)
        {
            SelectedObject = obj;
            SelectedObjectType = obj?.GetType().Name ?? "No selection";
            LoadProperties(obj);
        }

        partial void OnSelectedObjectChanged(object? value)
        {
            SelectedObjectType = value?.GetType().Name ?? "No selection";
            LoadProperties(value);
        }

        private void LoadProperties(object? obj)
        {
            Properties.Clear();

            if (obj == null)
                return;

            try
            {
                var type = obj.GetType();
                var properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                    .Where(p => p.CanRead)
                    .OrderBy(p => p.Name);

                foreach (var prop in properties)
                {
                    try
                    {
                        var value = prop.GetValue(obj);
                        var valueStr = value?.ToString() ?? "(null)";
                        
                        // Format the value based on type
                        if (value != null)
                        {
                            if (value is bool b)
                                valueStr = b ? "True" : "False";
                            else if (value is DateTime dt)
                                valueStr = dt.ToString("yyyy-MM-dd HH:mm:ss");
                            else if (value is float f)
                                valueStr = f.ToString("F2");
                            else if (value is double d)
                                valueStr = d.ToString("F2");
                        }

                        // Determine if property is editable (has public setter, is writable, not complex type)
                        bool isEditable = prop.CanWrite && 
                                         prop.SetMethod != null && 
                                         prop.SetMethod.IsPublic &&
                                         (prop.PropertyType.IsPrimitive || 
                                          prop.PropertyType == typeof(string) ||
                                          prop.PropertyType == typeof(DateTime) ||
                                          prop.PropertyType.IsEnum);

                        Properties.Add(new PropertyItem(prop.Name, valueStr, prop.PropertyType.Name, isEditable, prop, obj));
                    }
                    catch
                    {
                        // Skip properties that throw exceptions when accessed
                        Properties.Add(new PropertyItem(prop.Name, "(Error reading value)", prop.PropertyType.Name, false));
                    }
                }
            }
            catch (Exception ex)
            {
                Properties.Add(new PropertyItem("Error", ex.Message, ""));
            }
        }
    }

    /// <summary>
    /// Represents a property in the properties list
    /// </summary>
    public partial class PropertyItem : ObservableObject
    {
        [ObservableProperty]
        private string _name;

        [ObservableProperty]
        private string _value;

        [ObservableProperty]
        private string _type;

        [ObservableProperty]
        private bool _isEditable;

        [ObservableProperty]
        private bool _isBooleanProperty;

        [ObservableProperty]
        private bool _isNumericProperty;

        public PropertyInfo? PropertyInfo { get; set; }
        public object? SourceObject { get; set; }

        public PropertyItem(string name, string value, string type, bool isEditable = false, PropertyInfo? propertyInfo = null, object? sourceObject = null)
        {
            _name = name;
            _value = value;
            _type = type;
            _isEditable = isEditable;
            PropertyInfo = propertyInfo;
            SourceObject = sourceObject;

            // Determine property type for UI
            _isBooleanProperty = type == "Boolean";
            _isNumericProperty = type == "Int32" || type == "Single" || type == "Double" || type == "Int64";
        }

        partial void OnValueChanged(string value)
        {
            // Try to set the property value on the source object
            if (IsEditable && PropertyInfo != null && SourceObject != null && PropertyInfo.CanWrite)
            {
                try
                {
                    var convertedValue = Convert.ChangeType(value, PropertyInfo.PropertyType);
                    PropertyInfo.SetValue(SourceObject, convertedValue);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error setting property {Name}: {ex.Message}");
                }
            }
        }
    }
}

