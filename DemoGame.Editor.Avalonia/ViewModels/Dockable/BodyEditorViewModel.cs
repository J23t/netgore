using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using NetGore;
using NetGore.Content;
using NetGore.Graphics;
using NetGore.IO;

namespace DemoGame.Editor.Avalonia.ViewModels.Dockable
{
    /// <summary>
    /// ViewModel for Body Editor - migrated from BodyEditorForm (WinForms)
    /// Manages character body definitions
    /// </summary>
    public partial class BodyEditorViewModel : ViewModelBase, IDisposable
    {
        [ObservableProperty]
        private ObservableCollection<BodyInfo> _bodyItems = new();

        [ObservableProperty]
        private BodyInfo? _selectedBody;

        public BodyEditorViewModel()
        {
            LoadBodyList();
        }

        /// <summary>
        /// Loads the body list from BodyInfoManager
        /// </summary>
        private void LoadBodyList()
        {
            try
            {
                var bodies = BodyInfoManager.Instance.Bodies;
                BodyItems = new ObservableCollection<BodyInfo>(bodies.OrderBy(x => x.ID));
            }
            catch (Exception ex)
            {
                // Gracefully handle missing content files
                Console.WriteLine($"⚠️  Error loading body list: {ex.Message}");
                Console.WriteLine($"   Content files may not be available yet.");
                
                // Show empty list so panel still renders
                BodyItems = new ObservableCollection<BodyInfo>();
            }
        }

        /// <summary>
        /// Creates a new body
        /// </summary>
        [RelayCommand]
        private void CreateNewBody()
        {
            try
            {
                var newBody = BodyInfoManager.Instance.CreateBody();
                LoadBodyList(); // Refresh list
                SelectedBody = newBody; // Select the new item
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error creating body: {ex.Message}");
            }
        }

        /// <summary>
        /// Deletes the selected body
        /// </summary>
        [RelayCommand(CanExecute = nameof(CanDeleteBody))]
        private void DeleteBody()
        {
            if (SelectedBody == null)
                return;

            try
            {
                // In WinForms, this shows a confirmation dialog
                // For now, just delete (can add dialog later)
                var bodyToDelete = SelectedBody;
                SelectedBody = null; // Deselect first
                
                BodyInfoManager.Instance.RemoveBody(bodyToDelete.ID);
                LoadBodyList(); // Refresh list
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error deleting body: {ex.Message}");
            }
        }

        private bool CanDeleteBody() => SelectedBody != null;

        /// <summary>
        /// Called when selection changes
        /// </summary>
        partial void OnSelectedBodyChanged(BodyInfo? value)
        {
            DeleteBodyCommand.NotifyCanExecuteChanged();
            
            // Notify other panels of selection (for Properties panel, etc.)
            SelectionChanged?.Invoke(this, value);
        }

        /// <summary>
        /// Event raised when selection changes - other panels can subscribe to this
        /// </summary>
        public event EventHandler<object?>? SelectionChanged;

        /// <summary>
        /// Saves the body data (called on close/hide)
        /// </summary>
        private void Save()
        {
            try
            {
                // Save to dev path, then copy to build path (same as WinForms)
                BodyInfoManager.Instance.Save(ContentPaths.Dev);

                var src = BodyInfoManager.GetDefaultFilePath(ContentPaths.Dev);
                var dest = BodyInfoManager.GetDefaultFilePath(ContentPaths.Build);
                File.Copy(src, dest, true);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving bodies: {ex.Message}");
            }
        }

        /// <summary>
        /// Cleanup (saves on dispose, like WinForms FormClosing)
        /// </summary>
        public void Dispose()
        {
            Save();
            GC.SuppressFinalize(this);
        }
    }
}

