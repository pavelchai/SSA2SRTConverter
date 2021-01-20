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
		/// Creates a new settings for the SSA/ASS to SRT converter. 
		/// </summary>
		/// <param name="outputEncoding"> Determines encoding of the converted subtitles. If null - detected encoding will be used. </param>
		/// <param name="minShowTime"> Minimal time to show. If null - as defined in the subtitles. </param>
		/// <param name="nameConverter"> Converter for the names. If null - default converter will be used. </param>
		/// <param name="ignoreErrors"> Indicates whether all conversion exceptions should be ignored. </param>
		public SSA2SRTConverterSettings(Encoding outputEncoding = null, double? minShowTime = null, Func<string, string> nameConverter = null, bool? ignoreErrors = true)
		{
			this.OutputEncoding = outputEncoding;
			this.MinShowTime = minShowTime;
			this.IgnoreExceptions = ignoreErrors;
			this.NameConverter = nameConverter ?? 
				(s => s.Replace(".ass", ".srt").Replace(".ssa", ".srt").Replace(".zip", ".converted.zip"));
		}
		
		/// <summary>
		/// Determines encoding of the converted subtitles. If null - detected encoding will be used.
		/// </summary>
		public readonly Encoding OutputEncoding;
		
		/// <summary>
		/// Minimal time to show. If null - as defined in the subtitles.
		/// </summary>
		public readonly double? MinShowTime;
		
		/// <summary>
		/// Indicates whether all conversion exceptions should be ignored.
		/// </summary>
		public readonly bool? IgnoreExceptions;
		
		/// <summary>
		/// Converter for the names. If null - default converter will be used.
		/// </summary>
		public readonly Func<string, string> NameConverter;
	}
}