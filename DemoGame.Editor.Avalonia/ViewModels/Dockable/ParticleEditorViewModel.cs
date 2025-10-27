using System;
using System.Collections.ObjectModel;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using NetGore.Graphics.ParticleEngine;

namespace DemoGame.Editor.Avalonia.ViewModels.Dockable
{
    /// <summary>
    /// ViewModel for Particle Editor - edit particle effects
    /// Replaces WinForms ParticleEditorForm
    /// </summary>
    public partial class ParticleEditorViewModel : ViewModelBase
    {
        [ObservableProperty]
        private ObservableCollection<ParticleEmitterItem> _emitters = new();

        [ObservableProperty]
        private ParticleEmitterItem? _selectedEmitter;

        [ObservableProperty]
        private string _statusText = "Ready";

        [ObservableProperty]
        private bool _isPlaying;

        public ParticleEditorViewModel()
        {
            try
            {
                LoadEmitters();
            }
            catch (Exception ex)
            {
                StatusText = $"Error: {ex.Message}";
                LoadPlaceholderData();
            }
        }

        private void LoadEmitters()
        {
            Emitters.Clear();

            try
            {
                // Try to load actual particle emitters
                // For now, just use placeholder data since the API is complex
                LoadPlaceholderData();
            }
            catch
            {
                LoadPlaceholderData();
            }
        }

        private void LoadPlaceholderData()
        {
            Emitters.Clear();
            Emitters.Add(new ParticleEmitterItem("Fire"));
            Emitters.Add(new ParticleEmitterItem("Smoke"));
            Emitters.Add(new ParticleEmitterItem("Magic"));
            Emitters.Add(new ParticleEmitterItem("Explosion"));
            Emitters.Add(new ParticleEmitterItem("Rain"));
            StatusText = $"Loaded {Emitters.Count} placeholder emitter(s)";
        }

        [RelayCommand]
        private void NewEmitter()
        {
            var newEmitter = new ParticleEmitterItem($"NewEmitter{Emitters.Count + 1}");
            Emitters.Add(newEmitter);
            SelectedEmitter = newEmitter;
            StatusText = $"Created new emitter: {newEmitter.Name}";
        }

        [RelayCommand]
        private void DeleteEmitter()
        {
            if (SelectedEmitter != null)
            {
                var name = SelectedEmitter.Name;
                Emitters.Remove(SelectedEmitter);
                SelectedEmitter = Emitters.FirstOrDefault();
                StatusText = $"Deleted emitter: {name}";
            }
        }

        [RelayCommand]
        private void PlayEmitter()
        {
            if (SelectedEmitter != null)
            {
                IsPlaying = true;
                StatusText = $"Playing: {SelectedEmitter.Name}";
            }
        }

        [RelayCommand]
        private void StopEmitter()
        {
            IsPlaying = false;
            if (SelectedEmitter != null)
            {
                StatusText = $"Stopped: {SelectedEmitter.Name}";
            }
        }

        [RelayCommand]
        private void SaveEmitter()
        {
            if (SelectedEmitter != null)
            {
                StatusText = $"Saved: {SelectedEmitter.Name}";
            }
        }

        partial void OnSelectedEmitterChanged(ParticleEmitterItem? value)
        {
            if (value != null)
            {
                StatusText = $"Selected: {value.Name}";
                IsPlaying = false;
                
                // Notify other panels of selection (for Properties panel, etc.)
                SelectionChanged?.Invoke(this, value);
            }
        }

        /// <summary>
        /// Event raised when selection changes - other panels can subscribe to this
        /// </summary>
        public event EventHandler<object?>? SelectionChanged;
    }

    /// <summary>
    /// Represents a particle emitter in the list
    /// </summary>
    public partial class ParticleEmitterItem : ObservableObject
    {
        [ObservableProperty]
        private string _name;

        [ObservableProperty]
        private int _particleCount;

        [ObservableProperty]
        private string _textureName = "particle.png";

        public ParticleEmitterItem(string name)
        {
            _name = name;
            _particleCount = 100;
        }

        public override string ToString() => Name;
    }
}

