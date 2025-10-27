using Avalonia.Controls;
using DemoGame.Editor.Avalonia.ViewModels.Dockable;

namespace DemoGame.Editor.Avalonia.Views.Dockable
{
    /// <summary>
    /// Sound Editor Panel - Avalonia version of SoundEditorForm (WinForms)
    /// </summary>
    public partial class SoundEditorPanel : UserControl
    {
        public SoundEditorPanel()
        {
            InitializeComponent();
            DataContext = new SoundEditorViewModel();
        }

        /// <summary>
        /// Cleanup when panel is unloaded
        /// </summary>
        protected override void OnUnloaded(global::Avalonia.Interactivity.RoutedEventArgs e)
        {
            base.OnUnloaded(e);
            
            // Dispose ViewModel to cleanup SoundManager
            if (DataContext is SoundEditorViewModel viewModel)
            {
                viewModel.Dispose();
            }
        }
    }
}

