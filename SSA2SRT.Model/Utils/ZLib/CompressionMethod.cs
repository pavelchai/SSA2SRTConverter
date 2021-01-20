/*
 * SSA2SRT Converter.
 * Licensed under MIT License.
 * Copyright © 2021 Pavel Chaimardanov.
 */

namespace SSA2SRT.Model.Utils
{
    /// <summary>
    /// Represents the compression method.
    /// </summary>
    internal enum CompressionMethod : ushort
    {
        /// <summary>
        /// Store (without compression).
        /// </summary>
        Store = 0,

        /// <summary>
        /// Deflate compression method.
        /// </summary>
        Deflate = 8
    }
}