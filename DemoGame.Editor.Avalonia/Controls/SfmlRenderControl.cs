using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Platform;
using Avalonia.Threading;
using SFML.Graphics;
using SFML.Window;

namespace DemoGame.Editor.Avalonia.Controls
{
    /// <summary>
    /// Avalonia control that hosts SFML rendering
    /// Replaces WinForms GraphicsDeviceControl
    /// </summary>
    public class SfmlRenderControl : NativeControlHost
    {
        private RenderWindow? _renderWindow;
        private DispatcherTimer? _renderTimer;
        private bool _isInitialized;
        private bool _isDisposed;
        private IntPtr _nativeHandle;

        /// <summary>
        /// Gets the SFML RenderWindow for drawing
        /// </summary>
        public RenderWindow? RenderWindow => _renderWindow;

        /// <summary>
        /// Event raised when rendering occurs - subscribe to draw custom content
        /// </summary>
        public event EventHandler<RenderEventArgs>? RenderFrame;

        public SfmlRenderControl()
        {
            // Set default size
            Width = 800;
            Height = 600;
        }

        protected override IPlatformHandle CreateNativeControlCore(IPlatformHandle parent)
        {
            // Create a native window handle that SFML can use
            // On Linux, this creates an X11 window
            // On Windows, this creates a Win32 window
            
            var handle = base.CreateNativeControlCore(parent);
            _nativeHandle = handle.Handle;

            Console.WriteLine($"🎨 SfmlRenderControl: Native handle created: {_nativeHandle}");

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
            
            // Initialize SFML after control is attached
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
                Console.WriteLine($"🎨 SfmlRenderControl: Initializing SFML with handle {_nativeHandle}...");

                // Create SFML RenderWindow with the native handle
                _renderWindow = new RenderWindow(_nativeHandle);
                _renderWindow.SetVerticalSyncEnabled(false);
                _renderWindow.SetVisible(true);
                _renderWindow.SetActive(true);

                Console.WriteLine($"✅ SfmlRenderControl: SFML RenderWindow created successfully!");

                // Set up rendering timer (60 FPS)
                _renderTimer = new DispatcherTimer
                {
                    Interval = TimeSpan.FromMilliseconds(1000.0 / 60.0)
                };
                _renderTimer.Tick += RenderTimer_Tick;
                _renderTimer.Start();

                _isInitialized = true;

                Console.WriteLine($"✅ SfmlRenderControl: Rendering loop started at 60 FPS");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ SfmlRenderControl: Error initializing SFML: {ex.Message}");
                Console.WriteLine($"   Stack trace: {ex.StackTrace}");
            }
        }

        private void RenderTimer_Tick(object? sender, EventArgs e)
        {
            if (_renderWindow == null || _isDisposed)
                return;

            try
            {
                // Clear the window
                _renderWindow.Clear(new SFML.Graphics.Color(30, 30, 30));

                // Raise render event for custom drawing
                RenderFrame?.Invoke(this, new RenderEventArgs(_renderWindow));

                // Display
                _renderWindow.Display();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error during SFML rendering: {ex.Message}");
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

            if (_renderWindow != null)
            {
                try
                {
                    _renderWindow.Dispose();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error disposing SFML RenderWindow: {ex.Message}");
                }
                _renderWindow = null;
            }

            _isInitialized = false;
        }
    }

    /// <summary>
    /// Event args for rendering
    /// </summary>
    public class RenderEventArgs : EventArgs
    {
        public RenderWindow RenderWindow { get; }

        public RenderEventArgs(RenderWindow renderWindow)
        {
            RenderWindow = renderWindow;
        }
    }
}

