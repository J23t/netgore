using Avalonia.Controls;
using DemoGame.Editor.Avalonia.ViewModels.Dockable;

namespace DemoGame.Editor.Avalonia.Views.Dockable
{
    /// <summary>
    /// Selected Map Objects Panel - Avalonia version of SelectedMapObjectsForm (WinForms)
    /// </summary>
    public partial class SelectedMapObjectsPanel : UserControl
    {
        public SelectedMapObjectsPanel()
        {
            InitializeComponent();
            DataContext = new SelectedMapObjectsViewModel();
        }
    }
}

