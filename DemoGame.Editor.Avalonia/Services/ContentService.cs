using System;
using System.IO;
using System.Linq;
using NetGore;
using NetGore.Audio;
using NetGore.Content;
using NetGore.Graphics;
using NetGore.IO;
using DemoGame;

namespace DemoGame.Editor.Avalonia.Services
{
    /// <summary>
    /// Service for initializing and managing NetGore content system
    /// </summary>
    public static class ContentService
    {
        private static bool _isInitialized = false;
        private static IContentManager? _contentManager;
        private static IDrawingManager? _drawingManager;

        /// <summary>
        /// Gets whether the content system has been initialized
        /// </summary>
        public static bool IsInitialized => _isInitialized;

        /// <summary>
        /// Gets the content manager instance
        /// </summary>
        public static IContentManager? ContentManager => _contentManager;

        /// <summary>
        /// Gets the drawing manager instance
        /// </summary>
        public static IDrawingManager? DrawingManager => _drawingManager;

        /// <summary>
        /// Initializes the NetGore content system
        /// </summary>
        public static bool Initialize()
        {
            if (_isInitialized)
                return true;

            try
            {
                Console.WriteLine("🎮 ContentService: Initializing NetGore content system...");

                // Set up content paths
                var contentPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Content");
                Console.WriteLine($"   Content path: {contentPath}");

                if (!Directory.Exists(contentPath))
                {
                    Console.WriteLine($"⚠️  Content directory not found: {contentPath}");
                    Console.WriteLine($"   Creating symbolic link or copying DevContent...");
                    
                    // Try to find DevContent
                    var devContentPath = FindDevContentPath();
                    if (devContentPath != null && Directory.Exists(devContentPath))
                    {
                        Console.WriteLine($"   Found DevContent at: {devContentPath}");
                        // On Linux we can create a symlink
                        try
                        {
                            if (Environment.OSVersion.Platform == PlatformID.Unix)
                            {
                                var process = System.Diagnostics.Process.Start("ln", $"-sf \"{devContentPath}\" \"{contentPath}\"");
                                process?.WaitForExit();
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"   Warning: Could not create symlink: {ex.Message}");
                        }
                    }
                }

                // Initialize engine settings
                EngineSettingsInitializer.Initialize();
                Console.WriteLine("✅ Engine settings initialized");

                // Initialize content manager
                _contentManager = NetGore.Content.ContentManager.Create();
                Console.WriteLine("✅ Content manager initialized");

                // Initialize audio manager (may fail if no audio device)
                try
                {
                    var audioManager = AudioManager.GetInstance(_contentManager);
                    Console.WriteLine("✅ Audio manager initialized");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"⚠️  Audio manager initialization failed: {ex.Message}");
                    Console.WriteLine("   Editor will continue without audio support");
                }

                // Initialize GrhInfo (graphics system)
                try
                {
                    // GrhInfo.Load uses ContentPaths to find grhdata.dat in Data directory
                    if (File.Exists(Path.Combine(contentPath, "Data", "grhdata.dat")))
                    {
                        GrhInfo.Load(ContentPaths.Build, _contentManager);
                        var grhCount = GrhInfo.GrhDatas.Count();
                        Console.WriteLine($"✅ GrhInfo loaded: {grhCount} graphics");
                    }
                    else
                    {
                        Console.WriteLine($"⚠️  grhdata.dat not found in {contentPath}/Data/");
                        Console.WriteLine($"   GRH graphics will not be available");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"⚠️  GrhInfo loading failed: {ex.Message}");
                    Console.WriteLine($"   Stack: {ex.StackTrace}");
                }

                _isInitialized = true;
                Console.WriteLine("✅ ContentService: Initialization complete!");
                
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ ContentService: Initialization failed: {ex.Message}");
                Console.WriteLine($"   Stack trace: {ex.StackTrace}");
                return false;
            }
        }

        /// <summary>
        /// Tries to find the DevContent directory
        /// </summary>
        private static string? FindDevContentPath()
        {
            // Start from current directory and walk up looking for DevContent
            var currentDir = AppDomain.CurrentDomain.BaseDirectory;
            
            for (int i = 0; i < 6; i++)  // Look up to 6 levels
            {
                var devContentPath = Path.Combine(currentDir, "DevContent");
                if (Directory.Exists(devContentPath))
                    return devContentPath;
                
                var parentDir = Directory.GetParent(currentDir);
                if (parentDir == null)
                    break;
                    
                currentDir = parentDir.FullName;
            }
            
            return null;
        }

        /// <summary>
        /// Creates a drawing manager for SFML rendering
        /// </summary>
        public static IDrawingManager CreateDrawingManager(SFML.Graphics.RenderWindow renderWindow)
        {
            try
            {
                if (_contentManager == null)
                {
                    Console.WriteLine("⚠️  ContentManager not initialized, initializing now...");
                    Initialize();
                }

                // Create a drawing manager for this render window
                var drawingManager = new DrawingManager(renderWindow);
                
                Console.WriteLine("✅ DrawingManager created");
                
                return drawingManager;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error creating DrawingManager: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Cleans up resources
        /// </summary>
        public static void Shutdown()
        {
            if (!_isInitialized)
                return;

            try
            {
                Console.WriteLine("🔌 ContentService: Shutting down...");
                
                // Dispose of managers
                _drawingManager?.Dispose();
                _contentManager?.Dispose();
                
                _isInitialized = false;
                Console.WriteLine("✅ ContentService: Shutdown complete");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"⚠️  Error during shutdown: {ex.Message}");
            }
        }
    }
}

