using Avalonia.Controls;
using DemoGame.Editor.Avalonia.ViewModels.Dockable;

namespace DemoGame.Editor.Avalonia.Views.Dockable
{
    /// <summary>
    /// GRH Tree View Panel - Browse and select graphics resources
    /// </summary>
    public partial class GrhTreeViewPanel : UserControl
    {
        public GrhTreeViewPanel()
        {
            InitializeComponent();
            DataContext = new GrhTreeViewViewModel();
        }

        /// <summary>
        /// Gets the ViewModel for external access
        /// </summary>
        public GrhTreeViewViewModel? ViewModel => DataContext as GrhTreeViewViewModel;
    }
}

