using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using NetGore;
using NetGore.Content;
using NetGore.Graphics;

namespace DemoGame.Editor.Avalonia.ViewModels.Dockable
{
    /// <summary>
    /// ViewModel for GRH Tree View - browse and select graphics resources
    /// Replaces WinForms GrhTreeViewForm
    /// </summary>
    public partial class GrhTreeViewViewModel : ViewModelBase
    {
        [ObservableProperty]
        private string _filterText = string.Empty;

        [ObservableProperty]
        private ObservableCollection<GrhTreeNode> _rootNodes = new();

        [ObservableProperty]
        private GrhTreeNode? _selectedNode;

        [ObservableProperty]
        private GrhData? _selectedGrhData;

        [ObservableProperty]
        private string _statusText = "Ready";

        private IContentManager? _contentManager;

        public GrhTreeViewViewModel()
        {
            // Initialize with placeholder data for now
            StatusText = "Initializing GRH tree...";
            
            try
            {
                Initialize();
            }
            catch (Exception ex)
            {
                StatusText = $"Error: {ex.Message}";
            }
        }

        private void Initialize()
        {
            try
            {
                // Try to get content manager from GlobalState if available
                var globalStateType = Type.GetType("DemoGame.Editor.GlobalState, DemoGame.Editor");
                if (globalStateType != null)
                {
                    var instanceProp = globalStateType.GetProperty("Instance", 
                        System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static);
                    if (instanceProp != null)
                    {
                        var instance = instanceProp.GetValue(null);
                        if (instance != null)
                        {
                            var cmProp = globalStateType.GetProperty("ContentManager");
                            if (cmProp != null)
                            {
                                _contentManager = cmProp.GetValue(instance) as IContentManager;
                            }
                        }
                    }
                }

                if (_contentManager != null)
                {
                    LoadGrhTree();
                    StatusText = $"Loaded {CountGrhData()} GRH(s)";
                }
                else
                {
                    StatusText = "Content manager not available";
                    LoadPlaceholderData();
                }
            }
            catch (Exception ex)
            {
                StatusText = $"Error initializing: {ex.Message}";
                LoadPlaceholderData();
            }
        }

        private void LoadGrhTree()
        {
            RootNodes.Clear();
            
            try
            {
                // Get all GrhData
                var allGrhData = GrhInfo.GrhDatas.Where(x => x != null).ToList();
                
                // Group by category
                var rootCategories = allGrhData
                    .Where(g => g.Categorization != null && !g.Categorization.Category.ToString().Contains("/"))
                    .GroupBy(g => g.Categorization.Category.ToString().Split('/')[0])
                    .OrderBy(g => g.Key);

                foreach (var category in rootCategories)
                {
                    var node = new GrhTreeNode(category.Key);
                    BuildTreeRecursive(node, allGrhData, category.Key);
                    RootNodes.Add(node);
                }
            }
            catch (Exception ex)
            {
                StatusText = $"Error loading GRH tree: {ex.Message}";
            }
        }

        private void BuildTreeRecursive(GrhTreeNode parent, System.Collections.Generic.List<GrhData> allData, string categoryPath)
        {
            // Add GRH data at this level
            var dataAtLevel = allData
                .Where(g => g.Categorization != null && 
                           g.Categorization.Category.ToString() == categoryPath)
                .OrderBy(g => g.Categorization.Title.ToString());

            foreach (var grhData in dataAtLevel)
            {
                parent.Children.Add(new GrhTreeNode(grhData.Categorization.Title.ToString(), grhData));
            }

            // Add subcategories
            var subcategories = allData
                .Where(g => g.Categorization != null && 
                           g.Categorization.Category.ToString().StartsWith(categoryPath + "/") &&
                           g.Categorization.Category.ToString().Substring(categoryPath.Length + 1).Split('/').Length == 1)
                .Select(g => g.Categorization.Category.ToString())
                .Distinct()
                .OrderBy(c => c);

            foreach (var subcat in subcategories)
            {
                var node = new GrhTreeNode(subcat.Split('/').Last());
                BuildTreeRecursive(node, allData, subcat);
                parent.Children.Add(node);
            }
        }

        private void LoadPlaceholderData()
        {
            RootNodes.Clear();
            
            var charactersNode = new GrhTreeNode("Characters");
            charactersNode.Children.Add(new GrhTreeNode("Player", null));
            charactersNode.Children.Add(new GrhTreeNode("Enemy", null));
            RootNodes.Add(charactersNode);

            var itemsNode = new GrhTreeNode("Items");
            itemsNode.Children.Add(new GrhTreeNode("Weapons", null));
            itemsNode.Children.Add(new GrhTreeNode("Armor", null));
            RootNodes.Add(itemsNode);

            var tilesNode = new GrhTreeNode("Tiles");
            tilesNode.Children.Add(new GrhTreeNode("Ground", null));
            tilesNode.Children.Add(new GrhTreeNode("Wall", null));
            RootNodes.Add(tilesNode);
        }

        private int CountGrhData()
        {
            int count = 0;
            foreach (var root in RootNodes)
            {
                count += CountNodes(root);
            }
            return count;
        }

        private int CountNodes(GrhTreeNode node)
        {
            int count = node.GrhData != null ? 1 : 0;
            foreach (var child in node.Children)
            {
                count += CountNodes(child);
            }
            return count;
        }

        partial void OnFilterTextChanged(string value)
        {
            // Apply filter to tree
            // For now, just note in status
            if (!string.IsNullOrWhiteSpace(value))
            {
                StatusText = $"Filtering by: {value}";
            }
            else
            {
                StatusText = $"Loaded {CountGrhData()} GRH(s)";
            }
        }

        partial void OnSelectedNodeChanged(GrhTreeNode? value)
        {
            SelectedGrhData = value?.GrhData;
            
            if (value != null)
            {
                if (value.GrhData != null)
                {
                    StatusText = $"Selected: {value.Name} (GRH {value.GrhData.GrhIndex})";
                    
                    // Update GlobalState if available
                    UpdateGlobalStateGrh(value.GrhData);
                    
                    // Notify other panels of selection (for Properties panel, etc.)
                    SelectionChanged?.Invoke(this, value.GrhData);
                }
                else
                {
                    StatusText = $"Selected folder: {value.Name}";
                    SelectionChanged?.Invoke(this, null);
                }
            }
        }

        /// <summary>
        /// Event raised when selection changes - other panels can subscribe to this
        /// </summary>
        public event EventHandler<object?>? SelectionChanged;

        private void UpdateGlobalStateGrh(GrhData grhData)
        {
            try
            {
                // Try to update GlobalState.Instance.Map.GrhToPlace
                var globalStateType = Type.GetType("DemoGame.Editor.GlobalState, DemoGame.Editor");
                if (globalStateType != null)
                {
                    var instanceProp = globalStateType.GetProperty("Instance", 
                        System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static);
                    if (instanceProp != null)
                    {
                        var instance = instanceProp.GetValue(null);
                        if (instance != null)
                        {
                            var mapProp = globalStateType.GetProperty("Map");
                            if (mapProp != null)
                            {
                                var map = mapProp.GetValue(instance);
                                if (map != null)
                                {
                                    var method = map.GetType().GetMethod("SetGrhToPlace");
                                    if (method != null)
                                    {
                                        method.Invoke(map, new object[] { grhData.GrhIndex });
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch
            {
                // Silently fail - GlobalState might not be available yet
            }
        }

        [RelayCommand]
        private void Refresh()
        {
            Initialize();
        }

        [RelayCommand]
        private void NewGrh()
        {
            StatusText = "Create new GRH (not implemented yet)";
        }

        [RelayCommand]
        private void EditGrh()
        {
            if (SelectedGrhData != null)
            {
                StatusText = $"Edit GRH {SelectedGrhData.GrhIndex} (not implemented yet)";
            }
        }

        [RelayCommand]
        private void DeleteGrh()
        {
            if (SelectedGrhData != null)
            {
                StatusText = $"Delete GRH {SelectedGrhData.GrhIndex} (not implemented yet)";
            }
        }
    }

    /// <summary>
    /// Tree node for GRH hierarchy
    /// </summary>
    public partial class GrhTreeNode : ObservableObject
    {
        public string Name { get; }
        public GrhData? GrhData { get; }
        public ObservableCollection<GrhTreeNode> Children { get; } = new();

        [ObservableProperty]
        private bool _isExpanded;

        [ObservableProperty]
        private bool _isSelected;

        public GrhTreeNode(string name, GrhData? grhData = null)
        {
            Name = name;
            GrhData = grhData;
        }
    }
}

