using Avalonia.Controls;
using DemoGame.Editor.Avalonia.ViewModels.Dockable;

namespace DemoGame.Editor.Avalonia.Views.Dockable
{
    public partial class MapPreviewPanel : UserControl
    {
        public MapPreviewPanel()
        {
            InitializeComponent();
            DataContext = new MapPreviewViewModel();
        }
    }
}

