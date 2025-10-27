using System;
using Avalonia.Controls;
using DemoGame.Editor.Avalonia.ViewModels.Dockable;
using DemoGame.Editor.Avalonia.Controls;
using SFML.Graphics;

namespace DemoGame.Editor.Avalonia.Views.Dockable
{
    /// <summary>
    /// Skeleton Editor Panel - Character animation editor with SFML preview
    /// </summary>
    public partial class SkeletonEditorPanel : UserControl
    {
        public SkeletonEditorPanel()
        {
            InitializeComponent();
            DataContext = new SkeletonEditorViewModel();
            
            // Setup SFML rendering for skeleton preview
            SetupSkeletonRendering();
        }

        /// <summary>
        /// Gets the ViewModel for external access
        /// </summary>
        public SkeletonEditorViewModel? ViewModel => DataContext as SkeletonEditorViewModel;

        private void SetupSkeletonRendering()
        {
            try
            {
                // Subscribe to SFML rendering
                SkeletonSfmlControl.RenderFrame += SkeletonSfml_RenderFrame;
                
                Console.WriteLine("✅ SkeletonEditorPanel: SFML rendering hooked up");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"⚠️ SkeletonEditorPanel: Error setting up SFML: {ex.Message}");
            }
        }

        private void SkeletonSfml_RenderFrame(object? sender, RenderEventArgs e)
        {
            try
            {
                var rw = e.RenderWindow;
                
                // Draw skeleton preview (test pattern for now)
                DrawSkeletonPreview(rw);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in skeleton SFML render: {ex.Message}");
            }
        }

        private void DrawSkeletonPreview(RenderWindow rw)
        {
            // Draw a simple cross pattern to represent skeleton structure
            var centerX = rw.Size.X / 2;
            var centerY = rw.Size.Y / 2;
            var boneColor = new SFML.Graphics.Color(100, 100, 200);
            var jointColor = new SFML.Graphics.Color(200, 100, 100);
            
            // Vertical bone
            var verticalBone = new RectangleShape(new SFML.System.Vector2f(4, 150))
            {
                Position = new SFML.System.Vector2f(centerX - 2, centerY - 75),
                FillColor = boneColor
            };
            rw.Draw(verticalBone);
            
            // Horizontal bone
            var horizontalBone = new RectangleShape(new SFML.System.Vector2f(150, 4))
            {
                Position = new SFML.System.Vector2f(centerX - 75, centerY - 2),
                FillColor = boneColor
            };
            rw.Draw(horizontalBone);
            
            // Center joint
            var centerJoint = new CircleShape(8)
            {
                Position = new SFML.System.Vector2f(centerX - 8, centerY - 8),
                FillColor = jointColor,
                OutlineColor = new SFML.Graphics.Color(255, 150, 150),
                OutlineThickness = 2
            };
            rw.Draw(centerJoint);
            
            // End joints
            var joints = new[]
            {
                new SFML.System.Vector2f(centerX, centerY - 75),  // Top
                new SFML.System.Vector2f(centerX, centerY + 75),  // Bottom
                new SFML.System.Vector2f(centerX - 75, centerY),  // Left
                new SFML.System.Vector2f(centerX + 75, centerY),  // Right
            };
            
            foreach (var pos in joints)
            {
                var joint = new CircleShape(6)
                {
                    Position = new SFML.System.Vector2f(pos.X - 6, pos.Y - 6),
                    FillColor = jointColor
                };
                rw.Draw(joint);
            }
        }
    }
}

