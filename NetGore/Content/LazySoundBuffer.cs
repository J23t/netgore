using System;
using System.IO;
using SFML.Audio;

namespace SFML.Audio
{
    /// <summary>
    /// Simple wrapper for <see cref="SoundBuffer"/> that loads immediately.
    /// </summary>
    public class LazySoundBuffer : SoundBuffer
    {
        readonly string _filename;
        
        static readonly string[] _supportedExtensions = { ".ogg", ".wav", ".flac", ".mp3" };

        /// <summary>
        /// Initializes a new instance of the <see cref="LazySoundBuffer"/> class.
        /// </summary>
        /// <param name="filename">Sound file to load</param>
        public LazySoundBuffer(string filename) : base(FindSoundFile(filename))
        {
            _filename = filename;
        }
        
        /// <summary>
        /// Finds a sound file by trying different extensions if the exact filename doesn't exist.
        /// </summary>
        /// <param name="filename">The base filename to search for</param>
        /// <returns>The actual filename with extension</returns>
        static string FindSoundFile(string filename)
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
            // and let the SoundBuffer constructor throw the appropriate error
            return filename;
        }

        /// <summary>
        /// Gets the file name that this sound buffer uses to load.
        /// </summary>
        public string FileName => _filename;
    }
}

