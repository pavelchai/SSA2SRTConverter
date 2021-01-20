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
	/// Represents all supported unicode encodings.
	/// </summary>
	public static class UnicodeEncodings
	{
		/// <summary>
		/// UTF-7 Encoding (with BOM).
		/// </summary>
		public static readonly Encoding UTF7BOM = new UTF7EncodingWithBOM(true);
		
		/// <summary>
		/// UTF-8 Encoding (with BOM).
		/// </summary>
		public static readonly Encoding UTF8BOM = new UTF8Encoding(true);
		
		/// <summary>
		/// UTF-16 Encoding (Little endian, with BOM).
		/// </summary>
		public static readonly Encoding UTF16LEBOM = new UnicodeEncoding(false, true);
		
		/// <summary>
		/// UTF-16 Encoding (Big endian, with BOM).
		/// </summary>
		public static readonly Encoding UTF16BEBOM = new UnicodeEncoding(true, true);
		
		/// <summary>
		/// UTF-32 Encoding (Little endian, with BOM).
		/// </summary>
		public static readonly Encoding UTF32LEBOM = new UTF32Encoding(false, true);
		
		/// <summary>
		/// UTF-32 Encoding (Big endian, with BOM).
		/// </summary>
		public static readonly Encoding UTF32BEBOM = new UTF32Encoding(true, true);
		
		/// <summary>
		/// UTF-7 Encoding (no BOM).
		/// </summary>
		public static readonly Encoding UTF7NoBOM = new UTF7EncodingWithBOM(false);
		
		/// <summary>
		/// UTF-8 Encoding (no BOM).
		/// </summary>
		public static readonly Encoding UTF8NoBOM = new UTF8Encoding(false);
		
		/// <summary>
		/// UTF-16 Encoding (Little endian, no BOM).
		/// </summary>
		public static readonly Encoding UTF16LENoBOM = new UnicodeEncoding(false, false);
		
		/// <summary>
		/// UTF-16 Encoding (Big endian, no BOM).
		/// </summary>
		public static readonly Encoding UTF16BENoBOM = new UnicodeEncoding(true, false);
		
		/// <summary>
		/// UTF-32 Encoding (Little endian, no BOM).
		/// </summary>
		public static readonly Encoding UTF32LENoBOM = new UTF32Encoding(false, false);
		
		/// <summary>
		/// UTF-32 Encoding (Big endian, no BOM).
		/// </summary>
		public static readonly Encoding UTF32BENoBOM = new UTF32Encoding(true, false);
		
		/// <summary>
		/// Represents a UTF-7 Encoding (with BOM support).
		/// </summary>
		private sealed class UTF7EncodingWithBOM : UTF7Encoding
		{
			/// <summary>
			/// Preamle.
			/// </summary>
			private readonly byte[] premble;
			
			/// <summary>
			/// Creates a new UTF-7 Encoding (with BOM support).
			/// </summary>
			/// <param name="byteOrderMask"> True to specify that the GetPreamble() method returns a Unicode byte order mark, otherwise false. </param>
			public UTF7EncodingWithBOM(bool byteOrderMask)
			{
				this.premble = byteOrderMask ? new byte[] {0x2B, 0x2F, 0x76, 0x38} : new byte[0];
			}
			
			/// <inheritdoc/>
			public override byte[] GetPreamble()
			{
				return this.premble;
			}
		}
	}
}