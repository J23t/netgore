using System;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Input;
using DemoGame.Editor.Avalonia.Services;
using NetGore;
using NetGore.World;

namespace DemoGame.Editor.Avalonia.Views
{
    /// <summary>
    /// Dialog for selecting a map to open
    /// </summary>
    public partial class MapSelectionDialog : Window
    {
        public MapID? SelectedMapID { get; private set; }

        public MapSelectionDialog()
        {
            InitializeComponent();
            LoadMaps();
        }

        private void LoadMaps()
        {
            try
            {
                var maps = MapService.GetAllMaps();
                MapListBox.ItemsSource = maps;

                if (maps.Count > 0)
                {
                    MapListBox.SelectedIndex = 0;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading map list: {ex.Message}");
            }
        }

        private void OpenButton_Click(object? sender, RoutedEventArgs e)
        {
            if (MapListBox.SelectedItem is MapListItem selectedMap)
            {
                SelectedMapID = selectedMap.ID;
                Close(true);
            }
        }

        private void CancelButton_Click(object? sender, RoutedEventArgs e)
        {
            SelectedMapID = null;
            Close(false);
        }

        private void MapList_DoubleTapped(object? sender, TappedEventArgs e)
        {
            // Double-click to open
            if (MapListBox.SelectedItem is MapListItem selectedMap)
            {
                SelectedMapID = selectedMap.ID;
                Close(true);
            }
        }
    }
}

