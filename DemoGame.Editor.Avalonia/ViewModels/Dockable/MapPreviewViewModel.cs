using CommunityToolkit.Mvvm.ComponentModel;

namespace DemoGame.Editor.Avalonia.ViewModels.Dockable
{
    /// <summary>
    /// ViewModel for Map Preview - migrated from MapPreviewForm (WinForms)
    /// Shows a preview rendering of the map
    /// </summary>
    public partial class MapPreviewViewModel : ViewModelBase
    {
        [ObservableProperty]
        private string _statusMessage = "Map preview rendering will be implemented here";

        public MapPreviewViewModel()
        {
            // In WinForms, this contained MapPreviewScreenControl (custom rendering)
            // For now, placeholder - we'll add rendering in Phase 3 (complex forms)
        }
    }
}

