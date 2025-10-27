using Avalonia.Controls;
using DemoGame.Editor.Avalonia.ViewModels.Dockable;

namespace DemoGame.Editor.Avalonia.Views.Dockable
{
    /// <summary>
    /// NPC Chat Editor Panel - Edit dialog trees
    /// </summary>
    public partial class NPCChatEditorPanel : UserControl
    {
        public NPCChatEditorPanel()
        {
            InitializeComponent();
            DataContext = new NPCChatEditorViewModel();
        }

        /// <summary>
        /// Gets the ViewModel for external access
        /// </summary>
        public NPCChatEditorViewModel? ViewModel => DataContext as NPCChatEditorViewModel;
    }
}

