/*
 * SSA2SRT Converter.
 * Licensed under MIT License.
 * Copyright © 2021 Pavel Chaimardanov.
 */
using SSA2SRT.Model.Utils;
using System.Collections.Generic;
using System.Linq;

namespace SSA2SRT.Model
{
	/// <summary>
	/// Represents a read-only composite storer (for many input files that should be converted).
	/// </summary>
	internal sealed class CompositeReadOnlyStorer : IReadOnlyDataStorer
	{
		/// <summary>
		/// Creates a new read-only composite storer (for many input files that should be converted).
		/// </summary>
		/// <param name="data"> Data of the input files that should be converted. </param>
		public CompositeReadOnlyStorer(IEnumerable<SSA2SRTConverterData> data)
		{
			this.Entries = data.Select(s => new CompositeReadOnlyStorerEntry(s)).ToArray();
		}

		/// <inheritdoc/>
		public IReadOnlyList<IDataStorerEntry> Entries { get; private set; }

		/// <inheritdoc/>
		public byte[] Read(IDataStorerEntry entry)
		{
			Validation.NotNull("Entry", entry);

			CompositeReadOnlyStorerEntry fileEntry = entry as CompositeReadOnlyStorerEntry;
			if (fileEntry == null)
			{
				throw new InvalidEntryException(entry.Path);
			}

			return fileEntry.Data;
		}
	}
}