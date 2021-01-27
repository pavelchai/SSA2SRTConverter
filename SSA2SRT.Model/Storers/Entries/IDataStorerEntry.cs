/*
 * SSA2SRT Converter.
 * Licensed under MIT License.
 * Copyright © 2021 Pavel Chaimardanov.
 */

namespace SSA2SRT.Model
{
    /// <summary>
    /// Represents an entry of the data in the file storer.
    /// </summary>
    public interface IDataStorerEntry
    {
        /// <summary>
        /// Gets the path to the data in the file storer.
        /// </summary>
        /// <returns> Path to the data in the file storer. </returns>
        string Path { get; }
    }
}