using System;
using System.IO;
using SFML.Graphics;

namespace SFML.Graphics
{
    /// <summary>
    /// Simple wrapper for <see cref="Texture"/> that loads immediately.
    /// </summary>
    public class LazyTexture : Texture
    {
        readonly string _filename;
        
        static readonly string[] _supportedExtensions = { ".png", ".jpg", ".jpeg", ".bmp", ".tga", ".gif" };

        /// <summary>
        /// Initializes a new instance of the <see cref="LazyTexture"/> class.
        /// </summary>
        /// <param name="filename">Texture file to load</param>
        public LazyTexture(string filename) : base(FindImageFile(filename))
        {
            _filename = filename;
        }
        
        /// <summary>
        /// Finds an image file by trying different extensions if the exact filename doesn't exist.
        /// </summary>
        /// <param name="filename">The base filename to search for</param>
        /// <returns>The actual filename with extension</returns>
        static string FindImageFile(string filename)
        {
            // If the file exists as-is, return it
            if (File.Exists(filename))
                return filename;
            
            // Try each supported extension
            foreach (var ext in _supportedExtensions)
            {
                var filenameWithExt = filename + ext;
                if (File.Exists(filenameWithExt))
                    return filenameWithExt;
            }
            
            // If we didn't find the file, return the original filename
            // and let the Texture constructor throw the appropriate error
            return filename;
        }

        /// <summary>
        /// Gets the file name that this texture uses to load.
        /// </summary>
        public string FileName => _filename;
    }
}

