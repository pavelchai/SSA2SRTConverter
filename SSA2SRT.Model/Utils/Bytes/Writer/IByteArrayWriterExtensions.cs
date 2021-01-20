/*
 * SSA2SRT Converter.
 * Licensed under MIT License.
 * Copyright © 2021 Pavel Chaimardanov.
 */

namespace SSA2SRT.Model.Utils
{
    /// <summary>
    /// Represents the extensions for the <see cref="IByteArrayWriter"></see>.
    /// </summary>
    internal static class IByteArrayWriterExtensions
    {
        /// <summary>
        /// Writes the array of the bytes with the <see cref="IByteArrayWriter"></see>.
        /// </summary>
        /// <param name="writer"> The writer. </param>
        /// <param name="data"> Array of the bytes. </param>
        public static void WriteBytes(this IByteArrayWriter writer, params byte[] data)
        {
            writer.WriteBytes(data, 0, data.Length);
        }
    }
}