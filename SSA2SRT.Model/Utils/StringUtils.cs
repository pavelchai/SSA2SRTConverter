/*
 * SSA2SRT Converter.
 * Licensed under MIT License.
 * Copyright © 2021 Pavel Chaimardanov.
 */
using System;
using System.Text;
using System.Collections.Generic;

namespace SSA2SRT.Model.Utils
{
	/// <summary>
    /// Represents the utils for the strings.
    /// </summary>
	internal static class StringUtils
	{
		/// <summary>
		/// Finds encoding of a text and represents the text in respectively encoding.
		/// Modified solution from: https://stackoverflow.com/questions/1025332/determine-a-strings-encoding-in-c-sharp.
		/// </summary>
		/// <param name="b"> Array of the bytes of the text. </param>
		/// <param name="text"> Encoded text. </param>
		/// <param name="taster"> Number of bytes to check of the file. </param>
		/// <returns> Encoding of the text. </returns>
		/// <remarks> 
		/// Function to detect the encoding for UTF-7, UTF-8/16/32 (bom, no bom, little
		/// and big endian), and local default codepage, and potentially other codepages.
		/// 'taster' = number of bytes to check of the file (to save processing). Higher
		/// value is slower, but more reliable (especially UTF-8 with special characters
		/// later on may appear to be ASCII initially). If taster = 0, then taster
		/// becomes the length of the file (for maximum reliability). 'text' is simply
		/// the string with the discovered encoding applied to the file.</remarks>
		public static Encoding DetectTextEncoding(byte[] b, out string text, int taster = 1000)
		{
			// First check the low hanging fruit by checking if a
			// BOM/signature exists (sourced from http://www.unicode.org/faq/utf_bom.html#bom4)
			
			Encoding encoding = null;
			
		    if (b.Length >= 4 && b[0] == 0x00 && b[1] == 0x00 && b[2] == 0xFE && b[3] == 0xFF)
		    {
		    	encoding = UnicodeEncodings.UTF32BEBOM;
		    }
		    else if (b.Length >= 4 && b[0] == 0xFF && b[1] == 0xFE && b[2] == 0x00 && b[3] == 0x00)
		    {
		    	encoding = UnicodeEncodings.UTF32LEBOM;
		    }
		    else if (b.Length >= 2 && b[0] == 0xFE && b[1] == 0xFF)
		    {
		    	encoding = UnicodeEncodings.UTF16BEBOM;
		    }
		    else if (b.Length >= 2 && b[0] == 0xFF && b[1] == 0xFE)
		    {
		    	encoding = UnicodeEncodings.UTF16LEBOM;
		    }
		    else if (b.Length >= 3 && b[0] == 0xEF && b[1] == 0xBB && b[2] == 0xBF)
		    {
		    	encoding = UnicodeEncodings.UTF8BOM;
		    }
		    else if (b.Length >= 3 && b[0] == 0x2b && b[1] == 0x2f && b[2] == 0x76 && (b[3] == 0x38 || b[3] == 0x39 || b[3] == 0x2B || b[3] == 0x2F))
		    {
		    	encoding = UnicodeEncodings.UTF7BOM;
		    }
		    
		    if(encoding != null)
		    {
		    	text = encoding.GetString(b);
		    	return encoding;
		    }

		    // If the code reaches here, no BOM/signature was found, so now
		    // we need to 'taste' the file to see if can manually discover
		    //the encoding. A high taster value is desired for UTF-8
		   	if (taster == 0 || taster > b.Length)
		   	{
		   		taster = b.Length;
		   	}
		   	
		   	if (taster == 0)
		   	{
		   		text = "";
		   		return UnicodeEncodings.UTF16LENoBOM;
		   	}

		   	// Some text files are encoded in UTF8, but have no BOM/signature. Hence
		   	// the below manually checks for a UTF8 pattern. This code is based off
		   	// the top answer at: https://stackoverflow.com/questions/6555015/check-for-invalid-utf8
		   	// For our purposes, an unnecessarily strict (and terser/slower)
		   	// implementation is shown at: https://stackoverflow.com/questions/1031645/how-to-detect-utf-8-in-plain-c
		   	// For the below, false positives should be exceedingly rare (and would
		   	// be either slightly malformed UTF-8 (which would suit our purposes
		   	// anyway) or 8-bit extended ASCII/UTF-16/32 at a vanishingly long shot).
		    int i = 0;
		    bool utf8 = false;
		    while (i < taster - 4)
		    {
		        if (b[i] <= 0x7F)
		        {
		        	i += 1; continue;
		        }
		        
		        if (b[i] >= 0xC2 && b[i] <= 0xDF && b[i + 1] >= 0x80 && b[i + 1] < 0xC0)
		        {
		        	i += 2;
		        	utf8 = true;
		        	continue;
		        }
		        
		        if (b[i] >= 0xE0 && b[i] <= 0xF0 && b[i + 1] >= 0x80 && b[i + 1] < 0xC0 && b[i + 2] >= 0x80 && b[i + 2] < 0xC0)
		        {
		        	i += 3;
		        	utf8 = true;
		        	continue;
		        }
		        
		        if (b[i] >= 0xF0 && b[i] <= 0xF4 && b[i + 1] >= 0x80 && b[i + 1] < 0xC0 && b[i + 2] >= 0x80 && b[i + 2] < 0xC0 && b[i + 3] >= 0x80 && b[i + 3] < 0xC0)
		        {
		        	i += 4;
		        	utf8 = true;
		        	continue;
		        }
		        
		        utf8 = false; break;
		    }
		    
		    if (utf8)
		    {
		        text = UnicodeEncodings.UTF8NoBOM.GetString(b);
		        return UnicodeEncodings.UTF8NoBOM;
		    }

		    // The next check is a heuristic attempt to detect UTF-16 without a BOM.
		    // We simply look for zeroes in odd or even byte places, and if a certain
		    // threshold is reached, the code is 'probably' UTF-16.
		    const double threshold = 0.1;
		    int count = 0;
		    for (int n = 0; n < taster; n += 2)
		    {
		    	if (b[n] == 0)
		    	{
		    		count++;
		    	}
		    }
		    
		    if (((double)count) / taster > threshold)
		    {
		    	text = UnicodeEncodings.UTF16BENoBOM.GetString(b);
		    	return UnicodeEncodings.UTF16BENoBOM;
		    }
		    
		    count = 0;
		    for (int n = 1; n < taster; n += 2)
		    {
		    	if (b[n] == 0)
		    	{
		    		count++;
		    	}
		    }
		    
		    if (((double)count) / taster > threshold)
		    {
		    	text = UnicodeEncodings.UTF16LENoBOM.GetString(b);
		    	return UnicodeEncodings.UTF16LENoBOM;
		    }

		    for (int n = 0; n < taster - 9; n++)
		    {
		        if (
		            ((b[n + 0] == 'c' || b[n + 0] == 'C') && (b[n + 1] == 'h' || b[n + 1] == 'H') && (b[n + 2] == 'a' || b[n + 2] == 'A') && (b[n + 3] == 'r' || b[n + 3] == 'R') && (b[n + 4] == 's' || b[n + 4] == 'S') && (b[n + 5] == 'e' || b[n + 5] == 'E') && (b[n + 6] == 't' || b[n + 6] == 'T') && (b[n + 7] == '=')) ||
		            ((b[n + 0] == 'e' || b[n + 0] == 'E') && (b[n + 1] == 'n' || b[n + 1] == 'N') && (b[n + 2] == 'c' || b[n + 2] == 'C') && (b[n + 3] == 'o' || b[n + 3] == 'O') && (b[n + 4] == 'd' || b[n + 4] == 'D') && (b[n + 5] == 'i' || b[n + 5] == 'I') && (b[n + 6] == 'n' || b[n + 6] == 'N') && (b[n + 7] == 'g' || b[n + 7] == 'G') && (b[n + 8] == '='))
		            )
		        {
		            if (b[n + 0] == 'c' || b[n + 0] == 'C')
		            {
		            	n += 8;
		            }
		            else
		            {
		            	n += 9;
		            }
		            
		            if (b[n] == '"' || b[n] == '\'')
		            {
		            	n++;
		            }
		            int oldn = n;
		            
		            while (n < taster && (b[n] == '_' || b[n] == '-' || (b[n] >= '0' && b[n] <= '9') || (b[n] >= 'a' && b[n] <= 'z') || (b[n] >= 'A' && b[n] <= 'Z')))
		            {
		            	n++;
		            
		            }
		            byte[] nb = new byte[n-oldn];
		            Array.Copy(b, oldn, nb, 0, n-oldn);
		            try
		            {
		                string internalEnc = Encoding.ASCII.GetString(nb);
		                Encoding internalEncoding = Encoding.GetEncoding(internalEnc);
		                text = internalEncoding.GetString(b);
		                return internalEncoding;
		            }
		            catch
		            {
		            	break;
		            }
		        }
		    }

		    // If all else fails, the detected encoding will be defined as ASCII
		    text = Encoding.ASCII.GetString(b);
		    return Encoding.ASCII;
		}
		
		/// <summary>
        /// Returns an <see cref="IEnumerable{T}"></see> that contains the substrings in this instance that are delimited by elements of a specified unicode character array.
        /// </summary>
        /// <param name="inputString"> Input string. </param>
        /// <param name="separators"> A character array that delimits the substrings in this string. </param>
        /// <returns> An <see cref="IEnumerable{T}"></see> whose elements contains the substrings in the string that are delimited by one or more characters in separators. </returns>
        public static IEnumerable<string> Split(string inputString, params char[] separators)
        {
        	return Split(inputString, 0, inputString.Length, separators);
        }
        
        /// <summary>
        /// Returns an <see cref="IEnumerable{T}"></see> that contains the substrings in this instance that are delimited by elements of a specified unicode character array.
        /// </summary>
        /// <param name="inputString"> Input string. </param>
        /// <param name="offset"> Initial offset in the string. </param>
        /// <param name="count"> Count of the chars. </param>
        /// <param name="separators"> A character array that delimits the substrings in this string. </param>
        /// <returns> An <see cref="IEnumerable{T}"></see> whose elements contains the substrings in the string that are delimited by one or more characters in separators. </returns>
        public static IEnumerable<string> Split(string inputString, int offset, int count, params char[] separators)
        {
            if (count == 0)
            {
                yield return string.Empty;
                yield break;
            }
            
            int separatorsLength = separators.Length;
            if (separatorsLength == 0)
            {
            	yield return inputString.Substring(offset, count);
                yield break;
            }

            int index = 0, start = 0;
            int endIndex = count + offset;
            
            while (true)
            {
                string substring = Split(inputString, separators, separatorsLength, offset, endIndex, ref start, ref index);
                if (substring != null)
                {
                    yield return substring;
                }
                else
                {
                    break;
                }
            }
        }
		
		/// <summary>
        /// Returns an <see cref="IEnumerable{T}"></see> that contains the lines from the string.
        /// </summary>
        /// <param name="inputString"> Input string. </param>
        /// <returns> An <see cref="IEnumerable{T}"></see> whose elements contains the lines from the input string. </returns>
        /// <remarks> Supported separators of the lines are \n, \r and \r\n. </remarks>
        public static IEnumerable<string> SplitLines(string inputString)
        {
        	int stringLength = inputString.Length;
            if (stringLength == 0)
            {
                yield return string.Empty;
                yield break;
            }

            int index = 0, start = 0;

            while (true)
            {
                string substring = SplitLines(inputString, stringLength, ref start, ref index);
                if (substring != null)
                {
                    yield return substring;
                }
                else
                {
                    break;
                }
            }
        }
        
        /// <summary>
        /// Splits an input string to the lines.
        /// </summary>
        /// <param name="inputString"> Input string. </param>
        /// <param name="separators"> Array of the separators. </param>
        /// <param name="separatorsLength"> Lenght of the array of the separators. </param>
        /// <param name="initialIndex"> Initial index. </param>
        /// <param name="lastIndex"> Last index. </param>
        /// <param name="start"> Start index. </param>
        /// <param name="index"> Current index. </param>
        /// <returns> String if string isn't the last string, otherwise null. </returns>
        private static unsafe string Split(string inputString, char[] separators, int separatorsLength, int initialIndex, int lastIndex, ref int start, ref int index)
        {
            if (index > lastIndex)
            {
                // Last substring has been returned earlier
                return null;
            }
            
            // Gets an available length of the string (from the start offset)
            int avaliableLength = lastIndex - start;

            if (index != lastIndex)
            {
                char character;
                fixed (char* inputStringPtr = inputString, separatorPtr = separators)
                {
                    char* iPtr = &inputStringPtr[initialIndex + index];
                    while (avaliableLength-- > 0)
                    {
                        // Gets a current character
                        character = *iPtr;
                        
                        char* sPtr = separatorPtr;
                        int sLength = separatorsLength;

                        while (sLength-- > 0)
                        {
                            if (*iPtr == *sPtr++)
                            {
                                // Line separator, skip 1 character
                                string substring = inputString.Substring(initialIndex + start, index - start);
                                start = ++index;
                                return substring;
                            }
                        }

                        // Separator hasn't been found - moves on next character
                        iPtr++;
                        index++;
                    }
                }
            }

            // Returns a last substring
            index++;
            return inputString.Substring(initialIndex + start, lastIndex - initialIndex - start);
        }
		
		/// <summary>
        /// Splits an input string to the lines.
        /// </summary>
        /// <param name="inputString"> Input string. </param>
        /// <param name="stringLength"> Length of the string. </param>
        /// <param name="start"> Start index. </param>
        /// <param name="index"> Current index. </param>
        /// <returns> Line if line isn't the last line, otherwise null. </returns>
        private static unsafe string SplitLines(string inputString, int stringLength, ref int start, ref int index)
        {
            if (index > stringLength)
            {
                // Last substring has been returned earlier
                return null;
            }

            // Gets an available length of the string (from the start offset)
            int avaliableLength = stringLength - start;
            
            if (index != stringLength)
            {
                char characterC, characterN;

                fixed (char* inputStringPtr = inputString)
                {
                    char* iPtr = &inputStringPtr[index];
                    while (avaliableLength-- > 0)
                    {
                        // Gets a current character
                        characterC = *iPtr;
                        
                        if (characterC == '\r')
                        {
                            characterN = *++iPtr;
                            if (characterN == '\n')
                            {
                                // Line separator \r\n, skip 2 character
                                string substring = inputString.Substring(start, index - start);
                                index += 2;
                                start = index;
                                return substring;
                            }
                            else
                            {
                                // Line separator \r, skip 1 character
                                string substring = inputString.Substring(start, index - start);
                                start = ++index;
                                return substring;
                            }
                        }
                        else
                        {
                            if (characterC == '\n')
                            {
                                // Line separator \n, skip 1 character
                                string substring = inputString.Substring(start, index - start);
                                start = ++index;
                                return substring;
                            }
                            else
                            {
                                // Line separator hasn't been found - moves on next character
                                iPtr++;
                                index++;
                            }
                        }
                    }
                }
            }

            // Returns a last substring
            index++;
            return inputString.Substring(start, stringLength - start);
        }
	}
}