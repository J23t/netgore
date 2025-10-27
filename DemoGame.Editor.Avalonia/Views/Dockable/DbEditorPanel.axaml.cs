using Avalonia.Controls;
using DemoGame.Editor.Avalonia.ViewModels.Dockable;

namespace DemoGame.Editor.Avalonia.Views.Dockable
{
    /// <summary>
    /// Database Editor Panel - Edit game database tables
    /// </summary>
    public partial class DbEditorPanel : UserControl
    {
        public DbEditorPanel()
        {
            InitializeComponent();
            DataContext = new DbEditorViewModel();
        }

        /// <summary>
        /// Gets the ViewModel for external access
        /// </summary>
        public DbEditorViewModel? ViewModel => DataContext as DbEditorViewModel;
    }
}

