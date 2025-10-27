using System;
using Avalonia.Controls;
using Avalonia.Media;
using DemoGame.Editor.Avalonia.Views.Dockable;
using HorizontalAlignment = Avalonia.Layout.HorizontalAlignment;
using VerticalAlignment = Avalonia.Layout.VerticalAlignment;

namespace DemoGame.Editor.Avalonia.ViewModels
{
    /// <summary>
    /// Factory that creates panel views on-demand (lazy loading)
    /// This prevents loading Content data files until panels are actually shown
    /// </summary>
    public static class DockViewFactory
    {
        private static readonly object _lock = new object();
        private static readonly System.Collections.Generic.Dictionary<string, Control> _cachedViews = new();

        /// <summary>
        /// Creates or retrieves a cached view for the given tool ID
        /// </summary>
        public static Control? CreateView(string? id)
        {
            if (string.IsNullOrEmpty(id))
                return null;

            lock (_lock)
            {
                // Return cached view if it exists
                if (_cachedViews.TryGetValue(id, out var cachedView))
                    return cachedView;

                // Create new view based on ID
                Control? view = id switch
                {
                    // ✅ FULLY MIGRATED PANELS (working with business logic)
                    "MusicEditor" => new MusicEditorPanel(),
                    "SoundEditor" => new SoundEditorPanel(),
                    "BodyEditor" => new BodyEditorPanel(),
                    "SelectedObjects" => new SelectedMapObjectsPanel(),
                    "Properties" => new PropertiesPanel(),
                    
                    // ✅ PLACEHOLDER PANELS (structure created, awaiting implementation)
                    "MapPreview" => new MapPreviewPanel(),
                    "NPCChatEditor" => new NPCChatEditorPanel(),
                    "DbEditor" => new DbEditorPanel(),
                    "ParticleEditor" => new ParticleEditorPanel(),
                    "GrhTreeView" => new GrhTreeViewPanel(),
                    "GrhTileset" => new GrhTilesetPanel(),
                    "MapEditor" => new EditMapPanel(),
                    "SkeletonEditor" => new SkeletonEditorPanel(),
                    
                    // 📝 Not yet migrated (will show generic placeholder)
                    _ => CreatePlaceholder(id)
                };

                // Cache it
                if (view != null)
                    _cachedViews[id] = view;

                return view;
            }
        }

        /// <summary>
        /// Creates a placeholder panel for not-yet-migrated forms
        /// </summary>
        private static Control CreatePlaceholder(string id)
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
                            Text = id,
                            FontSize = 18,
                            FontWeight = FontWeight.Bold,
                            Foreground = Brushes.White,
                            HorizontalAlignment = HorizontalAlignment.Center
                        },
                        new TextBlock
                        {
                            Text = "Not yet migrated from WinForms",
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

