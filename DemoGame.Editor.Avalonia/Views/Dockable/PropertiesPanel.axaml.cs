using Avalonia.Controls;
using DemoGame.Editor.Avalonia.ViewModels.Dockable;

namespace DemoGame.Editor.Avalonia.Views.Dockable
{
    /// <summary>
    /// Properties Panel - shows properties of selected objects
    /// </summary>
    public partial class PropertiesPanel : UserControl
    {
        public PropertiesPanel()
        {
            InitializeComponent();
            DataContext = new PropertiesViewModel();
        }

        /// <summary>
        /// Gets the ViewModel for external access
        /// </summary>
        public PropertiesViewModel? ViewModel => DataContext as PropertiesViewModel;
    }
}

