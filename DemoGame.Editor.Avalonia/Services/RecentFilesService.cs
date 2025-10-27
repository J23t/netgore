using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using NetGore.World;

namespace DemoGame.Editor.Avalonia.Services
{
    /// <summary>
    /// Service for managing recent files list
    /// </summary>
    public class RecentFilesService
    {
        private const int MaxRecentFiles = 10;
        private static readonly string RecentFilesPath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "NetGore", "RecentMaps.txt");

        /// <summary>
        /// Adds a map to the recent files list
        /// </summary>
        public static void AddRecentMap(MapID mapId, string mapName = "")
        {
            try
            {
                var recent = GetRecentMaps().ToList();
                
                // Remove if already exists
                recent.RemoveAll(m => m.ID == mapId);
                
                // Add to front
                recent.Insert(0, new MapListItem { ID = mapId, Name = mapName });
                
                // Limit to max
                if (recent.Count > MaxRecentFiles)
                {
                    recent = recent.Take(MaxRecentFiles).ToList();
                }
                
                // Save
                SaveRecentMaps(recent);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error adding to recent files: {ex.Message}");
            }
        }

        /// <summary>
        /// Gets the list of recent maps
        /// </summary>
        public static IEnumerable<MapListItem> GetRecentMaps()
        {
            try
            {
                if (!File.Exists(RecentFilesPath))
                    return Enumerable.Empty<MapListItem>();

                var lines = File.ReadAllLines(RecentFilesPath);
                return lines
                    .Select(line =>
                    {
                        var parts = line.Split('|');
                        if (parts.Length >= 2 && int.TryParse(parts[0], out int id))
                        {
                            return new MapListItem 
                            { 
                                ID = new MapID(id), 
                                Name = parts[1] 
                            };
                        }
                        return null;
                    })
                    .Where(m => m != null)
                    .Select(m => m!);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading recent files: {ex.Message}");
                return Enumerable.Empty<MapListItem>();
            }
        }

        /// <summary>
        /// Saves the recent maps list
        /// </summary>
        private static void SaveRecentMaps(List<MapListItem> maps)
        {
            try
            {
                // Ensure directory exists
                var dir = Path.GetDirectoryName(RecentFilesPath);
                if (dir != null && !Directory.Exists(dir))
                {
                    Directory.CreateDirectory(dir);
                }

                // Write file
                var lines = maps.Select(m => $"{(int)m.ID}|{m.Name}");
                File.WriteAllLines(RecentFilesPath, lines);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving recent files: {ex.Message}");
            }
        }

        /// <summary>
        /// Clears the recent files list
        /// </summary>
        public static void ClearRecentMaps()
        {
            try
            {
                if (File.Exists(RecentFilesPath))
                {
                    File.Delete(RecentFilesPath);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error clearing recent files: {ex.Message}");
            }
        }
    }
}

