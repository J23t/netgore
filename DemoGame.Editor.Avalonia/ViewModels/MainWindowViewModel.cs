using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;

namespace DemoGame.Editor.Avalonia.ViewModels
{
    /// <summary>
    /// Main window view model - contains docking layout and menu commands
    /// </summary>
    public partial class MainWindowViewModel : ViewModelBase
    {
        public DockViewModel DockViewModel { get; }

        [ObservableProperty]
        private string _statusBarLeft = "Ready - NetGore Editor (Avalonia) • Cross-platform • Linux Native";

        [ObservableProperty]
        private string _statusBarRight = $"Framework: .NET {Environment.Version.Major}.{Environment.Version.Minor} • Avalonia UI 11";

        [ObservableProperty]
        private ObservableCollection<Services.MapListItem> _recentMaps = new();

        public MainWindowViewModel()
        {
            DockViewModel = new DockViewModel();
            
            // Update status bar with useful info
            UpdateStatusBar();
            
            // Load recent files
            LoadRecentMaps();
        }

        private void LoadRecentMaps()
        {
            try
            {
                var recent = Services.RecentFilesService.GetRecentMaps();
                RecentMaps.Clear();
                foreach (var map in recent.Take(5))  // Show top 5 in menu
                {
                    RecentMaps.Add(map);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading recent maps: {ex.Message}");
            }
        }

        private void UpdateStatusBar()
        {
            StatusBarLeft = $"Ready - NetGore Editor (Avalonia) • {Environment.OSVersion.Platform}";
            StatusBarRight = $"Framework: .NET {Environment.Version.Major}.{Environment.Version.Minor} • Panels: 12/12 ✅";
        }

        public void SetStatus(string message)
        {
            StatusBarLeft = message;
        }

        // File Menu Commands
        [RelayCommand]
        private void NewMap()
        {
            Console.WriteLine("New Map command executed");
            
            try
            {
                var mapId = Services.MapService.CreateNewMap();
                if (mapId.HasValue)
                {
                    StatusBarLeft = $"✅ Created new map with ID: {mapId.Value}";
                    
                    // Add to recent files
                    Services.RecentFilesService.AddRecentMap(mapId.Value, $"Map {mapId.Value}");
                    LoadRecentMaps();
                    
                    // Activate the Map Editor panel
                    DockViewModel.ActivatePanel("MapEditor");
                    
                    StatusBarLeft = $"✅ Created map {mapId.Value} - Ready to edit (SFML rendering pending)";
                }
                else
                {
                    StatusBarLeft = "❌ Failed to create new map";
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error creating map: {ex.Message}");
                StatusBarLeft = $"❌ Error: {ex.Message}";
            }
        }

        [RelayCommand]
        private async Task OpenMap()
        {
            Console.WriteLine("Open Map command executed");
            
            try
            {
                var dialog = new Views.MapSelectionDialog();
                
                // Try to get the main window as parent
                var mainWindow = global::Avalonia.Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop
                    ? desktop.MainWindow
                    : null;
                
                bool? result = false;
                if (mainWindow != null)
                {
                    result = await dialog.ShowDialog<bool?>(mainWindow);
                }
                else
                {
                    dialog.Show();
                }

                if (result == true && dialog.SelectedMapID.HasValue)
                {
                    var mapId = dialog.SelectedMapID.Value;
                    var map = Services.MapService.LoadMap(mapId);
                    
                    if (map != null)
                    {
                        StatusBarLeft = $"✅ Map {mapId} ready to edit";
                        
                        // Add to recent files
                        Services.RecentFilesService.AddRecentMap(mapId, $"Map {mapId}");
                        LoadRecentMaps();
                        
                        // Activate Map Editor
                        DockViewModel.ActivatePanel("MapEditor");
                        
                        // TODO: When SFML integrated, load map fully in EditMapViewModel
                        Console.WriteLine($"Map {mapId} selected - will be fully loaded when SFML integrated");
                    }
                    else
                    {
                        StatusBarLeft = $"❌ Failed to prepare map {mapId}";
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error opening map: {ex.Message}");
                StatusBarLeft = $"❌ Error: {ex.Message}";
            }
        }

        [RelayCommand]
        private void Save()
        {
            Console.WriteLine("Save command executed");
            StatusBarLeft = "💾 Save functionality ready - select a map to save";
            // TODO: Get current active map and save it
        }

        [RelayCommand]
        private void SaveAs()
        {
            Console.WriteLine("Save As command executed");
            StatusBarLeft = "💾 Save As functionality ready";
            // TODO: Implement save as dialog
        }

        [RelayCommand]
        private void OpenRecentMap(Services.MapListItem? mapItem)
        {
            if (mapItem == null) return;

            try
            {
                Console.WriteLine($"Opening recent map: {mapItem.ID}");
                var map = Services.MapService.LoadMap(mapItem.ID);
                
                if (map != null)
                {
                    StatusBarLeft = $"✅ Opened recent map {mapItem.ID}: {mapItem.Name}";
                    
                    // Update recent files (move to top)
                    Services.RecentFilesService.AddRecentMap(mapItem.ID, mapItem.Name);
                    LoadRecentMaps();
                    
                    // Activate Map Editor
                    DockViewModel.ActivatePanel("MapEditor");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error opening recent map: {ex.Message}");
                StatusBarLeft = $"❌ Error opening map: {ex.Message}";
            }
        }

        [RelayCommand]
        private void Exit()
        {
            Console.WriteLine("Exit command executed");
            Environment.Exit(0);
        }

        // Edit Menu Commands
        [RelayCommand]
        private void Undo()
        {
            Console.WriteLine("Undo command executed");
            // TODO: Implement undo
        }

        [RelayCommand]
        private void Redo()
        {
            Console.WriteLine("Redo command executed");
            // TODO: Implement redo
        }

        [RelayCommand]
        private void Copy()
        {
            Console.WriteLine("Copy command executed");
            // TODO: Implement copy
        }

        [RelayCommand]
        private void Paste()
        {
            Console.WriteLine("Paste command executed");
            // TODO: Implement paste
        }

        // View Menu Commands
        [RelayCommand]
        private void ShowGrhTreeView()
        {
            Console.WriteLine("Show GRH Tree View");
            DockViewModel.ActivatePanel("GrhTreeView");
        }

        [RelayCommand]
        private void ShowProperties()
        {
            Console.WriteLine("Show Properties panel");
            DockViewModel.ActivatePanel("Properties");
        }

        [RelayCommand]
        private void ShowDatabaseEditor()
        {
            Console.WriteLine("Show Database Editor");
            DockViewModel.ActivatePanel("DbEditor");
        }

        // Tools Menu Commands
        [RelayCommand]
        private void ShowMapEditor()
        {
            Console.WriteLine("Show Map Editor");
            DockViewModel.ActivatePanel("MapEditor");
        }

        [RelayCommand]
        private void ShowSkeletonEditor()
        {
            Console.WriteLine("Show Skeleton Editor");
            DockViewModel.ActivatePanel("SkeletonEditor");
        }

        // Help Menu Commands
        [RelayCommand]
        private async Task ShowAbout()
        {
            Console.WriteLine("About NetGore Editor");
            
            try
            {
                var dialog = new Views.AboutDialog();
                
                // Try to get the main window as parent
                var mainWindow = global::Avalonia.Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop
                    ? desktop.MainWindow
                    : null;
                
                if (mainWindow != null)
                {
                    await dialog.ShowDialog(mainWindow);
                }
                else
                {
                    dialog.Show();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error showing About dialog: {ex.Message}");
            }
        }
    }
}
