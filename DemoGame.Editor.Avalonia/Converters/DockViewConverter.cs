using System;
using System.Globalization;
using Avalonia.Data.Converters;
using DemoGame.Editor.Avalonia.ViewModels;

namespace DemoGame.Editor.Avalonia.Converters
{
    /// <summary>
    /// Converter that creates panel views from Tool IDs (lazy loading)
    /// </summary>
    public class DockViewConverter : IValueConverter
    {
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is string id)
            {
                return DockViewFactory.CreateView(id);
            }

            return null;
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}

