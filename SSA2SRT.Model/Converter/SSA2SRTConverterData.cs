/*
 * This file is a part of the AltPhotonic Studio (ST).
 * Copyright © 2020 Pavel Chaimardanov. All rights reserved.
 */
using System;

namespace SSA2SRT.Model
{
	/// <summary>
	/// Represents a data for the SSA/ASS to SRT converter.
	/// </summary>
	public sealed class SSA2SRTConverterData
	{
		/// <summary>
		/// Creates a new data for the SSA/ASS to SRT converter.
		/// </summary>
		/// <param name="name"> Name of the data. </param>
		/// <param name="data"> Data. </param>
		public SSA2SRTConverterData(string name, byte[] data)
		{
			this.Name = name;
			this.Data = data;
		}
		
		/// <summary>
		/// Name of the data.
		/// </summary>
		public string Name { get; set; }
		
		/// <summary>
		/// Data.
		/// </summary>
		public byte[] Data { get; set; }
	}
}