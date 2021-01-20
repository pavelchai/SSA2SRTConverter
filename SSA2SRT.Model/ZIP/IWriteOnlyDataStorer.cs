/*
 * SSA2SRT Converter.
 * Licensed under MIT License.
 * Copyright © 2021 Pavel Chaimardanov.
 */

namespace SSA2SRT.Model
{
    /// <summary>
    /// Represents a write-only data storer.
    /// </summary>
    public interface IWriteOnlyDataStorer
    {
        /// <summary>
        /// Adds contents of a data into the store.
        /// </summary>
        /// <param name="path"> Path of the data. </param>
        /// <param name="data"> Array of bytes containing the data. </param>
        /// <returns> Entry of the data in the storer. </returns>
        /// <remarks> Array of the data should be immutable. </remarks>
        /// <exception cref="Utils.ValueNullException">
        /// The exception that is thrown when path or data are null.
        /// </exception>
        IDataStorerEntry Add(string path, params byte[] data);

        /// <summary>
        /// Closes the storer and gets the array of bytes containing the saved data.
        /// </summary>
        /// <returns> Array of bytes containing the saved data. </returns>
        byte[] Close();
    }
}