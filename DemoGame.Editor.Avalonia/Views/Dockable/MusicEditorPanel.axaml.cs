using Avalonia.Controls;
using DemoGame.Editor.Avalonia.ViewModels.Dockable;

namespace DemoGame.Editor.Avalonia.Views.Dockable
{
    /// <summary>
    /// Music Editor Panel - Avalonia version of MusicEditorForm (WinForms)
    /// </summary>
    public partial class MusicEditorPanel : UserControl
    {
        public MusicEditorPanel()
        {
            InitializeComponent();
            DataContext = new MusicEditorViewModel();
        }

        /// <summary>
        /// Cleanup when panel is unloaded
        /// </summary>
        protected override void OnUnloaded(global::Avalonia.Interactivity.RoutedEventArgs e)
        {
            base.OnUnloaded(e);
            
            // Dispose ViewModel to cleanup MusicManager
            if (DataContext is MusicEditorViewModel viewModel)
            {
                viewModel.Dispose();
            }
        }
    }
}

