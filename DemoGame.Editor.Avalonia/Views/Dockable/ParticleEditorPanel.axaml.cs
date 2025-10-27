using Avalonia.Controls;
using DemoGame.Editor.Avalonia.ViewModels.Dockable;

namespace DemoGame.Editor.Avalonia.Views.Dockable
{
    /// <summary>
    /// Particle Editor Panel - Edit particle effects
    /// </summary>
    public partial class ParticleEditorPanel : UserControl
    {
        public ParticleEditorPanel()
        {
            InitializeComponent();
            DataContext = new ParticleEditorViewModel();
        }

        /// <summary>
        /// Gets the ViewModel for external access
        /// </summary>
        public ParticleEditorViewModel? ViewModel => DataContext as ParticleEditorViewModel;
    }
}

