/*
 * SSA2SRT Converter.
 * Licensed under MIT License.
 * Copyright © 2021 Pavel Chaimardanov.
 */
using System;
using System.Text;

namespace SSA2SRT.Model
{
	/// <summary>
	/// Represents the settings for the SSA/ASS to SRT converter. 
	/// </summary>
	public sealed class SSA2SRTConverterSettings 
	{
		/// <summary>
		/// Determines encoding of the converted subtitles. If null - detected encoding will be used.
		/// </summary>
		public Encoding OutputEncoding { get; set; } = null;
		
		/// <summary>
		/// Minimal time to show. If null - as defined in the subtitles.
		/// </summary>
		public double? MinShowTime { get; set; } = null;

		/// <summary>
		/// Indicates whether all conversion exceptions should be ignored.
		/// </summary>
		public bool IgnoreExceptions { get; set; } = true;

		/// <summary>
		/// Indicates whether all converted files should be saved in one zip file.
		/// </summary>
		public bool SaveInZipFile { get; set; } = false;

		/// <summary>
		/// Name of the zip file (if <see cref="SaveInZipFile"/> is true).
		/// </summary>
		public string ZipFileName { get; set; } = "converted.zip";

		/// <summary>
		/// Converter for the names.
		/// </summary>
		public Func<string, string> NameConverter { get; set; } = 
			(s => s.Replace(".ass", ".srt").Replace(".ssa", ".srt").Replace(".zip", ".converted.zip"));
	}
}