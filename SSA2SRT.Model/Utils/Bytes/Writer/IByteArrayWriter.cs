/*
 * SSA2SRT Converter.
 * Licensed under MIT License.
 * Copyright © 2021 Pavel Chaimardanov.
 */

namespace SSA2SRT.Model.Utils
{
    /// <summary>
    /// Represents a writer for the array of the bytes.
    /// </summary>
    internal interface IByteArrayWriter
    {
        /// <summary>
        /// Gets a current position in the <see cref="IByteArrayWriter"></see>.
        /// </summary>
        /// <returns> Current position in the <see cref="IByteArrayWriter"></see>. </returns>
        int Position { get; }

        /// <summary>
        /// Writes the array of the bytes.
        /// </summary>
        /// <param name="data"> Array of the bytes. </param>
        /// <param name="offset"> Offset in the data. </param>
        /// <param name="count"> Number of the written bytes. </param>
        void WriteBytes(byte[] data, int offset, int count);

        /// <summary>
        /// Creates a new array of the bytes from the all written data.
        /// </summary>
        /// <returns> New array of the bytes. </returns>
        byte[] GetBytes();
    }
}