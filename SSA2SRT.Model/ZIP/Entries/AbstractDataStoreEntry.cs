/*
 * SSA2SRT Converter.
 * Licensed under MIT License.
 * Copyright © 2021 Pavel Chaimardanov.
 */

namespace SSA2SRT.Model
{
	/// <summary>
	/// Represents a base class for the entry.
	/// </summary>
	public abstract class AbstractDataStoreEntry : IDataStorerEntry
	{
		/// <summary>
		/// Represents a base class for the entry.
		/// </summary>
		/// <param name="path"> Path to the data. </param>
		protected AbstractDataStoreEntry(string path)
		{
			this.Path = path;
		}

		/// <inheritdoc/>
		public string Path { get; private set; }
	}
}