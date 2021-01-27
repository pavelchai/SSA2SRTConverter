/*
 * SSA2SRT Converter.
 * Licensed under MIT License.
 * Copyright © 2021 Pavel Chaimardanov.
 */

namespace SSA2SRT.Model
{
	/// <summary>
	/// Represents an entry for the <see cref="CompositeReadOnlyStorer"></see>.
	/// </summary>
	internal sealed class CompositeReadOnlyStorerEntry : AbstractDataStoreEntry
	{
		/// <summary>
		/// Creates a new entry for the <see cref="CompositeReadOnlyStorer"></see>.
		/// </summary>
		/// <param name="data"> Data. </param>
		public CompositeReadOnlyStorerEntry(SSA2SRTConverterData data) : base(data.Name)
		{
			this.Data = data.Data;
		}

		/// <summary>
		/// Data of the entry;
		/// </summary>
		public readonly byte[] Data;
	}
}