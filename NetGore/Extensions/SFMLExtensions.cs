using System;
using System.Linq;
using SFML.Audio;
using SFML.Graphics;
using SFML.Window;

namespace NetGore
{
    /// <summary>
    /// Extension methods for SFML 2.6 types to provide compatibility with NetGore code.
    /// </summary>
    public static class SFMLExtensions
    {
        /// <summary>
        /// Checks if a Texture has been disposed. In SFML 2.6, we just check for null.
        /// </summary>
        public static bool IsDisposed(this Texture texture)
        {
            return texture == null;
        }

        /// <summary>
        /// Checks if a Font has been disposed. In SFML 2.6, we just check for null.
        /// </summary>
        public static bool IsDisposed(this Font font)
        {
            return font == null;
        }

        /// <summary>
        /// Checks if an Image has been disposed. In SFML 2.6, we just check for null.
        /// </summary>
        public static bool IsDisposed(this Image image)
        {
            return image == null;
        }

        /// <summary>
        /// Checks if a Music has been disposed. In SFML 2.6, we just check for null.
        /// </summary>
        public static bool IsDisposed(this Music music)
        {
            return music == null;
        }

        /// <summary>
        /// Checks if a SoundBuffer has been disposed. In SFML 2.6, we just check for null.
        /// </summary>
        public static bool IsDisposed(this SoundBuffer buffer)
        {
            return buffer == null;
        }

        /// <summary>
        /// Checks if a Shader has been disposed. In SFML 2.6, we just check for null.
        /// </summary>
        public static bool IsDisposed(this Shader shader)
        {
            return shader == null;
        }

        /// <summary>
        /// Checks if a RenderTexture has been disposed. In SFML 2.6, we just check for null.
        /// </summary>
        public static bool IsDisposed(this RenderTexture renderTexture)
        {
            return renderTexture == null;
        }

        /// <summary>
        /// Checks if a RenderWindow has been disposed. In SFML 2.6, we just check for null.
        /// </summary>
        public static bool IsDisposed(this RenderWindow renderWindow)
        {
            return renderWindow == null;
        }

        /// <summary>
        /// Checks if a Window has been disposed. In SFML 2.6, we just check for null.
        /// </summary>
        public static bool IsDisposed(this Window window)
        {
            return window == null;
        }

        /// <summary>
        /// Checks if a Sprite has been disposed. In SFML 2.6, we just check for null.
        /// </summary>
        public static bool IsDisposed(this Sprite sprite)
        {
            return sprite == null;
        }

        /// <summary>
        /// Checks if a Text has been disposed. In SFML 2.6, we just check for null.
        /// </summary>
        public static bool IsDisposed(this Text text)
        {
            return text == null;
        }
    }
}

