using System;
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;

namespace DemoGame.Editor.Avalonia.ViewModels.Dockable
{
    /// <summary>
    /// ViewModel for Selected Map Objects - migrated from SelectedMapObjectsForm (WinForms)
    /// Displays the currently selected objects on the map
    /// </summary>
    public partial class SelectedMapObjectsViewModel : ViewModelBase
    {
        [ObservableProperty]
        private ObservableCollection<object> _selectedObjects = new();

        [ObservableProperty]
        private object? _selectedItem;

        [ObservableProperty]
        private bool _hasMultipleSelection;

        public SelectedMapObjectsViewModel()
        {
            // In WinForms, this was wired to GlobalState.Instance.Map.SelectedObjsManager
            // For now, this is a placeholder - actual implementation would connect to map state
        }

        /// <summary>
        /// Updates the list of selected objects from the map
        /// </summary>
        public void UpdateSelectedObjects(ObservableCollection<object> objects)
        {
            SelectedObjects = objects;
            HasMultipleSelection = objects.Count > 1;
        }

        /// <summary>
        /// Called when selection changes
        /// </summary>
        partial void OnSelectedItemChanged(object? value)
        {
            // In full implementation, this would update the map selection
        }
    }
}

