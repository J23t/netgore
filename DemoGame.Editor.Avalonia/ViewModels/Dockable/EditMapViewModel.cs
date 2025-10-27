using System;
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DemoGame.Editor.Avalonia.Services;
using NetGore;

namespace DemoGame.Editor.Avalonia.ViewModels.Dockable
{
    /// <summary>
    /// ViewModel for Map Editor - main map editing interface
    /// Replaces WinForms EditMapForm
    /// </summary>
    public partial class EditMapViewModel : ViewModelBase
    {
        private object? _currentMap;  // Will be EditorMap when SFML integrated

        [ObservableProperty]
        private string _mapName = "No map loaded";

        [ObservableProperty]
        private int _mapWidth = 960;

        [ObservableProperty]
        private int _mapHeight = 960;

        [ObservableProperty]
        private int _mapId;

        [ObservableProperty]
        private string _currentTool = "Selection";

        [ObservableProperty]
        private string _currentLayer = "Ground";

        [ObservableProperty]
        private int _zoom = 100;

        [ObservableProperty]
        private bool _showGrid = true;

        [ObservableProperty]
        private bool _showWalls = true;

        [ObservableProperty]
        private bool _showEntities = true;

        [ObservableProperty]
        private string _statusText = "Ready";

        [ObservableProperty]
        private int _mouseX;

        [ObservableProperty]
        private int _mouseY;

        [ObservableProperty]
        private ObservableCollection<string> _availableTools = new();

        [ObservableProperty]
        private ObservableCollection<string> _availableLayers = new();

        public EditMapViewModel()
        {
            InitializeTools();
            InitializeLayers();
            StatusText = $"Map: {MapName} ({MapWidth}x{MapHeight})";
        }

        private void InitializeTools()
        {
            AvailableTools.Clear();
            AvailableTools.Add("Selection");
            AvailableTools.Add("Pencil");
            AvailableTools.Add("Fill");
            AvailableTools.Add("Eraser");
            AvailableTools.Add("Entity Placer");
            AvailableTools.Add("Wall Placer");
            AvailableTools.Add("Light Placer");
        }

        private void InitializeLayers()
        {
            AvailableLayers.Clear();
            AvailableLayers.Add("Ground");
            AvailableLayers.Add("Fringe");
            AvailableLayers.Add("Foreground");
            AvailableLayers.Add("Background");
            AvailableLayers.Add("Entities");
            AvailableLayers.Add("Walls");
        }

        /// <summary>
        /// Loads a map into this editor (simplified for now)
        /// </summary>
        public void LoadMap(int mapId, string mapName = "")
        {
            _currentMap = new { MapID = mapId, Name = mapName };
            MapId = mapId;
            MapName = !string.IsNullOrEmpty(mapName) ? mapName : $"Map {mapId}";
            StatusText = $"Map {mapId} ready (SFML rendering pending)";
        }

        /// <summary>
        /// Gets the current map ID being edited
        /// </summary>
        public int GetCurrentMapID() => MapId;

        [RelayCommand]
        private void SaveMap()
        {
            if (_currentMap != null)
            {
                bool saved = MapService.SaveMap(_currentMap);
                if (saved)
                {
                    StatusText = $"✅ Saved map: {MapName}";
                }
                else
                {
                    StatusText = $"❌ Error saving map: {MapName}";
                }
            }
            else
            {
                StatusText = "No map to save";
            }
        }

        [RelayCommand]
        private void ZoomIn()
        {
            if (Zoom < 400)
            {
                Zoom += 25;
                StatusText = $"Zoom: {Zoom}%";
            }
        }

        [RelayCommand]
        private void ZoomOut()
        {
            if (Zoom > 25)
            {
                Zoom -= 25;
                StatusText = $"Zoom: {Zoom}%";
            }
        }

        [RelayCommand]
        private void ResetZoom()
        {
            Zoom = 100;
            StatusText = "Zoom: 100%";
        }

        [RelayCommand]
        private void ToggleGrid()
        {
            ShowGrid = !ShowGrid;
            StatusText = $"Grid: {(ShowGrid ? "Visible" : "Hidden")}";
        }

        [RelayCommand]
        private void ToggleWalls()
        {
            ShowWalls = !ShowWalls;
            StatusText = $"Walls: {(ShowWalls ? "Visible" : "Hidden")}";
        }

        [RelayCommand]
        private void ToggleEntities()
        {
            ShowEntities = !ShowEntities;
            StatusText = $"Entities: {(ShowEntities ? "Visible" : "Hidden")}";
        }

        [RelayCommand]
        private void SelectTool(string? tool)
        {
            if (!string.IsNullOrEmpty(tool))
            {
                CurrentTool = tool;
                StatusText = $"Tool: {tool}";
            }
        }

        [RelayCommand]
        private void SelectLayer(string? layer)
        {
            if (!string.IsNullOrEmpty(layer))
            {
                CurrentLayer = layer;
                StatusText = $"Layer: {layer}";
            }
        }

        [RelayCommand]
        private void Undo()
        {
            StatusText = "Undo (not implemented)";
        }

        [RelayCommand]
        private void Redo()
        {
            StatusText = "Redo (not implemented)";
        }

        [RelayCommand]
        private void ClearLayer()
        {
            StatusText = $"Clear layer: {CurrentLayer} (not implemented)";
        }

        [RelayCommand]
        private void MapProperties()
        {
            StatusText = "Map properties dialog (not implemented)";
        }

        public void UpdateMousePosition(int x, int y)
        {
            MouseX = x;
            MouseY = y;
        }

        partial void OnCurrentToolChanged(string value)
        {
            StatusText = $"Tool: {value}";
        }

        partial void OnCurrentLayerChanged(string value)
        {
            StatusText = $"Layer: {value}";
        }

        partial void OnZoomChanged(int value)
        {
            // Could trigger map redraw at new zoom level
        }
    }
}

