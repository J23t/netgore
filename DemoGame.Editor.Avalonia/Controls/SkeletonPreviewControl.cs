using System;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Platform;
using Avalonia.Threading;
using SFML.Graphics;
using NetGore;
using NetGore.Graphics;
using DemoGame.Editor.Avalonia.Services;

namespace DemoGame.Editor.Avalonia.Controls
{
    /// <summary>
    /// SFML control for previewing skeleton animations
    /// </summary>
    public class SkeletonPreviewControl : NativeControlHost
    {
        private RenderWindow? _renderWindow;
        private DispatcherTimer? _renderTimer;
        private IDrawingManager? _drawingManager;
        private ICamera2D? _camera;
        private bool _isInitialized;
        private bool _isDisposed;
        private IntPtr _nativeHandle;
        private TickCount _currentTime;
        
        private SkeletonAnimation? _currentSkeleton;

        public SkeletonPreviewControl()
        {
            Width = 400;
            Height = 400;
            _currentTime = (TickCount)Environment.TickCount;
        }

        protected override IPlatformHandle CreateNativeControlCore(IPlatformHandle parent)
        {
            var handle = base.CreateNativeControlCore(parent);
            _nativeHandle = handle.Handle;
            Console.WriteLine($"🦴 SkeletonPreviewControl: Native handle created: {_nativeHandle}");
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
                Console.WriteLine($"🦴 SkeletonPreviewControl: Initializing SFML...");

                _renderWindow = new RenderWindow(_nativeHandle);
                _renderWindow.SetVerticalSyncEnabled(false);
                _renderWindow.SetVisible(true);
                _renderWindow.SetActive(true);

                if (ContentService.IsInitialized)
                {
                    _drawingManager = ContentService.CreateDrawingManager(_renderWindow);
                }

                _camera = new Camera2D(new Vector2(200, 200))
                {
                    Size = new Vector2(400, 400)
                };

                _renderTimer = new DispatcherTimer
                {
                    Interval = TimeSpan.FromMilliseconds(1000.0 / 60.0)
                };
                _renderTimer.Tick += RenderTimer_Tick;
                _renderTimer.Start();

                _isInitialized = true;
                Console.WriteLine($"✅ SkeletonPreviewControl: Initialization complete!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ SkeletonPreviewControl: Error initializing: {ex.Message}");
            }
        }

        private void RenderTimer_Tick(object? sender, EventArgs e)
        {
            if (_renderWindow == null || _isDisposed || _drawingManager == null)
                return;

            try
            {
                _currentTime = (TickCount)Environment.TickCount;
                _drawingManager.Update(_currentTime);

                _renderWindow.Clear(new SFML.Graphics.Color(30, 30, 40));

                var spriteBatch = _drawingManager.BeginDrawWorld(_camera);
                if (spriteBatch != null)
                {
                    try
                    {
                        if (_currentSkeleton != null)
                        {
                            // Update and draw skeleton at center
                            _currentSkeleton.Update(_currentTime);
                            
                            var centerPos = new Vector2(
                                _camera!.Center.X,
                                _camera.Center.Y
                            );
                            
                            // Draw skeleton at center position
                            _currentSkeleton.Draw(spriteBatch, centerPos, new NetGore.Color(255, 255, 255, 255), SpriteEffects.None);
                        }
                        else
                        {
                            DrawPlaceholder();
                        }
                    }
                    finally
                    {
                        _drawingManager.EndDrawWorld();
                    }
                }

                _renderWindow.Display();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error during skeleton preview rendering: {ex.Message}");
            }
        }

        private void DrawPlaceholder()
        {
            if (_renderWindow == null)
                return;

            var rect = new RectangleShape(new SFML.System.Vector2f(80, 120))
            {
                Position = new SFML.System.Vector2f(160, 140),
                FillColor = new SFML.Graphics.Color(70, 70, 90, 150),
                OutlineColor = new SFML.Graphics.Color(120, 120, 140),
                OutlineThickness = 2
            };
            _renderWindow.Draw(rect);
        }

        /// <summary>
        /// Sets the skeleton to preview
        /// </summary>
        public void SetSkeleton(SkeletonAnimation? skeleton)
        {
            _currentSkeleton = skeleton;
            
            if (_currentSkeleton != null)
            {
                Console.WriteLine($"🦴 SkeletonPreviewControl: Set skeleton");
            }
        }

        private void CleanupSfml()
        {
            _isDisposed = true;

            _renderTimer?.Stop();
            _renderTimer = null;

            _drawingManager?.Dispose();
            _drawingManager = null;

            _renderWindow?.Dispose();
            _renderWindow = null;

            _isInitialized = false;
        }
    }
}

