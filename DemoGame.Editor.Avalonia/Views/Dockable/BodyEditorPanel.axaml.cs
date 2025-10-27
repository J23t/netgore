using Avalonia.Controls;
using DemoGame.Editor.Avalonia.ViewModels.Dockable;

namespace DemoGame.Editor.Avalonia.Views.Dockable
{
    /// <summary>
    /// Body Editor Panel - Avalonia version of BodyEditorForm (WinForms)
    /// </summary>
    public partial class BodyEditorPanel : UserControl
    {
        public BodyEditorPanel()
        {
            InitializeComponent();
            DataContext = new BodyEditorViewModel();
        }

        /// <summary>
        /// Cleanup when panel is unloaded (saves data)
        /// </summary>
        protected override void OnUnloaded(global::Avalonia.Interactivity.RoutedEventArgs e)
        {
            base.OnUnloaded(e);
            
            // Dispose ViewModel to trigger save
            if (DataContext is BodyEditorViewModel viewModel)
            {
                viewModel.Dispose();
            }
        }
    }
}

