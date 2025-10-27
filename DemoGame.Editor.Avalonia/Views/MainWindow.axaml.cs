using System.Collections.Specialized;
using Avalonia.Controls;
using DemoGame.Editor.Avalonia.ViewModels;

namespace DemoGame.Editor.Avalonia.Views
{
    public partial class MainWindow : Window
    {
        private MainWindowViewModel? _viewModel;

        public MainWindow()
        {
            InitializeComponent();
            
            // Create and set the DataContext with MainWindowViewModel (includes DockViewModel)
            _viewModel = new MainWindowViewModel();
            DataContext = _viewModel;
            
            // Setup recent maps menu dynamically
            SetupRecentMapsMenu();
            
            // Subscribe to changes in recent maps
            _viewModel.RecentMaps.CollectionChanged += RecentMaps_CollectionChanged;
        }

        private void RecentMaps_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            SetupRecentMapsMenu();
        }

        private void SetupRecentMapsMenu()
        {
            if (_viewModel == null || RecentMapsMenu == null)
                return;

            // Clear existing items
            RecentMapsMenu.Items.Clear();

            // Add recent maps as menu items
            foreach (var map in _viewModel.RecentMaps)
            {
                var menuItem = new MenuItem
                {
                    Header = map.ToString(),
                    Command = _viewModel.OpenRecentMapCommand,
                    CommandParameter = map
                };
                RecentMapsMenu.Items.Add(menuItem);
            }

            // Add "No recent files" if empty
            if (_viewModel.RecentMaps.Count == 0)
            {
                RecentMapsMenu.Items.Add(new MenuItem
                {
                    Header = "(No recent files)",
                    IsEnabled = false
                });
            }
        }
    }
}
