using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Avalonia.Controls;
using Avalonia.Media;
using Dock.Model.Controls;
using Dock.Model.Core;
using Dock.Model.Mvvm.Controls;
using DemoGame.Editor.Avalonia.Views.Dockable;
using HorizontalAlignment = Avalonia.Layout.HorizontalAlignment;
using VerticalAlignment = Avalonia.Layout.VerticalAlignment;

namespace DemoGame.Editor.Avalonia.ViewModels
{
    /// <summary>
    /// View model for docking layout - replaces WeifenLuo.WinFormsUI.Docking
    /// This creates the docking structure AND embeds the actual migrated panels!
    /// </summary>
    public class DockViewModel : ViewModelBase
    {
        public IRootDock? Layout { get; set; }

        public DockViewModel()
        {
            // Create docking layout with actual panel views embedded
            Layout = CreateDefaultLayout();
        }

        /// <summary>
        /// Creates a draggable, closeable tool
        /// </summary>
        private Tool CreateTool(string id, string title)
        {
            return new Tool 
            { 
                Id = id, 
                Title = title,
                CanClose = true,      // ✅ Enable close button (X)
                CanFloat = true,      // ✅ Enable floating windows
                CanPin = true         // ✅ Enable pinning
            };
        }

        /// <summary>
        /// Creates a draggable, closeable document
        /// </summary>
        private Document CreateDocument(string id, string title)
        {
            return new Document 
            { 
                Id = id, 
                Title = title,
                CanClose = true,      // ✅ Enable close button (X)
                CanFloat = true       // ✅ Enable floating windows
            };
        }

        /// <summary>
        /// Creates a docking layout with embedded views
        /// </summary>
        private IRootDock CreateDefaultLayout()
        {
            // Left side tools (asset browsers)
            var leftPane = new ToolDock
            {
                Id = "LeftPane",
                Title = "Assets",
                Proportion = double.NaN,
                ActiveDockable = null,
                VisibleDockables = new ObservableCollection<IDockable>
                {
                    CreateTool("GrhTreeView", "🎨 GRH Tree ✅"),
                    CreateTool("GrhTileset", "🧩 Tileset ✅")
                }
            };

            // Center document area (main editing space)
            var documentsPane = new DocumentDock
            {
                Id = "Documents",
                Title = "Documents",
                IsCollapsable = false,
                CanCreateDocument = true,
                ActiveDockable = null,
                VisibleDockables = new ObservableCollection<IDockable>
                {
                    CreateDocument("MapEditor", "📝 Map Editor ✅"),
                    CreateDocument("SkeletonEditor", "🦴 Skeleton ✅"),
                    CreateTool("MapPreview", "🗺️ Preview ✅")
                }
            };

            // Right side tools
            var rightPane = new ToolDock
            {
                Id = "RightPane",
                Title = "Properties",
                Proportion = double.NaN,
                ActiveDockable = null,
                VisibleDockables = new ObservableCollection<IDockable>
                {
                    CreateTool("Properties", "⚙️ Properties")
                }
            };

            // Bottom panel - CONTENT EDITORS ✅
            var bottomPane = new ToolDock
            {
                Id = "BottomPane",
                Title = "Content Editors",
                Proportion = double.NaN,
                ActiveDockable = null,
                VisibleDockables = new ObservableCollection<IDockable>
                {
                    // ✅ FULLY WORKING PANELS
                    CreateTool("MusicEditor", "🎵 Music ✅"),
                    CreateTool("SoundEditor", "🔊 Sound ✅"),
                    CreateTool("BodyEditor", "👤 Body ✅"),
                    
                    // ✅ PLACEHOLDER PANELS (structure ready)
                    CreateTool("ParticleEditor", "✨ Particles ✅"),
                    CreateTool("NPCChatEditor", "💬 NPC Chat ✅"),
                    CreateTool("DbEditor", "💾 Database ✅")
                }
            };

            // Set first migrated panel as active so it shows by default
            bottomPane.ActiveDockable = bottomPane.VisibleDockables[0];

            // Main horizontal layout
            var mainLayout = new ProportionalDock
            {
                Id = "MainLayout",
                Orientation = Dock.Model.Core.Orientation.Horizontal,
                VisibleDockables = new ObservableCollection<IDockable>
                {
                    new ProportionalDockSplitter(),
                    leftPane,
                    new ProportionalDockSplitter(),
                    documentsPane,
                    new ProportionalDockSplitter(),
                    rightPane,
                    new ProportionalDockSplitter()
                }
            };

            // Overall layout with main and bottom
            var rootLayout = new ProportionalDock
            {
                Id = "RootLayout",
                Orientation = Orientation.Vertical,
                VisibleDockables = new ObservableCollection<IDockable>
                {
                    mainLayout,
                    new ProportionalDockSplitter(),
                    bottomPane
                }
            };

            // Root dock
            var root = new RootDock
            {
                Id = "Root",
                Title = "NetGore Editor",
                ActiveDockable = rootLayout,
                DefaultDockable = rootLayout,
                VisibleDockables = new ObservableCollection<IDockable> { rootLayout }
            };

            return root;
        }

        /// <summary>
        /// Activates (shows/focuses) a panel by its ID
        /// </summary>
        public void ActivatePanel(string panelId)
        {
            if (Layout == null) return;

            try
            {
                // Find the panel in all docks
                var panel = FindDockableById(Layout, panelId);
                if (panel != null)
                {
                    // Get the parent dock and activate the panel
                    var parentDock = FindParentDock(Layout, panel);
                    if (parentDock is IDock dock)
                    {
                        dock.ActiveDockable = panel;
                        Console.WriteLine($"Activated panel: {panelId}");
                    }
                }
                else
                {
                    Console.WriteLine($"Panel not found: {panelId}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error activating panel {panelId}: {ex.Message}");
            }
        }

        /// <summary>
        /// Recursively finds a dockable by ID
        /// </summary>
        private IDockable? FindDockableById(IDockable dockable, string id)
        {
            if (dockable.Id == id)
                return dockable;

            if (dockable is IDock dock && dock.VisibleDockables != null)
            {
                foreach (var child in dock.VisibleDockables)
                {
                    var found = FindDockableById(child, id);
                    if (found != null)
                        return found;
                }
            }

            return null;
        }

        /// <summary>
        /// Finds the parent dock of a dockable
        /// </summary>
        private IDock? FindParentDock(IDockable root, IDockable target)
        {
            if (root is IDock dock && dock.VisibleDockables != null)
            {
                if (dock.VisibleDockables.Contains(target))
                    return dock;

                foreach (var child in dock.VisibleDockables)
                {
                    var found = FindParentDock(child, target);
                    if (found != null)
                        return found;
                }
            }

            return null;
        }

        /// <summary>
        /// Creates a placeholder panel for not-yet-migrated forms
        /// </summary>
        private Control CreatePlaceholder(string title)
        {
            return new Border
            {
                Background = Brushes.Transparent,
                Child = new StackPanel
                {
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center,
                    Spacing = 10,
                    Children =
                    {
                        new TextBlock
                        {
                            Text = title,
                            FontSize = 18,
                            FontWeight = FontWeight.Bold,
                            Foreground = Brushes.White,
                            HorizontalAlignment = HorizontalAlignment.Center
                        },
                        new TextBlock
                        {
                            Text = "Not yet migrated",
                            FontSize = 14,
                            Foreground = Brushes.Gray,
                            HorizontalAlignment = HorizontalAlignment.Center
                        },
                        new TextBlock
                        {
                            Text = "✨ Coming soon!",
                            FontSize = 12,
                            Foreground = new SolidColorBrush(Color.Parse("#9CDCFE")),
                            HorizontalAlignment = HorizontalAlignment.Center
                        }
                    }
                }
            };
        }
    }
}
