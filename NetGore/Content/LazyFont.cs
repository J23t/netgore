using System;
using SFML.Graphics;

namespace SFML.Graphics
{
    /// <summary>
    /// Simple wrapper for <see cref="Font"/> that loads immediately.
    /// </summary>
    public class LazyFont : Font
    {
        readonly string _filename;
        readonly uint _defaultSize;

        /// <summary>
        /// Initializes a new instance of the <see cref="LazyFont"/> class.
        /// </summary>
        /// <param name="filename">Font file to load</param>
        /// <param name="charSize">Character size</param>
        public LazyFont(string filename, uint charSize = 30u) : base(filename)
        {
            if (charSize > ushort.MaxValue)
                throw new ArgumentOutOfRangeException(nameof(charSize));

            _filename = filename;
            _defaultSize = charSize;
        }

        /// <summary>
        /// Gets the file name that this font uses to load.
        /// </summary>
        public string FileName => _filename;

        /// <summary>
        /// Gets the default size used for this <see cref="Font"/>.
        /// </summary>
        public uint DefaultSize => _defaultSize;
    }
}

