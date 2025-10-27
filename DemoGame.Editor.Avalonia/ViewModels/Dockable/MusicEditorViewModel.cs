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
    /// ViewModel for Music Editor - migrated from MusicEditorForm (WinForms)
    /// Business logic stays the same, only UI framework changes
    /// </summary>
    public partial class MusicEditorViewModel : ViewModelBase, IDisposable
    {
        private readonly MusicManager? _musicManager;

        [ObservableProperty]
        private ObservableCollection<MusicInfo> _musicItems = new();

        [ObservableProperty]
        private MusicInfo? _selectedMusic;

        public MusicEditorViewModel()
        {
            try
            {
                Console.WriteLine($"🎵 MusicEditorViewModel: Initializing...");
                Console.WriteLine($"   Working directory: {System.IO.Directory.GetCurrentDirectory()}");
                
                _musicManager = new MusicManager();
                LoadMusicList();
                
                Console.WriteLine($"✅ MusicEditor: Loaded {MusicItems.Count} music files");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"⚠️  MusicManager initialization failed: {ex.Message}");
                Console.WriteLine($"   Stack trace: {ex.StackTrace}");
                Console.WriteLine($"   Inner: {ex.InnerException?.Message}");
                Console.WriteLine($"   Panel will show with empty list.");
                // MusicManager stays null, panel will still render
                MusicItems = new ObservableCollection<MusicInfo>();
            }
        }

        /// <summary>
        /// Loads the music list from ContentManager (same as WinForms version)
        /// </summary>
        private void LoadMusicList()
        {
            try
            {
                // Get music items from AudioManager (same as WinForms MusicInfoListBox.UpdateList)
                var contentManager = ContentManager.Create();
                var audioManager = AudioManager.GetInstance(contentManager);
                var musicInfos = audioManager.MusicManager.MusicInfos;
                
                if (musicInfos != null)
                {
                    MusicItems = new ObservableCollection<MusicInfo>(
                        musicInfos.Cast<MusicInfo>().OrderBy(x => x.ID)
                    );
                }
            }
            catch (Exception ex)
            {
                // Gracefully handle missing content files
                Console.WriteLine($"⚠️  Error loading music list: {ex.Message}");
                Console.WriteLine($"   Content files may not be available yet.");
                
                // Show placeholder data so panel still renders
                MusicItems = new ObservableCollection<MusicInfo>();
            }
        }

        /// <summary>
        /// Plays the selected music track
        /// </summary>
        [RelayCommand(CanExecute = nameof(CanPlayMusic))]
        private void PlayMusic()
        {
            try
            {
                if (SelectedMusic != null && _musicManager != null)
                {
                    _musicManager.Play(SelectedMusic.ID);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error playing music: {ex.Message}");
            }
        }

        private bool CanPlayMusic() => SelectedMusic != null && _musicManager != null;

        /// <summary>
        /// Stops the currently playing music
        /// </summary>
        [RelayCommand]
        private void StopMusic()
        {
            try
            {
                _musicManager?.Stop();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error stopping music: {ex.Message}");
            }
        }

        /// <summary>
        /// Called when selection changes - updates command states
        /// </summary>
        partial void OnSelectedMusicChanged(MusicInfo? value)
        {
            // Update command can-execute state
            PlayMusicCommand.NotifyCanExecuteChanged();
            
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
            _musicManager?.Stop();
            _musicManager?.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}

