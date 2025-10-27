using System;
using System.Collections.ObjectModel;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using NetGore.Audio;
using NetGore.Content;

namespace DemoGame.Editor.Avalonia.ViewModels.Dockable
{
    /// <summary>
    /// ViewModel for Sound Editor - migrated from SoundEditorForm (WinForms)
    /// Business logic stays the same, only UI framework changes
    /// </summary>
    public partial class SoundEditorViewModel : ViewModelBase, IDisposable
    {
        private readonly SoundManager? _soundManager;

        [ObservableProperty]
        private ObservableCollection<SoundInfo> _soundItems = new();

        [ObservableProperty]
        private SoundInfo? _selectedSound;

        public SoundEditorViewModel()
        {
            try
            {
                _soundManager = new SoundManager(ContentManager.Create());
                LoadSoundList();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"⚠️  SoundManager initialization failed: {ex.Message}");
                Console.WriteLine($"   Panel will show with empty list.");
                SoundItems = new ObservableCollection<SoundInfo>();
            }
        }

        /// <summary>
        /// Loads the sound list from AudioManager (same as WinForms version)
        /// </summary>
        private void LoadSoundList()
        {
            try
            {
                // Get sound items from AudioManager (same as WinForms SoundInfoListBox.UpdateList)
                var contentManager = ContentManager.Create();
                var audioManager = AudioManager.GetInstance(contentManager);
                var soundInfos = audioManager.SoundManager.SoundInfos;
                
                if (soundInfos != null)
                {
                    SoundItems = new ObservableCollection<SoundInfo>(
                        soundInfos.Cast<SoundInfo>().OrderBy(x => x.ID)
                    );
                }
            }
            catch (Exception ex)
            {
                // Gracefully handle missing content files
                Console.WriteLine($"⚠️  Error loading sound list: {ex.Message}");
                Console.WriteLine($"   Content files may not be available yet.");
                
                // Show placeholder data so panel still renders
                SoundItems = new ObservableCollection<SoundInfo>();
            }
        }

        /// <summary>
        /// Plays the selected sound
        /// </summary>
        [RelayCommand(CanExecute = nameof(CanPlaySound))]
        private void PlaySound()
        {
            try
            {
                if (SelectedSound != null && _soundManager != null)
                {
                    _soundManager.Play(SelectedSound.ID);
                }
            }
            catch (Exception ex)
            {
                // In WinForms, this showed MessageBox
                // In Avalonia, we could use a dialog or status message
                Console.WriteLine($"Error playing sound: {ex.Message}");
            }
        }

        private bool CanPlaySound() => SelectedSound != null && _soundManager != null;

        /// <summary>
        /// Stops the currently playing sound
        /// </summary>
        [RelayCommand]
        private void StopSound()
        {
            try
            {
                _soundManager?.Stop();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error stopping sound: {ex.Message}");
            }
        }

        /// <summary>
        /// Called when selection changes - updates command states
        /// </summary>
        partial void OnSelectedSoundChanged(SoundInfo? value)
        {
            // Update command can-execute state
            PlaySoundCommand.NotifyCanExecuteChanged();
            
            // Notify other panels of selection (for Properties panel, etc.)
            SelectionChanged?.Invoke(this, value);
        }

        /// <summary>
        /// Event raised when selection changes - other panels can subscribe to this
        /// </summary>
        public event EventHandler<object?>? SelectionChanged;

        /// <summary>
        /// Cleanup resources (same as WinForms FormClosing)
        /// </summary>
        public void Dispose()
        {
            _soundManager?.Stop();
            GC.SuppressFinalize(this);
        }
    }
}

