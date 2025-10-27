using System;
using Avalonia.Controls;
using DemoGame.Editor.Avalonia.ViewModels.Dockable;
using DemoGame.Editor.Avalonia.Controls;
using SFML.Graphics;

namespace DemoGame.Editor.Avalonia.Views.Dockable
{
    /// <summary>
    /// Map Editor Panel - Main map editing interface with SFML rendering
    /// </summary>
    public partial class EditMapPanel : UserControl
    {
        public EditMapPanel()
        {
            InitializeComponent();
            DataContext = new EditMapViewModel();
            
            // Hook up SFML rendering
            SetupSfmlRendering();
        }

        /// <summary>
        /// Gets the ViewModel for external access
        /// </summary>
        public EditMapViewModel? ViewModel => DataContext as EditMapViewModel;

        private void SetupSfmlRendering()
        {
            try
            {
                // Subscribe to SFML rendering events
                SfmlControl.RenderFrame += SfmlControl_RenderFrame;
                
                Console.WriteLine("✅ EditMapPanel: SFML rendering hooked up");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"⚠️ EditMapPanel: Error setting up SFML: {ex.Message}");
            }
        }

        private void SfmlControl_RenderFrame(object? sender, RenderEventArgs e)
        {
            try
            {
                var rw = e.RenderWindow;
                
                // Draw a simple test pattern to verify SFML is working
                DrawTestPattern(rw);
                
                // TODO: Integrate full map drawing when EditorMap is available
                // DrawMap(rw, ViewModel?.GetCurrentMap());
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in SFML render: {ex.Message}");
            }
        }

        private void DrawTestPattern(RenderWindow rw)
        {
            // Draw a grid to verify SFML is working
            var gridSize = 50f;
            var gridColor = new SFML.Graphics.Color(60, 60, 60);
            
            for (int x = 0; x < rw.Size.X; x += (int)gridSize)
            {
                var line = new RectangleShape(new SFML.System.Vector2f(1, rw.Size.Y))
                {
                    Position = new SFML.System.Vector2f(x, 0),
                    FillColor = gridColor
                };
                rw.Draw(line);
            }
            
            for (int y = 0; y < rw.Size.Y; y += (int)gridSize)
            {
                var line = new RectangleShape(new SFML.System.Vector2f(rw.Size.X, 1))
                {
                    Position = new SFML.System.Vector2f(0, y),
                    FillColor = gridColor
                };
                rw.Draw(line);
            }
            
            // Draw "SFML Rendering Active" text
            // (Would use SFML.Graphics.Text but needs font - using shape for now)
            var centerRect = new RectangleShape(new SFML.System.Vector2f(200, 100))
            {
                Position = new SFML.System.Vector2f(rw.Size.X / 2 - 100, rw.Size.Y / 2 - 50),
                FillColor = new SFML.Graphics.Color(40, 120, 40, 200),
                OutlineColor = new SFML.Graphics.Color(0, 255, 0),
                OutlineThickness = 2
            };
            rw.Draw(centerRect);
        }
    }
}

