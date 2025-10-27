using System;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Platform;
using Avalonia.Threading;
using SFML.Graphics;
using NetGore;
using NetGore.Graphics;
using NetGore.Graphics.ParticleEngine;
using DemoGame.Editor.Avalonia.Services;

namespace DemoGame.Editor.Avalonia.Controls
{
    /// <summary>
    /// SFML control for previewing particle effects
    /// </summary>
    public class ParticlePreviewControl : NativeControlHost
    {
        private RenderWindow? _renderWindow;
        private DispatcherTimer? _renderTimer;
        private IDrawingManager? _drawingManager;
        private ICamera2D? _camera;
        private bool _isInitialized;
        private bool _isDisposed;
        private IntPtr _nativeHandle;
        private TickCount _currentTime;
        
        private IParticleEffect? _currentEffect;

        public ParticlePreviewControl()
        {
            Width = 400;
            Height = 400;
            _currentTime = (TickCount)Environment.TickCount;
        }

        protected override IPlatformHandle CreateNativeControlCore(IPlatformHandle parent)
        {
            var handle = base.CreateNativeControlCore(parent);
            _nativeHandle = handle.Handle;
            Console.WriteLine($"✨ ParticlePreviewControl: Native handle created: {_nativeHandle}");
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
                Console.WriteLine($"✨ ParticlePreviewControl: Initializing SFML...");

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
                Console.WriteLine($"✅ ParticlePreviewControl: Initialization complete!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ ParticlePreviewControl: Error initializing: {ex.Message}");
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

                _renderWindow.Clear(new SFML.Graphics.Color(20, 20, 20));

                var spriteBatch = _drawingManager.BeginDrawWorld(_camera);
                if (spriteBatch != null)
                {
                    try
                    {
                        if (_currentEffect != null)
                        {
                            // Update and draw particle effect at center
                            _currentEffect.Update(_currentTime);
                            _currentEffect.Draw(spriteBatch);
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
                Console.WriteLine($"Error during particle preview rendering: {ex.Message}");
            }
        }

        private void DrawPlaceholder()
        {
            if (_renderWindow == null)
                return;

            var circle = new CircleShape(50)
            {
                Position = new SFML.System.Vector2f(150, 150),
                FillColor = new SFML.Graphics.Color(100, 100, 100, 100),
                OutlineColor = new SFML.Graphics.Color(150, 150, 150),
                OutlineThickness = 2
            };
            _renderWindow.Draw(circle);
        }

        /// <summary>
        /// Sets the particle effect to preview
        /// </summary>
        public void SetEffect(IParticleEffect? effect)
        {
            _currentEffect?.Dispose();
            _currentEffect = effect;
            
            // Center the effect
            if (_currentEffect != null && _camera != null)
            {
                _currentEffect.Position = _camera.Center;
                Console.WriteLine($"✨ ParticlePreviewControl: Set particle effect at {_currentEffect.Position}");
            }
        }

        private void CleanupSfml()
        {
            _isDisposed = true;

            _renderTimer?.Stop();
            _renderTimer = null;

            _currentEffect?.Dispose();
            _currentEffect = null;

            _drawingManager?.Dispose();
            _drawingManager = null;

            _renderWindow?.Dispose();
            _renderWindow = null;

            _isInitialized = false;
        }
    }
}

