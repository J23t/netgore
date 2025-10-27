using System;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Platform;
using Avalonia.Threading;
using SFML.Graphics;
using SFML.Window;
using SFML.System;
using NetGore;
using NetGore.Graphics;
using NetGore.World;
using DemoGame.Editor.Avalonia.Services;

namespace DemoGame.Editor.Avalonia.Controls
{
    /// <summary>
    /// Advanced SFML control for rendering NetGore maps
    /// </summary>
    public class MapRenderControl : NativeControlHost
    {
        private RenderWindow? _renderWindow;
        private DispatcherTimer? _renderTimer;
        private IDrawingManager? _drawingManager;
        private ICamera2D? _camera;
        private bool _isInitialized;
        private bool _isDisposed;
        private IntPtr _nativeHandle;
        private TickCount _currentTime;

        // Rendering options
        private bool _showGrid = true;
        private bool _showWalls = true;
        private bool _showEntities = true;
        private float _zoom = 1.0f;

        // Map data
        private object? _currentMap;  // Will be EditorMap when available

        /// <summary>
        /// Gets the SFML RenderWindow for drawing
        /// </summary>
        public RenderWindow? RenderWindow => _renderWindow;

        /// <summary>
        /// Gets the drawing manager
        /// </summary>
        public IDrawingManager? DrawingManager => _drawingManager;

        /// <summary>
        /// Gets/sets whether to show grid
        /// </summary>
        public bool ShowGrid
        {
            get => _showGrid;
            set => _showGrid = value;
        }

        /// <summary>
        /// Gets/sets whether to show walls
        /// </summary>
        public bool ShowWalls
        {
            get => _showWalls;
            set => _showWalls = value;
        }

        /// <summary>
        /// Gets/sets whether to show entities
        /// </summary>
        public bool ShowEntities
        {
            get => _showEntities;
            set => _showEntities = value;
        }

        /// <summary>
        /// Gets/sets zoom level
        /// </summary>
        public float Zoom
        {
            get => _zoom;
            set => _zoom = Math.Max(0.1f, Math.Min(4.0f, value));
        }

        public MapRenderControl()
        {
            Width = 800;
            Height = 600;
            _currentTime = (TickCount)Environment.TickCount;
        }

        protected override IPlatformHandle CreateNativeControlCore(IPlatformHandle parent)
        {
            var handle = base.CreateNativeControlCore(parent);
            _nativeHandle = handle.Handle;
            Console.WriteLine($"🗺️ MapRenderControl: Native handle created: {_nativeHandle}");
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
                Console.WriteLine($"🗺️ MapRenderControl: Initializing SFML with handle {_nativeHandle}...");

                // Create SFML RenderWindow
                _renderWindow = new RenderWindow(_nativeHandle);
                _renderWindow.SetVerticalSyncEnabled(false);
                _renderWindow.SetVisible(true);
                _renderWindow.SetActive(true);

                Console.WriteLine($"✅ MapRenderControl: SFML RenderWindow created");

                // Create drawing manager
                if (ContentService.IsInitialized)
                {
                    _drawingManager = ContentService.CreateDrawingManager(_renderWindow);
                    Console.WriteLine($"✅ MapRenderControl: DrawingManager created");
                }

                // Create camera
                _camera = new Camera2D(new Vector2(400, 300))
                {
                    Size = new Vector2(800, 600)
                };
                Console.WriteLine($"✅ MapRenderControl: Camera created");

                // Set up rendering timer (60 FPS)
                _renderTimer = new DispatcherTimer
                {
                    Interval = TimeSpan.FromMilliseconds(1000.0 / 60.0)
                };
                _renderTimer.Tick += RenderTimer_Tick;
                _renderTimer.Start();

                _isInitialized = true;
                Console.WriteLine($"✅ MapRenderControl: Initialization complete!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ MapRenderControl: Error initializing: {ex.Message}");
                Console.WriteLine($"   Stack trace: {ex.StackTrace}");
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

                // Clear
                _renderWindow.Clear(new SFML.Graphics.Color(30, 30, 30));

                // Begin drawing
                var spriteBatch = _drawingManager.BeginDrawWorld(_camera);
                if (spriteBatch != null)
                {
                    try
                    {
                        // Draw map if available
                        if (_currentMap != null)
                        {
                            DrawMap(spriteBatch);
                        }
                        else
                        {
                            // Draw placeholder
                            DrawPlaceholder(spriteBatch);
                        }
                    }
                    finally
                    {
                        _drawingManager.EndDrawWorld();
                    }
                }

                // Draw UI overlays (grid, etc.)
                DrawOverlays();

                // Display
                _renderWindow.Display();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error during map rendering: {ex.Message}");
            }
        }

        private void DrawMap(ISpriteBatch spriteBatch)
        {
            // TODO: Implement actual map drawing when EditorMap is available
            // For now, draw a test pattern
            DrawTestPattern(spriteBatch);
        }

        private void DrawTestPattern(ISpriteBatch spriteBatch)
        {
            if (_renderWindow == null)
                return;

            // Draw some test GRHs if available
            if (GrhInfo.GrhDatas.Any())
            {
                int grhIndex = 0;
                for (int y = 0; y < 10; y++)
                {
                    for (int x = 0; x < 10; x++)
                    {
                        var grh = new Grh(GrhInfo.GetData((GrhIndex)(grhIndex % 100 + 1)), AnimType.Loop, _currentTime);
                        grh.Update(_currentTime);
                        
                        try
                        {
                            grh.Draw(spriteBatch, new Vector2(x * 64, y * 64));
                        }
                        catch
                        {
                            // Skip invalid GRHs
                        }
                        grhIndex++;
                    }
                }
            }
        }

        private void DrawPlaceholder(ISpriteBatch spriteBatch)
        {
            // Draw a simple "No Map Loaded" message using shapes
            // (Would normally use text but that requires font setup)
        }

        private void DrawOverlays()
        {
            if (_renderWindow == null)
                return;

            // Draw grid if enabled
            if (_showGrid)
            {
                DrawGrid();
            }
        }

        private void DrawGrid()
        {
            if (_renderWindow == null)
                return;

            var gridSize = 32f;
            var gridColor = new SFML.Graphics.Color(60, 60, 60, 128);

            // Vertical lines
            for (float x = 0; x < _renderWindow.Size.X; x += gridSize)
            {
                var line = new RectangleShape(new Vector2f(1, _renderWindow.Size.Y))
                {
                    Position = new Vector2f(x, 0),
                    FillColor = gridColor
                };
                _renderWindow.Draw(line);
            }

            // Horizontal lines
            for (float y = 0; y < _renderWindow.Size.Y; y += gridSize)
            {
                var line = new RectangleShape(new Vector2f(_renderWindow.Size.X, 1))
                {
                    Position = new Vector2f(0, y),
                    FillColor = gridColor
                };
                _renderWindow.Draw(line);
            }
        }

        /// <summary>
        /// Sets the map to render
        /// </summary>
        public void SetMap(object? map)
        {
            _currentMap = map;
            Console.WriteLine($"🗺️ MapRenderControl: Map set: {map != null}");
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

