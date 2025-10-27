using System;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Platform;
using Avalonia.Threading;
using SFML.Graphics;
using SFML.Window;
using NetGore;
using NetGore.Graphics;
using DemoGame.Editor.Avalonia.Services;

namespace DemoGame.Editor.Avalonia.Controls
{
    /// <summary>
    /// SFML control for previewing GRH graphics
    /// </summary>
    public class GrhPreviewControl : NativeControlHost
    {
        private RenderWindow? _renderWindow;
        private DispatcherTimer? _renderTimer;
        private IDrawingManager? _drawingManager;
        private ICamera2D? _camera;
        private bool _isInitialized;
        private bool _isDisposed;
        private IntPtr _nativeHandle;
        private TickCount _currentTime;
        
        private Grh? _currentGrh;
        private GrhData? _currentGrhData;

        /// <summary>
        /// Gets the SFML RenderWindow for drawing
        /// </summary>
        public RenderWindow? RenderWindow => _renderWindow;

        public GrhPreviewControl()
        {
            Width = 400;
            Height = 400;
            _currentTime = (TickCount)Environment.TickCount;
        }

        protected override IPlatformHandle CreateNativeControlCore(IPlatformHandle parent)
        {
            var handle = base.CreateNativeControlCore(parent);
            _nativeHandle = handle.Handle;
            Console.WriteLine($"🎨 GrhPreviewControl: Native handle created: {_nativeHandle}");
            return handle;
        }

        protected override void DestroyNativeControlCore(IPlatformHandle control)
        {
            CleanupSfml();
            base.DestroyNativeControlCore(control);
        }

        protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
        {
            base.OnAttachedToVisualTree(e);
            Dispatcher.UIThread.Post(InitializeSfml, DispatcherPriority.Loaded);
        }

        protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
        {
            CleanupSfml();
            base.OnDetachedFromVisualTree(e);
        }

        private void InitializeSfml()
        {
            if (_isInitialized || _nativeHandle == IntPtr.Zero)
                return;

            try
            {
                Console.WriteLine($"🎨 GrhPreviewControl: Initializing SFML...");

                // Create SFML RenderWindow
                _renderWindow = new RenderWindow(_nativeHandle);
                _renderWindow.SetVerticalSyncEnabled(false);
                _renderWindow.SetVisible(true);
                _renderWindow.SetActive(true);

                // Create drawing manager
                if (ContentService.IsInitialized)
                {
                    _drawingManager = ContentService.CreateDrawingManager(_renderWindow);
                }

                // Create camera centered on preview
                _camera = new Camera2D(new Vector2(200, 200))
                {
                    Size = new Vector2(400, 400)
                };

                // Set up rendering timer (60 FPS)
                _renderTimer = new DispatcherTimer
                {
                    Interval = TimeSpan.FromMilliseconds(1000.0 / 60.0)
                };
                _renderTimer.Tick += RenderTimer_Tick;
                _renderTimer.Start();

                _isInitialized = true;
                Console.WriteLine($"✅ GrhPreviewControl: Initialization complete!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ GrhPreviewControl: Error initializing: {ex.Message}");
            }
        }

        private void RenderTimer_Tick(object? sender, EventArgs e)
        {
            if (_renderWindow == null || _isDisposed || _drawingManager == null)
                return;

            try
            {
                // Update time
                _currentTime = (TickCount)Environment.TickCount;

                // Update drawing manager
                _drawingManager.Update(_currentTime);

                // Clear with dark background
                _renderWindow.Clear(new SFML.Graphics.Color(40, 40, 40));

                // Begin drawing
                var spriteBatch = _drawingManager.BeginDrawWorld(_camera);
                if (spriteBatch != null)
                {
                    try
                    {
                        if (_currentGrh != null)
                        {
                            // Update animation
                            _currentGrh.Update(_currentTime);
                            
                            // Center the GRH in the preview
                            var size = _currentGrh.Size;
                            var centerPos = new Vector2(
                                (_camera!.Size.X - size.X) / 2,
                                (_camera.Size.Y - size.Y) / 2
                            );
                            
                            // Draw the GRH
                            _currentGrh.Draw(spriteBatch, centerPos);
                        }
                        else
                        {
                            // Draw placeholder text
                            DrawPlaceholder();
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error drawing GRH: {ex.Message}");
                    }
                    finally
                    {
                        _drawingManager.EndDrawWorld();
                    }
                }

                // Display
                _renderWindow.Display();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error during GRH preview rendering: {ex.Message}");
            }
        }

        private void DrawPlaceholder()
        {
            if (_renderWindow == null)
                return;

            // Draw a simple rectangle as placeholder
            var rect = new RectangleShape(new SFML.System.Vector2f(100, 100))
            {
                Position = new SFML.System.Vector2f(150, 150),
                FillColor = new SFML.Graphics.Color(80, 80, 80),
                OutlineColor = new SFML.Graphics.Color(120, 120, 120),
                OutlineThickness = 2
            };
            _renderWindow.Draw(rect);
        }

        /// <summary>
        /// Sets the GRH to preview
        /// </summary>
        public void SetGrh(GrhData? grhData)
        {
            _currentGrhData = grhData;
            
            if (grhData != null)
            {
                try
                {
                    _currentGrh = new Grh(grhData, AnimType.Loop, _currentTime);
                    Console.WriteLine($"🎨 GrhPreviewControl: Set GRH {grhData.GrhIndex} - {grhData.Categorization}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"⚠️ GrhPreviewControl: Error creating Grh: {ex.Message}");
                    _currentGrh = null;
                }
            }
            else
            {
                _currentGrh = null;
            }
        }

        private void CleanupSfml()
        {
            _isDisposed = true;

            if (_renderTimer != null)
            {
                _renderTimer.Stop();
                _renderTimer = null;
            }

            _drawingManager?.Dispose();
            _drawingManager = null;

            if (_renderWindow != null)
            {
                try
                {
                    _renderWindow.Dispose();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error disposing RenderWindow: {ex.Message}");
                }
                _renderWindow = null;
            }

            _isInitialized = false;
        }
    }
}

